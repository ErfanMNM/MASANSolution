// File: time_server_http.js
const express = require('express');
const app = express();
const PORT = 9000;

app.get('/get-time', (req, res) => {
    const now = new Date();
    const localTime = now.toLocaleString();
    res.send(localTime);
    console.log(`Sent time: ${localTime}`);
});

app.listen(PORT, () => {
    console.log(`✅ HTTP Time Service running at http://localhost:${PORT}/get-time`);
});
