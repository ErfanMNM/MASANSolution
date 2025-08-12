const { app, BrowserWindow, Tray, Menu } = require('electron');
const path = require('path');
require('./server'); // Khởi động server Express
require('./PO_Service_JSON') //khởi động máy chủ PO

let mainWindow;
let tray;

app.whenReady().then(() => {
    mainWindow = new BrowserWindow({
        width: 800,
        height: 600,
        webPreferences: {
            preload: path.join(__dirname, 'preload.js'),
            nodeIntegration: false,
            contextIsolation: true,
        },
    });

    mainWindow.loadURL('http://localhost:49212/api-docs');

    // Xử lý khi nhấn nút đóng cửa sổ
    mainWindow.on('close', (event) => {
        event.preventDefault();
        mainWindow.hide();
    });

    // Tạo biểu tượng khay hệ thống
    tray = new Tray(path.join(__dirname, 'icon.png')); // Đảm bảo có file icon.png trong thư mục
    const contextMenu = Menu.buildFromTemplate([
        { label: 'Mở giao diện', click: () => mainWindow.show() },
        { label: 'Thoát', click: () => {
            tray.destroy();
            app.quit();
        }}
    ]);

    tray.setToolTip('TANTIEN VISION');
    tray.setContextMenu(contextMenu);

    // Click vào icon trên khay để mở lại giao diện
    tray.on('click', () => mainWindow.show());
});

app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') {
        app.quit();
    }
});
