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
        
        this.initializeDatabase();
    }

    initializeDatabase() {
        // Tạo database chính và enable WAL mode
        const mainDb = new sqlite3.Database(this.dbPath);
        
        mainDb.serialize(() => {
            // Enable WAL mode for better concurrent access
            mainDb.run("PRAGMA journal_mode=WAL");
            mainDb.run("PRAGMA synchronous=NORMAL");
            mainDb.run("PRAGMA temp_store=memory");
            mainDb.run("PRAGMA mmap_size=268435456"); // 256MB
            mainDb.run("PRAGMA cache_size=10000");
            
            // Create tables
            mainDb.run(`
                CREATE TABLE IF NOT EXISTS POInfo (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    orderNo TEXT UNIQUE,
                    site TEXT,
                    factory TEXT,
                    productionLine TEXT,
                    productionDate TEXT,
                    shift TEXT,
                    orderQty INTEGER,
                    lotNumber TEXT,
                    productCode TEXT,
                    productName TEXT,
                    GTIN TEXT,
                    customerOrderNo TEXT,
                    uom TEXT,
                    lastUpdated TEXT
                )
            `);

            mainDb.run(`
                CREATE TABLE IF NOT EXISTS POLogs (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    orderNo TEXT,
                    action TEXT,
                    detail TEXT,
                    createdAt TEXT
                )
            `);
        });

        // Add to connection pool
        this.connections.push(mainDb);
        
        // Create additional connections
        for (let i = 1; i < this.maxConnections; i++) {
            const db = new sqlite3.Database(this.dbPath);
            db.run("PRAGMA journal_mode=WAL");
            db.run("PRAGMA synchronous=NORMAL");
            db.run("PRAGMA temp_store=memory");
            db.run("PRAGMA busy_timeout=30000");
            this.connections.push(db);
        }
    }

    getConnection() {
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
        this.busyConnections.delete(connection);
        
        // Process queue
        if (this.queue.length > 0) {
            const { resolve } = this.queue.shift();
            this.busyConnections.add(connection);
            resolve(connection);
        }
    }

    // Wrapper methods for database operations
    get(sql, params = []) {
        return new Promise(async (resolve, reject) => {
            const connection = await this.getConnection();
            
            const timeout = setTimeout(() => {
                this.releaseConnection(connection);
                reject(new Error('Database operation timeout'));
            }, 30000);

            connection.get(sql, params, (err, row) => {
                clearTimeout(timeout);
                this.releaseConnection(connection);
                
                if (err) reject(err);
                else resolve(row);
            });
        });
    }

    all(sql, params = []) {
        return new Promise(async (resolve, reject) => {
            const connection = await this.getConnection();
            
            const timeout = setTimeout(() => {
                this.releaseConnection(connection);
                reject(new Error('Database operation timeout'));
            }, 30000);

            connection.all(sql, params, (err, rows) => {
                clearTimeout(timeout);
                this.releaseConnection(connection);
                
                if (err) reject(err);
                else resolve(rows);
            });
        });
    }

    run(sql, params = []) {
        return new Promise(async (resolve, reject) => {
            const connection = await this.getConnection();
            const self = this;
            
            const timeout = setTimeout(() => {
                self.releaseConnection(connection);
                reject(new Error('Database operation timeout'));
            }, 30000);

            connection.run(sql, params, function(err) {
                clearTimeout(timeout);
                self.releaseConnection(connection);
                
                if (err) reject(err);
                else resolve({ lastID: this.lastID, changes: this.changes });
            });
        });
    }
}

// Create database pool instance
const dbPool = new DatabasePool('./po.db', 5);

module.exports = dbPool;
