using System;
using System.Windows.Forms;

namespace QR_MASAN_01.Utils
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
}
