using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.IO;

namespace SqlLiteClass
{
    public class SqlLite_Lib
    {
        public string _DBFILEPATH = "";
        public SqlLite_Lib() { }
        public SqlLite_Lib(string _path)
        {
            _DBFILEPATH = _path;
        }

        public List<string> Read1Column(string TableName, string Query, string collName)
        {
            List<string> temp = new List<string>();

            string cmd = "SELECT " + collName + " FROM " + TableName + " WHERE " + Query;
            using (SQLiteConnection _sqlLiteCONN = new SQLiteConnection("Data Source=" + _DBFILEPATH + ";Version=3;Compress=True;"))
            {
                _sqlLiteCONN.Open();
                using (SQLiteCommand CMD = new SQLiteCommand(cmd, _sqlLiteCONN))
                {
                    using (SQLiteDataReader Reader = CMD.ExecuteReader())
                    {
                        try
                        {
                            while (Reader.Read())
                            {
                                temp.Add(Reader.GetString(0));
                            }
                        }
                        catch (Exception)
                        {
                            return new List<string>();
                        }
                    }
                }
            }
            return temp;
        }
       
        public List<string> Get1Row(string TableName, string Query)
        {        
            List<string> temp = new List<string>();
            string cmd = "SELECT * FROM " + TableName + " WHERE " + Query;
            using (SQLiteConnection _sqlLiteCONN1 = new SQLiteConnection("Data Source=" + _DBFILEPATH + ";Version=3;Compress=True;"))
            {
                _sqlLiteCONN1.Open();
                using (SQLiteCommand CMD = new SQLiteCommand(cmd, _sqlLiteCONN1))
                {
                    using (SQLiteDataReader Reader = CMD.ExecuteReader())
                    {
                        try
                        {
                            while (Reader.Read())
                            {
                                for (int i = 0; i < Reader.FieldCount; i++)
                                {
                                    temp.Add(Reader.GetValue(i).ToString());
                                }
                            }
                        }
                        catch (Exception)
                        {
                            return temp;
                        }
                    }
                }
                _sqlLiteCONN1.Close();
            }

            return temp;
        }
        public void LOAD_CSV()
        {
            string csvFilePath = OpenCsvFileDialog();
            if (string.IsNullOrEmpty(csvFilePath)) return;
            try { ImportCsv(csvFilePath); }
            catch (Exception ex) { Console.WriteLine($"Đã xảy ra lỗi: {ex.Message}"); }
        }

        public int ImportCsv(string csvFilePath)
        {
            string[] lines = File.ReadAllLines(csvFilePath);
            using (var connection = new SQLiteConnection($"Data Source={_DBFILEPATH};Version=3;"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                using (var command = new SQLiteCommand("INSERT INTO CaseQRContent (CaseQR) VALUES (@CaseQR)", connection, transaction))
                {
                    int inserted = 0;
                    command.Parameters.Add("@CaseQR", DbType.String);
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        command.Parameters["@CaseQR"].Value = line.Trim();
                        command.ExecuteNonQuery();
                        inserted++;
                    }
                    transaction.Commit();
                    return inserted;
                }
            }
        }

       public  string OpenCsvFileDialog()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                openFileDialog.Title = "Chọn file CSV";

                // Hiển thị hộp thoại
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return openFileDialog.FileName;
                }
            }

            return null;
        }
    }
}
