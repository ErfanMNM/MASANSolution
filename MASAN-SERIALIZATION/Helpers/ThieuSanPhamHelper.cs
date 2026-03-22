using MASAN_SERIALIZATION.Configs;
using MASAN_SERIALIZATION.Production;
using MASAN_SERIALIZATION.Utils;
using System;
using System.Threading;
using System.Windows.Forms;

namespace MASAN_SERIALIZATION.Helpers
{
    /// <summary>
    /// Helper class to handle ThieuSanPham (product shortage) warning actions:
    /// - Blink red text on opTer
    /// - Write alarm codes to PLC
    /// - Enable/disable reset button
    /// </summary>
    public class ThieuSanPhamHelper
    {
        private static System.Windows.Forms.Timer _blinkTimer;
        private static bool _isBlinking = false;
        private static bool _blinkState = false;
        private static Control _opTerControl;
        private static string _warningText = "THIẾU SẢN PHẨM TRONG THÙNG - VUI LÒNG KIỂM TRA VÀ NHẤN RESET!";
        private static readonly object _lock = new object();

        /// <summary>
        /// Start the warning sequence when entering ThieuSanPham state:
        /// 1. Enable btnResetPO
        /// 2. Start blinking red text on opTer
        /// 3. Write alarm codes 2 and 3 to PLC
        /// </summary>
        public static void TriggerThieuSanPhamWarning(Control opTer)
        {
            lock (_lock)
            {
                _opTerControl = opTer;
                StartBlinkingText();
                WriteAlarmToPLC();
            }
        }

        /// <summary>
        /// Stop the warning sequence when exiting ThieuSanPham state.
        /// Also clears the PLC alarm (writes 0).
        /// </summary>
        public static void StopThieuSanPhamWarning()
        {
            lock (_lock)
            {
                StopBlinkingText();
                ClearPLCAlarm();
            }
        }

        /// <summary>
        /// Start blinking red text on opTer
        /// </summary>
        private static void StartBlinkingText()
        {
            if (_blinkTimer != null)
            {
                _blinkTimer.Stop();
                _blinkTimer.Dispose();
            }

            _blinkState = false;
            _isBlinking = true;

            _blinkTimer = new System.Windows.Forms.Timer();
            _blinkTimer.Interval = 500; // Blink every 500ms
            _blinkTimer.Tick += BlinkTimer_Tick;
            _blinkTimer.Start();

            // Set initial text immediately
            UpdateOpTerText(true);
        }

        /// <summary>
        /// Stop blinking text
        /// </summary>
        private static void StopBlinkingText()
        {
            _isBlinking = false;

            if (_blinkTimer != null)
            {
                _blinkTimer.Stop();
                _blinkTimer.Dispose();
                _blinkTimer = null;
            }

            // Restore normal text
            if (_opTerControl != null && _opTerControl.IsHandleCreated)
            {
                _opTerControl.InvokeIfRequired(() =>
                {
                    _opTerControl.Text = "";
                    _opTerControl.Visible = true;
                });
            }
        }

        private static void BlinkTimer_Tick(object sender, EventArgs e)
        {
            _blinkState = !_blinkState;
            UpdateOpTerText(_blinkState);
        }

        private static void UpdateOpTerText(bool visible)
        {
            if (_opTerControl == null || !_opTerControl.IsHandleCreated)
                return;

            try
            {
                _opTerControl.InvokeIfRequired(() =>
                {
                    if (visible)
                    {
                        _opTerControl.Text = _warningText;
                        _opTerControl.ForeColor = System.Drawing.Color.Red;
                        _opTerControl.Visible = true;
                    }
                    else
                    {
                        _opTerControl.Text = "";
                        _opTerControl.Visible = true;
                    }
                });
            }
            catch
            {
                // Ignore cross-thread invocation errors during shutdown
            }
        }

        /// <summary>
        /// Write alarm code 1 to PLC (both alarm lights).
        /// Reference: FDashboard.cs lines 1844-1894.
        /// Writing 1 triggers both warning lights.
        /// </summary>
        private static void WriteAlarmToPLC()
        {
            try
            {
                if (AppConfigs.Current.PLC_Duo_Mode)
                {
                    if (Globals.PLC_Connected_02 && Globals.PLC_02 != null)
                    {
                        Globals.PLC_02.Write(PLCAddress.Get("PLC2_Alarm_DM_C1"), 1);
                    }
                }
                else
                {
                    if (Globals.PLC_Connected && Globals.PLC != null)
                    {
                        Globals.PLC.Write(PLCAddress.Get("PLC_Alarm_DM_C1"), 1);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi ghi alarm PLC: {ex.Message}");
            }
        }

        /// <summary>
        /// Clear PLC alarm (write 0) to turn off both lights.
        /// </summary>
        public static void ClearPLCAlarm()
        {
            try
            {
                if (AppConfigs.Current.PLC_Duo_Mode)
                {
                    if (Globals.PLC_Connected_02 && Globals.PLC_02 != null)
                    {
                        Globals.PLC_02.Write(PLCAddress.Get("PLC2_Alarm_DM_C1"), 0);
                    }
                }
                else
                {
                    if (Globals.PLC_Connected && Globals.PLC != null)
                    {
                        Globals.PLC.Write(PLCAddress.Get("PLC_Alarm_DM_C1"), 0);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi xóa alarm PLC: {ex.Message}");
            }
        }

        /// <summary>
        /// Set custom warning text
        /// </summary>
        public static void SetWarningText(string text)
        {
            _warningText = text;
        }

        /// <summary>
        /// Check if warning is currently active
        /// </summary>
        public static bool IsWarningActive
        {
            get { return _isBlinking; }
        }
    }
}
