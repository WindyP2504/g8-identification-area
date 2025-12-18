using System;
using System.Collections.Generic;
using System.Threading;
using VTP_Induction.Common;
using VTP_Induction.Device;

namespace VTP_Induction
{
    using System.Drawing;
    using TColor = Int32;

    public interface iL2Communication
    {
        string END_FRAME_STR { get; set; }

        bool IsResetEn { get; set; }

        //L2CallbackDataReceived CallbackFunc { get; set; }

        //L2RawDataReceived RawDataReceived { get; set; }

        //L1Disconnect L2DisconectEvent { get; set; }

        //IsACKPkg CheckACKPkg { get; set; }

        //L2State_E L2State { get; set; }

        bool tcpipModeComConnected { get; set; }
        //CommConfig cfg { get; set; }

        void L2Start();

        void L2Stop();

        bool L2TXAddPkg(byte[] sendData, bool isNeedACK, uint resend, string sMidString = "", bool bClearRx = false, object spcecialData = null);

        //bool L2TXAddPkg(L2Pkg pkg);

        bool IsConnected();

        void L2Destroy();

        void ResetDevice();
    }
    public abstract class CDetector : CDevice
    {
        protected Globals GLb = Globals.getInstance();

        protected TForm m_Form;
        protected iL2Communication CommHandler;

        protected bool m_bRunningSet;
        protected List<string> m_slList;

        public double[] m_dApertureDegree = new double[5];
        public string[] m_sApertureDegree = new string[6];

        public int m_nSetAperture;
        public int m_nSetFilter;
        protected string m_sNameDevice;
        public int m_nDelayTm;
        public int m_nTimeoutTm;

        //Define device name
        public virtual bool m_bConnection
        {
            get
            {
                if (GLb.g_tSysCfg.bDemoMode)
                {
                    return true;
                }

                if (CommHandler == null)
                {
                    return false;
                }
                else
                {
                    return CommHandler.IsConnected();
                }
            }
        }

        public string m_sName
        {
            get
            {
                return m_sNameDevice;
            }
        }

        public bool isSerialConn = true;
        public bool m_bBreak;
        public bool m_bIsMeasRunning;

        // Correction Factor
        public int m_pnCorFacSpectrum_Count;
        public int[] m_pnCorFacSpectrum_WL;
        public double[] m_pdCorFacSpectrum_Data;

        //Process
        public Queue<DetectorCmdPkg> CmdList = new Queue<DetectorCmdPkg>();
        public DetectorCmdPkg CurCMDProcessing;

        public ManualResetEvent RecvDataEvent = new ManualResetEvent(false);

        private ThreadsManager ThMan = ThreadsManager.GetInstance();

        public Thread DetectorCMDProcess;
        public ManualResetEvent AddDCPEvent = new ManualResetEvent(false);
        public const int TIMEPERIOD = 2003;

        //Specific configuration
        public bool ACKReq = false;
        public uint Resend = 1;


        public CDetector(TForm Form)
        {
            m_Form = Form;
            m_bBreak = false;
            m_slList = new List<string>();

            m_bRunningSet = false;
            m_bIsMeasRunning = false;

            //m_dApertureDegree[0] = Globals.NO_WORK;
            //m_dApertureDegree[1] = Globals.NO_WORK;
            //m_dApertureDegree[2] = Globals.NO_WORK;
            //m_dApertureDegree[3] = Globals.NO_WORK;
            //m_dApertureDegree[4] = Globals.NO_WORK;

            m_sApertureDegree[0] = "";
            m_sApertureDegree[1] = "";
            m_sApertureDegree[2] = "";
            m_sApertureDegree[3] = "";
            m_sApertureDegree[4] = "";

        }

        public virtual void StartDetector()
        {
            //Thread
            DetectorCMDProcess = new Thread(new ThreadStart(DCMDProcessFunc));
            DetectorCMDProcess.Name = "Detector thread " + m_sName + (new Random()).Next(10000);

            ThMan.AddThread(DetectorCMDProcess.Name, DetectorCMDProcess);

            DetectorCMDProcess.Start();
        }

        //private void DCMDProcessFunc()
        //{
        //    while (true)
        //    {
        //        if (CmdList.Count > 0 || AddDCPEvent.WaitOne(TIMEPERIOD))
        //        {
        //            lock (CmdList)
        //            {
        //                if (CmdList.Count == 0)
        //                    continue;

        //                CurCMDProcessing = CmdList.Dequeue();

        //                if (CurCMDProcessing.isTXOnly)
        //                {
        //                    // Send data only
        //                    SendData(CurCMDProcessing.Data2Send, ACKReq, Resend);
        //                }
        //                else
        //                {
        //                    RecvDataEvent.Reset();
        //                    if (!m_bBreak)
        //                    {
        //                        // Send data
        //                        SendDataNWait(CurCMDProcessing.Data2Send, ACKReq, Resend, CurCMDProcessing.sMidString, CurCMDProcessing.bClearRx);

        //                        CurCMDProcessing.DoneEvent.Reset();
        //                        // And waiting respond package
        //                        if (WaitTime(ref RecvDataEvent, CurCMDProcessing.timeOutTm)) // NOTE : TIME_WAITING_RESP > resend x rs232 tx timeout ( TIME_WAIT_DEV_RESPOND)
        //                        {
        //                            CurCMDProcessing.DoneEvent.Set();
        //                        }
        //                        else
        //                        {//m_bBreak
        //                            string msg = "Detector TIMEOUT! " + CurCMDProcessing.timeOutTm;
        //                            if (m_bBreak && m_bConnection)
        //                            {
        //                                msg = "Order command was canceled.";
        //                            }
        //                            else if (!m_bConnection)
        //                            {
        //                                msg = "Connect was terminated.";
        //                            }

        //                            Log.LogWrite(Globals.LogLv.Information, msg);
        //                            // Do somethings here 
        //                            Log.LogWrite(Globals.LogLv.Information, msg);
        //                        }

        //                        RecvDataEvent.Reset();
        //                    }
        //                }
        //            }
        //        }

        //        AddDCPEvent.Reset();
        //    }
        //}

        private void DCMDProcessFunc()
        {

        }

        public virtual bool FeatureLoad(string sFile)
        {
            return true;
        }

        public virtual bool FeatureSave(string sFile)
        {
            return true;
        }

        public void ClearCMDList()
        {
            CmdList.Clear();
        }

        public void AddDCMDPkgToQueue(DetectorCmdPkg pkg)
        {
            lock (CmdList)
            {
                CmdList.Enqueue(pkg);
            }
            AddDCPEvent.Set();
        }

        public void DestroyCDetector()
        {
            m_bBreak = true;
            if (DetectorCMDProcess != null)
            {
                DetectorCMDProcess.Abort();
                ThMan.RemoveThread(DetectorCMDProcess.Name);
                DetectorCMDProcess = null;
            }
            Disconnect();
        }

        public virtual bool Connect()
        {
            return true;
        }
        public virtual bool capture()
        {
            return true;
        }
        public virtual bool CameraTriggerFromOutSignal()
        {
            return true;
        }
        public virtual bool ReadBarcodeFromHIK()
        {
            return true;
        }
        public Bitmap bmp = null;
        public virtual void Disconnect()
        {

        }

        public virtual bool IsRemote()
        {
            return true;
        }

        public virtual bool SetLocal()
        {
            return true;
        }

        public virtual bool SETTING()
        {
            return true;
        }

        public virtual bool SendCmd(string sCmd)
        {
            return true;
        }




        protected bool COM_Open()
        {
            m_bBreak = false;
            try
            {
                ClearCMDList();

                CommInit();
            }
            catch (Exception ex)
            {
                Log.LogWrite(Globals.LogLv.Information, ex.Message);
                return false;
            }

            return true;
        }

        protected void COM_Close()
        {
            try
            {
                ClearCMDList();

                CommDeInit();
            }
            catch (Exception ex)
            {
                Log.LogWrite(Globals.LogLv.Debug, ex.Message);
            }
        }

        protected void COM_Tx(string sCmd)
        {
            string sLog = "Detector::COM_Tx: ";
            DetectorCmdPkg pkg = new DetectorCmdPkg();
            pkg.Data2Send = Util.StrToByteArray(sCmd);

            Log.LogWrite(Globals.LogLv.Information, sLog + sCmd);
            AddDCMDPkgToQueue(pkg);

        }

        protected int WRITEByte(byte[] szCommand, int iLen)
        {
            /* NOTE : Implement code here */
            return 0;
        }

        protected int WriteBlockString(byte[] szCommand, int iLen)
        {
            /* NOTE :Implement code here */
            return 0;
        }

        protected string COM_TxRx(string sCmd, int dwLimitTm, string sMidString = "")
        {
            string sLog = "Detector::COM_TxRx: ";
            string sReceive = string.Empty;
            DetectorCmdPkg pkg = new DetectorCmdPkg();

            if (sCmd == string.Empty)
            {
                return string.Empty;
            }

            Log.LogWrite(Globals.LogLv.Information, sLog + ">>" + sCmd + " [" + dwLimitTm.ToString() + "]");
            pkg.isTXOnly = false;
            pkg.Data2Send = Util.StrToByteArray(sCmd);
            pkg.sMidString = sMidString;
            pkg.timeOutTm = dwLimitTm;
            pkg.bClearRx = true;
            AddDCMDPkgToQueue(pkg);

            if (WaitTime(ref pkg.DoneEvent, dwLimitTm))
            {
                sReceive = (Util.ByteArrayToStrUTF8(pkg.RecvData));
            }

            Log.LogWrite(Globals.LogLv.Information, sLog + "<<" + sReceive);
            return sReceive;
        }

        //---------------------------------------------------------------------------

        //###########################################################################
        //##################          ÀÏ¹ÝÇÔ¼ö    ###################################
        //###########################################################################

        private int Adjust(double Color, double Factor)
        {
            double Gamma = 0.80;
            int IntensityMax = 255;

            if (Color == 0.0)
            {
                return 0;
            }
            else
            {
                return (int)(IntensityMax * Math.Pow(Color * Factor, Gamma));
            }
        }
        //---------------------------------------------------------------------------

        private TColor WavelengthToRGB(double Wavelength)
        {
            double Red = 0.0;
            double Green = 0.0;
            double Blue = 0.0;
            double factor = 0.0;

            int R;
            int G;
            int B;
            if (380 <= Wavelength && Wavelength <= 439)
            {
                Red = -(Wavelength - 440) / (440 - 380);
                Green = 0.0;
                Blue = 1.0;
            }
            else if (439 < Wavelength && Wavelength <= 489)
            {
                Red = 0.0;
                Green = (Wavelength - 439) / (490 - 439);
                Blue = 1.0;
            }
            else if (489 < Wavelength && Wavelength <= 509)
            {
                Red = 0.0;
                Green = 1.0;
                Blue = -(Wavelength - 510) / (510 - 489);
            }
            else if (509 < Wavelength && Wavelength <= 579)
            {
                Red = (Wavelength - 509) / (580 - 509);
                Green = 1.0;
                Blue = 0.0;
            }
            else if (579 < Wavelength && Wavelength <= 644)
            {
                Red = 1.0;
                Green = -(Wavelength - 645) / (645 - 579);
                Blue = 0.0;
            }
            else if (644 < Wavelength && Wavelength <= 780)
            {
                Red = 1.0;
                Green = 0.0;
                Blue = 0.0;
            }
            else
            {
                Red = 0.0;
                Green = 0.0;
                Blue = 0.0;
            }
            if (380 <= Wavelength && Wavelength <= 419)
            {
                factor = 0.3 + 0.7 * (Wavelength - 380) / (420 - 380);
            }
            else if (419 < Wavelength && Wavelength <= 700)
            {
                factor = 1.0;
            }
            else if (700 < Wavelength && Wavelength <= 780)
            {
                factor = 0.3 + 0.7 * (780 - Wavelength) / (780 - 700);
            }
            else
            {
                factor = 0.0;
            }

            R = Adjust(Red, factor);
            G = Adjust(Green, factor);
            B = Adjust(Blue, factor);

            return R + G * 0x100 + B * 0x10000;
        }

        //---------------------------------------------------------------------------
        //---------------------------------------------------------------------------
        // CIE ÁÂÇ¥°èÀÇ °ªÀ» »óÈ£º¯°æÇÏ´Â ÇÔ¼ö
        // int type : º¯°æÇüÅÂ ¼³Á¤
        //            X Y
        //            ¡è¡è
        //       source destination
        //            3 : 31, 6 : 60, 7 : 76
        //---------------------------------------------------------------------------
        protected bool ChangeCIEValue(int type, double x, double y, ref double cx, ref double cy)
        {
            //if ( cx == NULL || cy == NULL )
            //    return false;

            switch (type)
            {
                case 36:
                    cx = 4 * x / (3 + 12 * y - 2 * x);
                    cy = 2 * (9 * y / (3 + 12 * y - 2 * x)) / 3;
                    break;
                case 37:
                    cx = 4 * x / (3 + 12 * y - 2 * x);
                    cy = 9 * y / (3 + 12 * y - 2 * x);
                    break;
                case 63:
                    cx = 3 * x / (2 * x - 8 * y + 4);
                    cy = y / (x - 4 * y + 2);
                    break;
                case 67:
                    // ÀÌ»óÇØ¼­ ¼öÁ¤ 2005. 5.3 Kim Hyeong Deok
                    //*cx = 3*x / (2.*x - 8*y + 4);
                    //*cy = 3*y / 2.;
                    cx = x;
                    cy = 3 * y / 2;
                    break;
                case 73:
                    cx = (27 * x / 4) / (9 * x / 2 - 12 * y + 9);
                    cy = 3 * y / (9 * x / 2 - 12 * y + 9);
                    break;
                case 76:
                    cx = x;
                    cy = 2 * y / 3;
                    break;
                default:
                    return false;
            }

            return true;
        }

        #region Communication type

        public virtual void CommInit()
        {

        }

        public virtual bool IsACKPackage(object data)
        {
            // Implement it belong to specific detectors
            return false;
        }

        public void CommDeInit()
        {
            if (CommHandler != null && CommHandler.IsConnected())
            {
                CommHandler.L2Stop();
            }
        }

        protected void OnDataReceived(object obj)
        {
            byte[] pkg = obj as byte[];

            // Process package here
            if (CurCMDProcessing != null)
            {
                CurCMDProcessing.RecvData = pkg;
            }
            else
            {
                Log.LogWrite(Globals.LogLv.Debug, "CurCMDProcessing = null");
            }

            // Raise event when match
            RecvDataEvent.Set();
        }

        private void SendData(byte[] data, bool isNeedACK, uint resend)
        {
            if (CommHandler != null && CommHandler.IsConnected())
            {
                if (CommHandler.L2TXAddPkg(data, isNeedACK, resend))
                {
                    //Log.LogWrite(Globals.LogLv.Verbose, CommHandler.cfg.GetName() + " Add pkg", true);
                }
                else
                {
                    //Log.LogWrite(Globals.LogLv.Verbose, CommHandler.cfg.GetName() + " Add pkg", false);
                }
            }
            else
            {
                Log.LogWrite(Globals.LogLv.Information, "Serial port has not opened yet.");
            }
        }


        private void SendDataNWait(byte[] data, bool isNeedACK, uint resend, string sMidString, bool bClearRx)
        {
            if (CommHandler != null && CommHandler.IsConnected())
            {
                if (CommHandler.L2TXAddPkg(data, isNeedACK, resend, sMidString))
                {
                    //Log.LogWrite(Globals.LogLv.Verbose, CommHandler.cfg.GetName() + " Add pkg", true);
                }
                else
                {
                    //Log.LogWrite(Globals.LogLv.Verbose, CommHandler.cfg.GetName() + " Add pkg", false);
                }
            }
            else
            {
                Log.LogWrite(Globals.LogLv.Information, "Serial port has not opened yet.");
            }
        }

        private bool WaitTime(ref ManualResetEvent WaitEvent, int deadTime)
        {
            int time = 0;

            while (deadTime > time)
            {
                if (WaitEvent.WaitOne(10))
                {
                    return true;
                }

                if (m_bBreak || !m_bConnection)
                {
                    return false;
                }
                time += 10;
            }

            return false;
        }

        #endregion
        #region Log function
        public void LogWrite(Globals.LogLv level, string str)
        {
            Log.LogWrite(level, str);
        }

        public void LogWrite(Globals.LogLv level, string str, bool b)
        {
            Log.LogWrite(level, str, b);
        }

        public void LogWrite(int level, string str)
        {
            Log.LogWrite(level, str);
        }
        #endregion

        public override string ToString()
        {
            return m_sName;
        }

        //---------------------------------------------------------------------------
    }

    public class DetectorCmdPkg
    {
        public bool isTXOnly = true;
        public byte[] Data2Send;
        public ManualResetEvent DoneEvent = new ManualResetEvent(false);
        public byte[] RecvData;
        public string sMidString = string.Empty;
        public int timeOutTm = 3000;
        public bool bClearRx = false;
    }
}
