const express = require('express');
const sqlite3 = require('sqlite3').verbose();
const multer = require('multer');
const fs = require('fs-extra');
const path = require('path');
const crypto = require('crypto');
const createCsvWriter = require('csv-writer').createObjectCsvWriter;

const app = express();
const PORT = process.env.PORT || 3000;

// Middleware
app.use(express.static('public'));
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Tạo thư mục tạm thời để lưu các file copy
const TEMP_DIR = path.join(__dirname, 'temp');
fs.ensureDirSync(TEMP_DIR);

// Cấu hình multer để upload file
const storage = multer.diskStorage({
    destination: (req, file, cb) => {
        cb(null, TEMP_DIR);
    },
    filename: (req, file, cb) => {
        // Tạo tên file unique để tránh conflict
        const uniqueSuffix = crypto.randomBytes(16).toString('hex');
        cb(null, `${uniqueSuffix}_${file.originalname}`);
    }
});

const upload = multer({
    storage: storage,
    fileFilter: (req, file, cb) => {
        // Chỉ chấp nhận file SQLite
        if (file.mimetype === 'application/x-sqlite3' ||
            file.originalname.toLowerCase().endsWith('.sqlite') ||
            file.originalname.toLowerCase().endsWith('.db') ||
            file.originalname.toLowerCase().endsWith('.sqlite3')) {
            cb(null, true);
        } else {
            cb(new Error('Chỉ chấp nhận file SQLite (.sqlite, .db, .sqlite3)'));
        }
    }
});

// Route chính
app.get('/', (req, res) => {
    res.sendFile(path.join(__dirname, 'public', 'index.html'));
});

// Route upload và đọc file SQLite
app.post('/upload-sqlite', upload.single('sqliteFile'), async (req, res) => {
    try {
        if (!req.file) {
            return res.status(400).json({ error: 'Không có file được chọn' });
        }

        const filePath = req.file.path;
        const fileId = path.basename(filePath, path.extname(filePath));

        console.log(`File được upload: ${req.file.originalname}`);
        console.log(`Đường dẫn tạm thời: ${filePath}`);

        // Đọc thông tin cơ bản về database
        const dbInfo = await analyzeSQLiteFile(filePath);

        res.json({
            success: true,
            fileId: fileId,
            originalName: req.file.originalname,
            size: req.file.size,
            dbInfo: dbInfo
        });

    } catch (error) {
        console.error('Lỗi khi xử lý file:', error);
        res.status(500).json({ error: error.message });
    }
});

// Route để lấy dữ liệu từ bảng cụ thể với filter
app.get('/table/:fileId/:tableName', async (req, res) => {
    try {
        const { fileId, tableName } = req.params;
        const limit = parseInt(req.query.limit) || 100;
        const offset = parseInt(req.query.offset) || 0;
        const filter = req.query.filter || '';
        const sortColumn = req.query.sortColumn || '';
        const sortOrder = req.query.sortOrder || 'ASC';

        const filePath = findFileById(fileId);
        if (!filePath) {
            return res.status(404).json({ error: 'File không tồn tại' });
        }

        const data = await getTableData(filePath, tableName, limit, offset, filter, sortColumn, sortOrder);
        res.json(data);

    } catch (error) {
        console.error('Lỗi khi đọc dữ liệu bảng:', error);
        res.status(500).json({ error: error.message });
    }
});

// Route để xuất dữ liệu CSV
app.get('/export/:fileId/:tableName/csv', async (req, res) => {
    try {
        const { fileId, tableName } = req.params;
        const filter = req.query.filter || '';
        const sortColumn = req.query.sortColumn || '';
        const sortOrder = req.query.sortOrder || 'ASC';

        const filePath = findFileById(fileId);
        if (!filePath) {
            return res.status(404).json({ error: 'File không tồn tại' });
        }

        const data = await getAllTableData(filePath, tableName, filter, sortColumn, sortOrder);

        if (data.length === 0) {
            return res.status(404).json({ error: 'Không có dữ liệu để xuất' });
        }

        // Tạo file CSV tạm thời
        const csvFileName = `${tableName}_${Date.now()}.csv`;
        const csvFilePath = path.join(TEMP_DIR, csvFileName);

        const columns = Object.keys(data[0]);
        const csvWriter = createCsvWriter({
            path: csvFilePath,
            header: columns.map(col => ({ id: col, title: col }))
        });

        await csvWriter.writeRecords(data);

        // Gửi file CSV về client
        res.download(csvFilePath, `${tableName}.csv`, (err) => {
            if (err) {
                console.error('Lỗi khi gửi file CSV:', err);
            }
            // Xóa file tạm thời sau khi gửi
            fs.remove(csvFilePath).catch(console.error);
        });

    } catch (error) {
        console.error('Lỗi khi xuất CSV:', error);
        res.status(500).json({ error: error.message });
    }
});

// Route để xuất dữ liệu JSON
app.get('/export/:fileId/:tableName/json', async (req, res) => {
    try {
        const { fileId, tableName } = req.params;
        const filter = req.query.filter || '';
        const sortColumn = req.query.sortColumn || '';
        const sortOrder = req.query.sortOrder || 'ASC';

        const filePath = findFileById(fileId);
        if (!filePath) {
            return res.status(404).json({ error: 'File không tồn tại' });
        }

        const data = await getAllTableData(filePath, tableName, filter, sortColumn, sortOrder);

        res.setHeader('Content-Type', 'application/json');
        res.setHeader('Content-Disposition', `attachment; filename="${tableName}.json"`);
        res.json(data);

    } catch (error) {
        console.error('Lỗi khi xuất JSON:', error);
        res.status(500).json({ error: error.message });
    }
});

// Route để lấy schema của bảng
app.get('/schema/:fileId/:tableName', async (req, res) => {
    try {
        const { fileId, tableName } = req.params;

        const filePath = findFileById(fileId);
        if (!filePath) {
            return res.status(404).json({ error: 'File không tồn tại' });
        }

        const schema = await getTableSchema(filePath, tableName);
        res.json(schema);

    } catch (error) {
        console.error('Lỗi khi đọc schema:', error);
        res.status(500).json({ error: error.message });
    }
});

// Route để thực hiện custom SQL query
app.post('/query/:fileId', async (req, res) => {
    try {
        const { fileId } = req.params;
        const { sql } = req.body;

        const filePath = findFileById(fileId);
        if (!filePath) {
            return res.status(404).json({ error: 'File không tồn tại' });
        }

        // Chỉ cho phép SELECT queries để đảm bảo an toàn
        if (!sql.trim().toLowerCase().startsWith('select')) {
            return res.status(400).json({ error: 'Chỉ được phép thực hiện SELECT queries' });
        }

        const result = await executeQuery(filePath, sql);
        res.json(result);

    } catch (error) {
        console.error('Lỗi khi thực hiện query:', error);
        res.status(500).json({ error: error.message });
    }
});

// Route để xóa file tạm thời
app.delete('/cleanup/:fileId', async (req, res) => {
    try {
        const { fileId } = req.params;
        const filePath = findFileById(fileId);

        if (filePath && fs.existsSync(filePath)) {
            await fs.remove(filePath);
            console.log(`Đã xóa file tạm thời: ${filePath}`);
        }

        res.json({ success: true });
    } catch (error) {
        console.error('Lỗi khi xóa file:', error);
        res.status(500).json({ error: error.message });
    }
});

// Các hàm tiện ích
function findFileById(fileId) {
    const files = fs.readdirSync(TEMP_DIR);
    const targetFile = files.find(file => file.startsWith(fileId));
    return targetFile ? path.join(TEMP_DIR, targetFile) : null;
}

function analyzeSQLiteFile(filePath) {
    return new Promise((resolve, reject) => {
        const db = new sqlite3.Database(filePath, sqlite3.OPEN_READONLY, (err) => {
            if (err) {
                reject(err);
                return;
            }
        });

        // Lấy danh sách các bảng
        db.all(`SELECT name, type FROM sqlite_master WHERE type IN ('table', 'view') ORDER BY name`, (err, tables) => {
            if (err) {
                db.close();
                reject(err);
                return;
            }

            // Lấy thông tin về từng bảng
            const tablePromises = tables.map(table => {
                return new Promise((resolveTable, rejectTable) => {
                    db.get(`SELECT COUNT(*) as count FROM "${table.name}"`, (err, result) => {
                        if (err) {
                            rejectTable(err);
                            return;
                        }
                        resolveTable({
                            name: table.name,
                            type: table.type,
                            rowCount: result.count
                        });
                    });
                });
            });

            Promise.all(tablePromises)
                .then(tablesInfo => {
                    db.close();
                    resolve({
                        tables: tablesInfo,
                        totalTables: tablesInfo.filter(t => t.type === 'table').length,
                        totalViews: tablesInfo.filter(t => t.type === 'view').length
                    });
                })
                .catch(err => {
                    db.close();
                    reject(err);
                });
        });
    });
}

function getTableData(filePath, tableName, limit = 100, offset = 0, filter = '', sortColumn = '', sortOrder = 'ASC') {
    return new Promise((resolve, reject) => {
        const db = new sqlite3.Database(filePath, sqlite3.OPEN_READONLY);

        let sql = `SELECT * FROM "${tableName}"`;
        let countSql = `SELECT COUNT(*) as total FROM "${tableName}"`;
        let params = [];
        let countParams = [];

        // Thêm filter nếu có
        if (filter) {
            // Lấy danh sách cột trước
            db.all(`PRAGMA table_info("${tableName}")`, (err, columns) => {
                if (err) {
                    db.close();
                    reject(err);
                    return;
                }

                const columnNames = columns.map(col => col.name);
                const whereConditions = columnNames.map(col => `"${col}" LIKE ?`).join(' OR ');
                const filterValue = `%${filter}%`;
                const filterParams = columnNames.map(() => filterValue);

                sql += ` WHERE ${whereConditions}`;
                countSql += ` WHERE ${whereConditions}`;
                params = [...filterParams];
                countParams = [...filterParams];

                // Thêm sorting nếu có
                if (sortColumn && columns.some(col => col.name === sortColumn)) {
                    sql += ` ORDER BY "${sortColumn}" ${sortOrder}`;
                }

                sql += ` LIMIT ? OFFSET ?`;
                params.push(limit, offset);

                executeQuery();
            });
        } else {
            // Thêm sorting nếu có
            if (sortColumn) {
                sql += ` ORDER BY "${sortColumn}" ${sortOrder}`;
            }

            sql += ` LIMIT ? OFFSET ?`;
            params = [limit, offset];

            executeQuery();
        }

        function executeQuery() {
            db.all(sql, params, (err, rows) => {
                if (err) {
                    db.close();
                    reject(err);
                    return;
                }

                // Lấy tổng số dòng
                db.get(countSql, countParams, (err, countResult) => {
                    db.close();
                    if (err) {
                        reject(err);
                        return;
                    }

                    resolve({
                        data: rows,
                        total: countResult.total,
                        limit: limit,
                        offset: offset,
                        filter: filter
                    });
                });
            });
        }
    });
}

function getAllTableData(filePath, tableName, filter = '', sortColumn = '', sortOrder = 'ASC') {
    return new Promise((resolve, reject) => {
        const db = new sqlite3.Database(filePath, sqlite3.OPEN_READONLY);

        let sql = `SELECT * FROM "${tableName}"`;
        let params = [];

        // Thêm filter nếu có
        if (filter) {
            db.all(`PRAGMA table_info("${tableName}")`, (err, columns) => {
                if (err) {
                    db.close();
                    reject(err);
                    return;
                }

                const columnNames = columns.map(col => col.name);
                const whereConditions = columnNames.map(col => `"${col}" LIKE ?`).join(' OR ');
                const filterValue = `%${filter}%`;
                params = columnNames.map(() => filterValue);

                sql += ` WHERE ${whereConditions}`;

                // Thêm sorting nếu có
                if (sortColumn && columns.some(col => col.name === sortColumn)) {
                    sql += ` ORDER BY "${sortColumn}" ${sortOrder}`;
                }

                executeQuery();
            });
        } else {
            // Thêm sorting nếu có
            if (sortColumn) {
                sql += ` ORDER BY "${sortColumn}" ${sortOrder}`;
            }

            executeQuery();
        }

        function executeQuery() {
            db.all(sql, params, (err, rows) => {
                db.close();
                if (err) {
                    reject(err);
                    return;
                }
                resolve(rows);
            });
        }
    });
}

function getTableSchema(filePath, tableName) {
    return new Promise((resolve, reject) => {
        const db = new sqlite3.Database(filePath, sqlite3.OPEN_READONLY);

        db.all(`PRAGMA table_info("${tableName}")`, (err, columns) => {
            if (err) {
                db.close();
                reject(err);
                return;
            }

            db.get(`SELECT sql FROM sqlite_master WHERE name = ? AND type = 'table'`, [tableName], (err, createSql) => {
                db.close();
                if (err) {
                    reject(err);
                    return;
                }

                resolve({
                    columns: columns,
                    createSql: createSql ? createSql.sql : null
                });
            });
        });
    });
}

function executeQuery(filePath, sql) {
    return new Promise((resolve, reject) => {
        const db = new sqlite3.Database(filePath, sqlite3.OPEN_READONLY);

        db.all(sql, (err, rows) => {
            db.close();
            if (err) {
                reject(err);
                return;
            }

            resolve({
                data: rows,
                rowCount: rows.length
            });
        });
    });
}

// Dọn dẹp file tạm thời khi tắt server
process.on('SIGINT', async () => {
    console.log('\nĐang dọn dẹp file tạm thời...');
    try {
        await fs.emptyDir(TEMP_DIR);
        console.log('Đã dọn dẹp xong.');
    } catch (error) {
        console.error('Lỗi khi dọn dẹp:', error);
    }
    process.exit(0);
});

// Dọn dẹp file cũ định kỳ (mỗi giờ)
setInterval(async () => {
    try {
        const files = await fs.readdir(TEMP_DIR);
        const now = Date.now();

        for (const file of files) {
            const filePath = path.join(TEMP_DIR, file);
            const stats = await fs.stat(filePath);

            // Xóa file cũ hơn 1 giờ
            if (now - stats.mtime.getTime() > 60 * 60 * 1000) {
                await fs.remove(filePath);
                console.log(`Đã xóa file cũ: ${file}`);
            }
        }
    } catch (error) {
        console.error('Lỗi khi dọn dẹp file cũ:', error);
    }
}, 60 * 60 * 1000); // Chạy mỗi giờ

app.listen(PORT, () => {
    console.log(`Server đang chạy tại http://localhost:${PORT}`);
    console.log(`Thư mục tạm thời: ${TEMP_DIR}`);
});