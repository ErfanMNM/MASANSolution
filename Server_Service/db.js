const sqlite3 = require('sqlite3').verbose();
const db = new sqlite3.Database('./po_data.db');

// Tạo bảng nếu chưa có
db.serialize(() => {
    // Bảng lưu thông tin PO
    db.run(`
    CREATE TABLE IF NOT EXISTS po_records (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      orderNo TEXT NOT NULL,
      uniqueCode TEXT NOT NULL,
      site TEXT,
      factory TEXT,
      productionLine TEXT,
      productionDate TEXT,
      shift TEXT,
      czFileUrl TEXT,
      createdAt TEXT DEFAULT CURRENT_TIMESTAMP,
      UNIQUE(orderNo, uniqueCode)
    )
  `);

    // Bảng log
    db.run(`
    CREATE TABLE IF NOT EXISTS po_logs (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      timestamp TEXT DEFAULT CURRENT_TIMESTAMP,
      status TEXT,
      orderNo TEXT,
      uniqueCode TEXT,
      czFile TEXT,
      message TEXT
    )
  `);
});

module.exports = db;
