using System;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using VTP_Induction.Common;

namespace VTP_Induction.Device
{
    public class CScale : CDevice
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
        public Globals.TScaleConfig m_ptDeviceConfig
        {
            get
            {
                return (Globals.TScaleConfig)GLb.g_tDevCfg.tScale.Clone();
            }
        }
        public Globals.TScaleConfig m_ptConfig
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

        public CScale(TForm Form)
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
        //private readonly SerialPort serialPort;
        private readonly StringBuilder _rxBuffer = new StringBuilder();
        private readonly object _rxLock = new object();

        public class CommConfig
        {
            public CommType commType = CommType.RS232;

            public string PortName = string.Empty;
            public int BaudRate = 115200;
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
        private bool isReading; // Biến để kiểm tra trạng thái đọc dữ liệu
        private string lastNetData = "";
        public void CommInit()
        {
            //CommConfig cfg = new CommConfig();

            cfg.commType = CommType.RS232;

            cfg.PortName = GLb.g_tDevCfg.tScale.sPort;
            cfg.BaudRate = int.Parse(GLb.g_tDevCfg.tScale.sBaudrate);
            cfg.DataBit = int.Parse(GLb.g_tDevCfg.tScale.sDatabits);
            if (GLb.g_tDevCfg.tScale.sParity == "None")
            {
                cfg.parity = Util.ParityConvert(0);
            }

            cfg.stopbit = Util.StopbitConvert(1);
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
                Disconnect(); // đảm bảo cổng được giải phóng

                CommInit();
                isReading = true;

                string sLog = "{ Scale: " + m_sName + "::Connect ";
                serialPort = new SerialPort(cfg.PortName, cfg.BaudRate, cfg.parity, cfg.DataBit, cfg.stopbit);
                serialPort.DataReceived += SerialPort_DataReceived;
                try
                {
                    if (!serialPort.IsOpen)
                    {
                        serialPort.Open();

                    }

                    // Có thể gán event nếu dùng
                    // serialPort.DataReceived += SerialPort_DataReceived;

                    bRet = true;
                }
                catch (Exception ex)
                {
                    Log.LogWrite(Globals.LogLv.Error, "Lỗi khi mở cổng ");
                    bRet = false;
                }

                p_bConnection = bRet;
                Log.LogWrite(Globals.LogLv.Information, sLog, bRet);
            }
            catch (Exception ex)
            {
                Log.LogWrite(Globals.LogLv.Error, "Connect failed: ");
                bRet = false;
            }

            return bRet;
        }


        public bool CheckScaleStatus()
        {
            bool bRet = serialPort.IsOpen;
            return bRet;
        }

        public void Disconnect()
        {
            string sLog = "{" + m_sName + "::Disconnect} ";


            try
            {
                if (serialPort != null)
                {
                    // serialPort.DataReceived -= SerialPort_DataReceived;
                    if (serialPort.IsOpen)
                    {
                        serialPort.DataReceived -= SerialPort_DataReceived;
                        serialPort.Close();
                        // Thread.Sleep(100); // chờ driver giải phóng
                    }

                    // serialPort.DataReceived -= SerialPort_DataReceived;
                    serialPort.Dispose(); // rất quan trọng để giải phóng cổng hoàn toàn
                    serialPort = null;
                }

                isReading = false;
            }
            catch (Exception ex)
            {
                Log.LogWrite(Globals.LogLv.Warning, "Lỗi khi disconnect");
            }

            Log.LogWrite(Globals.LogLv.Information, sLog);
        }
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!isReading)
            {
                return;
            }

            try
            {
                string chunk = serialPort.ReadExisting(); // không chặn, lấy hết buffer hiện có
                if (string.IsNullOrEmpty(chunk))
                {
                    return;
                }

                lock (_rxLock)
                {
                    _rxBuffer.Append(chunk);

                    string nl = serialPort.NewLine; // "\r\n" (đã đặt ở trên)
                    int idx;

                    // TÁCH các dòng hoàn chỉnh có đủ newline
                    while ((idx = _rxBuffer.ToString().IndexOf(nl, StringComparison.Ordinal)) >= 0)
                    {
                        string line = _rxBuffer.ToString(0, idx);
                        _rxBuffer.Remove(0, idx + nl.Length);

                        line = line.TrimEnd('\r', '\n');
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        // Lưu bản tin NET: gần nhất
                        if (Regex.IsMatch(line, @"\d"))
                        {
                            lastNetData = line;
                        }

                        // Cập nhật UI qua BeginInvoke để tránh cross-thread
                        //BeginInvoke(new Action(() =>
                        //{
                        //    AppendTextToTextbox(line);
                        //}));
                    }
                }
            }
            catch (Exception ex)
            {
                // Ghi log nếu cần, tránh throw ra ngoài
                //BeginInvoke(new Action(() =>
                //{
                //    AppendTextToTextbox("[ERR] " + ex.Message);
                //}));
            }
        }
        public string ReadScaleDataFormCom()
        {
            string sScale = "0";

            try
            {
                // Lấy dữ liệu thô cuối cùng
                string rawData = lastNetData;

                if (!string.IsNullOrEmpty(rawData))
                {
                    // Tìm số có cả phần thập phân (nếu có)
                    Match match = Regex.Match(rawData, @"\d+(\.\d+)?");

                    if (match.Success)
                    {
                        // Parse về double
                        double value = double.Parse(match.Value, System.Globalization.CultureInfo.InvariantCulture);

                        // Nếu dữ liệu gốc có "kg" thì đổi sang gram
                        if (rawData.IndexOf("kg", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            value = value * 1000;
                        }

                        // Nếu dữ liệu gốc có "g" thì giữ nguyên
                        // (Có thể bổ sung thêm check "mg" nếu cần)

                        sScale = ((int)Math.Round(value)).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                // Trả về 0 nếu có lỗi
                sScale = "0";
                // Có thể ghi log ở đây nếu muốn
            }

            return sScale;
        }

    }
}
