using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage;
using Google.Cloud.Storage.V1;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using Google.Cloud.BigQuery.V2;
using System.Data;
using System.Windows.Forms;

namespace SPMS1
{
    public partial class GoogleService : Component
    {
        public GoogleService()
        {
            InitializeComponent();
        }

        public GoogleService(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        private string upload_filePatch;

        [Category("Custom Properties")]
        [Description("Đường dẫn tệp cần tải lên.")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string Upload_filePatch
        {
            get { return upload_filePatch; }
            set { upload_filePatch = value; }
        }

        [Description("Tên của dự án (BucketName hoặc ProjectName)")]
        public string projectName { get; set; }

        [Category("Custom Properties")]
        [Description("Đường dẫn ")]
        public string ObjectName { get; set; }

        public string DataSheetID { get; set; }
        public string TableID { get; set; }
        [Category("Custom Properties")]
        [Description("Loại nội dung(Ví dụ: Text/Csv,...)")]
        public string contentType { get; set; } = null;


        [Category("Thông tin xác thực")]
        [Description("Đường dẫn tệp xác thực, tệp khóa.")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string credentialFile { get; set; }
        

        public Result Upload()
        {
            try
            {
                // Đảm bảo filePatch không null hoặc rỗng
                if (string.IsNullOrEmpty(Upload_filePatch))
                {
                    return new Result
                    {
                        Success = false,
                        Message = "Đường dẫn tệp cần tải lên không tồn tại"
                    };
                }

                // Chuyển đổi đường dẫn để phù hợp với định dạng
                Upload_filePatch = Upload_filePatch.Replace("\\", "/");

                // Mở file stream từ filePatch
                FileStream fileStream = null;
                try
                {
                    fileStream = File.OpenRead(Upload_filePatch);

                    // Tạo StorageClient từ file xác thực JSON
                    GoogleCredential credential = GoogleCredential.FromFile(credentialFile);
                    StorageClient storage = StorageClient.Create(credential);

                    // Upload file
                    var uploadedObject = storage.UploadObject(projectName, ObjectName, null, fileStream);

                    // Kiểm tra kết quả upload
                    if (uploadedObject != null)
                    {
                        return new Result
                        {
                            Success = true,
                            Message = "Tải lên tệp thành công"
                        };
                    }
                    else
                    {
                        return new Result { Success = false, Message = "Tải lên thất bại" };
                    }
                }
                catch (Exception ex) 
                {
                    return new Result { Success = false, Message = $"Tải lên thất bại: {ex.Message}" };
                }
                finally
                {
                    fileStream?.Dispose();
                }
            }
            catch (Exception ex)
            {
                // Trả về thông báo lỗi nếu có exception xảy ra
                return new Result { Success = false, Message = $"Tải lên thất bại: {ex.Message}" };
            }
        }

        public (bool success, string message) Download(string destinationFilePath)
        {
            try
            {
                // Tạo StorageClient từ file xác thực JSON
                GoogleCredential credential = GoogleCredential.FromFile(credentialFile);
                StorageClient storage = StorageClient.Create(credential);

                // Tải file về từ Google Storage
                FileStream outputFileStream = null;
                try
                {
                    outputFileStream = File.OpenWrite(destinationFilePath);
                    storage.DownloadObject(projectName, ObjectName, outputFileStream);
                }
                finally
                {
                    outputFileStream?.Dispose();
                }

                return (true, "Download successful.");
            }
            catch (Exception ex)
            {
                // Trả về thông báo lỗi nếu có exception xảy ra
                return (false, $"Download failed: {ex.Message}");
            }
        }

        public Big_Query_Result BigQuery(string Query)
        {
            DataTable dataTable = new DataTable();
            try
            {
                // Tạo StorageClient từ file xác thực JSON
                GoogleCredential credential = GoogleCredential.FromFile(credentialFile);
                BigQueryClient client = BigQueryClient.Create(projectName, credential);
                BigQueryTable table = client.GetTable(projectName, DataSheetID, TableID);

                // Truy vấn BigQuery
                //string query = $@"
                //SELECT *
                //FROM `sales-268504.FactoryIntegration.BatchProduction` 
                //WHERE `SUB_INV` = @sub_inv
                //AND `ORG_CODE` = ""MIP""
                //ORDER BY `LAST_UPDATE_DATE` DESC;
                //";

                BigQueryResults results = client.ExecuteQuery(Query, null);
                // Thêm cột vào DataTable
                foreach (var field in results.Schema.Fields)
                {
                    dataTable.Columns.Add(field.Name);
                }

                // Thêm hàng vào DataTable
                foreach (var row in results)
                {
                    DataRow dataRow = dataTable.NewRow();
                    foreach (var field in results.Schema.Fields)
                    {
                        dataRow[field.Name] = row[field.Name];
                    }
                    dataTable.Rows.Add(dataRow);
                }
                return new Big_Query_Result
                {
                    Success = true,
                    Message = "Truy vấn thành công",
                    DataTable = dataTable
                };
            }
            catch (Exception ex)
            {
                // Trả về thông báo lỗi nếu có exception xảy ra
                return new Big_Query_Result
                {
                    Success = true,
                    Message = $"Lỗi khi dùng bigquery: {ex.Message}",
                    DataTable = dataTable
                };
            }
        }

        public class Big_Query_Result
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public DataTable DataTable { get; set; }
        }

        public class Result
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }
    }
}
