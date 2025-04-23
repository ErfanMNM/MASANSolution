namespace SPMS1
{
    partial class OmronPLC_Hsl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.WK_Update = new System.ComponentModel.BackgroundWorker();
            // 
            // WK_Update
            // 
            this.WK_Update.WorkerReportsProgress = true;
            this.WK_Update.WorkerSupportsCancellation = true;
            this.WK_Update.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WK_Update_DoWork);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker WK_Update;
    }
}
