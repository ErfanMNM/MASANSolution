using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MASAN_SERIALIZATION.Views.Database
{
    public partial class DBBrowser : UIPage
    {
        public DBBrowser()
        {
            InitializeComponent();
        }

        private void DBBrowser_Load(object sender, EventArgs e)
        {
            webView21.Source = new Uri("C:\\MasanSerialization\\Database_Service\\sqliteviewer.html");
        }
    }
}
