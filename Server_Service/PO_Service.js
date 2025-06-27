const express = require('express');
const multer = require('multer');
const cors = require('cors');
const path = require('path');
const fs = require('fs');
const db = require('./db');
const readline = require('readline');
const crypto = require('crypto');

const app = express();
const PORT = 49212;

app.use(cors());
app.use(express.json());
app.use('/uploads', express.static(path.join(__dirname, 'uploads')));

// Đảm bảo thư mục uploads tồn tại
const uploadDir = path.join(__dirname, 'uploads');
if (!fs.existsSync(uploadDir)) {
    fs.mkdirSync(uploadDir);
}

// multer cấu hình lưu file
const storage = multer.diskStorage({
    destination: (req, file, cb) => cb(null, 'uploads/'),
    filename: (req, file, cb) => {
        const filePath = path.join(uploadDir, file.originalname);
        if (fs.existsSync(filePath)) {
            // Nếu file đã tồn tại, không lưu
            return cb(new Error('File đã tồn tại trên server, không được upload lại'), file.originalname);
        } else {
            cb(null, file.originalname); // Giữ nguyên tên file gốc
        }
    }
});
const upload = multer({ storage });

// Hàm ghi log vào DB
function logToDB(status, orderNo, uniqueCode, czFile, message) {
    const query = `INSERT INTO po_logs (status, orderNo, uniqueCode, czFile, message) VALUES (?, ?, ?, ?, ?)`;
    db.run(query, [status, orderNo, uniqueCode, czFile, message]);
}

// Đọc dòng đầu tiên của file
function readFirstLine(filePath) {
    return new Promise((resolve, reject) => {
        const rl = readline.createInterface({
            input: fs.createReadStream(filePath),
            crlfDelay: Infinity
        });
        rl.on('line', (line) => {
            rl.close();
            resolve(line.trim());
        });
        rl.on('error', reject);
    });
}

// Hàm tính hash SHA256 của file
function getFileHash(filePath) {
    return new Promise((resolve, reject) => {
        const hash = crypto.createHash('sha256');
        const stream = fs.createReadStream(filePath);
        stream.on('data', chunk => hash.update(chunk));
        stream.on('end', () => resolve(hash.digest('hex')));
        stream.on('error', reject);
    });
}

// Hàm phân tích mã GS1
function parseGS1(line) {
    const data = {};
    let i = 0;

    while (i < line.length) {
        if (line.startsWith('01', i)) {
            data.gtin = line.substr(i + 2, 14);
            i += 16;
        } else if (line.startsWith('21', i)) {
            i += 2;
            let serial = '';
            while (i < line.length && line.charCodeAt(i) !== 29) {
                serial += line[i++];
            }
            data.serial = serial;
            if (line.charCodeAt(i) === 29) i++;
        } else if (line.startsWith('93', i)) {
            i += 2;
            let internal = '';
            while (i < line.length && line.charCodeAt(i) !== 29) {
                internal += line[i++];
            }
            data.internal = internal;
            if (line.charCodeAt(i) === 29) i++;
        } else {
            i++;
        }
    }

    return data;
}

// ✅ API nhận PO + file
app.post('/api/po', upload.single('czFile'), async (req, res) => {
    if (err instanceof multer.MulterError || err) {
        // Xử lý lỗi từ multer
        return res.status(400).json({ error: err.message });
    }
    const {
        orderNo,
        uniqueCode,
        site,
        factory,
        productionLine,
        productionDate,
        shift
    } = req.body;

    const czFileName = req.file?.originalname || '';
    const czFilePath = req.file?.path;
    const fileUrl = req.file ? `${req.file.filename}` : null;

    if (!czFilePath) {
        return res.status(400).json({ error: 'Không có file đính kèm' });
    }

    // Kiểm tra file đã tồn tại (theo tên)
    const checkFileQuery = `SELECT COUNT(*) AS count FROM po_records WHERE czFileUrl LIKE ?`;
    db.get(checkFileQuery, [`%${czFileName}%`], async (err, row) => {
        if (err) {
            logToDB('ERROR', orderNo, uniqueCode, czFileName, 'Lỗi kiểm tra file: ' + err.message);
            return res.status(500).json({ error: 'Lỗi kiểm tra file tồn tại' });
        }

        if (row.count > 0) {
            logToDB('DUPLICATE_FILE', orderNo, uniqueCode, czFileName, 'File đã được upload trước');
            return res.status(400).json({ error: 'File đã tồn tại trong hệ thống' });
        }

        // Đọc dòng đầu tiên
        let firstLine = '';
        try {
            firstLine = await readFirstLine(czFilePath);
        } catch (err) {
            logToDB('ERROR', orderNo, uniqueCode, czFileName, 'Lỗi đọc file: ' + err.message);
            return res.status(500).json({ error: 'Không thể đọc file đính kèm' });
        }

        // Tính hash
        let fileHash = '';
        try {
            fileHash = await getFileHash(czFilePath);
        } catch (err) {
            logToDB('ERROR', orderNo, uniqueCode, czFileName, 'Lỗi tính hash file: ' + err.message);
            return res.status(500).json({ error: 'Không thể tính hash của file' });
        }

        // Kiểm tra định dạng GS1
        if (!firstLine.match(/01\d{14}/)) {
            logToDB('INVALID_FORMAT', orderNo, uniqueCode, czFileName, 'Không có mã GS1 hợp lệ');
            return res.status(400).json({ error: 'Dòng đầu không hợp lệ (không có mã GTIN bắt đầu bằng 01)' });
        }

        const gs1 = parseGS1(firstLine);

        // Kiểm tra trùng PO
        const checkQuery = `SELECT COUNT(*) AS count FROM po_records WHERE orderNo = ? AND uniqueCode = ?`;
        db.get(checkQuery, [orderNo, uniqueCode], (err, row) => {
            if (err) {
                logToDB('ERROR', orderNo, uniqueCode, czFileName, 'Lỗi kiểm tra trùng: ' + err.message);
                return res.status(500).json({ error: 'Lỗi kiểm tra trùng mã PO' });
            }

            if (row.count > 0) {
                logToDB('DUPLICATE', orderNo, uniqueCode, czFileName, 'PO đã tồn tại');
                return res.status(400).json({ message: 'PO đã tồn tại (trùng orderNo + uniqueCode)' });
            }

            // Lưu PO vào DB
            const insertQuery = `
                INSERT INTO po_records (orderNo, uniqueCode, site, factory, productionLine, productionDate, shift, czFileUrl)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?)
            `;
            db.run(insertQuery, [orderNo, uniqueCode, site, factory, productionLine, productionDate, shift, fileUrl], function (err) {
                if (err) {
                    logToDB('ERROR', orderNo, uniqueCode, czFileName, 'Lỗi insert DB: ' + err.message);
                    return res.status(500).json({ error: 'Lỗi khi lưu PO vào DB' });
                }

                logToDB('SUCCESS', orderNo, uniqueCode, czFileName, `PO ID=${this.lastID} đã lưu`);
                res.json({
                    message: 'Đã nhận PO và lưu vào SQLite thành công!',
                    data: {
                        id: this.lastID,
                        orderNo,
                        uniqueCode,
                        site,
                        factory,
                        productionLine,
                        productionDate,
                        shift,
                        czFileUrl: fileUrl,
                        hash: fileHash,
                        gs1
                    }
                });
            });
        });
    });
});

// ✅ API xem log
app.get('/api/po/logs', (req, res) => {
    db.all(`SELECT * FROM po_logs ORDER BY id DESC`, (err, rows) => {
        if (err) {
            return res.status(500).json({ error: 'Lỗi khi truy vấn log' });
        }
        res.json(rows);
    });
});

app.listen(PORT, () => {
    console.log(`✅ Server đang chạy tại http://localhost:${PORT}`);
});
