using Newtonsoft.Json;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace TestApp
{
    public partial class Form1 : UIForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            MES_Load_OrderNo_ToComboBox(uiComboBox1);
        }
        string _connectionString_PO_MES = $@"Data Source=C:/TempStorage-win32-x64/po.db;Version=3;";
        public void MES_Load_OrderNo_ToComboBox(UIComboBox comboBox)
        {
            try
            {
                using (var conn = new SQLiteConnection(_connectionString_PO_MES))
                {
                    string query = "SELECT DISTINCT orderNo FROM POInfo ORDER BY orderNo";
                    var adapter = new SQLiteDataAdapter(query, conn);
                    var table = new DataTable();

                    adapter.Fill(table);
                    // Thêm một dòng rỗng vào đầu danh sách
                    DataRow emptyRow = table.NewRow();
                    emptyRow["orderNo"] = "Chọn orderNO"; // Hoặc để trống
                    table.Rows.InsertAt(emptyRow, 0);
                    // Thiết lập DataSource cho ComboBox
                    comboBox.DataSource = table;
                    comboBox.DisplayMember = "orderNo";
                    comboBox.ValueMember = "orderNo";
                }
            }
            catch (Exception ex)
            {
                this.ShowErrorNotifier($"Lỗi khi tải dữ liệu: {ex.Message}");
            }
        }



        private void uiSymbolButton3_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable a = Get_PO_Info_By_OrderNo(uiComboBox1.SelectedValue.ToString());
                if (a.Rows.Count > 0)
                {

                    uiDataGridView1.DataSource = a;
                }
                else
                {
                    this.ShowErrorNotifier("Không có dữ liệu cho orderNo này.");
                }
            }
            catch (Exception ex)
            {
                this.ShowErrorNotifier($"Lỗi khi lấy dữ liệu: {ex.Message}");
            }

        }


        public DataTable Get_PO_Info_By_OrderNo(string orderNo)
        {
            using (var conn = new SQLiteConnection(_connectionString_PO_MES))
            {
                string query = "SELECT * FROM POInfo WHERE orderNo = @orderNo";
                var adapter = new SQLiteDataAdapter(query, conn);

                adapter.SelectCommand.Parameters.AddWithValue("@orderNo", orderNo);

                var table = new DataTable();
                adapter.Fill(table);

                //thêm cột số mã CZ

                table.Columns.Add("UniqueCodeCount", typeof(int));
                //lấy số mã CZ trong thư mục _codesPath/<orderNo>.db
                int uniqueCodeCount = Get_Unique_Code_MES_Count(orderNo);
                //cập nhật số mã CZ vào cột UniqueCodeCount
                foreach (DataRow row in table.Rows)
                {
                    row["UniqueCodeCount"] = uniqueCodeCount;
                }

                return table;
            }
        }

        public int Get_Unique_Code_MES_Count(string orderNo)
        {
            try
            {
                string czpath = "C:/TempStorage-win32-x64/codes" + "/" + orderNo + ".db";
                using (var conn = new SQLiteConnection($"Data Source={czpath};Version=3;"))
                {
                    string query = "SELECT COUNT(*) FROM UniqueCodes";
                    var command = new SQLiteCommand(query, conn);
                    command.Parameters.AddWithValue("@orderNo", orderNo);
                    conn.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count;
                }
            }
            catch
            {
                return 0;
            }
        }

        private void uiSymbolButton2_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable poList = MES_Get_PO_List();
                if (poList.Rows.Count > 0)
                {
                    uiDataGridView1.DataSource = poList;
                }
                else
                {
                    this.ShowErrorNotifier("Không có dữ liệu trong danh sách PO.");
                }
            }
            catch (Exception ex)
            {
                this.ShowErrorNotifier($"Lỗi khi lấy danh sách PO: {ex.Message}");
            }
        }

        public DataTable MES_Get_PO_List()
        {
            using (var conn = new SQLiteConnection(_connectionString_PO_MES))
            {
                string query = "SELECT * FROM POInfo ORDER BY orderNo";
                var adapter = new SQLiteDataAdapter(query, conn);
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        public DataTable MES_Get_PO_Logs()
        {
            using (var conn = new SQLiteConnection(_connectionString_PO_MES))
            {
                string query = "SELECT * FROM POLogs ORDER BY orderNo";
                var adapter = new SQLiteDataAdapter(query, conn);
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        private void uiSymbolButton5_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable poLogs = MES_Get_PO_Logs();
                if (poLogs.Rows.Count > 0)
                {
                    uiDataGridView1.DataSource = poLogs;
                }
                else
                {
                    this.ShowErrorNotifier("Không có dữ liệu trong danh sách PO Logs.");
                }
            }
            catch (Exception ex)
            {
                this.ShowErrorNotifier($"Lỗi khi lấy danh sách PO Logs: {ex.Message}");
            }
        }
        private AwsIotClientHelper awsClient; // để global giữ kết nối
        private void uiSymbolButton6_Click(object sender, EventArgs e)
        {
            string host = Setting.Current.Host;
            string clientId = Setting.Current.ClientId;

            string rootCAPath = Setting.Current.RootCAPath;
            string pfxPath = Setting.Current.PfxPath;
            string pfxPassword = Setting.Current.PfxPassword;

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

            awsClient.ConnectAsync();
        }

        private void AWS_Status_OnReceive(object sender, AwsIotClientHelper.AWSStatusReceiveEventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                uiListBox1.Items.Add($"📩 [{DateTime.Now:HH:mm:ss}] Nhận từ topic {e.Topic}: {e.Payload}");
                uiListBox1.SelectedIndex = uiListBox1.Items.Count - 1; // Tự động cuộn xuống cuối

            }));
        }

        public void SafeInvoke(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(action);
            }
            else
            {
                action();
            }
        }

        private void AWS_Status_Onchange(object sender, AwsIotClientHelper.AWSStatusEventArgs e)
        {
            switch (e.Status)
            {
                case AwsIotClientHelper.e_awsIot_status.Connected:

                    SafeInvoke(() =>
                    {
                        uiListBox1.Items.Add("✅ Kết nối thành công với AWS IoT Core.");
                        uiListBox1.SelectedIndex = uiListBox1.Items.Count - 1; // Tự động cuộn xuống cuối
                    });

                    break;
                case AwsIotClientHelper.e_awsIot_status.Disconnected:

                    SafeInvoke(() =>
                    {
                        uiListBox1.Items.Add("❌ Mất kết nối với AWS IoT Core.");
                        uiListBox1.SelectedIndex = uiListBox1.Items.Count - 1; // Tự động cuộn xuống cuối
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Connecting:

                    SafeInvoke(() =>
                    {
                        uiListBox1.Items.Add("🔄 Đang kết nối đến AWS IoT Core...");
                        uiListBox1.SelectedIndex = uiListBox1.Items.Count - 1; // Tự động cuộn xuống cuối
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Error:

                    SafeInvoke(() =>
                    {
                        uiListBox1.Items.Add($"⚠️ Lỗi: {e.Message}");
                        uiListBox1.SelectedIndex = uiListBox1.Items.Count - 1; // Tự động cuộn xuống cuối
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Subscribed:

                    SafeInvoke(() =>
                    {
                        uiListBox1.Items.Add($"✅ Đã đăng ký các topic: {e.Message}");
                        uiListBox1.SelectedIndex = uiListBox1.Items.Count - 1; // Tự động cuộn xuống cuối
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Unsubscribed:
                    
                    SafeInvoke(() =>
                    {
                        uiListBox1.Items.Add($"❌ Không thể đăng ký các topic: {e.Message}");
                        uiListBox1.SelectedIndex = uiListBox1.Items.Count - 1; // Tự động cuộn xuống cuối
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Published:

                    SafeInvoke(() =>
                    {
                        uiListBox1.Items.Add($"✅ Đã publish thành công: {e.Message}");
                        uiListBox1.SelectedIndex = uiListBox1.Items.Count - 1; // Tự động cuộn xuống cuối
                    });
                    break;
                case AwsIotClientHelper.e_awsIot_status.Unpublished:

                    SafeInvoke(() =>
                    {
                        uiListBox1.Items.Add($"⚠️ Không thể publish: {e.Message}");
                        uiListBox1.SelectedIndex = uiListBox1.Items.Count - 1; // Tự động cuộn xuống cuối
                    });
                    break;
            }
        }

        [ConfigFile("MSC\\Setting.ini")]
        public class Setting : IniConfig<Setting>
        {
            public string Host { get; set; }
            public string ClientId { get; set; }
            public string RootCAPath { get; set; }
            public string PfxPath { get; set; }
            public string PfxPassword { get; set; }

            public override void SetDefault()
            {
                base.SetDefault();
                Host = "a22qv9bgjnbsae-ats.iot.ap-southeast-1.amazonaws.com";
                ClientId = "MIPWP501";
                RootCAPath = @"C:\MIPWP501\AmazonRootCA1.pem";
                PfxPath = @"C:\MIPWP501\client-certificate.pfx";
                PfxPassword = "thuc";
            }
        }

        private void uiSymbolButton7_Click(object sender, EventArgs e)
        {
            string[] topicsToSub = new[]
                            {
                                "CZ/MIPWP501/response"
                            };

            awsClient.SubscribeMultiple(topicsToSub);
        }

        private void uiSymbolButton4_Click(object sender, EventArgs e)
        {
            //get unique codes from MES database
            try
            { 
                DataTable dataTable = Get_Unique_Codes_MES(uiComboBox1.SelectedValue.ToString());
                if (dataTable.Rows.Count > 0)
                {
                    uiDataGridView1.DataSource = dataTable;
                }
                else
                {
                    this.ShowErrorNotifier("Không có dữ liệu mã CZ cho orderNo này.");
                }
            }
            catch (Exception ex)
            {
                this.ShowErrorNotifier($"Lỗi khi lấy dữ liệu mã CZ: {ex.Message}");
            }

        }

        public DataTable Get_Unique_Codes_MES(string orderNo)
        {
            string czpath = "C:/TempStorage-win32-x64/codes" + "/" + orderNo + ".db";
            using (var conn = new SQLiteConnection($"Data Source={czpath};Version=3;"))
            {
                string query = "SELECT * FROM UniqueCodes";
                var adapter = new SQLiteDataAdapter(query, conn);
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        private void uiSymbolButton9_Click(object sender, EventArgs e)
        {
            try
            {
                //lấy danh sách mã unique code từ MES
                DataTable uniqueCodes = Get_Unique_Codes_MES(uiComboBox1.SelectedValue.ToString());

                //uiNumberBox1.Text là vị trí bắt đầu, numberBox2.Text là số lượng mã cần gửi
                int startIndex = Convert.ToInt32(uiNumPadTextBox1.Text);
                int count = Convert.ToInt32(uiNumPadTextBox2.Text);

                if (startIndex < 0 || startIndex >= uniqueCodes.Rows.Count)
                {
                    this.ShowErrorNotifier("Vị trí bắt đầu không hợp lệ.");
                    return;
                }
                if (count <= 0 || startIndex + count > uniqueCodes.Rows.Count)
                {
                    this.ShowErrorNotifier("Số lượng mã cần gửi không hợp lệ.");
                    return;
                }
                //lấy các mã unique code từ vị trí bắt đầu và số lượng cần gửi
                DataTable codesToSend = uniqueCodes.AsEnumerable()
                    .Skip(startIndex)
                    .Take(count)
                    .CopyToDataTable();
                //kiểm tra nếu không có mã nào để gửi
                if (codesToSend.Rows.Count == 0)
                {
                    this.ShowErrorNotifier("Không có mã nào để gửi.");
                    return;
                }
                //gửi từng mã unique code đến AWS IoT Core
                foreach (DataRow row in codesToSend.Rows)
                {
                    string uniqueCodea = row["Code"].ToString();
                   // string topic = "CZ/data";
                    //string payload = $"{{\"uniqueCode\": \"{uniqueCode}\"}}"; // Giả sử payload là JSON
                    var payload = new
                    {
                        message_id = $"{DateTime.Now.ToString("ss.fff")}-{uiComboBox1.SelectedValue.ToString()}",
                        orderNo = uiComboBox1.SelectedValue.ToString(),
                        uniqueCode = uniqueCodea,
                        status = 1,
                        activate_datetime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK"),
                        production_date = "2025-07-15 00:00:00.000",
                        thing_name = "MIPWP501"
                    };

                    string json = JsonConvert.SerializeObject(payload);

                    awsClient.Publish("CZ/data", json);
                    //uiListBox1.Items.Add($"📤 Đã gửi mã: {uniqueCode} đến topic: {topic}");

                    //thời gian chờ giữa các lần gửi để tránh quá tải là uiNumPadTextBox3.Text (mặc định là 1000ms)
                    int delay = Convert.ToInt32(uiNumPadTextBox3.Text);
                    if (delay > 0)
                    {
                        Thread.Sleep(delay); // Dừng luồng hiện tại trong khoảng thời gian delay
                    }

                }

            }
            catch (Exception ex)
            {
                this.ShowErrorNotifier($"Lỗi khi thực hiện hành động: {ex.Message}");
            }
        }

        private void uiListBox1_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show(uiListBox1.SelectedItem?.ToString() ?? "Không có mục nào được chọn.", "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
