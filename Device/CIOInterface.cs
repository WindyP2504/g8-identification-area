using System;
using System.Threading;
using System.Threading.Tasks;
using VTP_Induction.Device;

namespace VTP_Induction
{
    public static class CIOInterface
    {
        private static Globals GLb = Globals.getInstance();

        private static ThreadsManager ThMan = ThreadsManager.GetInstance();

        private static Thread IOThread1;

        private static bool Terminated = false;

        private static TForm parentForm;
        public static TCIO g_tCIO1;

        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public static void LoadData(TForm tForm)
        {
            // TODO: Complete member initialization
            parentForm = tForm;
            //instance = this;
            g_tCIO1 = new TCIO();
            StartThread();

        }

        public static void DestroyThread(TForm tForm)
        {
            //// TODO: Complete member initialization
            //parentForm = tForm;
            ////instance = this;
            //g_tCIO1 = new TCIO();
            StopThread();

        }


        public static async Task LoadDataAsync(TForm tForm)
        {
            parentForm = tForm;
            // Khởi tạo các g_tCIO và thực hiện INIT_IO() như trước
            g_tCIO1 = new TCIO();
            INIT_IO();
            // Khởi động các luồng như các Task
            Task[] tasks = new Task[]
            {
            Task.Run(() => StartTask(Execute1_)),
        // Các Task khác...
            };

            await Task.WhenAll(tasks);

            //await Task.WhenAll(task1, task2, task3, task4, task5, task6, task7, task8);
            // Tiếp tục với các Task khác...
        }


        public static void Destroy()
        {
            cancellationTokenSource.Cancel(); // Yêu cầu hủy bỏ các Task
        }

        private static async Task StartTask(Func<Task> executeFunction)
        {
            await executeFunction();

        }
        private static async Task Execute1_()
        {
            while (!Terminated)
            {
                await Task.Delay(5000); // Thay thế Thread.Sleep(5000)

                if (parentForm == null || parentForm.devHandler == null)
                {
                    continue;
                }

                PRC_CheckSensor_S1();
                if (Terminated)
                {
                    break;
                }


                //UIEvent e = new UIEvent();
                //e.cmd = UI_EVENT_CMD.UEC_UM_REFRESH_STATUS;
                //e.srcReq = "CIOInterface";
                //if (parentForm.UIEventReg != null)
                //    parentForm.UIEventReg(e);
            }
        }




        #region Additional functions    
        private static void StartThread()
        {
            IOThread1 = new Thread(new ThreadStart(Execute1));
            IOThread1.Name = "CIOInterface sensor capture thread 1";

            ThMan.AddThread(IOThread1.Name, IOThread1);

            IOThread1.Start();
        }
        private static void StopThread()
        {
            if (IOThread1 != null)
            {
                IOThread1.Abort();
                ThMan.RemoveThread(IOThread1.Name);
                IOThread1 = null;
            }
        }

        #endregion

        private static void Execute1()
        {
            while (!Terminated)
            {
                Thread.Sleep(5);
                if (parentForm == null)
                {
                    continue;
                }

                if (parentForm.devHandler == null)
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


                //Refresh UI status here...

                //UIEvent e = new UIEvent();
                //e.cmd = UI_EVENT_CMD.UEC_UM_REFRESH_STATUS;
                //e.srcReq = "CIOInterface";
                //if (parentForm.UIEventReg != null)
                //    parentForm.UIEventReg(e);

            }
        }


        private static bool PauseThread(bool bPause)
        {
            /* Implement code here */
            return false;
        }

        private static void PRC_CheckSensor_S1()
        {
            bool[] inputs = new bool[(int)Globals.k_InputConfig.INRow_End];
            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = INPUT_CheckInputSignal(i);
            }

            lock (g_tCIO1.m_bInputSignal)
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                    g_tCIO1.m_bInputSignal[i] = inputs[i];
                }
            }

        }

        private static void PRC_CheckSensor()
        {

            for (int i = 0; i < (int)Globals.k_InputConfig.INRow_End; i++)
            {
                lock (g_tCIO1.m_bInputSignal)
                {
                    g_tCIO1.m_bInputSignal[i] = INPUT_CheckInputSignal(i);
                }
            }

        }
        //public static bool INPUT_CheckInputSignal
        public static bool INPUT_CheckInputSignal(int nOutputNumber)
        {
            bool bRet = false;

            if (nOutputNumber >= GLb.g_tInputCfg.Length)
            {
                return bRet;
            }
            Globals.TEnumIOType eType = (Globals.TEnumIOType)Enum.Parse(typeof(Globals.TEnumIOType), GLb.g_tInputCfg[nOutputNumber].sType, true);

            switch (eType)
            {

                case Globals.TEnumIOType.IOPLCHMI:
                    if (parentForm.devHandler.cPLCHandler != null)
                    {
                        bRet = parentForm.devHandler.cPLCHandler.INPUT_CheckInputSignal(nOutputNumber);
                    }
                    break;
                default:
                    break;
            }

            return bRet;
        }

        public static bool OUTPUT_CheckOutputSignal(int nOutputNumber, int PlcIndex = 99)
        {
            bool bRet = false;
            if (nOutputNumber >= GLb.g_tOutputCfg.Length)
            {
                return bRet;
            }

            Globals.TEnumIOType eType = (Globals.TEnumIOType)Enum.Parse(typeof(Globals.TEnumIOType), GLb.g_tOutputCfg[nOutputNumber].sType, true);

            switch (eType)
            {

                case Globals.TEnumIOType.IOPLCHMI:
                    if (parentForm.devHandler.cPLCHandler != null)
                    {
                        // bRet = parentForm.devHandler.cPLCHandler[0].OUTPUT_CheckOutputSignal(nOutputNumber);
                    }
                    break;
                case Globals.TEnumIOType.IODAQ:
                    break;
                default:
                    break;
            }

            return bRet;
        }

        public static bool OutPutSignalNormal(int nOutputNumber, bool bOnOff = true)
        {
            bool bRet = false;


            if (nOutputNumber >= GLb.g_tOutputCfg.Length)
            {
                return bRet;
            }

            if (parentForm.devHandler == null)
            {
                return bRet;
            }

            if (g_tCIO1.m_bOutputSignal[nOutputNumber] == bOnOff)
            {
                return true;
            }

            Globals.TEnumIOType eType = (Globals.TEnumIOType)Enum.Parse(typeof(Globals.TEnumIOType), GLb.g_tOutputCfg[nOutputNumber].sType, true);

            switch (eType)
            {

                case Globals.TEnumIOType.IOPLCHMI:
                    if (parentForm.devHandler.cPLCHandler != null)
                    {
                        bRet = parentForm.devHandler.cPLCHandler.OutPutSignalNormal(nOutputNumber, bOnOff);
                    }
                    break;

                default:
                    break;
            }


            return bRet;
        }

        public static void INIT_IO()
        {
            for (int i = 0; i < g_tCIO1.m_bInputSignal.Length; i++)
            {
                g_tCIO1.m_bInputSignal[i] = false;
            }
            ////output
            for (int i = 0; i < g_tCIO1.m_bOutputSignal.Length; i++)
            {
                g_tCIO1.m_bOutputSignal[i] = false;
            }
            // DEMO MODE====================================================

        }
        internal static bool ClampSample(bool bOnOff)
        {
            bool bRet = true;
            //var watch = System.Diagnostics.Stopwatch.StartNew();
            //long dwStTm = watch.ElapsedMilliseconds;

            //if (bOnOff)// clamp on bit when on sensor clamp
            //{
            //    bRet &= OutPutSignalNormal((int)Globals.k_OutputConfig.OUTRow_CYLINDER_ROB_CLAMP_MANUAL, bOnOff);
            //    while (!g_tCIO.m_bInputSignal[(int)Globals.k_InputConfig.INRow_SENSOR_CYLINDER_ROB_CLAMP])
            //    {
            //        if (dwStTm > 2000)
            //        {
            //            OutPutSignalNormal((int)Globals.k_OutputConfig.OUTRow_CYLINDER_ROB_CLAMP_MANUAL, !bOnOff);
            //            Thread.Sleep(500);
            //            bRet = false;
            //            break;
            //        }
            //        Thread.Sleep(10);
            //        dwStTm = watch.ElapsedMilliseconds;
            //    }
            //    watch.Stop();
            //    //OutPutSignalNormal((int)Globals.k_OutputConfig.OUTRow_CYLINDER_ROB_CLAMP_MANUAL, !bOnOff);
            //    return bRet;
            //}
            //else
            //{
            //    bRet &= OutPutSignalNormal((int)Globals.k_OutputConfig.OUTRow_CYLINDER_ROB_CLAMP_MANUAL, bOnOff);
            //    while (!g_tCIO.m_bInputSignal[(int)Globals.k_InputConfig.INRow_SENSOR_CYLINDER_ROB_UNCLAMP])
            //    {
            //        if (dwStTm > 5000)
            //        {
            //            bRet = false;
            //            break;
            //        }
            //        Thread.Sleep(10);
            //        dwStTm = watch.ElapsedMilliseconds;
            //    }
            //    watch.Stop();
            //    //OutPutSignalNormal((int)Globals.k_OutputConfig.OUTRow_CYLINDER_ROB_CLAMP_MANUAL, !bOnOff);
            return bRet;

            //}
        }

        //public static bool bRobotReady
        //{
        //    get { return (g_tCIO.m_bOutputSignal[(int)Globals.k_OutputConfig_S1.OUTRow_ROBOT_READY]); }
        //}

        internal static bool GetInputPin(int nIndex)
        {

            return g_tCIO1.m_bInputSignal[nIndex];

        }

        internal static bool GetOutputPin(int nIndex, int PlcIndex)
        {
            return g_tCIO1.m_bOutputSignal[nIndex];
        }

        internal static bool SetPulseOutput(int nOutput, bool bOnOff = true, int ndelay = 500)
        {

            //new Thread(() =>
            //{
            //    if (g_tCIO.m_bOutputSignal[nOutput] == bOnOff)
            //    {
            //        OutPutSignalNormal(nOutput, !bOnOff);
            //    }

            //    OutPutSignalNormal(nOutput, bOnOff);
            //    Thread.Sleep(10);
            //    OutPutSignalNormal(nOutput, bOnOff);
            //    Thread.Sleep(ndelay);
            //    OutPutSignalNormal(nOutput, !bOnOff);
            //    Thread.Sleep(10);
            //    OutPutSignalNormal(nOutput, !bOnOff);
            //}).Start();


            return true;
        }

        public static bool bBarcodeSignal
        {
            //get { return (g_tCIO.m_bInputSignal[(int)Globals.k_InputConfig.INRow_BARCODE_SIGNAL]); }
            get { return false; }
        }
    }

    public class TCIO
    {
        private bool[] m_InputSignal;
        private bool[] m_OutputSignal;

        public bool[] m_bInputSignal
        {
            get
            {
                return m_InputSignal;
            }
            set
            {
                m_InputSignal = value;
            }
        }
        public bool[] m_bOutputSignal
        {
            get
            {
                return m_OutputSignal;
            }
            set
            {
                m_OutputSignal = value;
            }
        }

        public TCIO()
        {
            m_bInputSignal = new bool[(int)Globals.k_InputConfig.INRow_End];
            m_bOutputSignal = new bool[(int)Globals.k_OutputConfig.OUTRow_End];
        }
    }

}
