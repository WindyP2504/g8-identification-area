using System;
using System.IO.Ports;
using System.Threading;
using VTP_Induction.Common;

namespace VTP_Induction.Device
{
    public class CBarcode : CDevice
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
        public Globals.TBarcodeConfig m_ptDeviceConfig
        {
            get
            {
                return (Globals.TBarcodeConfig)GLb.g_tDevCfg.tBarcode.Clone();
            }
        }
        public Globals.TBarcodeConfig m_ptConfig
        {
            get
            {
                return m_ptDeviceConfig;
            }
        }
        //public Globals.TCommConfig m_ptCommCfg
        //{
        //    get
        //    {
        //        return m_ptConfig.tCommCfg;
        //    }
        //}

        public CBarcode(TForm Form)
        {
            m_Form = Form;
            this.p_sNameDevice = m_ptConfig.sDevName;
        }
        //TSCSDK.driver Printer = new TSCSDK.driver();
        public SerialPort serialPort;
        public enum CommType
        {
            RS232,
            TCPIP,
            UDP,
            USB
        }

        public class CommConfig
        {
            public CommType commType = CommType.RS232;

            public string PortName = string.Empty;
            public int BaudRate = 9600;
            public int DataBit = 8;
            public Parity parity = Parity.None;
            public StopBits stopbit = StopBits.One;

            public string IPAddress = "127.0.0.1";
            public int IPPort = 8888;

            public string udpRecvPort = "8888";
            public string udpSendPort = "8887";

            public int sendBufferSize = 4096;

            public bool isRTSOn = false;
            public bool isDTROn = false;

            public bool isRemoteMode = false;

            public CommConfig()
            {

            }

            public string[] GetAvailablePort()
            {
                return SerialPort.GetPortNames();
            }

            public string GetName()
            {
                if (isRemoteMode)
                {
                    return new Random().Next(100000).ToString();
                }

                if (commType == CommType.RS232)
                {
                    return PortName;
                }
                else if (commType == CommType.TCPIP)
                {
                    return IPAddress.Replace('.', '_') + "_" + IPPort.ToString("0000");
                }
                else if (commType == CommType.UDP)
                {
                    return IPAddress + ":" + udpRecvPort + ":" + udpSendPort;
                }
                else if (commType == CommType.USB)
                {
                    return "USB";
                }
                else
                {
                    return string.Empty;
                }
            }

            public override string ToString()
            {
                if (commType == CommType.RS232)
                {
                    return PortName;
                }
                if (commType == CommType.TCPIP)
                {
                    return IPAddress + "_" + IPPort;
                }
                else
                {
                    return (new Random()).Next(10000).ToString();
                }
            }
        }
        CommConfig cfg = new CommConfig();

        public void CommInit()
        {
            //CommConfig cfg = new CommConfig();

            cfg.commType = CommType.RS232;

            cfg.PortName = GLb.g_tDevCfg.tBarcode.sPort;
            cfg.BaudRate = int.Parse(GLb.g_tDevCfg.tBarcode.sBaudrate);
            cfg.DataBit = int.Parse(GLb.g_tDevCfg.tBarcode.sDatabits);
            if (GLb.g_tDevCfg.tBarcode.sParity == "None")
            {
                cfg.parity = Util.ParityConvert(0);
            }

            cfg.stopbit = Util.StopbitConvert(0);
            //cfg.isRemoteMode = GLb.g_tDevCfg.tRS232Barcode[Index].isRemoteMode;

            CommDeInit();
        }
        public void CommDeInit()
        {

        }

        public bool Connect()
        {
            bool bRet = false;
            try
            {
                if(p_bConnection)
                    Disconnect();
                CommInit();
                string sLog = "{ Barcode: " + m_sName + "::Connect ";
                //serialPort.DataReceived += SerialPort_DataReceived;
                serialPort = new SerialPort(cfg.PortName, cfg.BaudRate, cfg.parity, cfg.DataBit, cfg.stopbit);
                serialPort.ReadTimeout = 10000;
                try
                {
                    serialPort.DataReceived += SerialPort_DataReceived;
                    serialPort.Open();
                    bRet = true;
                }
                catch
                {
                    bRet = false;
                }
                if (!bRet)
                {
                    return bRet;
                }
                p_bConnection = bRet;
                Log.LogWrite(Globals.LogLv.Information, sLog, bRet);
            }
            catch
            {
                bRet = false;
            }
            return bRet;

        }
        private AutoResetEvent receiveEvent = new AutoResetEvent(false);
        private string receivedData = "";
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                receivedData = serialPort.ReadExisting().Trim();
                receiveEvent.Set(); // báo đã nhận
            }
            catch { }
        }
        public void Disconnect()
        {
            string sLog = "{" + m_sName + "::Disconnect} ";
            try
            {
                if (serialPort.IsOpen)
                    serialPort.Close();
            }
            catch
            {
                throw;
            }
            Log.LogWrite(Globals.LogLv.Information, sLog);
        }
        private void TriggerOn()
        {
            byte[] triggerOn = { 0x16, 0x54, 0x0D };  // \x16T\r
            serialPort.Write(triggerOn, 0, triggerOn.Length);
            //Console.WriteLine("🔴 Trigger ON");
        }

        private void TriggerOff()
        {
            byte[] triggerOff = { 0x16, 0x55, 0x0D };  // \x16U\r
            serialPort.Write(triggerOff, 0, triggerOff.Length);
            //Console.WriteLine("⚪ Trigger OFF");
        }

        public string ReadBarcoder(int timeoutMs = 90000)
        {
            try
            {
                receivedData = "";
                TriggerOn();

                if (receiveEvent.WaitOne(timeoutMs)) // chờ trong 15 giây
                {
                    TriggerOff();
                    return receivedData;
                }
                else
                {
                    TriggerOff();
                    return "";
                }
            }
            catch //(Exception ex)
            {
                TriggerOff();
                return "";
            }
        }
    }

}
