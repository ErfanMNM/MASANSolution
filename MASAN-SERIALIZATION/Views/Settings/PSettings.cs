using MASAN_SERIALIZATION.Configs;
using MASAN_SERIALIZATION.Diaglogs;
using SpT.Auth;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace MASAN_SERIALIZATION.Views.Settings
{
    public partial class PSettings : UIPage
    {
        private Dictionary<string, Control> _configControls = new Dictionary<string, Control>();
        private Dictionary<string, PropertyInfo> _configProperties = new Dictionary<string, PropertyInfo>();
        
        public PSettings()
        {
            InitializeComponent();
        }

        public void INIT()
        {
            try
            {
                GenerateConfigControls();
                LoadCurrentConfig();
            }
            catch (Exception ex)
            {
                this.ShowErrorTip($"Lỗi khởi tạo trang cài đặt: {ex.Message}");
            }
        }

        private void GenerateConfigControls()
        {
            var configType = typeof(AppConfigs);
            var properties = configType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && 
                           !p.Name.Equals("Current", StringComparison.OrdinalIgnoreCase) &&
                           p.DeclaringType == configType)
                .ToList();

            // Clear existing dynamic controls
            tabPageDynamic.Controls.Clear();
            _configControls.Clear();
            _configProperties.Clear();

            // Group properties by category
            var categories = GroupPropertiesByCategory(properties);
            
            int yPos = 20;
            int groupSpacing = 15;

            foreach (var category in categories)
            {
                // Create category group box
                var groupBox = new UIGroupBox()
                {
                    Text = category.Key,
                    Location = new Point(20, yPos),
                    Size = new Size(740, (category.Value.Count * 50) + 60),
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    FillColor = Color.FromArgb(255, 255, 255),
                    RectColor = Color.FromArgb(189, 195, 199),
                    Radius = 8,
                    RectSize = 1
                };
                tabPageDynamic.Controls.Add(groupBox);

                int itemYPos = 35;
                
                foreach (var property in category.Value)
                {
                    _configProperties[property.Name] = property;
                    
                    // Create modern card-like container
                    var itemPanel = new UIPanel()
                    {
                        Location = new Point(15, itemYPos),
                        Size = new Size(700, 40),
                        FillColor = Color.White,
                        RectColor = Color.FromArgb(224, 230, 237),
                        Radius = 12,
                        RectSize = 1
                    };
                    groupBox.Controls.Add(itemPanel);

                    // Create label with icon
                    var label = new UILabel()
                    {
                        Text = GetDisplayName(property.Name),
                        Location = new Point(15, 8),
                        Size = new Size(300, 24),
                        Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                        ForeColor = Color.FromArgb(52, 73, 94),
                        TextAlign = ContentAlignment.MiddleLeft
                    };
                    itemPanel.Controls.Add(label);

                    // Create control based on property type
                    Control control = CreateControlForProperty(property);
                    if (control != null)
                    {
                        control.Location = new Point(480, 5);
                        control.Size = GetControlSize(property.PropertyType);
                        control.Font = new Font("Tahoma", 10F);
                        control.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                        
                        itemPanel.Controls.Add(control);
                        _configControls[property.Name] = control;
                    }

                    itemYPos += 45;
                }
                
                yPos += groupBox.Height + groupSpacing;
            }
        }

        private Dictionary<string, List<PropertyInfo>> GroupPropertiesByCategory(List<PropertyInfo> properties)
        {
            var categories = new Dictionary<string, List<PropertyInfo>>();
            
            foreach (var property in properties)
            {
                string category = GetPropertyCategory(property.Name);
                if (!categories.ContainsKey(category))
                {
                    categories[category] = new List<PropertyInfo>();
                }
                categories[category].Add(property);
            }
            
            return categories;
        }
        
        private string GetPropertyCategory(string propertyName)
        {
            if (propertyName.Contains("TwoFA"))
                return "🔐 Bảo mật";
            if (propertyName.Contains("APP"))
                return "⚙️ Cấu hình ứng dụng";
            if (propertyName.Contains("Camera"))
                return "📹 Camera";
            if (propertyName.Contains("HandScan") || propertyName.Contains("COM"))
                return "🔌 Phần cứng";
            if (propertyName.Contains("AWS") || propertyName.Contains("host") || propertyName.Contains("CA") || propertyName.Contains("pfx") || propertyName.Contains("client"))
                return "☁️ AWS Cloud";
            if (propertyName.Contains("carton"))
                return "📦 Carton";
            return "⚙️ Cài đặt chung";
        }
        
        // UINumPadTextBox đã có sẵn numpad dialog khi double-click, không cần method riêng
        
        private Size GetControlSize(Type propertyType)
        {
            if (propertyType == typeof(bool))
                return new Size(60, 30);
            else if (propertyType == typeof(int))
                return new Size(120, 30);
            else
                return new Size(200, 30);
        }

        private Control CreateControlForProperty(PropertyInfo property)
        {
            var propertyType = property.PropertyType;

            if (propertyType == typeof(bool))
            {
                var uiSwitch = new UISwitch() 
                { 
                    Name = $"sw_{property.Name}",
                    ActiveText = "Bật",
                    InActiveText = "Tắt",
                    Size = new Size(60, 30)
                };
                return uiSwitch;
            }
            else if (propertyType == typeof(int))
            {
                var numPadTextBox = new UINumPadTextBox() 
                { 
                    Name = $"numpad_{property.Name}",
                    FillColor = Color.White,
                    RectColor = Color.FromArgb(189, 195, 199),
                    Radius = 8,
                    Font = new Font("Segoe UI", 10F),
                    //TextAlign = HorizontalAlignment.Center,
                    //HasMaximum = true,
                    Maximum = property.Name.ToLower().Contains("port") ? 65535 : int.MaxValue,
                    //HasMinimum = true,
                    Minimum = 0,
                    Watermark = "2-click: numpad | Ctrl+2-click: keyboard"
                };
                
                // UINumPadTextBox tự động có numpad dialog khi double click
                // Thêm thêm option cho bàn phím chữ bằng Ctrl+Double Click
                numPadTextBox.MouseDoubleClick += (s, e) => {
                    if (Control.ModifierKeys == Keys.Control)
                    {
                        ShowVirtualKeyboard(numPadTextBox, property.Name);
                    }
                };
                
                return numPadTextBox;
            }
            else if (propertyType == typeof(string))
            {
                var textBox = new UITextBox() 
                { 
                    Name = $"txt_{property.Name}",
                    FillColor = Color.White,
                    RectColor = Color.FromArgb(189, 195, 199),
                    Radius = 8,
                    Font = new Font("Segoe UI", 10F)
                };
                
                // Thêm double-click event để hiện bàn phím ảo
                textBox.DoubleClick += (s, e) => ShowVirtualKeyboard(textBox, property.Name);
                
                // Thêm tooltip để hướng dẫn người dùng
                textBox.Watermark = "Double-click để mở bàn phím ảo";
                
                // Special handling for password fields
                if (property.Name.ToLower().Contains("password"))
                {
                    textBox.PasswordChar = '●';
                }
                
                // Special handling for path fields
                if (property.Name.ToLower().Contains("path"))
                {
                    textBox.ReadOnly = true;
                    textBox.BackColor = Color.FromArgb(248, 248, 248);
                    
                    // Add browse button
                    var browseBtn = new UIButton()
                    {
                        Text = "📁",
                        Size = new Size(30, 30),
                        Location = new Point(170, 0),
                        Font = new Font("Segoe UI", 10F),
                        Radius = 8,
                        FillColor = Color.FromArgb(108, 117, 125),
                        FillHoverColor = Color.FromArgb(134, 142, 150),
                        FillPressColor = Color.FromArgb(73, 80, 87),
                        RectSize = 0,
                        ForeColor = Color.White
                    };
                    
                    string propName = property.Name;
                    browseBtn.Click += (s, e) => BrowseForFile(textBox, propName);
                    
                    // Path textbox cũng có thể dùng bàn phím ảo cho việc edit
                    textBox.DoubleClick += (s, e) => ShowVirtualKeyboard(textBox, propName);
                    
                    var container = new Panel()
                    {
                        Size = new Size(200, 30)
                    };
                    textBox.Size = new Size(165, 30);
                    container.Controls.Add(textBox);
                    container.Controls.Add(browseBtn);
                    
                    return container;
                }
                
                return textBox;
            }

            return null;
        }

        private string GetDisplayName(string propertyName)
        {
            // Convert property names to user-friendly display names
            var displayNames = new Dictionary<string, string>()
            {
                { "TwoFA_Enabled", "Xác thực hai yếu tố" },
                { "Camera_Main_IP", "IP Camera chính" },
                { "Camera_Sub_IP", "IP Camera phụ" },
                { "Camera_Main_Port", "Cổng Camera chính" },
                { "Camera_Sub_Port", "Cổng Camera phụ" },
                { "HandScanCOM01", "HandScan COM01" },
                { "HandScanCOM02", "HandScan COM02" },
                { "AWS_ENA", "Bật AWS" },
                { "host", "Host AWS" },
                { "rootCAPath", "Đường dẫn Root CA" },
                { "pfxPath", "Đường dẫn PFX" },
                { "pfxPassword", "Mật khẩu PFX" },
                { "clientId", "Client ID" },
                { "cartonPack", "Số sản phẩm 1 thùng" },
                { "cartonOfset", "Carton Offset" },
                { "Auto_Send_AWS", "Tự động gửi AWS" },
                { "APP_Mode", "Chế độ ứng dụng" },
                { "cartonAutoStart", "Chỉ quét thùng 1 lần" }
            };

            return displayNames.ContainsKey(propertyName) ? displayNames[propertyName] : propertyName;
        }

        private void LoadCurrentConfig()
        {
            var config = AppConfigs.Current;
            
            foreach (var kvp in _configControls)
            {
                var propertyName = kvp.Key;
                var control = kvp.Value;
                var property = _configProperties[propertyName];
                
                try
                {
                    var value = property.GetValue(config);
                    SetControlValue(control, value);
                }
                catch (Exception ex)
                {
                    // Log error but continue
                    System.Diagnostics.Debug.WriteLine($"Error loading value for {propertyName}: {ex.Message}");
                }
            }
        }
        
        private void BrowseForFile(UITextBox textBox, string propertyName)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                if (propertyName.ToLower().Contains("pem") || propertyName.ToLower().Contains("rootca"))
                {
                    openFileDialog.Filter = "PEM files (*.pem)|*.pem|All files (*.*)|*.*";
                    openFileDialog.Title = "Chọn file Root CA";
                }
                else if (propertyName.ToLower().Contains("pfx"))
                {
                    openFileDialog.Filter = "PFX files (*.pfx)|*.pfx|All files (*.*)|*.*";
                    openFileDialog.Title = "Chọn file Client Certificate";
                }
                else
                {
                    openFileDialog.Filter = "All files (*.*)|*.*";
                    openFileDialog.Title = "Chọn file";
                }
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox.Text = openFileDialog.FileName;
                }
            }
        }
        
        private void SetControlValue(Control control, object value)
        {
            if (control is UISwitch uiSwitch && value is bool boolValue)
            {
                uiSwitch.Active = boolValue;
            }
            else if (control is UINumPadTextBox numPadTextBox && value is int intValue)
            {
                numPadTextBox.Text = intValue.ToString();
            }
            else if (control is UITextBox numTextBox && value is int intValue2 && numTextBox.Name.StartsWith("txt_") && !numTextBox.Name.Contains("password") && !numTextBox.Name.Contains("path") && !numTextBox.Name.Contains("host") && !numTextBox.Name.Contains("client") && !numTextBox.Name.Contains("CA") && !numTextBox.Name.Contains("COM"))
            {
                numTextBox.Text = intValue2.ToString();
            }
            else if (control is UITextBox textBox && value is string stringValue)
            {
                textBox.Text = stringValue ?? string.Empty;
            }
            else if (control is Panel panel && value is string stringValue2)
            {
                // Handle path controls with browse button
                var textBoxInPanel = panel.Controls.OfType<UITextBox>().FirstOrDefault();
                if (textBoxInPanel != null)
                {
                    textBoxInPanel.Text = stringValue2 ?? string.Empty;
                }
            }
        }
        
        private object GetControlValue(Control control, Type targetType)
        {
            if (control is UISwitch uiSwitch && targetType == typeof(bool))
            {
                return uiSwitch.Active;
            }
            else if (control is UINumPadTextBox numPadTextBox && targetType == typeof(int))
            {
                if (int.TryParse(numPadTextBox.Text, out int result))
                    return result;
                return 0;
            }
            else if (control is UITextBox numTextBox && targetType == typeof(int) && numTextBox.Name.StartsWith("txt_") && !numTextBox.Name.Contains("password") && !numTextBox.Name.Contains("path") && !numTextBox.Name.Contains("host") && !numTextBox.Name.Contains("client") && !numTextBox.Name.Contains("CA") && !numTextBox.Name.Contains("COM"))
            {
                if (int.TryParse(numTextBox.Text, out int result))
                    return result;
                return 0;
            }
            else if (control is UITextBox textBox && targetType == typeof(string))
            {
                return textBox.Text;
            }
            else if (control is Panel panel && targetType == typeof(string))
            {
                // Handle path controls with browse button
                var textBoxInPanel = panel.Controls.OfType<UITextBox>().FirstOrDefault();
                return textBoxInPanel?.Text ?? string.Empty;
            }
            
            return null;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var config = AppConfigs.Current;
                
                // Cập nhật config từ UI động
                foreach (var kvp in _configControls)
                {
                    var propertyName = kvp.Key;
                    var control = kvp.Value;
                    var property = _configProperties[propertyName];
                    
                    try
                    {
                        var value = GetControlValue(control, property.PropertyType);
                        if (value != null)
                        {
                            property.SetValue(config, value);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.ShowErrorTip($"Lỗi cập nhật {propertyName}: {ex.Message}");
                        return;
                    }
                }

                // Lưu config
                config.Save();
                
                this.ShowSuccessTip("Cài đặt đã được lưu thành công!");
            }
            catch (Exception ex)
            {
                this.ShowErrorTip($"Lỗi lưu cài đặt: {ex.Message}");
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
             var result = this.ShowAskDialog("Bạn có chắc muốn khôi phục cài đặt mặc định không?");
            if (result)
            {
                try
                {
                    AppConfigs.Current.SetDefault();
                    LoadCurrentConfig();
                    this.ShowSuccessTip("Đã khôi phục cài đặt mặc định!");
                }
                catch (Exception ex)
                {
                    this.ShowErrorTip($"Lỗi khôi phục cài đặt: {ex.Message}");
                }
            }
        }

        private void ShowVirtualKeyboard(Control textControl, string propertyName)
        {
            try
            {
                var displayName = GetDisplayName(propertyName);
                var isPassword = propertyName.ToLower().Contains("password");
                
                string currentText = "";
                if (textControl is UITextBox textBox)
                {
                    currentText = textBox.Text;
                }
                else if (textControl is UINumPadTextBox numPadBox)
                {
                    currentText = numPadBox.Text;
                }
                
                var keyboard = new Entertext()
                {
                    TileText = $"Nhập giá trị cho {displayName}",
                    TextValue = currentText,
                    IsPassword = isPassword
                };
                
                if (keyboard.ShowDialog() == DialogResult.OK)
                {
                    if (textControl is UITextBox tb)
                    {
                        // Nếu là path field và readonly, cần bỏ readonly tạm thời để update
                        if (tb.ReadOnly)
                        {
                            tb.ReadOnly = false;
                            tb.Text = keyboard.TextValue;
                            tb.ReadOnly = true;
                        }
                        else
                        {
                            tb.Text = keyboard.TextValue;
                        }
                    }
                    else if (textControl is UINumPadTextBox npb)
                    {
                        npb.Text = keyboard.TextValue;
                    }
                }
            }
            catch (Exception ex)
            {
                this.ShowErrorTip($"Lỗi hiện thị bàn phím: {ex.Message}");
            }
        }
        
        private void PSettings_Initialize(object sender, EventArgs e)
        {
            uc_UserSetting1.CurrentUserName = Globals.CurrentUser.Username; // Thiết lập tên người dùng hiện tại
            uc_UserSetting1.INIT(); // Khởi tạo thông tin người dùng
            uc_UserManager1.CurrentUserName = Globals.CurrentUser.Username; // Thiết lập tên người dùng hiện tại
            if (Globals.CurrentUser.Role == "Admin")
            {
                uc_UserManager1.Enabled = true; // Hiển thị quản lý người dùng nếu là Admin
            }
            else
            {
                uc_UserManager1.Enabled = false; // Ẩn quản lý người dùng nếu không phải Admin
            }
        }

        private void uc_UserSetting1_OnUserAction(object sender, LoginActionEventArgs e)
        {
            this.ShowInfoNotifier($"{e.Message}");
        }

        private void uc_UserManager1_OnAction(object sender, LoginActionEventArgs e)
        {
            this.ShowInfoNotifier($"{e.Message}"); // Hiển thị thông báo khi có hành động từ quản lý người dùng
        }
    }
}