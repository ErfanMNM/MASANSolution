using Sunny.UI;
using CProject.Module;


namespace CProject.Views
{
    public partial class FDashboard : UIPage
    {
        private readonly DataPoolModule _dataPool = new DataPoolModule();

        public FDashboard()
        {
            InitializeComponent();
        }

        private void AddLog(string message)
        {
            lstResult.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            lstResult.SelectedIndex = lstResult.Items.Count - 1;
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
                var result = _dataPool.AddCodes(
                    poolName: poolName,
                    mode: 0,
                    filePath: openFileDialog.FileName,
                    singleCode: null,
                    dataTable: null,
                    createID: "SYSTEM",
                    createdBy: "Admin"
                );

                AddLog($"Thêm từ File: {result.Message}");
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
