const express = require('express');
const cors = require('cors');

const app = express();
app.use(cors());
app.use(express.json());

let tempStorage = Object.create(null);

// POST để set một hoặc nhiều key-value
app.post('/set', (req, res) => {
    const input = req.body;

    // Kiểm tra nếu input là mảng hoặc object đơn
    const items = Array.isArray(input) ? input : [input];
    const results = [];
    const errors = [];

    for (const item of items) {
        if (!item || typeof item !== 'object') {
            errors.push({ error: 'Each item must be an object' });
            continue;
        }
        const { key, value, ttl } = item;

        // Kiểm tra dữ liệu đầu vào
        if (!key || value === undefined) {
            errors.push({ key, error: 'Key and value are required' });
            continue;
        }
        if (ttl !== undefined && (typeof ttl !== 'number' || !Number.isFinite(ttl) || ttl <= 0 || ttl > 2147483647)) {
            errors.push({ key, error: 'TTL must be a positive number' });
            continue;
        }

        // Lưu giá trị
        const entry = { value, expiresAt: ttl ? Date.now() + ttl : null };
        tempStorage[key] = entry;

        // Nếu có TTL, đặt giá trị về 0 sau thời gian TTL
        if (ttl) {
            setTimeout(() => {
                if (tempStorage[key] === entry && entry.expiresAt <= Date.now()) {
                    tempStorage[key].value = 0; // Đặt về 0 thay vì xóa
                }
            }, ttl);
        }

        results.push({ success: true, key, value, ttl });
    }

    // Trả về phản hồi
    if (errors.length > 0) {
        return res.status(400).json({ results, errors });
    }
    res.json({ results });
});

// POST để lấy giá trị của nhiều key
app.post('/get', (req, res) => {
    const { keys } = req.body;

    // Kiểm tra dữ liệu đầu vào
    if (!Array.isArray(keys)) {
        return res.status(400).json({ error: 'Keys must be an array' });
    }

    const results = Object.create(null);
    for (const key of keys) {
        const data = tempStorage[key];
        results[key] = data && (data.expiresAt === null || data.expiresAt > Date.now()) ? data.value : 0;
    }

    res.json({ values: results });
});

// GET để lấy giá trị của một key
app.get('/get/:key', (req, res) => {
    const { key } = req.params;
    const data = tempStorage[key];

    if (data && (data.expiresAt === null || data.expiresAt > Date.now())) {
        res.json({ value: data.value, expiresAt: data.expiresAt });
    } else {
        res.json({ value: 0, expiresAt: data ? data.expiresAt : null });
    }
});

// DELETE để xóa key
app.delete('/delete/:key', (req, res) => {
    const { key } = req.params;
    delete tempStorage[key];
    res.json({ success: true });
});

// GET để liệt kê tất cả keys
app.get('/keys', (req, res) => {
    res.json({ keys: Object.keys(tempStorage) });
});

const PORT = process.env.PORT || 49211;
app.listen(PORT, () => {
    console.log(`Server is running on http://localhost:${PORT}`);
});
