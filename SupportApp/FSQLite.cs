using SpT.SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SupportApp
{
    public partial class FSQLite : Form
    {
        public FSQLite()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //mở dialog tìm file dạng .db sau đó điền vào đường dẫn của file đó

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "SQLite Database Files (*.db)|*.db|All Files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string dbPath = openFileDialog.FileName;
                    // Hiển thị đường dẫn file trong TextBox
                    textBox1.Text = dbPath;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SQLiteToCSVExporter.ExportToCSV(textBox1.Text, "SELECT \"_rowid_\",* FROM \"main\".\"UniqueCodes\" LIMIT 49999 OFFSET 0;", "C:/.ABC/tesst.csv");
        }
    }
}
