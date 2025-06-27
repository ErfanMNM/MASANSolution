using HslCommunication;
using MainClass;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpT;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using static MFI_Service.MFI_Service_Form;
using static QR_MASAN_01.ActiveLogs;
using static QR_MASAN_01.SystemLogs;

namespace QR_MASAN_01
{
    public partial class FDashboard_XK : UIPage
    {
        private POService poService = new POService(@"C:\Users\THUC\source\repos\ErfanMNM\MASANSolution\Server_Service\po_data.db");
        public FDashboard_XK()
        {
            InitializeComponent();
            WK_Process_AUData.RunWorkerAsync();
        }

        public void INIT()
        {
            try
            {
                // Khởi tạo các thành phần cần thiết
                WK_Update.RunWorkerAsync();
                WK_UI_CAM_Update.RunWorkerAsync();
                Camera.Connect();
                if (GlobalSettings.GetInt("CAMERA_SLOT") > 1)
                {
                    Camera_c.Connect();
                }
                PLC.PLC_IP = PLCAddress.Get("PLC_IP");
                PLC.PLC_PORT = Convert.ToInt32(PLCAddress.Get("PLC_PORT"));
                PLC.PLC_Ready_DM = PLCAddress.Get("PLC_Ready_DM");
                PLC.InitPLC();

                //lấy dòng cuối cùng của PO

                DataTable lastLog = ProductionLogs.GetLastLog_Datatable();
                if (lastLog.Rows.Count > 0)
                {
                    poService.LoadOrderNoToComboBox(ipOrderNO);
                    
                    DataRow row = lastLog.Rows[0];
                    GPOInfo.OrderNo = row["orderNO"].ToString();
                    //tiếp tục lấy full thông tin PO
                    ipOrderNO.SelectedItem = row["orderNO"].ToString();
                }
                else
                {
                    poService.LoadOrderNoToComboBox(ipOrderNO);
                }


            }
            catch (Exception ex)
            {
                SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.SYSTEM_ERROR, "Lỗi khởi tạo Dashboard", "System", ex.Message);
                SystemLogs.InsertToSQLite(systemLogs);

                // Hiển thị thông báo lỗi trên giao diện
                this.ShowErrorDialog("Lỗi khởi tạo Dashboard, Vui lòng tắt máy mở lại", ex.Message);
                //Hiện lên box
                ConUpdate($"Lỗi khởi tạo Dashboard: {ex.Message}");
                ConUpdate($"VUI LÒNG TẮT MÁY MỞ LẠI");
            }

        }

        MFI_Info _clientMFI = new MFI_Info();
        bool ClearPLC = false;

        public string dataBase_FileName = "";
        //lấy xem có thông tin phiên tạo mới hay không

        #region Các chương trình tạo dữ liệu MFI, đồng bộ với máy chủ
        //Đẩy mã mới tạo vào dic
        public void Push_Data_To_Dic()
        {
            DataTable dataTable = new DataTable();
            // Dictionary để lưu dữ liệu với CaseQR làm key
           // Dictionary<string, ProductData> ProductQR_Dictionary = new Dictionary<string, ProductData>();
           //Đẩy vào dic chính
            string connectionString = $@"Data Source={_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename};Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Câu lệnh SQL để đọc một cột (ví dụ: cột 'Name')
                string query = $"SELECT * FROM `QRContent`;";

                // Sử dụng SQLiteDataAdapter để đổ dữ liệu vào DataTable
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection))
                {
                    adapter.Fill(dataTable);
                }
                // Duyệt qua các hàng trong DataTable và thêm vào List<string>
                foreach (DataRow row in dataTable.Rows)
                {
                    // Đọc dữ liệu từ SQL Server
                    int ProductID = Convert.ToInt32(row["ProductID"]); // CaseID
                    string ProductQR = row["ProductQR"].ToString(); // CaseQR
                    int active = Convert.ToInt32(row["Active"]); // Active
                    string timeStampA = row["TimestampActive"].ToString(); ; 
                    string timeStampP = row["TimestampPrinted"].ToString(); ;
                    int timeUnixA = Convert.ToInt32(row["TimeUnixActive"]) ; 
                    int timeUnixP = Convert.ToInt32(row["TimeUnixPrinted"]); 


                    // Thêm dữ liệu vào Dictionary với CaseQR làm key
                    Globalvariable.Main_Content_Dictionary[ProductQR] = new ProductData
                    {
                        ProductID = ProductID,
                        Active = active,
                        TimeStampActive = timeStampA,
                        TimeStampPrinted = timeStampP,
                        TimeUnixActive = timeUnixA,
                        TimeUnixPrinted = timeUnixP

                    };
                    
                }

                this.Invoke(new Action(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Đẩy dữ liệu C thành công, số lượng: {dataTable.Rows.Count} ");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                }));
                connection.Close();
            }
        }
        public void Push_Data_To_Dic_C1()
        {
            DataTable dataTable = new DataTable();
            // Dictionary để lưu dữ liệu với CaseQR làm key
            // Dictionary<string, ProductData> ProductQR_Dictionary = new Dictionary<string, ProductData>();
            //Đẩy vào dic chính
            string connectionString = $@"Data Source={_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename_C1};Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Câu lệnh SQL để đọc một cột (ví dụ: cột 'Name')
                string query = $"SELECT * FROM `QRContent`;";

                // Sử dụng SQLiteDataAdapter để đổ dữ liệu vào DataTable
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection))
                {
                    adapter.Fill(dataTable);
                }

                // Duyệt qua các hàng trong DataTable và thêm vào List<string>
                foreach (DataRow row in dataTable.Rows)
                {
                    // Đọc dữ liệu từ SQL Server
                    int ProductID = Convert.ToInt32(row["ProductID"]); // CaseID
                    string ProductQR = row["ProductQR"].ToString(); // CaseQR
                    int active = Convert.ToInt32(row["Active"]); // Active
                    string timeStampA = row["TimestampActive"].ToString(); ;
                    string timeStampP = row["TimestampPrinted"].ToString(); ;
                    int timeUnixA = Convert.ToInt32(row["TimeUnixActive"]);
                    int timeUnixP = Convert.ToInt32(row["TimeUnixPrinted"]);


                    // Thêm dữ liệu vào Dictionary với CaseQR làm key
                    Globalvariable.Main_Content_Dictionary[ProductQR] = new ProductData
                    {
                        ProductID = ProductID,
                        Active = active,
                        TimeStampActive = timeStampA,
                        TimeStampPrinted = timeStampP,
                        TimeUnixActive = timeUnixA,
                        TimeUnixPrinted = timeUnixP

                    };
                }

                this.Invoke(new Action(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Đẩy dữ liệu C1 thành công, số lượng: {dataTable.Rows.Count} ");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                }));
                connection.Close();
            }
        }
        public void Push_Data_To_Dic_C2()
        {
            DataTable dataTable = new DataTable();
            // Dictionary để lưu dữ liệu với CaseQR làm key
            // Dictionary<string, ProductData> ProductQR_Dictionary = new Dictionary<string, ProductData>();
            //Đẩy vào dic chính
            string connectionString = $@"Data Source={_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename_C2};Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Câu lệnh SQL để đọc một cột (ví dụ: cột 'Name')
                string query = $"SELECT * FROM `QRContent`;";

                // Sử dụng SQLiteDataAdapter để đổ dữ liệu vào DataTable
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection))
                {
                    adapter.Fill(dataTable);
                }

                // Duyệt qua các hàng trong DataTable và thêm vào List<string>
                foreach (DataRow row in dataTable.Rows)
                {
                    // Đọc dữ liệu từ SQL Server
                    int ProductID = Convert.ToInt32(row["ProductID"]); // CaseID
                    string ProductQR = row["ProductQR"].ToString(); // CaseQR
                    int active = Convert.ToInt32(row["Active"]); // Active
                    string timeStampA = row["TimestampActive"].ToString(); ;
                    string timeStampP = row["TimestampPrinted"].ToString(); ;
                    int timeUnixA = Convert.ToInt32(row["TimeUnixActive"]);
                    int timeUnixP = Convert.ToInt32(row["TimeUnixPrinted"]);


                    // Thêm dữ liệu vào Dictionary với CaseQR làm key
                    Globalvariable.Main_Content_Dictionary[ProductQR] = new ProductData
                    {
                        ProductID = ProductID,
                        Active = active,
                        TimeStampActive = timeStampA,
                        TimeStampPrinted = timeStampP,
                        TimeUnixActive = timeUnixA,
                        TimeUnixPrinted = timeUnixP

                    };
                }

                this.Invoke(new Action(() =>
                {
                    ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Đẩy dữ liệu C2 thành công, số lượng: {dataTable.Rows.Count} ");
                    ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                }));
                connection.Close();
            }
        }
        #endregion

        #region Các cập nhật lên màn hình
        //Gửi lên màn hình và lưu log
        public void ConUpdate(string message)
        {
            this.Invoke(new Action(() =>
            {
                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: {message}");
                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
            }));
        }

        private void WK_Update_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!WK_Update.CancellationPending)
            {
                if (Globalvariable.WB_Color == Globalvariable.OK_Color)
                {
                    Globalvariable.WB_Color = Globalvariable.NG_Color;
                }
                else
                {
                    Globalvariable.WB_Color = Globalvariable.OK_Color;
                }


                //chế độ dữ liệu mới cũ

                bool printers = false;

                if (GPrinter.Printer_Status == e_PRINTER_Status.PRINTING)
                {
                    printers = true;
                }
                else
                {
                    printers = false;
                    if (GlobalSettings.Get("APPMODE") == "ADD_Data")
                    {
                        printers = true;
                    }
                }

                if (GlobalSettings.GetInt("CAMERA_SLOT") == 1)
                {
                    GCamera.Camera_Status_02 = e_Camera_Status.CONNECTED;


                }
                else
                {

                }
                //Ready
                if (GCamera.Camera_Status == e_Camera_Status.CONNECTED && GCamera.Camera_Status_02 == e_Camera_Status.CONNECTED && Globalvariable.Data_Status == e_Data_Status.READY && Globalvariable.PLCConnect && Globalvariable.setReady)
                {
                    if (Globalvariable.AllReady)
                    {
                        if (PLC.Ready != 1)
                        {
                            PLC.Ready = 1;
                        }

                    }
                    else
                    {
                        Globalvariable.AllReady = true;
                        if (PLC.Ready != 0)
                        {
                            PLC.Ready = 0;
                        }

                    }
                }
                else
                {

                    if (!Globalvariable.AllReady)
                    {

                    }
                    else
                    {
                        Globalvariable.AllReady = false;
                    }
                }

                if (APP.ByPass_Ready)
                {
                    PLC.Ready = 1;
                }
                
                if (ClearPLC)
                {
                    this.Invoke(new Action(() =>
                    {
                        btnClearPLC.Enabled = true;
                        btnClearPLC.Text = "Xóa lỗi PLC";
                        ClearPLC = false;
                    }));
                }
                //máy in


                //Kiểm tra PLC_ACTIVE_DM nếu = 1 set Globale ACTIVE = true dùng hsl
                OperateResult<int> read = PLC.plc.ReadInt32(PLCAddress.Get("PLC_Bypass_DM_C1"));
                if (read.IsSuccess)
                {
                    if (read.Content != 1)
                    {
                        if (Globalvariable.ACTIVE_C1 == false)
                        {
                            //ghi log 
                            ActiveLogs activeLogs = new ActiveLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_ActiveLogType.ACTIVE, "Bật Camera 01", "PLC", "Nhận kích hoạt camera 01 từ PLC, nhận giá trị khác 1");
                            //Ghi vào hàng chờ
                            ActiveLogQueue.Enqueue(activeLogs);
                            Globalvariable.ACTIVE_C1 = true;
                        }
                    }
                    else
                    {
                        if (Globalvariable.ACTIVE_C1 == true)
                        {
                            //ghi log
                            ActiveLogs activeLogs = new ActiveLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_ActiveLogType.ACTIVE, "Tắt Camera 01", "PLC", "Nhận ngừng kích hoạt camera 01 từ PLC, nhận giá trị bằng 1");
                            //Ghi vào hàng chờ
                            ActiveLogQueue.Enqueue(activeLogs);
                            Globalvariable.ACTIVE_C1 = false;
                        }
                    }
                }

                OperateResult<int> read1 = PLC.plc.ReadInt32(PLCAddress.Get("PLC_Bypass_DM_C2"));
                if (read1.IsSuccess)
                {
                    if (read1.Content != 1)
                    {
                        if (Globalvariable.ACTIVE_C2 == false)
                        {
                            ActiveLogs activeLogs = new ActiveLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_ActiveLogType.UNACTIVE, "Bật Camera 02", "PLC", "Nhận kích hoạt camera 02 từ PLC, nhận giá trị khác 1");
                            //Ghi vào hàng chờ
                            ActiveLogQueue.Enqueue(activeLogs);
                            Globalvariable.ACTIVE_C2 = true;
                        }
                    }
                    else
                    {
                        if(Globalvariable.ACTIVE_C2 == true)
                        {
                            ActiveLogs activeLogs = new ActiveLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_ActiveLogType.UNACTIVE, "Bật Camera 02", "PLC", "Nhận kích hoạt camera 02 từ PLC, nhận giá trị bằng 1");
                            //Ghi vào hàng chờ
                            ActiveLogQueue.Enqueue(activeLogs);
                            Globalvariable.ACTIVE_C2 = false;
                        }
                        
                    }
                }

                if(Globalvariable.ACTIVE_C1 && Globalvariable.ACTIVE_C2)
                {
                    ActiveLogs activeLogs = new ActiveLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_ActiveLogType.ACTIVE, "Kích hoạt kiểm", "PLC", "Nhận kích hoạt kiểm từ PLC");
                    //Ghi vào hàng chờ
                    ActiveLogQueue.Enqueue(activeLogs);
                    Globalvariable.ACTIVE = true;
                }
                else
                {
                    ActiveLogs activeLogs = new ActiveLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), e_ActiveLogType.ACTIVE, "Dừng kiểm", "PLC", "Nhận kích hoạt từ PLC");
                    //Ghi vào hàng chờ
                    ActiveLogQueue.Enqueue(activeLogs);
                    Globalvariable.ACTIVE = false;
                }

                Thread.Sleep(1000);
            }
        }

        //Cập nhật mã vừa đọc lên màn hình
        private void WK_Update_Result_To_UI_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!WK_UI_CAM_Update.CancellationPending)
            {
                this.Invoke((Action)(() => {
                    opContentC1.Text = Globalvariable.C1_UI.Curent_Content;
                    if (Globalvariable.C1_UI.IsPass)
                    {
                        opResultPassFailC1.Text = "TỐT";
                        opResultPassFailC1.FillColor = Color.Green;
                    }
                    else
                    {
                        opResultPassFailC1.Text = "LỖI";
                        opResultPassFailC1.FillColor = Color.Red;
                    }

                    opContentC2.Text = Globalvariable.C2_UI.Curent_Content;

                    if (Globalvariable.C2_UI.IsPass)
                    {
                        opResultPassFailC2.Text = "TỐT";
                        opResultPassFailC2.FillColor = Color.Green;
                    }
                    else
                    {
                        opResultPassFailC2.Text = "LỖI";
                        opResultPassFailC2.FillColor = Color.Red;
                    }


                    if (Alarm.Alarm1)
                    {
                        lblAlarm.Text = "CẢNH BÁO SAI BARCODE (" + Alarm.Alarm1_Count.ToString() + ")";
                        lblAlarm.FillColor = Globalvariable.NG_Color;
                    }
                }));
                Thread.Sleep(100);
            }
        }

        #endregion

        //Load lần đầu

        public string QR_Content = "Ver 18972";
        public string QR_Content_His = "";
        public string timeProcess = "0";
        public  int QRContentCount = 0;
        public bool ISPass = true;

        #region Xử lý tín hiệu từ camera

        private void Camera_ClientCallBack(SPMS1.enumClient eAE, string _strData)
        {
            switch (eAE)
            {
                case SPMS1.enumClient.CONNECTED:
                    if (GCamera.Camera_Status != e_Camera_Status.CONNECTED)
                    {
                        GCamera.Camera_Status = e_Camera_Status.CONNECTED;
                        
                    }
                    break;
                case SPMS1.enumClient.DISCONNECTED:
                    if (GCamera.Camera_Status != e_Camera_Status.DISCONNECTED)
                    {
                        GCamera.Camera_Status = e_Camera_Status.DISCONNECTED;
                        
                    }
                    break;
                case SPMS1.enumClient.RECEIVED:

                    if (GCamera.Camera_Status != e_Camera_Status.CONNECTED)
                    {
                        GCamera.Camera_Status = e_Camera_Status.CONNECTED;
                        
                    }
                    if (Globalvariable.Data_Status == e_Data_Status.READY)
                    {
                        Globalvariable.GCounter.Total_C1++;
                        try
                        {
                            if (!WK_CMR1.IsBusy)
                            {
                                WK_CMR1.RunWorkerAsync(_strData);
                            }
                            else if (!WK_CMR2.IsBusy)
                            {
                                WK_CMR2.RunWorkerAsync(_strData);
                            }
                            else if (!WK_CMR3.IsBusy)
                            {
                                WK_CMR3.RunWorkerAsync(_strData);
                            }
                            else
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi khi camera trả về : Không đủ luồng xử lí");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;

                                //ghi log lỗi
                                SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.CAMERA_ERROR, "Lỗi khi camera trả về C1", Globalvariable.CurrentUser.Username, "Không đủ luồng xử lí");
                                //thêm vào Queue để ghi log
                                SystemLogs.LogQueue.Enqueue(systemLogs);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.Invoke(new Action(() =>
                            {
                                ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi khi camera trả về : {ex.Message}");
                                ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                            }));

                            //ghi log lỗi
                            SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.CAMERA_ERROR, "Lỗi khi camera trả về C1", Globalvariable.CurrentUser.Username, ex.Message);
                            //thêm vào Queue để ghi log
                            SystemLogs.LogQueue.Enqueue(systemLogs);
                        }
                    }
                    else
                    {
                        ConUpdate("Máy chưa sẵn sàng");
                    }
                    
                    break;
                case SPMS1.enumClient.RECONNECT:
                    if (GCamera.Camera_Status != e_Camera_Status.RECONNECT)
                    {
                        GCamera.Camera_Status = e_Camera_Status.RECONNECT;
                    }
                    
                    
                    break;
            }
        }

        //camera 02
        private void Camera_c_ClientCallBack(SPMS1.enumClient eAE, string _strData)
        {
            switch (eAE)
            {
                case SPMS1.enumClient.CONNECTED:
                    if (GCamera.Camera_Status_02 != e_Camera_Status.CONNECTED)
                    {
                        GCamera.Camera_Status_02 = e_Camera_Status.CONNECTED;
                        
                    }
                    break;
                case SPMS1.enumClient.DISCONNECTED:
                    if (GCamera.Camera_Status_02 != e_Camera_Status.DISCONNECTED)
                    {
                        GCamera.Camera_Status_02 = e_Camera_Status.DISCONNECTED;
                       
                    }
                    break;
                case SPMS1.enumClient.RECEIVED:
                    if (GCamera.Camera_Status != e_Camera_Status.CONNECTED)
                    {
                        GCamera.Camera_Status = e_Camera_Status.CONNECTED;
                        
                    }
                    //xử lý dữ liệu nhận về
                    if (!WK_CMR4.IsBusy)
                    {
                        WK_CMR4.RunWorkerAsync(_strData);
                    }
                    else if (!WK_CMR5.IsBusy)
                    {
                        WK_CMR5.RunWorkerAsync(_strData);
                    }
                    else if (!WK_CMR6.IsBusy)
                    {
                        WK_CMR6.RunWorkerAsync(_strData);
                    }
                    else
                    {
                        this.Invoke(new Action(() =>
                        {
                            ipConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi khi camera 02 trả về : Không đủ luồng xử lí");
                            ipConsole.SelectedIndex = ipConsole.Items.Count - 1;
                        }));

                        Send_Result_Content_C2(e_Content_Result.ERROR, "Lỗi khi camera 02 trả về: Không đủ luồng xử lí");

                        //ghi log lỗi
                        SystemLogs systemLogs = new SystemLogs(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.Now.ToUnixTimeSeconds(), SystemLogs.e_LogType.CAMERA_ERROR, "Lỗi khi camera trả về C2", Globalvariable.CurrentUser.Username, "Không đủ luồng xử lí");
                        //thêm vào Queue để ghi log
                        SystemLogs.LogQueue.Enqueue(systemLogs);
                    }
                    break;
                case SPMS1.enumClient.RECONNECT:

                     break;
            }
        }

        public void Camera_01_Data_Recive(string _strData)
        {
            //Kích hoạt hệ thống đo đạc
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //Kiểm tra tính hợp lệ của dữ liệu
            if (!_strData.IsNullOrEmpty())
            {
                string codeClear = _strData.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");


                //bể này chỉ lưu thông tin nhằm so sánh sơ bộ
                if (GlobalSettings.GetInt("CAMERA_SLOT") > 1)
                {
                    //Kiểm tra mã đã kích hoạt hay chưa
                    if (Globalvariable.C1_Content_Dictionary.TryGetValue(codeClear, out ProductData C1ProductInfo))
                    {
                        if (C1ProductInfo.Active != 1)
                        {
                            C1ProductInfo.Active = 1;
                            C1ProductInfo.TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //chuyển từ chuỗi int time  thành string time
                            C1ProductInfo.TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                            //lấy giờ hiện tại đếm từ 1970
                            C1ProductInfo.TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                            C1ProductInfo.TimeUnixPrinted = Globalvariable.TimeUnixPrinter;

                        }
                    }

                    //chưa kích hoạt
                    else
                    {
                        if (GlobalSettings.Get("APPMODE") == "ADD_Data")
                        {
                            //nếu chưa có thì thêm mới vào C1
                            C1ProductInfo = new ProductData
                            {
                                ProductID = 0,
                                Active = 1,
                                TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                                TimeUnixPrinted = Globalvariable.TimeUnixPrinter
                            };
                            Globalvariable.C1_Content_Dictionary[codeClear] = C1ProductInfo;
                            Globalvariable.C1_Add_Content_To_SQLite_Queue.Enqueue((codeClear, C1ProductInfo));
                        }
                        else
                        {
                            //không làm gì cả
                        }

                    }
                }

                ////////////////////////////////////////
                //kiểm tra xem pass hay fail
                //Fail
                if (codeClear.Contains("FAIL"))
                {
                    //sút
                    Send_Result_Content_C1(e_Content_Result.FAIL, codeClear);
                    return;
                }
                //mã sai cấu trúc
                if (!CheckCodeFormatV2(codeClear, GlobalSettings.Get("Code_Content_Pattern")).IsOK)
                {
                    //sút
                    Send_Result_Content_C1(e_Content_Result.ERR_FORMAT, codeClear);
                    return;
                }
                //Kiểm tra mã
                if (Globalvariable.Main_Content_Dictionary.TryGetValue(codeClear, out ProductData ProductInfo))
                {
                    //mã đã active

                    //cập nhật lại thông tin
                    ProductInfo.Active = 1;
                    ProductInfo.TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    ProductInfo.TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    ProductInfo.TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    ProductInfo.TimeUnixPrinted = Globalvariable.TimeUnixPrinter;
                    //cập nhật SQLite
                    Globalvariable.Update_Content_To_SQLite_Queue.Enqueue(ProductInfo);
                    Send_Result_Content_C1(e_Content_Result.PASS, codeClear);
                    return;

                }
                //nếu mã chưa tồn tại => chưa active
                else
                {
                    //nếu khác mode thêm thì đá ra
                    if (GlobalSettings.Get("APPMODE") != "ADD_Data")
                    {
                        Send_Result_Content_C1(e_Content_Result.NOT_FOUND, codeClear);
                        return;
                    }

                    //thêm vào Dic
                    ProductInfo = new ProductData
                    {
                        ProductID = 0,
                        Active = 1,
                        TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    };

                    Globalvariable.Main_Content_Dictionary[codeClear] = ProductInfo;
                    //thêm vào hàng đợi để thêm vào SQLite
                    Globalvariable.Add_Content_To_SQLite_Queue.Enqueue((codeClear,ProductInfo));
                    //báo pass xuống PLC
                    Send_Result_Content_C1(e_Content_Result.PASS, codeClear);
                    return;
                }
               
            }
            //Mã rỗng
            else
            {
                Send_Result_Content_C1(e_Content_Result.EMPTY, "MÃ RỖNG");
                return;
            }
        }

        public void Camera_02_Data_Recive(string _strData)
        {
            //Kích hoạt hệ thống đo đạc
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //Kiểm tra tính hợp lệ của dữ liệu
            if (!_strData.IsNullOrEmpty())
            {
                string codeClear = _strData.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
                //Kiểm tra trong bể chung

                if(Globalvariable.Main_Content_Dictionary.TryGetValue(codeClear, out ProductData ProductInfo))
                {
                    //nếu đã kích hoạt thì không làm gì cả
                    if (ProductInfo.Active != 1)
                    {
                        //chưa kích hoạt thì cập nhật lại thông tin
                        ProductInfo.Active = 1;
                        ProductInfo.TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        ProductInfo.TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                        ProductInfo.TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                        ProductInfo.TimeUnixPrinted = Globalvariable.TimeUnixPrinter;
                        //cập nhật SQLite
                        Globalvariable.Update_Content_To_SQLite_Queue.Enqueue(ProductInfo);
                    }
                }
                //nếu chưa tồn tại
                else
                {
                    if (GlobalSettings.Get("APPMODE") == "ADD_Data")
                    {
                        //nếu chưa có thì thêm mới vào C1
                        ProductInfo = new ProductData
                        {
                            ProductID = 0,
                            Active = 1,
                            TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                            TimeUnixPrinted = Globalvariable.TimeUnixPrinter

                        };
                        Globalvariable.Main_Content_Dictionary[codeClear] = ProductInfo;
                        Globalvariable.Add_Content_To_SQLite_Queue.Enqueue((codeClear, ProductInfo));
                    }
                }
                //không đọc được
                if (codeClear.Contains("FAIL"))
                {
                    //sút
                    Send_Result_Content_C2(e_Content_Result.FAIL, codeClear);
                    return;
                }

                //sai cấu trúc
                if (!CheckCodeFormatV2(codeClear, GlobalSettings.Get("Code_Content_Pattern")).IsOK)
                {
                    //sút
                    Send_Result_Content_C2(e_Content_Result.ERR_FORMAT, codeClear);
                    return;
                }

                //Kiểm tra trong bể C2
                if (Globalvariable.C2_Content_Dictionary.TryGetValue(codeClear, out ProductData C2ProductInfo))
                {
                    //chế độ loại trùng bật => kiểm tra active

                    //mã chưa active thì active

                    //cập nhật SQLite
                    Globalvariable.C2_Update_Content_To_SQLite_Queue.Enqueue(C2ProductInfo);

                    //cập nhật lại thông tin
                    C2ProductInfo.Active = 1;
                    C2ProductInfo.TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    C2ProductInfo.TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    C2ProductInfo.TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    C2ProductInfo.TimeUnixPrinted = Globalvariable.TimeUnixPrinter;


                    //mã chưa active => active
                    Send_Result_Content_C2(e_Content_Result.PASS, codeClear);
                    return;
                }

                //chưa có mã
                else
                {
                    if (GlobalSettings.Get("APPMODE") != "ADD_Data")
                    {
                        Send_Result_Content_C2(e_Content_Result.NOT_FOUND, codeClear);
                        return;
                    }

                    //thêm vào Dic
                    C2ProductInfo = new ProductData
                    {
                        ProductID = 0,
                        Active = 1,
                        TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                        TimeUnixPrinted = Globalvariable.TimeUnixPrinter

                    };
                    Globalvariable.C2_Content_Dictionary[codeClear] = C2ProductInfo;
                    //thêm vào hàng đợi để thêm vào SQLite
                    Globalvariable.C2_Add_Content_To_SQLite_Queue.Enqueue((codeClear, C2ProductInfo));
                    //báo pass xuống PLC
                    Send_Result_Content_C2(e_Content_Result.PASS, codeClear);
                    return;
                }
            }
            //Mã rỗng
            else
            {
                //mã không đúng, in ra màn hình thông báo

                Send_Result_Content_C2(e_Content_Result.EMPTY, "MÃ RỖNG");
            }

        }

        #region Quản lý PLC và gửi tín hiệu PLC

        public enum e_Content_Result
        {
            PASS,//tốt
            FAIL, //lỗi
            REWORK, //thả lại
            DUPLICATE, //trùng
            EMPTY,//không có
            ERROR, //lỗi không xác định
            ERR_FORMAT, //lỗi định dạng
            NOT_FOUND //không tìm thấy mã
        }

        public void Send_Result_Content_C1(e_Content_Result content_Result, string _content)
        {
            switch (content_Result)
            {
                case e_Content_Result.PASS:

                    Globalvariable.GCounter.Total_Pass_C1++;
                    Globalvariable.C1_UI.Curent_Content = _content;
                    Globalvariable.C1_UI.IsPass = true;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write = PLC.plc.Write(PLCAddress.Get("PLC_C1_RejectDM"), short.Parse("1"));
                    if (write.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_1_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_1_Fail_C1++;
                    }
                    break;

                case e_Content_Result.FAIL:

                    Globalvariable.GCounter.Camera_Read_Fail_C1++;
                    Globalvariable.GCounter.Total_Failed_C1++;

                    Globalvariable.C1_UI.Curent_Content = "Không đọc được";
                    Globalvariable.C1_UI.IsPass = false;

                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write1 = PLC.plc.Write(PLCAddress.Get("PLC_C1_RejectDM"), short.Parse("0"));

                    if (write1.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C1++;
                    }

                    break;

                case e_Content_Result.REWORK:

                    Globalvariable.GCounter.Rework_C1++; //Cái này không cộng vào số pass nếu phát hiện trùng

                    Globalvariable.C1_UI.Curent_Content = "Thả lại:" + _content;
                    Globalvariable.C1_UI.IsPass = true;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write5 = PLC.plc.Write(PLCAddress.Get("PLC_C1_RejectDM"), short.Parse("1"));
                    if (write5.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_1_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_1_Fail_C1++;
                    }
                    break;

                case e_Content_Result.DUPLICATE:

                    Globalvariable.GCounter.Duplicate_C1++;
                    Globalvariable.GCounter.Total_Failed_C1 += 1;

                    Globalvariable.C1_UI.Curent_Content = "Mã đã kích hoạt (trùng)";
                    Globalvariable.C1_UI.IsPass = false;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write4 = PLC.plc.Write(PLCAddress.Get("PLC_C1_RejectDM"), short.Parse("0"));
                    if (write4.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C1++;
                    }
                    break;

                case e_Content_Result.EMPTY:

                    Globalvariable.GCounter.Empty_C1++;
                    Globalvariable.GCounter.Total_Failed_C1 += 1;

                    Globalvariable.C1_UI.Curent_Content = _content;
                    Globalvariable.C1_UI.IsPass = false;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write3 = PLC.plc.Write(PLCAddress.Get("PLC_C1_RejectDM"), short.Parse("0"));
                    if (write3.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C1++;
                    }
                    break;


                case e_Content_Result.ERR_FORMAT:

                    Globalvariable.GCounter.Format_C1++;
                    Globalvariable.GCounter.Total_Failed_C1++;

                    Globalvariable.C1_UI.Curent_Content = "Sai cấu trúc!!!";
                    Globalvariable.C1_UI.IsPass = false;

                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write2 = PLC.plc.Write(PLCAddress.Get("PLC_C1_RejectDM"), short.Parse("0"));
                    if (write2.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C1++;
                    }
                    break;

                case e_Content_Result.NOT_FOUND:

                    Globalvariable.GCounter.Empty_C1++;
                    Globalvariable.GCounter.Total_Failed_C1++;
                    Globalvariable.C1_UI.Curent_Content = "Mã không tồn tại";
                    Globalvariable.C1_UI.IsPass = false;
                    OperateResult write8 = PLC.plc.Write(PLCAddress.Get("PLC_C1_RejectDM"), short.Parse("0"));
                    if (write8.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C1++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C1++;
                    }
                    break;
            }
        }

        public void Send_Result_Content_C2(e_Content_Result content_Result, string _content)
        {
            switch (content_Result)
            {
                case e_Content_Result.PASS:

                    Globalvariable.C2_UI.Curent_Content = _content;
                    Globalvariable.C2_UI.IsPass = true;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write = PLC.plc.Write(PLCAddress.Get("PLC_C2_RejectDM"), short.Parse("1"));
                    if (write.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_1_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_1_Fail_C2++;
                    }
                    break;

                case e_Content_Result.FAIL:

                    Globalvariable.C2_UI.Curent_Content = "Không đọc được";
                    Globalvariable.C2_UI.IsPass = false;

                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write1 = PLC.plc.Write(PLCAddress.Get("PLC_C2_RejectDM"), short.Parse("0"));

                    if (write1.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C2++;
                    }

                    break;

                case e_Content_Result.REWORK:
                    Globalvariable.C2_UI.Curent_Content = "Thả lại:" + _content;
                    Globalvariable.C2_UI.IsPass = true;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write5 = PLC.plc.Write(PLCAddress.Get("PLC_C2_RejectDM"), short.Parse("1"));
                    if (write5.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_1_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_1_Fail_C2++;
                    }
                    break;

                case e_Content_Result.DUPLICATE:
                    Globalvariable.C2_UI.Curent_Content = "Mã đã kích hoạt (trùng)";
                    Globalvariable.C2_UI.IsPass = false;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write4 = PLC.plc.Write(GlobalSettings.Get("PLC_C2_RejectDM"), short.Parse("0"));
                    if (write4.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C2++;
                    }
                    break;

                case e_Content_Result.EMPTY:
                    Globalvariable.C2_UI.Curent_Content = _content;
                    Globalvariable.C2_UI.IsPass = false;
                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write3 = PLC.plc.Write(GlobalSettings.Get("PLC_C2_RejectDM"), short.Parse("0"));
                    if (write3.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C2++;
                    }
                    break;

                case e_Content_Result.ERROR:

                    Globalvariable.C2_UI.Curent_Content = _content;
                    Globalvariable.C2_UI.IsPass = false;

                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write9 = PLC.plc.Write(GlobalSettings.Get("PLC_C2_RejectDM"), short.Parse("0"));
                    if (write9.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C2++;
                    }
                    break;
                case e_Content_Result.ERR_FORMAT:

                    Globalvariable.C2_UI.Curent_Content = "Sai cấu trúc!!!";
                    Globalvariable.C2_UI.IsPass = false;

                    //gửi xuống PLC và xử lý tại đây
                    OperateResult write2 = PLC.plc.Write(PLCAddress.Get("PLC_C2_RejectDM"), short.Parse("0"));
                    if (write2.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C2++;
                    }
                    break;

                case e_Content_Result.NOT_FOUND:

                    Globalvariable.C2_UI.Curent_Content = "Mã không tồn tại";
                    Globalvariable.C2_UI.IsPass = false;
                    OperateResult write8 = PLC.plc.Write(PLCAddress.Get("PLC_C2_RejectDM"), short.Parse("0"));
                    if (write8.IsSuccess)
                    {
                        Globalvariable.GCounter.PLC_0_Pass_C2++;
                    }
                    else
                    {
                        Globalvariable.GCounter.PLC_0_Fail_C2++;
                    }
                    break;
            }
        }

        private void PLC_PLCStatus_OnChange(object sender, SPMS1.OmronPLC_Hsl.PLCStatusEventArgs e)
        {
            switch (e.Status)
            {
                case SPMS1.OmronPLC_Hsl.PLCStatus.Connecting:
                    Globalvariable.PLCConnect = true;
                    break;
                case SPMS1.OmronPLC_Hsl.PLCStatus.Disconnect:
                    Globalvariable.PLCConnect = false;
                    break;
            }
        }
        #endregion


        public (bool IsOK, string Message) CheckCodeFormatV2(string code, string pattern)
        {
            // Kiểm tra định dạng mã QR
            if (string.IsNullOrEmpty(code))
            {
                return (false, "Mã QR không được để trống.");
            }
            // i.tcx.com.vn/[N13]0A509[N5][SN8] : các ký tự trong ngoặc [] là các ký tự quy định ví dụ N13 là 13 số, S8 là 8 ký tự chữ, SN8 là 8 ký tự chữ và số, dự vào đây kiểm tra định dạng mã
            if (!System.Text.RegularExpressions.Regex.IsMatch(code, pattern))
            {
                return (false, "Sai định dạng");
            }

            return (true, "Mã QR hợp lệ.");
        }

        public void Update_Active_Status_Main(ProductData _productInfo)
        {
            
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename};Version=3;"))
            {
                connection.Open();
                string query = "UPDATE `QRContent` SET " +
                               "`Active` = '1', " +
                               "`TimeStampActive` = @TimeStampActive, " +
                               "`TimeUnixActive` = @TimeUnixActive  " +
                               "`TimeStampPrinted` = @TimestampPrinted" +
                               "`TimeUnixPrinted` = @TimeUnixPrinted" +
                               "WHERE _rowid_ = @RowId";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RowId", _productInfo.ProductID);
                    command.Parameters.AddWithValue("@TimeStampActive", _productInfo.TimeStampActive);
                    command.Parameters.AddWithValue("@TimeUnixActive",_productInfo.TimeUnixActive);

                    command.Parameters.AddWithValue("@TimeStampPrinted", _productInfo.TimeStampPrinted);
                    command.Parameters.AddWithValue("@TimeUnixPrinted", _productInfo.TimeUnixPrinted);
                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
        }
        public void Add_Content_To_SQLite_Main((string Content, ProductData _productinfo) _Queue)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename};Version=3;"))
            {
                connection.Open();
                string query = "INSERT INTO `QRContent` " +
                            "(ProductQR, Active, TimestampActive, TimeUnixActive, TimestampPrinted, TimeUnixPrinted) " +
                    "VALUES (@QR,1,@TimeStamp,@TimeUnix,@TimestampPrinted, @TimeUnixPrinted);";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@QR", _Queue.Content);
                    command.Parameters.AddWithValue("@TimeStamp", _Queue._productinfo.TimeStampActive);
                    command.Parameters.AddWithValue("@TimeUnix", _Queue._productinfo.TimeUnixActive);
                    command.Parameters.AddWithValue("@TimestampPrinted", _Queue._productinfo.TimeStampPrinted);
                    command.Parameters.AddWithValue("@TimeUnixPrinted", _Queue._productinfo.TimeUnixPrinted);

                    int rowsAffected = command.ExecuteNonQuery();
                }

                using (SQLiteCommand cmd = new SQLiteCommand("SELECT last_insert_rowid();", connection))
                {
                    long id = (long)cmd.ExecuteScalar();

                    //cập nhật vào Globalvariable.Main_Content_Dictionary
                    if (Globalvariable.Main_Content_Dictionary.TryGetValue(_Queue.Content, out ProductData productData))
                    {
                        // Nếu đã có trong dictionary, cập nhật ProductID
                        productData.ProductID = Convert.ToInt32(id);
                    }
                    else
                    {
                        // Nếu chưa có, thêm mới vào dictionary
                        Globalvariable.Main_Content_Dictionary[_Queue.Content] = new ProductData
                        {
                            ProductID = Convert.ToInt32(id),
                            Active = 1,
                            TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                            TimeUnixPrinted = Globalvariable.TimeUnixPrinter

                        };
                    }
                }
            }
        }

        public void Update_Active_Status_C1(ProductData _productInfo)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename_C1};Version=3;"))
            {
                connection.Open();
                string query = "UPDATE `QRContent` SET " +
                               "`Active` = '1', " +
                               "`TimeStampActive` = @TimeStampActive, " +
                               "`TimeUnixActive` = @TimeUnixActive  " +
                               "`TimeStampPrinted` = @TimestampPrinted" +
                               "`TimeUnixPrinted` = @TimeUnixPrinted" +
                               "WHERE _rowid_ = @RowId";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RowId", _productInfo.ProductID);
                    command.Parameters.AddWithValue("@TimeStampActive", _productInfo.TimeStampActive);
                    command.Parameters.AddWithValue("@TimeUnixActive", _productInfo.TimeUnixActive);

                    command.Parameters.AddWithValue("@TimeStampPrinted", _productInfo.TimeStampPrinted);
                    command.Parameters.AddWithValue("@TimeUnixPrinted", _productInfo.TimeUnixPrinted);
                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
        }
        public void Add_Content_To_SQLite_C1((string Content, ProductData _productinfo) _Queue)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename_C1};Version=3;"))
            {
                connection.Open();
                string query = "INSERT INTO `QRContent` " +
                            "(ProductQR, Active, TimestampActive, TimeUnixActive, TimestampPrinted, TimeUnixPrinted) " +
                    "VALUES (@QR,1,@TimeStamp,@TimeUnix,@TimestampPrinted, @TimeUnixPrinted);";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@QR", _Queue.Content);
                    command.Parameters.AddWithValue("@TimeStamp", _Queue._productinfo.TimeStampActive);
                    command.Parameters.AddWithValue("@TimeUnix", _Queue._productinfo.TimeUnixActive);
                    command.Parameters.AddWithValue("@TimestampPrinted", _Queue._productinfo.TimeStampPrinted);
                    command.Parameters.AddWithValue("@TimeUnixPrinted", _Queue._productinfo.TimeUnixPrinted);

                    int rowsAffected = command.ExecuteNonQuery();
                }

                using (SQLiteCommand cmd = new SQLiteCommand("SELECT last_insert_rowid();", connection))
                {
                    long id = (long)cmd.ExecuteScalar();

                    //cập nhật vào Globalvariable.Main_Content_Dictionary
                    if (Globalvariable.C1_Content_Dictionary.TryGetValue(_Queue.Content, out ProductData productData))
                    {
                        // Nếu đã có trong dictionary, cập nhật ProductID
                        productData.ProductID = Convert.ToInt32(id);
                    }
                    else
                    {
                        // Nếu chưa có, thêm mới vào dictionary
                        Globalvariable.C1_Content_Dictionary[_Queue.Content] = new ProductData
                        {
                            ProductID = Convert.ToInt32(id),
                            Active = 1,
                            TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                            TimeUnixPrinted = Globalvariable.TimeUnixPrinter

                        };
                    }
                }
            }
        }

        public void Update_Active_Status_C2(ProductData _productInfo)
        {

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename_C2};Version=3;"))
            {
                connection.Open();
                string query = "UPDATE `QRContent` SET " +
                               "`Active` = '1', " +
                               "`TimeStampActive` = @TimeStampActive, " +
                               "`TimeUnixActive` = @TimeUnixActive  " +
                               "`TimeStampPrinted` = @TimestampPrinted" +
                               "`TimeUnixPrinted` = @TimeUnixPrinted" +
                               "WHERE _rowid_ = @RowId";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RowId", _productInfo.ProductID);
                    command.Parameters.AddWithValue("@TimeStampActive", _productInfo.TimeStampActive);
                    command.Parameters.AddWithValue("@TimeUnixActive", _productInfo.TimeUnixActive);

                    command.Parameters.AddWithValue("@TimeStampPrinted", _productInfo.TimeStampPrinted);
                    command.Parameters.AddWithValue("@TimeUnixPrinted", _productInfo.TimeUnixPrinted);
                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
        }
        public void Add_Content_To_SQLite_C2((string Content, ProductData _productinfo) _Queue)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={_clientMFI.Data_Content_Folder + _clientMFI.Data_Content_Filename_C2};Version=3;"))
            {
                connection.Open();
                string query = "INSERT INTO `QRContent` " +
                            "(ProductQR, Active, TimestampActive, TimeUnixActive, TimestampPrinted, TimeUnixPrinted) " +
                    "VALUES (@QR,1,@TimeStamp,@TimeUnix,@TimestampPrinted, @TimeUnixPrinted);";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@QR", _Queue.Content);
                    command.Parameters.AddWithValue("@TimeStamp", _Queue._productinfo.TimeStampActive);
                    command.Parameters.AddWithValue("@TimeUnix", _Queue._productinfo.TimeUnixActive);
                    command.Parameters.AddWithValue("@TimestampPrinted", _Queue._productinfo.TimeStampPrinted);
                    command.Parameters.AddWithValue("@TimeUnixPrinted", _Queue._productinfo.TimeUnixPrinted);

                    int rowsAffected = command.ExecuteNonQuery();
                }

                using (SQLiteCommand cmd = new SQLiteCommand("SELECT last_insert_rowid();", connection))
                {
                    long id = (long)cmd.ExecuteScalar();

                    //cập nhật vào Globalvariable.Main_Content_Dictionary
                    if (Globalvariable.C2_Content_Dictionary.TryGetValue(_Queue.Content, out ProductData productData))
                    {
                        // Nếu đã có trong dictionary, cập nhật ProductID
                        productData.ProductID = Convert.ToInt32(id);
                    }
                    else
                    {
                        // Nếu chưa có, thêm mới vào dictionary
                        Globalvariable.C2_Content_Dictionary[_Queue.Content] = new ProductData
                        {
                            ProductID = Convert.ToInt32(id),
                            Active = 1,
                            TimeStampActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            TimeStampPrinted = DateTimeOffset.FromUnixTimeSeconds(Globalvariable.TimeUnixPrinter).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            TimeUnixActive = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                            TimeUnixPrinted = Globalvariable.TimeUnixPrinter

                        };
                    }
                }
            }
        }

        #endregion

        private void btnResetCounter_Click(object sender, EventArgs e)
        {

            OperateResult write = PLC.plc.Write("D22", short.Parse("1"));
            if (write.IsSuccess)
            {

                lblFail.Value = 0;
                lblPass.Value = 0;
                lblTotal.Value = 0;
            }
            else
            {

            }
        }

        private void btnClearPLC_Click(object sender, EventArgs e)
        {
            btnClearPLC.Enabled = false;
            btnClearPLC.Text = "Đang xóa lỗi";
            OperateResult write = PLC.plc.Write("D18", short.Parse("1"));
            if (write.IsSuccess)
            {
                btnClearPLC.Enabled = true;

                btnClearPLC.Text = "Xóa lỗi PLC";

                Alarm.Alarm1 = false;
                Alarm.Alarm1_Count = 0;

                lblAlarm.Text = "-";
                lblAlarm.FillColor = Globalvariable.OK_Color;

            }
            else
            {
                ClearPLC = true;
                //btnClearPLC.Enabled = true;
            }
        }

        private void WK_Process_SQLite_DoWork(object sender, DoWorkEventArgs e)
        {
            while(!WK_UI_CAM_Update.CancellationPending)
            {
                
                if(Globalvariable.TimeUnixPrinter == 0)
                {
                    Globalvariable.TimeUnixPrinter = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                }
                //Bể chính

                //cập nhật
                if (Globalvariable.Update_Content_To_SQLite_Queue.Count > 0)
                {
                    Update_Active_Status_Main(Globalvariable.Update_Content_To_SQLite_Queue.Dequeue());
                }

                //thêm mới
                if(Globalvariable.Add_Content_To_SQLite_Queue.Count > 0)
                {
                    Add_Content_To_SQLite_Main(Globalvariable.Add_Content_To_SQLite_Queue.Dequeue());
                }

                //Bể C1
                //cập nhật
                if (Globalvariable.C1_Update_Content_To_SQLite_Queue.Count > 0)
                {
                    Update_Active_Status_C1(Globalvariable.C1_Update_Content_To_SQLite_Queue.Dequeue());
                }
                //thêm mới
                if (Globalvariable.C1_Add_Content_To_SQLite_Queue.Count > 0)
                {
                    Add_Content_To_SQLite_C1(Globalvariable.C1_Add_Content_To_SQLite_Queue.Dequeue());
                }
                //Bể C2
                //cập nhật
                if (Globalvariable.C2_Update_Content_To_SQLite_Queue.Count > 0)
                {
                    Update_Active_Status_C2(Globalvariable.C2_Update_Content_To_SQLite_Queue.Dequeue());
                }
                //thêm mới
                if (Globalvariable.C2_Add_Content_To_SQLite_Queue.Count > 0)
                {
                    Add_Content_To_SQLite_C2(Globalvariable.C2_Add_Content_To_SQLite_Queue.Dequeue());
                }
                Thread.Sleep(100);
            }
        }

        #region Các luồng xử lý dữ liệu khi có tín hiệu
        private double maxTimeT1 = 0;
        private double maxTimeT2 = 0;
        private double maxTimeT3 = 0;
        private double maxTimeT4 = 0;
        private double maxTimeT5 = 0;
        private double maxTimeT6 = 0;
        private void WK_CMR1_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;
            
            Camera_01_Data_Recive(inputString);

            stopwatch.Stop();

                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT1)
                {
                    maxTimeT1 = stopwatch.Elapsed.TotalMilliseconds;
                }
                Globalvariable.GCounter.WK1_TimeProcess_C1 = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT1}" ;

        }
        private void WK_CMR2_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;

            //WhenDataRecive(inputString);
            Camera_01_Data_Recive(inputString);

            stopwatch.Stop();

            if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT2)
            {
                maxTimeT2 = stopwatch.Elapsed.TotalMilliseconds;
            }
            Globalvariable.GCounter.WK2_TimeProcess_C1 = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT2}";

        }
        private void WK_CMR3_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;

            //WhenDataRecive(inputString);
            Camera_01_Data_Recive(inputString);

            stopwatch.Stop();

                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT3)
                {
                    maxTimeT3 = stopwatch.Elapsed.TotalMilliseconds;
                }
            Globalvariable.GCounter.WK3_TimeProcess_C1 = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT3}";
        }
        

        private void WK_CMR4_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;

            Camera_02_Data_Recive(inputString);

            stopwatch.Stop();

                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT4)
                {
                    maxTimeT4 = stopwatch.Elapsed.TotalMilliseconds;
                }
                Globalvariable.GCounter.WK4_TimeProcess_C2 = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT4}";
            }
        private void WK_CMR5_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;
            
            Camera_02_Data_Recive(inputString);
            
            stopwatch.Stop();

                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT5)
                {
                    maxTimeT5 = stopwatch.Elapsed.TotalMilliseconds;
                }
                Globalvariable.GCounter.WK5_TimeProcess_C2 = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT5}";
            }
        private void WK_CMR6_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            // Nhận dữ liệu truyền vào
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string inputString = e.Argument as string;

            Camera_02_Data_Recive(inputString);

            stopwatch.Stop();

                if (stopwatch.Elapsed.TotalMilliseconds > maxTimeT6)
                {
                    maxTimeT6 = stopwatch.Elapsed.TotalMilliseconds;
                }
                Globalvariable.GCounter.WK5_TimeProcess_C2 = $"{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 4).ToString()}/{maxTimeT5}";
            }

        #endregion

        private void uiTitlePanel5_Click(object sender, EventArgs e)
        {

        }

        private void ipConsole_DoubleClick(object sender, EventArgs e)
        {
            this.ShowInfoDialog(ipConsole.SelectedItem as string);
        }

        public void SendUnActive()
        {
            OperateResult operateResult = PLC.plc.Write(PLCAddress.Get("PLC_Bypass_DM_C1"),int.Parse("1"));
            OperateResult operateResult2 = PLC.plc.Write(PLCAddress.Get("PLC_Bypass_DM_C2"), int.Parse("1"));
            //không cần đợi trả về làm gì
        }

        public void LoadPO ()
        {

        }

        

        private void btnUpdatePO_Click(object sender, EventArgs e)
        {
            poService.LoadOrderNoToComboBox(ipOrderNO);
        }


        DataTable dataTable1 = new DataTable();
        private void ipOrderNO_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataTable1 = poService.GetPOByOrderNo(ipOrderNO.Text);
            if(dataTable1.Rows.Count > 0 )
            {
                opUniqueCode.Text = dataTable1.Rows[0]["uniqueCode"].ToString();
                opSite.Text = dataTable1.Rows[0]["site"].ToString();
                opFactory.Text = dataTable1.Rows[0]["factory"].ToString();
                opProductionLine.Text = dataTable1.Rows[0]["productionLine"].ToString();
                opProductionDate.Text = dataTable1.Rows[0]["productionDate"].ToString();
                opShift.Text = dataTable1.Rows[0]["shift"].ToString();
            }
        }

        private void btnSavePO_Click(object sender, EventArgs e)
        {
            //kiểm tra các thông tin PO
            ConUpdate("Bắt đầu điều chỉnh PO");
            //Lấy thông tin file PO

        }

        private void WK_LoadPO_DoWork(object sender, DoWorkEventArgs e)
        {
           
        }
    }
}