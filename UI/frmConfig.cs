using DevExpress.XtraSplashScreen;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using VTP_Induction.Common;
using VTP_Induction.Device;

namespace VTP_Induction.UI
{
    public partial class frmConfig : UserControl
    {
        private Globals GLb = Globals.getInstance();
        private string sLogMain = "frmConfig: ";
        private const int TIMEOUT = 5000;/* NOTE : */
        private bool m_bClosePermission = true;

        private string m_sPassword = string.Empty;

        private BindingList<DataAxisConfig> recordsDataAxisConfig = new BindingList<DataAxisConfig>();
        private BindingList<DataJigConfig> recordsDataJigConfig = new BindingList<DataJigConfig>();
        private BindingList<DataDetectorPosConfig> recordsDataDetectorPosConfig = new BindingList<DataDetectorPosConfig>();
        private TForm parentForm;

        public frmConfig(TForm form)
        {
            parentForm = form;
            InitializeComponent();
            //m_sPassword = GLb.g_tSysCfg.Password;

            InitDeviceCfgs();
            DataToUI();
            GLb.g_SoftwareNameVersion = textBoxSoftwareName.Text;
        }
        public void InitDeviceCfgs()
        {
            if (true)
            {
                foreach (Globals.TDetectorConfig cfg in GLb.g_tDevCfg.tDetectorList)
                {
                    CameraConfig rp = new CameraConfig(cfg, cfg.sDevGroup);
                    if (rp != null)
                    {
                        rp.dataChangedEvent += new DataChangedNow(dataChangedHandler);
                        flowLayoutPanelDetector.Controls.Add(rp);
                    }
                }

                Globals.TScaleConfig Scalecfg = GLb.g_tDevCfg.tScale;
                ScaleRS232 rpScale = new ScaleRS232(Scalecfg, Scalecfg.sDevGroup);
                if (rpScale != null)
                {
                    rpScale.dataChangedEvent += new DataChangedNow(dataChangedHandler);

                    flowLayoutPanelDetector.Controls.Add(rpScale);
                }

                Globals.TBarcodeConfig BarcodeCfg = GLb.g_tDevCfg.tBarcode;
                BarcodeReader rpBarcode = new BarcodeReader(BarcodeCfg, BarcodeCfg.sDevGroup);
                if (rpBarcode != null)
                {
                    rpBarcode.dataChangedEvent += new DataChangedNow(dataChangedHandler);

                    flowLayoutPanelDetector.Controls.Add(rpBarcode);
                }

                Globals.TPrinterGodex printerGodex = GLb.g_tDevCfg.tPrinterGodex;
                PrinterGodex rpPrinterGodex = new PrinterGodex(printerGodex);
                if (rpPrinterGodex != null)
                {
                    rpPrinterGodex.dataChangedEvent += new DataChangedNow(dataChangedHandler);

                    flowLayoutPanelDetector.Controls.Add(rpPrinterGodex);
                }

                //Globals.TWMSConfig WMScfg = GLb.g_tDevCfg.tWMS;
                //Globals.RFIDConfig RFIDcfg = GLb.g_tDevCfg.tRFID;

                //APIwms rpWMS = new APIwms(WMScfg, WMScfg.sDevGroup);

                //if (rpWMS != null)
                //{
                //    rpWMS.dataChangedEvent += new DataChangedNow(dataChangedHandler);

                //    flowLayoutPanelWMS.Controls.Add(rpWMS);
                //}

                //APIrfid rpRFID = new APIrfid(RFIDcfg, RFIDcfg.sDevGroup);

                //if (rpWMS != null)
                //{
                //    rpWMS.dataChangedEvent += new DataChangedNow(dataChangedHandler);

                //    flowLayoutPanelWMS.Controls.Add(rpWMS);
                //}

                //if (rpRFID != null)
                //{
                //    rpRFID.dataChangedEvent += new DataChangedNow(dataChangedHandler);

                //    flowLayoutPanelWMS.Controls.Add(rpRFID);
                //}

                Globals.TSQLConfig tSQL = GLb.g_tSQLConfig;
                SQLConfig rpSQLConfig = new SQLConfig(tSQL);
                if (rpSQLConfig != null)
                {
                    rpSQLConfig.dataChangedEvent += new DataChangedNow(dataChangedHandler);
                    flowLayoutPanelDB.Controls.Add(rpSQLConfig);
                }
            }
            else
            {

            }
        }
        private bool bInitDone = false;
        private void dataChangedHandler()
        {
            if (bInitDone)
            {
                this.buttonRefresh.Appearance.BackColor = System.Drawing.Color.OrangeRed;
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            //if (buttonRefresh.BackColor == Color.Tomato)
            DialogResult dlg = MessageBox.Show("Do you want to reset all of the connections", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dlg == DialogResult.OK)
            {
                buttonRefresh.Enabled = false;
                GLb.SaveAllData(true);

                //Refresh all device
                parentForm.devHandler.StopAllDev(true);
                parentForm.devHandler.StartAllDev(true);
                parentForm.devHandler.DevConnect(true);

                this.buttonRefresh.Appearance.BackColor = System.Drawing.Color.DodgerBlue;
                buttonRefresh.Enabled = true;
            }
        }
        private bool DataToUI()
        {
            bool bRet = false;
            try
            {

                foreach (Control ctrl in flowLayoutPanelDetector.Controls)
                {
                    ScaleRS232 rpScale = ctrl as ScaleRS232;
                    if (rpScale != null)
                    {
                        rpScale.DataToUI();
                    }
                    else
                    {
                        BarcodeReader rpBarcode = ctrl as BarcodeReader;
                        if (rpBarcode != null)
                        {
                            rpBarcode.DataToUI();
                        }
                        else
                        {
                            PrinterGodex rpPrinterGodex = ctrl as PrinterGodex;
                            if (rpPrinterGodex != null)
                            {
                                rpPrinterGodex.DataToUI();
                            }
                        }
                    }

                }

                //foreach (var control in flowLayoutPanelWMS.Controls)
                //{
                //    APIwms apiwmsControl = control as APIwms;
                //    if (apiwmsControl != null)
                //    {
                //        apiwmsControl.DataToUI();
                //    }
                //}
                //foreach (var control in flowLayoutPanelWMS.Controls)
                //{
                //    APIrfid apiRFIDControl = control as APIrfid;
                //    if (apiRFIDControl != null)
                //    {
                //        apiRFIDControl.DataToUI();
                //    }
                //}

                foreach (var control in flowLayoutPanelDB.Controls)
                {
                    SQLConfig sQLConfig = control as SQLConfig;
                    if (sQLConfig != null)
                    {
                        sQLConfig.DataToUI();
                    }
                }

                txbWeightStandard.Text = GLb.g_tSysCfg.nWeightStandard.ToString();
                txtScaleError.Text = GLb.g_tSysCfg.nScaleError.ToString();
                txtTimeScale.Text = GLb.g_tSysCfg.nTimeScale.ToString();
                textBoxSoftwareName.Text = GLb.g_tSysCfg.sSoftwareName;
                txtPalletNumber.Text = GLb.g_tSysCfg.nPalletNumber.ToString();
                txtParcelNumber.Text = GLb.g_tSysCfg.nParcelNumber.ToString();
                txtParcelType.Text = GLb.g_tSysCfg.sParcelType;
                checkBoxPrintPallet.Checked = GLb.g_tSysCfg.bPrintPallet;

                txtPos1.Text = GLb.g_tSysCfg.sPositions[0].ToString();
                txtPos2.Text = GLb.g_tSysCfg.sPositions[1].ToString();
                txtPos3.Text = GLb.g_tSysCfg.sPositions[2].ToString();
                txtPos4.Text = GLb.g_tSysCfg.sPositions[3].ToString();
                txtPos5.Text = GLb.g_tSysCfg.sPositions[4].ToString();
                txtPos6.Text = GLb.g_tSysCfg.sPositions[5].ToString();

                bRet = true;

            }
            catch (Exception ex)
            {
                Log.LogWrite(Globals.LogLv.Verbose, ex);
                MessageBox.Show("Config data is broken!");
            }
            return bRet;
        }
        private bool UIToData()
        {
            bool bRet = false;
            try
            {
                //foreach (CameraConfig rp in flowLayoutPanelDetector.Controls)
                //{
                //    rp.UIToData();
                //}
                foreach (Control ctrl in flowLayoutPanelDetector.Controls)
                {
                    ScaleRS232 rpScale = ctrl as ScaleRS232;
                    if (rpScale != null)
                    {
                        rpScale.UIToData();
                    }
                    else
                    {
                        BarcodeReader rpBarcode = ctrl as BarcodeReader;
                        if (rpBarcode != null)
                        {
                            rpBarcode.UIToData();
                        }
                        else
                        {
                            PrinterGodex rpPrinterGodex = ctrl as PrinterGodex;
                            if (rpPrinterGodex != null)
                            {
                                rpPrinterGodex.UIToData();
                            }
                        }
                    }
                }

                //foreach (var control in flowLayoutPanelWMS.Controls)
                //{
                //    APIwms apiwmsControl = control as APIwms;
                //    if (apiwmsControl != null)
                //    {
                //        apiwmsControl.UIToData();
                //    }
                //}
                //foreach (var control in flowLayoutPanelWMS.Controls)
                //{
                //    APIrfid apiRFIDControl = control as APIrfid;
                //    if (apiRFIDControl != null)
                //    {
                //        apiRFIDControl.UIToData();
                //    }
                //}

                foreach (var control in flowLayoutPanelDB.Controls)
                {
                    SQLConfig sQLConfig = control as SQLConfig;
                    if (sQLConfig != null)
                    {
                        sQLConfig.UIToData();
                    }
                }

                //GLb.g_tSysCfg.n_MultipleCamera = int.Parse(txtScaleValue.Text);
                GLb.g_tSysCfg.nWeightStandard = int.Parse(txbWeightStandard.Text);
                GLb.g_tSysCfg.nScaleError = int.Parse(txtScaleError.Text);
                GLb.g_tSysCfg.nTimeScale = int.Parse(txtTimeScale.Text);
                GLb.g_tSysCfg.sSoftwareName = textBoxSoftwareName.Text;
                GLb.g_tSysCfg.nParcelNumber = int.Parse(txtParcelNumber.Text);
                GLb.g_tSysCfg.nPalletNumber = int.Parse(txtPalletNumber.Text);
                GLb.g_tSysCfg.sParcelType = txtParcelType.Text;
                GLb.g_tSysCfg.bPrintPallet = checkBoxPrintPallet.Checked;

                GLb.g_tSysCfg.sPositions[0] = txtPos1.Text;
                GLb.g_tSysCfg.sPositions[1] = txtPos2.Text;
                GLb.g_tSysCfg.sPositions[2] = txtPos3.Text;
                GLb.g_tSysCfg.sPositions[3] = txtPos4.Text;
                GLb.g_tSysCfg.sPositions[4] = txtPos5.Text;
                GLb.g_tSysCfg.sPositions[5] = txtPos6.Text;

                bRet = true;
            }
            catch (Exception ex)
            {
                Log.LogWrite(Globals.LogLv.Verbose, ex);
                MessageBox.Show("Correct all input data first!!! ");
                bRet = false;
            }
            return bRet;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            SplashScreenManager.ShowForm(null, typeof(WaitForm1), true, true, false);

            string sLog = sLogMain + "OK button is clicked.";

            try
            {
                if (UIToData())
                {
                    GLb.g_SoftwareNameVersion = textBoxSoftwareName.Text;

                    GLb.SaveAllData(true);
                    //Refresh all device
                    parentForm.devHandler.StopAllDev(false);
                    parentForm.devHandler.StartAllDev(false);
                    parentForm.devHandler.DevConnect(false);
                    parentForm.InitAllPanelDevDiagnostics();

                    //parentForm.SetPermission(GLb.currentUser);
                    this.buttonRefresh.Appearance.BackColor = System.Drawing.Color.DodgerBlue;

                    // ChangeButtonDisp(false);
                }
            }
            catch (Exception ex)
            {
                Log.LogWrite(Globals.LogLv.Verbose, ex);
            }
            finally
            {
                Thread.Sleep(500);
                SplashScreenManager.CloseForm(false);
            }
        }

        private void buttonPrintManual_Click(object sender, EventArgs e)
        {
            bool bRet = parentForm.devHandler.cPrinter.PrintBarcode(txtPrintManual.Text, "", "", "", "");
            if (bRet)
            {
                MessageBox.Show("IN THÀNH CÔNG " + txtPrintManual.Text);
            }
            else
            {
                MessageBox.Show("IN LỖI " + txtPrintManual.Text);
            }
        }
    }
}
