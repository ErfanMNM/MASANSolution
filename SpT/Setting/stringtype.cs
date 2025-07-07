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

    public partial class stringtype : UserControl
    {
        public PropertyInfo PropInfo { get; set; }
        public object Instance { get; set; }

        public stringtype()
        {
            InitializeComponent();
        }

        public void Init(PropertyInfo prop, object instance)
        {
            PropInfo = prop;
            Instance = instance;

            labelName.Text = prop.Name;

            if (prop.PropertyType == typeof(bool))
            {
                checkBoxValue.Visible = true;
                textBoxValue.Visible = false;

                checkBoxValue.Active = (bool)prop.GetValue(instance);
                checkBoxValue.ActiveChanged += (s, e) =>
                {
                    prop.SetValue(instance, checkBoxValue.Active);
                };
            }
            else
            {
                checkBoxValue.Visible = false;
                textBoxValue.Visible = true;

                textBoxValue.Text = prop.GetValue(instance)?.ToString();
                textBoxValue.TextChanged += (s, e) =>
                {
                    try
                    {
                        object converted = Convert.ChangeType(textBoxValue.Text, prop.PropertyType);
                        prop.SetValue(instance, converted);
                    }
                    catch
                    {
                        // ignore, hoặc hiển thị lỗi
                    }
                };
            }
        }
    }
}
