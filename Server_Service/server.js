const express = require('express');
const cors = require('cors');

const app = express();
app.use(cors());
app.use(express.json());

let tempStorage = {};

// POST để set key-value với TTL tùy chỉnh hoặc vĩnh viễn
app.post('/set', (req, res) => {
    const { key, value, ttl } = req.body;

    // Kiểm tra dữ liệu đầu vào
    if (!key || value === undefined) {
        return res.status(400).json({ error: 'Key and value are required' });
    }
    if (ttl !== undefined && (typeof ttl !== 'number' || ttl <= 0)) {
        return res.status(400).json({ error: 'TTL must be a positive number' });
    }

    // Lưu giá trị
    tempStorage[key] = { value, expiresAt: ttl ? Date.now() + ttl : null };

    // Nếu có TTL, đặt giá trị về 0 sau thời gian TTL
    if (ttl) {
        setTimeout(() => {
            if (tempStorage[key] && tempStorage[key].expiresAt <= Date.now()) {
                tempStorage[key].value = 0; // Đặt về 0 thay vì xóa
            }
        }, ttl);
    }

    res.json({ success: true, key, value, ttl });
});

// GET để lấy giá trị theo key
app.get('/get/:key', (req, res) => {
    const { key } = req.params;
    const data = tempStorage[key];

    if (data && (data.expiresAt === null || data.expiresAt > Date.now())) {
        res.json({ value: data.value });
    } else {
        // Nếu hết hạn, trả về giá trị 0
        res.json({ value: data ? data.value : 0 });
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

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
    console.log(`Server is running on http://localhost:${PORT}`);
});