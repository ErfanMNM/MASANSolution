using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASAN_SERIALIZATION.Production
{
    public class ProductionOrder
    {
        //vị trí thư mục lưu trữ thông tin po lấy từ MES
        private static string poAPIServerPath;
        private static string poAPIServerFileName;

        //khởi tạo thông tin đường dẫn đến cơ sở dữ liệu PO
        private static string poDatabase ;

        public ProductionOrder()
        {
            //tao thư mục lưu trữ thông tin po lấy từ MES nằm trong thư mục cài đặt của ứng dụng ở C:\MasanSerialization\Server_Service\
            poAPIServerPath = @"C:\MasanSerialization\Server_Service\";
            poAPIServerFileName = "po.db";
        }

        #region Các thuộc tính thông tin PO
        public string orderNo { get; set; } = "-";
        public string site { get; set; } = "-";
        public string factory { get; set; } = "-";
        public string productionLine { get; set; } = "-";
        public string productionDate { get; set; } = "-";
        public string shift { get; set; } = "-";
        public string orderQty { get; set; } = "-";
        public string lotNumber { get; set; } = "-";
        public string productCode { get; set; } = "-";
        public string productName { get; set; } = "-";
        public string GTIN { get; set; } = "-";
        public string customerOrderNo { get; set; } = "-";
        public string uom { get; set; } = "-";
        public string cartonSize { get; set; } = "-";
        public string totalCZCode { get; set; } = "-"; //Tổng số mã CZ đã nhận từ MES
        public Product_Counter counter { get; set; } = new Product_Counter();
        #endregion

        #region Các thống kê
        public class Product_Counter
        {
            public int passCount { get; set; } = 0;//Pass
            public int failCount { get; set; } = 0;//Lỗi
            public int duplicateCount { get; set; } = 0;//Trùng lặp
            public int readfailCount { get; set; } = 0;//Không đọc được
            public int notfoundCount { get; set; } = 0;//Không tồn tại
            public int totalCount { get; set; } = 0;//Tổng số mã đã quét
            public int totalCartonCount { get; set; } = 0;//Tổng số thùng đã quét
            public int activatedCartonCount { get; set; } = 0; //Tổng số thùng đã kích hoạt
            public int errorCartonCount { get; set; } = 0; //Tổng số thùng lỗi (lỗi kích hoạt, lỗi gửi AWS, v.v.)
            public int errorCount { get; set; } = 0;//Tổng số lỗi khác (gửi PLC không được, timeout, v.v.)
        }

        //thông tin bộ đếm gửi nhận aws
        public class AWS_Send_Counter
        {
            //số chưa gửi (chưa kích hoạt)
            public int pendingCount { get; set; } = 0; //Số mã chưa gửi
            //số đã gửi
            public int sentCount { get; set; } = 0; //Số mã đã gửi thành công
            //số gửi lỗi
            public int failedCount { get; set; } = 0; //Số mã gửi lỗi
        }

        public class AWS_Recived_Counter
        {
            //số chưa gửi (chưa kích hoạt)
            public int waitingCount { get; set; } = 0; //số mã đang chờ nhận
            //số đã gửi
            public int recivedCount { get; set; } = 0; //đã nhận thành công
        }

        #endregion

        #region Các phương thức lấy dữ liệu từ MES
        public GetfromMES getfromMES { get; } = new GetfromMES();

        public class GetfromMES
        {
            public (bool issucess, string mesage, DataTable PO) ProductionOrder_List()
            {
                string dbPath = $@"{poAPIServerPath}{poAPIServerFileName}";
                try
                {
                    if (!System.IO.File.Exists(dbPath))
                    {
                        return (false, "Cơ sở dữ liệu PO không tồn tại.", null);
                    }

                    using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT * FROM po_records ORDER BY orderNo";
                        var adapter = new SQLiteDataAdapter(query, conn);
                        var table = new DataTable();
                        adapter.Fill(table);
                        return (table.Rows.Count > 0) ? (true, "Lấy dữ liệu thành công.", table) : (false, "Không có dữ liệu PO nào.", null);
                    }
                }
                catch (Exception ex)
                {
                    return (false, $"Lỗi P01 khi kiểm tra cơ sở dữ liệu PO: {ex.Message}", null);
                }

            }

            //lấy thông tin chi tiết của một PO theo orderNo
            public (bool issucess, string mesage, DataTable PO) ProductionOrder_Detail(string orderNo)
            {
                string dbPath = $@"{poAPIServerPath}{poAPIServerFileName}";
                try
                {
                    if (!System.IO.File.Exists(dbPath))
                    {
                        return (false, "Cơ sở dữ liệu PO không tồn tại.", null);
                    }
                    using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                    {
                        conn.Open();
                        string query = "SELECT * FROM POInfo WHERE orderNo = @orderNo";
                        var cmd = new SQLiteCommand(query, conn);
                        cmd.Parameters.AddWithValue("@orderNo", orderNo);
                        var adapter = new SQLiteDataAdapter(cmd);
                        var table = new DataTable();
                        adapter.Fill(table);
                        return (table.Rows.Count > 0) ? (true, "Lấy dữ liệu thành công.", table) : (false, "Không tìm thấy PO với orderNo: " + orderNo, null);
                    }
                }
                catch (Exception ex)
                {
                    return (false, $"Lỗi P02 khi lấy thông tin chi tiết PO: {ex.Message}", null);
                }
            }

            //lấy số count mã CZ nằm trong thư mục _codesPath/<orderNo>.db SELECT COUNT(*) FROM `UniqueCodes`;
            public (bool issucess, string mesage, int CzCodeCount) Get_Unique_Code_MES_Count(string orderNo)
            {
                try
                {
                    string czCodesPath = $@"{poAPIServerPath}/codes/{orderNo}.db";
                    using (var conn = new SQLiteConnection($"Data Source={czCodesPath};Version=3;"))
                    {
                        string query = "SELECT COUNT(*) FROM UniqueCodes";
                        var command = new SQLiteCommand(query, conn);
                        command.Parameters.AddWithValue("@orderNo", orderNo);
                        conn.Open();
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return (true, "Lấy số lượng mã CZ thành công.", count);
                    }
                }
                catch (Exception ex)
                {
                    return (false, $"Lỗi P03 khi lấy số lượng mã CZ: {ex.Message}", 0);
                }
            }
        }

        #endregion

        #region Các phương thức lấy từ dữ liệu sản xuất (run)

        public class GetDataPO
        {

        }

        #endregion
    }
}
