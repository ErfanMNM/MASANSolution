using Org.BouncyCastle.Tls;
using SpT.Auth;
using SpT.Logs;
using SpT.Setting;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition.Primitives;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace QR_MASAN_01.Views.Settings
{
    public partial class FAppSetting : UIPage
    {
        public FAppSetting()
        {
            InitializeComponent();
        }

        //gọi log

       LogHelper<LoginAction> logssss;

        public void FAppSetting_Load()
        {
            LoadSettingToTreeView(treeView);
            logssss = new LogHelper<LoginAction>(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TanTien", "Logs", "userlog.logs"));
        }

        public void LoadSettingToTreeView(UITreeView treeView)
        {
            treeView.Nodes.Clear();
            var sectionDict = new Dictionary<string, List<PropertyInfo>>();

            foreach (var prop in typeof(Setting).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.GetCustomAttribute<ConfigIgnoreAttribute>() != null)
                    continue;

                var sectionAttr = prop.GetCustomAttribute<ConfigSectionAttribute>();
                string section = sectionAttr?.Section ?? "Setup";

                if (!sectionDict.ContainsKey(section))
                    sectionDict[section] = new List<PropertyInfo>();

                sectionDict[section].Add(prop);
            }

            foreach (var kvp in sectionDict)
            {
                TreeNode sectionNode = new TreeNode($"[{kvp.Key}]");
                sectionNode.Tag = null; // để phân biệt với prop node
                treeView.Nodes.Add(sectionNode);

                foreach (var prop in kvp.Value)
                {
                    object value = prop.GetValue(Setting.Current);
                    TreeNode propNode = new TreeNode($"{prop.Name} = {value}");
                    propNode.Tag = prop;
                    sectionNode.Nodes.Add(propNode);
                }
            }

            treeView.ExpandAll();
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {

            if (e.Node.Tag is PropertyInfo prop)
            {
                object oldValue = prop.GetValue(Setting.Current);
                object key = prop.Name;
            }

        }

        private void FAppSetting_Initialize(object sender, EventArgs e)
        {

            uc_UserSetting1.CurrentUserName = Globalvariable.CurrentUser.Username; // Thiết lập tên người dùng hiện tại
            uc_UserSetting1.INIT(); // Khởi tạo thông tin người dùng
            uc_UserManager2.CurrentUserName = Globalvariable.CurrentUser.Username; // Thiết lập tên người dùng hiện tại
            if(Globalvariable.CurrentUser.Role == "Admin")
            {
                uc_UserManager2.Enabled = true; // Hiển thị quản lý người dùng nếu là Admin
            }
            else
            {
                uc_UserManager2.Enabled = false; // Ẩn quản lý người dùng nếu không phải Admin
            }
        }

        private void uc_UserSetting1_OnUserAction(object sender, LoginActionEventArgs e)
        {
            this.ShowInfoTip(e.Message);
        }

        private void uc_UserManager2_OnAction(object sender, LoginActionEventArgs e)
        {

            this.ShowInfoTip(e.Message);

            Task.Run( async () =>
            {
                try
                {
                    await logssss.WriteLogAsync(Globalvariable.CurrentUser.Username, LoginAction.AdminPrivileges, e.Message);
                }
                catch (Exception ex)
                {
                    // log hoặc ignore
                    Console.WriteLine($"Lỗi ghi log: {ex.Message}");
                }

            });
        }
    }
}
