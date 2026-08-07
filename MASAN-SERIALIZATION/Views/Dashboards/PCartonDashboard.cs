using DATALOGIC_SCAN;
using MASAN_SERIALIZATION.Configs;
using MASAN_SERIALIZATION.Enums;
using MASAN_SERIALIZATION.Production;
using MASAN_SERIALIZATION.Utils;
using SpT.Logs;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MASAN_SERIALIZATION.Views.Dashboards
{
    public partial class PCartonDashboard : UIPage
    {
        private bool IsTestModeEnabled => AppConfigs.Current.TestMode;

        Connection _ScanConection01 = new Connection();
        Connection _ScanConection02 = new Connection();

        BackgroundWorker _bw_update_ui = new BackgroundWorker();
        BackgroundWorker _bw_http_server = new BackgroundWorker();

        private HttpListener _httpListener;
        private readonly int _httpPort = 9999;
        private bool _httpServerRunning = false;

        //tạo file log 
        private LogHelper<e_LogType> PCLog;

        public PCartonDashboard()
        {
            InitializeComponent();
            PCLog = new LogHelper<e_LogType>(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MASAN-SERIALIZATION", "Logs", "Pages", "PClog.ptl"));
        }

        #region Các hàm khởi tạo

        public void INIT()
        {
            if(AppConfigs.Current.cartonScanerMode != 1)
            {
                _ScanConection01.SERIALPORT = serialPort1;
                _ScanConection01.EVENT += _ScanConection01_EVENT;
                _ScanConection01.LOAD();
                _ScanConection01.CONNECT(AppConfigs.Current.HandScanCOM01);

                _ScanConection02.SERIALPORT = serialPort2;
                _ScanConection02.EVENT += _ScanConection02_EVENT;
                _ScanConection02.LOAD();
                _ScanConection02.CONNECT(AppConfigs.Current.HandScanCOM02);
            }
            else
            {
                tcpClient1.IP = AppConfigs.Current.cartonScanerTCP_IP;
                tcpClient1.Port = AppConfigs.Current.cartonScanerTCP_Port;

                tcpClient1.Connect();
            }

            _bw_update_ui.WorkerSupportsCancellation = true;
            _bw_update_ui.DoWork += bw_update_ui;

            if (!_bw_update_ui.IsBusy)
            {
                _bw_update_ui.RunWorkerAsync();
            }
            else
            {
                this.ShowErrorDialog("Vui lòng không thao tác liên tiếp nhiều lần");
            }

            // Khởi động HTTP Server BackgroundWorker
            _bw_http_server.WorkerSupportsCancellation = true;
            _bw_http_server.DoWork += bw_http_server_DoWork;
            if (!_bw_http_server.IsBusy)
            {
                _bw_http_server.RunWorkerAsync();
            }


        }

        string lastWarning = string.Empty;
        private void bw_update_ui(object sender, DoWorkEventArgs e)
        {
            while (!_bw_update_ui.CancellationPending)
            {
                try
                {
                    int lastID = Globals.ProductionData.counter.cartonID - 1;
                    int nextID = Globals.ProductionData.counter.cartonID + 1;
                    this.InvokeIfRequired(() =>
                    {
                        uiLabel1.Text = Globals.test.ToString();
                        uiLabel2.Text = Globals.test2.ToString();

                        opCartonMaxID.Text = Globals.ProductionData.counter.cartonID.ToString();
                        opCartonCode.Text = Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID, out ProductionCartonData cartonData) ? cartonData.cartonCode : "Chưa có mã thùng";
                        opcartonPackCount.Text = Globals.ProductionData.counter.carton_Packing_Count.ToString();


                        opLastActive.Text = Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID - 1, out ProductionCartonData cartonData1) ? cartonData1.Activate_Datetime : "Chưa có thời gian kích hoạt";
                        opLastID.Text = (Globals.ProductionData.counter.cartonID - 1).ToString();
                        opLastCode.Text = Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID - 1, out ProductionCartonData cartonData2) ? cartonData2.cartonCode : "Chưa có mã thùng";

                        opnextID.Text = (Globals.ProductionData.counter.cartonID + 1).ToString();
                        opnextCode.Text = Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData cartonData3) ? cartonData3.cartonCode : "Chưa có mã thùng";
                        opnextStart.Text = Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData cartonData4) ? cartonData4.Start_Datetime : "Chưa có thời gian kích hoạt";

                        uiLabel3.Text = Globals.HandScan01_Connected.ToString() + "/" + Globals.HandScan02_Connected;

                        if (Globals.Canhbao != lastWarning)
                        {
                            lastWarning = Globals.Canhbao;
                            opWarning.Items.Insert(0, Globals.Canhbao);
                        }
                    });

                }
                catch (Exception ex)
                {
                    PCLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, " Lỗi PC01 :" + ex.Message);
                }

                Thread.Sleep(500); // Đợi 1 giây trước khi lặp lại
            }
        }

        #endregion

        #region HTTP Server

        // Classes for JSON serialization
        public class CartonScanRequest
        {
            public string machineName { get; set; }
            public string cartonCode { get; set; }
            public string scannedAt { get; set; }
            public string mode { get; set; }
        }

        public class CartonScanResponse
        {
            public bool success { get; set; }
            public string message { get; set; }
            public string status { get; set; }
            public int cartonIndex { get; set; }
            public string orderNo { get; set; }
            public int productCount { get; set; }
            public string activateDate { get; set; }
        }

        private void bw_http_server_DoWork(object sender, DoWorkEventArgs e)
        {
            _httpServerRunning = true;
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"http://+:{_httpPort}/");

            try
            {
                _httpListener.Start();
                LogHttpServerStatus("HTTP Server started on port " + _httpPort);

                while (!_bw_http_server.CancellationPending)
                {
                    try
                    {
                        var context = _httpListener.GetContext();
                        HandleHttpRequests(context);
                    }
                    catch (HttpListenerException) when (_bw_http_server.CancellationPending)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (!_bw_http_server.CancellationPending)
                        {
                            PCLog.WriteLogAsync("System", e_LogType.Error, "HTTP Request Error: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PCLog.WriteLogAsync("System", e_LogType.Error, "HTTP Server Error: " + ex.Message);
            }
            finally
            {
                _httpServerRunning = false;
                if (_httpListener != null && _httpListener.IsListening)
                {
                    _httpListener.Stop();
                    _httpListener.Close();
                }
                LogHttpServerStatus("HTTP Server stopped");
            }
        }

        private void HandleHttpRequests(HttpListenerContext context)
        {
            try
            {
                string path = context.Request.Url.AbsolutePath;
                string method = context.Request.HttpMethod;

                if (path == "/health" && method == "GET")
                {
                    HandleHealthCheck(context);
                }
                else if (path == "/carton/scan" && method == "POST")
                {
                    HandleCartonRequest(context);
                }
                else
                {
                    SendJsonResponse(context, 404, false, "Not Found", null);
                }
            }
            catch (Exception ex)
            {
                PCLog.WriteLogAsync("System", e_LogType.Error, "HandleHttpRequests Error: " + ex.Message);
                SendJsonResponse(context, 500, false, "Internal Server Error: " + ex.Message, null);
            }
        }

        private void HandleHealthCheck(HttpListenerContext context)
        {
            var response = new
            {
                status = "ok",
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            SendJsonResponse(context, 200, true, "OK", response);
        }

        private void HandleCartonRequest(HttpListenerContext context)
        {
            using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            {
                string jsonBody = reader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(jsonBody))
                {
                    SendJsonResponse(context, 400, false, "Request body is empty", null);
                    return;
                }

                CartonScanRequest request;
                try
                {
                    request = Newtonsoft.Json.JsonConvert.DeserializeObject<CartonScanRequest>(jsonBody);
                }
                catch
                {
                    // Fallback: try parsing as pipe-separated format for backward compatibility
                    string[] parts = jsonBody.Split('|');
                    if (parts.Length >= 2)
                    {
                        request = new CartonScanRequest
                        {
                            cartonCode = parts[0].Trim(),
                            machineName = parts[1].Trim()
                        };
                    }
                    else
                    {
                        SendJsonResponse(context, 400, false, "Invalid format. Expected JSON or <Mã thùng>|<Tên máy>", null);
                        return;
                    }
                }

                if (request == null || string.IsNullOrEmpty(request.cartonCode))
                {
                    SendJsonResponse(context, 400, false, "Invalid request: cartonCode is required", null);
                    return;
                }

                string cartonCode = request.cartonCode.Trim();
                string machineName = request.machineName ?? "";

                PCLog.WriteLogAsync("HTTP", e_LogType.DataChange, $"Received carton: {cartonCode} from {machineName}");

                // Xử lý carton dựa trên tên máy
                if (machineName.Contains("Lane01") || machineName.Contains("Line01") || machineName.Contains("SC0"))
                {
                    HandScan01_Process(cartonCode);
                }
                else if (machineName.Contains("Lane02") || machineName.Contains("Line02") || machineName.Contains("SC1"))
                {
                    HandScan02_Process(cartonCode);
                }
                else
                {

                    //kiểm tra xem thùng đang chạy có code chưa

                    if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData cartonDatah1))
                    { 
                        //kiểm tra xem chẵn hay lẻ 
                        if(Globals.ProductionData.counter.cartonID %2==0)
                        {
                            //đúng là chẵn (thùng 2)
                            if (cartonDatah1.cartonCode == "0")
                            {
                                HandScan02_Process(cartonCode);
                            }
                            else
                            {
                                HandScan01_Process(cartonCode);
                            }
                        }
                        else
                        {
                            //đúng là lẻ (thùng 1)
                            if (cartonDatah1.cartonCode == "0")
                            {
                                HandScan01_Process(cartonCode);
                            }
                            else
                            {
                                HandScan02_Process(cartonCode);
                            }
                        }

                    }

                }

                // Lấy thông tin carton sau khi xử lý
                int currentCartonID = Globals.ProductionData.counter.cartonID;
                string currentCartonCode = "";
                string currentStatus = "OK";
                string currentOrderNo = Globals.ProductionData.orderNo ?? "";
                int productCount = Globals.ProductionData.counter.carton_Packing_Count;
                string activateDate = "";

                if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(currentCartonID, out ProductionCartonData cartonData))
                {
                    currentCartonCode = cartonData.cartonCode;
                    activateDate = cartonData.Activate_Datetime ?? "";
                    if (cartonData.Production_Datetime != "0")
                    {
                        currentStatus = "COMPLETED";
                    }
                    else if (cartonData.Start_Datetime != "0")
                    {
                        currentStatus = "IN_PROGRESS";
                    }
                    else
                    {
                        currentStatus = "PENDING";
                    }
                }

                var response = new CartonScanResponse
                {
                    success = true,
                    message = "Carton scanned successfully",
                    status = currentStatus,
                    cartonIndex = currentCartonID,
                    orderNo = currentOrderNo,
                    productCount = productCount,
                    activateDate = activateDate
                };
                SendCartonScanResponse(context, 200, response);
            }
        }

        private void SendCartonScanResponse(HttpListenerContext context, int statusCode, CartonScanResponse response)
        {
            try
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                PCLog.WriteLogAsync("System", e_LogType.Error, "SendCartonScanResponse Error: " + ex.Message);
            }
        }

        private void SendJsonResponse(HttpListenerContext context, int statusCode, bool success, string message, object data)
        {
            try
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;

                var jsonResponse = new
                {
                    success = success,
                    message = message,
                    data = data
                };

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonResponse);
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                PCLog.WriteLogAsync("System", e_LogType.Error, "SendJsonResponse Error: " + ex.Message);
            }
        }

        private void LogHttpServerStatus(string message)
        {
            try
            {
                this.InvokeIfRequired(() =>
                {
                    opLane01.Items.Insert(0, $"[HTTP] {message}");
                });
            }
            catch { }
        }

        private void StopHttpServer()
        {
            if (_bw_http_server.IsBusy)
            {
                _bw_http_server.CancelAsync();
            }
            PCLog.WriteLogAsync("System", e_LogType.Info, "HTTP Server stop requested");
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            StopHttpServer();
            if (_bw_update_ui.IsBusy)
            {
                _bw_update_ui.CancelAsync();
            }
            base.Dispose(disposing);
        }

        //scan chẵn
        private void _ScanConection02_EVENT(e_Serial e, string s)
        {
            switch (e)
            {
                case e_Serial.Connected:
                    if (!Globals.HandScan02_Connected)
                    {
                        Globals.HandScan02_Connected = true;
                    }
                    break;
                case e_Serial.Disconnected:
                    if (Globals.HandScan02_Connected)
                    {
                        Globals.HandScan02_Connected = false;
                    }
                    break;
                case e_Serial.Recive:

                    this.InvokeIfRequired(() =>
                    {
                        opLane02.Items.Insert(0, s.Trim());
                    });

                    if (AppConfigs.Current.cartonScaner_Only_Once)
                    { 
                        break;
                    }
                    HandScan02_Process(s);
                    break;
            }
        }

        string saveLine = AppConfigs.Current.cartonCode_Line01;
        private void _ScanConection01_EVENT(e_Serial e, string s)
        {
            switch (e)
            {
                case e_Serial.Connected:
                    if (!Globals.HandScan01_Connected)
                    {
                        Globals.HandScan01_Connected = true;
                    }
                    break;
                case e_Serial.Disconnected:

                    if (Globals.HandScan01_Connected)
                    {
                        Globals.HandScan01_Connected = false;
                    }
                    break;
                case e_Serial.Recive:
                    this.InvokeIfRequired(() =>
                    {
                        opLane01.Items.Insert(0, s.Trim());
                        opLane01.Items.Insert(0, saveLine);
                    });

                    if (AppConfigs.Current.cartonScaner_Only_Once)
                    {
                        if (s.Contains(AppConfigs.Current.cartonCode_Line01) || s.Contains(AppConfigs.Current.cartonCode_Line02))
                        {
                            
                            saveLine = s.Trim();
                            this.InvokeIfRequired(() =>
                            {

                                opLane01.Items.Insert(0, "Đổi Line " + saveLine);
                            });
                        }
                        else
                        {
                            if(saveLine.Contains(AppConfigs.Current.cartonCode_Line01))
                            {
                                this.InvokeIfRequired(() =>
                                {

                                    opLane01.Items.Insert(0, "Xử lý Lane 01" + saveLine);
                                });
                                HandScan01_Process(s.Trim());
                            }
                            else if (saveLine.Contains(AppConfigs.Current.cartonCode_Line02))
                            {
                                this.InvokeIfRequired(() =>
                                {

                                    opLane01.Items.Insert(0, "Xử lý Lane 02" + saveLine);
                                });
                                HandScan02_Process(s.Trim());
                            }
                        }
                    }
                    else
                    {
                        HandScan01_Process(s);
                    }
                    
                    break;
            }


        }

        private void btnSend1_Click(object sender, EventArgs e)
        {
            string test1 = ipTest1.Text.Split('-')[2];
            int test1Int = Convert.ToInt32(test1);
            test1Int++;
            ipTest1.Text = ipTest1.Text.Split('-')[0] + "-" + ipTest1.Text.Split('-')[1] + "-" + test1Int.ToString();
            HandScan01_Process(ipTest1.Text);
        }

        private void HandScan02_Process(string s)
        {
            //lưu log scan
            PCLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.DataChange, "Scan 02" + s.Trim(), s.Trim());
            //kiểm tra xem thùng đang chạy chẵn hay lẻ
            if (Globals.ProductionData.counter.cartonID % 2 != 0)
            {
                //thùng lẻ
                //kiểm tra xem thùng tiếp theo có mã chưa
                if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData cartonData))
                {
                    //nếu thùng tiếp theo đã có mã thì cập nhật mã thùng mới
                    if (cartonData.cartonCode != "0")
                    {
                        //Đã có mã thùng thì không làm gì cả
                    }
                    else
                    {
                        //kiểm tra mã đã từng tồn tại chưa
                        if (!IsTestModeEnabled && Globals_Database.Dictionary_ProductionCarton_Data.Values.Any(x => x.cartonCode == s.Trim()))
                        {
                            this.InvokeIfRequired(() =>
                            {
                                this.ShowErrorNotifier("Mã thùng đã tồn tại. Vui lòng kiểm tra lại!", false, 5000);
                            });
                            return;
                        }
                        else
                        {
                            //nếu chưa tồn tại thì cập nhật mã thùng mới
                            cartonData.cartonCode = s.Trim();
                            cartonData.Start_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700");
                            //thêm vào hàng chờ cập nhật
                            Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonData);
                            //Globals_Database.Activate_Carton.Enqueue(s.Trim());
                        }
                    }
                }

            }
            else
            {

                //thùng chẵn tức là thùng đang chạy
                //kiểm tra xem thùng chẵn có mã chưa
                if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID, out ProductionCartonData cartonDataL))
                {
                    //nếu thùng đã có mã thì không làm gì cả
                    if (cartonDataL.cartonCode != "0")
                    {
                        //Đã có mã thùng thì không làm gì cả
                    }
                    else
                    {
                        //kiểm tra mã đã từng tồn tại chưa
                        if (!IsTestModeEnabled && Globals_Database.Dictionary_ProductionCarton_Data.Values.Any(x => x.cartonCode == s.Trim()))
                        {
                            this.InvokeIfRequired(() =>
                            {
                                this.ShowErrorNotifier("Mã thùng đã tồn tại. Vui lòng kiểm tra lại!", false, 5000);
                            });
                            return;
                        }
                        else
                        {
                            //nếu chưa tồn tại thì cập nhật mã thùng mới
                            cartonDataL.cartonCode = s.Trim();
                            cartonDataL.Start_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700");
                            //thêm vào hàng chờ cập nhật
                            Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonDataL);
                        }
                    }
                }
            }
        }

        private void HandScan01_Process(string s)
        {
            PCLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.DataChange, "Scan 01" + s.Trim(), s.Trim());

            this.InvokeIfRequired(() =>
            {
                opLane01.Items.Insert(0, s.Trim());
            });
            //kiểm tra xem thùng đang chạy chẵn hay lẻ
            if (Globals.ProductionData.counter.cartonID % 2 == 0)
            {
                //thùng chẵn
                //kiểm tra xem thùng tiếp theo có mã chưa
                if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID + 1, out ProductionCartonData cartonData))
                {
                    //nếu thùng tiếp theo đã có mã thì cập nhật mã thùng mới
                    if (cartonData.cartonCode != "0")
                    {
                        //Đã có mã thùng thì không làm gì cả
                    }
                    else
                    {
                        //kiểm tra mã đã từng tồn tại chưa
                        if (!IsTestModeEnabled && Globals_Database.Dictionary_ProductionCarton_Data.Values.Any(x => x.cartonCode == s.Trim()))
                        {
                            this.InvokeIfRequired(() =>
                            {
                                this.ShowErrorNotifier("Mã thùng đã tồn tại. Vui lòng kiểm tra lại!", false, 5000);
                            });
                            return;
                        }
                        else
                        {
                            //nếu chưa tồn tại thì cập nhật mã thùng mới
                            cartonData.cartonCode = s.Trim();
                            cartonData.Start_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700");
                            //thêm vào hàng chờ cập nhật
                            Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonData);
                            //Globals_Database.Activate_Carton.Enqueue(s.Trim());
                        }
                    }
                }
            }
            else
            {
                //thùng lẻ tức là thùng đang chạy
                //kiểm tra xem thùng lẻ có mã chưa
                if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID, out ProductionCartonData cartonDataL))
                {
                    //nếu thùng đã có mã thì không làm gì cả
                    if (cartonDataL.cartonCode != "0")
                    {
                        //Đã có mã thùng thì không làm gì cả
                    }
                    else
                    {
                        //kiểm tra mã đã từng tồn tại chưa
                        if (!IsTestModeEnabled && Globals_Database.Dictionary_ProductionCarton_Data.Values.Any(x => x.cartonCode == s.Trim()))
                        {
                            this.InvokeIfRequired(() =>
                            {
                                this.ShowErrorNotifier("Mã thùng đã tồn tại. Vui lòng kiểm tra lại!", false, 5000);
                            });
                            return;
                        }
                        else
                        {
                            //nếu chưa tồn tại thì cập nhật mã thùng mới
                            cartonDataL.cartonCode = s.Trim();
                            cartonDataL.Start_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700");
                            //thêm vào hàng chờ cập nhật
                            Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonDataL);

                            ////kết thúc thùng chẵn cũ
                            //cartonData.cartonCode = s.Trim();
                            //cartonData.Activate_Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700");
                            //Globals_Database.Update_Product_To_Record_Carton_Queue.Enqueue(cartonData);
                            //Globals_Database.Activate_Carton.Enqueue(s.Trim());
                        }
                    }
                }
            }
        }

        private void btnSend2_Click(object sender, EventArgs e)
        {
            string test2 = ipTest2.Text.Split('-')[2];
            int test2Int = Convert.ToInt32(test2);
            test2Int++;
            ipTest2.Text = ipTest2.Text.Split('-')[0] + "-" + ipTest2.Text.Split('-')[1] + "-" + test2Int.ToString();
            HandScan02_Process(ipTest2.Text);
        }

        private void tcpClient1_ClientCallBack(SpT.Communications.TCP.enumClient state, string data)
        {
            switch (state)
            {
                case SpT.Communications.TCP.enumClient.CONNECTED:
                    this.InvokeIfRequired(() =>
                    {
                        opLane01.Items.Insert(0, "TCP Connected");
                    });
                    Globals.e_Hand_Scan_Carton_State = e_Camera_State.CONNECTED;
                    break;
                case SpT.Communications.TCP.enumClient.DISCONNECTED:
                    this.InvokeIfRequired(() =>
                    {
                        opLane01.Items.Insert(0, "TCP Disconnected" );
                    });
                    Globals.e_Hand_Scan_Carton_State = e_Camera_State.DISCONNECTED;
                    break;
                case SpT.Communications.TCP.enumClient.RECEIVED:
                    //phân tách ký tự <p> để lấy thùng bên nào ví dụ SC1<p>123456789 là thùng bên 1

                    string lane = data.Split(new string[] { "<p>" }, StringSplitOptions.None)[0];
                    string codeContent = data.Split(new string[] { "<p>" }, StringSplitOptions.None)[1];

                    if (lane != "SC1")
                    {
                        HandScan01_Process(codeContent);
                    }
                    else
                    {
                        HandScan02_Process(codeContent);
                    }

                    break;
                case SpT.Communications.TCP.enumClient.RECONNECT:
                    this.InvokeIfRequired(() =>
                    {
                        opLane01.Items.Insert(0, "TCP Reconnect");
                    });
                    Globals.e_Hand_Scan_Carton_State = e_Camera_State.RECONNECTING;
                    break;
            }
        }

        private void btnNextCarton_Click(object sender, EventArgs e)
        {

            if(ipTest1.Text != "secrettantien512")
            {
                this.ShowErrorDialog("Vui lòng không nhấn thử");
                return;
            }
            //kiểm tra thùng hiện tại đã có mã chưa
            if (Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID, out ProductionCartonData cartonData))
            {
                if (cartonData.cartonCode == "0")
                {
                    this.ShowErrorDialog("Thùng hiện tại chưa có mã thùng. Vui lòng quét mã thùng trước khi chuyển sang thùng mới.");
                    return;
                }
            }


            Globals.ProductionData.counter.cartonID += 1;
            Globals.ProductionData.counter.carton_Packing_Count = 0;

            //thêm thùng mới vào csdl
            ProductionCartonData cartonData1 = new ProductionCartonData();
            // chọn ID mới theo max ID hiện có để tránh lệch khi đã tạo/xóa trước đó
            cartonData1.cartonID = Globals_Database.Dictionary_ProductionCarton_Data.Keys.DefaultIfEmpty(0).Max() + 1;
            cartonData1.cartonCode = "0";
            cartonData1.Activate_Datetime = "0";
            cartonData1.Activate_User = "0";
            cartonData1.Production_Datetime = "0";
            cartonData1.orderNo = Globals.ProductionData.orderNo;
            cartonData1.Start_Datetime = "0";
            Globals_Database.Dictionary_ProductionCarton_Data.Add(cartonData1.cartonID, cartonData1);
            //thêm cái thùng mới vào cuối để đủ thùng
            Globals.ProductionData.setDB.Insert_Carton(cartonData1, cartonData1.orderNo);


        }

        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            // Kiểm tra và kích hoạt thùng liên tục trước đó nếu chưa được kích hoạt
            CheckAndActivatePreviousCarton();
        }

        /// <summary>
        /// Kiểm tra thùng liên tục trước đó, nếu chưa kích hoạt thì tự động kích hoạt
        /// </summary>
        private void CheckAndActivatePreviousCarton()
        {
            try
            {
                Globals.Log.WriteLogAsync("System", e_LogType.Info, "Bắt đầu kiểm tra thùng liên tục chưa kích hoạt");

                // Chờ một chút để đảm bảo dữ liệu đã được load
                Thread.Sleep(2000);

                // Kiểm tra xem có đơn hàng nào đang hoạt động không
                if (string.IsNullOrEmpty(Globals.ProductionData.orderNo))
                {
                    Globals.Log.WriteLogAsync("System", e_LogType.Info, "Không có đơn hàng đang hoạt động, bỏ qua kiểm tra thùng");
                    return;
                }

                if(Globals_Database.Dictionary_ProductionCarton_Data.TryGetValue(Globals.ProductionData.counter.cartonID - 1 , out ProductionCartonData cartonData))
                {
                    this.ShowInfoDialog($"Kiểm tra thùng ID={cartonData.cartonID}, Code={cartonData.cartonCode}, Start={cartonData.Start_Datetime}, Production={cartonData.Production_Datetime}");

                    if (cartonData.cartonCode != "0" &&
                        cartonData.Start_Datetime != "0" &&
                        cartonData.Production_Datetime == "0")
                    {
                        // Thêm vào hàng đợi kích hoạt
                        Globals_Database.Activate_Carton.Enqueue(cartonData.cartonCode);
                        Globals.Log.WriteLogAsync("System", e_LogType.Info,
                            $"Tự động kích hoạt thùng ID={cartonData.cartonID}, Code={cartonData.cartonCode}");
                        this.ShowSuccessDialog($"Đã tự động kích hoạt thùng ID={cartonData.cartonID}, Code={cartonData.cartonCode}");
                    }
                }
               
            }
            catch (Exception ex)
            {
                Globals.Log.WriteLogAsync("System", e_LogType.Error, "Lỗi khi kiểm tra và kích hoạt thùng tự động: " + ex.Message);
            }
        }
    }
}
