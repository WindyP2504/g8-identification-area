using System.Drawing;
using VTP_Induction.Common;

namespace VTP_Induction.Device
{
    public class CPrinter : CDevice
    {
        public Globals GLb = Globals.getInstance();

        protected TForm m_Form;
        public string m_sName
        {
            get
            {
                return p_sNameDevice;
            }
        }
        protected string p_sNameDevice = string.Empty;

        protected bool p_bConnection = false;

        public virtual bool m_bConnection
        {
            get
            {
                if (GLb.g_tSysCfg.bDemoMode)
                {
                    return true;
                }

                return p_bConnection;
            }
        }
        public Globals.TPrinterConfig m_ptDeviceConfig
        {
            get
            {
                return (Globals.TPrinterConfig)GLb.g_tDevCfg.tPRINTER.Clone();
            }
        }
        public Globals.TDevUtilDeviceConfig m_ptConfig
        {
            get
            {
                return m_ptDeviceConfig;
            }
        }
        public Globals.TCommConfig m_ptCommCfg
        {
            get
            {
                return m_ptConfig.tCommCfg;
            }
        }

        public CPrinter(TForm Form)
        {
            m_Form = Form;
            this.p_sNameDevice = m_ptCommCfg.sDevName;
        }
        TSCSDK.driver Printer = new TSCSDK.driver();

        public bool Connect()
        {
            Disconnect();

            bool bRet = false;
            string sLog = "{ Printer " + m_sName + "::Connect ";

            bRet = Printer.openport("TSC PEX-2360R");


            if (!bRet)
            {
                return bRet;
            }

            p_bConnection = bRet;
            Log.LogWrite(Globals.LogLv.Information, sLog, bRet);
            return bRet;


        }
        public void Disconnect()
        {
            string sLog = "{" + m_sName + "::Disconnect} ";
            Log.LogWrite(Globals.LogLv.Information, sLog);

        }

        public bool PrintBarcode(string TextString)
        {
            bool bRet = false;
            try
            {

                bRet = Printer.openport("TSC PEX-2360R");

                string WT1 = "TTKT1-HNI-NLN";
                string B1 = "TextString";
                byte[] result_utf8 = System.Text.Encoding.UTF8.GetBytes("TEXT 40,620,\"ARIAL.TTF\",0,12,12,\"utf8 test Wörter auf Deutsch\"");

                //driver.about();
                //bRet = Printer.openport("TSC PEX-2360R");
                bRet = Printer.sendcommand("SIZE 4,6");
                bRet = Printer.sendcommand("SPEED 5");
                bRet = Printer.sendcommand("BACK SPEED 4");
                bRet = Printer.sendcommand("DENSITY 12");
                bRet = Printer.sendcommand("DIRECTION 1");
                bRet = Printer.sendcommand("SET TEAR ON");
                bRet = Printer.sendcommand("CODEPAGE UTF-8");
                Printer.clearbuffer();
                //driver.downloadpcx("UL.PCX", "UL.PCX");
                //Printer.windowsfont(40, 490, 48, 0, 0, 0, "Arial", "Windows Font Test");
                //Printer.windowsfontunicode(40, 550, 48, 0, 0, 0, "Arial", "Windows Unicode Test");
                //bRet = Printer.sendcommand("PUTBMP 50,50,\"D:\\Test.bmp\"");
                //bRet = Printer.sendcommand("DRAWLINE 40,40,400,3"); // Vẽ đường ngang, tọa độ (40,40) dài 400px, dày 3px
                //bRet = Printer.sendcommand("DRAWLINE 40,40,3,400"); // Vẽ đường dọc, tọa độ (40,40) cao 400px, dày 3px
                //bRet = Printer.sendcommand("PUTPCX 40,40,\"UL.PCX\"");

                Bitmap temp = new Bitmap("D:\\Test.bmp");
                //Bitmap temp = new Bitmap("D:\\Test.bmp");
                Printer.send_bitmap(0, 0, temp);
                //Printer.send_bitmap(0, 0, temp);
                //driver.sendcommand(result_utf8);
                Printer.barcode("100", "250", "128", "250", "2", "0", "6", "6", B1);
                Printer.printerfont("150", "710", "1", "0", "7", "7", WT1);
                //Printer.barcode("900", "1030", "QRCODE", "250", "2", "0", "6", "6", B1);
                Printer.sendcommand("QRCODE 850,1030,L,12,A,0,\"VTP2092000\"");
                Printer.printlabel("1", "1");




                Printer.closeport();
            }
            catch
            {
                bRet = false;
            }
            return bRet;
        }

    }
}
