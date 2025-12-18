using MitsubishiPLC;
using System.Collections.Generic;
using System.Linq;
using VTP_Induction.Common;
using VTP_Induction.Device;

namespace VTP_Induction
{
    public class CPLCMitsubishi : CPLC
    {
        public CPLCMitsubishi(TForm Form, int nLCISDeviceID)
            : base(Form, nLCISDeviceID)
        {
            m_nScanSignalX = new List<short>();
            m_nScanSignalY = new List<short>();
            m_nScanSignalM = new List<short>();

            for (int i = 0; i < m_ptDeviceConfig.m_listAddressScan.Count; i++)
            {
                switch (m_ptDeviceConfig.m_listAddressScan[i].nCommType)
                {
                    case Globals.TEnumPLCAddressType.Input:
                        break;
                    case Globals.TEnumPLCAddressType.Output:
                        break;
                    case Globals.TEnumPLCAddressType.Monitor:
                        break;
                    case Globals.TEnumPLCAddressType.X:
                        {
                            for (int j = 0; j < m_ptDeviceConfig.m_listAddressScan[i].nModule; j++)
                            {
                                m_nScanSignalX.Add(0);
                            }
                        }
                        break;
                    case Globals.TEnumPLCAddressType.Y:
                        {
                            for (int j = 0; j < m_ptDeviceConfig.m_listAddressScan[i].nModule; j++)
                            {
                                m_nScanSignalY.Add(0);
                            }
                        }
                        break;
                    case Globals.TEnumPLCAddressType.M:
                        {
                            for (int j = 0; j < m_ptDeviceConfig.m_listAddressScan[i].nModule; j++)
                            {
                                m_nScanSignalM.Add(0);
                            }
                        }
                        break;
                    case Globals.TEnumPLCAddressType.Communication:
                        break;
                    default:
                        break;
                }
            }
            try
            {
                plc = new PLC();
            }
            catch (System.Exception ex)
            {
                Log.LogWrite(Globals.LogLv.Warning, ex.Message);
                Log.LogWrite(Globals.LogLv.Verbose, ex);
            }
        }

        public override void PRC_WareHouseParameter()
        {

            for (int i = 0; i < m_ptDeviceConfig.m_listWarehouseParameterAddressScan.Count(); i++)
            {
                int iReturnCode = -1;
                //Return code
                nALS01[i] = ReadOneWordPLC(m_ptDeviceConfig.m_listWarehouseParameterAddressScan[i].sALS01_Name, ref iReturnCode);
                nALS02[i] = ReadOneWordPLC(m_ptDeviceConfig.m_listWarehouseParameterAddressScan[i].sALS02_Name, ref iReturnCode);
                nALS03[i] = ReadOneWordPLC(m_ptDeviceConfig.m_listWarehouseParameterAddressScan[i].sALS03_Name, ref iReturnCode);

                nTp01[i] = ReadOneWordPLC(m_ptDeviceConfig.m_listWarehouseParameterAddressScan[i].sTp01_Name, ref iReturnCode);
                nTp02[i] = ReadOneWordPLC(m_ptDeviceConfig.m_listWarehouseParameterAddressScan[i].sTp02_Name, ref iReturnCode);
                nTp03[i] = ReadOneWordPLC(m_ptDeviceConfig.m_listWarehouseParameterAddressScan[i].sTp03_Name, ref iReturnCode);

                nHum01[i] = ReadOneWordPLC(m_ptDeviceConfig.m_listWarehouseParameterAddressScan[i].sHum01_Name, ref iReturnCode);
                nHum02[i] = ReadOneWordPLC(m_ptDeviceConfig.m_listWarehouseParameterAddressScan[i].sHum02_Name, ref iReturnCode);
                nHum03[i] = ReadOneWordPLC(m_ptDeviceConfig.m_listWarehouseParameterAddressScan[i].sHum03_Name, ref iReturnCode);
            }
        }

        public override void PRC_CheckSensor()
        {
            int nReturnCode = -1;
            for (int i = 0; i < m_ptDeviceConfig.m_listAddressScan.Count; i++)
            {
                switch (m_ptDeviceConfig.m_listAddressScan[i].nCommType)
                {
                    case Globals.TEnumPLCAddressType.Input:
                        break;
                    case Globals.TEnumPLCAddressType.Output:
                        break;
                    case Globals.TEnumPLCAddressType.X:
                        {
                            short[] arrDeviceValue = ReadBlockWordPLC(m_ptDeviceConfig.m_listAddressScan[i].sAddress, m_ptDeviceConfig.m_listAddressScan[i].nModule, ref nReturnCode);
                            if (nReturnCode == 0)
                            {
                                int nLenght = m_nScanSignalX.Count < arrDeviceValue.Length ? m_nScanSignalX.Count : arrDeviceValue.Length;
                                for (int nBlock = 0; nBlock < nLenght; nBlock++)
                                {
                                    m_nScanSignalX[nBlock] = arrDeviceValue[nBlock];
                                }
                            }
                        }
                        break;
                    case Globals.TEnumPLCAddressType.Y:
                        {
                            short[] arrDeviceValue = ReadBlockWordPLC(m_ptDeviceConfig.m_listAddressScan[i].sAddress, m_ptDeviceConfig.m_listAddressScan[i].nModule, ref nReturnCode);
                            if (nReturnCode == 0)
                            {
                                int nLenght = m_nScanSignalY.Count < arrDeviceValue.Length ? m_nScanSignalY.Count : arrDeviceValue.Length;
                                for (int nBlock = 0; nBlock < nLenght; nBlock++)
                                {
                                    m_nScanSignalY[nBlock] = arrDeviceValue[nBlock];
                                }
                            }
                        }
                        break;
                    case Globals.TEnumPLCAddressType.M:
                        {
                            short[] arrDeviceValue = ReadBlockWordPLC(m_ptDeviceConfig.m_listAddressScan[i].sAddress, m_ptDeviceConfig.m_listAddressScan[i].nModule, ref nReturnCode);
                            if (nReturnCode == 0)
                            {
                                int nLenght = m_nScanSignalM.Count < arrDeviceValue.Length ? m_nScanSignalM.Count : arrDeviceValue.Length;
                                for (int nBlock = 0; nBlock < nLenght; nBlock++)
                                {
                                    m_nScanSignalM[nBlock] = arrDeviceValue[nBlock];
                                }
                            }
                        }
                        break;
                    case Globals.TEnumPLCAddressType.Communication:
                        {
                            int iReturnCode = -1;                //Return code
                            nComunication = ReadOneWordPLC(m_ptDeviceConfig.m_listAddressScan[i].sAddress, ref iReturnCode);
                        }
                        break;
                    case Globals.TEnumPLCAddressType.Position:
                        {
                            int iReturnCode = -1;                //Return code
                            nPosition = ReadOneWordPLC(m_ptDeviceConfig.m_listAddressScan[i].sAddress, ref iReturnCode);
                        }
                        break;
                    case Globals.TEnumPLCAddressType.CountRnR:
                        {
                            int iReturnCode = -1;                //Return code
                            nCountRnR = ReadOneWordPLC(m_ptDeviceConfig.m_listAddressScan[i].sAddress, ref iReturnCode);
                        }
                        break;
                    case Globals.TEnumPLCAddressType.SpeedMotor:
                        {
                            int iReturnCode = -1;                //Return code
                            nSpeedMotor = ReadOneWordPLC(m_ptDeviceConfig.m_listAddressScan[i].sAddress, ref iReturnCode);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public override bool ReadBitPLC(string szDeviceName)
        {
            bool bRet = false;
            int nIndexStart = 0;
            int nIndexRead = 0;
            if (szDeviceName.Contains("X"))
            {
                foreach (var item in m_ptDeviceConfig.m_listAddressScan)
                {
                    if (item.nCommType == Globals.TEnumPLCAddressType.X)
                    {
                        nIndexStart = int.Parse(item.sAddress.Replace("X", ""), System.Globalization.NumberStyles.HexNumber);
                        break;
                    }
                }
                nIndexRead = int.Parse(szDeviceName.Replace("X", ""), System.Globalization.NumberStyles.HexNumber);
                int nBlock = (nIndexRead - nIndexStart) / 16;
                int nBit = (nIndexRead - nIndexStart) % 16;
                bRet = (m_nScanSignalX[nBlock] & 1 << nBit) != 0;
            }
            else if (szDeviceName.Contains("M"))
            {
                foreach (var item in m_ptDeviceConfig.m_listAddressScan)
                {
                    if (item.nCommType == Globals.TEnumPLCAddressType.M)
                    {
                        nIndexStart = int.Parse(item.sAddress.Replace("M", ""));
                        break;
                    }
                }
                nIndexRead = int.Parse(szDeviceName.Replace("M", ""));
                int nBlock = (nIndexRead - nIndexStart) / 16;
                int nBit = (nIndexRead - nIndexStart) % 16;
                bRet = (m_nScanSignalM[nBlock] & 1 << nBit) != 0;
            }
            else if (szDeviceName.Contains("Y"))
            {
                foreach (var item in m_ptDeviceConfig.m_listAddressScan)
                {
                    if (item.nCommType == Globals.TEnumPLCAddressType.Y)
                    {
                        nIndexStart = int.Parse(item.sAddress.Replace("Y", ""), System.Globalization.NumberStyles.HexNumber);
                        break;
                    }
                }
                nIndexRead = int.Parse(szDeviceName.Replace("Y", ""), System.Globalization.NumberStyles.HexNumber);
                int nBlock = (nIndexRead - nIndexStart) / 16;
                int nBit = (nIndexRead - nIndexStart) % 16;
                bRet = (m_nScanSignalY[nBlock] & 1 << nBit) != 0;
            }
            return bRet;
        }

        internal override bool INPUT_CheckInputSignal(int nIONumber)
        {
            // DEMO MODE====================================================
            if (GLb.g_tSysCfg.bDemoMode)
            {
                return false;
            }

            string szDeviceName = GLb.g_tInputCfg[nIONumber].nModule;

            bool bRet = false;
            int nIndexStart = 0;
            int nIndexRead = 0;
            if (szDeviceName.Contains("X"))
            {
                foreach (var item in m_ptDeviceConfig.m_listAddressScan)
                {
                    if (item.nCommType == Globals.TEnumPLCAddressType.X)
                    {
                        nIndexStart = int.Parse(item.sAddress.Replace("X", ""), System.Globalization.NumberStyles.HexNumber);
                        break;
                    }
                }
                nIndexRead = int.Parse(szDeviceName.Replace("X", ""), System.Globalization.NumberStyles.HexNumber);
                int nBlock = (nIndexRead - nIndexStart) / 16;
                int nBit = (nIndexRead - nIndexStart) % 16;
                if (nBlock >= 0)
                {
                    bRet = (m_nScanSignalX[nBlock] & 1 << nBit) != 0;
                }
                else
                {
                    bRet = false;
                }
            }
            else if (szDeviceName.Contains("M"))
            {
                foreach (var item in m_ptDeviceConfig.m_listAddressScan)
                {
                    if (item.nCommType == Globals.TEnumPLCAddressType.M)
                    {
                        nIndexStart = int.Parse(item.sAddress.Replace("M", ""));
                        break;
                    }
                }
                nIndexRead = int.Parse(szDeviceName.Replace("M", ""));
                int nBlock = (nIndexRead - nIndexStart) / 16;
                int nBit = (nIndexRead - nIndexStart) % 16;
                if (nBlock >= 0)
                {
                    bRet = (m_nScanSignalM[nBlock] & 1 << nBit) != 0;
                }
                else
                {
                    bRet = false;
                }
            }
            else if (szDeviceName.Contains("Y"))
            {
                foreach (var item in m_ptDeviceConfig.m_listAddressScan)
                {
                    if (item.nCommType == Globals.TEnumPLCAddressType.Y)
                    {
                        nIndexStart = int.Parse(item.sAddress.Replace("Y", ""), System.Globalization.NumberStyles.HexNumber);
                        break;
                    }
                }
                nIndexRead = int.Parse(szDeviceName.Replace("Y", ""), System.Globalization.NumberStyles.HexNumber);
                int nBlock = (nIndexRead - nIndexStart) / 16;
                int nBit = (nIndexRead - nIndexStart) % 16;
                if (nBlock >= 0)
                {
                    bRet = (m_nScanSignalY[nBlock] & 1 << nBit) != 0;
                }
                else
                {
                    bRet = false;
                }
            }

            if (GLb.g_tInputCfg[nIONumber].nValue == 1)
            {
                return bRet;
            }
            else
            {
                return !bRet;
            }
        }

        internal override bool OUTPUT_CheckOutputSignal(int nIONumber)
        {
            // DEMO MODE====================================================
            if (GLb.g_tSysCfg.bDemoMode)
            {
                return false;
            }

            string szDeviceName = GLb.g_tOutputCfg[nIONumber].nModule;

            bool bRet = false;
            int nIndexStart = 0;
            int nIndexRead = 0;
            if (szDeviceName.Contains("X"))
            {
                foreach (var item in m_ptDeviceConfig.m_listAddressScan)
                {
                    if (item.nCommType == Globals.TEnumPLCAddressType.X)
                    {
                        nIndexStart = int.Parse(item.sAddress.Replace("X", ""), System.Globalization.NumberStyles.HexNumber);
                        break;
                    }
                }
                nIndexRead = int.Parse(szDeviceName.Replace("X", ""), System.Globalization.NumberStyles.HexNumber);
                int nBlock = (nIndexRead - nIndexStart) / 16;
                int nBit = (nIndexRead - nIndexStart) % 16;
                bRet = (m_nScanSignalX[nBlock] & 1 << nBit) != 0;
            }
            else if (szDeviceName.Contains("M"))
            {
                foreach (var item in m_ptDeviceConfig.m_listAddressScan)
                {
                    if (item.nCommType == Globals.TEnumPLCAddressType.M)
                    {
                        nIndexStart = int.Parse(item.sAddress.Replace("M", ""));
                        break;
                    }
                }
                nIndexRead = int.Parse(szDeviceName.Replace("M", ""));
                int nBlock = (nIndexRead - nIndexStart) / 16;
                int nBit = (nIndexRead - nIndexStart) % 16;
                bRet = (m_nScanSignalM[nBlock] & 1 << nBit) != 0;
            }
            else if (szDeviceName.Contains("Y"))
            {
                foreach (var item in m_ptDeviceConfig.m_listAddressScan)
                {
                    if (item.nCommType == Globals.TEnumPLCAddressType.Y)
                    {
                        nIndexStart = int.Parse(item.sAddress.Replace("Y", ""), System.Globalization.NumberStyles.HexNumber);
                        break;
                    }
                }
                nIndexRead = int.Parse(szDeviceName.Replace("Y", ""), System.Globalization.NumberStyles.HexNumber);
                int nBlock = (nIndexRead - nIndexStart) / 16;
                int nBit = (nIndexRead - nIndexStart) % 16;
                bRet = (m_nScanSignalY[nBlock] & 1 << nBit) != 0;
            }

            if (GLb.g_tOutputCfg[nIONumber].nValue == 1)
            {
                return bRet;
            }
            else
            {
                return !bRet;
            }
        }

        protected override bool OUTPUT_SetOutputSignal(int nIONumber, bool bOnOff)
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

            return WriteOneBitPLC(GLb.g_tOutputCfg[nIONumber].nModule, bOn);
        }
    }
}
