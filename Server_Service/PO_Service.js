const express = require('express');
const bodyParser = require('body-parser');
const cors = require('cors');
const fs = require('fs');
const sqlite3 = require('sqlite3').verbose();
const poDB = require('./db_po');
const swaggerUi = require('swagger-ui-express');
const swaggerJsdoc = require('swagger-jsdoc');

const app = express();
const PORT = 49212;

app.use(cors());
app.use(bodyParser.json());

// Swagger setup
const swaggerOptions = {
    definition: {
        openapi: '3.0.0',
        info: {
            title: 'PO Management API',
            version: '1.0.0',
            description: 'API quản lý PO, uniqueCode và lịch sử phục vụ hệ thống sản xuất'
        },
        servers: [{ url: `http://localhost:${PORT}` }]
    },
    apis: [__filename]
};
const swaggerSpec = swaggerJsdoc(swaggerOptions);
app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(swaggerSpec));

// Tạo thư mục codes nếu chưa có
if (!fs.existsSync('./codes')) {
    fs.mkdirSync('./codes');
}

// Mở DB riêng theo PO
function getCodeDB(orderNo) {
    const safeOrderNo = orderNo.replace(/[^a-zA-Z0-9_\-]/g, '_');
    const dbPath = `./codes/${safeOrderNo}.db`;
    const codeDB = new sqlite3.Database(dbPath);
    codeDB.serialize(() => {
        codeDB.run(`
            CREATE TABLE IF NOT EXISTS UniqueCodes (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                code TEXT,
                createdAt TEXT
            )
        `);
    });
    return codeDB;
}

/**
 * @swagger
 * /api/orders:
 *   post:
 *     summary: Tạo hoặc cập nhật PO, lưu mã uniqueCode chống trùng
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             required:
 *               - orderNo
 *               - uniqueCode
 *               - site
 *               - factory
 *               - productionLine
 *               - productionDate
 *               - shift
 *               - orderQty
 *               - lotNumber
 *               - productCode
 *               - productName
 *               - GTIN
 *               - customerOrderNo
 *             properties:
 *               orderNo:
 *                 type: string
 *                 example: "PO_001"
 *               uniqueCode:
 *                 type: array
 *                 items:
 *                   type: string
 *                 example: ["CODE001", "CODE002", "CODE003"]
 *               site:
 *                 type: string
 *                 example: "SITE_X"
 *               factory:
 *                 type: string
 *                 example: "FACTORY_Y"
 *               productionLine:
 *                 type: string
 *                 example: "LINE_1"
 *               productionDate:
 *                 type: string
 *                 example: "2025-07-02"
 *               shift:
 *                 type: string
 *                 example: "A"
 *               orderQty:
 *                 type: number
 *                 example: 1000
 *               lotNumber:
 *                 type: string
 *                 example: "LOT_123"
 *               productCode:
 *                 type: string
 *                 example: "PROD_XYZ"
 *               productName:
 *                 type: string
 *                 example: "Sản phẩm A"
 *               GTIN:
 *                 type: string
 *                 example: "8931234567890"
 *               customerOrderNo:
 *                 type: string
 *                 example: "CUST_PO_999"
 *     responses:
 *       200:
 *         description: Thành công
 *       400:
 *         description: Thiếu dữ liệu
 */
app.post('/api/orders', (req, res) => {
    const {
        orderNo, uniqueCode, site, factory, productionLine,
        productionDate, shift, orderQty, lotNumber,
        productCode, productName, GTIN, customerOrderNo
    } = req.body;

    const requiredFields = {
        orderNo, uniqueCode, site, factory, productionLine,
        productionDate, shift, orderQty, lotNumber,
        productCode, productName, GTIN, customerOrderNo
    };

    for (const [key, value] of Object.entries(requiredFields)) {
        if (
            value === undefined ||
            value === null ||
            (typeof value === 'string' && value.trim() === '') ||
            (key === 'uniqueCode' && (!Array.isArray(value) || value.length === 0))
        ) {
            return res.status(400).json({
                message: `Trường '${key}' đang thiếu hoặc không hợp lệ.`
            });
        }
    }

    if (Array.isArray(requiredFields.uniqueCode)) {
        for (let i = 0; i < requiredFields.uniqueCode.length; i++) {
            const code = requiredFields.uniqueCode[i];
            if (typeof code !== 'string' || code.trim() === '') {
                return res.status(400).json({
                    message: `Phần tử uniqueCode tại vị trí ${i} không hợp lệ (cần string không rỗng).`
                });
            }
        }
    }

    const now = new Date().toISOString();

    poDB.get(`SELECT id FROM POInfo WHERE orderNo = ?`, [orderNo], (err, row) => {
        if (err) return res.status(500).json({ message: 'Lỗi DB.' });

        const data = [site, factory, productionLine, productionDate, shift,
            orderQty, lotNumber, productCode, productName, GTIN,
            customerOrderNo, now, orderNo];

        function insertLog(action) {
            const logData = {
                orderNo, site, factory, productionLine,
                productionDate, shift, orderQty, lotNumber,
                productCode, productName, GTIN, customerOrderNo,
                codeCount: uniqueCode.length
            };
            poDB.run(
                `INSERT INTO POLogs (orderNo, action, detail, createdAt) VALUES (?, ?, ?, ?)`,
                [orderNo, action, JSON.stringify(logData), now]
            );
        }

        function insertCodes() {
            const codeDB = getCodeDB(orderNo);
            codeDB.all(`SELECT code FROM UniqueCodes`, [], (err, rows) => {
                if (err) {
                    codeDB.close();
                    return res.status(500).json({ message: 'Lỗi đọc mã từ DB.' });
                }
                const existingCodesSet = new Set(rows.map(r => r.code));
                const newCodes = uniqueCode.filter(code => !existingCodesSet.has(code));
                const duplicateCount = uniqueCode.length - newCodes.length;
                const stmt = codeDB.prepare(`INSERT INTO UniqueCodes (code, createdAt) VALUES (?, ?)`);
                newCodes.forEach(code => stmt.run([code, now]));
                stmt.finalize(() => {
                    codeDB.get(`SELECT COUNT(*) AS count FROM UniqueCodes`, [], (err, row) => {
                        const existingCount = row ? row.count : 0;
                        codeDB.close();
                        res.json({
                            orderNo, site, factory, productionLine,
                            productionDate, shift, orderQty, lotNumber,
                            productCode, productName, GTIN, customerOrderNo,
                            uniqueCode: {
                                insertedCount: newCodes.length,
                                duplicateCount,
                                existingCount
                            },
                            httpStatus: 200,
                            message: `Đã xử lý PO ${orderNo} thành công.`
                        });
                    });
                });
            });
        }

        if (row) {
            poDB.run(`
                UPDATE POInfo SET site=?, factory=?, productionLine=?, productionDate=?, shift=?,
                orderQty=?, lotNumber=?, productCode=?, productName=?, GTIN=?,
                customerOrderNo=?, lastUpdated=? WHERE orderNo=?
            `, data, err => {
                if (err) return res.status(500).json({ message: 'Lỗi update PO.' });
                insertLog('update');
                insertCodes();
            });
        } else {
            poDB.run(`
                INSERT INTO POInfo (site, factory, productionLine, productionDate, shift,
                orderQty, lotNumber, productCode, productName, GTIN,
                customerOrderNo, lastUpdated, orderNo)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
            `, [site, factory, productionLine, productionDate, shift,
                orderQty, lotNumber, productCode, productName, GTIN,
                customerOrderNo, now, orderNo], err => {
                    if (err) return res.status(500).json({ message: 'Lỗi insert PO.' });
                    insertLog('insert');
                    insertCodes();
                });
        }
    });
});

/**
 * @swagger
 * /api/orders:
 *   get:
 *     summary: Lấy danh sách PO mới nhất
 *     responses:
 *       200:
 *         description: Thành công
 */
app.get('/api/orders', (req, res) => {
    poDB.all(`SELECT * FROM POInfo ORDER BY lastUpdated DESC LIMIT 100`, [], (err, rows) => {
        if (err) return res.status(500).json({ message: 'Lỗi đọc PO.' });
        res.json({ count: rows.length, data: rows });
    });
});

/**
 * @swagger
 * /api/orders/logs:
 *   get:
 *     summary: Lấy log lịch sử PO với filter
 *     parameters:
 *       - in: query
 *         name: filters
 *         schema: { type: string }
 *         description: JSON filter, ví dụ {"orderNo":"PO_001"}
 *       - in: query
 *         name: numberlog
 *         schema: { type: integer }
 *         description: Số lượng log muốn lấy (default 100)
 *     responses:
 *       200:
 *         description: Thành công
 */
app.get('/api/orders/logs', (req, res) => {
    const { filters, numberlog } = req.query;
    let query = `SELECT id, orderNo, action, createdAt, detail FROM POLogs`;
    let conditions = [];
    let params = [];

    if (filters) {
        try {
            const parsedFilters = JSON.parse(filters);
            for (const key in parsedFilters) {
                if ([
                    'orderNo', 'site', 'factory', 'productionLine',
                    'productionDate', 'shift', 'lotNumber',
                    'productCode', 'productName', 'GTIN', 'customerOrderNo'
                ].includes(key)) {
                    conditions.push(`json_extract(detail, '$.${key}') = ?`);
                    params.push(parsedFilters[key]);
                }
            }
        } catch {
            return res.status(400).json({ message: 'Lỗi filters, cần dạng JSON.' });
        }
    }

    if (conditions.length > 0) {
        query += ` WHERE ` + conditions.join(' AND ');
    }
    query += ` ORDER BY createdAt DESC LIMIT ?`;
    params.push(parseInt(numberlog) || 100);

    poDB.all(query, params, (err, rows) => {
        if (err) return res.status(500).json({ message: 'Lỗi đọc log.' });
        const data = rows.map(r => ({
            id: r.id,
            orderNo: r.orderNo,
            action: r.action,
            createdAt: r.createdAt,
            detail: JSON.parse(r.detail)
        }));
        res.json({ count: data.length, data });
    });
});

/**
 * @swagger
 * /api/orders/{orderNo}/codes:
 *   get:
 *     summary: Lấy danh sách uniqueCode của PO
 *     parameters:
 *       - in: path
 *         name: orderNo
 *         required: true
 *         schema: { type: string }
 *     responses:
 *       200:
 *         description: Thành công
 */
app.get('/api/orders/:orderNo/codes', (req, res) => {
    const { orderNo } = req.params;
    const safeOrderNo = orderNo.replace(/[^a-zA-Z0-9_\-]/g, '_');
    const dbPath = `./codes/${safeOrderNo}.db`;

    if (!fs.existsSync(dbPath)) {
        return res.status(404).json({ message: 'Không tìm thấy DB của PO này.' });
    }

    const codeDB = new sqlite3.Database(dbPath);
    codeDB.all(`SELECT * FROM UniqueCodes ORDER BY createdAt ASC`, [], (err, rows) => {
        codeDB.close();
        if (err) return res.status(500).json({ message: 'Lỗi đọc mã.' });
        res.json({ count: rows.length, data: rows });
    });
});

const server = app.listen(PORT, () => {
    console.log(`🚀 Server chạy tại http://localhost:${PORT}`);
    console.log(`🛠️ Swagger UI: http://localhost:${PORT}/api-docs`);
});
server.setTimeout(10 * 60 * 1000);
