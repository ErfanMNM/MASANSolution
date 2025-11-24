// fileManager.js - JSON-based file storage to replace SQLite
const fs = require('fs');
const path = require('path');

class FileManager {
    constructor() {
        this.dataDir = './data';
        this.codesDir = './codes_json';
        this.init();
    }

    init() {
        // Tạo thư mục data nếu chưa có
        if (!fs.existsSync(this.dataDir)) {
            fs.mkdirSync(this.dataDir, { recursive: true });
        }
        
        // Tạo thư mục codes_json nếu chưa có  
        if (!fs.existsSync(this.codesDir)) {
            fs.mkdirSync(this.codesDir, { recursive: true });
        }
    }

    // Thread-safe file operations với retry mechanism
    async writeFileWithRetry(filePath, data, maxRetries = 3) {
        for (let attempt = 1; attempt <= maxRetries; attempt++) {
            try {
                await this.writeFileAtomic(filePath, data);
                return true;
            } catch (error) {
                if (attempt === maxRetries) {
                    throw error;
                }
                // Wait và retry
                await new Promise(resolve => setTimeout(resolve, 10 + Math.random() * 40));
            }
        }
    }

    // Atomic write để tránh corruption
    async writeFileAtomic(filePath, data) {
        const tempPath = `${filePath}.tmp`;
        const jsonData = JSON.stringify(data, null, 2);
        
        return new Promise((resolve, reject) => {
            fs.writeFile(tempPath, jsonData, 'utf8', (err) => {
                if (err) {
                    reject(err);
                    return;
                }
                
                // Rename temp file to final file (atomic operation)
                fs.rename(tempPath, filePath, (renameErr) => {
                    if (renameErr) {
                        // Cleanup temp file on error
                        fs.unlink(tempPath, () => {});
                        reject(renameErr);
                        return;
                    }
                    resolve();
                });
            });
        });
    }

    async readFileWithRetry(filePath, defaultValue = null, maxRetries = 3) {
        for (let attempt = 1; attempt <= maxRetries; attempt++) {
            try {
                if (!fs.existsSync(filePath)) {
                    return defaultValue;
                }
                
                const data = fs.readFileSync(filePath, 'utf8');
                return JSON.parse(data);
            } catch (error) {
                if (attempt === maxRetries) {
                    console.error(`Failed to read file after ${maxRetries} attempts:`, error);
                    return defaultValue;
                }
                // Wait và retry
                await new Promise(resolve => setTimeout(resolve, 10 + Math.random() * 40));
            }
        }
    }

    // PO Info Operations
    async savePOInfo(poData) {
        const filePath = path.join(this.dataDir, `${this.sanitizeFileName(poData.orderNo)}.json`);
        const timestamp = new Date().toISOString();
        
        const data = {
            ...poData,
            lastUpdated: timestamp,
            createdAt: poData.createdAt || timestamp
        };
        
        await this.writeFileWithRetry(filePath, data);
        return data;
    }

    async getPOInfo(orderNo) {
        const filePath = path.join(this.dataDir, `PO_${this.sanitizeFileName(orderNo)}.json`);
        return await this.readFileWithRetry(filePath);
    }

    async getAllPOInfo(limit = 100) {
        const files = fs.readdirSync(this.dataDir)
            .filter(file => file.startsWith('PO_') && file.endsWith('.json'))
            .map(file => {
                const filePath = path.join(this.dataDir, file);
                const stats = fs.statSync(filePath);
                return { file, filePath, mtime: stats.mtime };
            })
            .sort((a, b) => b.mtime - a.mtime)
            .slice(0, limit);

        const results = [];
        for (const { filePath } of files) {
            try {
                const data = await this.readFileWithRetry(filePath);
                if (data) results.push(data);
            } catch (error) {
                console.error(`Error reading PO file ${filePath}:`, error);
            }
        }
        
        return results;
    }

    // Codes Operations - Lưu theo GTIN
    async saveUniqueCodes(gtin, blockNo, codes) {
        const safeGTIN = this.sanitizeFileName(gtin);
        const filePath = path.join(this.codesDir, `GTIN_${safeGTIN}.json`);

        // Read existing data
        let existingData = await this.readFileWithRetry(filePath, {
            gtin: gtin,
            blocks: {},
            totalCodes: 0,
            lastUpdated: new Date().toISOString()
        });

        if (!existingData.blocks) {
            existingData.blocks = {};
        }

        const timestamp = new Date().toISOString();
        const processedCodes = codes.map(code => ({
            code: code,
            createdAt: timestamp,
            blockNo: blockNo
        }));

        // Update or create block
        if (!existingData.blocks[blockNo]) {
            existingData.blocks[blockNo] = {
                codes: [],
                createdAt: timestamp,
                lastUpdated: timestamp
            };
        }

        // Get existing codes in this block to check duplicates
        const existingCodes = new Set(existingData.blocks[blockNo].codes.map(c => c.code));
        const newCodes = processedCodes.filter(c => !existingCodes.has(c.code));
        
        // Add new codes
        existingData.blocks[blockNo].codes.push(...newCodes);
        existingData.blocks[blockNo].lastUpdated = timestamp;
        
        // Update totals
        existingData.totalCodes = Object.values(existingData.blocks)
            .reduce((total, block) => total + block.codes.length, 0);
        existingData.lastUpdated = timestamp;

        await this.writeFileWithRetry(filePath, existingData);

        return {
            insertedCount: newCodes.length,
            duplicateCount: codes.length - newCodes.length,
            totalExistingCount: existingData.totalCodes,
            blockCount: existingData.blocks[blockNo].codes.length
        };
    }

    async getUniqueCodes(gtin, blockNo = null) {
        const safeGTIN = this.sanitizeFileName(gtin);
        const filePath = path.join(this.codesDir, `GTIN_${safeGTIN}.json`);
        
        const data = await this.readFileWithRetry(filePath);
        if (!data || !data.blocks) return [];

        if (blockNo) {
            return data.blocks[blockNo] ? data.blocks[blockNo].codes : [];
        }

        // Return all codes from all blocks
        const allCodes = [];
        for (const block of Object.values(data.blocks)) {
            allCodes.push(...block.codes);
        }
        
        return allCodes.sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt));
    }

    async getCodesCount(gtin, blockNo = null) {
        const codes = await this.getUniqueCodes(gtin, blockNo);
        return codes.length;
    }

    async deleteBlockCodes(gtin, blockNo) {
        const safeGTIN = this.sanitizeFileName(gtin);
        const filePath = path.join(this.codesDir, `GTIN_${safeGTIN}.json`);
        
        const data = await this.readFileWithRetry(filePath);
        if (!data || !data.blocks || !data.blocks[blockNo]) {
            return false;
        }

        delete data.blocks[blockNo];
        
        // Update totals
        data.totalCodes = Object.values(data.blocks)
            .reduce((total, block) => total + block.codes.length, 0);
        data.lastUpdated = new Date().toISOString();

        await this.writeFileWithRetry(filePath, data);
        return true;
    }

    // Logs Operations (JSON-based logs)
    async savePOLog(logData) {
        const date = new Date().toISOString().split('T')[0];
        const filePath = path.join(this.dataDir, `logs_${date}.json`);
        
        const timestamp = new Date().toISOString();
        const logEntry = {
            id: Date.now() + Math.random(), // Simple unique ID
            timestamp: timestamp,
            ...logData
        };

        // Read existing logs
        let existingLogs = await this.readFileWithRetry(filePath, { date: date, logs: [] });
        if (!existingLogs.logs) {
            existingLogs = { date: date, logs: [] };
        }

        // Add new log
        existingLogs.logs.push(logEntry);
        
        // Keep only last 1000 logs per day to prevent huge files
        if (existingLogs.logs.length > 1000) {
            existingLogs.logs = existingLogs.logs.slice(-1000);
        }

        await this.writeFileWithRetry(filePath, existingLogs);
        return logEntry;
    }

    async getPOLogs(filters = {}, limit = 100) {
        const logFiles = fs.readdirSync(this.dataDir)
            .filter(file => file.startsWith('logs_') && file.endsWith('.json'))
            .sort((a, b) => b.localeCompare(a)) // Latest first
            .slice(0, 10); // Only check last 10 days

        let allLogs = [];
        
        for (const file of logFiles) {
            const filePath = path.join(this.dataDir, file);
            const data = await this.readFileWithRetry(filePath, { logs: [] });
            
            if (data.logs) {
                allLogs.push(...data.logs);
            }
        }

        // Apply filters
        if (filters.orderNo) {
            allLogs = allLogs.filter(log => log.orderNo === filters.orderNo);
        }

        // Sort by timestamp desc and limit
        allLogs.sort((a, b) => new Date(b.timestamp) - new Date(a.timestamp));
        
        return allLogs.slice(0, limit);
    }

    // Utility functions
    sanitizeFileName(name) {
        return name.replace(/[^a-zA-Z0-9_\-]/g, '_');
    }

    // Check if codes file exists for a GTIN
    codesFileExists(gtin) {
        const safeGTIN = this.sanitizeFileName(gtin);
        const filePath = path.join(this.codesDir, `GTIN_${safeGTIN}.json`);
        return fs.existsSync(filePath);
    }

    // Get all POs by GTIN
    async getPOsByGTIN(gtin, excludeOrderNo = null) {
        const files = fs.readdirSync(this.dataDir)
            .filter(file => file.startsWith('PO_') && file.endsWith('.json'));

        const results = [];
        for (const file of files) {
            const filePath = path.join(this.dataDir, file);
            try {
                const data = await this.readFileWithRetry(filePath);
                if (data && data.GTIN === gtin && data.orderNo !== excludeOrderNo) {
                    results.push(data);
                }
            } catch (error) {
                console.error(`Error reading PO file ${filePath}:`, error);
            }
        }

        return results;
    }

    // Get total orderQty for a GTIN (excluding specific orderNo if provided)
    async getTotalOrderQtyByGTIN(gtin, excludeOrderNo = null) {
        const pos = await this.getPOsByGTIN(gtin, excludeOrderNo);
        return pos.reduce((total, po) => {
            const qty = parseInt(po.orderQty) || 0;
            return total + qty;
        }, 0);
    }

    // Get storage stats
    async getStorageStats() {
        const poFiles = fs.readdirSync(this.dataDir)
            .filter(file => file.startsWith('PO_') && file.endsWith('.json'));

        const codeFiles = fs.readdirSync(this.codesDir)
            .filter(file => file.endsWith('.json'));

        const logFiles = fs.readdirSync(this.dataDir)
            .filter(file => file.startsWith('logs_') && file.endsWith('.json'));

        let totalSize = 0;
        let totalFiles = poFiles.length + codeFiles.length + logFiles.length;

        // Calculate total size
        [...poFiles.map(f => path.join(this.dataDir, f)),
         ...codeFiles.map(f => path.join(this.codesDir, f)),
         ...logFiles.map(f => path.join(this.dataDir, f))
        ].forEach(filePath => {
            try {
                totalSize += fs.statSync(filePath).size;
            } catch (e) {}
        });

        return {
            totalFiles,
            poFiles: poFiles.length,
            codeFiles: codeFiles.length,
            logFiles: logFiles.length,
            totalSizeBytes: totalSize,
            totalSizeMB: Math.round(totalSize / (1024 * 1024) * 100) / 100
        };
    }
}

// Singleton instance
const fileManager = new FileManager();

module.exports = fileManager;