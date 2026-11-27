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

        this.dbPools.set(safeOrderNo, pool);
        this.operationQueue.set(safeOrderNo, []);
        pool.ready = (async () => {
            try {
                for (let i = 0; i < this.maxConnectionsPerDB; i++) {
                    const db = await new Promise((resolve, reject) => {
                        const opened = new sqlite3.Database(dbPath, err => err ? reject(err) : resolve(opened));
                    });
                    pool.connections.push(db);
                    for (const sql of ['PRAGMA busy_timeout=30000', 'PRAGMA journal_mode=WAL', 'PRAGMA synchronous=NORMAL', 'PRAGMA temp_store=memory', 'PRAGMA cache_size=5000',
                        `CREATE TABLE IF NOT EXISTS UniqueCodes (id INTEGER PRIMARY KEY AUTOINCREMENT, code TEXT UNIQUE, createdAt TEXT, blockNo TEXT)`]) {
                        await new Promise((resolve, reject) => db.run(sql, err => err ? reject(err) : resolve()));
                    }
                }
                pool.initialized = true;
            } catch (error) {
                for (const db of pool.connections.splice(0)) db.close(() => {});
                if (this.dbPools.get(safeOrderNo) === pool) {
                    this.dbPools.delete(safeOrderNo);
                    this.operationQueue.delete(safeOrderNo);
                }
                throw error;
            }
        })();
        pool.ready.catch(() => {});
    }

    async getConnection(orderNo) {
        const safeOrderNo = orderNo.replace(/[^a-zA-Z0-9_\-]/g, '_');
        const pool = this.getDBPool(safeOrderNo);
        await pool.ready;
        
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
        
        if (!pool || !pool.connections.includes(connection) || !pool.busyConnections.has(connection)) return;
        
        pool.busyConnections.delete(connection);
        
        // Process queue
        if (pool.queue.length > 0) {
            const { resolve, timeoutId } = pool.queue.shift();
            clearTimeout(timeoutId);
            pool.busyConnections.add(connection);
            resolve(connection);
        }
    }

    // The lease remains busy until the native operation and transaction cleanup finish.
    async batchInsertCodes(orderNo, codes, blockNo, createdAt) {
        const connection = await this.getConnection(orderNo);
        let timedOut = false;
        const run = sql => new Promise((resolve, reject) => connection.run(sql, err => err ? reject(err) : resolve()));
        return new Promise((resolve, reject) => {
            const timeout = setTimeout(() => {
                timedOut = true;
                reject(new Error('Batch insert timeout'));
            }, 60000);
            const operation = async () => {
                let inTransaction = false;
                let stmt;
                try {
                    await run('BEGIN TRANSACTION');
                    inTransaction = true;
                    if (timedOut) throw new Error('Batch insert timeout');
                    if (codes.length > 0) {
                        stmt = await new Promise((resolveStatement, rejectStatement) => {
                            const prepared = connection.prepare(
                                'INSERT OR IGNORE INTO UniqueCodes (code, createdAt, blockNo) VALUES (?, ?, ?)',
                                err => err ? rejectStatement(err) : resolveStatement(prepared));
                        });
                    }
                    let insertCount = 0;
                    try {
                        await Promise.all(codes.map(code => new Promise((resolveRow, rejectRow) => {
                            stmt.run([code, createdAt, blockNo], function(err) {
                                if (err) rejectRow(err);
                                else { insertCount += this.changes > 0 ? 1 : 0; resolveRow(); }
                            });
                        })));
                    } finally {
                        // finalize waits for already queued statement runs, even after a row error.
                        if (stmt) await new Promise((resolveStatement, rejectStatement) => stmt.finalize(err => err ? rejectStatement(err) : resolveStatement()));
                    }
                    if (timedOut) throw new Error('Batch insert timeout');
                    await run('COMMIT');
                    inTransaction = false;
                    return { insertedCount: insertCount };
                } catch (error) {
                    if (inTransaction) {
                        try { await run('ROLLBACK'); }
                        catch (rollbackError) {
                            // Never return a connection with unknown transaction state to the pool.
                            const safeOrderNo = orderNo.replace(/[^a-zA-Z0-9_\-]/g, '_');
                            const pool = this.dbPools.get(safeOrderNo);
                            if (pool) {
                                pool.connections = pool.connections.filter(conn => conn !== connection);
                                pool.busyConnections.delete(connection);
                                if (pool.connections.length === 0) {
                                    for (const pending of pool.queue.splice(0)) { clearTimeout(pending.timeoutId); pending.reject(rollbackError); }
                                    this.dbPools.delete(safeOrderNo);
                                    this.operationQueue.delete(safeOrderNo);
                                }
                            }
                            connection.close(() => {});
                        }
                    }
                    throw error;
                }
            };
            operation().then(resolve, reject).finally(() => {
                clearTimeout(timeout);
                this.releaseConnection(orderNo, connection);
            });
        });
    }

    // Get codes count with connection pooling
    async getCodesCount(orderNo, blockNo = null) {
        const connection = await this.getConnection(orderNo);
        
        return new Promise((resolve, reject) => {
            const timeout = setTimeout(() => {
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
            if (pool.initialized && pool.connections.length > 0 && pool.busyConnections.size === 0 && pool.queue.length === 0) {
                // Close all connections for this orderNo
                pool.connections.forEach(conn => {
                    try {
                        conn.close(() => {});
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
