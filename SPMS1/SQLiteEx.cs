using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPMS1
{
    public partial class SQLiteEx : Component
    {

        public string Database_File_Patch {  get; set; }
        public string CsV_File_Patch { get; set; }
        public SQLiteEx()
        {
            InitializeComponent();
        }

        public SQLiteEx(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void CsV_Insert_To_Sqlite()
        {

        }
    }
}
