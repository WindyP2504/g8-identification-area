using System;
using System.Windows.Forms;
using VTP_Induction.Common;
using VTP_Induction.Device;

namespace VTP_Induction.UI
{
    public partial class panelDeviceConfig : UserControl
    {
        private Globals GLb = Globals.getInstance();
        private TForm parentForm;
        private int nDeviceID = 0;
        public Globals.TDevUtilDeviceConfig m_ptDevConfig;
        private int nIndexDev = -1;
        public panelDeviceConfig()
        {
            InitializeComponent();
        }
        private void EnabledUI(bool bActive)
        {
            comboBoxPortDev.Enabled = bActive;
            comboBoxBaudDev.Enabled = bActive;
            comboBoxParityDev.Enabled = bActive;
            comboBoxDataBitsDev.Enabled = bActive;
            comboBoxStopBitsDev.Enabled = bActive;
            propertyGridPower.Enabled = bActive;
            textBoxIP_Client.Enabled = bActive;
            textBoxPort_Client.Enabled = bActive;
            buttonSetup.Enabled = bActive;
            textBoxIP_Server.Enabled = bActive;
            textBoxPort_Server.Enabled = bActive;
        }
        public bool UIToData()
        {
            bool bRet = false;
            m_ptDevConfig = GLb.g_tDevCfg.tDeviceList[nIndexDev];
            propertyGridPower.SelectedObject = m_ptDevConfig;

            m_ptDevConfig.tCommCfg.sPort = Util.GetComboxString(this.comboBoxPortDev);
            m_ptDevConfig.tCommCfg.nBaud = Convert.ToInt32(comboBoxBaudDev.SelectedItem.ToString());
            m_ptDevConfig.tCommCfg.nParity = comboBoxParityDev.SelectedIndex;
            m_ptDevConfig.tCommCfg.nDataBits = Convert.ToInt32(comboBoxDataBitsDev.SelectedItem.ToString());
            m_ptDevConfig.tCommCfg.nStopBits = comboBoxStopBitsDev.SelectedIndex;
            //m_ptDevConfig.tCommCfg.isRemoteMode = this.checkBoxRemoteMode.Checked;

            m_ptDevConfig.tCommCfg.sIP_Client = this.textBoxIP_Client.Text;
            m_ptDevConfig.tCommCfg.nPort_Client = Convert.ToInt32(this.textBoxPort_Client.Text);
            m_ptDevConfig.tCommCfg.nPort_Server = Convert.ToInt32(this.textBoxPort_Server.Text);
            m_ptDevConfig.tCommCfg.sIP_Server = this.textBoxIP_Server.Text;

            //m_ptDevConfig.tCommCfg.sIP_Client_UDP = this.textBoxIP_Client_UDP.Text;
            //m_ptDevConfig.tCommCfg.udpSendPort = Convert.ToInt32(this.textBoxPort_ClientUDP_Send.Text);
            //m_ptDevConfig.tCommCfg.udpRecvPort = Convert.ToInt32(this.textBoxPort_ClientUDP_Rcv.Text);

            bRet = true;
            return bRet;
        }
        private void simpleButtonConnect_Click(object sender, EventArgs e)
        {
            CDevice dt = parentForm.devHandler.GetDeviceHandler(m_ptDevConfig.nLCISDeviceID);

            if (dt != null)
            {
                if (simpleButtonConnect.Text == "&Connect")
                {
                    // Update ui to database
                    try
                    {
                        UIToData();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    bool bTemp = false;
                    if (m_ptDevConfig.bActive)
                    {
                        try
                        {
                            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(null, typeof(WaitForm1), true, true, false);
                            bTemp = dt.Connect();
                        }
                        catch (Exception ex)
                        {
                            Log.LogWrite(Globals.LogLv.Verbose, ex);
                            Log.LogWrite(Globals.LogLv.Debug, ex.Message);
                        }
                        finally
                        {
                            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                        }

                        //Connect
                        if (!bTemp)
                        {
                            // FAIL
                            string msg = string.Empty;
                            if (dt.m_bConnection)
                            {
                                msg += "Serial port open successfully, but detector can not remote.";
                            }
                            else
                            {
                                msg += "Can not open serial port. Check again!";
                            }

                            MessageBox.Show(msg);
                        }
                        else
                        {
                            //Success
                            //Disable all control

                            simpleButtonConnect.Text = "&Disconnect";
                            EnabledUI(false);
                        }
                    }
                }
                else
                {
                    dt.Disconnect();
                    simpleButtonConnect.Text = "&Connect";
                    EnabledUI(true);
                }
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

        }
    }
}
