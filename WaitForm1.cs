using DevExpress.XtraWaitForm;
using System;
using System.Windows.Forms;


namespace VTP_Induction
{
    public partial class WaitForm1 : WaitForm
    {
        public WaitForm1()
        {
            InitializeComponent();
            ReloadUserLanguage();
            this.progressPanel1.AutoHeight = true;
        }

        #region Overrides

        public override void SetCaption(string caption)
        {
            base.SetCaption(caption);
            this.progressPanel1.Caption = caption;
        }
        public override void SetDescription(string description)
        {
            base.SetDescription(description);
            this.progressPanel1.Description = description;
        }
        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
        }

        #endregion

        private void WaitForm1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.progressPanel1.Description = "Done";

        }

        private void ReloadUserLanguage()
        {
            progressPanel1.Caption = "Please wait";
            progressPanel1.Description = "Loading....";
        }

    }
}