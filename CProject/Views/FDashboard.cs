using Sunny.UI;
using CProject.Module;
using System.ComponentModel;
using Microsoft.Win32;

namespace CProject.Views
{
    public partial class FDashboard : UIPage
    {
        private readonly DataPoolModule _dataPool = new DataPoolModule();
        private BackgroundWorker? _workerImportFile;
        private string _workerFilePath = string.Empty;
        private string _workerPoolName = string.Empty;

        public FDashboard()
        {
            InitializeComponent();
            InitBackgroundWorker();
        }

        private void InitBackgroundWorker()
        {
            _workerImportFile = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _workerImportFile.DoWork += WorkerImportFile_DoWork;
            _workerImportFile.ProgressChanged += WorkerImportFile_ProgressChanged;
            _workerImportFile.RunWorkerCompleted += WorkerImportFile_RunWorkerCompleted;
        }

        private void WorkerImportFile_DoWork(object? sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            var args = e.Argument as object[];
            string poolName = (string)args![0];
            string filePath = (string)args[1];

            var result = _dataPool.AddCodes(
                poolName: poolName,
                mode: 0,
                filePath: filePath,
                singleCode: null,
                dataTable: null,
                createID: "SYSTEM",
                createdBy: "Admin",
                progressCallback: (current, total) =>
                {
                    int percent = (int)((double)current / total * 100);
                    worker?.ReportProgress(percent, $"Đang xử lý: {current}/{total}");
                }
            );

            e.Result = result;
        }

        private void WorkerImportFile_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            AddLog(e.UserState?.ToString() ?? "");
        }

        private void WorkerImportFile_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            btnAddCodeFile.Enabled = true;
            btnCreatePool.Enabled = true;
            btnAddCodeSingle.Enabled = true;
            btnGetPoolInfo.Enabled = true;

            if (e.Error != null)
            {
                AddLog($"Lỗi: {e.Error.Message}");
            }
            else if (e.Cancelled)
            {
                AddLog("Đã hủy!");
            }
            else
            {
                var result = e.Result as DataPoolAddCodesResult;
                if (result != null)
                {
                    AddLog($"Thêm từ File: {result.Message}");
                }
            }
        }

        private void AddLog(string message)
        {
            if (lstResult.InvokeRequired)
            {
                lstResult.Invoke(new Action(() =>
                {
                    lstResult.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
                    lstResult.SelectedIndex = lstResult.Items.Count - 1;
                }));
            }
            else
            {
                lstResult.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
                lstResult.SelectedIndex = lstResult.Items.Count - 1;
            }
        }

        private void btnCreatePool_Click(object? sender, EventArgs e)
        {
            string poolName = txtPoolName.Text.Trim();
            if (string.IsNullOrEmpty(poolName))
            {
                UIMessageBox.ShowWarning("Vui lòng nhập tên Pool!");
                return;
            }

            var poolInfo = new DataPoolModule.PoolInfo(
                id: 0,
                name: poolName,
                description: "Test Pool",
                batchID: "BATCH001",
                createID: "SYSTEM",
                note: "Created for testing",
                createdBy: "Admin",
                createDatetime: DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            );

            var result = _dataPool.CreatePool(poolInfo);
            if (result.Success)
            {
                AddLog($"Tạo Pool thành công: {poolName}");
                AddLog($"Đường dẫn: {result.Data}");
            }
            else
            {
                AddLog($"Lỗi: {result.Message}");
            }
        }

        private void btnAddCodeSingle_Click(object? sender, EventArgs e)
        {
            string poolName = txtPoolName.Text.Trim();
            string code = txtCode.Text.Trim();

            if (string.IsNullOrEmpty(poolName))
            {
                UIMessageBox.ShowWarning("Vui lòng nhập tên Pool!");
                return;
            }
            if (string.IsNullOrEmpty(code))
            {
                UIMessageBox.ShowWarning("Vui lòng nhập Code!");
                return;
            }

            var result = _dataPool.AddCodes(
                poolName: poolName,
                mode: 1,
                filePath: null,
                singleCode: code,
                dataTable: null,
                createID: "SYSTEM",
                createdBy: "Admin"
            );

            AddLog($"Thêm 1 Code: {result.Message}");
        }

        private void btnAddCodeFile_Click(object? sender, EventArgs e)
        {
            string poolName = txtPoolName.Text.Trim();
            if (string.IsNullOrEmpty(poolName))
            {
                UIMessageBox.ShowWarning("Vui lòng nhập tên Pool!");
                return;
            }

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files|*.txt;*.csv|All Files|*.*",
                Title = "Chọn file chứa mã code"
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Disable buttons during import
                btnAddCodeFile.Enabled = false;
                btnCreatePool.Enabled = false;
                btnAddCodeSingle.Enabled = false;
                btnGetPoolInfo.Enabled = false;

                AddLog($"Bắt đầu import file: {openFileDialog.SafeFileName}");
                
                _workerImportFile.RunWorkerAsync(new object[] { poolName, openFileDialog.FileName });
            }
        }

        private void btnGetPoolInfo_Click(object? sender, EventArgs e)
        {
            string poolName = txtPoolName.Text.Trim();
            if (string.IsNullOrEmpty(poolName))
            {
                UIMessageBox.ShowWarning("Vui lòng nhập tên Pool!");
                return;
            }

            var pathResult = _dataPool.GetPoolPath(poolName);
            if (pathResult.Success)
            {
                AddLog($"Pool Path: {pathResult.Data}");
                if (System.IO.File.Exists(pathResult.Data))
                {
                    AddLog("Pool tồn tại!");
                }
                else
                {
                    AddLog("Pool không tồn tại!");
                }
            }
            else
            {
                AddLog($"Lỗi: {pathResult.Message}");
            }
        }
    }
}
