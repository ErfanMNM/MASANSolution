using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPMS1
{
    public static class ScreenControl
    {
        public static void ToggleFullScreen(dynamic control)
        {

            if (control.WindowState != FormWindowState.Maximized)
            {
                control.Tag = control.WindowState;
                control.WindowState = FormWindowState.Normal;
                control.FormBorderStyle = FormBorderStyle.None;
                control .WindowState = FormWindowState.Maximized;
            }
            else
            {
                control.WindowState = (FormWindowState)control.Tag;
                control.FormBorderStyle = FormBorderStyle.Sizable;
            }


        }
    }

}
