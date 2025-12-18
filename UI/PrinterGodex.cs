using EzioDll;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;
using VTP_Induction.Device;

namespace VTP_Induction.UI
{
    public partial class PrinterGodex : UserControl
    {
        private Globals.TPrinterGodex cfg;
        public DataChangedNow dataChangedEvent;

        public PrinterGodex(Globals.TPrinterGodex g_cfg)
        {
            cfg = g_cfg;
            InitializeComponent();
        }

        public bool DataToUI()
        {
            try
            {
                switch (cfg.connectType)
                {
                    case Globals.TPrinterConnectType.USB:
                        rbUsb.Checked = true;
                        break;
                    case Globals.TPrinterConnectType.LAN:
                        rbLan.Checked = true;
                        break;
                    case Globals.TPrinterConnectType.DRIVER:
                        rbDriver.Checked = true;
                        break;
                    case Globals.TPrinterConnectType.COM:
                        rbCom.Checked = true;
                        break;
                    default:
                        // Nếu không có giá trị nào hợp lệ, bạn có thể thực hiện hành động khác như bỏ chọn tất cả hoặc đặt mặc định.
                        rbUsb.Checked = false;
                        rbLan.Checked = false;
                        rbDriver.Checked = false;
                        rbCom.Checked = false;
                        break;
                }
                txtBaud.Text = cfg.sBaud;
                txtPort.Text = cfg.sPort;
                cboCom.Text = cfg.sCom;
                cboDriver.Text = cfg.sDriver;
                cboUsb.Text = cfg.sUsbName;
                txtLanIp.Text = cfg.sLanIp;
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
                if (rbUsb.Checked)
                {
                    cfg.connectType = Globals.TPrinterConnectType.USB;
                }
                else if (rbLan.Checked)
                {
                    cfg.connectType = Globals.TPrinterConnectType.LAN;
                }
                else if (rbDriver.Checked)
                {
                    cfg.connectType = Globals.TPrinterConnectType.DRIVER;
                }
                else if (rbCom.Checked)
                {
                    cfg.connectType = Globals.TPrinterConnectType.COM;
                }

                cfg.sBaud = txtBaud.Text.Trim();
                cfg.sPort = txtPort.Text.Trim();
                cfg.sLanIp = txtLanIp.Text.Trim();
                cfg.sCom = cboCom.Text.Trim();
                cfg.sDriver = cboDriver.Text.Trim();
                cfg.sUsbName = cboUsb.Text.Trim();
                cfg.bActive = true;

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

        private void PrinterGodex_Load(object sender, EventArgs e)
        {
            // Find USB Port
            FindPrinter_USB();

            // Find Com Port
            string[] ComPrinter = SerialPort.GetPortNames();
            if (ComPrinter != null)
            {
                cboCom.Items.Clear();
                for (int i = 0; i < ComPrinter.Length; i++)
                {
                    cboCom.Items.Add(ComPrinter[i]);
                }

                if (cboCom.Items.Count > 0)
                {
                    cboCom.SelectedIndex = 0;
                }
            }

            // Find GoDEX Driver Printer
            string[] DriverPrinter = GodexPrinter.GetDriverPrinter();
            if (DriverPrinter != null)
            {
                cboDriver.Items.Clear();
                for (int i = 0; i < DriverPrinter.Length; i++)
                {
                    cboDriver.Items.Add(DriverPrinter[i]);
                }

                if (cboDriver.Items.Count > 0)
                {
                    cboDriver.SelectedIndex = 0;
                }
            }
        }

        private void FindPrinter_USB()
        {
            cboUsb.Items.Clear();
            List<string> PrinterList = GodexPrinter.GetPrinter_USB();
            for (int i = 0; i < PrinterList.Count; i++)
            {
                cboUsb.Items.Add(PrinterList[i]);
            }

            if (cboUsb.Items.Count > 0)
            {
                cboUsb.SelectedIndex = 0;
            }
        }
    }
}
