const express = require('express');
const cors = require('cors');

const app = express();
app.use(cors());
app.use(express.json());

let tempStorage = {};

// API set key bằng GET
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
// API lấy tất cả các key
app.get('/keys', (req, res) => {
    res.json({ keys: Object.keys(tempStorage) });
});

//máy in/////
//////////////////////////
// Đảm bảo key Printer_Status tồn tại
// Đảm bảo các biến luôn tồn tại
if (!tempStorage['Printer_Status']) tempStorage['Printer_Status'] = '-1';
if (!tempStorage['Printer_Command']) tempStorage['Printer_Command'] = '0';

// API lấy trạng thái máy in (do máy in trả về)
app.get('/printer/status', (req, res) => {
    res.json({ status: tempStorage['Printer_Status'] });
});

// API lấy cờ tác động
app.get('/printer/command', (req, res) => {
    res.json({ command: tempStorage['Printer_Command'] });
});

// Phục vụ giao diện máy in
app.get('/mayin', (req, res) => {
    res.sendFile(__dirname + '/public/mayin.html');
});

// API đổi trạng thái máy in từ giao diện
app.get('/printer/toggle', (req, res) => {
    tempStorage['Printer_Command'] = tempStorage['Printer_Command'] === "PRINTTING" ? "STOPPED" : "PRINTTING";
    tempStorage['Printer_Status'] = "-2"; // Chờ máy in phản hồi
    console.log("Waiting for printer response...");
    res.json({ success: true });
});

// API máy in gửi trạng thái và nhận cờ tác động
let printerTimeout;
// Máy in gửi trạng thái lên server
app.get('/printer/update/:status', (req, res) => {
    const { status } = req.params;

    // Nếu đang chờ phản hồi (-2) và có phản hồi, reset cờ tác động
    if (tempStorage['Printer_Status'] === "-2" && status !== "-2") {
        tempStorage['Printer_Command'] = 0; // Xóa cờ
        console.log("Printer responded. Resetting command.");
    }

    tempStorage['Printer_Status'] = status;

    // Xóa timeout cũ nếu có
    if (printerTimeout) clearTimeout(printerTimeout);

    // Nếu không có phản hồi trong 60 giây => mất kết nối
    printerTimeout = setTimeout(() => {
        tempStorage['Printer_Status'] = "-1";  // Server mất kết nối với máy in
        console.log("Printer lost connection (status = -1)");
    }, 60000);

    res.json({ command: tempStorage['Printer_Command'] });
});


const PORT = 3000;
app.listen(PORT, () => {
    console.log(`Server is running on http://localhost:${PORT}`);
});
