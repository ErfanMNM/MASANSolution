using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MASAN_SERIALIZATION.Utils
{
    public static class FormExtension
    {
        /// <summary>
        /// Gọi action trong UI thread nếu cần, tránh lặp Invoke(new Action(() => {}))
        /// </summary>
        public static void InvokeIfRequired(this Control control, Action action)
        {
            if (control == null || control.IsDisposed) return;

            if (control.InvokeRequired)
            {
                try
                {
                    control.Invoke(action);
                }
                catch
                {
                    // Nuốt lỗi khi form đang dispose
                }
            }
            else
            {
                action();
            }
        }

        
    }


    public static class TreeViewExtensions
    {
        public static void AddObject(this TreeView treeView, object obj, string rootName = null)
        {
            if (obj == null) return;

            Type type = obj.GetType();
            TreeNode rootNode = new TreeNode(rootName ?? type.Name);

            foreach (PropertyInfo prop in type.GetProperties())
            {
                object value = prop.GetValue(obj);
                rootNode.Nodes.Add($"{prop.Name}: {value}");
            }

            treeView.Nodes.Add(rootNode);
        }

        public static void UIAddObject(this UITreeView treeView, object obj, string rootName = null)
        {
            if (obj == null) return;

            Type type = obj.GetType();
            TreeNode rootNode = new TreeNode(rootName ?? type.Name);

            foreach (PropertyInfo prop in type.GetProperties())
            {
                object value = prop.GetValue(obj);
                rootNode.Nodes.Add($"{prop.Name}: {value}");
            }

            treeView.Nodes.Add(rootNode);
        }

    }
}
