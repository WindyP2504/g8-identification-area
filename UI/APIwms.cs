using System;
using System.Windows.Forms;
using VTP_Induction.Common;
using VTP_Induction.Device;

namespace VTP_Induction.UI
{
    public partial class APIwms : UserControl
    {
        private Globals.TWMSConfig cfg;
        public DataChangedNow dataChangedEvent;
        public APIwms(Globals.TWMSConfig cfg, string s)
        {
            this.cfg = cfg;
            InitializeComponent();
            groupBoxDev.Text = s;

        }

        public bool DataToUI()
        {
            try
            {
                this.textBoxNameDev.Text = cfg.sDevName;
                this.checkBoxUseDev.Checked = cfg.bActive;
                this.txtUrl.Text = cfg.URL;

                return true;
            }
            catch //(System.Exception ex)
            {
                return false;
            }
        }
        public bool UIToData()
        {
            try
            {
                cfg.sDevName = this.textBoxNameDev.Text;
                cfg.bActive = this.checkBoxUseDev.Checked;
                cfg.URL = this.txtUrl.Text;
                return true;
            }
            catch //(System.Exception ex)
            {
                return false;
            }

        }
        public bool AddEventUpdateNewChange()
        {
            try
            {
                this.textBoxNameDev.TextChanged += new System.EventHandler(this.textBoxNameDev_TextChanged_1);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogWrite(Globals.LogLv.Verbose, ex);
                return false;
            }
        }

        private void textBoxNameDev_TextChanged_1(object sender, EventArgs e)
        {
            if (dataChangedEvent != null)
            {
                dataChangedEvent();
            }
        }
    }
}
