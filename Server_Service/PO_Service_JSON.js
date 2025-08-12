// PO_Service_JSON.js - Sử dụng JSON files và request queue thay vì SQLite
const express = require('express');
const bodyParser = require('body-parser');
const cors = require('cors');
const fs = require('fs');
const path = require('path');
const fileManager = require('./fileManager');
const swaggerUi = require('swagger-ui-express');
const swaggerJsdoc = require('swagger-jsdoc');
const sanitizeBodyMiddleware = require('./Mid/sanitizeBodyMiddleware');

const app = express();
const PORT = 49212; // Port khác để test song song

app.use(cors());
app.use(sanitizeBodyMiddleware);

// Swagger setup
const swaggerOptions = {
    definition: {
        openapi: '3.0.0',
        info: {
            title: 'PO Management API (JSON Version)',
            version: '2.0.0',
            description: 'API nhận thông tin PO - JSON File Storage version'
        },
        servers: [{ url: `http://localhost:${PORT}` }]
    },
    apis: [__filename]
};
const swaggerSpec = swaggerJsdoc(swaggerOptions);
app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(swaggerSpec));

// Tạo các thư mục cần thiết
['./logs', './exports', './requests'].forEach(dir => {
    if (!fs.existsSync(dir)) {
        fs.mkdirSync(dir, { recursive: true });
    }
});

// ===== REQUEST QUEUE SYSTEM =====
class RequestQueue {
    constructor() {
        this.queue = [];
        this.processing = false;
        this.maxQueueSize = 1000;
        this.stats = {
            processed: 0,
            errors: 0,
            queued: 0,
            startTime: Date.now()
        };
    }

    async addRequest(requestHandler) {
        return new Promise((resolve, reject) => {
            if (this.queue.length >= this.maxQueueSize) {
                reject(new Error('Request queue is full'));
                return;
            }

            const request = {
                id: Date.now() + Math.random(),
                handler: requestHandler,
                resolve,
                reject,
                timestamp: new Date().toISOString(),
                timeout: setTimeout(() => {
                    reject(new Error('Request timeout'));
                }, 60000) // 60 second timeout
            };

            this.queue.push(request);
            this.stats.queued++;
            
            console.log(`📝 Request ${request.id} added to queue. Queue length: ${this.queue.length}`);
            
            // Start processing if not already running
            this.processQueue();
        });
    }

    async processQueue() {
        if (this.processing || this.queue.length === 0) {
            return;
        }

        this.processing = true;
        console.log(`🔄 Starting queue processing. Queue length: ${this.queue.length}`);

        while (this.queue.length > 0) {
            const request = this.queue.shift();
            
            try {
                clearTimeout(request.timeout);
                console.log(`⚡ Processing request ${request.id}`);
                
                const result = await request.handler();
                request.resolve(result);
                this.stats.processed++;
                
                console.log(`✅ Request ${request.id} completed successfully`);
                
            } catch (error) {
                console.log(`❌ Request ${request.id} failed:`, error.message);
                request.reject(error);
                this.stats.errors++;
            }
            
            // Small delay between requests để tránh overwhelm
            await new Promise(resolve => setTimeout(resolve, 10));
        }

        this.processing = false;
        console.log(`🏁 Queue processing completed`);
    }

    getStats() {
        return {
            ...this.stats,
            queueLength: this.queue.length,
            processing: this.processing,
            uptime: Date.now() - this.stats.startTime
        };
    }
}

const requestQueue = new RequestQueue();

// ===== LOGGING FUNCTIONS =====
function logError(error, context = '') {
    try {
        const timestamp = new Date().toISOString();
        const logMessage = `[${timestamp}] ERROR: ${context ? context + ' - ' : ''}${error.message || error}\n${error.stack || ''}\n${'='.repeat(80)}\n`;
        
        const logFileName = `error_${new Date().toISOString().split('T')[0]}.txt`;
        const logPath = path.join('./logs', logFileName);
        
        fs.appendFileSync(logPath, logMessage);
        console.error(`[${timestamp}] ERROR logged:`, error.message);
    } catch (logErr) {
        console.error('Failed to write error log:', logErr);
    }
}

function saveRequestToFile(requestData, responseData = null) {
    try {
        const timestamp = new Date().toISOString();
        const date = timestamp.split('T')[0];
        
        const fileName = `requests_${date}.txt`;
        const filePath = path.join('./requests', fileName);
        
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
                sampleCodes: Array.isArray(requestData.uniqueCode) ? 
                    requestData.uniqueCode.slice(0, 3).map(code => code.replace(/\x1D/g, '<GS>')) : []
            }
        };
        
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
        
        const logEntry = `[${timestamp}] POST /api/orders - ${requestData.orderNo}/${requestData.blockNo}
${JSON.stringify(requestInfo, null, 2)}
${'='.repeat(100)}

`;
        
        fs.appendFileSync(filePath, logEntry);
        console.log(`📝 Request logged to ${fileName}`);
        
    } catch (error) {
        logError(error, 'Save request to file error');
    }
}

function saveUniqueCodesToFile(orderNo, blockNo, uniqueCodes) {
    try {
        const timestamp = new Date().toISOString();
        const date = timestamp.split('T')[0];
        const time = timestamp.split('T')[1].split('.')[0].replace(/:/g, '-');
        
        const safeOrderNo = orderNo.replace(/[^a-zA-Z0-9_\-]/g, '_');
        const safeBlockNo = blockNo.replace(/[^a-zA-Z0-9_\-]/g, '_');
        const fileName = `codes_${safeOrderNo}_${safeBlockNo}_${date}_${time}.txt`;
        const filePath = path.join('./requests', fileName);
        
        const header = `# UniqueCodes Backup
# OrderNo: ${orderNo}
# BlockNo: ${blockNo}
# Timestamp: ${timestamp}
# TotalCodes: ${uniqueCodes.length}
# Format: Each line is one uniqueCode

`;
        
        const processedCodes = uniqueCodes.map(code => 
            code.replace(/\x1D/g, '<GS>')
        ).join('\n');
        
        const content = header + processedCodes + '\n';
        
        fs.writeFileSync(filePath, content, 'utf8');
        console.log(`📦 Codes backed up to ${fileName}`);
        
        return fileName;
        
    } catch (error) {
        logError(error, `Save uniqueCodes to file error for ${orderNo}/${blockNo}`);
        return null;
    }
}

// ===== API ENDPOINTS =====

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

        // Validation
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
                        message: `Phần tử uniqueCode tại vị trí ${i} không hợp lệ.`,
                        at: new Date().toISOString()
                    });
                }
            }
        }

        // Add to queue for processing
        const result = await requestQueue.addRequest(async () => {
            return await processOrderRequest({
                orderNo, uniqueCode, blockNo, site, factory, productionLine,
                productionDate, shift, orderQty, lotNumber,
                productCode, productName, GTIN, customerOrderNo, uom
            });
        });

        res.json(result);

    } catch (error) {
        logError(error, `POST /api/orders error for orderNo: ${req.body.orderNo}`);
        res.status(500).json({
            message: error.message.includes('queue') ? 'Hệ thống đang quá tải, vui lòng thử lại.' : 'Lỗi xử lý request.',
            at: new Date().toISOString()
        });
    }
});

async function processOrderRequest(requestData) {
    const {
        orderNo, uniqueCode, blockNo, site, factory, productionLine,
        productionDate, shift, orderQty, lotNumber,
        productCode, productName, GTIN, customerOrderNo, uom
    } = requestData;

    const now = new Date().toISOString();
    
    try {
        // Log request
        saveRequestToFile(requestData);
        
        // Backup codes
        const backupFileName = saveUniqueCodesToFile(orderNo, blockNo, uniqueCode);

        // Save/Update PO Info
        const poData = {
            orderNo, site, factory, productionLine, productionDate, shift,
            orderQty, lotNumber, productCode, productName, GTIN,
            customerOrderNo, uom
        };
        
        const existingPO = await fileManager.getPOInfo(orderNo);
        const savedPO = await fileManager.savePOInfo(poData);
        
        // Log action
        const logData = {
            orderNo,
            action: existingPO ? 'update' : 'insert',
            detail: JSON.stringify({
                orderNo, site, factory, productionLine,
                productionDate, shift, orderQty, lotNumber,
                productCode, productName, GTIN, customerOrderNo, uom,
                blockNo, codeCount: uniqueCode.length
            })
        };
        
        await fileManager.savePOLog(logData);

        // Process codes - chuyển {GS} thành ký tự GS thật
        const processedCodes = uniqueCode.map(code => 
            code.replace(/\<GS\>/g, String.fromCharCode(29))
        );

        // Save codes và get thống kê
        const codeResult = await fileManager.saveUniqueCodes(orderNo, blockNo, processedCodes);
        
        // Final result
        const result = {
            orderNo, site, factory, productionLine,
            productionDate, shift, orderQty, lotNumber,
            productCode, productName, GTIN, customerOrderNo, uom,
            uniqueCode: {
                insertedCount: codeResult.insertedCount,
                duplicateCount: codeResult.duplicateCount,
                totalExistingCount: codeResult.totalExistingCount
            },
            blockNo,
            receiveQty: codeResult.blockCount,
            httpStatus: 200,
            message: `Đã xử lý PO ${orderNo} thành công.`,
            storage: 'JSON',
            queueStats: requestQueue.getStats()
        };

        if (backupFileName) {
            result.backupInfo = {
                codesBackupFile: backupFileName,
                backupLocation: './requests/'
            };
        }

        // Log response
        saveRequestToFile(requestData, result);
        
        return result;

    } catch (error) {
        logError(error, `Process order request error for orderNo: ${orderNo}`);
        throw error;
    }
}

/**
 * @swagger  
 * /api/orders:
 *   get:
 *     summary: Lấy danh sách PO mới nhất (JSON Storage)
 *     parameters:
 *       - in: query
 *         name: limit
 *         schema: { type: integer }
 *         description: Số lượng PO muốn lấy (default 100)
 *     responses:
 *       200: { description: Thành công }
 */
app.get('/api/orders', async (req, res) => {
    try {
        const limit = parseInt(req.query.limit) || 100;
        const orders = await fileManager.getAllPOInfo(limit);
        
        res.json({ 
            count: orders.length, 
            data: orders,
            storage: 'JSON',
            message: 'Data loaded from JSON files'
        });
    } catch (err) {
        logError(err, 'Error reading PO list from JSON');
        res.status(500).json({ message: 'Lỗi đọc PO từ JSON files.' });
    }
});

/**
 * @swagger
 * /api/orders/logs:
 *   get:
 *     summary: Lấy log lịch sử PO từ JSON files
 *     parameters:
 *       - in: query
 *         name: orderNo
 *         schema: { type: string }
 *       - in: query  
 *         name: limit
 *         schema: { type: integer }
 *     responses:
 *       200: { description: Thành công }
 */
app.get('/api/orders/logs', async (req, res) => {
    try {
        const { orderNo, limit } = req.query;
        const filters = {};
        
        if (orderNo) {
            filters.orderNo = orderNo;
        }
        
        const logs = await fileManager.getPOLogs(filters, parseInt(limit) || 100);
        
        res.json({ 
            count: logs.length, 
            data: logs,
            storage: 'JSON'
        });
    } catch (err) {
        logError(err, 'Error reading logs from JSON');
        res.status(500).json({ message: 'Lỗi đọc log từ JSON files.' });
    }
});

/**
 * @swagger
 * /api/orders/{orderNo}/codes:
 *   get:
 *     summary: Lấy danh sách uniqueCode của PO từ JSON
 *     parameters:
 *       - in: path
 *         name: orderNo
 *         required: true
 *         schema: { type: string }
 *       - in: query
 *         name: blockNo
 *         schema: { type: string }
 *         description: Lọc theo blockNo
 *     responses:
 *       200: { description: Thành công }
 */
app.get('/api/orders/:orderNo/codes', async (req, res) => {
    try {
        const { orderNo } = req.params;
        const { blockNo } = req.query;
        
        if (!fileManager.codesFileExists(orderNo)) {
            return res.status(404).json({ message: 'Không tìm thấy codes của PO này.' });
        }

        const codes = await fileManager.getUniqueCodes(orderNo, blockNo);
        
        res.json({ 
            count: codes.length, 
            data: codes,
            orderNo,
            blockNo: blockNo || 'all',
            storage: 'JSON'
        });

    } catch (error) {
        logError(error, `GET codes error for orderNo: ${req.params.orderNo}`);
        res.status(500).json({ message: 'Lỗi đọc mã từ JSON files.' });
    }
});

/**
 * @swagger
 * /api/queue/stats:
 *   get:
 *     summary: Thống kê request queue
 *     responses:
 *       200: { description: Queue statistics }
 */
app.get('/api/queue/stats', (req, res) => {
    try {
        const stats = requestQueue.getStats();
        const storageStats = fileManager.getStorageStats();
        
        res.json({
            queue: stats,
            storage: storageStats,
            server: {
                port: PORT,
                uptime: process.uptime(),
                memory: process.memoryUsage()
            }
        });
    } catch (error) {
        logError(error, 'Queue stats error');
        res.status(500).json({ message: 'Lỗi lấy thống kê.' });
    }
});

// Error handling middleware
app.use((err, req, res, next) => {
    logError(err, `Unhandled error at ${req.method} ${req.url}`);
    res.status(500).json({
        message: 'Đã xảy ra lỗi server.',
        at: new Date().toISOString()
    });
});

// Start server
const server = app.listen(PORT, () => {
    console.log(`🚀 PO Service (JSON Version) chạy tại http://localhost:${PORT}`);
    console.log(`🛠️ Swagger UI: http://localhost:${PORT}/api-docs`);
    console.log(`📁 Storage: JSON Files (No SQLite)`);
    console.log(`⚡ Queue: Enabled (Sequential Processing)`);
});

server.setTimeout(10 * 60 * 1000);

// Graceful shutdown
process.on('SIGTERM', () => {
    console.log('📤 Graceful shutdown...');
    server.close(() => {
        console.log('✅ Server closed');
        process.exit(0);
    });
});

// Error handlers
process.on('uncaughtException', (err) => {
    logError(err, 'Uncaught Exception');
    console.error('Uncaught Exception:', err);
    process.exit(1);
});

process.on('unhandledRejection', (reason, promise) => {
    logError(new Error(reason), 'Unhandled Rejection');
    console.error('Unhandled Rejection at:', promise, 'reason:', reason);
});

module.exports = app;