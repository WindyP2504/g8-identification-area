using System;
using System.Windows.Forms;
using VTP_Induction.Common;
using VTP_Induction.Device;

namespace VTP_Induction.UI
{
    public delegate void DataChangedNow();
    public partial class CameraConfig : UserControl
    {
        private Globals.TDetectorConfig cfg;
        public DataChangedNow dataChangedEvent;

        public CameraConfig(Globals.TDetectorConfig cfg, string s)
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
                this.textBoxID.Text = cfg.sID;
                this.textBoxCameraIP.Text = cfg.sCameraIp;
                this.textBoxCameraPort.Text = cfg.sCameraPort;
                this.CheckBoxUseBarcodeTCP.Checked = cfg.bUseBarcodeServer;
                this.textBoxExporSureTime.Text = cfg.sExporeSureTime;
                this.cboCamMode.SelectedItem = cfg.sCameraMode;

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
                cfg.sID = this.textBoxID.Text;
                cfg.sCameraIp = this.textBoxCameraIP.Text;
                cfg.sCameraPort = this.textBoxCameraPort.Text;
                cfg.bUseBarcodeServer = this.CheckBoxUseBarcodeTCP.Checked;
                cfg.sExporeSureTime = this.textBoxExporSureTime.Text;
                cfg.sCameraMode = this.cboCamMode.SelectedItem.ToString();
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
                this.textBoxNameDev.TextChanged += new System.EventHandler(this.textBoxNameDev_TextChanged);
                this.textBoxID.TextChanged += new System.EventHandler(this.textBoxNameDev_TextChanged);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogWrite(Globals.LogLv.Verbose, ex);
                return false;
            }
        }
        private void textBoxNameDev_TextChanged(object sender, EventArgs e)
        {
            if (dataChangedEvent != null)
            {
                dataChangedEvent();
            }
        }
    }
}
