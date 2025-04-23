using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SPMS1
{
    public partial class InternetClass : Component
    {


        public InternetClass()
        {
            InitializeComponent();
        }

        [Description("URL to ping")]
        public string Url { get; set; } = "https://www.google.com";
        public InternetClass(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        public bool IsOK()
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send(Url);
                    return reply.Status == IPStatus.Success;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public event EventHandler<ErrorEventArgs> ErrorOccurred;

        public double GetInternetSpeed()
        {
            try
            {
                WebClient client = new WebClient();
                DateTime startTime = DateTime.Now;
                byte[] data = client.DownloadData("http://www.google.com");
                DateTime endTime = DateTime.Now;
                double bytes = data.Length;
                double seconds = (endTime - startTime).TotalSeconds;
                double speed = (bytes / 1024) / seconds; // Speed in KBps
                return speed;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex.HResult, ex.Message);
                return 0;
            }
                
        }

        protected virtual void OnErrorOccurred(int errorCode, string errorMessage)
        {
            ErrorOccurred?.Invoke(this, new ErrorEventArgs(errorCode, errorMessage));
        }

    }

    public class ErrorEventArgs : EventArgs
    {
        public int ErrorCode { get; }
        public string ErrorMessage { get; }

        public ErrorEventArgs(int errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }
}
