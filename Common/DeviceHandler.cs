using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using VTP_Induction.Common;
using VTP_Induction.Device;

namespace VTP_Induction
{
    public class DeviceHandler
    {
        private Globals GLb = Globals.getInstance();

        private ThreadsManager ThMan = ThreadsManager.GetInstance();

        private Thread DevProcessCmdThread;
        private ManualResetEvent AddDevEvent = new ManualResetEvent(false);
        private Queue<DeviceEvent> DevEventList = new Queue<DeviceEvent>();

        private Thread DevProcessCmdThreadSub1;
        private ManualResetEvent AddDevEventSub1 = new ManualResetEvent(false);
        private Queue<DeviceEvent> DevEventListSub1 = new Queue<DeviceEvent>();
        private List<MacroFunc> listThreadSub1 = new List<MacroFunc>();

        private Thread DevProcessCmdThreadSub2;
        private ManualResetEvent AddDevEventSub2 = new ManualResetEvent(false);
        private Queue<DeviceEvent> DevEventListSub2 = new Queue<DeviceEvent>();
        private List<MacroFunc> listThreadSub2 = new List<MacroFunc>();

        private Thread DevProcessCmdThreadSub3;
        private ManualResetEvent AddDevEventSub3 = new ManualResetEvent(false);
        private Queue<DeviceEvent> DevEventListSub3 = new Queue<DeviceEvent>();
        private List<MacroFunc> listThreadSub3 = new List<MacroFunc>();

        private bool bSeverPVIsOpen = false;
        public bool SeverPVIsOpen
        {
            get { return bSeverPVIsOpen; }
        }

        public BindingList<CDetector> detectorHandler = new BindingList<CDetector>();

        public CPLC cPLCHandler;

        public CPrinterApos cPrinter;
        public CScale cScale;
        public CBarcode cBarcode;
        public WMS cWMS;
        public WMS cRFID;
        public CPrinterGodex cPrinterGodex;

        private TForm parentForm;

        public string activeDetectorName = string.Empty;

        public DeviceHandler(TForm form)
        {
            parentForm = form;

            //StopAllDev(form, true);
            StartAllDev(true);
        }

        public bool lastIOMeasCome = false;
        public Bitmap m_Bitmap = null;

        //public void IOBarcodeScanHandleFunc_Old()
        //{
        //    bool bTemp = CIOInterface.bBarcodeSignal;

        //    if (lastIOBarcodeCome != bTemp)
        //    {
        //        lastIOBarcodeCome = bTemp;
        //        if (bTemp)
        //        {
        //            //Start Barcode read
        //            new Thread(() =>
        //            {
        //                lastCheckBarcode = DateTime.Now;
        //                Log.LogWrite(Globals.LogLv.Information, "CHECK BARCODE>>>");
        //                if (cBarcode.Count < GLb.g_tModel.nBarcodeIdx)
        //                {
        //                    string sTemp = cBarcode[GLb.g_tModel.nBarcodeIdx].SendCMD();

        //                    if (sTemp != string.Empty)
        //                    {
        //                        nCurrentBacodeIndex = GLb.g_tModel.nBarcodeIdx;
        //                    }
        //                    else
        //                    {
        //                        nCurrentBacodeIndex = -1;
        //                    }

        //                    prioBarcode = GLb.g_tModel.nBarcodeIdx;
        //                }
        //            }).Start();
        //        }
        //    }
        //   // else if (CIOInterface.bBarcodeSignal && GLb.SeqMode == UI.E_SEQ_MODE.FREE_MODE && cBarcode[GLb.g_tModel.nBarcodeIdx].GLb.g_sBarcodeQueue.Count == 0 /*&& lastCheckBarcode != null*/
        //    else if (CIOInterface.bBarcodeSignal && GLb.SeqMode == UI.E_SEQ_MODE.FREE_MODE && GLb.g_sBarcodeQueue.Count == 0 /*&& lastCheckBarcode != null*/)
        //    {
        //        DateTime newDate = DateTime.Now;

        //        double dL = 0;

        //        if (lastCheckBarcode != null)
        //        {
        //            dL = (newDate - lastCheckBarcode).TotalSeconds;
        //        }
        //        else
        //        {
        //            dL = 5;
        //        }

        //        if (dL >= 4)
        //        {
        //            //Start Barcode read
        //            new Thread(() =>
        //            {
        //                lastCheckBarcode = DateTime.Now;
        //                Log.LogWrite(Globals.LogLv.Information, "CHECK BARCODE>>>");
        //                if (GLb.g_tSysCfg.bAutoChangeModel)
        //                {
        //                    if (cBarcode.Count < prioBarcode)
        //                    {
        //                        string sTemp = cBarcode[prioBarcode].SendCMD();
        //                        if (sTemp != string.Empty)
        //                        {
        //                            GLb.g_tModel.nBarcodeIdx = prioBarcode;
        //                            nCurrentBacodeIndex = GLb.g_tModel.nBarcodeIdx;
        //                        }
        //                        else
        //                        {
        //                            nCurrentBacodeIndex = -1;
        //                        }

        //                        prioBarcode++;

        //                        if (prioBarcode >= cBarcode.Count)
        //                        {
        //                            prioBarcode = 0;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (cBarcode.Count < GLb.g_tModel.nBarcodeIdx)
        //                    {
        //                        string sTemp = cBarcode[GLb.g_tModel.nBarcodeIdx].SendCMD();

        //                        if (sTemp != string.Empty)
        //                        {
        //                            nCurrentBacodeIndex = GLb.g_tModel.nBarcodeIdx;
        //                        }
        //                        else
        //                        {
        //                            nCurrentBacodeIndex = -1;
        //                        }
        //                    }
        //                }

        //            }).Start();
        //        }
        //    }

        //    lastIOBarcodeCome = bTemp;

        //}

        //public void IOBarcodeScanHandleFunc()
        //{
        //    bool bTemp = CIOInterface.bBarcodeSignal;

        //    if (lastIOBarcodeCome != bTemp)
        //    {
        //        lastIOBarcodeCome = bTemp;
        //        if (bTemp)
        //        {
        //            //Start Barcode read
        //            new Thread(() =>
        //            {
        //                lastCheckBarcode = DateTime.Now;
        //                Log.LogWrite(Globals.LogLv.Information, "CHECK BARCODE>>>");

        //                if (GLb.g_tSysCfg.bAutoChangeModel)
        //                {
        //                    for (int i = 0; i < cBarcode.Count; i++)
        //                    {
        //                        cBarcode[i].TriggerOn();
        //                    }
        //                }
        //                else
        //                {
        //                    string sTemp = cBarcode[GLb.g_tModel.nBarcodeIdx].SendCMD();
        //                }

        //            }).Start();
        //        }
        //    }
        //    // else if (CIOInterface.bBarcodeSignal && GLb.SeqMode == UI.E_SEQ_MODE.FREE_MODE && cBarcode[GLb.g_tModel.nBarcodeIdx].GLb.g_sBarcodeQueue.Count == 0 /*&& lastCheckBarcode != null*/
        //    else if (CIOInterface.bBarcodeSignal && GLb.SeqMode == UI.E_SEQ_MODE.FREE_MODE && GLb.g_sBarcodeQueue.Count == 0 /*&& lastCheckBarcode != null*/)
        //    {
        //        DateTime newDate = DateTime.Now;
        //        double dWaitTimeBarcode = 0;

        //        if (lastCheckBarcode != null)
        //        {
        //            dWaitTimeBarcode = (newDate - lastCheckBarcode).TotalSeconds;
        //        }
        //        else
        //        {
        //            dWaitTimeBarcode = 5;
        //        }

        //        // Scan each 4 seconds if cannot read barcode
        //        if (dWaitTimeBarcode >= 4)
        //        {
        //            //Start Barcode read
        //            new Thread(() =>
        //            {
        //                lastCheckBarcode = DateTime.Now;
        //                Log.LogWrite(Globals.LogLv.Information, "CHECK BARCODE>>>");
        //                if (GLb.g_tSysCfg.bAutoChangeModel)
        //                {
        //                    for (int i = 0; i < cBarcode.Count; i++)
        //                    {
        //                        cBarcode[i].TriggerOn();
        //                    }
        //                }
        //                else
        //                {
        //                    if (cBarcode.Count < GLb.g_tModel.nBarcodeIdx)
        //                    {
        //                        string sTemp = cBarcode[GLb.g_tModel.nBarcodeIdx].SendCMD();
        //                    }
        //                }

        //            }).Start();
        //        }
        //    }

        //    lastIOBarcodeCome = bTemp;
        //}

        internal CDevice GetDeviceHandler(int nLCISDeviceID)
        {
            Globals.TDevUtilDeviceConfig dev = GLb.g_tDevCfg.tDeviceList.Find(x =>
                x.nLCISDeviceID == nLCISDeviceID
            );
            if (dev != null)
            {
                switch (dev.nDeviceType)
                {
                    case Globals.TEnumUtilDevice.BARCODEMANAGER:
                    //return cBarcodeManagerHandler;

                    case Globals.TEnumUtilDevice.PLC:
                        if (cPLCHandler != null)
                        {
                            return cPLCHandler;
                        }
                        else
                        {
                            return null;
                        }
                    case Globals.TEnumUtilDevice.API:
                        if (cWMS != null)
                        {
                            return cWMS;
                        }
                        else
                        {
                            return null;
                        }
                    case Globals.TEnumUtilDevice.PRINTER:

                        if (cPrinter != null)
                        {
                            return cPrinter;
                        }
                        else
                        {
                            return null;
                        }
                    case Globals.TEnumUtilDevice.BARCODE:

                        if (cBarcode != null)
                        {
                            return cBarcode;
                        }
                        else
                        {
                            return null;
                        }
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
        }

        public void StopAllDev(bool bAll = true)
        {
            Log.LogWrite(Globals.LogLv.Debug, "StopAllDev!");
            DevThreadStop();
            // Destroy Detector Cam for next connect
            foreach (CDetector pt in detectorHandler)
            {
                pt.Disconnect();
                pt.DestroyCDetector();
            }
            detectorHandler.Clear();

            //Destroy CBarcode

            //Destroy IO

            if (cPLCHandler != null)
            {
                cPLCHandler.Disconnect();
                cPLCHandler.Destroy();
            }
            if (cPrinter != null)
            {
                cPrinter.Disconnect();
            }
            if (cBarcode != null)
            {
                cBarcode.Disconnect();
            }
            if (cScale != null)
            {
                cScale.Disconnect();
            }
            if(cPrinterGodex != null)
            {
                cPrinterGodex.Disconnect();
            }
        }

        public void StartAllDev(bool bAll = true)
        {
            // Add PLC
            if (GLb.g_tDevCfg.tPLC.bActive || true)
            {
                Globals.TPLCConfig devConfig = GLb.g_tDevCfg.tPLC;
                cPLCHandler = new CPLCMitsubishi(parentForm, devConfig.nLCISDeviceID);
            }
            if (GLb.g_tDevCfg.tPRINTER.bActive || true)
            {
                Globals.TPrinterConfig PrinterConfig = GLb.g_tDevCfg.tPRINTER;
                cPrinter = new CPrinterApos(parentForm);
            }
            if (GLb.g_tDevCfg.tScale.bActive || true)
            {
                Globals.TScaleConfig ScaleConfig = GLb.g_tDevCfg.tScale;
                cScale = new CScale(parentForm);
            }
            if (GLb.g_tDevCfg.tBarcode.bActive || true)
            {
                Globals.TBarcodeConfig BarcodeConfig = GLb.g_tDevCfg.tBarcode;
                cBarcode = new CBarcode(parentForm);
            }
            if (GLb.g_tDevCfg.tWMS.bActive)
            {
                Globals.TWMSConfig WmsConfig = GLb.g_tDevCfg.tWMS;
                cWMS = new WMS(0);
            }

            if (GLb.g_tDevCfg.tRFID.bActive)
            {
                Globals.RFIDConfig RFIDConfig = GLb.g_tDevCfg.tRFID;
                cRFID = new WMS(0);
            }

            if (GLb.g_tDevCfg.tPrinterGodex.bActive || true)
            {
                Globals.TPrinterGodex printerGodex = GLb.g_tDevCfg.tPrinterGodex;
                cPrinterGodex = new CPrinterGodex(parentForm);
            }

            // Add Camera HIK
            for (int i = 0; i < GLb.g_tDevCfg.tHIK.Length; i++)
            {
                if (GetDetectorHandler(GLb.g_tDevCfg.tHIK[i].sDevName) == null)
                {
                    //HIKScannerBarcode dt = new HIKScannerBarcode(parentForm, i);
                    //detectorHandler.Add(dt); //HIK
                }
            }

            if (cPLCHandler != null)
            {
                cPLCHandler.StartThread();
            }

            parentForm.Text = GLb.g_SoftwareNameVersion;

            //Start Detector thread
            foreach (CDetector dt in detectorHandler)
            {
                try
                {
                    dt.StartDetector();
                }
                catch (System.Exception ex)
                {
                    Log.LogWrite(Globals.LogLv.Debug, ex.Message);
                }
            }

            DevThreadStart();
        }

        public void DevConnect(bool bAll = true)
        {
            //Log.LogWrite(Globals.LogLv.Debug, "DevConnect!");
            try
            {
                foreach (CDetector d in detectorHandler)
                {
                    if (!d.m_bConnection)
                    {
                        d.Connect();
                    }
                }

                if (cPLCHandler != null)
                {
                    cPLCHandler.Connect();
                }
                if (cWMS != null)
                {
                    cWMS.Connect();
                }
                if (cPrinter != null)
                {
                    cPrinter.Connect();
                }
                if (cScale != null)
                {
                    cScale.Connect();
                }
                if (cBarcode != null)
                {
                    cBarcode.Connect();
                }
                if(cPrinterGodex != null)
                {
                    cPrinterGodex.Connect();
                }
            }
            catch (System.Exception ex)
            {
                Log.LogWrite(Globals.LogLv.Debug, ex.Message);
            }
        }

        public CDetector GetDetectorHandler(string name)
        {
            if (detectorHandler == null || detectorHandler.Count == 0)
            {
                return null;
            }

            CDetector dt = null;

            try
            {
                dt = detectorHandler.First(x => x.m_sName == name);
            }
            catch /*(System.Exception ex)*/
            {
                //if not found
                dt = null;
            }

            return dt;
        }

        public void RemoveAllDetectorFromList(bool bAll)
        {
            try
            {
                for (int i = detectorHandler.Count - 1; i >= 0; i--)
                {
                    CDetector d = detectorHandler[i];

                    d.DestroyCDetector();
                    detectorHandler.Remove(d);
                }
            }
            catch (System.Exception ex)
            {
                Log.LogWrite(Globals.LogLv.Information, ex.Message);
            }

            //detectorHandler.Clear();
        }

        public bool RemoveDetectorFromList(string name)
        {
            if (name != null)
            {
                CDetector temp = null;
                try
                {
                    temp = detectorHandler.First(x => x.m_sName == name);
                }
                catch /*(System.Exception ex)*/
                {
                    // If not found
                    temp = null;
                }

                if (temp == null)
                {
                    return false;
                }

                temp.DestroyCDetector();

                detectorHandler.Remove(temp);

                return true;
            }
            else
            {
                return false;
            }
        }

        public void DevThreadStart()
        {
            DevProcessCmdThread = new Thread(new ThreadStart(ProcessCmdExe));
            DevProcessCmdThread.Name = "Device handler thread";
            ThMan.AddThread(DevProcessCmdThread.Name, DevProcessCmdThread);
            DevProcessCmdThread.Start();

            DevProcessCmdThreadSub1 = new Thread(new ThreadStart(ProcessCmdExeSub1));
            DevProcessCmdThreadSub1.Name = "Device handler thread Sub1";
            ThMan.AddThread(DevProcessCmdThreadSub1.Name, DevProcessCmdThreadSub1);
            DevProcessCmdThreadSub1.Start();

            DevProcessCmdThreadSub2 = new Thread(new ThreadStart(ProcessCmdExeSub2));
            DevProcessCmdThreadSub2.Name = "Device handler thread Sub2";
            ThMan.AddThread(DevProcessCmdThreadSub2.Name, DevProcessCmdThreadSub2);
            DevProcessCmdThreadSub2.Start();

            DevProcessCmdThreadSub3 = new Thread(new ThreadStart(ProcessCmdExeSub3));
            DevProcessCmdThreadSub3.Name = "Device handler thread Sub3";
            ThMan.AddThread(DevProcessCmdThreadSub3.Name, DevProcessCmdThreadSub3);
            DevProcessCmdThreadSub3.Start();
        }

        public void DevThreadStop()
        {
            if (DevProcessCmdThread != null)
            {
                DevProcessCmdThread.Abort();
                ThMan.RemoveThread(DevProcessCmdThread.Name);
                DevProcessCmdThread = null;
            }

            if (DevProcessCmdThreadSub1 != null)
            {
                DevProcessCmdThreadSub1.Abort();
                ThMan.RemoveThread(DevProcessCmdThreadSub1.Name);
                DevProcessCmdThreadSub1 = null;
            }

            if (DevProcessCmdThreadSub2 != null)
            {
                DevProcessCmdThreadSub2.Abort();
                ThMan.RemoveThread(DevProcessCmdThreadSub2.Name);
                DevProcessCmdThreadSub2 = null;
            }

            if (DevProcessCmdThreadSub3 != null)
            {
                DevProcessCmdThreadSub3.Abort();
                ThMan.RemoveThread(DevProcessCmdThreadSub3.Name);
                DevProcessCmdThreadSub3 = null;
            }
        }

        private void ProcessCmdExe()
        {
            //while (true)
            //{
            //    if (DevEventList.Count > 0 || AddDevEvent.WaitOne(TIMEOUT))
            //    {
            //        lock (DevEventList)
            //        {
            //            CurCMDProcessing = DevEventList.Dequeue();

            //            //Processing_CMD(CurCMDProcessing);
            //        }
            //    }

            //    AddDevEvent.Reset();
            //} // End while

            //while ( true)
            //{

            //}

            //foreach( var item in detectorHandler)
            //{
            //    item.CameraTriggerFromOutSignal();
            //}
        }

        private void ProcessCmdExeSub1()
        {
            //while (true)
            //{
            //    if (DevEventListSub1.Count > 0 || AddDevEventSub1.WaitOne(TIMEOUT))
            //    {
            //        lock (DevEventListSub1)
            //        {
            //            DeviceEvent CMDProcessing = DevEventListSub1.Dequeue();
            //            //Processing_CMD(CMDProcessing);
            //            // add result to list
            //            MacroFunc m = CMDProcessing.data0 as MacroFunc;
            //            listThreadSub1.Add(m);
            //            bThreadSubRet_1 = true;
            //            //Log.LogWrite(Globals.LogLv.Information, m.DisplayName, m.bRes);
            //        }
            //    }

            //    AddDevEventSub1.Reset();
            //} // End while
        }

        private void ProcessCmdExeSub2()
        {
            //while (true)
            //{
            //    if (DevEventListSub2.Count > 0 || AddDevEventSub2.WaitOne(TIMEOUT))
            //    {
            //        lock (DevEventListSub2)
            //        {
            //            DeviceEvent CMDProcessing = DevEventListSub2.Dequeue();

            //            //Processing_CMD(CMDProcessing);
            //            // add result to list
            //            MacroFunc m = CMDProcessing.data0 as MacroFunc;
            //            listThreadSub2.Add(m);
            //            bThreadSubRet_2 = true;
            //            //Log.LogWrite(Globals.LogLv.Information, m.DisplayName, m.bRes);
            //        }
            //    }

            //    AddDevEventSub2.Reset();
            //} // End while
        }

        private void ProcessCmdExeSub3()
        {
            //while (true)
            //{
            //    if (DevEventListSub3.Count > 0 || AddDevEventSub3.WaitOne(TIMEOUT))
            //    {
            //        lock (DevEventListSub3)
            //        {
            //            DeviceEvent CMDProcessing = DevEventListSub3.Dequeue();

            //            //Processing_CMD(CMDProcessing);
            //            // add result to list
            //            MacroFunc m = CMDProcessing.data0 as MacroFunc;
            //            listThreadSub3.Add(m);
            //            bThreadSubRet_3 = true;
            //            //Log.LogWrite(Globals.LogLv.Information, m.DisplayName, m.bRes);
            //        }
            //    }
            //    AddDevEventSub3.Reset();
            //} // End while
        }

        public void ResetAllSubThread()
        {
            AddDevEventSub1.Reset();
            AddDevEventSub2.Reset();
            AddDevEventSub3.Reset();
            DevEventListSub1.Clear();
            DevEventListSub2.Clear();
            DevEventListSub3.Clear();
            listThreadSub1.Clear();
            listThreadSub2.Clear();
            listThreadSub3.Clear();
        }

        private void ClearAllFileInDirectory(string path)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(path);

            //Clear all files
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            //Clear all sub-folders
            //foreach (DirectoryInfo dir in di.GetDirectories())
            //{
            //    dir.Delete(true);
            //}
        }

        // Load a bitmap without locking it.
        private Bitmap LoadBitmapUnlocked(string file_name)
        {
            try
            {
                using (Bitmap bm = new Bitmap(file_name))
                {
                    return new Bitmap(bm);
                }
            }
            catch { }
            return null;
        }

        //protected virtual string BasicCmd(MacroFunc m)
        //{
        //    int nTimeOut = m.n_timeout;
        //    bool bRet = false;
        //    string retStr = string.Empty;

        //    string sLog = string.Empty;

        //     SET or RESET Flag

        //    if (m.Cmd.mainData.IndexOf("ALWAYS_OK") == 0)
        //    {
        //        bRet = true;
        //    }
        //    else if (m.Cmd.mainData.IndexOf("ALWAYS_NG") == 0)
        //    {
        //        bRet = false;
        //    }
        //    else if (m.Cmd.mainData == "DOOR_OPEN")
        //    {
        //        bRet = CIOInterface.DoorOpen(100, 10000);
        //    }
        //    else if (m.Cmd.mainData == "DOOR_CLOSE")
        //    {
        //        bRet = CIOInterface.DoorClose(100, 10000);
        //    }
        //    else if (m.Cmd.name == "TEST_WIP_(VALUE)")
        //    {
        //        GLb.g_sWipid = m.Par1;

        //        GLb.UpdateInfo(new UpdateEventArgs(Globals.GUIupdate.WIPID, GLb.g_sWipid));

        //        m.sListMeasData.Add(GLb.g_sWipid);
        //        if (m.MeasItemsName == "") m.MeasItemsName = "WIPID";

        //        m.Cmd.name = m.Cmd.name.Replace("VALUE", m.Par1);
        //        bRet = true;
        //    }
        //    else if (m.Cmd.mainData.IndexOf("AMBIENT_LIGHT_ON") == 0)
        //    {
        //        bRet = parentForm.devHandler.RobotIOHandler.ControlAmbientLight(false);
        //    }
        //    else if (m.Cmd.mainData.IndexOf("AMBIENT_LIGHT_OFF") == 0)
        //    {
        //        bRet = parentForm.devHandler.RobotIOHandler.ControlAmbientLight(true);
        //    }
        //    else if (m.Cmd.mainData.IndexOf("SEND_MODEL_PLC") == 0)
        //    {
        //        if (m.Par1 != string.Empty)
        //        {
        //            m.Cmd.mainData = m.Cmd.mainData.Replace(":VALUE", m.Par1);
        //            if (cPLCHandler != null && cPLCHandler.m_bConnection)
        //            {
        //                int nAddress = 0;
        //                if (Int32.TryParse(m.Par1, out nAddress))
        //                {
        //                    bRet = cPLCHandler.WriteWordModelPLC(nAddress);
        //                }
        //                else bRet = false;
        //            }
        //            else bRet = false;
        //        }
        //        else bRet = false;
        //    }
        //    else if (m.Cmd.mainData.IndexOf("LIGHT_TURN_ON_ALL") == 0)
        //    {
        //        foreach (CLightSource cls in LightHandler)
        //        {
        //            cls.LightTurnOnAll();
        //        }
        //        bRet = true;
        //    }
        //    else if (m.Cmd.mainData.IndexOf("LIGHT_TURN_OFF_ALL") == 0)
        //    {
        //        foreach (CLightSource cls in LightHandler)
        //        {
        //            cls.LightTurnOffAll();
        //        }
        //        bRet = true;
        //    }
        //    else if (m.Cmd.name.IndexOf("SEND_CAPTURE_DONE") >= 0)
        //    {
        //        if (m.Par1 != string.Empty)
        //        {
        //            m.Cmd.mainData = m.Cmd.mainData.Replace("VALUE", m.Par1);
        //        }

        //        int nPos = m.Cmd.mainData.IndexOf(":");
        //        string sData = m.Cmd.mainData.Substring(nPos + 1).Trim();
        //        string[] sTemp = sData.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //        if (sTemp.Length > 0)
        //        {
        //            int nIndex = 0;
        //            if (int.TryParse(sTemp[0], out nIndex))
        //            {

        //                bRet = CIOInterface.SetPulseOutput(nIndex);
        //            }
        //        }
        //        else if (sTemp.Length > 1)
        //        {
        //            int nIndex = 0;
        //            int nDelay = 0;
        //            if (int.TryParse(sTemp[0], out nIndex) && int.TryParse(sTemp[1], out nDelay))
        //            {
        //                bRet = CIOInterface.SetPulseOutput(nIndex, true, nDelay);
        //            }

        //        }
        //    }
        //    else if (m.Cmd.mainData.IndexOf("PINVISION_WAITDONE") == 0)
        //    {
        //        Measurement here
        //        parentForm.panelSeqView.pinPanel2.PV_SetIndex(-98);

        //        int nidx = m.n_Par1Val(0);
        //        if (nidx > GLb.g_PinvisionData.Length - 1) nidx = GLb.g_PinvisionData.Length - 1;

        //        GLb.g_PinvisionData[nidx].PinvisionReportData.Clear();
        //        GLb.g_PinvisionData[nidx].Result = Globals.RESULT_CHECK;

        //        m.PinvisionRes = Globals.RESULT_CHECK;

        //        for (int i = 0; i < parentForm.panelSeqView.pinPanel2.NumOfPinView; i++)
        //        {
        //            if (parentForm.panelSeqView.PinVisionIsSeparateMeasure(i))
        //            {
        //                continue;
        //            }
        //            string mesErr = string.Empty;
        //             parentForm.panelSeqView.PinVisionMeas(i, out mesErr);
        //            parentForm.panelSeqView.PinVisionMeasAsync(i, out mesErr);

        //            if (mesErr != string.Empty)
        //            {
        //                GLb.g_sChkReason += mesErr + "\r\n";
        //            }
        //        }

        //         Wait all data done
        //        while (!parentForm.panelSeqView.pinPanel2.PV_isMeasDone())
        //        {
        //            Thread.Sleep(20);
        //        }
        //        bRet = (parentForm.panelSeqView.pinPanel2.PV_MeasResult == PinvisionLib.MEAS_RESULT_STATUS.OK ||
        //                parentForm.panelSeqView.pinPanel2.PV_MeasResult == PinvisionLib.MEAS_RESULT_STATUS.NG);

        //        if (bRet)
        //        {
        //            GLb.g_sChkReason = string.Empty;
        //        }

        //        string sErr = string.Empty;
        //        List<PinvisionLib.ResultPackage> PinvisionDataTemp = parentForm.panelSeqView.pinPanel2.PV_GetResultPack(out sErr);
        //        if (PinvisionDataTemp != null || PinvisionDataTemp.Count > 0)
        //        {
        //            GLb.g_PinvisionData[nidx].PinvisionReportData.AddRange(PinvisionDataTemp);
        //        }

        //        string sWrite = string.Empty;

        //        if (bRet)
        //        {

        //            if (sWrite == string.Empty)
        //            {
        //                for (int i = 0; i < PinvisionDataTemp.Count; i++)
        //                {
        //                    sWrite += PinvisionDataTemp[i].ToString() + "\r\n";
        //                }
        //            }

        //            List<string> sBendingList = parentForm.panelSeqView.pinPanel2.PV_GetBendingValueList();

        //            if (sBendingList != null && sBendingList.Count > 0)
        //            {
        //                sWrite += "\r\nBENDING VALUE:\r\n";
        //                foreach (string s in sBendingList)
        //                {
        //                    sWrite += s + "\r\n";
        //                }
        //            }

        //            if (parentForm.panelSeqView.pinPanel2.PV_MeasResult == PinvisionLib.MEAS_RESULT_STATUS.OK)
        //            {
        //                m.PinvisionRes = Globals.RESULT_OK;
        //            }
        //            else
        //            {
        //                m.PinvisionRes = Globals.RESULT_NG;
        //            }
        //        }
        //        else
        //        {
        //            m.PinvisionRes = Globals.RESULT_CHECK;
        //        }

        //        GLb.g_PinvisionData[nidx].Result = m.PinvisionRes;

        //        if (!parentForm.panelSeqView.IsBreakRunSeq)
        //        {
        //            Save system log
        //            if (sWrite != string.Empty)
        //            {
        //                try
        //                {
        //                    string sMeasResultPath = CreateResultPath(parentForm.panelSeqView.sHeaderLog);
        //                    File.WriteAllText(sMeasResultPath, sWrite);
        //                }
        //                catch (System.Exception ex)
        //                {
        //                    Log.LogWrite(Globals.LogLv.Verbose, ex);
        //                }
        //            }

        //            DateTime dt = DateTime.Now;
        //            string sWip = GLb.g_sWipid;
        //            if (sWip.Trim() == string.Empty)
        //            {
        //                sWip = "NOWIP";
        //            }

        //            Export result image
        //            for (int i = 0; i < parentForm.panelSeqView.pinPanel2.NumOfPinView; i++)
        //            {
        //                if (parentForm.panelSeqView.PinVisionIsSeparateMeasure(i))
        //                {
        //                    continue;
        //                }
        //                string sRes = string.Empty;
        //                if (parentForm.panelSeqView.pinPanel2.PV_detailResult(i) == PinvisionLib.MEAS_RESULT_STATUS.OK)
        //                {
        //                    sRes = "OK";
        //                }
        //                else if (parentForm.panelSeqView.pinPanel2.PV_detailResult(i) == PinvisionLib.MEAS_RESULT_STATUS.NG)
        //                {
        //                    sRes = "NG";
        //                }
        //                else if (parentForm.panelSeqView.pinPanel2.PV_detailResult(i) == PinvisionLib.MEAS_RESULT_STATUS.CHECK)
        //                {
        //                    sRes = "CHECK";
        //                }
        //                else if (parentForm.panelSeqView.pinPanel2.PV_detailResult(i) == PinvisionLib.MEAS_RESULT_STATUS.NONE)
        //                {
        //                    sRes = "OK";
        //                }

        //                string OrgImgPath = Globals.g_sPVInspectionImgDir +
        //                dt.Year.ToString("0000") +
        //                "\\" + dt.Month.ToString("00") +
        //                "\\" + dt.Day.ToString("00") +
        //                "\\" + sRes +
        //                "\\" + GLb.g_sCurrentModel +
        //                "\\" + sWip +
        //                "\\" + "Index" + i + "_" +
        //                dt.ToString("yyyy-MM-dd_HH-mm-ss") +
        //                ".jpg";

        //                if (!Directory.Exists(Path.GetDirectoryName(OrgImgPath)))
        //                {
        //                    Util.Create_Dir(Path.GetDirectoryName(OrgImgPath));
        //                }

        //                try
        //                {
        //                    parentForm.panelSeqView.pinPanel2.PV_ExportImage(i, OrgImgPath);
        //                }
        //                catch
        //                {
        //                }
        //            }
        //        }

        //    }

        //    else if (m.Cmd.mainData.IndexOf("PINVISION_MEAS") == 0)
        //    {
        //        Measurement here
        //        int Pinidx = m.n_Par1Val(0);
        //        parentForm.panelSeqView.pinPanel2.PV_SetIndex(Pinidx);

        //        int nidx = Pinidx;
        //        if (nidx > GLb.g_PinvisionData.Length - 1) nidx = GLb.g_PinvisionData.Length - 1;

        //        GLb.g_PinvisionData[nidx].PinvisionReportData.Clear();
        //        GLb.g_PinvisionData[nidx].Result = Globals.RESULT_CHECK;
        //        m.PinvisionRes = Globals.RESULT_CHECK;

        //        string mesErr = string.Empty;
        //        parentForm.panelSeqView.PinVisionMeas(Pinidx, out mesErr);
        //         parentForm.panelSeqView.PinVisionMeasAsync(Pinidx, out mesErr);
        //        if (mesErr != string.Empty)
        //        {
        //            GLb.g_sChkReason += mesErr + "\r\n";
        //        }
        //         Wait all data done
        //        while (!parentForm.panelSeqView.pinPanel2.PV_isMeasDone())
        //        {
        //            Thread.Sleep(20);
        //        }
        //        bRet = (parentForm.panelSeqView.pinPanel2.PV_MeasResult == PinvisionLib.MEAS_RESULT_STATUS.OK ||
        //                parentForm.panelSeqView.pinPanel2.PV_MeasResult == PinvisionLib.MEAS_RESULT_STATUS.NG);

        //        if (bRet)
        //        {
        //            GLb.g_sChkReason = string.Empty;
        //        }

        //        string sErr = string.Empty;
        //        List<PinvisionLib.ResultPackage> PinvisionDataTemp = parentForm.panelSeqView.pinPanel2.PV_GetResultPack(out sErr);
        //        if (PinvisionDataTemp != null || PinvisionDataTemp.Count > 0)
        //        {
        //            GLb.g_PinvisionData[nidx].PinvisionReportData.AddRange(PinvisionDataTemp);
        //        }

        //        string sWrite = string.Empty;

        //        if (bRet)
        //        {

        //            if (sWrite == string.Empty)
        //            {
        //                for (int i = 0; i < PinvisionDataTemp.Count; i++)
        //                {
        //                    sWrite += PinvisionDataTemp[i].ToString() + "\r\n";
        //                }
        //            }

        //            List<string> sBendingList = parentForm.panelSeqView.pinPanel2.PV_GetBendingValueList();

        //            if (sBendingList != null && sBendingList.Count > 0)
        //            {
        //                sWrite += "\r\nBENDING VALUE:\r\n";
        //                foreach (string s in sBendingList)
        //                {
        //                    sWrite += s + "\r\n";
        //                }
        //            }

        //            if (parentForm.panelSeqView.pinPanel2.PV_MeasResult == PinvisionLib.MEAS_RESULT_STATUS.OK)
        //            {
        //                m.PinvisionRes = Globals.RESULT_OK;
        //            }
        //            else
        //            {
        //                m.PinvisionRes = Globals.RESULT_NG;
        //            }
        //        }
        //        else
        //        {
        //            m.PinvisionRes = Globals.RESULT_CHECK;
        //        }

        //        GLb.g_PinvisionData[nidx].Result = m.PinvisionRes;

        //        if (!parentForm.panelSeqView.IsBreakRunSeq)
        //        {
        //            Save system log
        //            if (sWrite != string.Empty)
        //            {
        //                try
        //                {
        //                    string sMeasResultPath = CreateResultPath(parentForm.panelSeqView.sHeaderLog);
        //                    File.WriteAllText(sMeasResultPath, sWrite);
        //                }
        //                catch (System.Exception ex)
        //                {
        //                    Log.LogWrite(Globals.LogLv.Verbose, ex);
        //                }
        //            }

        //            DateTime dt = DateTime.Now;
        //            string sWip = GLb.g_sWipid;
        //            if (sWip.Trim() == string.Empty)
        //            {
        //                sWip = "NOWIP";
        //            }

        //            Export result image
        //            string sRes = string.Empty;
        //            if (parentForm.panelSeqView.pinPanel2.PV_detailResult(Pinidx) == PinvisionLib.MEAS_RESULT_STATUS.OK)
        //            {
        //                sRes = "OK";
        //            }
        //            else if (parentForm.panelSeqView.pinPanel2.PV_detailResult(Pinidx) == PinvisionLib.MEAS_RESULT_STATUS.NG)
        //            {
        //                sRes = "NG";
        //            }
        //            else if (parentForm.panelSeqView.pinPanel2.PV_detailResult(Pinidx) == PinvisionLib.MEAS_RESULT_STATUS.CHECK)
        //            {
        //                sRes = "CHECK";
        //            }
        //            else if (parentForm.panelSeqView.pinPanel2.PV_detailResult(Pinidx) == PinvisionLib.MEAS_RESULT_STATUS.NONE)
        //            {
        //                sRes = "OK";
        //            }

        //            string OrgImgPath = Globals.g_sPVInspectionImgDir +
        //            dt.Year.ToString("0000") +
        //            "\\" + dt.Month.ToString("00") +
        //            "\\" + dt.Day.ToString("00") +
        //            "\\" + sRes +
        //            "\\" + GLb.g_sCurrentModel +
        //            "\\" + sWip +
        //            "\\" + "Index" + Pinidx + "_" +
        //            dt.ToString("yyyy-MM-dd_HH-mm-ss") +
        //            ".jpg";

        //            if (!Directory.Exists(Path.GetDirectoryName(OrgImgPath)))
        //            {
        //                Util.Create_Dir(Path.GetDirectoryName(OrgImgPath));
        //            }

        //            try
        //            {
        //                parentForm.panelSeqView.pinPanel2.PV_ExportImage(Pinidx, OrgImgPath);
        //            }
        //            catch
        //            {

        //            }
        //        }
        //    }
        //    else if (m.Cmd.mainData.IndexOf("PINVISION_READ_BARCODE") == 0)
        //    {
        //        string sRes = parentForm.panelSeqView.pinPanel2.PV_ReadBarcode();

        //        string sTemp = BarcodeParseValue(sRes.Trim());

        //        if (sTemp != string.Empty)
        //        {
        //            bRet = true;
        //            GLb.g_sWipid = sTemp;
        //            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        //            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
        //            GLb.g_sWipid = r.Replace(GLb.g_sWipid, "");
        //            GLb.UpdateInfo(new UpdateEventArgs(Globals.GUIupdate.WIPID, GLb.g_sWipid));
        //        }
        //        else
        //        {
        //            bRet = false;
        //        }

        //        m.bRes = bRet;

        //        return retStr;
        //    }

        //    else if (m.Cmd.mainData.IndexOf("PINVISION_SUB_IMAGE") == 0)
        //    {
        //        int n0 = 0;
        //        int n1 = 1;
        //        //Measurement here
        //        int n0 = m.n_Par1Val(0);
        //        int n1 = m.n_Par1Val(1);
        //        if (n0 < 0) n0 = 0;
        //        if (n0 > m_SaveMat.Length - 1) n0 = m_SaveMat.Length - 1;
        //        if (n1 < 0) n1 = 0;
        //        if (n1 > m_SaveMat.Length - 1) n1 = m_SaveMat.Length - 1;

        //        if (n0 == n1)
        //        {
        //            bRet = false;
        //            Log.LogWrite(Globals.LogLv.Information, "cannot subtract same image");
        //        }
        //        else
        //        {
        //            bRet = GetSubMat(m_SaveMat[n0], m_SaveMat[n1], 13);
        //            if (!bRet)
        //            {
        //                Log.LogWrite(Globals.LogLv.Information, "cannot subtract image");
        //            }
        //            else if (m_Bitmap != null)
        //            {
        //                DateTime dt = DateTime.Now;
        //                string sWip = GLb.g_sWipid;
        //                if (sWip.Trim() == string.Empty)
        //                {
        //                    sWip = "NOWIP";
        //                }

        //                string OrgImgPath = Globals.g_sPVOriginImgDir +
        //                dt.Year.ToString("0000") +
        //                "\\" + dt.Month.ToString("00") +
        //                "\\" + dt.Day.ToString("00") +
        //                "\\" + sWip + "_" + "COMPARE" + "_" +
        //                dt.ToString("yyyy-MM-dd_HH-mm-ss") +
        //                ".bmp";

        //                if (!Directory.Exists(Path.GetDirectoryName(OrgImgPath)))
        //                {
        //                    Util.Create_Dir(Path.GetDirectoryName(OrgImgPath));
        //                }
        //                if (GLb.g_tSysCfg.bPinvisionSaveOrgImg)
        //                {
        //                    m_Bitmap.Save(OrgImgPath);
        //                }

        //                string sPar1 = m.Par1;
        //                if (sPar1.Trim() != string.Empty)
        //                {
        //                    string[] sPinPosList = sPar1.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //                    List<int> iPinPosList = new List<int>();

        //                    if (sPinPosList.Length > 0)
        //                    {

        //                        foreach (string s in sPinPosList)
        //                        {
        //                            int iTemp = -1;
        //                            if (Int32.TryParse(s, out iTemp))
        //                            {
        //                                if (iTemp >= 0)
        //                                {
        //                                    iPinPosList.Add(iTemp);
        //                                }
        //                            }
        //                        }
        //                        if (iPinPosList.Count > 0)
        //                        {

        //                            for (int i = 0; i < iPinPosList.Count; i++)
        //                            {
        //                                string sErr = "";
        //                                if (parentForm.panelSeqView.PinVisionSetImg(iPinPosList[i], (System.Drawing.Bitmap)m_Bitmap.Clone(), out sErr) != 0)
        //                                {
        //                                    bRet = false;
        //                                    GLb.g_sChkReason = sErr;

        //                                    break;
        //                                }
        //                            }

        //                        }
        //                        else
        //                        {
        //                            bRet = false;
        //                            GLb.g_sChkReason = "No window load this image";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        bRet = false;
        //                        GLb.g_sChkReason = "No window load this image";
        //                    }
        //                }
        //                else
        //                {
        //                    bRet = true;
        //                }

        //            }
        //            else
        //            {
        //                bRet = false;
        //                Log.LogWrite(Globals.LogLv.Information, "Image = null");
        //            }
        //        }

        //        m.bRes = bRet;
        //        if (!m.bRes)
        //        {
        //            GLb.g_sChkReason = "Cannot create image";
        //        }
        //        return retStr;

        //    }
        //    else if (m.Cmd.mainData.IndexOf("READ_GMES_DATA") == 0)
        //    {
        //         Read Data from gmes and set to Pinvision lib
        //        if (parentForm.panelSeqView.pinPanel2 != null)
        //        {
        //            parentForm.panelSeqView.pinPanel2.PV_ClearSpecText();
        //        }
        //        string sRet = "";
        //        string[] sData = m.Par1.Trim().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        //        string ItemName = m.MeasItemsName;
        //        if (m.Par1.Trim() == "" || sData == null || sData.Length < 1)
        //        {
        //            bRet = false;
        //            Log.LogWrite(Globals.LogLv.Information, "READ_GMES_DATA: Parameter not found", bRet);
        //        }
        //        else
        //        {
        //            for (int kk = 0; kk < sData.Length; kk++)
        //            {
        //                string sFind = sData[kk].Trim();
        //                SpecField sf = parentForm.devHandler.GmesHandler.sPecList.Find(x => x.sSpecName == sFind);

        //                if (sf != null)
        //                {
        //                    sRet = sf.sVal;
        //                    bRet = true;
        //                }
        //                else
        //                {
        //                    sRet = "-9999";
        //                    bRet = false;
        //                }
        //                m.sListMeasData.Add(sRet);

        //                set to Pinvision lib
        //                if (parentForm.panelSeqView.pinPanel2 != null)
        //                {
        //                    parentForm.panelSeqView.pinPanel2.PV_SetSpecText(kk, sRet);
        //                }
        //                if (ItemName == "") m.MeasItemsName += sFind + ",";
        //            }

        //        }
        //    }

        //    else if (m.Cmd.mainData.IndexOf("TEST_DATA:") >= 0)
        //    {
        //        try
        //        {
        //            int nposTemp = m.Cmd.mainData.IndexOf("TEST_DATA:");
        //            string[] bdata = m.Cmd.mainData.Substring(nposTemp + 10).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //            GLb.g_dDispOnTime = 20;  // Test

        //            if (!ChangeSpecFromFile(m))
        //            {
        //                m.isSetRetAsCheck = true;
        //                GLb.g_sChkReason = "Cannot read WB time table";
        //                bRet = false;
        //            }
        //            else
        //            {
        //                for (int i = 0; i < bdata.Length; i++)
        //                {
        //                    m.sListMeasData.Add(bdata[i]);
        //                }
        //                if (m.Par2 == "") m.Par2 = "Data0,Data1";
        //                bRet = true;
        //            }

        //        }
        //        catch
        //        {
        //            bRet = false;
        //        }

        //    }

        //    else if (m.Cmd.mainData.IndexOf("TEST_WIPID:") >= 0)
        //    {
        //        try
        //        {
        //            int nposTemp = m.Cmd.mainData.IndexOf("TEST_WIPID:");
        //            GLb.g_sWipid = m.Cmd.mainData.Substring(nposTemp + 11).Trim();
        //            GLb.UpdateInfo(new UpdateEventArgs(Globals.GUIupdate.WIPID, GLb.g_sWipid));
        //            bRet = true;
        //        }
        //        catch
        //        {
        //            bRet = false;
        //        }

        //    }
        //    else if (m.Cmd.mainData.IndexOf("DELAY") >= 0)
        //    {
        //        bRet = true;
        //    }
        //    else if (m.Cmd.mainData.IndexOf("CHECK_") == 0) // Check Sequence
        //    {
        //        if (m.Cmd.mainData == "CHECK_PREV_NG")
        //        {
        //            bRet = m.isPrevNG;
        //        }
        //        else if (m.Cmd.mainData == "CHECK_PREV_OK")
        //        {
        //            bRet = !m.isPrevNG;
        //        }
        //        else if (m.Cmd.mainData == "CHECK_BEFORE_NG")
        //        {
        //            bRet = m.isBeforeNG;
        //        }
        //        else if (m.Cmd.mainData == "CHECK_BEFORE_OK")
        //        {
        //            bRet = !m.isBeforeNG;
        //        }
        //        else if (m.Cmd.mainData == "CHECK_PINVISION_PREV_OK")
        //        {
        //            bRet = !m.PinvisionPrevNG;
        //            if (bRet) parentForm.panelSeqView.bNgPalletDown = false;
        //            else parentForm.panelSeqView.bNgPalletDown = true;
        //        }
        //        else if (m.Cmd.mainData == "CHECK_PINVISION_PREV_NG")
        //        {
        //            bRet = m.PinvisionPrevNG;
        //        }
        //        else if (m.Cmd.mainData == "CHECK_AUTO_SEQ")
        //        {
        //            if (GLb.g_tBootUp.bGMES_Used && GLb.g_tSysCfg.bAutoChangeModel && GLb.g_tSysCfg.bCheckGmesJobfile)
        //            {
        //                if (GmesHandler.m_Info.sMODEL_NAME != null)
        //                {
        //                    if (GLb.g_tSysCfg.bDonotUseSuffix && !GLb.g_tSysCfg.bModelMappingByKey)
        //                    {
        //                        GLb.g_sGmesModel = GmesHandler.m_Info.sMODEL_NAME;
        //                    }
        //                    else if (GLb.g_tSysCfg.bDonotUseSuffix && GLb.g_tSysCfg.bModelMappingByKey)
        //                    {
        //                        string[] s = GmesHandler.m_Info.sMODEL_NAME.Split('-');
        //                        if (s.Length > 1) GLb.g_sGmesModel = s[0];
        //                        else GLb.g_sGmesModel = GmesHandler.m_Info.sMODEL_NAME;
        //                    }
        //                    else
        //                    {
        //                        GLb.g_sGmesModel = GmesHandler.m_Info.sMODEL_NAME + Util.MODEL_NAME_SEPRATE + GmesHandler.m_Info.sMODEL_SUFFIX;
        //                    }
        //                    if ((GLb.g_ViewingSeqName != GLb.g_tModel.Jobfile) || (GLb.g_sCurrentModel != GLb.g_sGmesModel))
        //                    {
        //                        GLb.g_bChangeModel = true;
        //                        bRet = false;
        //                    }
        //                    else
        //                    {
        //                        GLb.g_bChangeModel = false;
        //                        bRet = true;
        //                    }
        //                }
        //                else bRet = false;
        //            }
        //            else
        //            {
        //                GLb.g_bChangeModel = false;
        //                bRet = true;
        //            }

        //        }
        //        else if (m.Cmd.mainData == "CHECK_BEFORE_OK")
        //        {
        //            bRet = !m.isBeforeNG;
        //        }
        //        else if (m.Cmd.mainData.IndexOf("CHECK_MODEL_GROUP_") == 0)
        //        {
        //            bRet = true;
        //            if (m.Cmd.mainData == "CHECK_MODEL_GROUP_MRA_CID")
        //            {
        //                GLb.g_tModel.nModelGroup = Globals.ModelGroup.MRA_CID;
        //            }
        //            else if (m.Cmd.mainData == "CHECK_MODEL_GROUP_MRA_ICD")
        //            {
        //                GLb.g_tModel.nModelGroup = Globals.ModelGroup.MRA_ICD;
        //            }
        //            else if (m.Cmd.mainData == "CHECK_MODEL_GROUP_MFA")
        //            {
        //                GLb.g_tModel.nModelGroup = Globals.ModelGroup.MFA;
        //            }
        //            else if (m.Cmd.mainData == "CHECK_MODEL_GROUP_GROUP_2")
        //            {
        //                GLb.g_tModel.nModelGroup = Globals.ModelGroup.GROUP_2;
        //            }
        //            else
        //            {
        //                bRet = false;
        //            }
        //            Log.LogWrite(Globals.LogLv.Information, "Check Model Group:" + m.Cmd.mainData, bRet);
        //        }
        //        else if (m.Cmd.mainData.IndexOf("CHECK_RUNNING_MODEL") >= 0)
        //        {
        //            m.sListMeasData.Add(GLb.g_tModel.nModelGroup.ToString().Replace(",", string.Empty));
        //            if (m.Par2 == "") m.Par2 = "Model:";
        //            bRet = true;
        //            Delay(m.n_delayTime);
        //        }
        //        else
        //        {
        //            m.isSetRetAsCheck = true;
        //            GLb.g_sChkReason = "Command Error";
        //        }

        //    }
        //    else if (m.Cmd.mainData.IndexOf("LOAD_") == 0) // LoadFile
        //    {

        //        if (m.Cmd.mainData == "LOAD_WB_TABLE")
        //        {
        //            GLb.g_sWBFileName = Globals.g_sWBFileDir + m.Par1 + ".csv";
        //            if (GLb.g_tModel.bUseWBCorr && (!File.Exists(GLb.g_sWBFileName)))
        //            {
        //                bRet = false;
        //            }
        //            else
        //            {
        //                bRet = true;
        //            }
        //            Log.LogWrite(Globals.LogLv.Information, "Get WB Table: " + GLb.g_sWBFileName, bRet);
        //        }
        //        else if (m.Cmd.mainData == "LOAD_BLU_TABLE")
        //        {
        //            GLb.g_sBLUFileName = Globals.g_sBLUFileDir + m.Par1 + ".csv"; ;
        //            if (GLb.g_tModel.bUseBLUCorr && (!File.Exists(GLb.g_sBLUFileName)))
        //            {
        //                bRet = false;
        //            }
        //            else
        //            {
        //                bRet = true;
        //            }
        //            Log.LogWrite(Globals.LogLv.Information, "Get BLU Table: " + GLb.g_sBLUFileName, bRet);
        //        }
        //        else
        //        {
        //            m.isSetRetAsCheck = true;
        //            GLb.g_sChkReason = "Command Error";
        //        }

        //    }
        //    else if (m.Cmd.mainData.IndexOf("PRODUCT_DETECTION_SENSOR") >= 0)
        //    {

        //    }
        //    else if (m.Cmd.mainData.IndexOf("WAIT_THREAD_DONE") >= 0)
        //    {
        //        bRet = false;

        //        if (m.Par1 != string.Empty)
        //        {
        //            m.Cmd.mainData = m.Cmd.mainData.Replace("VALUE", m.Par1);
        //        }

        //        int nPos = m.Cmd.mainData.IndexOf(":");
        //        string sData = m.Cmd.mainData.Substring(nPos + 1).Trim();
        //        string[] sTemp = sData.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //        if (sTemp.Length > 0)
        //        {
        //            int nIndex = 0;
        //            if (int.TryParse(sTemp[0], out nIndex))
        //            {
        //                m.DisplayName = m.DisplayName.Replace("VALUE", sTemp[0]);
        //                if (nIndex == 1)
        //                {
        //                    while (true)
        //                    {
        //                        if ((listThreadSub1.Count >= 1 && bThreadSubRet_1) || GLb.g_bStopProcess)   //break if Stop or Sub Thread res exist
        //                        {
        //                            bThreadSubRet_1 = false;
        //                            break;
        //                        }
        //                    }
        //                    lock (listThreadSub1)
        //                    {
        //                        try
        //                        {
        //                            foreach (var item in listThreadSub1)
        //                            {
        //                                if (item != null)
        //                                {
        //                                    bRet = item.bRes;
        //                                    listThreadSub1.Remove(item);
        //                                    item.sListMeasData.AddRange(item.sListMeasData);
        //                                    if (item.sListMeasData.Count > 0)
        //                                    {
        //                                        item.MeasItemsName = item.MeasItemsName + "," + item.MeasItemsName;
        //                                    }
        //                                    if (!bRet)
        //                                    {
        //                                        GLb.g_sChkReason += "\n" + item.DisplayName;
        //                                        break;
        //                                    }
        //                                    break;
        //                                }
        //                                else break;
        //                            }
        //                        }
        //                        catch { }
        //                    }
        //                }
        //                else if (nIndex == 2)
        //                {
        //                    while (true)
        //                    {
        //                        if ((listThreadSub2.Count >= 1 && bThreadSubRet_2) || GLb.g_bStopProcess)   //break if Stop or Sub Thread res exist
        //                        {
        //                            bThreadSubRet_2 = false;
        //                            break;
        //                        }
        //                    }
        //                    lock (listThreadSub2)
        //                    {
        //                        try
        //                        {
        //                            foreach (var item in listThreadSub2)
        //                            {
        //                                if (item != null)
        //                                {
        //                                    bRet = item.bRes;
        //                                    listThreadSub2.Remove(item);
        //                                    item.sListMeasData.AddRange(item.sListMeasData);
        //                                    if (item.sListMeasData.Count > 0)
        //                                    {
        //                                        item.MeasItemsName = item.MeasItemsName + "," + item.MeasItemsName;
        //                                    }
        //                                    if (!bRet)
        //                                    {
        //                                        GLb.g_sChkReason += "\n" + item.DisplayName;
        //                                        break;
        //                                    }
        //                                    break;
        //                                }
        //                                else break;
        //                            }
        //                        }
        //                        catch { }
        //                    }

        //                }
        //                else if (nIndex == 3)
        //                {
        //                    while (true)
        //                    {
        //                        if ((listThreadSub3.Count >= 1 && bThreadSubRet_3) || GLb.g_bStopProcess)   //break if Stop or Sub Thread res exist
        //                        {
        //                            bThreadSubRet_3 = false;
        //                            break;
        //                        }
        //                    }
        //                    lock (listThreadSub3)
        //                    {
        //                        try
        //                        {
        //                            foreach (var item in listThreadSub3)
        //                            {
        //                                if (item != null)
        //                                {
        //                                    bRet = item.bRes;
        //                                    listThreadSub3.Remove(item);
        //                                    item.sListMeasData.AddRange(item.sListMeasData);
        //                                    if (item.sListMeasData.Count > 0)
        //                                    {
        //                                        item.MeasItemsName = item.MeasItemsName + "," + item.MeasItemsName;
        //                                    }
        //                                    if (!bRet)
        //                                    {
        //                                        GLb.g_sChkReason += "\n" + item.DisplayName;
        //                                        break;
        //                                    }
        //                                    break;
        //                                }
        //                                else break;
        //                            }
        //                        }
        //                        catch { }
        //                    }
        //                }
        //                Log.LogWrite(Globals.LogLv.Debug, "WAIT_THREAD_DONE " + sTemp[0] + ": " + bRet);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        bRet = false;
        //    }

        //    m.bRes = bRet;

        //    return retStr;

        //}
        public string CreateResultPath(string sTemp)
        {
            string sMeasResultPath = "";
            string sWip = GLb.g_sWipid;
            if (string.IsNullOrEmpty(sWip.Trim()))
            {
                sWip = "NOWIP";
            }

            DateTime dt = DateTime.Now;
            sMeasResultPath =
                GLb.g_sMesResultDir
                + dt.Year.ToString("0000")
                + "\\"
                + dt.Month.ToString("00")
                + "\\"
                + dt.Day.ToString("00")
                + "\\"
                + "("
                + sWip
                + "-"
                + sTemp
                + ")_"
                + dt.ToString("yyyy-MM-dd_HH-mm-ss")
                + ".txt";

            if (!Directory.Exists(Path.GetDirectoryName(sMeasResultPath)))
            {
                Create_Dir(Path.GetDirectoryName(sMeasResultPath));
            }
            return sMeasResultPath;
        }

        public static bool Create_Dir(string Dir)
        {
            string sStr = string.Empty;

            if (!Directory.Exists(Dir))
            {
                for (int i = 0; i < Dir.Length; i++)
                {
                    sStr = sStr + Dir.Substring(i, 1);
                    if (Dir.Substring(i, 1) == "\\" || i == Dir.Length - 1)
                    {
                        if (Directory.Exists(sStr))
                        {
                            continue;
                        }

                        try
                        {
                            Directory.CreateDirectory(sStr);
                        }
                        catch
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private string ROBOTCmd(MacroFunc m)
        {
            bool bRet = false;
            string retStr = string.Empty;

            if (m.Cmd.mainData.Contains("VALUE"))
            {
                if (m.Par1 == string.Empty)
                {
                    m.Par1 = "1";
                }
                m.Cmd.mainData = m.Cmd.mainData.Replace("VALUE", m.Par1);
                m.DisplayName = m.DisplayName.Replace("VALUE", m.Par1);
            }

            //NOTE: do not run if robot have not init
            if (GLb.g_tSysCfg.bUseRobotMotion) { }
            else { }

            m.bRes = bRet;
            if (!m.bRes)
            {
                m.isSetRetAsCheck = true;
                // GLb.g_sChkReason = "ROBOT IS NOT RESPONDING";
            }

            return retStr;
        }

        private string MOTIONCmd(MacroFunc m)
        {
            bool bRet = false;
            string retStr = string.Empty;

            if (m.Cmd.mainData.Contains("VALUE"))
            {
                if (m.Par1 == string.Empty)
                {
                    m.Par1 = "1";
                }
                m.Cmd.mainData = m.Cmd.mainData.Replace("VALUE", m.Par1);
                m.DisplayName = m.DisplayName.Replace("VALUE", m.Par1);
            }
            //NOTE: Check Motion Done

            m.bRes = bRet;
            if (!m.bRes)
            {
                m.isSetRetAsCheck = true;
                //GLb.g_sChkReason = "MOTION IS NOT RESPONDING";
            }

            return retStr;
        }

        private object LockObj = new object();

        public void AddACMD(DeviceEvent e)
        {
            lock (LockObj)
            {
                switch (e.type)
                {
                    case DEV_EVENT_TYPE.DET_EFFECT:
                        {
                            lock (DevEventList)
                            {
                                DevEventList.Enqueue(e);
                            }
                            AddDevEvent.Set();
                        }
                        break;
                    case DEV_EVENT_TYPE.DET_EFFECT_SUB1:
                        {
                            lock (DevEventListSub1)
                            {
                                DevEventListSub1.Enqueue(e);
                            }
                            AddDevEventSub1.Set();
                            // add result to list
                        }
                        break;
                    case DEV_EVENT_TYPE.DET_EFFECT_SUB2:
                        {
                            lock (DevEventListSub2)
                            {
                                DevEventListSub2.Enqueue(e);
                            }
                            AddDevEventSub2.Set();
                        }
                        break;
                    case DEV_EVENT_TYPE.DET_EFFECT_SUB3:
                        {
                            lock (DevEventListSub3)
                            {
                                DevEventListSub3.Enqueue(e);
                            }
                            AddDevEventSub3.Set();
                        }
                        break;
                    case DEV_EVENT_TYPE.DET_NOT_EFFECT:
                        {
                            BackgroundWorker worker = new BackgroundWorker();
                            worker.WorkerReportsProgress = true;
                            worker.WorkerSupportsCancellation = true;
                            worker.DoWork += delegate
                            {
                                //Processing_CMD(e);
                            };
                            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                                ProcessRunWorkerCompleted
                            );
                            worker.RunWorkerAsync();
                        }
                        break;
                    default:
                        {
                            BackgroundWorker worker = new BackgroundWorker();
                            worker.WorkerReportsProgress = true;
                            worker.WorkerSupportsCancellation = true;
                            worker.DoWork += delegate
                            {
                                //Processing_CMD(e);
                            };
                            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                                ProcessRunWorkerCompleted
                            );
                            worker.RunWorkerAsync();
                        }
                        break;
                }
            }
        }

        private void ProcessRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) { }

        public bool Delay(int nTime, ref bool breakSignal)
        {
            long time = 0;
            bool bRet = false;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            time = watch.ElapsedMilliseconds;

            while (nTime > time)
            {
                if (breakSignal)
                {
                    bRet = true;
                    break;
                }
                Thread.Sleep(10);
                time = watch.ElapsedMilliseconds;
            }
            watch.Stop();
            return bRet;
        }

        public bool Delay(int nTime)
        {
            long time = 0;
            bool bRet = false;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            time = watch.ElapsedMilliseconds;

            while (nTime > time)
            {
                //if (GLb.g_bStopProcess)   //break if Stop
                //{
                //    bRet = true;
                //    break;
                //}
                Thread.Sleep(10);
                time = watch.ElapsedMilliseconds;
            }
            watch.Stop();
            return bRet;
        }

        public bool WaitForSubXML()
        {
            bool bRet = false;
            long deadTime = 2000;

            if (deadTime < 5000)
            {
                deadTime = 5000;
            }

            long time = 0;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            time = watch.ElapsedMilliseconds;

            while (deadTime > time)
            {
                //if (GLb.g_bStopProcess)
                //{
                //    sReason = "Running stop";
                //    bRet = false;
                //    break;
                //}
                //if (GLb.g_bTestEndChkOM)
                //{
                //    bRet = false;
                //    sReason = "Other pc sequence running stop.";
                //    break;
                //}

                time = watch.ElapsedMilliseconds;
            }
            watch.Stop();
            //Log.LogWrite(Globals.LogLv.Information, sLog + sReason, bRet);
            return bRet;
        }

        public bool WaitForFinish()
        {
            bool bRet = false;
            long deadTime = 2000;

            string sLog = "WaitForOtherMachineFinish: ";
            Log.LogWrite(Globals.LogLv.Information, sLog + "...");
            //deadTime = (int)GLb.g_tSysCfg.nMasterTimeout;
            if (deadTime < 5000)
            {
                deadTime = 5000;
            }

            long time = 0;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            time = watch.ElapsedMilliseconds;
            while (deadTime > time)
            {
                //if (GLb.g_bStopProcess)
                //{
                //    sReason = "Running stop";
                //    bRet = false;
                //    break;
                //}
                //if (GLb.g_bTestEndChkOM || GLb.g_bTestEndNorOM)
                //{
                //    bRet = true;
                //    break;
                //}
                Thread.Sleep(10);
                time = watch.ElapsedMilliseconds;
            }
            watch.Stop();
            //Log.LogWrite(Globals.LogLv.Information, sLog + sReason, bRet);
            return bRet;
        }

        public bool WaitSalveCmdFinish()
        {
            bool bRet = false;
            long deadTime = 10000;

            if (deadTime < 5000)
            {
                deadTime = 5000;
            }

            long time = 0;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            time = watch.ElapsedMilliseconds;
            while (deadTime > time)
            {
                //if (GLb.g_bStopProcess)
                //{
                //    sReason = "Running stop";
                //    bRet = false;
                //    break;
                //}
                //if (!GLb.g_bWaitingSlavecmd)
                //{
                //    bRet = true;
                //    break;
                //}
                Thread.Sleep(10);
                time = watch.ElapsedMilliseconds;
            }
            watch.Stop();
            //Log.LogWrite(Globals.LogLv.Information, sLog + sReason, bRet);
            return bRet;
        }

        public void ClearAlarm()
        {
            //RobotIOHandler.ClearAlarm();
        }

        #region DeepLearning
        public bool m_bDLConnect = false;

        // 1 = OK
        // Other = NG

        #endregion

        //        #region OQA_TEST_FUNC
        //        private DealerSocket mySocket;
        //        private NetMQPoller myPoller;
        //        private static Computer cp;

        //        //PING______________________________________________

        //        public void Ping()
        //        {
        //            bSeverPVIsOpen = cp.Network.Ping(GLb.g_tDevCfg.tRobotCom.sIP, 1000);
        //        }

        //        //CONNECT____________________________________________
        //        public bool ConnectOqa(string sIp, int nPort)
        //        {
        //            mySocket.Options.Identity = Encoding.UTF8.GetBytes(sIp);
        //            bool b = !myPoller.IsRunning;
        //            if (b)
        //            {
        //                try
        //                {
        //                    string s = string.Format("tcp://{0}:{1}", sIp, nPort.ToString());
        //                    mySocket.Connect(s);
        //                    mySocket.ReceiveReady += mySocket_ReceiveReady;
        //                    myPoller.Add(mySocket);
        //                    myPoller.RunAsync();
        //                    Log.LogWrite(Globals.LogLv.Information, "OQA server is connected!");
        //                    return true;
        //                }
        //                catch (Exception e)
        //                {
        //                    Log.LogWrite(Globals.LogLv.Information, "Fail! " + e.Message);
        //                    return false;
        //                }
        //            }
        //            return false;
        //        }

        //        //
        //        private void mySocket_ReceiveReady(object sender, NetMQSocketEventArgs e)
        //        {
        //            NetMQMessage msg = ReceivingSocketExtensions.ReceiveMultipartMessage(e.Socket, 4);
        //            //this.PrintFrame(msg);
        //        }

        //        //private void PrintFrame(NetMQMessage msg)
        //        //{
        //        //    string[] array = new string[]
        //        //    {
        //        //        "",
        //        //        "",
        //        //        ""
        //        //    };
        //        //    string text = "";
        //        //    checked
        //        //    {
        //        //        int num = msg.FrameCount - 1;
        //        //        for (int i = 0; i <= num; i++)
        //        //        {
        //        //            array[i] = msg[i].ConvertToString();
        //        //            text = text + array[i] + ",";
        //        //        }
        //        //        Log.LogWrite(Globals.LogLv.Information, "Received message from OQA server: " + text);

        //        //        bool flag = Operators.CompareString(array[1], "PAUSE", false) == 0;

        //        //        if (flag)
        //        //        {
        //        //            Log.LogWrite(Globals.LogLv.Information, "Received PAUSE message from OQA server");
        //        //            this.SendMessage("ACK", "");
        //        //            mdl_JudgeTool.JudgeTool_WIP = array[2];
        //        //            mdl_Common_Variable.ErrorIndex = 12;
        //        //        }
        //        //        else
        //        //        {
        //        //            bool flag2 = Operators.CompareString(array[1], "RESUME", false) == 0;
        //        //            if (flag2)
        //        //            {
        //        //                mdl_Log.WriteProgramLog("[JudgeTool] Received RESUME message", "");
        //        //                this.SendMessage("ACK", "");
        //        //                mdl_JudgeTool.JudgeTool.Command = "Received 'RESUME' message from JudgeTool";
        //        //                this.DisplayMessage();
        //        //                bool flag3 = mdl_Common_Variable.ErrorIndex == 12;
        //        //                if (flag3)
        //        //                {
        //        //                    mdl_Common_Variable.ErrorIndex = 0;
        //        //                }
        //        //            }
        //        //            else
        //        //            {
        //        //                bool flag4 = Operators.CompareString(array[1], "ACK", false) == 0;
        //        //                if (flag4)
        //        //                {
        //        //                    mdl_JudgeTool.JudgeTool.Command = array[2];
        //        //                    mdl_JudgeTool.JudgeTool.Command = "Received 'ACK' message from JudgeTool";
        //        //                    this.DisplayMessage();
        //        //                    mdl_Log.WriteProgramLog("[JudgeTool] Received ACK message", "");
        //        //                }
        //        //                else
        //        //                {
        //        //                    this.SendMessage("ACK", "");
        //        //                }
        //        //            }
        //        //        }
        //        //    }
        //        //}
        //#endregion
    }
}
