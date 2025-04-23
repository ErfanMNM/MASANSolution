using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPMS1.SQLiteAPI
{
    internal class SQLiteService
    {
        //public DataTable Get_Pallet_With_Hour(string TimeSearch)
        //{
        //    using (SQLiteConnection connection = new SQLiteConnection($"Data Source={Server_Database_File};Version=3;"))
        //    {
        //        connection.Open();
        //        string query = $"SELECT `PalletQR`, `CaseQRs`, `StartTime`, `StopTime` FROM PalletData WHERE StartTime LIKE '%{TimeSearch}%';";

        //        using (SQLiteCommand command = new SQLiteCommand(query, connection))
        //        {
        //            using (SQLiteDataAdapter da = new SQLiteDataAdapter(command))
        //            {
        //                DataTable dt = new DataTable();
        //                da.Fill(dt);
        //                return dt;
        //            }
        //        }
        //    }
        //}
    }
}
