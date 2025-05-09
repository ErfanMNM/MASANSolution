using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using HslCommunication;
using Sunny.UI;
using DATALOGIC_SCAN;
using System.Drawing;
using System.Windows.Forms;
using Dialogs;
using Diaglogs;

namespace QR_MASAN_01
{
    public partial class ScanQR : UIPage
    {
        public static bool formOpen { get; set; } = true;
        Connection _ScanConection = new Connection();

    
        public ScanQR()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            if (!WK_Check.IsBusy)
            {
                if (Globalvariable.Data_Status == e_Data_Status.READY)
                {
                    if (!WK_Check.IsBusy)
                    {
                        WK_Check.RunWorkerAsync(ipQRContent.Text);
                    }
                    else
                    {
                        this.ShowErrorDialog("Vui lòng không thao tác liên tiếp nhiều lần");
                    }
                }
                else
                {
                   this.ShowErrorDialog("Vui lòng chờ hệ thống tải xong dữ liệu sản xuất!");
                }
            }
            else
            {
               this.ShowErrorDialog("LỖI!!!!");
            }
        }

        public void INIT()
        {
            //opScanerCOM.Text = Globalvariable.config.ScanerCOM;
            _ScanConection.SERIALPORT = serialPort1;
            _ScanConection.EVENT += _ScanConection_EVENT; ;
            _ScanConection.LOAD();
            _ScanConection.CONNECT("COM2");
        }

        private void _ScanConection_EVENT(e_Serial e, string s)
        {
            switch (e)
            {
                case e_Serial.Connected:
                    Invoke(new Action(() => { opScanerSTT.Text = "Kết nối"; opScanerSTT.FillColor = Globalvariable.OK_Color; }));
                    break;
                case e_Serial.Disconnected:
                    Invoke(new Action(() => { opScanerSTT.Text = "Mất kết nối"; opScanerSTT.FillColor = Globalvariable.NG_Color; }));
                    break;
                case e_Serial.Recive:
                    string content = s;
                    if(formOpen)
                    {
                        if (Globalvariable.Data_Status == e_Data_Status.READY)
                        {
                            if (!WK_Check.IsBusy)
                            {
                                Invoke(new Action(() => { ipQRContent.Text = content; }));
                                WK_Check.RunWorkerAsync(content);
                            }
                            else
                            {
                                this.ShowErrorDialog("Vui lòng không thao tác liên tiếp nhiều lần");
                            }
                        }
                        else
                        {
                            this.ShowErrorDialog("Vui lòng chờ hệ thống tải xong dữ liệu sản xuất!");
                        }
                    }
                    else
                    {
                        this.ShowErrorDialog("Vui lòng bật sang bảng Quét QR!!!!");
                    }
                    
                    break;
                default:
                    break;
            }
        }
        bool Mode = false;
        public void SearchCode(string searchQR)
        {
            //tìm kiếm mã QR 
            if (Globalvariable.Data_Status == e_Data_Status.READY)
            {
                if (!string.IsNullOrEmpty(searchQR) && searchQR.Contains(Globalvariable.QRgoc))
                {
                    if (Globalvariable.ProductQR_Dictionary.TryGetValue(searchQR, out ProductData ProductInfo))
                    {
                        if (ProductInfo.Active != 1)
                        {
                            if(Mode)
                            {
                                ProductInfo.Active = 1;
                                ProductInfo.TimeStamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                                //đưa vào hàng chờ để xử lí data
                                Globalvariable.UpdateQueue120.Enqueue(ProductInfo.ProductID);

                                this.Invoke(new Action(() =>
                                {
                                    opCMD.Items.Add($"{DateTime.Now:HH:mm:ss}: Sản phẩm: `{searchQR}` Đã đưa vào hàng chờ kích hoạt thành công");
                                    opCMD.SelectedIndex = opCMD.Items.Count - 1;
                                }));
                            }
                            else
                            {
                                this.Invoke(new Action(() =>
                                {
                                    opCMD.Items.Add($"{DateTime.Now:HH:mm:ss}: Sản phẩm: `{searchQR}` Chưa được kích hoạt");
                                    opCMD.SelectedIndex = opCMD.Items.Count - 1;
                                }));
                            }    
                            
                        }
                        else
                        {
                            this.Invoke(new Action(() =>
                            {
                                opCMD.Items.Add($"{DateTime.Now:HH:mm:ss}: Sản phẩm: `{searchQR}` ĐÃ KÍCH HOẠT lúc : {ProductInfo.TimeStamp}");
                                opCMD.SelectedIndex = opCMD.Items.Count - 1;
                            }));
                        }
                    }
                    else
                    {
                        //đá chai không có trong data
                        if (Globalvariable.APPMODE != e_Mode.OLDMode)
                        {
                            this.Invoke(new Action(() =>
                            {
                                opCMD.Items.Add($"{DateTime.Now:HH:mm:ss}: KHÔNG TÌM THẤY sản phẩm: `{searchQR}` : {ProductInfo.TimeStamp}");
                                opCMD.SelectedIndex = opCMD.Items.Count - 1;
                            }));
                        }
                        else
                        {
                            if (Mode)
                            {
                                Globalvariable.AddQueue120.Enqueue(searchQR);

                                this.Invoke(new Action(() =>
                                {
                                    opCMD.Items.Add($"{DateTime.Now:HH:mm:ss}: Sản phẩm: `{searchQR}` Đã đưa vào hàng chờ kích hoạt thành công");
                                    opCMD.SelectedIndex = opCMD.Items.Count - 1;
                                }));
                            }
                            else
                            {
                                this.Invoke(new Action(() =>
                                {
                                    opCMD.Items.Add($"{DateTime.Now:HH:mm:ss}: Sản phẩm: `{searchQR}` Chưa được kích hoạt");
                                    opCMD.SelectedIndex = opCMD.Items.Count - 1;
                                }));
                            }
                                
                        }
                    }
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        opCMD.Items.Add($"{DateTime.Now:HH:mm:ss}: Mã QR đầu vào không đúng định dạng sản xuất hôm nay.");
                        opCMD.SelectedIndex = opCMD.Items.Count - 1;
                    }));
                }
                
            }
            else
            {
                this.Invoke(new Action(() =>
                {
                    opCMD.Items.Add($"{DateTime.Now:HH:mm:ss}: Vui lòng chờ máy lấy dữ liệu sản xuất hoàn tất rồi thử lại.");
                    opCMD.SelectedIndex = opCMD.Items.Count - 1;
                }));
            }

        }

        private void WK_Check_DoWork(object sender, DoWorkEventArgs e)
        {
            string searchQR = e.Argument as string; // Nhận giá trị truyền vào
            SearchCode(searchQR);
        }
        private void ScanQR_Initialize(object sender, EventArgs e)
        {
            formOpen = true;
        }

        private void ScanQR_Finalize(object sender, EventArgs e)
        {
            formOpen = false;
        }

        private void btnKeyBoard_Click(object sender, EventArgs e)
        {
            using (var dialog = new Entertext())
            {
                dialog.TextValue = ipQRContent.Text;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ipQRContent.Text = dialog.TextValue;
                }
            }
        }

        private void ipSWMode_ValueChanged(object sender, bool value)
        {
            if(ipSWMode.Active)
            {
                opModeMess.Text = "Phần mềm sẽ tự kích hoạt mã mới lưu vào csdl khi quét";
                Mode = true;
            }
            else
            {
                opModeMess.Text= "Phần mềm chỉ hiện thị trạng thái mã, gạt sw để thay đổi";
                Mode = false;
            }
        }
    }
}
