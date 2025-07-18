using Sunny.UI;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        UdpClient udpReceiver;
        bool isReceiving = false;
        UdpClient udpClient;
        IPEndPoint plcEndPoint;


        private void StartReceiving()
        {
            if (isReceiving) return;
            isReceiving = true;

            int localPort = 9600;
            udpReceiver = new UdpClient(localPort);
            udpReceiver.BeginReceive(ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedData = udpReceiver.EndReceive(ar, ref remoteEP);

                string hexString = BitConverter.ToString(receivedData);
                string textString = Encoding.ASCII.GetString(receivedData);

                // Đẩy log lên UI thread
                this.Invoke(new Action(() =>
                {
                    uiListBox1.Items.Add($"[Recv] {remoteEP.Address}:{remoteEP.Port} => {hexString}");
                    uiListBox1.TopIndex = uiListBox1.Items.Count - 1;
                }));

                udpReceiver.BeginReceive(ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    uiListBox1.Items.Add($"[Recv Error] {ex.Message}");
                }));
            }
        }

        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            try
            {
                string hexString = uiTextBox1.Text.Trim();
                byte[] data = HexStringToByteArray(hexString);

                // Gửi frame FINS
                udpClient.Send(data, data.Length, plcEndPoint);
                uiListBox1.Items.Add($"[Sent] {BitConverter.ToString(data)}");
                uiListBox1.TopIndex = uiListBox1.Items.Count - 1;

                // Nhận phản hồi
                Task.Run(() =>
                {
                    try
                    {
                        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                        byte[] received = udpClient.Receive(ref remoteEP);
                        string hexResp = BitConverter.ToString(received);

                        this.Invoke(new Action(() =>
                        {
                            uiListBox1.Items.Add($"[Recv] {remoteEP.Address}:{remoteEP.Port} => {hexResp}");
                            uiListBox1.TopIndex = uiListBox1.Items.Count - 1;
                        }));
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(new Action(() =>
                        {
                            uiListBox1.Items.Add($"[Recv Error] {ex.Message}");
                            uiListBox1.TopIndex = uiListBox1.Items.Count - 1;
                        }));
                    }
                });
            }
            catch (Exception ex)
            {
                uiListBox1.Items.Add($"[Error] {ex.Message}");
                uiListBox1.TopIndex = uiListBox1.Items.Count - 1;
            }
        }


        private byte[] HexStringToByteArray(string hex)
        {
            hex = hex.Replace(" ", "").Replace("-", "");
            if (hex.Length % 2 != 0)
                throw new Exception("Hex length phải chẵn.");

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Init UDP client cho vừa gửi vừa nhận
            udpClient = new UdpClient(0); // OS tự cấp port local
            udpClient.Client.ReceiveTimeout = 3000; // 3s timeout nếu cần

            string plcIP = "127.0.0.1"; // đổi IP PLC thật của ông ở đây
            int plcPort = 9600;
            plcEndPoint = new IPEndPoint(IPAddress.Parse(plcIP), plcPort);
        }

    }
}
