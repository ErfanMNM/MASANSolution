using MASAN_SERIALIZATION.Production;
using MASAN_SERIALIZATION.Utils;
using SpT.Static;
using Sunny.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MASAN_SERIALIZATION.Production.ProductionOrder;

namespace MASAN_SERIALIZATION.Views.SCADA
{
    public partial class PStatictis : UIPage
    {
        Dictionary<PropertyInfo, panelS> bindings = new Dictionary<PropertyInfo, panelS>();
        Dictionary<string, panelS> extraBindings = new Dictionary<string, panelS>();


        private BackgroundWorker WK_Update = new BackgroundWorker()
        {
            WorkerSupportsCancellation = true
        };
        public PStatictis()
        {
            InitializeComponent();
        }

        #region sự kiện page

        public void INIT()
        {
            Render_MEM();
            WK_Update.DoWork += WK_Update_DoWork;
            
        }

        

        private void PStatictis_Initialize(object sender, EventArgs e)
        {
            if (!WK_Update.IsBusy)
            {
                WK_Update.RunWorkerAsync();
            }
        }

        private void PStatictis_Finalize(object sender, EventArgs e)
        {
            WK_Update.CancelAsync();
        }

        #endregion

        #region Các hàm khởi tạo

        private void Render_MEM()
        {
            var dicCountUc = new panelS() { LabelName = "Dictionary Count" };
            opMEMFlow.Controls.Add(dicCountUc);
            extraBindings["Dictionary Count"] = dicCountUc;

            Type type = typeof(Product_Counter);
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                panelS uc = new panelS() { LabelName = prop.Name };
                opMEMFlow.Controls.Add(uc);
                bindings[prop] = uc;
            }
        }

        #endregion

        #region Các luồng và hàm của nó

        private void WK_Update_DoWork(object sender, DoWorkEventArgs e)
        {
            while(!WK_Update.CancellationPending)
            {
                try
                {
                    this.InvokeIfRequired(() =>
                    {
                        Update_BindingProps();
                        Update_ExtraProp();
                    });
                    
                }
                catch (Exception ex)
                {

                    this.ShowErrorNotifier($"Lỗi S123 cập nhật dữ liệu: {ex.Message}");
                }

                Thread.Sleep(500); // Cập nhật mỗi 0.5 giây
            }
        }

        private void Update_ExtraProp()
        {
            foreach (var kv in extraBindings)
            {
                var key = kv.Key;
                var uc = kv.Value;

                if (key == "Dictionary Count")
                {
                    uc.LabelValue = Globals_Database.Dictionary_ProductionCode_Data?.Count.ToString() ?? "0";
                }
            }
        }

        private void Update_BindingProps()
        {
            foreach (var kv in bindings)
            {
                var prop = kv.Key;
                var uc = kv.Value;
                if (Globals.ProductionData.counter != null)
                {
                    var value = prop.GetValue(Globals.ProductionData.counter);
                    uc.LabelValue = value?.ToString() ?? "0";
                }
            }
        }

        #endregion
    }
}
