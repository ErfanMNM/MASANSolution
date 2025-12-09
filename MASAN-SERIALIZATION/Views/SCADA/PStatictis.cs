using MASAN_SERIALIZATION.Production;
using MASAN_SERIALIZATION.Utils;
using Newtonsoft.Json;
using SpT.Static;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using static MASAN_SERIALIZATION.Production.ProductionOrder;

namespace MASAN_SERIALIZATION.Views.SCADA
{
    public partial class PStatictis : UIPage
    {
        Dictionary<PropertyInfo, panelS> bindings = new Dictionary<PropertyInfo, panelS>();
        Dictionary<string, panelS> extraBindings = new Dictionary<string, panelS>();
        Dictionary<PropertyInfo, panelS> globalsBindings = new Dictionary<PropertyInfo, panelS>();
        Dictionary<PropertyInfo, panelS> globalsDatabaseBindings = new Dictionary<PropertyInfo, panelS>();
        
        // Dictionary để map tên kỹ thuật thành tên dễ đọc
        Dictionary<string, string> friendlyNames = new Dictionary<string, string>()
        {
            // Queue Counts
            {"Dictionary Count", "Số mã RAM"},
            {"DCarton Count", "Số thùng RAM"},
            {"Update Product Queue Count", "Hàng chờ cập nhật"},
            {"Insert Record Queue Count", "📝 Hàng đợi ghi bản ghi"},
            {"Insert Record CS Queue Count", "📝 Hàng đợi ghi CS"},
            {"Update Carton Queue Count", "📦 Hàng đợi cập nhật thùng"},
            {"Activate Carton Queue Count", "✅ Hàng đợi kích hoạt thùng"},
            {"AWS Receive Queue Count", "☁️ Hàng đợi nhận AWS"},
            {"AWS Send Queue Count", "☁️ Hàng đợi gửi AWS"},
            
            // CurrentUser
            {"CurrentUser.UserName", "👤 Tên người dùng"},
            {"CurrentUser.FullName", "👤 Họ tên đầy đủ"},
            {"CurrentUser.Role", "🔐 Vai trò"},
            {"CurrentUser.Department", "🏢 Phòng ban"},
            {"CurrentUser.IsActive", "✅ Trạng thái hoạt động"},
            {"CurrentUser.LastLogin", "🕐 Lần đăng nhập cuối"},
            
            // PLCCounter
            {"PLCCounter.total", "🔢 Tổng sản phẩm"},
            {"PLCCounter.total_pass", "✅ SP đạt"},
            {"PLCCounter.total_failed", "❌ SP lỗi"},
            {"PLCCounter.camera_read_fail", "📷 Lỗi đọc camera"},
            
            // ProductionData
            {"ProductionData.orderNo", "📋 Số đơn hàng"},
            {"ProductionData.site", "🏭 Địa điểm"},
            {"ProductionData.factory", "🏗️ Nhà máy"},
            {"ProductionData.productionLine", "⚙️ Dây chuyền SX"},
            {"ProductionData.productionDate", "📅 Ngày sản xuất"},
            {"ProductionData.shift", "⏰ Ca làm việc"},
            {"ProductionData.orderQty", "📊 Số lượng đặt hàng"},
            {"ProductionData.lotNumber", "🔖 Số lô"},
            {"ProductionData.productCode", "🏷️ Mã sản phẩm"},
            {"ProductionData.productName", "📦 Tên sản phẩm"},
            {"ProductionData.gtin", "🔢 Mã GTIN"},
            {"ProductionData.customerOrderNo", "👥 Đơn hàng khách"},
            {"ProductionData.uom", "📏 Đơn vị tính"},
            {"ProductionData.cartonSize", "📦 Kích thước thùng"},
            {"ProductionData.totalCZCode", "🔢 Tổng mã CZ"},
            
            // AWS Counters
            {"AWS_Send.pendingCount", "⏳ AWS - Chờ gửi"},
            {"AWS_Send.sentCount", "✅ AWS - Đã gửi"},
            {"AWS_Send.failedCount", "❌ AWS - Gửi lỗi"},
            {"AWS_Recived.waitingCount", "⏳ AWS - Chờ nhận"},
            {"AWS_Recived.recivedCount", "✅ AWS - Đã nhận"},
            
            // Camera HMI
            {"CameraMain_HMI.Camera_Content", "📷 Nội dung Camera chính"},
            {"CameraMain_HMI.Camera_Status", "📷 Trạng thái Camera chính"},
            {"CameraMain_HMI.ID", "🆔 ID Camera chính"},
            {"CameraSub_HMI.Camera_Content", "📷 Nội dung Camera phụ"},
            {"CameraSub_HMI.Camera_Status", "📷 Trạng thái Camera phụ"},
            {"CameraSub_HMI.ID", "🆔 ID Camera phụ"},
            
            // Counter (Product_Counter)
            {"Counter.passCount", "✅ Số lượng đạt"},
            {"Counter.failCount", "❌ Số lượng lỗi"},
            {"Counter.duplicateCount", "🔄 Số lượng trùng lặp"},
            {"Counter.readfailCount", "📖 Lỗi đọc"},
            {"Counter.notfoundCount", "🔍 Không tìm thấy"},
            {"Counter.errorCount", "⚠️ Số lỗi"},
            {"Counter.totalCount", "🔢 Tổng số lượng"},
            {"Counter.totalCartonCount", "📦 Tổng số thùng"},
            {"Counter.activatedCartonCount", "✅ Thùng đã kích hoạt"},
            {"Counter.errorCartonCount", "❌ Thùng lỗi"},
            {"Counter.cartonID", "🆔 ID thùng hiện tại"},
            {"Counter.carton_Packing_Code", "📦 Mã thùng đóng gói"},
            {"Counter.carton_Packing_ID", "🆔 ID thùng đóng gói"},
            {"Counter.carton_Packing_Count", "📊 Số lượng đóng gói"},
            
            // Globals simple properties
            {"Globals.AppState", "🖥️ Trạng thái ứng dụng"},
            {"Globals.AppRenderState", "🎨 Trạng thái giao diện"},
            {"Globals.ACTIVE_State", "⚡ Trạng thái hoạt động"},
            {"Globals.APP_Ready", "✅ Ứng dụng sẵn sàng"},
            {"Globals.Device_Ready", "🔧 Thiết bị sẵn sàng"},
            {"Globals.CameraMain_State", "📷 Trạng thái Camera chính"},
            {"Globals.CameraSub_State", "📷 Trạng thái Camera phụ"},
            {"Globals.HandScan01_Connected", "🔌 Kết nối HandScan 01"},
            {"Globals.HandScan02_Connected", "🔌 Kết nối HandScan 02"},
            {"Globals.PLC_Connected", "🔌 Kết nối PLC"},
            {"Globals.Production_State", "⚙️ Trạng thái sản xuất"},
            {"Globals.Canhbao", "⚠️ Cảnh báo"},
            {"Globals.test", "🧪 Test flag"},
            {"Globals.test2", "🧪 Test counter"},
            {"Globals.AWS_IoT_Status", "☁️ Trạng thái AWS IoT"}
        };

        private string orderNo { get; set; } = string.Empty; // Biến để lưu số đơn hàng hiện tại


        private BackgroundWorker WK_Update = new BackgroundWorker()
        {
            WorkerSupportsCancellation = true
            
        };
        public PStatictis()
        {
            InitializeComponent();
        }

        // Helper method để lấy tên dễ đọc
        private string GetFriendlyName(string technicalName)
        {
            return friendlyNames.ContainsKey(technicalName) ? friendlyNames[technicalName] : technicalName;
        }

        #region sự kiện page

        public void INIT()
        {
            try
            {
                Render_MEM();
                WK_Update.DoWork += WK_Update_DoWork;
                //webView23.Source = new Uri(@"C:\test\b3.html");
            }
            catch (Exception ex)
            {
                this.ShowErrorNotifier($"Lỗi khởi tạo PStatictis: {ex.Message}");
            }
                


        }

        public static void InjectOptionToHtml(string templatePath, string outputPath, object option)
        {
            // Đọc template HTML
            string html = File.ReadAllText(templatePath, Encoding.UTF8);

            // Serialize object option thành JSON
            string json = JsonConvert.SerializeObject(option, Formatting.Indented);

            // Escape JSON để đảm bảo không bị lỗi script
            string safeJson = json.Replace("</script>", "</scr\" + \"ipt>");

            // Inject JSON vào placeholder
            html = html.Replace("{{ECHARTS_OPTION}}", safeJson);

            // Ghi ra file output
            File.WriteAllText(outputPath, html, Encoding.UTF8);
        }

        private void PStatictis_Initialize(object sender, EventArgs e)
        {
            if (!WK_Update.IsBusy)
            {
                WK_Update.RunWorkerAsync();
            }
        }

        private void PStatictis_Finalize(object sender, EventArgs e)
        {
            WK_Update.CancelAsync();
        }

        #endregion

        #region Các hàm khởi tạo

        private void Render_MEM()
        {
            // Globals_Database Dictionary counts
            var dicCountUc = new panelS() { LabelName = GetFriendlyName("Dictionary Count") };
            opMEMFlow.Controls.Add(dicCountUc);
            extraBindings["Dictionary Count"] = dicCountUc;

            var dicCartonCountUc = new panelS() { LabelName = GetFriendlyName("DCarton Count") };
            opMEMFlow.Controls.Add(dicCartonCountUc);
            extraBindings["DCarton Count"] = dicCartonCountUc;

            // Globals_Database Queue counts
            var updateProductQueueUc = new panelS() { LabelName = GetFriendlyName("Update Product Queue Count") };
            opMEMFlow.Controls.Add(updateProductQueueUc);
            extraBindings["Update Product Queue Count"] = updateProductQueueUc;

            var insertRecordQueueUc = new panelS() { LabelName = GetFriendlyName("Insert Record Queue Count") };
            opMEMFlow.Controls.Add(insertRecordQueueUc);
            extraBindings["Insert Record Queue Count"] = insertRecordQueueUc;

            var insertRecordCSQueueUc = new panelS() { LabelName = GetFriendlyName("Insert Record CS Queue Count") };
            opMEMFlow.Controls.Add(insertRecordCSQueueUc);
            extraBindings["Insert Record CS Queue Count"] = insertRecordCSQueueUc;

            var updateCartonQueueUc = new panelS() { LabelName = GetFriendlyName("Update Carton Queue Count") };
            opMEMFlow.Controls.Add(updateCartonQueueUc);
            extraBindings["Update Carton Queue Count"] = updateCartonQueueUc;

            var activateCartonQueueUc = new panelS() { LabelName = GetFriendlyName("Activate Carton Queue Count") };
            opMEMFlow.Controls.Add(activateCartonQueueUc);
            extraBindings["Activate Carton Queue Count"] = activateCartonQueueUc;

            var awsReceiveQueueUc = new panelS() { LabelName = GetFriendlyName("AWS Receive Queue Count") };
            opMEMFlow.Controls.Add(awsReceiveQueueUc);
            extraBindings["AWS Receive Queue Count"] = awsReceiveQueueUc;

            var awsSendQueueUc = new panelS() { LabelName = GetFriendlyName("AWS Send Queue Count") };
            opMEMFlow.Controls.Add(awsSendQueueUc);
            extraBindings["AWS Send Queue Count"] = awsSendQueueUc;

            // Globals class properties
            Type globalsType = typeof(Globals);
            foreach (var prop in globalsType.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                // Nếu là object phức tạp, hiển thị thuộc tính con
                if (prop.Name == "CurrentUser" && prop.PropertyType.Name == "UserData")
                {
                    // Hiển thị các thuộc tính của CurrentUser
                    foreach (var userProp in prop.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        string key = $"CurrentUser.{userProp.Name}";
                        panelS uc = new panelS() { LabelName = GetFriendlyName(key) };
                        opMEMFlow.Controls.Add(uc);
                        extraBindings[key] = uc;
                    }
                }
                else if (prop.Name == "CameraMain_PLC_Counter" && prop.PropertyType.Name == "PLCCounter")
                {
                    // Hiển thị các thuộc tính của PLCCounter
                    foreach (var plcProp in prop.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        string key = $"PLCCounter.{plcProp.Name}";
                        panelS uc = new panelS() { LabelName = GetFriendlyName(key) };
                        opMEMFlow.Controls.Add(uc);
                        extraBindings[key] = uc;
                    }
                }
                else if (prop.Name == "ProductionData" && prop.PropertyType.Name == "ProductionOrder")
                {
                    // Hiển thị các thuộc tính string của ProductionOrder
                    foreach (var poProp in prop.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (poProp.PropertyType == typeof(string) || poProp.PropertyType.IsPrimitive)
                        {
                            string key = $"ProductionData.{poProp.Name}";
                            panelS uc = new panelS() { LabelName = GetFriendlyName(key) };
                            opMEMFlow.Controls.Add(uc);
                            extraBindings[key] = uc;
                        }
                        else if (poProp.Name == "awsSendCounter")
                        {
                            // Hiển thị AWS_Send_Counter properties
                            foreach (var awsProp in poProp.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                            {
                                string key = $"AWS_Send.{awsProp.Name}";
                                panelS uc = new panelS() { LabelName = GetFriendlyName(key) };
                                opMEMFlow.Controls.Add(uc);
                                extraBindings[key] = uc;
                            }
                        }
                        else if (poProp.Name == "awsRecivedCounter")
                        {
                            // Hiển thị AWS_Recived_Counter properties
                            foreach (var awsProp in poProp.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                            {
                                string key = $"AWS_Recived.{awsProp.Name}";
                                panelS uc = new panelS() { LabelName = GetFriendlyName(key) };
                                opMEMFlow.Controls.Add(uc);
                                extraBindings[key] = uc;
                            }
                        }
                    }
                }
                else
                {
                    // Các thuộc tính đơn giản khác
                    string key = $"Globals.{prop.Name}";
                    panelS uc = new panelS() { LabelName = GetFriendlyName(key) };
                    opMEMFlow.Controls.Add(uc);
                    globalsBindings[prop] = uc;
                }
            }

            // CameraMain_HMI static properties
            Type cameraMainType = typeof(CameraMain_HMI);
            foreach (var prop in cameraMainType.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                string key = $"CameraMain_HMI.{prop.Name}";
                panelS uc = new panelS() { LabelName = GetFriendlyName(key) };
                opMEMFlow.Controls.Add(uc);
                extraBindings[key] = uc;
            }

            // CameraSub_HMI static properties
            Type cameraSubType = typeof(CameraSub_HMI);
            foreach (var prop in cameraSubType.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                string key = $"CameraSub_HMI.{prop.Name}";
                panelS uc = new panelS() { LabelName = GetFriendlyName(key) };
                opMEMFlow.Controls.Add(uc);
                extraBindings[key] = uc;
            }

            // Globals_Database class properties (static)
            Type globalsDatabaseType = typeof(Globals_Database);
            foreach (var prop in globalsDatabaseType.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                if (!prop.Name.Contains("Dictionary") && !prop.Name.Contains("Queue"))
                {
                    string key = $"Globals_Database.{prop.Name}";
                    panelS uc = new panelS() { LabelName = GetFriendlyName(key) };
                    opMEMFlow.Controls.Add(uc);
                    globalsDatabaseBindings[prop] = uc;
                }
            }

            // Original Product_Counter properties
            Type type = typeof(Product_Counter);
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string key = $"Counter.{prop.Name}";
                panelS uc = new panelS() { LabelName = GetFriendlyName(key) };
                opMEMFlow.Controls.Add(uc);
                bindings[prop] = uc;
            }
        }
        #endregion

        #region Các luồng và hàm của nó
        int refreshCount = 10; // Biến đếm số lần cập nhật
        private void WK_Update_DoWork(object sender, DoWorkEventArgs e)
        {
            while(!WK_Update.CancellationPending)
            {
                refreshCount++;
                try
                {
                    

                    this.InvokeIfRequired(() =>
                    {
                        Update_BindingProps();
                        Update_ExtraProp();
                        Update_GlobalsProps();
                        Update_GlobalsDatabaseProps();
                    });

                    try
                    {
                        if (refreshCount > 10)
                        {
                            refreshCount = 0; // Reset đếm sau mỗi 10 lần cập nhật
                            orderNo = Globals.ProductionData.orderNo;
                            HourlyChartData hourlyChartData = GetLast6Hours($@"{Globals.ProductionData.dbPath}/Record_{Globals.ProductionData.orderNo}.db");

                            var labels = hourlyChartData.Labels;
                            var labelOption = new
                            {
                                show = true,
                                position = "inside",
                                fontSize = 12
                            };

                            var option = new
                            {
                                tooltip = new
                                {
                                    trigger = "axis",
                                    axisPointer = new { type = "shadow" }
                                },
                                legend = new { data = new[] { "Tổng", "Tốt", "Loại", "Không Kiểm" } },
                                toolbox = new
                                {
                                    show = true,
                                    orient = "vertical",
                                    left = "right",
                                    top = "center",
                                    feature = new
                                    {
                                        mark = new { show = true },
                                        dataView = new { show = true, readOnly = false },
                                        magicType = new { show = true, type = new[] { "line", "bar", "stack", "pie" } },
                                        restore = new { show = true },
                                        saveAsImage = new { show = true }
                                    }
                                },
                                xAxis = new object[]
                                {
                                    new
                                    {
                                        type = "category",
                                        axisTick = new { show = false },
                                        data = labels
                                    }
                                                                    },
                                yAxis = new object[]
                                                                    {
                                    new { type = "value" }
                                                                    },
                                series = new object[]
                                                                    {
                                    new
                                    {
                                        name = "Tổng",
                                        type = "bar",
                                        barGap = 0,
                                        label = labelOption,
                                        emphasis = new { focus = "series" },
                                        data = hourlyChartData.Tong
                                    },
                                    new
                                    {
                                        name = "Tốt",
                                        type = "bar",
                                        label = labelOption,
                                        emphasis = new { focus = "series" },
                                        data = hourlyChartData.Tot
                                    },
                                    new
                                    {
                                        name = "Không kiểm",
                                        type = "bar",
                                        label = labelOption,
                                        emphasis = new { focus = "series" },
                                        data = hourlyChartData.Khac
                                    },
                                    new
                                    {
                                        name = "Loại",
                                        type = "bar",
                                        label = labelOption,
                                        emphasis = new { focus = "series" },
                                        data = hourlyChartData.Loi
                                    },
                                }
                            };

                            string templatePath = @"C:\MasanSerialization\Chart_Database\bar-label-rotation.html";
                            string templatePatho = @"C:\MasanSerialization\Chart_Database\bar-label-rotation-out.html";
                            // Xóa file HTML cũ nếu có
                            if (File.Exists(templatePatho))
                            {
                                File.Delete(templatePatho);
                                Console.WriteLine("Đã xóa file HTML cũ.");
                            }
                            else
                            {
                                Console.WriteLine("Không tìm thấy file cũ, bỏ qua bước xóa.");
                            }

                            InjectOptionToHtml(templatePath, templatePatho, option);

                            this.InvokeIfRequired(() =>
                            {
                                webView21.Source = new Uri(templatePatho);
                                webView21.Refresh();
                            });


                            //biểu đồ 02

                            var option02 = new
                            {
                                tooltip = new
                                {
                                    trigger = "item"
                                },
                                legend = new
                                {
                                    orient = "vertical",
                                    left = "left"
                                },
                                series = new[]
                                {
                                    new
                                    {
                                        name = "Sản lượng",
                                        type = "pie",
                                        radius = "50%",
                                        data = new object[]
                                        {
                                            new { value = Globals.ProductionData.counter.passCount, name = "Tốt", itemStyle = new { color = "#4CAF50" } },
                                            new { value = Globals.ProductionData.counter.failCount,  name = "Lỗi", itemStyle = new { color = "#FF5722" } },
                                            new { value = 0,  name = "Không kiểm", itemStyle = new { color = "#9E9E9E" } }
                                        },
                                        emphasis = new
                                        {
                                            itemStyle = new
                                            {
                                                shadowBlur = 10,
                                                shadowOffsetX = 0,
                                                shadowColor = "rgba(0, 0, 0, 0.5)"
                                            }
                                        }
                                    }
                                }
                            };
                            string templatePath2 = @"C:\MasanSerialization\Chart_Database\pie.html";
                            string templatePatho2 = @"C:\MasanSerialization\Chart_Database\pie-out.html";
                            // Xóa file HTML cũ nếu có
                            if (File.Exists(templatePatho2))
                            {
                                File.Delete(templatePatho2);
                                Console.WriteLine("Đã xóa file HTML cũ.");
                            }
                            else
                            {
                                Console.WriteLine("Không tìm thấy file cũ, bỏ qua bước xóa.");
                            }

                            InjectOptionToHtml(templatePath2, templatePatho2, option02);

                            this.InvokeIfRequired(() =>
                            {
                                webView22.Source = new Uri(templatePatho2);
                                webView22.Refresh();
                            });


                            //biểu đồ 03
                            var labelOption3 = new
                            {
                                show = true,
                                position = "insideBottom",
                                distance = 15,
                                align = "left",
                                verticalAlign = "middle",
                                rotate = 90,
                                fontSize = 16,
                                rich = new
                                {
                                    name = new { }
                                }
                            };

                            var option3 = new
                            {
                                tooltip = new
                                {
                                    trigger = "axis",
                                    axisPointer = new
                                    {
                                        type = "shadow"
                                    }
                                },
                                legend = new
                                {
                                    data = new[] { "Đã hoàn tất", "Đang gửi", "Đang chờ", "Gửi lại" }
                                },
                                xAxis = new object[]
                                {
                                new
                                {
                                    type = "category",
                                    axisTick = new { show = false },
                                    data = new[] { "Thống kê" }
                                }
                                                        },
                                yAxis = new object[]
                                                        {
                                new
                                {
                                    type = "value"
                                }
                                                        },
                                series = new object[]
                                                        {
                                new
                                {
                                    name = "Đã hoàn tất",
                                    type = "bar",
                                    barGap = 0,
                                    label = labelOption3,
                                    emphasis = new { focus = "series" },
                                    data = new object[] { Globals.ProductionData.awsRecivedCounter.recivedCount }
                                },
                                new
                                {
                                    name = "Đang gửi",
                                    type = "bar",
                                    label = labelOption3,
                                    emphasis = new { focus = "series" },
                                    data = new object[] { Globals.ProductionData.awsSendCounter.sentCount }
                                },
                                new
                                {
                                    name = "Đang chờ",
                                    type = "bar",
                                    label = labelOption3,
                                    emphasis = new { focus = "series" },
                                    data = new object[] { Globals.ProductionData.awsSendCounter.pendingCount }
                                },
                                new
                                {
                                    name = "Gửi lại",
                                    type = "bar",
                                    label = labelOption3,
                                    emphasis = new { focus = "series" },
                                    data = new object[] { Globals.ProductionData.awsSendCounter.failedCount }
                                }
                                                        }
                            };

                            string templatePath3 = @"C:\MasanSerialization\Chart_Database\bar-label-rotation.html";
                            string templatePatho3 = @"C:\MasanSerialization\Chart_Database\bar-label-rotation-out2.html";
                            // Xóa file HTML cũ nếu có
                            if (File.Exists(templatePatho3))
                            {
                                File.Delete(templatePatho3);
                                Console.WriteLine("Đã xóa file HTML cũ.");
                            }
                            else
                            {
                                Console.WriteLine("Không tìm thấy file cũ, bỏ qua bước xóa.");
                            }

                            InjectOptionToHtml(templatePath3, templatePatho3, option3);

                            this.InvokeIfRequired(() =>
                            {
                                webView23.Source = new Uri(templatePatho3);
                                webView23.Refresh();
                            });
                        }
                    }
                    catch
                    {
                        //không làm gì cả
                    }

                }
                catch (Exception ex)
                {

                    this.ShowErrorNotifier($"Lỗi S123 cập nhật dữ liệu: {ex.Message}");
                }

                Thread.Sleep(500); // Cập nhật mỗi 0.5 giây
            }
        }

        private void Update_ExtraProp()
        {
            foreach (var kv in extraBindings)
            {
                var key = kv.Key;
                var uc = kv.Value;

                try
                {
                    // Queue counts
                    switch (key)
                    {
                        case "Dictionary Count":
                            uc.LabelValue = Globals_Database.Dictionary_ProductionCode_Data?.Count.ToString() ?? "0";
                            break;
                        case "DCarton Count":
                            uc.LabelValue = Globals_Database.Dictionary_ProductionCarton_Data?.Count.ToString() ?? "0";
                            break;
                        case "Update Product Queue Count":
                            uc.LabelValue = Globals_Database.Update_Product_To_SQLite_Queue?.Count.ToString() ?? "0";
                            break;
                        case "Insert Record Queue Count":
                            uc.LabelValue = Globals_Database.Insert_Product_To_Record_Queue?.Count.ToString() ?? "0";
                            break;
                        case "Insert Record CS Queue Count":
                            uc.LabelValue = Globals_Database.Insert_Product_To_Record_CS_Queue?.Count.ToString() ?? "0";
                            break;
                        case "Update Carton Queue Count":
                            uc.LabelValue = Globals_Database.Update_Product_To_Record_Carton_Queue?.Count.ToString() ?? "0";
                            break;
                        case "Activate Carton Queue Count":
                            uc.LabelValue = Globals_Database.Activate_Carton?.Count.ToString() ?? "0";
                            break;
                        case "AWS Receive Queue Count":
                            uc.LabelValue = Globals_Database.aWS_Recive_Datas?.Count.ToString() ?? "0";
                            break;
                        case "AWS Send Queue Count":
                            uc.LabelValue = Globals_Database.aWS_Send_Datas?.Count.ToString() ?? "0";
                            break;
                    }

                    // CurrentUser properties
                    if (key.StartsWith("CurrentUser."))
                    {
                        var propName = key.Substring("CurrentUser.".Length);
                        if (Globals.CurrentUser != null)
                        {
                            var prop = Globals.CurrentUser.GetType().GetProperty(propName);
                            if (prop != null)
                            {
                                var value = prop.GetValue(Globals.CurrentUser);
                                uc.LabelValue = value?.ToString() ?? "null";
                            }
                        }
                        else
                        {
                            uc.LabelValue = "null";
                        }
                    }

                    // PLCCounter properties
                    if (key.StartsWith("PLCCounter."))
                    {
                        var propName = key.Substring("PLCCounter.".Length);
                        if (Globals.CameraMain_PLC_Counter != null)
                        {
                            var prop = Globals.CameraMain_PLC_Counter.GetType().GetProperty(propName);
                            if (prop != null)
                            {
                                var value = prop.GetValue(Globals.CameraMain_PLC_Counter);
                                uc.LabelValue = value?.ToString() ?? "0";
                            }
                        }
                        else
                        {
                            uc.LabelValue = "0";
                        }
                    }

                    // ProductionData properties
                    if (key.StartsWith("ProductionData."))
                    {
                        var propName = key.Substring("ProductionData.".Length);
                        if (Globals.ProductionData != null)
                        {
                            var prop = Globals.ProductionData.GetType().GetProperty(propName);
                            if (prop != null)
                            {
                                var value = prop.GetValue(Globals.ProductionData);
                                uc.LabelValue = value?.ToString() ?? "-";
                            }
                        }
                        else
                        {
                            uc.LabelValue = "-";
                        }
                    }

                    // AWS_Send Counter properties
                    if (key.StartsWith("AWS_Send."))
                    {
                        var propName = key.Substring("AWS_Send.".Length);
                        if (Globals.ProductionData?.awsSendCounter != null)
                        {
                            var prop = Globals.ProductionData.awsSendCounter.GetType().GetProperty(propName);
                            if (prop != null)
                            {
                                var value = prop.GetValue(Globals.ProductionData.awsSendCounter);
                                uc.LabelValue = value?.ToString() ?? "0";
                            }
                        }
                        else
                        {
                            uc.LabelValue = "0";
                        }
                    }

                    // AWS_Recived Counter properties
                    if (key.StartsWith("AWS_Recived."))
                    {
                        var propName = key.Substring("AWS_Recived.".Length);
                        if (Globals.ProductionData?.awsRecivedCounter != null)
                        {
                            var prop = Globals.ProductionData.awsRecivedCounter.GetType().GetProperty(propName);
                            if (prop != null)
                            {
                                var value = prop.GetValue(Globals.ProductionData.awsRecivedCounter);
                                uc.LabelValue = value?.ToString() ?? "0";
                            }
                        }
                        else
                        {
                            uc.LabelValue = "0";
                        }
                    }

                    // CameraMain_HMI properties
                    if (key.StartsWith("CameraMain_HMI."))
                    {
                        var propName = key.Substring("CameraMain_HMI.".Length);
                        var prop = typeof(CameraMain_HMI).GetProperty(propName);
                        if (prop != null)
                        {
                            var value = prop.GetValue(null);
                            uc.LabelValue = value?.ToString() ?? "null";
                        }
                    }

                    // CameraSub_HMI properties
                    if (key.StartsWith("CameraSub_HMI."))
                    {
                        var propName = key.Substring("CameraSub_HMI.".Length);
                        var prop = typeof(CameraSub_HMI).GetProperty(propName);
                        if (prop != null)
                        {
                            var value = prop.GetValue(null);
                            uc.LabelValue = value?.ToString() ?? "null";
                        }
                    }
                }
                catch (Exception ex)
                {
                    uc.LabelValue = $"Error: {ex.Message}";
                }
            }
        }

        private void Update_BindingProps()
        {
            foreach (var kv in bindings)
            {
                var prop = kv.Key;
                var uc = kv.Value;
                if (Globals.ProductionData.counter != null)
                {
                    var value = prop.GetValue(Globals.ProductionData.counter);
                    uc.LabelValue = value?.ToString() ?? "0";
                }
            }
        }

        private void Update_GlobalsProps()
        {
            foreach (var kv in globalsBindings)
            {
                var prop = kv.Key;
                var uc = kv.Value;
                try
                {
                    var value = prop.GetValue(null);
                    if (value != null)
                    {
                        if (value is bool boolValue)
                        {
                            uc.LabelValue = boolValue.ToString();
                        }
                        else if (value is Enum enumValue)
                        {
                            uc.LabelValue = enumValue.ToString();
                        }
                        else if (value is string stringValue)
                        {
                            uc.LabelValue = string.IsNullOrEmpty(stringValue) ? "Empty" : stringValue;
                        }
                        else if (value is int || value is float || value is double)
                        {
                            uc.LabelValue = value.ToString();
                        }
                        else if (value.GetType().IsClass && value.GetType() != typeof(string))
                        {
                            uc.LabelValue = value.GetType().Name;
                        }
                        else
                        {
                            uc.LabelValue = value.ToString();
                        }
                    }
                    else
                    {
                        uc.LabelValue = "null";
                    }
                }
                catch (Exception ex)
                {
                    uc.LabelValue = $"Error: {ex.Message}";
                }
            }
        }

        private void Update_GlobalsDatabaseProps()
        {
            foreach (var kv in globalsDatabaseBindings)
            {
                var prop = kv.Key;
                var uc = kv.Value;
                try
                {
                    var value = prop.GetValue(null);
                    if (value != null)
                    {
                        if (value is bool boolValue)
                        {
                            uc.LabelValue = boolValue.ToString();
                        }
                        else if (value is Enum enumValue)
                        {
                            uc.LabelValue = enumValue.ToString();
                        }
                        else if (value is string stringValue)
                        {
                            uc.LabelValue = string.IsNullOrEmpty(stringValue) ? "Empty" : stringValue;
                        }
                        else if (value is int || value is float || value is double)
                        {
                            uc.LabelValue = value.ToString();
                        }
                        else if (value.GetType().IsClass && value.GetType() != typeof(string))
                        {
                            uc.LabelValue = value.GetType().Name;
                        }
                        else
                        {
                            uc.LabelValue = value.ToString();
                        }
                    }
                    else
                    {
                        uc.LabelValue = "null";
                    }
                }
                catch (Exception ex)
                {
                    uc.LabelValue = $"Error: {ex.Message}";
                }
            }
        }

        #endregion

        public class HourlyChartData
        {
            public string[] Labels { get; set; }
            public int[] Tong { get; set; }   // Tổng = Pass + Failed + Khác
            public int[] Tot { get; set; }    // Pass
            public int[] Loi { get; set; }    // Failed
            public int[] Khac { get; set; }   // còn lại
        }
        public static HourlyChartData GetLast6Hours(string dbPath)
            {
                // Chốt mốc giờ: tròn giờ hiện tại và lùi 5 giờ
                var now = DateTime.Now;
                var endHour = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
                var startHour = endHour.AddHours(-3);

            var labels = Enumerable.Range(0, 6)
                        .Select(i => startHour.AddHours(i).ToString("H:00")) // cộng 7 giờ khi hiển thị
                        .ToArray();

                int[] tong = new int[4];
                int[] tot = new int[4];
                int[] loi = new int[4];
                int[] khac = new int[4];

                using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    conn.Open();

                // Gộp theo từng block giờ. Dùng replace(ActivateDate,'T',' ') để chắc kèo ISO8601.
                string sql = @"
                WITH src AS (
                    SELECT 
                        -- Cắt mili-giây và offset
                        strftime('%Y-%m-%d %H:00:00', substr(ActivateDate, 1, 19)) AS HourBlock,
                        lower(coalesce(trim(Status), '')) AS st
                    FROM Records
                    WHERE datetime(substr(ActivateDate, 1, 19)) >= datetime(@start)
                        AND datetime(substr(ActivateDate, 1, 19)) <= datetime(@end, '+59 minutes', '+59 seconds')
                ),
                agg AS (
                    SELECT 
                        HourBlock,
                        COUNT(*)                                                  AS Total,
                        SUM(CASE WHEN st='pass'   THEN 1 ELSE 0 END)              AS Good,
                        SUM(CASE WHEN st='error' THEN 1 ELSE 0 END)              AS Other,
                        SUM(CASE WHEN st NOT IN ('pass','error') THEN 1 ELSE 0 END) AS Bad
                    FROM src
                    GROUP BY HourBlock
                )
                SELECT HourBlock, Total, Good, Bad, Other
                FROM agg
                ORDER BY HourBlock;
                ";


                using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@start", startHour.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@end", endHour.ToString("yyyy-MM-dd HH:mm:ss"));

                        using (var rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                var hourBlock = DateTime.Parse(rd["HourBlock"].ToString());
                                int idx = (int)(hourBlock - startHour).TotalHours;
                                if (idx < 0 || idx >= 6) continue;

                                int total = Convert.ToInt32(rd["Total"]);
                                int good = Convert.ToInt32(rd["Good"]);
                                int bad = Convert.ToInt32(rd["Bad"]);
                                int other = Convert.ToInt32(rd["Other"]);

                                tong[idx] = total;
                                tot[idx] = good;
                                loi[idx] = bad;
                                khac[idx] = other;
                            }
                        }
                    }
                }

                return new HourlyChartData
                {
                    Labels = labels, // ví dụ: ["11:00","12:00",...]
                    Tong = tong,
                    Tot = tot,
                    Loi = loi,
                    Khac = khac
                };
            }

    }
}
