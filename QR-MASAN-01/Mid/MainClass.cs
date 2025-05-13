using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainClass
{
    public class MFI_Info
    {
        public string Case_Barcode { get; set; } = "Barcode";
        public string Product_Barcode { get; set; } = "0";
        public string Case_LOT { get; set; } = "16122024";
        public string Batch_Code { get; set; }
        public string Block_Size { get; set; }
        public string Case_Size { get; set; }
        public string Pallet_Size { get; set; }
        public string SanLuong { get; set; }
        public string Operator { get; set; }
        public string Pallet_QR_Type { get; set; }
        public string MFI_ID { get; set; }
        public string QRCode_Folder { get; set; }
        public string QRCode_FileName { get; set; }
        public e_MFI_Status MFI_Status { get; set; } = e_MFI_Status.STARTUP;
    }


    public enum e_MFI_Status
    {
        STARTUP,
        SQLite_LOAD,
        SQLite_SAVE,
        PUSHSERVER,
        FREE
    }
}
