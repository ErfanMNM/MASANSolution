using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpT.Setting
{
    public partial class UC_Section : UserControl
    {
        public UC_Section()
        {
            InitializeComponent();
        }
        public void Init(string sectionName, List<PropertyInfo> props, object instance)
        {
            uiTitlePanel1.Text = $"[{sectionName}]";

            foreach (var prop in props)
            {
                stringtype item = new stringtype();
                //item.Dock = DockStyle.Top;
                item.Init(prop, instance);
                uiFlowLayoutPanel1.Controls.Add(item);
            }
        }
    }
}
