using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunny.UI;

namespace TestApp
{
    public partial class DataWizardForm : UIForm
    {
        public string[] GeneratedData { get; private set; }

        public DataWizardForm()
        {
            InitializeComponent();
            InitializeWizard();
        }

        private void InitializeWizard()
        {
            this.Text = "Data Wizard";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            
            // Default settings
            uiComboBox_DataType.SelectedIndex = 0;
            uiIntegerUpDown_Count.Value = 100;
            uiTextBox_Prefix.Text = "CODE";
            uiTextBox_StartNumber.Text = "1";
            uiIntegerUpDown_Length.Value = 6;
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                var dataList = new List<string>();
                int count = (int)uiIntegerUpDown_Count.Value;
                string dataType = uiComboBox_DataType.SelectedItem.ToString();

                switch (dataType)
                {
                    case "Số tuần tự":
                        GenerateSequentialNumbers(dataList, count);
                        break;
                    case "Mã có tiền tố":
                        GeneratePrefixedCodes(dataList, count);
                        break;
                    case "Mã ngẫu nhiên":
                        GenerateRandomCodes(dataList, count);
                        break;
                    case "Thời gian":
                        GenerateTimestamps(dataList, count);
                        break;
                }

                GeneratedData = dataList.ToArray();
                uiListBox_Preview.Items.Clear();
                
                // Show preview (max 20 items)
                int previewCount = Math.Min(20, GeneratedData.Length);
                for (int i = 0; i < previewCount; i++)
                {
                    uiListBox_Preview.Items.Add(GeneratedData[i]);
                }
                
                if (GeneratedData.Length > 20)
                {
                    uiListBox_Preview.Items.Add($"... và {GeneratedData.Length - 20} mục khác");
                }

                uiLabel_Status.Text = $"Đã tạo {GeneratedData.Length} mục dữ liệu";
                uiButton_OK.Enabled = true;
            }
            catch (Exception ex)
            {
                uiLabel_Status.Text = $"Lỗi: {ex.Message}";
            }
        }

        private void GenerateSequentialNumbers(List<string> dataList, int count)
        {
            int start = int.Parse(uiTextBox_StartNumber.Text);
            int length = (int)uiIntegerUpDown_Length.Value;

            for (int i = 0; i < count; i++)
            {
                dataList.Add((start + i).ToString().PadLeft(length, '0'));
            }
        }

        private void GeneratePrefixedCodes(List<string> dataList, int count)
        {
            string prefix = uiTextBox_Prefix.Text;
            int start = int.Parse(uiTextBox_StartNumber.Text);
            int length = (int)uiIntegerUpDown_Length.Value;

            for (int i = 0; i < count; i++)
            {
                string number = (start + i).ToString().PadLeft(length, '0');
                dataList.Add($"{prefix}{number}");
            }
        }

        private void GenerateRandomCodes(List<string> dataList, int count)
        {
            var random = new Random();
            int length = (int)uiIntegerUpDown_Length.Value;
            var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            for (int i = 0; i < count; i++)
            {
                var code = new StringBuilder();
                for (int j = 0; j < length; j++)
                {
                    code.Append(characters[random.Next(characters.Length)]);
                }
                dataList.Add(code.ToString());
            }
        }

        private void GenerateTimestamps(List<string> dataList, int count)
        {
            var startTime = DateTime.Now;

            for (int i = 0; i < count; i++)
            {
                var timestamp = startTime.AddSeconds(i);
                dataList.Add(timestamp.ToString("yyyyMMddHHmmss"));
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void UiComboBox_DataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string dataType = uiComboBox_DataType.SelectedItem?.ToString();
            
            // Enable/disable controls based on data type
            bool needsPrefix = dataType == "Mã có tiền tố";
            bool needsStartNumber = dataType == "Số tuần tự" || dataType == "Mã có tiền tố";
            bool needsLength = dataType != "Thời gian";

            uiTextBox_Prefix.Enabled = needsPrefix;
            uiTextBox_StartNumber.Enabled = needsStartNumber;
            uiIntegerUpDown_Length.Enabled = needsLength;
        }
    }
}