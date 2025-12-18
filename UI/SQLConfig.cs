using System;
using System.Windows.Forms;
using VTP_Induction.Device;

namespace VTP_Induction.UI
{
    public partial class SQLConfig : UserControl
    {
        private Globals.TSQLConfig cfg;
        public DataChangedNow dataChangedEvent;

        public SQLConfig(Globals.TSQLConfig g_cfg)
        {
            cfg = g_cfg;

            InitializeComponent();

            cboType.Items.Clear();
            cboType.Items.AddRange(Enum.GetNames(typeof(Globals.SQLType)));
        }

        public bool DataToUI()
        {
            try
            {
                txtConStr.Text = cfg.SqlString;
                cboType.SelectedItem = cfg.sqlType.ToString();
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
                cfg.SqlString = txtConStr.Text.Trim();
                cfg.sqlType = (Globals.SQLType)Enum.Parse(typeof(Globals.SQLType), cboType.Text.Trim());
                return true;
            }
            catch //(System.Exception ex)
            {
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
