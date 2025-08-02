using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestApp
{
    public partial class COMV : UIPage
    {
        private RealCOMBridge comBridge;
        private bool isConnected = false;

        public COMV()
        {
            InitializeComponent();
            InitializeControls();
        }

        private void InitializeControls()
        {
            LoadCOMPorts();
            UpdateStatus("Chưa kết nối", Color.Red);
            progressBar.Value = 0;
        }

        private void LoadCOMPorts()
        {
            try
            {
                cmbVirtualPort.Items.Clear();
                cmbRealPort.Items.Clear();
                
                cmbVirtualPort.Items.Add("--- Chọn COM Port cho App 1 ---");
                cmbRealPort.Items.Add("--- Chọn COM Port cho App 2 ---");
                
                // Add available COM ports
                List<string> availablePorts = RealCOMBridge.GetAvailableCOMPorts();
                List<string> suggestedPorts = RealCOMBridge.GetSuggestedCOMPairs();
                List<string> virtualPorts = VirtualCOMManager.MemoryBasedCOM.GetAvailableVirtualPorts();

                // Add available ports first
                foreach (string port in availablePorts)
                {
                    cmbVirtualPort.Items.Add($"{port} (Available)");
                    cmbRealPort.Items.Add($"{port} (Available)");
                }

                // Add virtual memory-based ports
                foreach (string port in virtualPorts)
                {
                    cmbVirtualPort.Items.Add($"{port} (Virtual)");
                    cmbRealPort.Items.Add($"{port} (Virtual)");
                }

                // Add suggested ports if not already added
                foreach (string port in suggestedPorts)
                {
                    if (!availablePorts.Contains(port) && !virtualPorts.Contains(port))
                    {
                        cmbVirtualPort.Items.Add($"{port} (Suggested)");
                        cmbRealPort.Items.Add($"{port} (Suggested)");
                    }
                }
                
                if (cmbVirtualPort.Items.Count > 1)
                {
                    cmbVirtualPort.SelectedIndex = 1; // First available/suggested port
                    if (cmbRealPort.Items.Count > 2)
                        cmbRealPort.SelectedIndex = 2; // Second available/suggested port
                    else if (cmbRealPort.Items.Count > 1)
                        cmbRealPort.SelectedIndex = 1;
                }
                
                LogData($"[{DateTime.Now:HH:mm:ss}] 🚀 COM Bridge sẵn sàng!");
                LogData($"[{DateTime.Now:HH:mm:ss}] 📊 Available: {availablePorts.Count} ports: {string.Join(", ", availablePorts)}");
                LogData($"[{DateTime.Now:HH:mm:ss}] 💾 Virtual: {virtualPorts.Count} ports: {string.Join(", ", virtualPorts)}");
                
                if (availablePorts.Count == 0 && virtualPorts.Count == 0)
                {
                    LogData($"[{DateTime.Now:HH:mm:ss}] ⚠️ Không tìm thấy COM port nào!");
                    LogData($"[{DateTime.Now:HH:mm:ss}] 💡 Nhấn 'Tạo Virtual COM' để tạo ports tự động");
                    LogData($"[{DateTime.Now:HH:mm:ss}] 🔧 Hoặc nhấn 'Hướng dẫn' để xem cách cài com0com");
                }
                else
                {
                    LogData($"[{DateTime.Now:HH:mm:ss}] 💡 Chọn 2 COM port khác nhau và nhấn Kết nối");
                    if (virtualPorts.Count > 0)
                    {
                        LogData($"[{DateTime.Now:HH:mm:ss}] ✨ Virtual ports có sẵn - không cần com0com!");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Lỗi khi tải COM ports: " + ex.Message);
            }
        }

        private void COMV_Load(object sender, EventArgs e)
        {
            LoadCOMPorts();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (cmbVirtualPort.SelectedIndex <= 0 || cmbRealPort.SelectedIndex <= 0)
            {
                ShowErrorMessage("Vui lòng chọn COM Port cho cả 2 ứng dụng!");
                return;
            }

            string port1 = ExtractCOMPort(cmbVirtualPort.SelectedItem.ToString());
            string port2 = ExtractCOMPort(cmbRealPort.SelectedItem.ToString());

            if (port1 == port2)
            {
                ShowErrorMessage("Hai ứng dụng phải dùng COM port khác nhau!");
                return;
            }

            try
            {
                UpdateStatus("Đang tạo COM Bridge...", Color.Orange);
                progressBar.Value = 50;

                // Check if ports are virtual or real and create appropriate bridge
                bool isPort1Virtual = IsVirtualPort(port1);
                bool isPort2Virtual = IsVirtualPort(port2);

                if (isPort1Virtual || isPort2Virtual)
                {
                    LogData($"[{DateTime.Now:HH:mm:ss}] 🔍 Detected virtual ports: {port1}={isPort1Virtual}, {port2}={isPort2Virtual}");
                    
                    // Create hybrid bridge for virtual ports
                    if (CreateVirtualBridge(port1, port2))
                    {
                        isConnected = true;
                        progressBar.Value = 100;
                        
                        UpdateStatus($"✅ Virtual Bridge: {port1} ↔ {port2}", Color.Green);
                        UpdateUIOnConnect();
                        
                        LogData($"[{DateTime.Now:HH:mm:ss}] 🎯 Virtual COM Bridge được tạo thành công!");
                        LogData($"[{DateTime.Now:HH:mm:ss}] 💾 Sử dụng memory-based virtual ports");
                        LogData($"[{DateTime.Now:HH:mm:ss}] 🔄 Dữ liệu tự động chuyển tiếp 2 chiều!");
                        LogData($"[{DateTime.Now:HH:mm:ss}] 🧪 Test: Gửi data từ form hoặc external app");
                    }
                    else
                    {
                        throw new Exception("Không thể tạo Virtual Bridge");
                    }
                }
                else
                {
                    // Use real COM bridge for physical ports
                    comBridge = new RealCOMBridge();
                    comBridge.StatusChanged += COMBridge_StatusChanged;
                    comBridge.DataTransferred += COMBridge_DataTransferred;
                    
                    if (comBridge.CreateBridge(port1, port2))
                    {
                        isConnected = true;
                        progressBar.Value = 100;
                        
                        UpdateStatus($"✅ COM Bridge: {port1} ↔ {port2}", Color.Green);
                        UpdateUIOnConnect();
                        
                        LogData($"[{DateTime.Now:HH:mm:ss}] 🎯 Real COM Bridge được tạo thành công!");
                        LogData($"[{DateTime.Now:HH:mm:ss}] 📱 Hercules Serial: Connect đến {port1}");
                        LogData($"[{DateTime.Now:HH:mm:ss}] 📱 Hercules Serial: Connect đến {port2}");
                        LogData($"[{DateTime.Now:HH:mm:ss}] 🔄 Dữ liệu tự động chuyển tiếp 2 chiều!");
                        LogData($"[{DateTime.Now:HH:mm:ss}] 🧪 Test: Gõ text trong Hercules tab {port1} → sẽ thấy trong tab {port2}");
                    }
                    else
                    {
                        throw new Exception("Không thể tạo COM Bridge");
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateStatus("Lỗi: " + ex.Message, Color.Red);
                progressBar.Value = 0;
                ShowErrorMessage("Không thể tạo bridge: " + ex.Message);
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            DisconnectBridge();
        }

        private void DisconnectBridge()
        {
            try
            {
                isConnected = false;
                
                if (comBridge != null)
                {
                    // Real COM bridge
                    comBridge.CloseBridge();
                    comBridge.Dispose();
                    comBridge = null;
                }
                else
                {
                    // Virtual COM bridge - close virtual ports
                    string port1 = ExtractCOMPort(cmbVirtualPort.SelectedItem?.ToString() ?? "");
                    string port2 = ExtractCOMPort(cmbRealPort.SelectedItem?.ToString() ?? "");
                    
                    var vPort1 = VirtualCOMManager.MemoryBasedCOM.GetPort(port1);
                    var vPort2 = VirtualCOMManager.MemoryBasedCOM.GetPort(port2);
                    
                    vPort1?.Close();
                    vPort2?.Close();
                    
                    LogData($"[{DateTime.Now:HH:mm:ss}] 🔌 Virtual ports {port1}, {port2} closed");
                }
                
                UpdateStatus("Đã ngắt kết nối", Color.Red);
                progressBar.Value = 0;
                UpdateUIOnDisconnect();
                
                LogData($"[{DateTime.Now:HH:mm:ss}] 🔌 COM Bridge đã đóng");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Lỗi khi ngắt kết nối: " + ex.Message);
            }
        }

        private void btnSendData_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                ShowErrorMessage("Chưa kết nối COM Bridge!");
                return;
            }

            try
            {
                string data = txtDataToSend.Text;
                if (!string.IsNullOrEmpty(data))
                {
                    if (comBridge != null)
                    {
                        // Real COM bridge
                        comBridge.SendTestData(data, true);
                    }
                    else
                    {
                        // Virtual COM bridge - send to first virtual port
                        string port1 = ExtractCOMPort(cmbVirtualPort.SelectedItem.ToString());
                        var vPort = VirtualCOMManager.MemoryBasedCOM.GetPort(port1);
                        if (vPort != null && vPort.IsOpen)
                        {
                            byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(data + "\r\n");
                            vPort.Write(dataBytes);
                            LogData($"[{DateTime.Now:HH:mm:ss}] Test → {port1}: {data}");
                        }
                    }
                    txtDataToSend.Clear();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Lỗi khi gửi dữ liệu: " + ex.Message);
            }
        }

        private void txtDataToSend_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSendData_Click(sender, e);
                e.Handled = true;
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            txtDataReceived.Clear();
        }

        private void btnCreateVirtual_Click(object sender, EventArgs e)
        {
            // Try to create virtual COM ports without com0com dependency
            if (TryCreateVirtualCOMPorts())
            {
                string successMessage = @"🎉 VIRTUAL COM PORTS TẠO THÀNH CÔNG!

✅ ĐÃ TẠO:
• VCOM1 ↔ VCOM2 (Virtual COM pair)
• VCOM3 ↔ VCOM4 (Virtual COM pair)

📝 CÁCH SỬ DỤNG:
1. Refresh COM ports để thấy VCOM ports
2. Chọn VCOM1 và VCOM2 để tạo bridge
3. Mở Hercules SETUP → Serial
4. Connect đến VCOM1 và VCOM2

💡 LƯU Ý:
• Ports được tạo tự động bởi ứng dụng
• Không cần cài com0com
• Restart ứng dụng nếu cần";

                UIMessageBox.Show(successMessage, "Virtual COM Created", UIStyle.Green, UIMessageBoxButtons.OK);
                LoadCOMPorts(); // Refresh to show new ports
            }
            else
            {
                // Fallback to instructions
                ShowInstructions();
            }
        }

        private bool TryCreateVirtualCOMPorts()
        {
            try
            {
                LogData($"[{DateTime.Now:HH:mm:ss}] 🔄 Đang tạo Virtual COM ports...");

                // Check if running as administrator for registry approach
                if (VirtualCOMManager.IsAdministrator())
                {
                    LogData($"[{DateTime.Now:HH:mm:ss}] 🔑 Running as Administrator - trying registry approach");
                    
                    if (VirtualCOMManager.VirtualCOMRegistry.CreateVirtualCOMPair("VCOM1", "VCOM2"))
                    {
                        LogData($"[{DateTime.Now:HH:mm:ss}] ✅ VCOM1 ↔ VCOM2 created via registry");
                        
                        if (VirtualCOMManager.VirtualCOMRegistry.CreateVirtualCOMPair("VCOM3", "VCOM4"))
                        {
                            LogData($"[{DateTime.Now:HH:mm:ss}] ✅ VCOM3 ↔ VCOM4 created via registry");
                            return true;
                        }
                    }
                }
                else
                {
                    LogData($"[{DateTime.Now:HH:mm:ss}] ⚠️ Not Administrator - trying memory approach");
                }

                // Fallback to memory-based approach
                LogData($"[{DateTime.Now:HH:mm:ss}] 🔄 Trying memory-based virtual COM...");
                var createdPorts = VirtualCOMManager.CreateRegistrylessVirtualCOM(4);
                
                if (createdPorts.Count >= 4)
                {
                    LogData($"[{DateTime.Now:HH:mm:ss}] ✅ Created {createdPorts.Count} virtual COM ports");
                    LogData($"[{DateTime.Now:HH:mm:ss}] 📋 Ports: {string.Join(", ", createdPorts)}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                LogData($"[{DateTime.Now:HH:mm:ss}] ❌ Error creating virtual COM: {ex.Message}");
                return false;
            }
        }

        private void ShowInstructions()
        {
            string message = @"🔧 COM BRIDGE - HƯỚNG DẪN

✅ CÁCH SỬ DỤNG:
1. Cài đặt com0com để tạo virtual COM ports
2. Chọn 2 COM port khác nhau (VD: COM1 và COM2)
3. Nhấn 'Kết nối' để tạo bridge
4. Mở Hercules SETUP → Serial

📝 HERCULES SETUP:
• Tab 1: Serial → COM1, 9600, 8, N, 1
• Tab 2: Serial → COM2, 9600, 8, N, 1
• Open cả 2 tabs
• Test: Gõ text trong tab COM1 → thấy trong tab COM2!

📥 CÀI ĐẶT COM0COM:
• Download: sourceforge.net/projects/com0com
• Cài đặt và tạo pairs: COM1-COM2, COM3-COM4
• Restart ứng dụng để detect ports

💡 VIRTUAL COM (KHÔNG CẦN COM0COM):
• Nhấn 'Tạo Virtual COM' để thử tạo tự động
• Cần quyền Administrator cho registry approach
• Hoặc sử dụng memory-based ports

🎯 TEST NHANH:
• Có sẵn COM ports → Chọn và Connect
• Chưa có → Cài com0com hoặc dùng Virtual COM";

            UIMessageBox.Show(message, "Hướng dẫn COM Bridge", UIStyle.Blue, UIMessageBoxButtons.OK);
        }

        private void btnRefreshPorts_Click(object sender, EventArgs e)
        {
            LoadCOMPorts();
            LogData($"[{DateTime.Now:HH:mm:ss}] 🔄 Đã refresh COM ports");
        }

        private string ExtractCOMPort(string displayText)
        {
            if (string.IsNullOrEmpty(displayText) || displayText.StartsWith("---"))
                return "";
            
            // Extract "COM1" from "COM1 (Available)" or "COM1 (Suggested)"
            int spaceIndex = displayText.IndexOf(' ');
            if (spaceIndex > 0)
                return displayText.Substring(0, spaceIndex);
            
            return displayText;
        }

        private bool IsVirtualPort(string portName)
        {
            // Check if port is in virtual memory-based ports
            var virtualPorts = VirtualCOMManager.MemoryBasedCOM.GetAvailableVirtualPorts();
            return virtualPorts.Contains(portName);
        }

        private bool CreateVirtualBridge(string port1, string port2)
        {
            try
            {
                // Get virtual ports
                var vPort1 = VirtualCOMManager.MemoryBasedCOM.GetPort(port1);
                var vPort2 = VirtualCOMManager.MemoryBasedCOM.GetPort(port2);

                if (vPort1 == null || vPort2 == null)
                {
                    LogData($"[{DateTime.Now:HH:mm:ss}] ❌ Virtual ports not found: {port1}, {port2}");
                    return false;
                }

                // Open virtual ports
                if (!vPort1.Open() || !vPort2.Open())
                {
                    LogData($"[{DateTime.Now:HH:mm:ss}] ❌ Failed to open virtual ports");
                    return false;
                }

                // Set up data forwarding events
                vPort1.DataReceived += (sender, data) =>
                {
                    if (vPort2.IsOpen)
                    {
                        vPort2.Write(data);
                        string text = System.Text.Encoding.UTF8.GetString(data);
                        LogData($"[{DateTime.Now:HH:mm:ss}] {port1} → {port2}: {text.Trim()}");
                    }
                };

                vPort2.DataReceived += (sender, data) =>
                {
                    if (vPort1.IsOpen)
                    {
                        vPort1.Write(data);
                        string text = System.Text.Encoding.UTF8.GetString(data);
                        LogData($"[{DateTime.Now:HH:mm:ss}] {port2} → {port1}: {text.Trim()}");
                    }
                };

                LogData($"[{DateTime.Now:HH:mm:ss}] ✅ Virtual bridge events configured");
                return true;
            }
            catch (Exception ex)
            {
                LogData($"[{DateTime.Now:HH:mm:ss}] ❌ Error creating virtual bridge: {ex.Message}");
                return false;
            }
        }

        private void COMBridge_StatusChanged(object sender, string status)
        {
            LogData(status);
        }

        private void COMBridge_DataTransferred(object sender, string data)
        {
            LogData(data);
        }

        private void UpdateStatus(string message, Color color)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateStatus(message, color)));
                return;
            }
            
            lblStatus.Text = "Trạng thái: " + message;
            lblStatus.ForeColor = color;
        }

        private void UpdateUIOnConnect()
        {
            btnConnect.Enabled = false;
            btnDisconnect.Enabled = true;
            btnSendData.Enabled = true;
            cmbVirtualPort.Enabled = false;
            cmbRealPort.Enabled = false;
        }

        private void UpdateUIOnDisconnect()
        {
            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
            btnSendData.Enabled = false;
            cmbVirtualPort.Enabled = true;
            cmbRealPort.Enabled = true;
        }

        private void LogData(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => LogData(message)));
                return;
            }
            
            txtDataReceived.AppendText(message + Environment.NewLine);
            txtDataReceived.ScrollToCaret();
        }

        private void ShowErrorMessage(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowErrorMessage(message)));
                return;
            }
            
            UIMessageBox.ShowError(message);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (isConnected)
            {
                DisconnectBridge();
            }
            base.OnFormClosing(e);
        }
    }
}