using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace VTP_Induction.Device
{
    public class Globals
    {
        public static Globals instance;
        public TForm Tform;

        public static Globals getInstance()
        {
            if (instance == null)
            {
                instance = new Globals();
            }

            return instance;
        }

        public TBootUp g_tSysCfg;
        public TAxisIO[] g_tInputCfg1;
        public TDevConfig g_tDevCfg;
        public TAxisIO[] g_tOutputCfg1;

        //System Variable
        public const int MAX_DETECTOR = 20;
        public int parcelCount = 0;

        // Token: 0x04000002 RID: 2
        public int parcelNoReadCount = 0;

        // Token: 0x04000003 RID: 3
        public double hourCount = 0.0;
        public Socket SocketClient;

        // Token: 0x04000005 RID: 5
        public bool isSocketBusy = false;

        // Token: 0x04000006 RID: 6
        public StringBuilder sb;

        // Token: 0x04000007 RID: 7
        // private SettingForm settingForm = new SettingForm();

        // Token: 0x04000008 RID: 8
        public System.Windows.Forms.Timer timer;

        // Token: 0x04000009 RID: 9
        public TcpClient tcpclnt = new TcpClient();

        public static string g_sHomeDir =
            System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)
            + "\\";
        public static string g_sBootUpDir = g_sHomeDir + "BootUp\\";
        public static string g_sCmdListDir = g_sHomeDir + "CmdList\\";
        public static string g_sParserStrDir = g_sHomeDir + "CmdList\\SystemPara\\";
        public static string g_sOptionStrFile = g_sHomeDir + "CmdList\\SystemPara\\OPTION.opt";
        public static string g_sExPrStrFile = g_sHomeDir + "CmdList\\SystemPara\\EXPARAMETER.expr";
        public static string g_sModelDir = g_sHomeDir + "Model\\";
        public static string g_sSeqDir = g_sHomeDir + "Seq\\";
        public static string g_sAOIsDir = g_sHomeDir + "Aois\\";
        public static string g_sLanguageDir = g_sHomeDir + "Language\\";

        public bool g_bGrabbing = false;

        //Writter: Phong
        public static string g_sCamConfigDir = g_sHomeDir + "CamConfig\\";
        public CameraConfig[] g_tCamConfig;
        //

        public string g_SoftwareNameVersion = " G8 Home WCS Ver 01";
        public string g_sLogDirData = "D:\\G8Home\\DATA\\";
        public static string g_sLogDir = "D:\\G8Home\\Log\\";
        public static string g_sLogDirMaster = "D:\\G8Home\\Log(Master)\\";
        public static string g_sLogDirSlave = "D:\\G8Home\\Log(Slave)\\";
        public string g_LogTempImageSavetoRam = "R:\\Temp\\";
        public string g_LogTempRamFolder = "R:\\Temp";
        public string g_filePathRam = "";
        public string g_filePathRamOld = "";
        public bool g_bSaveDone = false;
        public string g_sBarcodeSystem = "";
        public string g_sBarcodeSystemOld = "";
        public int n_barcodePreviousTime;
        public string fileNameSaveImage = "";
        public int iParcelCount = 0;

        public bool PalletInProgress = false;
        public string CurrentPalletID = "";
        public bool PalletScanCompleted = false;
        public string CurrentWH_Code = "";
        public string CurrentItemCode = "";

        public bool IsAcceptStartNextPallet = false;


        public TAxisIO[] g_tInputCfg;
        public TAxisIO[] g_tOutputCfg;

        public bool g_bSQLCheck = false;
        public int workerNumber = 1;
        public string cameraSmartSelected = "";
        private static Globals GLb = Globals.getInstance();
        public static string g_sLogTemp
        {
            get
            {
                //if (GLb.g_tSysCfg.PCMode == PCcomMode.MASTER)
                //    g_sLogDir = g_sLogDirMaster;
                //else if (GLb.g_tSysCfg.PCMode == PCcomMode.SLAVE)
                //    g_sLogDir = g_sLogDirSlave;
                return g_sLogDir;
            }
        }
        public static string g_sSystemLog
        {
            get { return g_sLogTemp + "SystemLog\\"; }
        }
        public string g_sMesResultDir
        {
            get { return g_sLogTemp + "MeasResult\\"; }
        }
        public string wmsUrl = "http://192.168.1.101:2005/graphql";
        public string ftpUrl = "ftp://induction@" + "";
        public string URL_XML_FILE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Config\\config.xml";
        public string logfileAddress = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Log\\";
        public Stream stm;

        public string folderName = DateTime.Now.ToString("yyyy-MM-dd");
        public string s_IpAddSetting;
        public string s_portAddSetting;
        public string s_CameraName;
        public string s_InductionNumber;
        public string s_FtpIP;

        //SQL Config
        public TSQLConfig g_tSQLConfig;

        //Biến thực hiện lệnh sx
        public bool IsInTask = false;

        public bool runContinue = true;
        public bool runSocket = true;
        public bool m_bGrabbing = true;

        public string[] serialports;
        public Queue<string> fileFtpList = new Queue<string>();
        public SerialPort Barcode_Scanner;
        public Parity GeneralParity = Parity.None;
        public StopBits GeneralStopBit = StopBits.One;
        public StringBuilder SbHh = new StringBuilder();
        public string barcodeImgLabel;
        public string barcodePreivous;
        public int barcodePreviousTime;
        public int performanceBarcodeRight = 0;
        public int performanceCount = 0;
        public int performanceTime = 60000;
        public int performanceMax = 0;
        public int nTotalItems = 0;
        public int nPassItems = 0;
        public int nFailItems = 0;
        public int nTotalParcel = 30;
        public int nTotalParcelAll   = 30;
        public int nParcelDone = 0;
        public int nTotalPallet = 4;
        public int nPalletDone = 0;
        public string g_sWipid = "";
        public int n_InductionNumber = 88;
        public Configure ConfigureDB = new Configure();

        private XDocument xmldoc;
        private XDocument xmlSysdoc;

        public static Configure ConfigureParameter;

        public static void WriteLogFile(string DirName, string fileName, string info)
        {
            try
            {
                if (!Directory.Exists(DirName))
                {
                    Directory.CreateDirectory(DirName);
                }
                FileInfo fileInfo = new FileInfo(DirName + "\\" + fileName);
                if (!fileInfo.Exists)
                {
                    FileStream fileStream = File.Create(DirName + "\\" + fileName);
                    fileStream.Close();
                    fileInfo = new FileInfo(DirName + "\\" + fileName);
                    fileStream.Dispose();
                }
                FileStream fileStream2 = fileInfo.OpenWrite();
                StreamWriter streamWriter = new StreamWriter(fileStream2);
                streamWriter.BaseStream.Seek(0L, SeekOrigin.End);
                streamWriter.Write("{0} \n\r", info);
                streamWriter.Flush();
                streamWriter.Close();
                streamWriter.Dispose();
                fileStream2.Dispose();
                fileInfo = null;
            }
            catch { }
        }

        public static Configure ReadXMLfile(string FileName)
        {
            Configure result = default(Configure);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(FileName);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("configure/DBType");
            XmlNode xmlNode2 = xmlDocument.SelectSingleNode("configure/MySqlConnStr");
            XmlNode xmlNode3 = xmlDocument.SelectSingleNode("configure/SQLServerConnStr");
            XmlNode xmlNode4 = xmlDocument.SelectSingleNode("configure/MsgFileQty");
            XmlNode xmlNode5 = xmlDocument.SelectSingleNode("configure/Title");
            XmlNode xmlNode6 = xmlDocument.SelectSingleNode("configure/UrlWcs201");
            XmlNode xmlNode7 = xmlDocument.SelectSingleNode("configure/HttpTimeout");
            XmlNode xmlNode8 = xmlDocument.SelectSingleNode("configure/OBRDesc");
            XmlNode xmlNode9 = xmlDocument.SelectSingleNode("configure/ConnProbeFrequency");
            XmlNode xmlNode10 = xmlDocument.SelectSingleNode("configure/HBFrequency");
            XmlNode xmlNode11 = xmlDocument.SelectSingleNode("configure/isSendHB");
            XmlNode xmlNode12 = xmlDocument.SelectSingleNode("configure/OracleConnStr");
            XmlNode xmlNode13 = xmlDocument.SelectSingleNode("configure/PLCDesc");
            XmlNode xmlNode14 = xmlDocument.SelectSingleNode("configure/ProductName");
            if (xmlNode.InnerText == "SQLSERVER")
            {
                result.ConnectDBString = xmlNode3.InnerText;
            }
            else if (xmlNode.InnerText == "Oracle")
            {
                result.ConnectDBString = xmlNode12.InnerText;
            }
            result.DBType = xmlNode.InnerText;
            result.MsgFileQty = Convert.ToInt32(xmlNode4.InnerText);
            result.Title = xmlNode5.InnerText;
            result.ConnProbeFrequency = Convert.ToInt32(xmlNode9.InnerText);
            result.HBFrequency = Convert.ToInt32(xmlNode10.InnerText);
            result.isSendHB = Convert.ToInt32(xmlNode11.InnerText);
            result.PLCDesc = xmlNode13.InnerText;
            result.BcrDesc = xmlNode8.InnerText;
            result.ProductName = xmlNode14.InnerText;
            result.UrlWcs201 = xmlNode6.InnerText;
            result.HttpTimeout = Convert.ToInt16(xmlNode7.InnerText);
            return result;
        }

        public static string getBytesToHexStr(byte[] msg, int len)
        {
            string text = "";
            for (int i = 0; i < len; i++)
            {
                text += Convert.ToString(msg[i], 16).PadLeft(2, '0').ToUpper();
            }
            return text;
        }

        public static string getBytesToStr(byte[] msg, int len)
        {
            string text = "";
            for (int i = 0; i < len; i++)
            {
                text += Convert.ToChar(msg[i]).ToString().ToUpper();
            }
            return text;
        }

        public static byte[] getBytes(string msg)
        {
            byte[] array = new byte[msg.Length / 2];
            int num = 0;
            while (msg.Length > 0)
            {
                array[num++] = Convert.ToByte(Convert.ToInt32(msg.Substring(0, 2), 16));
                msg = ((msg.Length <= 2) ? "" : msg.Substring(2));
            }
            return array;
        }

        public void SaveSystemConfig()
        {
            XElement emp = xmldoc.Descendants("CameraHIK").FirstOrDefault();
            if (emp != null)
            {
                emp.Element("IpAddress").Value = s_IpAddSetting;
                emp.Element("Port").Value = s_portAddSetting;
                emp.Element("CameraName").Value = cameraSmartSelected;
                emp.Element("InductionNumber").Value = s_InductionNumber;
                emp.Element("FtpIP").Value = s_FtpIP;
            }

            // Kiểm tra và xử lý trường hợp emp_sql là null
            XElement emp_sql = xmldoc.Descendants("SQL").FirstOrDefault();
            if (emp_sql == null)
            {
                // Nếu emp_sql null, tạo mới phần tử <SQL> và thêm vào xmldoc
                emp_sql = new XElement("SQL",
                    new XElement("SQLType", g_tSQLConfig.sqlType.ToString()),
                    new XElement("SQLString", g_tSQLConfig.SqlString)
                );
                xmldoc.Root.Add(emp_sql); // Thêm phần tử mới vào root của tài liệu XML
            }
            else
            {
                // Nếu emp_sql đã tồn tại, chỉ cần cập nhật các giá trị
                emp_sql.Element("SQLType").Value = g_tSQLConfig.sqlType.ToString();
                emp_sql.Element("SQLString").Value = g_tSQLConfig.SqlString;
            }

            xmldoc.Save(URL_XML_FILE); // Lưu lại tài liệu XML
        }

        public void LoadSystemConfig()
        {
            if (File.Exists(URL_XML_FILE))
            {
                xmldoc = XDocument.Load(URL_XML_FILE);

                var data = xmldoc
                    .Descendants("CameraHIK")
                    .Select(p => new
                    {
                        IpAddress = p.Element("IpAddress").Value,
                        //CameraName = p.Element("").Value,
                        Port = p.Element("Port").Value,
                        CameraName = p.Element("CameraName").Value,
                        InductionNumber = p.Element("InductionNumber").Value,
                        FtpIP = p.Element("FtpIP").Value,
                    })
                    .OrderBy(p => p.IpAddress)
                    .ToList();

                s_IpAddSetting = data[0].IpAddress;
                s_portAddSetting = data[0].Port;
                s_InductionNumber = data[0].InductionNumber;
                n_InductionNumber = int.Parse(data[0].InductionNumber.ToString());
                cameraSmartSelected = data[0].CameraName;
                s_FtpIP = data[0].FtpIP;

                var dataSql = xmldoc
                    .Descendants("SQL")
                    .Select(p => new
                    {
                        SQLType = p.Element("SQLType").Value,
                        SQLString = p.Element("SQLString").Value,
                    }).ToList();

                if (dataSql != null)
                {
                    g_tSQLConfig.SqlString = dataSql[0].SQLString;
                    g_tSQLConfig.sqlType = (SQLType)Enum.Parse(typeof(SQLType), dataSql[0].SQLType);
                }
            }
        }

        public string getIPadd(int tt)
        {
            string result;
            switch (tt)
            {
                case 1:
                    result = "149.194.207.11";
                    break;
                case 2:
                    result = "149.194.207.12";
                    break;
                case 3:
                    result = "149.194.207.13";
                    break;
                case 4:
                    result = "149.194.207.14";
                    break;
                case 5:
                    result = "149.194.207.15";
                    break;
                case 6:
                    result = "149.194.207.16";
                    break;
                case 7:
                    result = "149.194.207.17";
                    break;
                case 8:
                    result = "149.194.207.18";
                    break;
                case 9:
                    result = "149.194.207.19";
                    break;
                case 10:
                    result = "149.194.207.20";
                    break;
                case 11:
                    result = "149.194.207.21";
                    break;
                case 12:
                    result = "149.194.207.22";
                    break;
                default:
                    result = "192.168.1.199";
                    break;
            }
            return result;
        }

        public string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_]+", "", RegexOptions.Compiled);
        }

        public Globals()
        {
            g_tSysCfg = new TBootUp();

            //g_tModel = new TModel();
            g_tInputCfg = new TAxisIO[(int)Globals.k_InputConfig.INRow_End]; // IO
            g_tOutputCfg = new TAxisIO[(int)Globals.k_OutputConfig.OUTRow_End]; // IO
            g_tDevCfg = new TDevConfig();
            g_tSQLConfig = new TSQLConfig();
            // Input
            for (int i = 0; i < (int)Globals.k_InputConfig.INRow_End; i++)
            {
                g_tInputCfg[i] = new TAxisIO();
            }

            // Output
            for (int i = 0; i < (int)Globals.k_OutputConfig.OUTRow_End; i++)
            {
                g_tOutputCfg[i] = new TAxisIO();
            }
            //Init directory
            if (!Directory.Exists(g_sBootUpDir))
            {
                Directory.CreateDirectory(g_sBootUpDir);
            }

            ///Writter: Phong
            if (!Directory.Exists(g_sCamConfigDir))
            {
                Directory.CreateDirectory(g_sCamConfigDir);
            }
            //////
        }

        public bool TDeviceCfgDataLoad()
        {
            try
            {
                // load g_tBootUp
                g_tDevCfg = (TDevConfig)
                    Util.ProgramDataLoad(g_sBootUpDir + "Detector.cfg", typeof(TDevConfig));

                if (g_tDevCfg != null)
                {
                    g_tDevCfg.CorrectNameDev();
                    g_tDevCfg.buildListDevs();
                }

                return true;
            }
            catch (Exception ex)
            {
                //Delete error file
                if (
                    MessageBox.Show(
                        "Do you want to delete error configuration file?",
                        "Note",
                        MessageBoxButtons.YesNo
                    ) == DialogResult.Yes
                )
                {
                    File.Delete(g_sBootUpDir + "Detector1.cfg");
                }

                return false;
            }
        }

        public bool TDeviceCfgDataSave()
        {
            try
            {
                if (g_tDevCfg != null)
                {
                    g_tDevCfg.CorrectNameDev();
                    g_tDevCfg.buildListDevs();
                }
                // save g_tBootUp
                Util.ProgramDataSave(g_sBootUpDir + "Detector.cfg", typeof(TDevConfig), g_tDevCfg);

                return true;
            }
            catch /*(Exception ex)*/
            {
                return false;
            }
        }

        public bool TDevCfgDataLoad()
        {
            bool bRet1 = TDeviceCfgDataLoad();
            return bRet1;
        }

        public bool BootUpDataLoad()
        {
            try
            {
                // load g_tBootUp
                g_tSysCfg = (TBootUp)
                    Util.ProgramDataLoad(g_sBootUpDir + "BootUp.cfg", typeof(TBootUp));

                return true;
            }
            catch /*(Exception ex)*/
            {
                //Delete error file
                if (
                    MessageBox.Show(
                        "Do you want to delete error configuration file?",
                        "Note",
                        MessageBoxButtons.YesNo
                    ) == DialogResult.Yes
                )
                {
                    File.Delete(g_sBootUpDir + "BootUp.cfg");
                }
                return false;
            }
        }

        public bool BootUpDataSave()
        {
            try
            {
                // save g_tBootUp
                Util.ProgramDataSave(g_sBootUpDir + "BootUp.cfg", typeof(TBootUp), g_tSysCfg);

                return true;
            }
            catch /*(Exception ex)*/
            {
                return false;
            }
        }

        public bool AxisIODataLoad()
        {
            bool bRet = false;
            try
            {
                TAxisIO[] objTemp1 = (TAxisIO[])
                    Util.ProgramDataLoad(g_sBootUpDir + "AxisInput1.cfg", typeof(TAxisIO[]));

                int nMinLength1 =
                    objTemp1.Length < g_tInputCfg.Length ? objTemp1.Length : g_tInputCfg.Length;
                for (int i = 0; i < nMinLength1; i++)
                {
                    g_tInputCfg[i] = objTemp1[i];
                }
                bRet = true;
            }
            catch (Exception ex)
            {
                try
                {
                    TAxisIO[] objTemp1 = (TAxisIO[])
                        Util.ProgramDataLoad(g_sBootUpDir + "AxisInput1_.cfg", typeof(TAxisIO[]));

                    int nMinLength1 =
                        objTemp1.Length < g_tInputCfg.Length ? objTemp1.Length : g_tInputCfg.Length;
                    for (int i = 0; i < nMinLength1; i++)
                    {
                        g_tInputCfg[i] = objTemp1[i];
                    }

                    bRet = true;
                }
                catch
                {
                    //Delete error file
                    MessageBox.Show(
                        "AxisInput1.cfg is corrupted \r\n\r\nContact the Administrator \r\n\r\nThe program will be closed \r\n\r\nIf you are administrator, confirm to continue (ctr)"
                    );
                    if (!Control.ModifierKeys.HasFlag(Keys.Control))
                    {
                        ThreadsManager.GetInstance().StopAll();
                        System.Environment.Exit(1);
                    }
                    else
                    {
                        //File.Delete(g_sBootUpDir + "AxisInput.cfg");
                        MessageBox.Show("Continue as Administrator");
                    }

                    bRet = false;
                }
            }

            try
            {
                TAxisIO[] objTemp1 = (TAxisIO[])
                    Util.ProgramDataLoad(g_sBootUpDir + "AxisOutput1.cfg", typeof(TAxisIO[]));

                int nMinLength1 =
                    objTemp1.Length < g_tOutputCfg.Length ? objTemp1.Length : g_tOutputCfg.Length;
                for (int i = 0; i < nMinLength1; i++)
                {
                    g_tOutputCfg[i] = objTemp1[i];
                }

                bRet = true;
            }
            catch /*(Exception ex)*/
            {
                try
                {
                    TAxisIO[] objTemp1 = (TAxisIO[])
                        Util.ProgramDataLoad(g_sBootUpDir + "AxisOutput1_.cfg", typeof(TAxisIO[]));

                    int nMinLength1 =
                        objTemp1.Length < g_tOutputCfg.Length
                            ? objTemp1.Length
                            : g_tOutputCfg.Length;
                    for (int i = 0; i < nMinLength1; i++)
                    {
                        g_tOutputCfg[i] = objTemp1[i];
                    }

                    bRet = true;
                }
                catch
                {
                    //Delete error file
                    MessageBox.Show(
                        "AxisOutput.cfg is corrupted \r\n\r\nContact the Administrator \r\n\r\nThe program will be closed \r\n\r\nIf you are administrator, confirm to continue (ctr)"
                    );
                    if (!Control.ModifierKeys.HasFlag(Keys.Control))
                    {
                        ThreadsManager.GetInstance().StopAll();
                        System.Environment.Exit(1);
                    }
                    else
                    {
                        //File.Delete(g_sBootUpDir + "AxisOutput.cfg");
                        MessageBox.Show("Continue as Administrator");
                    }
                    bRet = false;
                }
            }

            return bRet;
        }

        public bool TDevCfgDataSave()
        {
            bool bRet1 = TDeviceCfgDataSave();
            return bRet1;
        }

        public bool AxisIODataSave(bool b = false)
        {
            bool bRet = false;
            try
            {
                // save g_tInputCfg
                Util.ProgramDataSave(
                    g_sBootUpDir + "AxisInput1.cfg",
                    typeof(TAxisIO[]),
                    g_tInputCfg
                );

                if (b)
                {
                    Util.ProgramDataSave(
                        g_sBootUpDir + "AxisInput1_.cfg",
                        typeof(TAxisIO[]),
                        g_tInputCfg
                    );
                }
                bRet = true;
            }
            catch /*(Exception ex)*/
            {
                bRet = false;
            }
            try
            {
                // save g_tOutputCfg
                Util.ProgramDataSave(
                    g_sBootUpDir + "AxisOutput1.cfg",
                    typeof(TAxisIO[]),
                    g_tOutputCfg
                );
                if (b)
                {
                    Util.ProgramDataSave(
                        g_sBootUpDir + "AxisOutput1_.cfg",
                        typeof(TAxisIO[]),
                        g_tOutputCfg
                    );
                }
            }
            catch /*(Exception ex)*/
            {
                bRet = false;
            }
            return bRet;
        }

        public bool LoadMotionConfig()
        {
            //bool bRet0 = AxisCfgDataLoad();
            //bool bRet1 = JigCfgDataLoad();
            bool bRet2 = AxisIODataLoad();
            return (bRet2);
        }

        public bool SaveMotionConfig(bool b = false)
        {
            bool bRet2 = AxisIODataSave(b);
            return (bRet2);
        }

        public bool SaveSysCfg()
        {
            bool bRet0 = TDevCfgDataSave();
            // bool bRet1 = TSysCfgDataSave();
            return (bRet0);
        }

        public bool LoadSysCfg()
        {
            bool bRet0 = TDevCfgDataLoad();
            //bool bRet1 = TSysCfgDataLoad();
            return (bRet0);
        }

        public bool SaveAllData(bool bSave = true)
        {
            bool bRet0 = BootUpDataSave();
            bool bRet1 = SaveMotionConfig();
            bool bRet2 = SaveSysCfg();
            //Writter: Phong
            bool bRet4 = CamConfigDataSave();
            SaveSystemConfig();
            //
            //bool bRet3 = SaveTModel();
            return (bRet0 && bRet1 && bRet2 && bRet4);
        }

        public bool LoadAllData()
        {
            bool bRet0 = BootUpDataLoad();
            //bool bRet1 = LoadMotionConfig();
            bool bRet2 = LoadSysCfg();
            //LoadSystemConfig();
            //Writter: Phong
            bool bRet3 = CamConfigDataLoad();

            return (bRet0 && bRet2 && bRet3);
        }

        #region Class Extension
        public class TDetectorConfig
        {
            public bool bActive = false;
            public bool bUseBarcodeServer = false;

            public string sDevGroup = string.Empty;
            public string sDevName = string.Empty;
            public string sID = string.Empty;

            public string sCameraIp = string.Empty;
            public string sCameraPort = string.Empty;
            public string sExporeSureTime = string.Empty;
            public string sCameraMode = string.Empty;

            public override string ToString()
            {
                return sDevName;
            }
        }

        public class TRS232DeviceConfig
        {
            public bool bActive = false;

            public string sDevGroup = string.Empty;
            public string sDevName = string.Empty;
            public string sID = string.Empty;

            public string sPort = string.Empty;
            public string sBaudrate = string.Empty;
            public string sParity = string.Empty;
            public string sDatabits = string.Empty;
            public string sStopbits = string.Empty;

            public override string ToString()
            {
                return sDevName;
            }

            public object Clone()
            {
                return this.MemberwiseClone();
            }
        }

        public class TScaleConfig : TRS232DeviceConfig
        {
            //public bool bActive = false;

            //public string sDevGroup = string.Empty;
            //public string sDevName = string.Empty;
            //public string sID = string.Empty;

            //public string sPort = string.Empty;
            //public string sBaudrate = string.Empty;
            //public string sParity = string.Empty;
            //public string sDatabits = string.Empty;
            //public string sStopbits = string.Empty;

            //public override string ToString()
            //{
            //    return sDevName;
            //}
            //public object Clone()
            //{
            //    return this.MemberwiseClone();
            //}
        }

        public class TBarcodeConfig : TRS232DeviceConfig { }

        public class THIKConfig : TDetectorConfig
        {
            //public bool bActive = false;
            //public bool bUseBarcodeServer = false;

            //public string sDevGroup = string.Empty;
            //public string sDevName = string.Empty;
            //public string sID = string.Empty;

            //public string sCameraIp = string.Empty;
            //public string sCameraPort = string.Empty;
            //public string sExporeSureTime = string.Empty;

            //public override string ToString()
            //{
            //    return sDevName;
            //}
        }

        public class TWMSConfig
        {
            public bool bActive = false;
            public string URL = "";
            public string sDevName = string.Empty;
            public string sDevGroup = string.Empty;

            public override string ToString()
            {
                return sDevName;
            }
        }

        public class RFIDConfig
        {
            public bool bActive = false;
            public string URL = "";
            public string sDevName = string.Empty;
            public string sDevGroup = string.Empty;

            public override string ToString()
            {
                return sDevName;
            }
        }

        public class TPrinterConfig : TDevUtilDeviceConfig
        {
            public override TEnumUtilDevice nDeviceType
            {
                get { return TEnumUtilDevice.PRINTER; }
            }

            private string m_AddressPrinter = "";

            [Description("Example Displaying hint 2")]
            [Category("Detector")]
            [DisplayName("Address Printer ")]
            public string nAddressPrinter
            {
                get { return m_AddressPrinter; }
                set { m_AddressPrinter = value; }
            }
        }
        public class TSQLConfig
        {
            public SQLType sqlType;
            public string SqlString { get; set; }

            public TSQLConfig()
            {
                sqlType = SQLType.MSSQL;
            }
        }

        public class TPrinterGodex : TDevUtilDeviceConfig
        {
            public TPrinterConnectType connectType;
            public string sUsbName;
            public string sCom;
            public string sBaud;
            public string sLanIp;
            public string sPort;
            public string sDriver;

            public override TEnumUtilDevice nDeviceType
            {
                get { return TEnumUtilDevice.PRINTER; }
            }
        }


        public class TAxisIO
        {
            public string nNo { get; set; }
            public string sItem { get; set; }
            public string sType { get; set; }
            public string nModule { get; set; }
            public int nBitIndex { get; set; }
            public int nValue { get; set; }

            public TAxisIO()
            {
                nNo = "00";
                sItem = "Green Tower Lamp";
                sType = "NONE";
                nModule = "-1";
                nBitIndex = 0;
                nValue = 1;
            }
        };

        public struct Configure
        {
            public string ConnectDBString;

            public int MsgFileQty;

            public string Title;

            public string DBType;

            public int HBFrequency;

            public int ConnProbeFrequency;

            public int isSendHB;

            public string UrlWcs201;

            public int HttpTimeout;

            public string PLCDesc;

            public string BcrDesc;

            public string ProductName;

            public string pwd;
        }

        public class TBootUp
        {
            public bool bDemoMode = false; // run all device ok
            public bool bSimulationMode = false; // Run simulation some device
            public string szSelectedModel = "";
            public bool bGMES_Used = false;
            public int n_MultipleCamera = 1;
            public string sSoftwareName = "";

            public int nLogLevel;
            public string szDrawPatternList = string.Empty;
            public string szRunningSequence = string.Empty;
            public string szPassword = string.Empty;

            public string szDateFormat = "yyyy_MM_dd";
            public string szTimeFormat = "HH_mm_ss";

            public int[] nTemp = new int[32];
            public double[] dTemp = new double[32];
            public bool[] bTemp = new bool[32];
            public string[] szTemp = new string[32]; /* length is 256*/

            //Jig config
            public double dSafetyZAxisMovePos = 0;
            public double dRy = 0;
            public double dRx = 0;
            public double dMinMeasDistance = 10; // 10mm
            public double dMeasDistance = 500; // 500 mm

            public int nLanguage = 0;
            public bool bSpecFromGmes = false;
            public int nRobotSafeModel = 1;

            public string StartDayStr = string.Empty;
            public int nPassCountJig1 = 0;
            public int nFailCountJig1 = 0;
            public int nPassCountJig2 = 0;
            public int nFailCountJig2 = 0;
            public int nPassCountJig3 = 0;
            public int nFailCountJig3 = 0;
            public int nPassCountJig4 = 0;
            public int nFailCountJig4 = 0;
            public int nPassCountPin = 0;
            public int nFailCountPin = 0;

            public int nScaleValue = 0;
            public int nScaleError = 0;
            public int nTimeScale = 0;

            public int nParcelNumber = 0;
            public int nPalletNumber = 0;
            public string sParcelType = "";

            public bool bPrintPallet = false;

            public string[] sPositions = { "","","","","","" };

            public TBootUp()
            {
                //for (int i = 0; i < 500; i++)
                //{
                //    defaultPoints[i] = new TMotionPoint();
                //}
                //for (int i = 0; i < 2; i++)
                //{
                //    softwareExecute[i] = new TSoftwareExecuteInfo();
                //}
                //for (int i = 0; i < 500; i++)
                //{
                //    defaultPoints[i] = new TMotionPoint();
                //}

                //for (int i = 0; i < 500; i++)
                //{
                //    defaultPointsRobot[i] = new TRobotPoint();
                //}
            }

            public bool bUseCNCMotion = true;

            public bool bUseRobotMotion = true;
        };

        public struct writeLoginfo
        {
            public string Directory;

            public string FileName;

            public string loginfo;
        }

        public class TSystemConfig
        {
            public TCaseTestEnd CaseTestEnd = new TCaseTestEnd();
            public TTTimeToGmes TTimeToGmes = new TTTimeToGmes();
            public LifeTimeClass[] LifeTime = new LifeTimeClass[2];

            //public TMotionPoint[] defaultPoints = new TMotionPoint[MOTION_POS_COUNT_MAX];

            public List<string> PV_AOIList = new List<string>();
            public List<int> PV_cVisionList = new List<int>();

            //public string[] MasterSampleWip = new string[(int)MasterSampeList.Count];

            public int nMasterTimeout = 0;
            public int nSlaveTimeout = 0;
            public string Password = string.Empty;
            public double PalletDownWaitTime = 0;
            public int nWipLength = 15;
            public string sCR250SetupCmd = string.Empty;
            public int nRobotSafeModel = 1;
            public int nRobotSafePos = 1;
            public int nMotionSafePos = 0;
            public string sSoftwareName = "";

            //public BarcodeScanType eBarcodeScanType = BarcodeScanType.MASS_STAMP;
            public double dFontExportScale = 1;

            //public TEnumLightType nLightSourceType = TEnumLightType.LFINE;
            //public TEnumGMESType nGmesType = TEnumGMESType.XML;
            public PCcomMode PCMode = PCcomMode.NONE;

            public string sOqaIp = "10.224.200.79";
            public int nOqaPort = 3737;

            public int nLanguage = 0;
            public int nNumberColumnPinPanel = 2;

            private bool m_bDemoMode = false; // run all device ok

            [DisplayName("Demo Mode")]
            public bool bDemoMode
            {
                get { return m_bDemoMode; }
                set { m_bDemoMode = value; }
            }

            private bool m_bSimulationMode = false; // Run simulation some device can simulation

            [DisplayName("Simulation Mode")]
            public bool bSimulationMode
            {
                get { return m_bSimulationMode; }
                set { m_bSimulationMode = value; }
            }

            private bool m_bAutoChangeModel = false;

            [DisplayName("Auto Change Model")]
            public bool bAutoChangeModel
            {
                get { return m_bAutoChangeModel; }
                set { m_bAutoChangeModel = value; }
            }

            private bool m_bModelMappingByKey = false;

            [DisplayName("Model Mapping By Key")]
            public bool bModelMappingByKey
            {
                get { return m_bModelMappingByKey; }
                set { m_bModelMappingByKey = value; }
            }

            private bool m_bStartBybarcode = false;

            [DisplayName("Start Meas by Barcode")]
            public bool bStartBybarcode
            {
                get { return m_bStartBybarcode; }
                set { m_bStartBybarcode = value; }
            }
            private bool m_bUseRobotMotion = false;

            [DisplayName("Use Robot")]
            public bool bUseRobotMotion
            {
                get { return m_bUseRobotMotion; }
                set { m_bUseRobotMotion = value; }
            }
            private bool m_bUseCNCMotion = false;

            [DisplayName("Use CNC Motion")]
            public bool bUseCNCMotion
            {
                get { return m_bUseCNCMotion; }
                set { m_bUseCNCMotion = value; }
            }
            private bool m_bRobotMoveToSafe = false;

            [DisplayName("Robot Move To Safe")]
            public bool bRobotMoveToSafe
            {
                get { return m_bRobotMoveToSafe; }
                set { m_bRobotMoveToSafe = value; }
            }
            private bool m_bMotionMoveToSafe = false;

            [DisplayName("Motion Move To Safe")]
            public bool bMotionMoveToSafe
            {
                get { return m_bMotionMoveToSafe; }
                set { m_bMotionMoveToSafe = value; }
            }
            private bool m_bAutoScale = false;

            [DisplayName("Auto Scale")]
            public bool bAutoScale
            {
                get { return m_bAutoScale; }
                set { m_bAutoScale = value; }
            }
            private bool m_bShowInCompactMode = false;

            [DisplayName("Show main window in simple mode")]
            public bool bShowInCompactMode
            {
                get { return m_bShowInCompactMode; }
                set { m_bShowInCompactMode = value; }
            }
            private bool m_bSpecFromGmes = false;

            [DisplayName("Download Spec GMES")]
            public bool bSpecFromGmes
            {
                get { return m_bSpecFromGmes; }
                set { m_bSpecFromGmes = value; }
            }
            private bool m_bDonotUseSuffix = false;

            [DisplayName("Do not use SUFFIX")]
            public bool bDonotUseSuffix
            {
                get { return m_bDonotUseSuffix; }
                set { m_bDonotUseSuffix = value; }
            }
            private bool m_bUserCheck = true;

            [DisplayName("User Check")]
            public bool bUserCheck
            {
                get { return m_bUserCheck; }
                set { m_bUserCheck = value; }
            }
            private bool m_bKeepOnlyLastBarcode = false;

            [DisplayName("Keep only last barcode")]
            public bool bKeepOnlyLastBarcode
            {
                get { return m_bKeepOnlyLastBarcode; }
                set { m_bKeepOnlyLastBarcode = value; }
            }
            private bool m_bBarcodeReadFromBuffer = true;

            [DisplayName("Barcode read from BUFFER")]
            public bool bBarcodeReadFromBuffer
            {
                get { return m_bBarcodeReadFromBuffer; }
                set { m_bBarcodeReadFromBuffer = value; }
            }
            private bool m_bPinvisionSaveOrgImg = false;

            [DisplayName("Save Origin Images")]
            public bool bPinvisionSaveOrgImg
            {
                get { return m_bPinvisionSaveOrgImg; }
                set { m_bPinvisionSaveOrgImg = value; }
            }
            private bool m_bOqaUse = false;

            [DisplayName("USE OQA")]
            public bool bOqaUse
            {
                get { return m_bOqaUse; }
                set { m_bOqaUse = value; }
            }
            private bool m_bShowHideMenu = false;

            [DisplayName("Show Menu Bar")]
            public bool bShowHideMenu
            {
                get { return m_bShowHideMenu; }
                set { m_bShowHideMenu = value; }
            }
            private bool m_bShowCount = false;

            [DisplayName("Show Count")]
            public bool bShowCount
            {
                get { return m_bShowCount; }
                set { m_bShowCount = value; }
            }
            private bool m_bCheckGmesJobfile = false;

            [DisplayName("Check Gmes in job file")]
            public bool bCheckGmesJobfile
            {
                get { return m_bCheckGmesJobfile; }
                set { m_bCheckGmesJobfile = value; }
            }
            private bool m_bOnBarcodeByPLC = false;

            [DisplayName("On barcode by PLC")]
            public bool bOnBarcodeByPLC
            {
                get { return m_bOnBarcodeByPLC; }
                set { m_bOnBarcodeByPLC = value; }
            }
            private bool m_bUploadSpecGmes = false;

            [DisplayName("Upload Spec GMES")]
            public bool bUploadSpecGmes
            {
                get { return m_bUploadSpecGmes; }
                set { m_bUploadSpecGmes = value; }
            }
            private bool m_bLockModel = false;

            [DisplayName("Lock Model")]
            public bool bLockModel
            {
                get { return m_bLockModel; }
                set { m_bLockModel = value; }
            }
            private bool m_bGmes_Upload = false;

            [DisplayName("GMES Upload NG")]
            public bool bGmesUploadNG
            {
                get { return m_bGmes_Upload; }
                set { m_bGmes_Upload = value; }
            }

            public TSystemConfig()
            {
                for (int i = 0; i < 2; i++)
                {
                    LifeTime[i] = new LifeTimeClass();
                }
            }

            public void InitArrData()
            {
                if (this.LifeTime == null || LifeTime.Length < 2)
                {
                    LifeTime = new LifeTimeClass[2];
                    for (int i = 0; i < 2; i++)
                    {
                        LifeTime[i] = new LifeTimeClass();
                    }
                }
            }
        }

        public class TTTimeToGmes
        {
            public bool bUse = false;
            public string sName = string.Empty;
        }

        public class TCaseTestEnd
        {
            public PalletAction OKCase = PalletAction.Pallet_Down;
            public PalletAction NGCase = PalletAction.Pallet_Down;
            public PalletAction CHECKCase = PalletAction.Pallet_Down;
        }

        public class TWbDefault
        {
            public double dX = 0;
            public double dY = 0;
            public double dZ = 0;
            public double dBase = 0;
        }

        public class TLIFETIME
        {
            public TSampleCount SampleCount = new TSampleCount();
        }

        public class TSampleCount
        {
            public int nPass = 0;
            public int nFail = 0;
            public int nTotal = 0;
        }

        public class LifeTimeClass
        {
            public string Item { get; set; }
            public int Spec { get; set; }
            public int Usage { get; set; }
            public int Remain
            {
                get { return (Spec - Usage); }
            }
            public double Warning { get; set; }

            public LifeTimeClass(string item, int spec, int usage, double warning)
            {
                this.Item = item;
                this.Spec = spec;
                this.Usage = usage;
                this.Warning = Warning;
            }

            public LifeTimeClass()
            {
                this.Item = string.Empty;
                this.Spec = 0;
                this.Usage = 0;
                this.Warning = 0;
            }
        }

        private class BooleanNameConverter : BooleanConverter
        {
            public override object ConvertTo(
                ITypeDescriptorContext context,
                CultureInfo culture,
                object value,
                Type destType
            )
            {
                return (bool)value ? "ON" : "OFF";
            }

            public override object ConvertFrom(
                ITypeDescriptorContext context,
                CultureInfo culture,
                object value
            )
            {
                return (string)value == "ON";
            }
        }

        public class TCommConfig
        {
            private string szDevName = string.Empty;

            [Description("Example Displaying hint 2")]
            [Category("COM Parameter")]
            [DisplayName("Device Name")]
            public string sDevName
            {
                get { return szDevName; }
                set { szDevName = value; }
            }

            private string szDevGroup = string.Empty;

            [Description("Example Displaying hint 2")]
            [Category("COM Parameter")]
            [DisplayName("Device Group Name")]
            public string sDevGroup
            {
                get { return szDevGroup; }
                set { szDevGroup = value; }
            }

            private CommDelimiter m_nzDeli = 0;

            [Description("Example Displaying hint 2")]
            [Category("COM Parameter")]
            [DisplayName("Delimiter")]
            [TypeConverter(typeof(EnumNameConverter))]
            public CommDelimiter nzDeli
            {
                get { return m_nzDeli; }
                set { m_nzDeli = value; }
            }

            [Browsable(false)] //this property should be visible
            public int nDeli
            {
                get { return (int)m_nzDeli; }
                set { m_nzDeli = (CommDelimiter)value; }
            }

            public override string ToString()
            {
                return sDevName;
            }

            private string m_sPort = "COM2";

            [Description("Example Displaying hint 2")]
            [Category("COM Parameter")]
            [DisplayName("Port")]
            public string sPort
            {
                get { return m_sPort; }
                set { m_sPort = value; }
            }

            public int nBaud = 0;
            public int nParity = 0;
            public int nDataBits = 0;
            public int nStopBits = 0;
            public bool isRemoteMode = false;

            public string sIP_Client = "127.0.0.1";
            public int nPort_Client = 23;

            public string sIP_Server = "127.0.0.1";
            public int nPort_Server = 23;

            public string udpRecvPort = "8888";
            public string udpSendPort = "8887";

            private bool m_isRTSOn = false;

            [Description("Example Displaying hint 2")]
            [Category("COM Parameter")]
            [DisplayName("RTS State")]
            [TypeConverter(typeof(BooleanNameConverter))]
            public bool isRTSOn
            {
                get { return m_isRTSOn; }
                set { m_isRTSOn = value; }
            }

            private bool m_isDTROn = false;

            [Description("Example Displaying hint 2")]
            [Category("COM Parameter")]
            [DisplayName("DTR State")]
            [TypeConverter(typeof(BooleanNameConverter))]
            public bool isDTROn
            {
                get { return m_isDTROn; }
                set { m_isDTROn = value; }
            }
        }

        //public class THIKConfig : TDetectorConfig
        //{

        //}
        public class TDevUtilDeviceConfig : ICloneable
        {
            bool m_bActive = false; // Use this detector

            [Description("Example Displaying hint 2")]
            [Category("Device")]
            [DisplayName("Active Device")]
            [TypeConverter(typeof(BooleanNameConverter))]
            public bool bActive
            {
                get { return m_bActive; }
                set { m_bActive = value; }
            }

            [Description("Example Displaying hint 2")]
            [Category("Device")]
            [DisplayName("LCIS Device ID")]
            public virtual int nLCISDeviceID
            {
                get
                {
                    int nValue = 10 * ((int)nDeviceType) + nIndex;
                    return nValue;
                }
            }

            private TEnumUtilDevice m_nDetectorType = TEnumUtilDevice.PLC;

            [Description("Example Displaying hint 2")]
            [Category("Device")]
            [DisplayName("Device Type")]
            [TypeConverter(typeof(EnumNameConverter))]
            public virtual TEnumUtilDevice nDeviceType
            {
                get { return m_nDetectorType; }
            }

            private int m_nIndex = 0;

            [Description("Example Displaying hint 2")]
            [Category("Device")]
            [DisplayName("Index")]
            [ReadOnly(true)] //but just read only
            public int nIndex
            {
                get { return m_nIndex; }
                set { m_nIndex = value; }
            }

            private int m_nMeasDoneWT = 5000;

            [Description("Example Displaying hint 2")]
            [Category("Device")]
            [DisplayName("Measurement Done Wait Time")]
            public int nMeasDoneWT
            {
                get { return m_nMeasDoneWT; }
                set { m_nMeasDoneWT = value; }
            }

            CommType m_nCommType = CommType.RS232;

            [Description("Example Displaying hint 2")]
            [Category("Device")]
            [DisplayName("Communication Type")]
            [TypeConverter(typeof(EnumNameConverter))]
            public CommType nCommType
            {
                get { return m_nCommType; }
                set { m_nCommType = value; }
            }

            private TCommConfig _comCfg = new TCommConfig();

            [Description("Example Displaying hint 2")]
            [Category("Communication")]
            [DisplayName("Communication Parameter")]
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public TCommConfig tCommCfg
            {
                get { return _comCfg; }
                set { _comCfg = value; }
            }

            public override string ToString()
            {
                return _comCfg.sDevName;
            }

            public object Clone()
            {
                return this.MemberwiseClone();
            }
        }

        public class TPLCConfig : TDevUtilDeviceConfig
        {
            public override TEnumUtilDevice nDeviceType
            {
                get { return TEnumUtilDevice.PLC; }
            }

            private int m_nStationNumber = 0;

            [Description("Example Displaying hint 2")]
            [Category("Detector")]
            [DisplayName("Logical Station Number")]
            public int nStationNumber
            {
                get { return m_nStationNumber; }
                set { m_nStationNumber = value; }
            }

            private string p_sInputModuleAddress = "D3200";

            [Description("Example Displaying hint 2")]
            [Category("Detector")]
            [DisplayName("Start Input Address")]
            public string m_sInputModuleAddress
            {
                get { return p_sInputModuleAddress; }
                set { p_sInputModuleAddress = value; }
            }

            private int p_nInputBlockData = 2;

            [Description("Example Displaying hint 2")]
            [Category("Detector")]
            [DisplayName("Number Of Input Module")]
            public int m_nInputBlockData
            {
                get { return p_nInputBlockData; }
                set { p_nInputBlockData = value; }
            }

            private string p_sOutputModuleAddress = "D3000";

            [Description("Example Displaying hint 2")]
            [Category("Detector")]
            [DisplayName("Start Output Address")]
            public string m_sOutputModuleAddress
            {
                get { return p_sOutputModuleAddress; }
                set { p_sOutputModuleAddress = value; }
            }

            private int p_nOutputBlockData = 2;

            [Description("Example Displaying hint 2")]
            [Category("Detector")]
            [DisplayName("Number Of Output Module")]
            public int m_nOutputBlockData
            {
                get { return p_nOutputBlockData; }
                set { p_nOutputBlockData = value; }
            }

            private TPLCAddress[] p_listAddressScan = new TPLCAddress[0];

            [Description("Example Displaying hint 2")]
            [Category("Detector")]
            [DisplayName("PLC Address")]
            public TPLCAddress[] m_arrayAddressScan
            {
                get { return p_listAddressScan; }
                set { p_listAddressScan = value; }
            }

            [Browsable(false)] //this property should be visible
            [XmlIgnore]
            public List<TPLCAddress> m_listAddressScan
            {
                get { return p_listAddressScan.ToList(); }
            }

            //WarehouseParemeter

            private TPLCWarehouseParameterAddress[] p_listWarehouseParameterrAddressScan =
                new TPLCWarehouseParameterAddress[0];

            [Description("Example Displaying hint 2")]
            [Category("Detector")]
            [DisplayName("PLC Inverter Address")]
            public TPLCWarehouseParameterAddress[] m_listWarehouseParameterAddressScan
            {
                get { return p_listWarehouseParameterrAddressScan; }
                set { p_listWarehouseParameterrAddressScan = value; }
            }

            [Browsable(false)] //this property should be visible
            [XmlIgnore]
            public List<TPLCWarehouseParameterAddress> m_listWarehouseAddressScan
            {
                get { return p_listWarehouseParameterrAddressScan.ToList(); }
            }
        }

        public class TPLCAddress
        {
            [DisplayName("Name")]
            public string sName { get; set; }

            [DisplayName("Address")]
            public string sAddress { get; set; }

            [DisplayName("Number Of Module")]
            public int nModule { get; set; }

            [DisplayName("Bool Value")]
            public bool bValue { get; set; }

            [DisplayName("Short Value")]
            public bool nValue { get; set; }

            TEnumPLCAddressType m_nCommType = TEnumPLCAddressType.Monitor;

            [DisplayName("Address Type")]
            [TypeConverter(typeof(EnumNameConverter))]
            public TEnumPLCAddressType nCommType
            {
                get { return m_nCommType; }
                set { m_nCommType = value; }
            }

            public TPLCAddress()
            {
                sName = "N/A";
                sAddress = "D2000";
                nModule = 2;
                m_nCommType = TEnumPLCAddressType.Monitor;
            }

            public override string ToString()
            {
                return sName;
            }
        }

        public class TPLCWarehouseParameterAddress
        {
            [DisplayName("ALS01")]
            public string sALS01_Name { get; set; }

            [DisplayName("ALS02")]
            public string sALS02_Name { get; set; }

            [DisplayName("ALS03")]
            public string sALS03_Name { get; set; }

            [DisplayName("Temparature01")]
            public string sTp01_Name { get; set; }

            [DisplayName("Temparature02")]
            public string sTp02_Name { get; set; }

            [DisplayName("Temparature03")]
            public string sTp03_Name { get; set; }

            [DisplayName("Humidity01")]
            public string sHum01_Name { get; set; }

            [DisplayName("Humidity02")]
            public string sHum02_Name { get; set; }

            [DisplayName("Humidity03")]
            public string sHum03_Name { get; set; }

            TEnumPLCAddressType m_nCommType = TEnumPLCAddressType.Monitor;

            [DisplayName("Address Type")]
            [TypeConverter(typeof(EnumNameConverter))]
            public TEnumPLCAddressType nCommType
            {
                get { return m_nCommType; }
                set { m_nCommType = value; }
            }

            public TPLCWarehouseParameterAddress()
            {
                sALS01_Name = "N/A";
                sALS02_Name = "N/A";
                sALS03_Name = "N/A";

                sTp01_Name = "N/A";
                sTp02_Name = "N/A";
                sTp03_Name = "N/A";

                sHum01_Name = "N/A";
                sHum02_Name = "N/A";
                sHum03_Name = "N/A";

                m_nCommType = TEnumPLCAddressType.Monitor;
            }

            public override string ToString()
            {
                return sALS01_Name;
            }
        }

        public class TDevConfig
        {
            [XmlIgnore]
            public List<TDetectorConfig> tDetectorList = new List<TDetectorConfig>();
            public List<TDevUtilDeviceConfig> tDeviceList = new List<TDevUtilDeviceConfig>();
            public THIKConfig[] tHIK = new THIKConfig[1];

            public TWMSConfig tWMS = new TWMSConfig();
            public TScaleConfig tScale = new TScaleConfig();
            public TBarcodeConfig tBarcode = new TBarcodeConfig();
            public RFIDConfig tRFID = new RFIDConfig();
            public TPLCConfig tPLC = new TPLCConfig();
            public TPrinterConfig tPRINTER = new TPrinterConfig();
            public TPrinterGodex tPrinterGodex = new TPrinterGodex();

            public TDevConfig()
            {
                //for (int i = 0; i < 1; i++)
                //{
                //    //tBasler[i] = new TBaslerConfig();
                //    //tBasler[i].sDevGroup = "BASLER" + i.ToString();
                //    tHIK[i] = new THIKConfig();
                //    tHIK[i].sDevGroup = "HIK" + i.ToString();
                //}
            }

            public void buildListDevs()
            {
                tDeviceList.Clear();
                tDeviceList.Add(tPLC);
                tDeviceList.Add(tPRINTER);
                tDeviceList.Add(tPrinterGodex);
                tDetectorList.Clear();
                //tDetectorList.Add(tHIK[0]);
                //tDetectorList.Add(tHIK[1]);
            }

            public void CorrectNameDev()
            {
                //if (tHIK == null || tHIK.Length != 1) tHIK = new THIKConfig[1];
                //for (int i = 0; i < 1; i++)
                //{
                //    if (tHIK[i] == null) tHIK[i] = new THIKConfig();
                //    if (tHIK[i].sDevName.Trim() == string.Empty) tHIK[i].sDevName = "HIK" + i.ToString();
                //    tHIK[i].sDevGroup = "HIK" + i.ToString();
                //}

                if (tWMS == null)
                {
                    tWMS = new TWMSConfig();
                }

                if (tWMS.sDevName.Trim() == string.Empty)
                {
                    tWMS.sDevName = "WMS";
                    tWMS.sDevGroup = "WMS";
                }

                if (tRFID == null)
                {
                    tRFID = new RFIDConfig();
                }

                if (tRFID.sDevName.Trim() == string.Empty)
                {
                    tRFID.sDevName = "RFID";
                    tRFID.sDevGroup = "RFID";
                }

                if (tScale == null)
                {
                    tScale = new TScaleConfig();
                }

                if (tScale.sDevName.Trim() == string.Empty)
                {
                    tScale.sDevName = "SCALE";
                    tScale.sDevGroup = "SCALE";
                }

                if (tBarcode == null)
                {
                    tBarcode = new TBarcodeConfig();
                }

                if (tBarcode.sDevName.Trim() == string.Empty)
                {
                    tBarcode.sDevName = "BARCODE";
                    tBarcode.sDevGroup = "BARCODE";
                }

                if (tPLC == null)
                {
                    tPLC = new TPLCConfig();
                }

                if (tPLC.tCommCfg.sDevName == string.Empty)
                {
                    tPLC.tCommCfg.sDevName = "PLC";
                    tPLC.tCommCfg.sDevGroup = "PLC";
                }

                if (tPRINTER == null)
                {
                    tPRINTER = new TPrinterConfig();
                }

                if (tPrinterGodex == null)
                {
                    tPrinterGodex = new TPrinterGodex();
                }


                if (tPRINTER.tCommCfg.sDevName == string.Empty)
                {
                    tPRINTER.tCommCfg.sDevName = "PRINTER TSC";
                    tPRINTER.tCommCfg.sDevGroup = "PRINTER TSC";
                }
            }
        };

        #region Writter: Phong
        public class CameraConfig
        {
            public int NoCam { get; set; }
            public string Name { get; set; }
            public string ID { get; set; }
            public float ExposureTime { get; set; }
            public string cameraIp { get; set; }
            public int cameraPort { get; set; }
            public string TriggerMode { get; set; }
            public bool IsActive { get; set; }
            public bool IsUseTcp { get; set; }

            public CameraConfig()
            {
                Name = "";
                cameraIp = "";
                TriggerMode = "";
            }
        }

        public bool CamConfigDataLoad()
        {
            try
            {
                g_tCamConfig = (CameraConfig[])
                    Util.ProgramDataLoad(g_sCamConfigDir + "CamConfig.cfg", typeof(CameraConfig[]));
                return true;
            }
            catch /*(Exception ex)*/
            {
                //Delete error file
                if (
                    MessageBox.Show(
                        "Do you want to delete error configuration file?",
                        "Note",
                        MessageBoxButtons.YesNo
                    ) == DialogResult.Yes
                )
                {
                    File.Delete(g_sCamConfigDir + "CamConfig.cfg");
                }
                return false;
            }
        }

        public bool CamConfigDataSave()
        {
            try
            {
                Util.ProgramDataSave(
                    g_sCamConfigDir + "CamConfig.cfg",
                    typeof(CameraConfig[]),
                    g_tCamConfig
                );
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        #endregion
        #endregion

        #region Enum
        public enum TPrinterConnectType
        {
            USB,
            LAN,
            COM,
            DRIVER,
        }

        public enum TEnumUtilDevice
        {
            [Description("IOMODULE")]
            IOMODULE,

            [Description("GMES")]
            GMES,

            [Description("IO MANAGER")]
            IOMANAGER,

            [Description("DEV MANAGER")]
            DEVMANAGER,

            [Description("PLC")]
            PLC,

            [Description("GMES")]
            GMESERVER,

            [Description("ROBOT")]
            ROBOT,

            [Description("BARCODE")]
            BARCODE,

            [Description("READER")]
            READER,

            [Description("API")]
            API,

            [Description("PRINTER")]
            PRINTER,

            [Description("SCALE")]
            SCALE,

            [Description("BARCODEMANAGER")]
            BARCODEMANAGER,

            [Description("(None)")]
            diCount,
        }

        public enum PalletAction
        {
            Stay,
            Pallet_Down,
        }

        public enum k_InputConfig : int
        {
            //Inverter
            [Description("START PRINT 1")]
            INRow_START_PRINT_01,

            [Description("CHECK PRINTER STATUS")]
            INRow_PRINT_STATUS_01,

            [Description("SENSOR TRIGGER SCANNER")]
            INRow_SENSOR_TRIGGER_SCANNER,

            [Description("SENSOR OUT CONVEYOR")]
            INRow_SENSOR_OUT_CONVEYOR,

            //
            [Description("RobModelData11")]
            INRow_RobModelData11,

            [Description("RobModelData12")]
            INRow_RobModelData12,

            [Description("End")]
            INRow_End,
        }

        public enum k_OutputConfig : int
        {
            [Description("PRINT DONE")]
            OUTRow_PRINT_DONE,

            [Description("PRINT DONE 2")]
            OUTRow_CONVEYOR_02,

            [Description("CONVEYOR 03 RUNNING")]
            OUTRow_CONVEYOR_03,

            [Description("CONVEYOR 04 RUNNING")]
            OUTRow_CONVEYOR_04,

            [Description("CONVEYOR 05 RUNNING")]
            OUTRow_CONVEYOR_05,

            [Description("CONVEYOR 06 RUNNING")]
            OUTRow_CONVEYOR_06,

            [Description("CONVEYOR 07 RUNNING")]
            OUTRow_CONVEYOR_07,

            [Description("CONVEYOR 08 RUNNING")]
            OUTRow_CONVEYOR_08,

            [Description("CONVEYOR 09 RUNNING")]
            OUTRow_CONVEYOR_09,

            [Description("CONVEYOR 10 RUNNING")]
            OUTRow_CONVEYOR_10,

            [Description("End")]
            OUTRow_End,
        }

        public enum InductionName
        {
            Induction01,
            Induction02,
            Induction03,
            Induction04,
            Induction05,
            Induction06,
            Induction07,
            Induction08,
            Induction09,
            Induction10,
            Induction11,
            Induction12,
        }

        public enum CommType
        {
            RS232,
            TCPIP,
            UDP,
            USB,
        }

        public enum CommDelimiter
        {
            NONE,
            CRLF,
            CR,
            LF,
        }

        public enum TEnumPLCAddressType
        {
            Input,
            Output,
            Monitor,
            X,
            Y,
            M,
            Communication,
            Position,
            CountRnR,
            SpeedMotor,
            Text,
            InverterParameter,
        };

        public enum SQLType
        {
            MSSQL,
            MYSQL,
            POSTGRESQL,
            MONGODB,
            SQLLITE,
            ORACLE
        }

        public enum LogLv
        {
            // Tracing information and debugging minutiae; generally only switched on in unusual situations
            // (for receiver information )
            Verbose = 0,

            // Internal control flow and diagnostic state dumps to facilitate pinpointing of recognized problems
            Debug = 1,

            // Events of interest or that have relevance to outside observers; the default enabled minimum logging level
            Information = 2,

            // Indicators of possible issues or service/functionality degradation
            Warning = 3,

            // Indicating a failure within the application or connected system
            Error = 4,

            //Critical errors causing complete failure of the application
            Fatal = 5,

            kLVL_Count,
        }

        public enum PLCErrorCode
        {
            PortConnectionError = 0x01808008,
            LabelInformationError = 0x01802001,
        }

        public enum LogType
        {
            SYSTEM,
            GMES,
        }

        public enum TEnumIOType
        {
            IOCARD,
            IOMOTION,
            IOMODULE,
            IOPLC,
            IOPLCHMI,
            IODAQ,
            NONE,
        }

        public enum PCcomMode
        {
            MASTER,
            SLAVE,
            NONE,
        }

        public enum GUIupdate
        {
            NONE,
            GMESINFO,
            GMESCONNECT,
            GMESDISCONNECT,
            WIPID,
            CHANGEMODEL,
            BARCODE_ERROR,
        }

        private class EnumNameConverter : EnumConverter
        {
            private Type _enumType;

            /// <summary>Initializing instance</summary>
            /// <param name="type">type Enum</param>
            ///this is only one function, that you must
            ///to change. All another functions for enum
            ///you can use by Ctrl+C/Ctrl+V
            public EnumNameConverter(Type type)
                : base(type)
            {
                _enumType = type;
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
            {
                return destType == typeof(string);
            }

            public override object ConvertTo(
                ITypeDescriptorContext context,
                CultureInfo culture,
                object value,
                Type destType
            )
            {
                FieldInfo fi = _enumType.GetField(Enum.GetName(_enumType, value));
                DescriptionAttribute dna = (DescriptionAttribute)
                    Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));

                if (dna != null)
                {
                    return dna.Description;
                }
                else
                {
                    return value.ToString();
                }
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type srcType)
            {
                return srcType == typeof(string);
            }

            public override object ConvertFrom(
                ITypeDescriptorContext context,
                CultureInfo culture,
                object value
            )
            {
                foreach (FieldInfo fi in _enumType.GetFields())
                {
                    DescriptionAttribute dna = (DescriptionAttribute)
                        Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));

                    if ((dna != null) && ((string)value == dna.Description))
                    {
                        return Enum.Parse(_enumType, fi.Name);
                    }
                }
                return Enum.Parse(_enumType, (string)value);
            }
        }

        public static class EnumExtensions
        {
            public static string GetEnumDescription(Enum value)
            {
                FieldInfo fi = value.GetType().GetField(value.ToString());

                DescriptionAttribute[] attributes =
                    fi.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    as DescriptionAttribute[];

                if (attributes != null && attributes.Any())
                {
                    return attributes.First().Description;
                }

                return value.ToString();
            }

            public static List<string> GetDescriptions(Type type)
            {
                var descs = new List<string>();
                var names = Enum.GetNames(type);
                foreach (var name in names)
                {
                    var field = type.GetField(name);
                    var fds = field.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    foreach (DescriptionAttribute fd in fds)
                    {
                        descs.Add(fd.Description);
                    }
                }
                return descs;
            }

            public static int GetEnumValueFromDescription(string description, Type type)
            {
                if (!type.IsEnum)
                {
                    throw new ArgumentException();
                }

                FieldInfo[] fields = type.GetFields();
                var field = fields
                    .SelectMany(
                        f => f.GetCustomAttributes(typeof(DescriptionAttribute), false),
                        (f, a) => new { Field = f, Att = a }
                    )
                    .Where(a => ((DescriptionAttribute)a.Att).Description == description)
                    .SingleOrDefault();
                if (field == null)
                {
                    throw new ArgumentException();
                }
                int nValue = (int)field.Field.GetRawConstantValue();
                return nValue;
            }
        }
        #endregion
    }
}
