// codeDbManager.js - Quản lý database connections cho từng PO
const sqlite3 = require('sqlite3').verbose();
const fs = require('fs');
const path = require('path');

class CodeDBManager {
    constructor() {
        this.dbPools = new Map(); // Map orderNo -> connection pool
        this.maxConnectionsPerDB = 3;
        this.operationQueue = new Map(); // Map orderNo -> operation queue
    }

    // Tạo hoặc lấy connection pool cho một PO
    getDBPool(orderNo) {
        const safeOrderNo = orderNo.replace(/[^a-zA-Z0-9_\-]/g, '_');
        
        if (!this.dbPools.has(safeOrderNo)) {
            this.createDBPool(safeOrderNo);
        }
        
        return this.dbPools.get(safeOrderNo);
    }

    createDBPool(safeOrderNo) {
        const dbPath = `./codes/${safeOrderNo}.db`;
        
        // Ensure codes directory exists
        if (!fs.existsSync('./codes')) {
            fs.mkdirSync('./codes');
        }

        const pool = {
            connections: [],
            busyConnections: new Set(),
            queue: []
        };

        // Create connections
        for (let i = 0; i < this.maxConnectionsPerDB; i++) {
            const db = new sqlite3.Database(dbPath);
            
            db.serialize(() => {
                // Enable WAL mode and optimize for concurrent access
                db.run("PRAGMA journal_mode=WAL");
                db.run("PRAGMA synchronous=NORMAL");
                db.run("PRAGMA temp_store=memory");
                db.run("PRAGMA busy_timeout=30000");
                db.run("PRAGMA cache_size=5000");
                
                // Create table if not exists
                db.run(`
                    CREATE TABLE IF NOT EXISTS UniqueCodes (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        code TEXT UNIQUE,
                        createdAt TEXT,
                        blockNo TEXT
                    )
                `);
            });
            
            pool.connections.push(db);
        }

        this.dbPools.set(safeOrderNo, pool);
        this.operationQueue.set(safeOrderNo, []);
    }

    async getConnection(orderNo) {
        const safeOrderNo = orderNo.replace(/[^a-zA-Z0-9_\-]/g, '_');
        const pool = this.getDBPool(safeOrderNo);
        
        return new Promise((resolve, reject) => {
            const availableConnection = pool.connections.find(conn => !pool.busyConnections.has(conn));
            
            if (availableConnection) {
                pool.busyConnections.add(availableConnection);
                resolve(availableConnection);
            } else {
                // Add to queue with timeout
                const timeoutId = setTimeout(() => {
                    const index = pool.queue.findIndex(item => item.reject === reject);
                    if (index !== -1) {
                        pool.queue.splice(index, 1);
                    }
                    reject(new Error('Database connection timeout'));
                }, 30000);

                pool.queue.push({ 
                    resolve: (conn) => {
                        clearTimeout(timeoutId);
                        resolve(conn);
                    }, 
                    reject,
                    timeoutId
                });
            }
        });
    }

    releaseConnection(orderNo, connection) {
        const safeOrderNo = orderNo.replace(/[^a-zA-Z0-9_\-]/g, '_');
        const pool = this.dbPools.get(safeOrderNo);
        
        if (!pool) return;
        
        pool.busyConnections.delete(connection);
        
        // Process queue
        if (pool.queue.length > 0) {
            const { resolve, timeoutId } = pool.queue.shift();
            clearTimeout(timeoutId);
            pool.busyConnections.add(connection);
            resolve(connection);
        }
    }

    // Optimized batch insert with transaction
    async batchInsertCodes(orderNo, codes, blockNo, createdAt) {
        const connection = await this.getConnection(orderNo);
        const self = this;
        
        return new Promise((resolve, reject) => {
            const timeout = setTimeout(() => {
                self.releaseConnection(orderNo, connection);
                reject(new Error('Batch insert timeout'));
            }, 60000);

            connection.serialize(() => {
                connection.run("BEGIN TRANSACTION", (err) => {
                    if (err) {
                        clearTimeout(timeout);
                        self.releaseConnection(orderNo, connection);
                        return reject(err);
                    }

                    const stmt = connection.prepare(`
                        INSERT OR IGNORE INTO UniqueCodes (code, createdAt, blockNo) 
                        VALUES (?, ?, ?)
                    `);
                    
                    let insertCount = 0;
                    let completed = 0;
                    let errorOccurred = false;

                    if (codes.length === 0) {
                        stmt.finalize();
                        connection.run("COMMIT");
                        clearTimeout(timeout);
                        self.releaseConnection(orderNo, connection);
                        return resolve({ insertedCount: 0 });
                    }

                    codes.forEach((code, index) => {
                        stmt.run([code, createdAt, blockNo], function(err) {
                            if (errorOccurred) return;
                            
                            if (err) {
                                errorOccurred = true;
                                stmt.finalize();
                                connection.run("ROLLBACK");
                                clearTimeout(timeout);
                                self.releaseConnection(orderNo, connection);
                                return reject(err);
                            }
                            
                            if (this.changes > 0) {
                                insertCount++;
                            }
                            
                            completed++;
                            
                            if (completed === codes.length) {
                                stmt.finalize((err) => {
                                    if (err) {
                                        connection.run("ROLLBACK");
                                        clearTimeout(timeout);
                                        self.releaseConnection(orderNo, connection);
                                        return reject(err);
                                    }

                                    connection.run("COMMIT", (err) => {
                                        clearTimeout(timeout);
                                        self.releaseConnection(orderNo, connection);
                                        
                                        if (err) {
                                            reject(err);
                                        } else {
                                            resolve({ insertedCount: insertCount });
                                        }
                                    });
                                });
                            }
                        });
                    });
                });
            });
        });
    }

    // Get codes count with connection pooling
    async getCodesCount(orderNo, blockNo = null) {
        const connection = await this.getConnection(orderNo);
        
        return new Promise((resolve, reject) => {
            const timeout = setTimeout(() => {
                this.releaseConnection(orderNo, connection);
                reject(new Error('Get count timeout'));
            }, 15000);

            const sql = blockNo 
                ? "SELECT COUNT(*) AS count FROM UniqueCodes WHERE blockNo = ?"
                : "SELECT COUNT(*) AS count FROM UniqueCodes";
            
            const params = blockNo ? [blockNo] : [];

            connection.get(sql, params, (err, row) => {
                clearTimeout(timeout);
                this.releaseConnection(orderNo, connection);
                
                if (err) {
                    reject(err);
                } else {
                    resolve(row ? row.count : 0);
                }
            });
        });
    }

    // Get all codes with connection pooling
    async getAllCodes(orderNo) {
        const connection = await this.getConnection(orderNo);
        
        return new Promise((resolve, reject) => {
            const timeout = setTimeout(() => {
                this.releaseConnection(orderNo, connection);
                reject(new Error('Get all codes timeout'));
            }, 30000);

            connection.all("SELECT code FROM UniqueCodes", [], (err, rows) => {
                clearTimeout(timeout);
                this.releaseConnection(orderNo, connection);
                
                if (err) {
                    reject(err);
                } else {
                    resolve(rows || []);
                }
            });
        });
    }

    // Check if database exists
    dbExists(orderNo) {
        const safeOrderNo = orderNo.replace(/[^a-zA-Z0-9_\-]/g, '_');
        const dbPath = `./codes/${safeOrderNo}.db`;
        return fs.existsSync(dbPath);
    }

    // Cleanup unused connections (call periodically)
    cleanup() {
        const now = Date.now();
        
        this.dbPools.forEach((pool, orderNo) => {
            if (pool.busyConnections.size === 0 && pool.queue.length === 0) {
                // Close all connections for this orderNo
                pool.connections.forEach(conn => {
                    try {
                        conn.close();
                    } catch (e) {
                        console.error('Error closing connection:', e);
                    }
                });
                
                this.dbPools.delete(orderNo);
                this.operationQueue.delete(orderNo);
            }
        });
    }
}

// Singleton instance
const codeDBManager = new CodeDBManager();

// Cleanup every 5 minutes
setInterval(() => {
    codeDBManager.cleanup();
}, 5 * 60 * 1000);

module.exports = codeDBManager;