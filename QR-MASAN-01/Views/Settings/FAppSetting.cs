using SpT.Setting;
using Sunny.UI;
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
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace QR_MASAN_01.Views.Settings
{
    public partial class FAppSetting : UIPage
    {
        public FAppSetting()
        {
            InitializeComponent();
        }

        public void FAppSetting_Load()
        {
            LoadSettingToTreeView(treeView);
            //flowLayoutPanel1.Controls.Clear();

            //var sectionDict = new Dictionary<string, List<PropertyInfo>>();
            //foreach (var prop in typeof(Setting).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            //{
            //    if (prop.GetCustomAttribute<ConfigIgnoreAttribute>() != null)
            //        continue;

            //    var sectionAttr = prop.GetCustomAttribute<ConfigSectionAttribute>();
            //    string section = sectionAttr?.Section ?? "Setup";

            //    if (!sectionDict.ContainsKey(section))
            //        sectionDict[section] = new List<PropertyInfo>();

            //    sectionDict[section].Add(prop);
            //}

            //// Load các UC_Section
            //foreach (var kvp in sectionDict)
            //{
            //    UC_Section sectionUC = new UC_Section();
            //    sectionUC.Width = flowLayoutPanel1.ClientSize.Width - 30; // căn chỉnh vừa khít
            //    sectionUC.Init(kvp.Key, kvp.Value, Setting.Current);
            //    flowLayoutPanel1.Controls.Add(sectionUC);
            //}

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
                // Hiển thị InputBox để người dùng nhập giá trị mới
                string input = Microsoft.VisualBasic.Interaction.InputBox(
                    $"Nhập giá trị mới cho {prop.Name} (kiểu {prop.PropertyType.Name}):",
                    "Chỉnh Config",
                    oldValue?.ToString() ?? "");

                if (!string.IsNullOrWhiteSpace(input))
                {
                    try
                    {
                        object convertedValue = Convert.ChangeType(input, prop.PropertyType);
                        prop.SetValue(Setting.Current, convertedValue);
                        e.Node.Text = $"{prop.Name} = {convertedValue}";
                        MessageBox.Show($"Đã cập nhật {prop.Name} = {convertedValue} thành công 😎");
                    }
                    catch
                    {
                        MessageBox.Show($"Giá trị không hợp lệ cho {prop.PropertyType.Name}");
                    }
                }
            }


        }
    }
}
