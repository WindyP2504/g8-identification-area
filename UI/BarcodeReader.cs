using System;
using System.Windows.Forms;
using VTP_Induction.Device;

namespace VTP_Induction.UI
{
    //public delegate void DataChangedNow();
    public partial class BarcodeReader : UserControl
    {
        private Globals.TBarcodeConfig cfg;
        public DataChangedNow dataChangedEvent;

        public BarcodeReader(Globals.TBarcodeConfig cfg, string s)
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

                this.comboBoxBaudrate.SelectedItem = cfg.sBaudrate;
                this.comboBoxParity.SelectedItem = cfg.sParity;
                this.comboBoxPort.SelectedItem = cfg.sPort;
                this.comboBoxStopBits.SelectedItem = cfg.sStopbits;
                this.comboBoxDatabits.SelectedItem = cfg.sDatabits;
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
                cfg.sBaudrate = this.comboBoxBaudrate.SelectedItem.ToString();
                cfg.sPort = this.comboBoxPort.SelectedItem.ToString();
                cfg.sParity = this.comboBoxParity.SelectedItem.ToString();
                cfg.sStopbits = this.comboBoxStopBits.SelectedItem.ToString();
                cfg.sDatabits = this.comboBoxDatabits.SelectedItem.ToString();


                return true;
            }
            catch //(System.Exception ex)
            {
                return false;
            }

        }
        public bool AddEventUpdateNewChange()
        {
            //try
            //{
            //    this.textBoxNameDev.TextChanged += new System.EventHandler(this.textBoxNameDev_TextChanged);
            //    this.textBoxPort.TextChanged += new System.EventHandler(this.textBoxNameDev_TextChanged);

            return true;
            //}
            //catch (Exception ex)
            //{
            //    Log.LogWrite(Globals.LogLv.Verbose, ex);
            //    return false;
            //}
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
