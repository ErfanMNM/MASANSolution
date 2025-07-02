// db_po.js
const sqlite3 = require('sqlite3').verbose();
const db = new sqlite3.Database('./po.db');

db.serialize(() => {
    db.run(`
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
            lastUpdated TEXT
        )
    `);

    db.run(`
        CREATE TABLE IF NOT EXISTS POLogs (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            orderNo TEXT,
            action TEXT,
            detail TEXT,
            createdAt TEXT
        )
    `);
});

module.exports = db;
