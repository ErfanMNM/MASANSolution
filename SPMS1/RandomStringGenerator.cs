using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPMS1
{
    public partial class RandomStringGenerator : Component
    {
        private Random random = new Random();

        // Sự kiện khi tạo chuỗi thành công, trả về DataTable
        public event EventHandler<DataTable> Completed;

        // Sự kiện khi gặp lỗi, truyền đối tượng Exception
        public event EventHandler<Exception> ErrorOccurred;

        //trả về progress 
        public event EventHandler<Exception> ProgressChange;

        // Khởi tạo BackgroundWorker
        private BackgroundWorker backgroundWorker;

        public RandomStringGenerator()
        {
            InitializeBackgroundWorker();
        }

        public RandomStringGenerator(IContainer container)
        {
            container.Add(this);
            InitializeBackgroundWorker();
        }

        private void InitializeBackgroundWorker()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        /// <summary>
        /// Tạo chuỗi ngẫu nhiên trong luồng riêng với độ dài và ký tự tùy chọn.
        /// </summary>
        /// <param name="length">Độ dài của chuỗi ngẫu nhiên</param>
        /// <param name="count">Số lượng chuỗi cần tạo</param>
        /// <param name="chars">Chuỗi ký tự tùy chọn. Nếu không nhập, sẽ dùng giá trị mặc định.</param>
        public void GenerateRandomStrings(int length, int count, string chars = null)
        {
            try
            {
                // Nếu không có ký tự nhập, sử dụng ký tự mặc định
                if (string.IsNullOrEmpty(chars))
                {
                    chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                }

                // Khởi tạo dữ liệu để gửi qua BackgroundWorker
                var parameters = new object[] { length, count, chars };

                // Bắt đầu BackgroundWorker
                backgroundWorker.RunWorkerAsync(parameters);
            }
            catch (Exception ex)
            {
                // Kích hoạt sự kiện Error nếu có lỗi
                ErrorOccurred?.Invoke(this, ex);
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Nhận tham số từ RunWorkerAsync
            var parameters = (object[])e.Argument;
            int length = (int)parameters[0];
            int count = (int)parameters[1];
            string chars = (string)parameters[2];

            // Tạo DataTable để chứa các chuỗi
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Generated String", typeof(string));

            // HashSet để đảm bảo chuỗi không trùng lặp
            HashSet<string> generatedStrings = new HashSet<string>();
            int progress = 90;
            // Tạo chuỗi ngẫu nhiên
            for (int i = 0; i < count; i++)
            {
                if(i >= count/progress)
                {
                    backgroundWorker.ReportProgress(100-progress,count);
                }
                string randomString;
                do
                {
                    // Tạo chuỗi ngẫu nhiên
                    randomString = new string(Enumerable.Repeat(chars, length)
                        .Select(s => s[random.Next(s.Length)]).ToArray());
                } while (generatedStrings.Contains(randomString)); // Kiểm tra trùng lặp

                // Thêm chuỗi vào HashSet và DataTable
                generatedStrings.Add(randomString);
                dataTable.Rows.Add(randomString);
            }

            // Truyền kết quả cho RunWorkerCompleted
            e.Result = dataTable;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // Kích hoạt sự kiện Error nếu có lỗi
                ErrorOccurred?.Invoke(this, e.Error);
            }
            else
            {
                // Kích hoạt sự kiện Completed khi hoàn thành
                Completed?.Invoke(this, (DataTable)e.Result);
            }
        }

        
    }
}
