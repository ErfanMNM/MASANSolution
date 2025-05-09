const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('electronAPI', {
    setTemp: (key, value) => ipcRenderer.invoke('set-temp', key, value),
    getTemp: (key) => ipcRenderer.invoke('get-temp', key),
    deleteTemp: (key) => ipcRenderer.invoke('delete-temp', key),
});
