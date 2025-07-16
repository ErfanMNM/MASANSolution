using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpT.Static
{
    
    public partial class panelS : UserControl
    {

        public string LabelName
        {
            get => opName.Text;
            set => opName.Text = value;
        }

        public string LabelValue
        {
            get => opValue.Text;
            set => opValue.Text = value;
        }
        public panelS()
        {
            InitializeComponent();
        }
    }
}
