const express = require('express');
const app = express();
const PORT = 51239;

const { DateTime } = require('luxon'); // Dùng luxon để xử lý múi giờ

app.get('/get-time', (req, res) => {
    const tz = 'Asia/Ho_Chi_Minh';
    const now = DateTime.now().setZone(tz); // Lấy giờ theo múi giờ VN

    const result = {
        isoTime: now.toUTC().toISO(),         // Giờ UTC chuẩn ISO 8601
        timezone: tz,                         // Tên múi giờ
        localTime: now.toFormat("yyyy-MM-dd HH:mm:ss"), // Giờ local readable
        utcOffset: now.toFormat("ZZ")         // Độ lệch UTC, ví dụ: +07:00
    };

    res.json(result);
    console.log(`✅ Sent time info: ${JSON.stringify(result)}`);
});

app.listen(PORT, () => {
    console.log(`🚀 Time Service running at http://localhost:${PORT}/get-time`);
});
