const express = require('express');
const bodyParser = require('body-parser');
const cors = require('cors');
const fs = require('fs');
const path = require('path');
const sqlite3 = require('sqlite3').verbose();
const poDB = require('./db_po');
const codeDBManager = require('./codeDbManager');
const swaggerUi = require('swagger-ui-express');
const swaggerJsdoc = require('swagger-jsdoc');
const sanitizeBodyMiddleware = require('./Mid/sanitizeBodyMiddleware');

const app = express();
const PORT = 49212;

app.use(cors());
//app.use(bodyParser.json());

app.use(sanitizeBodyMiddleware);

// Swagger setup
const swaggerOptions = {
    definition: {
        openapi: '3.0.0',
        info: {
            title: 'PO Management API',
            version: '1.0.1',
            description: 'API nhận thông tin PO cho máy kích hoạt mã 2D'
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

// Tạo thư mục logs nếu chưa có
if (!fs.existsSync('./logs')) {
    fs.mkdirSync('./logs');
}

// Tạo thư mục exports nếu chưa có
if (!fs.existsSync('./exports')) {
    fs.mkdirSync('./exports');
}

// Tạo thư mục requests nếu chưa có
if (!fs.existsSync('./requests')) {
    fs.mkdirSync('./requests');
}

// Hàm ghi log lỗi vào file txt
function logError(error, context = '') {
    try {
        const timestamp = new Date().toISOString();
        const logMessage = `[${timestamp}] ERROR: ${context ? context + ' - ' : ''}${error.message || error}\n${error.stack || ''}\n${'='.repeat(80)}\n`;
        
        const logFileName = `error_${new Date().toISOString().split('T')[0]}.txt`;
        const logPath = path.join('./logs', logFileName);
        
        fs.appendFileSync(logPath, logMessage);
        console.error(`[${timestamp}] ERROR logged to ${logPath}:`, error);
    } catch (logErr) {
        console.error('Failed to write error log:', logErr);
    }
}

// Hàm lưu request vào file txt
function saveRequestToFile(requestData, responseData = null) {
    try {
        const timestamp = new Date().toISOString();
        const date = timestamp.split('T')[0];
        
        // Tạo tên file theo ngày
        const fileName = `requests_${date}.txt`;
        const filePath = path.join('./requests', fileName);
        
        // Tạo nội dung request
        const requestInfo = {
            timestamp,
            method: 'POST',
            url: '/api/orders',
            orderNo: requestData.orderNo,
            blockNo: requestData.blockNo,
            codeCount: Array.isArray(requestData.uniqueCode) ? requestData.uniqueCode.length : 0,
            requestBody: {
                orderNo: requestData.orderNo,
                blockNo: requestData.blockNo,
                site: requestData.site,
                factory: requestData.factory,
                productionLine: requestData.productionLine,
                productionDate: requestData.productionDate,
                shift: requestData.shift,
                orderQty: requestData.orderQty,
                lotNumber: requestData.lotNumber,
                productCode: requestData.productCode,
                productName: requestData.productName,
                GTIN: requestData.GTIN,
                customerOrderNo: requestData.customerOrderNo,
                uom: requestData.uom,
                uniqueCodeCount: Array.isArray(requestData.uniqueCode) ? requestData.uniqueCode.length : 0,
                // Lưu sample codes (first 3) để reference
                sampleCodes: Array.isArray(requestData.uniqueCode) ? 
                    requestData.uniqueCode.slice(0, 3).map(code => code.replace(/\x1D/g, '<GS>')) : []
            }
        };
        
        // Thêm response info nếu có
        if (responseData) {
            requestInfo.response = {
                httpStatus: responseData.httpStatus,
                message: responseData.message,
                insertedCount: responseData.uniqueCode ? responseData.uniqueCode.insertedCount : 0,
                duplicateCount: responseData.uniqueCode ? responseData.uniqueCode.duplicateCount : 0,
                totalExistingCount: responseData.uniqueCode ? responseData.uniqueCode.totalExistingCount : 0,
                receiveQty: responseData.receiveQty
            };
        }
        
        // Format nội dung
        const logEntry = `[${timestamp}] POST /api/orders - ${requestData.orderNo}/${requestData.blockNo}
${JSON.stringify(requestInfo, null, 2)}
${'='.repeat(100)}

`;
        
        // Append vào file
        fs.appendFileSync(filePath, logEntry);
        
        console.log(`📝 Request saved to ${filePath}`);
        
    } catch (error) {
        logError(error, 'Save request to file error');
    }
}

// Hàm lưu uniqueCodes vào file riêng biệt (backup)
function saveUniqueCodesToFile(orderNo, blockNo, uniqueCodes) {
    try {
        const timestamp = new Date().toISOString();
        const date = timestamp.split('T')[0];
        const time = timestamp.split('T')[1].split('.')[0].replace(/:/g, '-');
        
        // Tạo tên file: codes_OrderNo_BlockNo_Date_Time.txt
        const safeOrderNo = orderNo.replace(/[^a-zA-Z0-9_\-]/g, '_');
        const safeBlockNo = blockNo.replace(/[^a-zA-Z0-9_\-]/g, '_');
        const fileName = `codes_${safeOrderNo}_${safeBlockNo}_${date}_${time}.txt`;
        const filePath = path.join('./requests', fileName);
        
        // Tạo header
        const header = `# UniqueCodes Backup
# OrderNo: ${orderNo}
# BlockNo: ${blockNo}
# Timestamp: ${timestamp}
# TotalCodes: ${uniqueCodes.length}
# Format: Each line is one uniqueCode

`;
        
        // Process codes (chuyển GS character thành <GS> để dễ đọc)
        const processedCodes = uniqueCodes.map(code => 
            code.replace(/\x1D/g, '<GS>')
        ).join('\n');
        
        const content = header + processedCodes + '\n';
        
        // Ghi file
        fs.writeFileSync(filePath, content, 'utf8');
        
        console.log(`📦 UniqueCodes backed up to ${filePath}`);
        
        return fileName;
        
    } catch (error) {
        logError(error, `Save uniqueCodes to file error for ${orderNo}/${blockNo}`);
        return null;
    }
}

// Request queue để serialize các write operations
const requestQueue = [];
let isProcessingQueue = false;

async function processQueue() {
    if (isProcessingQueue || requestQueue.length === 0) {
        return;
    }
    
    isProcessingQueue = true;
    
    while (requestQueue.length > 0) {
        const { operation, resolve, reject } = requestQueue.shift();
        
        try {
            const result = await operation();
            resolve(result);
        } catch (error) {
            reject(error);
        }
    }
    
    isProcessingQueue = false;
}

function queueOperation(operation) {
    return new Promise((resolve, reject) => {
        requestQueue.push({ operation, resolve, reject });
        processQueue().catch(reject);
    });
}

/**
 * @swagger
 * /api/orders:
 *   post:
 *     summary: Tạo hoặc cập nhật PO, khóa chính là orderNO.
 *              Lưu uniqueCode theo blockNo trùng sẽ tự động không lưu.
 *              Trả về số lượng mã mới, trùng và tổng số mã hiện có.    
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             required:
 *               - orderNo
 *               - uniqueCode
 *               - blockNo
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
 *               - uom
 *             properties:
 *               orderNo: { type: string, example: "PO_001" }
 *               uniqueCode: { type: array, items: { type: string }, example: ["CODE001","CODE002"] }
 *               blockNo: { type: string, example: "BLOCK_001" }
 *               site: { type: string, example: "SITE_X" }
 *               factory: { type: string, example: "FACTORY_Y" }
 *               productionLine: { type: string, example: "LINE_1" }
 *               productionDate: { type: string, example: "2025-07-02" }
 *               shift: { type: string, example: "A" }
 *               orderQty: { type: number, example: 1000 }
 *               lotNumber: { type: string, example: "LOT_123" }
 *               productCode: { type: string, example: "PROD_XYZ" }
 *               productName: { type: string, example: "Sản phẩm A" }
 *               GTIN: { type: string, example: "8931234567890" }
 *               customerOrderNo: { type: string, example: "CUST_PO_999" }
 *               uom: { type: string, example: "PCS" }
 *     responses:
 *       500: { description: Lỗi máy chủ hoặc DB }
 *       200: { description: Thành công }
 *       400: { description: Thiếu dữ liệu }
 */
app.post('/api/orders', async (req, res) => {
    try {
        const {
            orderNo, uniqueCode, blockNo, site, factory, productionLine,
            productionDate, shift, orderQty, lotNumber,
            productCode, productName, GTIN, customerOrderNo, uom
        } = req.body;

        const requiredFields = {
            orderNo, uniqueCode, blockNo, site, factory, productionLine,
            productionDate, shift, orderQty, lotNumber,
            productCode, productName, GTIN, customerOrderNo, uom
        };

        for (const [key, value] of Object.entries(requiredFields)) {
            if (
                value === undefined || value === null ||
                (typeof value === 'string' && value.trim() === '') ||
                (key === 'uniqueCode' && (!Array.isArray(value) || value.length === 0))
            ) {
                return res.status(400).json({
                    message: `Trường '${key}' đang thiếu hoặc không hợp lệ.`,
                    at: new Date().toISOString()
                });
            }
        }

        if (Array.isArray(uniqueCode)) {
            for (let i = 0; i < uniqueCode.length; i++) {
                const code = uniqueCode[i];
                if (typeof code !== 'string' || code.trim() === '') {
                    return res.status(400).json({
                        message: `Phần tử uniqueCode tại vị trí ${i} không hợp lệ (cần string không rỗng).`,
                        at: new Date().toISOString()
                    });
                }
            }
        }

        const now = new Date().toISOString();

        // 🔥 LƯU REQUEST VÀO FILE TXT TRƯỚC KHI XỬ LÝ
        const requestData = {
            orderNo, uniqueCode, blockNo, site, factory, productionLine,
            productionDate, shift, orderQty, lotNumber,
            productCode, productName, GTIN, customerOrderNo, uom
        };
        
        // Lưu request log
        saveRequestToFile(requestData);
        
        // Backup uniqueCodes ra file riêng
        const backupFileName = saveUniqueCodesToFile(orderNo, blockNo, uniqueCode);

        // Queue the write operation to prevent database locking
        const result = await queueOperation(async () => {
            return await processOrderRequest({
                orderNo, uniqueCode, blockNo, site, factory, productionLine,
                productionDate, shift, orderQty, lotNumber,
                productCode, productName, GTIN, customerOrderNo, uom, now
            });
        });

        // 🔥 LƯU RESPONSE VÀO FILE TXT SAU KHI XỬ LÝ
        saveRequestToFile(requestData, result);

        // Thêm thông tin backup vào response
        if (backupFileName) {
            result.backupInfo = {
                codesBackupFile: backupFileName,
                backupLocation: './requests/'
            };
        }

        res.json(result);

    } catch (error) {
        logError(error, `POST /api/orders error for orderNo: ${req.body.orderNo}`);
        res.status(500).json({
            message: 'Lỗi xử lý request.',
            at: new Date().toISOString()
        });
    }
});

async function processOrderRequest({
    orderNo, uniqueCode, blockNo, site, factory, productionLine,
    productionDate, shift, orderQty, lotNumber,
    productCode, productName, GTIN, customerOrderNo, uom, now
}) {
    try {
        // Check if PO exists
        const existingPO = await poDB.get(`SELECT id FROM POInfo WHERE orderNo = ?`, [orderNo]);

        const data = [
            site, factory, productionLine, productionDate, shift,
            orderQty, lotNumber, productCode, productName, GTIN,
            customerOrderNo, uom, now, orderNo
        ];

        // Insert or update PO info
        if (existingPO) {
            await poDB.run(`
                UPDATE POInfo SET site=?, factory=?, productionLine=?, productionDate=?, shift=?,
                orderQty=?, lotNumber=?, productCode=?, productName=?, GTIN=?,
                customerOrderNo=?, uom=?, lastUpdated=? WHERE orderNo=?
            `, data);
        } else {
            await poDB.run(`
                INSERT INTO POInfo (site, factory, productionLine, productionDate, shift,
                orderQty, lotNumber, productCode, productName, GTIN,
                customerOrderNo, uom, lastUpdated, orderNo)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
            `, data);
        }

        // Log the action
        const logData = {
            orderNo, site, factory, productionLine,
            productionDate, shift, orderQty, lotNumber,
            productCode, productName, GTIN, customerOrderNo, uom,
            blockNo, codeCount: uniqueCode.length
        };

        await poDB.run(
            `INSERT INTO POLogs (orderNo, action, detail, createdAt) VALUES (?, ?, ?, ?)`,
            [orderNo, existingPO ? 'update' : 'insert', JSON.stringify(logData), now]
        );

        // Process codes with optimized batch insert
        const processedCodes = uniqueCode.map(code => 
            code.replace(/\<GS\>/g, String.fromCharCode(29))
        );

        // Get existing codes to check for duplicates
        const existingRows = await codeDBManager.getAllCodes(orderNo);
        const existingCodesSet = new Set(existingRows.map(r => r.code));
        const newCodes = processedCodes.filter(code => !existingCodesSet.has(code));
        const duplicateCount = processedCodes.length - newCodes.length;

        // Batch insert new codes
        let insertedCount = 0;
        if (newCodes.length > 0) {
            const result = await codeDBManager.batchInsertCodes(orderNo, newCodes, blockNo, now);
            insertedCount = result.insertedCount;
        }

        // Get counts
        const receiveQty = await codeDBManager.getCodesCount(orderNo, blockNo);
        const totalExistingCount = await codeDBManager.getCodesCount(orderNo);

        return {
            orderNo, site, factory, productionLine,
            productionDate, shift, orderQty, lotNumber,
            productCode, productName, GTIN, customerOrderNo, uom,
            uniqueCode: {
                insertedCount,
                duplicateCount,
                totalExistingCount
            },
            blockNo,
            receiveQty,
            httpStatus: 200,
            message: `Đã xử lý PO ${orderNo} thành công.`
        };

    } catch (error) {
        logError(error, `Process order request error for orderNo: ${orderNo}`);
        throw error;
    }
}

// Hàm export uniqueCode ra file txt theo blockNo
async function exportBlockToFile(orderNo, blockNo) {
    try {
        const connection = await codeDBManager.getConnection(orderNo);
        
        return new Promise((resolve, reject) => {
            const timeout = setTimeout(() => {
                codeDBManager.releaseConnection(orderNo, connection);
                reject(new Error('Export timeout'));
            }, 30000);

            connection.all(
                `SELECT code, createdAt FROM UniqueCodes WHERE blockNo = ? ORDER BY createdAt ASC`, 
                [blockNo], 
                (err, rows) => {
                    clearTimeout(timeout);
                    codeDBManager.releaseConnection(orderNo, connection);
                    
                    if (err) {
                        reject(err);
                        return;
                    }

                    if (rows.length === 0) {
                        reject(new Error('Không tìm thấy codes cho blockNo này'));
                        return;
                    }

                    // Tạo nội dung file
                    const timestamp = new Date().toISOString();
                    const header = `# Export UniqueCodes\n# OrderNo: ${orderNo}\n# BlockNo: ${blockNo}\n# ExportTime: ${timestamp}\n# TotalCodes: ${rows.length}\n\n`;
                    
                    const codes = rows.map(row => {
                        // Chuyển GS character về {GS} để dễ đọc
                        return row.code.replace(/\x1D/g, '<GS>');
                    }).join('\n');

                    const content = header + codes;

                    // Tạo tên file
                    const safeOrderNo = orderNo.replace(/[^a-zA-Z0-9_\-]/g, '_');
                    const safeBlockNo = blockNo.replace(/[^a-zA-Z0-9_\-]/g, '_');
                    const fileName = `${safeOrderNo}_${safeBlockNo}_${timestamp.split('T')[0]}.txt`;
                    const filePath = path.join('./exports', fileName);

                    // Ghi file
                    fs.writeFileSync(filePath, content, 'utf8');
                    
                    resolve({
                        fileName,
                        filePath,
                        totalCodes: rows.length,
                        exportTime: timestamp
                    });
                }
            );
        });

    } catch (error) {
        logError(error, `Export error for orderNo: ${orderNo}, blockNo: ${blockNo}`);
        throw error;
    }
}

// Hàm import uniqueCode từ file txt
async function importBlockFromFile(orderNo, blockNo, filePath, replaceExisting = false) {
    try {
        if (!fs.existsSync(filePath)) {
            throw new Error('File không tồn tại');
        }

        const content = fs.readFileSync(filePath, 'utf8');
        const lines = content.split('\n');
        
        // Bỏ qua các dòng comment và dòng trống
        const codes = lines
            .filter(line => !line.startsWith('#') && line.trim() !== '')
            .map(line => {
                // Chuyển {GS} về GS character
                return line.trim().replace(/<GS>/g, String.fromCharCode(29));
            })
            .filter(code => code.length > 0);

        if (codes.length === 0) {
            throw new Error('File không chứa codes hợp lệ');
        }

        const now = new Date().toISOString();
        
        // Nếu replaceExisting = true, xóa codes cũ của blockNo này
        if (replaceExisting) {
            const connection = await codeDBManager.getConnection(orderNo);
            
            await new Promise((resolve, reject) => {
                const timeout = setTimeout(() => {
                    codeDBManager.releaseConnection(orderNo, connection);
                    reject(new Error('Delete timeout'));
                }, 30000);

                connection.run(
                    `DELETE FROM UniqueCodes WHERE blockNo = ?`, 
                    [blockNo], 
                    (err) => {
                        clearTimeout(timeout);
                        codeDBManager.releaseConnection(orderNo, connection);
                        if (err) reject(err);
                        else resolve();
                    }
                );
            });
        }

        // Import codes mới
        const result = await codeDBManager.batchInsertCodes(orderNo, codes, blockNo, now);
        
        return {
            fileName: path.basename(filePath),
            importedCodes: result.insertedCount,
            totalCodesInFile: codes.length,
            duplicateCount: codes.length - result.insertedCount,
            blockNo,
            replaceExisting,
            importTime: now
        };

    } catch (error) {
        logError(error, `Import error for orderNo: ${orderNo}, file: ${filePath}`);
        throw error;
    }
}

/**
 * @swagger
 * /api/orders:
 *   get:
 *     summary: Lấy danh sách PO mới nhất
 *     responses:
 *       200: { description: Thành công }
 */
app.get('/api/orders', async (req, res) => {
    try {
        const rows = await poDB.all(`SELECT * FROM POInfo ORDER BY lastUpdated DESC LIMIT 100`, []);
        res.json({ count: rows.length, data: rows });
    } catch (err) {
        logError(err, 'Error reading PO list');
        res.status(500).json({ message: 'Lỗi đọc PO.' });
    }
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
 *       200: { description: Thành công }
 */
app.get('/api/orders/logs', async (req, res) => {
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

    try {
        const rows = await poDB.all(query, params);
        const data = rows.map(r => ({
            id: r.id,
            orderNo: r.orderNo,
            action: r.action,
            createdAt: r.createdAt,
            detail: JSON.parse(r.detail)
        }));
        res.json({ count: data.length, data });
    } catch (err) {
        logError(err, 'Error reading logs');
        res.status(500).json({ message: 'Lỗi đọc log.' });
    }
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
 *       200: { description: Thành công }
 */
app.get('/api/orders/:orderNo/codes', async (req, res) => {
    try {
        const { orderNo } = req.params;
        
        if (!codeDBManager.dbExists(orderNo)) {
            return res.status(404).json({ message: 'Không tìm thấy DB của PO này.' });
        }

        const connection = await codeDBManager.getConnection(orderNo);
        
        const timeout = setTimeout(() => {
            codeDBManager.releaseConnection(orderNo, connection);
            res.status(500).json({ message: 'Timeout reading codes.' });
        }, 30000);

        connection.all(`SELECT * FROM UniqueCodes ORDER BY createdAt ASC`, [], (err, rows) => {
            clearTimeout(timeout);
            codeDBManager.releaseConnection(orderNo, connection);
            
            if (err) {
                logError(err, `Error reading codes for orderNo: ${orderNo}`);
                return res.status(500).json({ message: 'Lỗi đọc mã.' });
            }
            
            res.json({ count: rows.length, data: rows });
        });

    } catch (error) {
        logError(error, `GET codes error for orderNo: ${req.params.orderNo}`);
        res.status(500).json({ message: 'Lỗi đọc mã.' });
    }
});

/**
 * @swagger
 * /api/orders/{orderNo}/blocks/{blockNo}/export:
 *   get:
 *     summary: Export uniqueCode của một blockNo ra file txt
 *     parameters:
 *       - in: path
 *         name: orderNo
 *         required: true
 *         schema: { type: string }
 *       - in: path
 *         name: blockNo
 *         required: true
 *         schema: { type: string }
 *     responses:
 *       200: 
 *         description: Export thành công
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 fileName: { type: string }
 *                 totalCodes: { type: number }
 *                 downloadUrl: { type: string }
 *       404: { description: Không tìm thấy PO hoặc block }
 */
app.get('/api/orders/:orderNo/blocks/:blockNo/export', async (req, res) => {
    try {
        const { orderNo, blockNo } = req.params;
        
        if (!codeDBManager.dbExists(orderNo)) {
            return res.status(404).json({ message: 'Không tìm thấy PO này.' });
        }

        const result = await exportBlockToFile(orderNo, blockNo);
        
        res.json({
            fileName: result.fileName,
            totalCodes: result.totalCodes,
            exportTime: result.exportTime,
            downloadUrl: `/api/exports/${result.fileName}`,
            message: `Đã export ${result.totalCodes} codes của block ${blockNo} thành công.`
        });

    } catch (error) {
        logError(error, `Export API error for ${req.params.orderNo}/${req.params.blockNo}`);
        
        if (error.message.includes('Không tìm thấy codes')) {
            res.status(404).json({ message: error.message });
        } else {
            res.status(500).json({ message: 'Lỗi export block.' });
        }
    }
});

/**
 * @swagger
 * /api/orders/{orderNo}/blocks/{blockNo}/import:
 *   post:
 *     summary: Import uniqueCode từ file txt vào một blockNo
 *     parameters:
 *       - in: path
 *         name: orderNo
 *         required: true
 *         schema: { type: string }
 *       - in: path
 *         name: blockNo
 *         required: true
 *         schema: { type: string }
 *       - in: query
 *         name: replaceExisting
 *         schema: { type: boolean }
 *         description: Có thay thế codes hiện có không (default false)
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             required: [filePath]
 *             properties:
 *               filePath: 
 *                 type: string
 *                 description: Đường dẫn đến file txt (relative to exports folder)
 *                 example: "PO_001_BLOCK_001_2025-08-12.txt"
 *     responses:
 *       200: { description: Import thành công }
 */
app.post('/api/orders/:orderNo/blocks/:blockNo/import', async (req, res) => {
    try {
        const { orderNo, blockNo } = req.params;
        const { filePath: inputFilePath } = req.body;
        const replaceExisting = req.query.replaceExisting === 'true';

        if (!inputFilePath) {
            return res.status(400).json({ message: 'Thiếu filePath.' });
        }

        // Xác định đường dẫn file
        let fullFilePath;
        if (path.isAbsolute(inputFilePath)) {
            fullFilePath = inputFilePath;
        } else {
            fullFilePath = path.join('./exports', inputFilePath);
        }

        const result = await importBlockFromFile(orderNo, blockNo, fullFilePath, replaceExisting);
        
        res.json({
            fileName: result.fileName,
            importedCodes: result.importedCodes,
            totalCodesInFile: result.totalCodesInFile,
            duplicateCount: result.duplicateCount,
            blockNo: result.blockNo,
            replaceExisting: result.replaceExisting,
            importTime: result.importTime,
            message: `Đã import ${result.importedCodes} codes mới vào block ${blockNo}.`
        });

    } catch (error) {
        logError(error, `Import API error for ${req.params.orderNo}/${req.params.blockNo}`);
        
        if (error.message.includes('không tồn tại') || error.message.includes('không chứa codes')) {
            res.status(400).json({ message: error.message });
        } else {
            res.status(500).json({ message: 'Lỗi import block.' });
        }
    }
});

/**
 * @swagger
 * /api/exports/{fileName}:
 *   get:
 *     summary: Download file export
 *     parameters:
 *       - in: path
 *         name: fileName
 *         required: true
 *         schema: { type: string }
 *     responses:
 *       200: { description: File download }
 */
app.get('/api/exports/:fileName', (req, res) => {
    try {
        const { fileName } = req.params;
        const filePath = path.join('./exports', fileName);
        
        if (!fs.existsSync(filePath)) {
            return res.status(404).json({ message: 'File không tồn tại.' });
        }

        res.download(filePath, fileName, (err) => {
            if (err) {
                logError(err, `Download error for file: ${fileName}`);
                if (!res.headersSent) {
                    res.status(500).json({ message: 'Lỗi download file.' });
                }
            }
        });

    } catch (error) {
        logError(error, `Download API error for file: ${req.params.fileName}`);
        res.status(500).json({ message: 'Lỗi download file.' });
    }
});

/**
 * @swagger
 * /api/exports:
 *   get:
 *     summary: Lấy danh sách file exports
 *     responses:
 *       200: { description: Danh sách files }
 */
app.get('/api/exports', (req, res) => {
    try {
        const files = fs.readdirSync('./exports')
            .filter(file => file.endsWith('.txt'))
            .map(file => {
                const filePath = path.join('./exports', file);
                const stats = fs.statSync(filePath);
                return {
                    fileName: file,
                    size: stats.size,
                    createdTime: stats.birthtime,
                    modifiedTime: stats.mtime,
                    downloadUrl: `/api/exports/${file}`
                };
            })
            .sort((a, b) => b.modifiedTime - a.modifiedTime);

        res.json({
            count: files.length,
            files: files
        });

    } catch (error) {
        logError(error, 'List exports error');
        res.status(500).json({ message: 'Lỗi đọc danh sách exports.' });
    }
});

/**
 * @swagger
 * /api/requests:
 *   get:
 *     summary: Lấy danh sách file requests đã lưu
 *     parameters:
 *       - in: query
 *         name: date
 *         schema: { type: string }
 *         description: Lọc theo ngày (YYYY-MM-DD)
 *       - in: query
 *         name: type
 *         schema: { type: string, enum: [requests, codes] }
 *         description: Loại file (requests hoặc codes backup)
 *     responses:
 *       200: { description: Danh sách files }
 */
app.get('/api/requests', (req, res) => {
    try {
        const { date, type } = req.query;
        
        let files = fs.readdirSync('./requests')
            .filter(file => file.endsWith('.txt'));
        
        // Lọc theo type
        if (type === 'requests') {
            files = files.filter(file => file.startsWith('requests_'));
        } else if (type === 'codes') {
            files = files.filter(file => file.startsWith('codes_'));
        }
        
        // Lọc theo date
        if (date) {
            files = files.filter(file => file.includes(date));
        }
        
        const fileInfos = files.map(file => {
            const filePath = path.join('./requests', file);
            const stats = fs.statSync(filePath);
            
            let fileType = 'unknown';
            if (file.startsWith('requests_')) fileType = 'requests_log';
            if (file.startsWith('codes_')) fileType = 'codes_backup';
            
            return {
                fileName: file,
                fileType: fileType,
                size: stats.size,
                createdTime: stats.birthtime,
                modifiedTime: stats.mtime,
                downloadUrl: `/api/requests/${file}`
            };
        }).sort((a, b) => b.modifiedTime - a.modifiedTime);

        res.json({
            count: fileInfos.length,
            files: fileInfos,
            totalSize: fileInfos.reduce((sum, file) => sum + file.size, 0)
        });

    } catch (error) {
        logError(error, 'List request files error');
        res.status(500).json({ message: 'Lỗi đọc danh sách request files.' });
    }
});

/**
 * @swagger
 * /api/requests/{fileName}:
 *   get:
 *     summary: Download hoặc xem nội dung file request
 *     parameters:
 *       - in: path
 *         name: fileName
 *         required: true
 *         schema: { type: string }
 *       - in: query
 *         name: view
 *         schema: { type: boolean }
 *         description: true để xem nội dung, false để download
 *     responses:
 *       200: { description: File content hoặc download }
 */
app.get('/api/requests/:fileName', (req, res) => {
    try {
        const { fileName } = req.params;
        const { view } = req.query;
        const filePath = path.join('./requests', fileName);
        
        if (!fs.existsSync(filePath)) {
            return res.status(404).json({ message: 'File không tồn tại.' });
        }

        if (view === 'true') {
            // Xem nội dung file
            const content = fs.readFileSync(filePath, 'utf8');
            const lines = content.split('\n');
            
            res.json({
                fileName: fileName,
                totalLines: lines.length,
                content: content,
                preview: lines.slice(0, 50), // First 50 lines
                fileSize: fs.statSync(filePath).size
            });
        } else {
            // Download file
            res.download(filePath, fileName, (err) => {
                if (err) {
                    logError(err, `Download request file error: ${fileName}`);
                    if (!res.headersSent) {
                        res.status(500).json({ message: 'Lỗi download file.' });
                    }
                }
            });
        }

    } catch (error) {
        logError(error, `Request file API error: ${req.params.fileName}`);
        res.status(500).json({ message: 'Lỗi truy cập file.' });
    }
});

/**
 * @swagger
 * /api/requests-stats:
 *   get:
 *     summary: Thống kê các request files
 *     responses:
 *       200: { description: Thống kê }
 */
app.get('/api/requests-stats', (req, res) => {
    try {
        const files = fs.readdirSync('./requests')
            .filter(file => file.endsWith('.txt'));
        
        const requestFiles = files.filter(file => file.startsWith('requests_'));
        const codeFiles = files.filter(file => file.startsWith('codes_'));
        
        const totalSize = files.reduce((sum, file) => {
            const filePath = path.join('./requests', file);
            return sum + fs.statSync(filePath).size;
        }, 0);
        
        // Thống kê theo ngày
        const statsByDate = {};
        files.forEach(file => {
            let date = 'unknown';
            
            if (file.startsWith('requests_')) {
                const match = file.match(/requests_(\d{4}-\d{2}-\d{2})\.txt/);
                if (match) date = match[1];
            } else if (file.startsWith('codes_')) {
                const match = file.match(/codes_.*_(\d{4}-\d{2}-\d{2})_/);
                if (match) date = match[1];
            }
            
            if (!statsByDate[date]) {
                statsByDate[date] = { requestFiles: 0, codeFiles: 0, totalFiles: 0 };
            }
            
            if (file.startsWith('requests_')) statsByDate[date].requestFiles++;
            if (file.startsWith('codes_')) statsByDate[date].codeFiles++;
            statsByDate[date].totalFiles++;
        });

        res.json({
            totalFiles: files.length,
            requestFiles: requestFiles.length,
            codeFiles: codeFiles.length,
            totalSizeBytes: totalSize,
            totalSizeMB: Math.round(totalSize / (1024 * 1024) * 100) / 100,
            statsByDate: statsByDate,
            oldestFile: files.length > 0 ? Math.min(...files.map(f => {
                const stats = fs.statSync(path.join('./requests', f));
                return stats.birthtime.getTime();
            })) : null,
            newestFile: files.length > 0 ? Math.max(...files.map(f => {
                const stats = fs.statSync(path.join('./requests', f));
                return stats.birthtime.getTime();
            })) : null
        });

    } catch (error) {
        logError(error, 'Request stats error');
        res.status(500).json({ message: 'Lỗi thống kê request files.' });
    }
});

// Middleware xử lý lỗi toàn cục
app.use((err, req, res, next) => {
    logError(err, `Unhandled error at ${req.method} ${req.url}`);
    res.status(500).json({
        message: 'Đã xảy ra lỗi server.',
        at: new Date().toISOString()
    });
});

const server = app.listen(PORT, () => {
    console.log(`🚀 Server chạy tại http://localhost:${PORT}`);
    console.log(`🛠️ Swagger UI: http://localhost:${PORT}/api-docs`);
});
server.setTimeout(10 * 60 * 1000);

// Xử lý lỗi uncaught exception
process.on('uncaughtException', (err) => {
    logError(err, 'Uncaught Exception');
    console.error('Uncaught Exception:', err);
    process.exit(1);
});

// Xử lý lỗi unhandled rejection
process.on('unhandledRejection', (reason, promise) => {
    logError(new Error(reason), 'Unhandled Rejection');
    console.error('Unhandled Rejection at:', promise, 'reason:', reason);
});
