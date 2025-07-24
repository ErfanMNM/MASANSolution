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

namespace TeraUI.State
{
    public partial class tuStatePanel : UIUserControl
    {
        public int Blink_Interval { get; set; } = 1000; // Default blink interval in milliseconds
        public bool IsBlinking { get; set; } = true; // Flag to control blinking state
        public Color ONcolor { get; set; } = Color.Green;
        public Color OFFcolor { get; set; } = Color.FromArgb(255, 255, 255); // Default color for OFF state

        public bool IsOn
        {
            get { return opValue.FillColor == ONcolor; }
            set
            {
                if (value)
                {
                    opValue.FillColor = ONcolor;
                }
                else
                {
                    opValue.FillColor = OFFcolor;
                }
            }
        }

        public string Value
        {
            get { return opValue.Text; }
            set { opValue.Text = value; }
        }

        public Color BackGroundColor
        {
            get { return uiTableLayoutPanel6.BackColor; }
            set { uiTableLayoutPanel6.BackColor = value; }
        }

        public string Title
        {
            get { return opName.Text; }
            set { opName.Text = value; }
        }

        public tuStatePanel()
        {
            InitializeComponent();
        }

        private void tuStatePanel_Load(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if(!IsBlinking)
                    {
                        System.Threading.Thread.Sleep(Blink_Interval);
                        continue;
                    }
                    // Toggle the state of IsOn
                    if (IsOn)
                    {
                        Invoke(new Action(() => IsOn = false));
                    }
                    else
                    {
                        Invoke(new Action(() => IsOn = true));
                    }
                    System.Threading.Thread.Sleep(Blink_Interval);
                }
            });
        }
    }
}
