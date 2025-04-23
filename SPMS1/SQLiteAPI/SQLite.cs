using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPMS1.SQLiteAPI
{
    public partial class SQLite : Component
    {
        public SQLite()
        {
            InitializeComponent();
        }

        public SQLite(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public class SQLite_Result
        {
            //public bool 
        }
    }
}
