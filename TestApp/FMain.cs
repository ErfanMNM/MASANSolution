using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


using SpT.Logs;
using Sunny.UI;

namespace TestApp
{
    public partial class FMain : UIForm
    {
        #region Private Fields - Page Controls

        COMV COMVPage = new COMV();

        #endregion

        #region Private Fields - Background Workers

        #endregion

        #region Constructor
        public FMain()
        {
            InitializeComponent();
            InitializeUI();
        }
        #endregion

        #region Form Events
        private void FMain_Load(object sender, EventArgs e)
        {
            ToggleFullScreen();
            InitializePage();
        }

        private void btnAppClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMini_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        #endregion

        #region Private Methods - Initialization
        private void InitializeUI()
        {
            try
            {
                UIStyles.CultureInfo = CultureInfos.en_US;
                UIStyles.GlobalFont = true;
                UIStyles.GlobalFontName = "Tahoma";

                MainTabControl = TabBody;
                NavMenu.TabControl = TabBody;
            }
            catch (Exception ex)
            {
                this.ShowErrorTip($"Lỗi EM01 khởi tạo giao diện: {ex.Message}");
            }
        }

        private void InitializePage()
        {
            NavMenu.Nodes.Clear();
            NavMenu.CreateNode(AddPage(COMVPage, 1001));
        }
        #endregion

        #region Private Methods - UI Operations
        private void ToggleFullScreen()
        {
            if (this.WindowState != FormWindowState.Maximized)
            {
                this.Tag = this.WindowState;
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = (FormWindowState)this.Tag;
                this.FormBorderStyle = FormBorderStyle.Sizable;
            }
        }
        #endregion

        
    }
}
