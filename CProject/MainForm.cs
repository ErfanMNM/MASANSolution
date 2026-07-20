using CProject.Views;
using Sunny.UI;

namespace CProject
{
    public partial class MainForm : UIForm
    {
        private FDashboard _fdashboard = new FDashboard();
        public MainForm()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            InitUI(MainTabBody);
        }

        private void InitUI(UITabControl mainTabBody)
        {
            mainTabBody = MainTabBody;
            MainNavMenu.TabControl = MainTabBody;
            MainNavMenu.CreateNode(AddPage(_fdashboard, 1001));
            MainNavMenu.SelectPage(1001);
        }


    }
}
