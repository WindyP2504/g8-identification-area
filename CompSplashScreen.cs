using DevExpress.XtraSplashScreen;
using System;

namespace VTP_Induction
{
    public partial class CompSplashScreen : SplashScreen
    {
        public CompSplashScreen()
        {
            InitializeComponent();
        }

        #region Overrides

        public override void ProcessCommand(Enum cmd, object arg)
        {
            switch ((SplashScreenCommand)cmd)
            {
                case SplashScreenCommand.SetLabel:
                    string sMsg = arg as string;
                    labelControl2.Text = sMsg;
                    break;

                case SplashScreenCommand.SetProgress:
                    marqueeProgressBarControl1.EditValue = arg;
                    break;
            }
            base.ProcessCommand(cmd, arg);
        }

        #endregion

        public enum SplashScreenCommand
        {
            SetLabel,
            SetProgress,
            SetTopMost,
        }
    }
}