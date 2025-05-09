const express = require('express');
const cors = require('cors');

const app = express();
app.use(cors());
app.use(express.json());

let tempStorage = {};
let timerStorage = {};

// Key-Value đơn giản
app.get('/set/:key/:value', (req, res) => {
    const { key, value } = req.params;
    if (!key) return res.status(400).json({ error: 'Key is required' });

    tempStorage[key] = value;
    res.json({ success: true, key, value });
});

app.get('/get/:key', (req, res) => {
    const { key } = req.params;
    res.json({ value: tempStorage[key] || null });
});

app.delete('/delete/:key', (req, res) => {
    const { key } = req.params;
    delete tempStorage[key];
    res.json({ success: true });
});

app.get('/keys', (req, res) => {
    res.json({ keys: Object.keys(tempStorage) });
});

// TIMER ---------------------------------------------------

// ✅ Create new variable
app.post('/timer/create', (req, res) => {
    const { key, defaultValue, timeMs } = req.body;

    if (!key || timeMs == null || defaultValue == null) {
        return res.status(400).json({ error: 'Missing key, timeMs or defaultValue' });
    }

    // Nếu đã tồn tại thì báo lỗi
    if (timerStorage[key]) {
        return res.status(400).json({ error: 'Key already exists' });
    }

    // Gán giá trị mặc định cho value khi tạo mới
    timerStorage[key] = {
        value: defaultValue,
        defaultValue,
        timeMs,
        startTime: null,
        timeoutId: null,
    };

    res.json({ success: true, message: 'Variable created' });
});

// ✅ Set temporary value with auto reset
app.post('/timer/set/:key/:value', (req, res) => {
    const { key } = req.params;
    const { value } = req.body;

    const timer = timerStorage[key];
    if (!timer) return res.status(404).json({ error: 'Key not found' });

    if (value === undefined) {
        return res.status(400).json({ error: 'Value is required' });
    }

    if (timer.timeoutId) {
        clearTimeout(timer.timeoutId);
    }

    timer.value = value;
    timer.startTime = Date.now();

    timer.timeoutId = setTimeout(() => {
        timer.value = timer.defaultValue;
        timer.timeoutId = null;
        timer.startTime = null;
        console.log(`Timer '${key}' auto-reset to default`);
    }, timer.timeMs);

    res.json({ success: true, message: 'Value set with timer', value });
});

// ✅ Get current value
app.get('/timer/value/:key', (req, res) => {
    const { key } = req.params;
    const timer = timerStorage[key];
    if (!timer) return res.status(404).json({ error: 'Key not found' });

    let remaining = 0;
    if (timer.startTime && timer.timeoutId) {
        const elapsed = Date.now() - timer.startTime;
        remaining = Math.max(timer.timeMs - elapsed, 0);
    }

    res.json({
        key,
        value: timer.value,
        defaultValue: timer.defaultValue,
        timeMs: timer.timeMs,
        remainingTimeMs: remaining
    });
});

// ✅ Get all timers
app.get('/timer/list', (req, res) => {
    const result = Object.entries(timerStorage).map(([key, timer]) => {
        let remaining = 0;
        if (timer.startTime && timer.timeoutId) {
            const elapsed = Date.now() - timer.startTime;
            remaining = Math.max(timer.timeMs - elapsed, 0);
        }

        return {
            key,
            value: timer.value,
            defaultValue: timer.defaultValue,
            timeMs: timer.timeMs,
            remainingTimeMs: remaining
        };
    });

    res.json(result);
});

// ✅ Delete a timer
app.delete('/timer/delete/:key', (req, res) => {
    const { key } = req.params;
    if (timerStorage[key]?.timeoutId) {
        clearTimeout(timerStorage[key].timeoutId);
    }
    delete timerStorage[key];
    res.json({ success: true });
});

// ✅ Reset timer to default immediately
app.post('/timer/reset/:key', (req, res) => {
    const { key } = req.params;
    const timer = timerStorage[key];

    if (!timer) return res.status(404).json({ error: 'Key not found' });

    if (timer.timeoutId) {
        clearTimeout(timer.timeoutId);
        timer.timeoutId = null;
    }

    // Reset value to default
    timer.value = timer.defaultValue;
    timer.startTime = null;

    res.json({ success: true, message: 'Timer reset to default' });
});

const PORT = 3000;
app.listen(PORT, () => {
    console.log(`Server is running on http://localhost:${PORT}`);
});
