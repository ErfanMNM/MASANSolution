using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SPMS1.MQTT;
using Sunny;
using Sunny.UI;

namespace QR_MASAN_01.Views
{
    public partial class FormTest : UIPage
    {
        public FormTest()
        {
            InitializeComponent();
           
        }

        private void MqtT_Client1_ClientCallback(enumMqttEvent @event, string topic, string message)
        {
            switch (@event)
            {
                case enumMqttEvent.CONNECTED:
                    Invoke(new Action(() =>
                    {
                        labelStatus.Text = "Đã kết nối MQTT";
                        labelStatus.BackColor = System.Drawing.Color.Green;
                    }));
                    break;

                case enumMqttEvent.DISCONNECTED:
                    Invoke(new Action(() =>
                    {
                        labelStatus.Text = "Mất kết nối MQTT";
                        labelStatus.BackColor = System.Drawing.Color.Red;
                    }));
                    break;

                case enumMqttEvent.RECEIVED:
                    Invoke(new Action(() =>
                    {
                        uiListBox1.Items.Add($"[{topic}] {message}");
                        uiListBox1.SelectedIndex = uiListBox1.Items.Count - 1;
                    }));
                    break;
            }
        }

        private async void btnSub_Click(object sender, EventArgs e)
        {

                await mqtT_Client1.SubscribeAsync("TIGER");
        }

        private async void btnPub_Click(object sender, EventArgs e)
        {
            await mqtT_Client1.PublishAsync("TIGER", "Xin chào MQTT!");
        }

        private void FormTest_Load(object sender, EventArgs e)
        {
            mqtT_Client1.ClientCallback += MqtT_Client1_ClientCallback;
            mqtT_Client1.ConnectAsync();
        }
    }
}
