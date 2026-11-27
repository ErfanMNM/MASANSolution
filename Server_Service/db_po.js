// db_po.js
const sqlite3 = require('sqlite3').verbose();
const fs = require('fs');

class DatabasePool {
    constructor(dbPath, maxConnections = 5) {
        this.dbPath = dbPath;
        this.maxConnections = maxConnections;
        this.connections = [];
        this.queue = [];
        this.busyConnections = new Set();
        
        this.ready = this.initializeDatabase();
        this.ready.catch(() => {});
    }

    async initializeDatabase() {
        const open = () => new Promise((resolve, reject) => {
            const db = new sqlite3.Database(this.dbPath, err => err ? reject(err) : resolve(db));
        });
        const execute = (db, sql) => new Promise((resolve, reject) => db.run(sql, err => err ? reject(err) : resolve()));
        try {
            for (let i = 0; i < this.maxConnections; i++) {
                const db = await open();
                this.connections.push(db);
                for (const sql of ['PRAGMA busy_timeout=30000', 'PRAGMA journal_mode=WAL', 'PRAGMA synchronous=NORMAL', 'PRAGMA temp_store=memory']) await execute(db, sql);
                if (i === 0) {
                    await execute(db, 'PRAGMA mmap_size=268435456');
                    await execute(db, 'PRAGMA cache_size=10000');
                    await execute(db, `CREATE TABLE IF NOT EXISTS POInfo (
                        id INTEGER PRIMARY KEY AUTOINCREMENT, orderNo TEXT UNIQUE, site TEXT, factory TEXT,
                        productionLine TEXT, productionDate TEXT, shift TEXT, orderQty INTEGER, lotNumber TEXT,
                        productCode TEXT, productName TEXT, GTIN TEXT, customerOrderNo TEXT, uom TEXT, lastUpdated TEXT
                    )`);
                    await execute(db, `CREATE TABLE IF NOT EXISTS POLogs (
                        id INTEGER PRIMARY KEY AUTOINCREMENT, orderNo TEXT, action TEXT, detail TEXT, createdAt TEXT
                    )`);
                }
            }
        } catch (error) {
            for (const db of this.connections.splice(0)) db.close(() => {});
            throw error;
        }
    }

    async getConnection() {
        await this.ready;
        return new Promise((resolve, reject) => {
            const availableConnection = this.connections.find(conn => !this.busyConnections.has(conn));
            
            if (availableConnection) {
                this.busyConnections.add(availableConnection);
                resolve(availableConnection);
            } else {
                // Add to queue
                this.queue.push({ resolve, reject });
            }
        });
    }

    releaseConnection(connection) {
        if (!this.busyConnections.has(connection)) return;
        this.busyConnections.delete(connection);
        
        // Process queue
        if (this.queue.length > 0) {
            const { resolve } = this.queue.shift();
            this.busyConnections.add(connection);
            resolve(connection);
        }
    }

    // Wrapper methods for database operations
    async get(sql, params = []) {
        const connection = await this.getConnection();
        return new Promise((resolve, reject) => {
            
            const timeout = setTimeout(() => {
                reject(new Error('Database operation timeout'));
            }, 30000);

            try { connection.get(sql, params, (err, row) => {
                clearTimeout(timeout);
                this.releaseConnection(connection);
                
                if (err) reject(err);
                else resolve(row);
            }); } catch (error) {
                clearTimeout(timeout);
                this.releaseConnection(connection);
                reject(error);
            }
        });
    }

    async all(sql, params = []) {
        const connection = await this.getConnection();
        return new Promise((resolve, reject) => {
            
            const timeout = setTimeout(() => {
                reject(new Error('Database operation timeout'));
            }, 30000);

            try { connection.all(sql, params, (err, rows) => {
                clearTimeout(timeout);
                this.releaseConnection(connection);
                
                if (err) reject(err);
                else resolve(rows);
            }); } catch (error) {
                clearTimeout(timeout);
                this.releaseConnection(connection);
                reject(error);
            }
        });
    }

    async run(sql, params = []) {
        const connection = await this.getConnection();
        return new Promise((resolve, reject) => {
            const self = this;
            
            const timeout = setTimeout(() => {
                reject(new Error('Database operation timeout'));
            }, 30000);

            try { connection.run(sql, params, function(err) {
                clearTimeout(timeout);
                self.releaseConnection(connection);
                
                if (err) reject(err);
                else resolve({ lastID: this.lastID, changes: this.changes });
            }); } catch (error) {
                clearTimeout(timeout);
                self.releaseConnection(connection);
                reject(error);
            }
        });
    }
}

// Create database pool instance
const dbPool = new DatabasePool('./po.db', 5);

module.exports = dbPool;
