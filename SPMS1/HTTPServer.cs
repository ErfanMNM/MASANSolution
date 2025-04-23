using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace SPMS1
{
    public partial class HTTPServer : Component
    {
        private HttpListener listener = new HttpListener();
        private bool started = false;

        // Sự kiện khi có lỗi trong quá trình xuất
        public event EventHandler<Exception> ErrorOccurred;

        public HTTPServer()
        {
            InitializeComponent();
        }

        public HTTPServer(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        public string Host { get; set; } = "http://0.0.0.0:8080/";

        public void Start()
        {
            if (!started)
            {
                listener.Prefixes.Add(Host);
                backgroundWorker1.RunWorkerAsync();
                started = true;
            }
        }

        public void Stop()
        {
            backgroundWorker1.CancelAsync();
            listener.Stop();
            started = false;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // Bắt đầu lắng nghe yêu cầu
            listener.Start();
            while (!backgroundWorker1.CancellationPending)
            {
                try
                {
                    // Đợi yêu cầu từ client
                    HttpListenerContext context = listener.GetContext();
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    // Kiểm tra đường dẫn và tham số query
                    if (request.Url.AbsolutePath == "/sv1" && request.QueryString["content"] == "svstt")
                    {
                        SendResponse(context, "Server is running");
                    }
                    else if (request.Url.AbsolutePath == "/sv1" && request.QueryString["PRC"] == "1")
                    {
                        // Đường dẫn đến file CSV có sẵn trong ổ C
                        string csvFilePath = @"C:\Users\THUC\Downloads\Dtaat\SVA_1234.csv";  // Thay thế với đường dẫn thực tế đến file CSV
                        string zipFilePath = Path.Combine(Path.GetTempPath(), "SVA_1234.zip");

                        if (File.Exists(csvFilePath))
                        {
                            // Nén file CSV vào file ZIP
                            try
                            {
                                using (FileStream zipToCreate = new FileStream(zipFilePath, FileMode.Create))
                                using (ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create))
                                {
                                    ZipArchiveEntry readmeEntry = archive.CreateEntry("file.csv");
                                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                                    {
                                        // Đọc file CSV và ghi vào trong zip
                                        string csvContent = File.ReadAllText(csvFilePath);
                                        writer.Write(csvContent);
                                    }
                                }

                                // Cài đặt thông tin phản hồi cho file ZIP
                                response.ContentType = "application/zip";
                                response.ContentEncoding = Encoding.UTF8;
                                response.ContentLength64 = new FileInfo(zipFilePath).Length;

                                // Gửi file ZIP về client
                                using (FileStream fs = new FileStream(zipFilePath, FileMode.Open))
                                {
                                    fs.CopyTo(response.OutputStream);
                                }
                            }
                            catch (Exception ex)
                            {
                                // Xử lý lỗi nén file
                                response.StatusCode = 500; // Internal Server Error
                                SendResponse(context, "Error creating zip file: " + ex.Message);
                            }
                        }
                        else
                        {
                            // Nếu file CSV không tồn tại, trả về lỗi 404
                            response.StatusCode = 404;
                            SendResponse(context, "CSV file not found.");
                        }
                    }
                    else
                    {
                        // Nếu không phải yêu cầu hợp lệ, trả về mã lỗi 404
                        response.StatusCode = 404;
                        SendResponse(context, "Not Found");
                    }
                }
                catch (Exception ex)
                {
                    // Nếu có lỗi, gửi sự kiện lỗi
                    ErrorOccurred?.Invoke(this, ex);
                }
            }
        }

        private static void SendResponse(HttpListenerContext context, string message)
        {
            HttpListenerResponse response = context.Response;
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            response.ContentLength64 = buffer.Length;
            response.ContentType = "text/plain";
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
    }
}
