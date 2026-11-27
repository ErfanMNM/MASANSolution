// fileManager.js - JSON-based file storage to replace SQLite
const fs = require('fs');
const path = require('path');
const { randomUUID } = require('crypto');

class FileManager {
    constructor() {
        this.dataDir = './data';
        this.codesDir = './codes_json';
        this.fileOperations = new Map();
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
        const tempPath = `${filePath}.${process.pid}.${randomUUID()}.tmp`;
        const jsonData = JSON.stringify(data, null, 2);
        try {
            await fs.promises.writeFile(tempPath, jsonData, { encoding: 'utf8', flag: 'wx' });
            await fs.promises.rename(tempPath, filePath);
        } finally {
            await fs.promises.unlink(tempPath).catch(error => {
                if (error.code !== 'ENOENT') console.error('Unable to remove temporary JSON file:', error.code);
            });
        }
    }

    // Serialize the entire read-modify-write operation, not just the rename.
    withFileLock(filePath, operation) {
        const absolutePath = path.resolve(filePath);
        const key = process.platform === 'win32' ? absolutePath.toLowerCase() : absolutePath;
        const previous = this.fileOperations.get(key) || Promise.resolve();
        const current = previous.then(operation);
        const tail = current.catch(() => {});
        this.fileOperations.set(key, tail);
        return current.finally(() => {
            if (this.fileOperations.get(key) === tail) this.fileOperations.delete(key);
        });
    }

    async readFileWithRetry(filePath, defaultValue = null, maxRetries = 3) {
        for (let attempt = 1; attempt <= maxRetries; attempt++) {
            try {
                const data = await fs.promises.readFile(filePath, 'utf8');
                return JSON.parse(data);
            } catch (error) {
                if (error.code === 'ENOENT') return defaultValue;
                if (attempt === maxRetries) {
                    throw error;
                }
                // Wait và retry
                await new Promise(resolve => setTimeout(resolve, 10 + Math.random() * 40));
            }
        }
    }

    // PO Info Operations
    async savePOInfo(poData) {
        const name = this.sanitizeFileName(poData.orderNo);
        // Daily logs share dataDir; reserve their namespace without moving existing files.
        const filePath = path.join(this.dataDir, `${/^logs_/i.test(name) ? 'PO_' : ''}${name}.json`);
        return this.withFileLock(filePath, async () => {
            const stored = await this.readFileWithRetry(filePath);
            if (stored && stored.orderNo !== poData.orderNo) throw new Error('PO filename collision');
            const existing = await this.getPOInfo(poData.orderNo);
            const timestamp = new Date().toISOString();

            const data = {
                ...poData,
                lastUpdated: timestamp,
                createdAt: poData.createdAt || (existing && existing.createdAt) || timestamp
            };

            await this.writeFileWithRetry(filePath, data);
            return data;
        });
    }

    async getPOInfo(orderNo) {
        const name = this.sanitizeFileName(orderNo);
        const reserved = /^logs_/i.test(name);
        const filePath = path.join(this.dataDir, `${reserved ? 'PO_' : ''}${name}.json`);
        const data = await this.readFileWithRetry(filePath);
        if (data && data.orderNo === orderNo) return data;
        // Accept prefixed files as well without changing existing storage names.
        const prefixed = await this.readFileWithRetry(path.join(this.dataDir, `${reserved ? '' : 'PO_'}${name}.json`));
        return prefixed && prefixed.orderNo === orderNo ? prefixed : null;
    }

    async getAllPOInfo(limit = 100) {
        const files = fs.readdirSync(this.dataDir)
            .filter(file => file.endsWith('.json'))
            .map(file => {
                const filePath = path.join(this.dataDir, file);
                const stats = fs.statSync(filePath);
                return { file, filePath, mtime: stats.mtime };
            })
            .sort((a, b) => b.mtime - a.mtime);

        const results = [];
        const seen = new Set();
        for (const { filePath } of files) {
            try {
                const data = await this.readFileWithRetry(filePath);
                if (!data || typeof data.orderNo !== 'string' || seen.has(data.orderNo)) continue;
                const name = this.sanitizeFileName(data.orderNo);
                const file = path.basename(filePath);
                if (file !== `${name}.json` && file !== `PO_${name}.json`) continue;
                const current = await this.getPOInfo(data.orderNo);
                if (current) {
                    results.push(current);
                    seen.add(data.orderNo);
                }
                if (results.length >= limit) break;
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
        return this.withFileLock(filePath, async () => {

            // Read existing data
            let existingData = await this.readFileWithRetry(filePath, {
                gtin: gtin,
                blocks: {},
                totalCodes: 0,
                lastUpdated: new Date().toISOString()
        });
        if (existingData.gtin !== gtin) throw new Error('GTIN filename collision');

        existingData.blocks = Object.assign(Object.create(null), existingData.blocks);

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

        // Get ALL existing codes in GTIN (across all blocks) to check duplicates
        const allExistingCodes = new Set();
        for (const block of Object.values(existingData.blocks)) {
            for (const codeEntry of block.codes) {
                allExistingCodes.add(codeEntry.code);
            }
        }
        
        // Filter out codes that already exist in ANY block of this GTIN
        const newCodes = processedCodes.filter(c => {
            if (allExistingCodes.has(c.code)) return false;
            allExistingCodes.add(c.code);
            return true;
        });
        
        // Add new codes
        for (const code of newCodes) existingData.blocks[blockNo].codes.push(code);
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
        });
    }

    async getUniqueCodes(gtin, blockNo = null) {
        const safeGTIN = this.sanitizeFileName(gtin);
        const filePath = path.join(this.codesDir, `GTIN_${safeGTIN}.json`);
        
        const data = await this.readFileWithRetry(filePath);
        if (data && data.gtin !== gtin) throw new Error('GTIN filename collision');
        if (!data || !data.blocks) return [];

        if (blockNo) {
            return Object.hasOwn(data.blocks, blockNo) ? data.blocks[blockNo].codes : [];
        }

        // Return all codes from all blocks
        const allCodes = [];
        for (const block of Object.values(data.blocks)) {
            for (const code of block.codes) allCodes.push(code);
        }
        
        return allCodes.sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt));
    }

    async getCodesCount(gtin, blockNo = null) {
        const filePath = path.join(this.codesDir, `GTIN_${this.sanitizeFileName(gtin)}.json`);
        const data = await this.readFileWithRetry(filePath);
        if (data && data.gtin !== gtin) throw new Error('GTIN filename collision');
        if (!data || !data.blocks) return 0;
        if (blockNo) return Object.hasOwn(data.blocks, blockNo) ? data.blocks[blockNo].codes.length : 0;
        return Object.values(data.blocks).reduce((total, block) => total + block.codes.length, 0);
    }

    async deleteBlockCodes(gtin, blockNo) {
        const safeGTIN = this.sanitizeFileName(gtin);
        const filePath = path.join(this.codesDir, `GTIN_${safeGTIN}.json`);
        return this.withFileLock(filePath, async () => {

            const data = await this.readFileWithRetry(filePath);
            if (data && data.gtin !== gtin) throw new Error('GTIN filename collision');
            if (!data || !data.blocks || !Object.hasOwn(data.blocks, blockNo)) {
                return false;
            }

            delete data.blocks[blockNo];

            // Update totals
            data.totalCodes = Object.values(data.blocks)
                .reduce((total, block) => total + block.codes.length, 0);
            data.lastUpdated = new Date().toISOString();

            await this.writeFileWithRetry(filePath, data);
            return true;
        });
    }

    // Logs Operations (JSON-based logs)
    async savePOLog(logData) {
        const date = new Date().toISOString().split('T')[0];
        const filePath = path.join(this.dataDir, `logs_${date}.json`);
        return this.withFileLock(filePath, async () => {

            const timestamp = new Date().toISOString();
            const logEntry = {
                id: Date.now() + Math.random(), // Simple unique ID
                timestamp: timestamp,
                ...logData
            };

            // Read existing logs
            let existingLogs = await this.readFileWithRetry(filePath, { date: date, logs: [] });
            if (!Array.isArray(existingLogs.logs)) throw new Error('Invalid PO log storage');

            // Add new log
            existingLogs.logs.push(logEntry);

            // Keep only last 1000 logs per day to prevent huge files
            if (existingLogs.logs.length > 1000) {
                existingLogs.logs = existingLogs.logs.slice(-1000);
            }

            await this.writeFileWithRetry(filePath, existingLogs);
            return logEntry;
        });
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
    async codesFileExists(gtin) {
        const safeGTIN = this.sanitizeFileName(gtin);
        const filePath = path.join(this.codesDir, `GTIN_${safeGTIN}.json`);
        const data = await this.readFileWithRetry(filePath);
        if (data && data.gtin !== gtin) throw new Error('GTIN filename collision');
        return data !== null;
    }

    // Get all POs by GTIN
    async getPOsByGTIN(gtin, excludeOrderNo = null) {
        const orders = await this.getAllPOInfo(Infinity);
        return orders.filter(data => data.GTIN === gtin && data.orderNo !== excludeOrderNo);
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
            .filter(file => file.endsWith('.json') && !file.startsWith('logs_'));

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
