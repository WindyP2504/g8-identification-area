using MitsubishiPLC;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using VTP_Induction.Common;
using VTP_Induction.Device;


namespace VTP_Induction
{
    public abstract class CPLC : CDevice
    {
        public Globals.TDevUtilDeviceConfig m_ptConfig
        {
            get
            {
                return m_ptDeviceConfig;
            }
        }

        public Globals.TPLCConfig m_ptDeviceConfig
        {
            get
            {
                return (Globals.TPLCConfig)GLb.g_tDevCfg.tPLC.Clone();
            }
        }

        public Globals.TCommConfig m_ptCommCfg
        {
            get
            {
                return m_ptConfig.tCommCfg;
            }
        }

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

        protected string p_sNameDevice = string.Empty;
        public string m_sName
        {
            get
            {
                return p_sNameDevice;
            }
        }
        protected int p_nLCISDeviceID = 0;
        public int m_nLCISDeviceID
        {
            get { return p_nLCISDeviceID; }
        }

        //private ActUtlTypeLib.ActUtlTypeClass plc;
        protected PLC plc;

        public int m_nSelectModel = 1;
        public int m_nSelectPos = -1;

        public static int DEFAULT_TIMEOUT = 11000;
        public int m_nTimeOut = 6000;

        protected Globals GLb = Globals.getInstance();
        protected ThreadsManager ThMan = ThreadsManager.GetInstance();
        protected Thread ScanProcess;
        protected bool Terminated = false;
        protected bool m_bBusy = false;
        protected TForm m_Form;

        protected bool[] m_bInputSignal;
        protected bool[] m_bOutputSignal;

        public uint m_nPositionSignal = 0;
        public uint m_nModelSignal = 0;

        public bool m_bBreak;

        public static CPLC instance;

        public List<short> m_nScanSignalX = new List<short>();
        public List<short> m_nScanSignalY = new List<short>();
        public List<short> m_nScanSignalM = new List<short>();

        public short[] nALS01 = new short[30];
        public short[] nALS02 = new short[30];
        public short[] nALS03 = new short[30];

        public short[] nTp01 = new short[30];
        public short[] nTp02 = new short[30];
        public short[] nTp03 = new short[30];

        public short[] nHum01 = new short[30];
        public short[] nHum02 = new short[30];
        public short[] nHum03 = new short[30];

        public short nComunication = 0;
        public short nPosition = 0;
        public short nCountRnR = 0;
        public short nSpeedMotor = 25;

        public CPLC(TForm Form, int nLCISDeviceID)
        {
            m_Form = Form;
            this.p_nLCISDeviceID = nLCISDeviceID;
            this.p_sNameDevice = m_ptCommCfg.sDevName;
        }

        #region Additional functions

        public virtual void StartThread()
        {
            ScanProcess = new Thread(new ThreadStart(Execute));
            ScanProcess.Name = m_sName + " sensor capture thread" + (new Random()).Next(10000);

            ThMan.AddThread(ScanProcess.Name, ScanProcess);

            ScanProcess.Start();
        }

        public virtual void Destroy()
        {
            m_bBreak = true;
            if (ScanProcess != null)
            {
                ScanProcess.Abort();
                ThMan.RemoveThread(ScanProcess.Name);
                ScanProcess = null;
            }
        }
        #endregion

        #region RS232
        protected virtual void CommInit()
        {
            CommDeInit();

            int nSize = m_ptDeviceConfig.m_nInputBlockData * 16;
            m_bInputSignal = new bool[nSize];
            nSize = m_ptDeviceConfig.m_nOutputBlockData * 16;
            m_bOutputSignal = new bool[nSize];

            //Set the value of 'LogicalStationNumber' to the property.

            plc.ActLogicalStationNumber = m_ptDeviceConfig.nStationNumber;
            plc.ActUtlType = true;

            int iReturnCode = -1;                //Return code

            //The Open method is executed.
            iReturnCode = plc.Open();

            if (iReturnCode != 0)
            {
                string sErrorCode = string.Format("0x{0:x8} [HEX]", iReturnCode);

                Exception ex = new Exception("Cannot open PLC error code " + sErrorCode);
                Log.LogWrite(Globals.LogLv.Debug, ex.Message);
                throw ex;
            }
        }

        protected virtual void CommDeInit()
        {
            if (plc != null)
            {
                int iReturnCode = -1;                //Return code
                iReturnCode = plc.Close();
                string sErrorCode = string.Format("0x{0:x8} [HEX]", iReturnCode);
                p_bConnection = false;
            }
        }
        private bool COM_Open()
        {
            string sLog = "{" + m_sName + "::COM_Open} ";
            m_bBreak = false;
            try
            {
                CommInit();
                Log.LogWrite(Globals.LogLv.Information, sLog, true);
            }
            catch (Exception ex)
            {
                Log.LogWrite(Globals.LogLv.Debug, ex.Message);
                Log.LogWrite(Globals.LogLv.Information, sLog, false);
                return false;
            }

            return true;
        }

        protected void COM_Close()
        {
            try
            {
                CommDeInit();
            }
            catch (Exception ex)
            {
                Log.LogWrite(Globals.LogLv.Debug, ex.Message);
            }
        }


        #endregion

        protected virtual void Execute()
        {
            while (!Terminated)
            {
                Thread.Sleep(30);
                /*
                if (m_bStopSignal)
                {
                    m_bStopAck = true;
                    Thread.Sleep(100);
                    continue;
                }*/
                if (m_bBusy)
                {
                    continue;
                }
                if (!m_bConnection)
                {
                    continue;
                }

                PRC_CheckSensor();
                if (Terminated)
                {
                    break;
                }

                if (Terminated)
                {
                    break;
                }

                if (m_Form == null)
                {
                    continue;
                }
            }
        }
        public virtual void PRC_WareHouseParameter()
        {

        }
        public virtual void PRC_CheckSensor()
        {
            try
            {
                int nReturnCode = -1;
                short[] arrDeviceValue = ReadBlockWordPLC(m_ptDeviceConfig.m_sInputModuleAddress, m_ptDeviceConfig.m_nInputBlockData, ref nReturnCode);
                if (nReturnCode == 0)
                {
                    int nIndex = 0;
                    for (int nBlock = 0; nBlock < arrDeviceValue.Length; nBlock++)
                    {
                        for (int nBit = 0; nBit < 16; nBit++)
                        {
                            nIndex = nBlock * 16 + nBit;
                            if ((arrDeviceValue[nBlock] & 1 << nBit) != 0)
                            {
                                m_bInputSignal[nIndex] = true;
                            }
                            else
                            {
                                m_bInputSignal[nIndex] = false;
                            }
                        }
                    }
                }

            }
            catch
            {

            }
            try
            {
                int nReturnCode = -1;
                short[] arrDeviceValue = ReadBlockWordPLC(m_ptDeviceConfig.m_sOutputModuleAddress, m_ptDeviceConfig.m_nOutputBlockData, ref nReturnCode);
                if (nReturnCode == 0)
                {
                    int nIndex = 0;
                    for (int nBlock = 0; nBlock < arrDeviceValue.Length; nBlock++)
                    {
                        for (int nBit = 0; nBit < 16; nBit++)
                        {
                            nIndex = nBlock * 16 + nBit;
                            if ((arrDeviceValue[nBlock] & 1 << nBit) != 0)
                            {
                                m_bOutputSignal[nIndex] = true;
                            }
                            else
                            {
                                m_bOutputSignal[nIndex] = false;
                            }

                        }
                    }
                }
            }
            catch
            {

            }
        }

        public virtual bool Connect()
        {
            bool bRet = false;
            string sLog = "{" + m_sName + "::Connect ";
            if (!m_ptConfig.bActive)
            {
                return false;
            }
            // DEMO MODE====================================================
            if (GLb.g_tSysCfg.bDemoMode)
            {
                bRet = true;
                Log.LogWrite(Globals.LogLv.Information, sLog, bRet);
                return bRet;
            }
            bRet = COM_Open();
            if (!bRet)
            {
                return bRet;
            }
            bRet = IsRemote();
            if (!bRet)
            {
                COM_Close();
                Log.LogWrite(Globals.LogLv.Information, sLog, bRet);
                return bRet;
            }
            p_bConnection = bRet;
            Log.LogWrite(Globals.LogLv.Information, sLog, bRet);
            return bRet;
        }

        public void Disconnect()
        {
            string sLog = "{" + m_sName + "::Disconnect} ";

            COM_Close();
            Log.LogWrite(Globals.LogLv.Information, sLog);
        }
        //---------------------------------------------------------------------------
        protected virtual bool IsRemote()
        {
            string sLog = "{" + m_sName + "::IsRemote} ";
            bool bRet = true;
            PRC_CheckSensor();
            Log.LogWrite(Globals.LogLv.Information, sLog, bRet);

            return bRet;
        }
        //---------------------------------------------------------------------------

        internal virtual bool INPUT_CheckInputSignal(int nIONumber)
        {
            // DEMO MODE====================================================
            if (GLb.g_tSysCfg.bDemoMode)
            {
                return false;
            }

            int nModule = Convert.ToInt32(GLb.g_tInputCfg[nIONumber].nModule);

            int nIndex = nModule * 16 + GLb.g_tInputCfg[nIONumber].nBitIndex;

            if (m_bInputSignal == null || nIONumber >= GLb.g_tInputCfg.Length || GLb.g_tInputCfg[nIONumber].nBitIndex < 0 || nIndex >= m_bInputSignal.Length || nIndex < 0)
            {
                return false;
            }

            if (GLb.g_tInputCfg[nIONumber].nValue != 0)
            {
                return m_bInputSignal[nIndex];
            }
            else
            {
                return !m_bInputSignal[nIndex];
            }
        }

        internal virtual bool OUTPUT_CheckOutputSignal(int nIONumber)
        {
            // DEMO MODE====================================================
            if (GLb.g_tSysCfg.bDemoMode)
            {
                return false;
            }

            int nModule = Convert.ToInt32(GLb.g_tInputCfg[nIONumber].nModule);

            int nIndex = nModule * 16 + GLb.g_tOutputCfg[nIONumber].nBitIndex;

            if (m_bOutputSignal == null || nIONumber >= GLb.g_tOutputCfg.Length || GLb.g_tOutputCfg[nIONumber].nBitIndex < 0 || nIndex >= m_bOutputSignal.Length || nIndex < 0)
            {
                return false;
            }

            if (GLb.g_tOutputCfg[nIONumber].nValue != 0)
            {
                return m_bOutputSignal[nIndex];
            }
            else
            {
                return !m_bOutputSignal[nIndex];
            }
        }

        public bool lockKeyIOSend = false;

        internal bool OutPutSignalNormal(int nOutputNumber, bool bOnOff)
        {
            //if (!lockKeyIOSend)
            //{
            //    lockKeyIOSend = true;
            bool bRet = false;
            bRet = OUTPUT_SetOutputSignal(nOutputNumber, bOnOff);
            //lockKeyIOSend = false;
            return bRet;
            //}
            //else
            //    return false;
        }

        protected virtual bool OUTPUT_SetOutputSignal(int nIONumber, bool bOnOff)
        {
            // DEMO MODE====================================================
            if (GLb.g_tSysCfg.bDemoMode)
            {
                return true;
            }

            if (nIONumber >= GLb.g_tOutputCfg.Length)
            {
                return false;
            }

            bool bOn = bOnOff;
            if (GLb.g_tOutputCfg[nIONumber].nValue == 0)
            {
                bOn = !bOn;
            }

            int nModule = Convert.ToInt32(GLb.g_tInputCfg[nIONumber].nModule);

            int nSelectmodule = nModule;
            int nBit = GLb.g_tOutputCfg[nIONumber].nBitIndex;

            return WriteOutportBit(nSelectmodule, nBit, bOn);
        }

        protected virtual bool WriteOutportBit(int nModuleNo, int nBitNo, bool bOn)
        {
            string sAddress = null;
            string sStartAddress = m_ptDeviceConfig.m_sOutputModuleAddress.ToUpper();
            if (sStartAddress.Contains("D")) // D Register
            {
                sStartAddress = sStartAddress.Replace("D", "");
                int nStartAddress = -1;
                string sBit = nBitNo.ToString("X");
                if (Int32.TryParse(sStartAddress, out nStartAddress))
                {
                    sAddress = "D" + (nStartAddress + nModuleNo).ToString() + "." + sBit;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (WriteOneBitPLC(sAddress, bOn))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region Read Write Data to PLC

        public virtual bool ReadBitPLC(string szDeviceName)
        {
            bool bRet = false;

            return bRet;
        }

        public virtual short ReadOneBitPLC(string szDeviceName, ref int iReturnCode)
        {
            short[] arrDeviceValue = new short[1];		    //Data for 'DeviceValue'
            iReturnCode = plc.ReadDeviceRandom2(szDeviceName, 1, out arrDeviceValue);
            return arrDeviceValue[0];
        }

        protected virtual short ReadOneWordPLC(string szDeviceName, ref int iReturnCode)
        {
            short[] arrDeviceValue = new short[1];		    //Data for 'DeviceValue'
            iReturnCode = plc.ReadDeviceRandom2(szDeviceName, 1, out arrDeviceValue);

            return arrDeviceValue[0];
        }

        //public short[] ReadBlockBitPLC(int nAddress, int iNumberOfBlock, ref int iReturnCode)
        //{
        //    short[] arrDeviceValue = new short[iNumberOfBlock];		    //Data for 'DeviceValue'
        //    string address = "S" + nAddress.ToString();
        //    iReturnCode = plc.ReadDeviceBlock2(address, iNumberOfBlock, out arrDeviceValue[0]);
        //    if (iReturnCode == 0)
        //    {
        //        return arrDeviceValue;
        //    }
        //    else if (iReturnCode == (int)Globals.PLCErrorCode.PortConnectionError)
        //    {
        //        m_bConnection = false;
        //        return null;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        protected virtual short[] ReadBlockWordPLC(string sAddress, int iNumberOfBlock, ref int iReturnCode)
        {
            short[] arrDeviceValue = new short[iNumberOfBlock];		    //Data for 'DeviceValue'
            iReturnCode = plc.ReadDeviceBlock2(sAddress, iNumberOfBlock, out arrDeviceValue);
            //Log.LogWrite(Globals.LogLv.Verbose, "PLC ReadBlockWordPLC" + sAddress);
            if (iReturnCode == 0)
            {
                return arrDeviceValue;
            }
            else if (iReturnCode == (int)Globals.PLCErrorCode.PortConnectionError)
            {
                p_bConnection = false;
                return null;
            }
            else
            {
                return null;
            }

        }
        protected virtual short[] ReadBlockWordPLCS7(string sAddress, int iNumberOfBlock, ref int iReturnCode)
        {
            short[] arrDeviceValue = new short[iNumberOfBlock];		    //Data for 'DeviceValue'
            iReturnCode = plc.ReadDeviceBlock2(sAddress, iNumberOfBlock, out arrDeviceValue);
            //Log.LogWrite(Globals.LogLv.Verbose, "PLC ReadBlockWordPLC" + sAddress);
            if (iReturnCode == 0)
            {
                return arrDeviceValue;
            }
            else if (iReturnCode == (int)Globals.PLCErrorCode.PortConnectionError)
            {
                p_bConnection = false;
                return null;
            }
            else
            {
                return null;
            }

        }
        public virtual bool WriteOneBitPLC(string sAddress, bool bValue)
        {
            short[] arrDeviceValue = new short[1];		    //Data for 'DeviceValue'
            if (bValue)
            {
                arrDeviceValue[0] = 1;
            }
            else
            {
                arrDeviceValue[0] = 0;
            }

            int iReturnCode = plc.WriteDeviceRandom2(sAddress, 1, ref arrDeviceValue);
            if (iReturnCode == 0)
            {
                return true;
            }
            else if (iReturnCode == (int)Globals.PLCErrorCode.PortConnectionError)
            {
                p_bConnection = false;
                return false;
            }
            else
            {
                return false;
            }
        }

        public virtual bool WriteOneWordPLC(string sAddress, short bValue)
        {
            short[] arrDeviceValue = new short[1];		    //Data for 'DeviceValue'
            arrDeviceValue[0] = bValue;
            int iReturnCode = plc.WriteDeviceBlock2(sAddress, 1, ref arrDeviceValue);
            if (iReturnCode == 0)
            {
                return true;
            }
            else if (iReturnCode == (int)Globals.PLCErrorCode.PortConnectionError)
            {
                p_bConnection = false;
                return false;
            }
            else
            {
                return false;
            }
        }
        public virtual bool WriteStringToWordPLC(string sAddress, string sData, int nLenght = 0)
        {
            short[] arrDeviceValue = new short[1];		    //Data for 'DeviceValue'

            if (sData != null && sData != "")
            {
                nLenght = sData.Length;
                arrDeviceValue = new short[nLenght];
                for (int i = 0; i < arrDeviceValue.Length; i++)
                {
                    arrDeviceValue[i] = Convert.ToInt16(sData[i]);
                }
            }
            else
            {
                if (nLenght != 0)
                {
                    arrDeviceValue = new short[nLenght];
                    for (int i = 0; i < arrDeviceValue.Length; i++)
                    {
                        arrDeviceValue[i] = 0;
                    }
                }
            }

            int iReturnCode = plc.WriteDeviceBlock2(sAddress, arrDeviceValue.Length, ref arrDeviceValue);
            if (iReturnCode == 0)
            {
                return true;
            }
            else if (iReturnCode == (int)Globals.PLCErrorCode.PortConnectionError)
            {
                p_bConnection = false;
                return false;
            }
            else
            {
                return false;
            }
        }

        public virtual bool ReadWordToStringPLC(string sAddress, ref string sData, int nLenght)
        {
            sData = string.Empty;
            if (nLenght > 0)
            {
                short[] arrDeviceValue = new short[nLenght];		    //Data for 'DeviceValue'
                int iReturnCode = plc.ReadDeviceBlock2(sAddress, nLenght, out arrDeviceValue);
                if (iReturnCode == 0)
                {
                    // convert arrDeviceValue to sData                    
                    sData = Util.ShortArrayToStrUTF8(arrDeviceValue).Trim('\0');
                    return true;
                }
                else if (iReturnCode == (int)Globals.PLCErrorCode.PortConnectionError)
                {
                    p_bConnection = false;
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool SetTrafficLightByM(int color)
        {
            // Tắt tất cả nếu yêu cầu OFF hoặc color không hợp lệ
            if (color < 0 || color > 2)
            {
                bool ok = true;
                ok &= WriteOneBitPLC("M10", true);
                return ok;
            }

            // One-hot
            bool o = true;
            o &= WriteOneBitPLC("M10", false);
            o &= WriteOneBitPLC("M0", color == 0);
            o &= WriteOneBitPLC("M1", color == 1);
            o &= WriteOneBitPLC("M2", color == 2);
            return o;
        }

        #endregion
    }

}