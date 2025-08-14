using HslCommunication.Profinet.Siemens.S7PlusHelper;
using MASAN_SERIALIZATION.Configs;
using MASAN_SERIALIZATION.Enums;
using MASAN_SERIALIZATION.Production;
using MASAN_SERIALIZATION.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpT.Logs;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing.QrCode.Internal;
using static AwsIotClientHelper;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MASAN_SERIALIZATION.Views.AWS
{

    public partial class PAws : UIPage
    {
        public AwsIotClientHelper awsClient;
        //khai báo log 
        private LogHelper<e_LogType> AWSLog;

        public PAws()
        {
            InitializeComponent();
        }

        public void INIT()
        {
            //khởi tạo log
            AWSLog = new LogHelper<e_LogType>(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MASAN-SERIALIZATION", "Logs", "Pages", "AWSlog.ptl"));
            Connect_AWS();
        }

        private void Connect_AWS()
        {
            if (AppConfigs.Current.AWS_ENA)
            {
                //kết nối MQTT
                string host = AppConfigs.Current.host;
                string clientId = AppConfigs.Current.clientId;
                string rootCAPath = AppConfigs.Current.rootCAPath;
                string pfxPath = AppConfigs.Current.pfxPath;
                string pfxPassword = AppConfigs.Current.pfxPassword;

                awsClient = new AwsIotClientHelper(
                    host,
                    clientId,
                    rootCAPath,
                    "",
                    pfxPath,
                    pfxPassword

                );
                awsClient.AWSStatus_OnChange += AWS_Status_Onchange;
                awsClient.AWSStatus_OnReceive += AWS_Status_OnReceive;
                Task.Run(() =>
                {
                    try
                    {
                        awsClient.ConnectAsync().Wait();
                    }
                    catch (Exception ex)
                    {
                        AWSLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"🔴 [{DateTime.Now}] Lỗi kết nối AWS: {ex.Message}");
                        this.InvokeIfRequired(() =>
                        {

                            opConsole.Items.Insert(0, $"🔴 [{DateTime.Now}] Lỗi kết nối AWS: {ex.Message}");

                        });

                    }
                });
            }

        }

        private void AWS_Status_OnReceive(object sender, AWSStatusReceiveEventArgs e)
        {
            if (e.Topic == "CZ/MIPWP501/response")
            {
                JObject jsonObject = JObject.Parse(e.Payload);
                // Lấy giá trị của trường "status" từ JSON
                string status = jsonObject["status"]?.ToString();
                //lấy giá trị của trường "message_id" từ JSON
                string messageId = jsonObject["message_id"]?.ToString();
                // Lấy giá trị của trường "error_message" từ JSON
                string errorMessage = jsonObject["error_message"]?.ToString();

                this.InvokeIfRequired(() =>
                {
                    opReciveConsole.Items.Insert(0, $"{DateTime.Now:HH:mm:ss}: {status}: {e.Topic} - {e.Payload}");
                });
            }
            else
            {
                this.InvokeIfRequired(() =>
                {
                    uiListBox1.Items.Insert(0, $"{DateTime.Now:HH:mm:ss}:{e.Topic} - {e.Payload}");
                });
            }
            


        }

        private void AWS_Status_Onchange(object sender, AWSStatusEventArgs e)
        {
            switch (e.Status)
            {
                case AwsIotClientHelper.e_awsIot_status.Connected:
                    // ghi log
                    //ghi log
                    AWSLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Info, "Đã Kết nối AWS");
                    this.InvokeIfRequired(() =>
                    {
                        //cập nhật trạng thái kết nối
                        opConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Kết nối thành công với AWS IoT Core.");
                    });

                    string[] topicsToSub = new[]
                                      {
                                            "CZ/MIPWP501/response",
                                            "CZ/data"
                                        };

                    awsClient.SubscribeMultiple(topicsToSub);

                    break;
                case AwsIotClientHelper.e_awsIot_status.Disconnected:
                    //ghi log
                    AWSLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Warning, "Đã Ngắt kết nối AWS");
                    this.InvokeIfRequired(() =>
                    {
                        opConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Đã ngắt kết nối AWS IoT Core. Đang thử kết nối lại...");
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Connecting:
                    //ghi log
                    AWSLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Info, "Đang kết nốilại AWS");
                    this.InvokeIfRequired(() =>
                    {
                        //cập nhật trạng thái kết nối
                        opConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Đang kết nối lại với AWS IoT Core...");
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Error:
                    //ghi log
                    AWSLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Error, $"Lỗi kết nối AWS: {e.Message}");
                    this.InvokeIfRequired(() =>
                    {
                        //cập nhật trạng thái kết nối
                        opConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Lỗi kết nối AWS IoT Core: {e.Message}");
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Subscribed:
                    //ghi log
                    AWSLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Info, "Đã đăng ký các topic thành công");
                    this.InvokeIfRequired(() =>
                    {
                        //cập nhật trạng thái kết nối
                        opConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Đã đăng ký các topic thành công.");
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Unsubscribed:
                    //ghi log
                    AWSLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Warning, "Đã hủy đăng ký các topic");
                    this.InvokeIfRequired(() =>
                    {
                        //cập nhật trạng thái kết nối
                        opConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Đã hủy đăng ký các topic.");
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Published:

                    break;
                case AwsIotClientHelper.e_awsIot_status.Unpublished:
                    //ghi log
                    AWSLog.WriteLogAsync(Globals.CurrentUser.Username, e_LogType.Warning, "Không thể publish dữ liệu");
                    this.InvokeIfRequired(() =>
                    {
                        //cập nhật trạng thái kết nối
                        opConsole.Items.Add($"{DateTime.Now:HH:mm:ss}: Không thể publish dữ liệu, chưa kết nối.");
                    });
                    break;
            }
        }

        private void opReciveConsole_DoubleClick(object sender, EventArgs e)
        {
            this.ShowInfoDialog(opReciveConsole.SelectedItem?.ToString() ?? "Không có dữ liệu nào được chọn để hiển thị.");
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            INIT();
        }

        DataTable SendCodes = new DataTable();
        private void btnGetData_Click(object sender, EventArgs e)
        {
            TResult getCodeSend = Globals.ProductionData.getDataPO.Get_Codes_Send(Globals.ProductionData.orderNo);

            this.InvokeIfRequired(() =>
            {
                opConsole.Items.Insert(0, getCodeSend.message);
            });

            SendCodes = getCodeSend.data;
        }

        private void btnSendOne_Click(object sender, EventArgs e)
        {
            string orderNO = Globals.ProductionData.orderNo;
            string ID = SendCodes.Rows[0]["ID"].ToString();
            string code = SendCodes.Rows[0]["Code"].ToString();
            string Status = SendCodes.Rows[0]["Status"].ToString();
            string activateDate = SendCodes.Rows[0]["ActivateDate"].ToString();
            string productionDate = SendCodes.Rows[0]["ProductionDate"].ToString();
            string cartonCode = SendCodes.Rows[0]["cartonCode"].ToString();
            AWSSendPayload payload = new AWSSendPayload
            {
                message_id = $"{ID}-{orderNO}",
                orderNo = orderNO,
                uniqueCode = code,
                cartonCode = cartonCode,
                status = Status.ToInt32(),
                activate_datetime = activateDate,
                production_date = productionDate,
                thing_name = "MIPWP501"
            };
            string json = JsonConvert.SerializeObject(payload);
            //show bảng thông tin hỏi trước khi gửi
            var confirmResult = this.ShowYesNoDialog($"Bạn có chắc chắn muốn gửi dữ liệu này không?\n\n" +
                $"payload: {json}");
            if (confirmResult != DialogResult.Yes)
            {
                this.InvokeIfRequired(() =>
                {
                    opConsole.Items.Insert(0, $"❌ [{DateTime.Now}] Đã hủy gửi dữ liệu: {json}");
                });
                return; // Nếu người dùng chọn "No", thoát khỏi phương thức
            }
            //publish dữ liệu
            var rs = awsClient.Publish_V2("CZ/data", json);
            if (rs.Issuccess)
            {
                //cập nhật trạng thái đã gửi
                this.InvokeIfRequired(() =>
                {
                    opConsole.Items.Insert(0, $"✅ [{DateTime.Now}] Đã gửi dữ liệu thành công: {json}");
                });
            }
            else
            {
                //ghi log lỗi không gửi được
                this.InvokeIfRequired(() =>
                {
                    opConsole.Items.Insert(0, $"❌ [{DateTime.Now}] Lỗi gửi dữ liệu: {rs.msg}");
                });
            }
        }
        // Add the following method to the PAws class to resolve the CS1061 error.
        // This method provides the missing definition for 'ShowYesNoDialog'.

        private DialogResult ShowYesNoDialog(string message)
        {
            return MessageBox.Show(message, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        private void btnLoadPOInfo_Click(object sender, EventArgs e)
        {
            if (Globals.Production_State == e_Production_State.NoSelectedPO)
            {
                this.ShowWarningDialog("Vui lòng chọn đơn hàng trước khi tải thông tin.");
                return;
            }

            iporderNo.Text = Globals.ProductionData.orderNo;
            ipProductionDate.Text = Globals.ProductionData.productionDate;
        }

        int lines = 0;
        int lastLine = 0; // Biến để theo dõi dòng cuối cùng đã đọc
        private void btnSendTest_Click(object sender, EventArgs e)
        {
            string filePath = ipfilePath.Text;
            List<string> values = new List<string>();

            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {

                    string line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Tách bằng dấu phẩy, lấy cột đầu tiên
                    string[] parts = line.Split('\n');
                    if (parts.Length > 0)
                    {
                        values.Add(parts[0].Trim());
                        this.InvokeIfRequired(() =>
                        {
                            opConsole.Items.Insert(0, $"✅ [{DateTime.Now}] Đang đọc xx dòng {parts[0].Trim()} từ file CSV: {filePath}");
                        });
                    }
                }

            }

            // Sử dụng trong vòng lặp
            Task.Run(() =>
            {
                try
                {
                    this.InvokeIfRequired(() =>
                    {
                        opConsole.Items.Insert(0, $"✅ [{DateTime.Now}] Đã đọc {values.Count} mã từ file CSV: {filePath}");
                    });
                    int index = 0;
                    foreach (var val in values)
                    {
                        index++;
                        AWSSendPayload payload = new AWSSendPayload
                        {
                            message_id = $"{index}-{iporderNo.Text}-{DateTime.UtcNow.ToString("O")}",
                            orderNo = iporderNo.Text,
                            uniqueCode = val,
                            cartonCode = ipcartonCode.Text,
                            status = 1,
                            activate_datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"),
                            production_date = ipProductionDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff +0700"),
                            thing_name = "MIPWP501"
                        };

                        string json = JsonConvert.SerializeObject(payload);

                        //gưi dữ liệu
                        var rs = awsClient.Publish_V2("CZ/data", json);

                        //lưu pay load vô csv

                        this.InvokeIfRequired(() =>
                        {
                            uiTextBox1.Text = json;
                            opConsole.Items.Insert(0, $"{json}");
                        });
                    }
                    Task.Delay(100).Wait(); // Giả lập thời gian gửi dữ liệu
                    
                }
                catch (Exception ex)
                {
                    this.InvokeIfRequired(() =>
                    {
                        opConsole.Items.Insert(0, $"❌ [{DateTime.Now}] Lỗi đọc file CSV: {ex.Message}");
                    });
                }
            });
        }

        private void opConsole_DoubleClick(object sender, EventArgs e)
        {
            this.ShowInfoDialog(opConsole.SelectedItem?.ToString() ?? "Không có dữ liệu nào được chọn để hiển thị.");
        }

        private void btnFilePath_Click(object sender, EventArgs e)
        {
            //mở file csv
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Lấy đường dẫn file đã chọn
                    string filePath = openFileDialog.FileName;
                    ipfilePath.Text = filePath;
                }
            }
        }


        public string ReadCsvGetLineCol(string filePath, int lines)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    int currentLine = 0;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        currentLine++;

                        if (currentLine == lines) // dòng thứ 2
                        {
                            var parts = line.Split(',');

                            if (parts.Length >= 3)
                            {
                                return parts[2].Trim(); // cột thứ 3
                            }
                            else
                            {
                                throw new Exception("❌ File không đủ cột.");
                            }
                        }
                    }

                    throw new Exception("❌ File không đủ dòng.");
                }
            }
            catch (Exception ex)
            {
                return $"❌ Lỗi: {ex.Message}";
            }
        }

        private void btnSendAll_Click(object sender, EventArgs e)
        {

        }

        private void opReciveConsole_DoubleClick_1(object sender, EventArgs e)
        {
            this.ShowInfoDialog(opReciveConsole.SelectedItem.ToString());
        }
    }
}
