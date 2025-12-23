using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VTP_Induction.Common;
using VTP_Induction.Device;
using VTP_Induction.UI;

namespace VTP_Induction
{
    public partial class TForm : Form
    {
        #region Fields & State
        Globals GLb = Globals.getInstance();
        string[] itemss;
        ListViewItem lvItem;

        public DeviceHandler devHandler;
        private WcsHttpServer _server;

        public Globals.TDevUtilDeviceConfig m_ptDevConfig;
        public Globals.TDevUtilDeviceConfig m_ptDevConfig1;
        private int nIndexDev = 0;
        private delegate void delegateAppendText(string str);
        public IotWarehouse.IOTWareHouseDAL IOTUpdate = new IotWarehouse.IOTWareHouseDAL();
        public frmConfig frmCfg;
        public string g_sHomeDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Config\\Gridview_data.txt";
        private int nIndexPostion = -1;
        private string baseUrl = "http://192.168.110.189";

        #endregion

        #region Construction & Lifecycle

        public TForm()
        {
            InitializeComponent();

            SplashScreenManager.ShowForm(this, typeof(CompSplashScreen), true, true, false);
            //devHandler = new DeviceHandler(this);

            try
            {
                Log.LoadData();
                SplashScreenManager.Default.SendCommand(
                    CompSplashScreen.SplashScreenCommand.SetLabel,
                    "Load All data...."
                );
                GLb.LoadAllData();
                devHandler = new DeviceHandler(this);
                SplashScreenManager.Default.SendCommand(
                    CompSplashScreen.SplashScreenCommand.SetLabel,
                    "Initialize UI...."
                );
                GLb.LoadSystemConfig();

                SplashScreenManager.Default.SendCommand(
                    CompSplashScreen.SplashScreenCommand.SetLabel,
                    "Load BootUp data...."
                );
                GLb.BootUpDataLoad();

                SplashScreenManager.Default.SendCommand(
                    CompSplashScreen.SplashScreenCommand.SetLabel,
                    "Load PLC Configuration data...."
                );

                GLb.LoadMotionConfig();

                CIOInterface.LoadData(this);
                InitGridView();
                InitComboBox();
                DataToUI_MotionPLCControl();
                DataToUI_PLCConfig();
                startupFolder();
                timer1.Enabled = true;
                this.frmCfg = new frmConfig(this);
                GLb.g_bSQLCheck = true;
                //ServerWCS = new JsonServer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                writeLog("ERROR :" + ex.ToString());
            }
            finally
            {
                Thread.Sleep(500);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                InitAllPanelDevDiagnostics();
            }
        }

        private void TForm_Load(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    _server = new WcsHttpServer(8500);

                    _server.Log += msg =>
                    {
                        if (this.InvokeRequired)
                            this.BeginInvoke(new Action(() => writeLog(msg)));
                        else
                            writeLog(msg);
                    };

                    _server.OrderReceived += data =>
                    {
                        if (this.InvokeRequired)
                            this.BeginInvoke(new Action(() => HandleOrder(data)));
                        else
                            HandleOrder(data);
                    };

                    _server.PoReceived += data =>
                    {
                        if (this.InvokeRequired)
                            this.BeginInvoke(new Action(() => HandleDonePallet(data)));
                        else
                            HandleDonePallet(data);
                    };

                    _server.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Server start error! Please check again! " + ex.Message);
                    writeLog("Khởi động server lỗi: " + ex.Message);
                }

                ResizeListViewColumns();

                LoadDataFromSqlToListView();
                labelNameSoftware.Text = GLb.g_SoftwareNameVersion;
                PrintQueueHelper.ResetToFirstWaitingItem(lvPrintList);

                LoadProductionData();
                InitPlan();
                devHandler.DevConnect(true);
            }
            catch (Exception ex)
            {
                writeLog("Lỗi khởi động: " + ex.Message);
                MessageBox.Show("Lỗi khởi động. Vui lòng tắt chương trình và kiểm tra lại hệ thống!!!: " + ex.Message, "CẢNH BÁO NGHIÊM TRỌNG!!!", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
            }
        }

        private void UpdateCounterUI()
        {
            lblPass.Text = GLb.nPassItems + "/" + GLb.nTotalItems;
            lblFail.Text = GLb.nFailItems + "/" + GLb.nTotalItems;

            lblPerPass.Text =
                (GLb.nTotalItems > 0)
                    ? (GLb.nPassItems * 100.0 / GLb.nTotalItems).ToString("F1") + "%"
                    : "0.0%";

            lblPerFail.Text =
                (GLb.nTotalItems > 0)
                    ? (GLb.nFailItems * 100.0 / GLb.nTotalItems).ToString("F1") + "%"
                    : "0.0%";
        }

        public enum BtnState
        {
            RUNNING,
            STOP,
            MANUAL,
            CONFIG,
        }

        private void ResizeListViewColumns()
        {
            int totalWidth = lvPrintList.ClientSize.Width;
            lvPrintList.Columns[0].Width = totalWidth * 10 / 100; // Cột STT
            lvPrintList.Columns[1].Width = totalWidth * 30 / 100; // Cột Mã Định Danh
            lvPrintList.Columns[2].Width = totalWidth * 30 / 100; // Cột Trạng Thái
            lvPrintList.Columns[3].Width = totalWidth * 20 / 100; // Cột mã pallet
        }

        public void startupFolder()
        {
            try
            {
                // Directory.CreateDirectory is idempotent (safe if folder already exists)
                Directory.CreateDirectory(GLb.g_sLogDirData);

                var imageDir = Path.Combine(GLb.g_sLogDirData, "Image", GLb.fileNameSaveImage);
                var logDir = Path.Combine(GLb.g_sLogDirData, "Log");

                Directory.CreateDirectory(imageDir);
                Directory.CreateDirectory(logDir);

                var logFile = Path.Combine(logDir, GLb.fileNameSaveImage + ".txt");
                if (!File.Exists(logFile))
                {
                    File.WriteAllText(logFile, string.Empty);
                }
            }
            catch (Exception ex)
            {
                // tránh throw ở giai đoạn khởi động
                writeLog("[startupFolder] " + ex);
            }
        }

        private void InsertEvent(int eventType, string text)
        {
            try
            {
                if (lvlog.InvokeRequired)
                {
                    lvlog.BeginInvoke(
                        new MethodInvoker(() => InsertListView(lvlog, eventType, text))
                    );
                }
                else
                {
                    InsertListView(lvlog, eventType, text);
                }

                MakeLogFile.WriteSystemEvent(text);
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
            }
        }

        private void InsertListView(ListView _listView, int _eventType, string _text)
        {
            itemss = new string[2];
            itemss[0] = DateTime.Now.ToString("HH:mm:ss-fff");
            itemss[1] = _text;
            lvItem = new ListViewItem(itemss);

            switch (_eventType)
            {
                case 1:
                    break;
                case 2:
                    lvItem.ForeColor = Color.Red;
                    break;
                default:
                    lvItem.BackColor = Color.Black;
                    break;
            }
            _listView.Items.Insert(0, lvItem);
            //WriteLogToFile(itemss);
        }

        #endregion

        #region Logging

        public void writeLog(string dataLog)
        {
            try
            {
                InsertEvent(1, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " : " + dataLog);
            }
            catch (Exception ex)
            {
                string x = ex.ToString();
            }
        }

        public void LoadProductionData()
        {
            lblCountParcel.Text = GLb.nParcelDone + "/" + GLb.g_tSysCfg.nParcelNumber;
            lblCountPallet.Text = GLb.nPalletDone + "/" + GLb.g_tSysCfg.nPalletNumber;

            int total, pass, fail;
            CounterService.Load(out total, out pass, out fail);

            GLb.nTotalItems = total;
            GLb.nPassItems = pass;
            GLb.nFailItems = fail;

            UpdateCounterUI();
        }

        public void RunProcessCaptureImage()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += delegate
            {
                MainProcessBarcode();
            };
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                ProcessCaptureImageCompleted
            );
            worker.RunWorkerAsync();
        }

        public void ProcessCaptureImageCompleted(object sender, RunWorkerCompletedEventArgs e) { }

        public class PlanConfig
        {
            public int ParcelQuantity { get; set; }
            public int TotalParcel { get; set; }
            public int PalletQuantity { get; set; }
            public string ProductionName { get; set; }
            public int ParcelRealTimeNumber { get; set; }
            public int PalletRealTimeNumber { get; set; }
            public string Position { get; set; }
        }

        public PlanConfig LoadPlanConfig()
        {
            PlanConfig config = new PlanConfig();

            string query = @"
SELECT TOP 1
    Ctn,
    Item_Code,
    Inner_Pallet,
    TotalParcel,
    TotalPallet,
    RealtimeParcel,
    RealTimePallet,
    Position
FROM dbo.WCS_PLAN_CONFIG_Temp
ORDER BY Id ASC";

            using (SqlConnection conn = new SqlConnection(GLb.g_tSQLConfig.SqlString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // INT → NULL = 0
                        config.ParcelQuantity =
                            reader["Ctn"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Ctn"]);

                        config.TotalParcel =
                            reader["TotalParcel"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TotalParcel"]);

                        config.PalletQuantity =
                            reader["TotalPallet"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TotalPallet"]);

                        config.ParcelRealTimeNumber =
                            reader["RealtimeParcel"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RealtimeParcel"]);

                        config.PalletRealTimeNumber =
                            reader["RealTimePallet"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RealTimePallet"]);

                        // STRING → NULL = ""
                        config.ProductionName =
                            reader["Item_Code"] == DBNull.Value ? string.Empty : reader["Item_Code"].ToString();

                        config.Position =
                            reader["Position"] == DBNull.Value ? string.Empty : reader["Position"].ToString().Trim();
                    }
                }
            }

            return config;
        }

        public void InitPlan()
        {
            PlanConfig plan = LoadPlanConfig();

            GLb.nParcelDone = plan.ParcelRealTimeNumber;
            GLb.nPalletDone = plan.PalletRealTimeNumber;
            GLb.nTotalParcelAll = plan.TotalParcel;
            GLb.nTotalPallet = plan.PalletQuantity;
            _parcelPerPallet = plan.ParcelQuantity;
            GLb.nTotalParcel = plan.ParcelQuantity;

            //UI Update
            lblProductionName.Text = "SẢN PHẨM: " + plan.ProductionName ?? string.Empty;
            SetLabelText(lblCountParcel, GLb.nParcelDone + "/" + GLb.nTotalParcel, Color.Aqua);
            SetLabelText(lblCountPallet, GLb.nPalletDone + "/" + GLb.nTotalPallet, Color.Aqua);

            for(int i = 0; i < GLb.g_tSysCfg.sPositions.Length; i++)
            {
                if(GLb.g_tSysCfg.sPositions[i] == plan.Position)
                {
                    GetLocalPositionPallet(i);
                    break;
                }
                GetLocalPositionPallet(-1);
            }
        }

        private void GetLocalPositionPallet(int index)
        {
            switch (index)
            {
                case 0:
                    timerBlink.Start();
                    PosPanTemp = plPos01;
                    nIndexPostion = 0;
                    plPos01.BackColor = Color.LightGreen;
                    plPos02.BackColor = Color.DarkGray;
                    plPos03.BackColor = Color.DarkGray;
                    lblRealPos1.Text = GLb.g_tSysCfg.sPositions[index];
                    lblRealPos2.Text = "";
                    lblRealPos3.Text = "";
                    break;
                case 1:
                    timerBlink.Start();
                    PosPanTemp = plPos02;
                    nIndexPostion = 1;
                    plPos02.BackColor = Color.LightGreen;
                    plPos01.BackColor = Color.DarkGray;
                    plPos03.BackColor = Color.DarkGray;
                    lblRealPos2.Text = GLb.g_tSysCfg.sPositions[index];
                    lblRealPos1.Text = "";
                    lblRealPos3.Text = "";
                    break ;
                case 2:
                    timerBlink.Start();
                    PosPanTemp = plPos03;
                    nIndexPostion = 2;
                    plPos03.BackColor = Color.LightGreen;
                    plPos02.BackColor = Color.DarkGray;
                    plPos01.BackColor = Color.DarkGray;
                    lblRealPos3.Text = GLb.g_tSysCfg.sPositions[index];
                    lblRealPos2.Text = "";
                    lblRealPos1.Text = "";
                    break;
                default:
                    timerBlink.Stop();
                    nIndexPostion = -1;
                    plPos01.BackColor = Color.DarkGray;
                    plPos02.BackColor = Color.DarkGray;
                    plPos03.BackColor = Color.DarkGray;
                    lblRealPos1.Text = "";
                    lblRealPos2.Text = "";
                    lblRealPos3.Text = "";
                    break;
            }
        }

        //private static int CalculateTargetParcelThisPallet(int palletDone, int totalPallet, int totalParcelAll, int parcelPerPallet)
        //{
        //    if (totalPallet <= 0 || parcelPerPallet <= 0)
        //        return 0;

        //    if (palletDone >= totalPallet)
        //        return 0;

        //    if (palletDone == totalPallet - 1)
        //    {
        //        int remaining = totalParcelAll - (parcelPerPallet * palletDone);
        //        return Math.Max(0, remaining);
        //    }
        //    return parcelPerPallet;
        //}

        private void AppendText(string text)
        {
            try
            {
                if (richTextBoxLog.Lines.Count() > 100)
                {
                    richTextBoxLog.Clear();
                }

                if (richTextBoxLog.InvokeRequired)
                {
                    delegateAppendText method = AppendText;
                    Invoke(method, text);
                    return;
                }
                richTextBoxLog.AppendText(text);
                if (richTextBoxLog.Lines.Count() > 100)
                {
                    richTextBoxLog.Clear();
                    lvlog.Items.Clear();
                }
                richTextBoxLog.ScrollToCaret();
            }
            catch { }
        }

        private Thread m_hReceiveThread = null;

        string ScaleRaw;
        private volatile bool _isPrinting = false;
        private int _parcelPerPallet = 0;

        public void MainProcessBarcode()
        {
            _isPrinting = false;
            GLb.g_bGrabbing = true;
            int finalWeight = 0;
            try
            {
                while (GLb.g_bGrabbing)
                {
                    if (GLb.PalletInProgress && !GLb.PalletScanCompleted)
                    {
                        SetLabelText(lblPushInformation, "ĐANG CHỜ QUÉT PALLET: " + GLb.CurrentPalletID, Color.OrangeRed);
                        Thread.Sleep(200);
                        continue;
                    }
                    // Read scale value
                    string ScaleRaw = devHandler.cScale.ReadScaleDataFormCom();
                    string digitsOnly = new string(ScaleRaw.Where(char.IsDigit).ToArray());
                    int scaleValue = 0;
                    if (!int.TryParse(digitsOnly, out scaleValue))
                    {
                        continue;
                    }

                    // If the scale value is less than 10, reset process
                    if (scaleValue < 20)
                    {
                        SetLabelText(lblPushInformation, "CHỜ ĐẶT SẢN PHẨM LÊN CÂN", Color.Gray);
                        SetLabelText(labelStatus, "", Color.Black);
                        devHandler.cPLCHandler.SetTrafficLightByM(2); //Yellow
                        Thread.Sleep(100);
                        _isPrinting = false;
                        continue;
                    }

                    if (_isPrinting)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    _isPrinting = true;
                    SetLabelText(lblPushInformation, "CÂN SẢN PHẨM...", Color.OrangeRed);

                    bool weightOK = IsStableWeight(out finalWeight, GLb.g_tSysCfg.nScaleValue, GLb.g_tSysCfg.nScaleError, GLb.g_tSysCfg.nTimeScale, 200);
                    if (!weightOK)
                    {
                        SetLabelText(labelStatus, "NG", Color.Red);
                        SetLabelText(lblPushInformation, "SẢN PHẨM KHÔNG ĐẠT, VUI LÒNG BỎ RA KHỎI CÂN", Color.Red);
                        devHandler.cPLCHandler.SetTrafficLightByM(0); //RED
                        WaitForZeroWeight();
                        UpdateItemTotal(false);
                        continue;
                    }

                    SetLabelText(labelStatus, "OK", Color.Lime);
                    Thread.Sleep(500);

                    SetLabelText(lblPushInformation, "ĐANG IN TEM...", Color.OrangeRed);

                    string parcelCode = PrintQueueHelper.GetNextParcelCode(lvPrintList);
                    string palletCode = GLb.CurrentPalletID;
                    string ItemName = GLb.CurrentItemCode;
                    string WH_Code = GLb.CurrentWH_Code;

                    SetLabelText(lblPalletCode,"MÃ PALLET: " + palletCode, Color.LightYellow);
                    if (string.IsNullOrEmpty(parcelCode))
                    {
                        SetLabelText(lblPushInformation, "HẾT DANH SÁCH IN / CHƯA CÓ THÙNG CHỜ IN", Color.Gray);
                        _isPrinting = false;
                        Thread.Sleep(500);
                        continue;
                    }

                    bool printOK = devHandler.cPrinterGodex.PrintBarcode(parcelCode, ItemName, palletCode, WH_Code, finalWeight.ToString());
                    //bool printOK = true;
                    Thread.Sleep(1500);

                    if (!printOK)
                    {
                        SetLabelText(labelStatus, "NG", Color.Red);
                        SetLabelText(lblPushInformation, "IN TEM THẤT BẠI, VUI LÒNG BỎ RA KHỎI CÂN", Color.Red);
                        devHandler.cPLCHandler.SetTrafficLightByM(0); //RED
                        WaitForZeroWeight();
                        _isPrinting = false;
                        UpdateItemTotal(false);
                        PrintQueueHelper.MarkFail(lvPrintList);
                        continue;
                    }

                    // Step 3: Read barcode
                    string BarcodeTemp = "";
                    SetLabelText(lblPushInformation, "ĐANG ĐỌC MÃ VẠCH...", Color.OrangeRed);
                    bool readOK = WaitForBarcodeRead(out BarcodeTemp, 5000);
                    if (readOK && BarcodeTemp == parcelCode)
                    //if (readOK && BarcodeTemp == "TP0231")
                    {
                        _isPrinting = false;
                        SetLabelText(labelStatus, "OK", Color.Lime);
                        SetLabelText(lblPushInformation, "MÃ VẠCH:" + BarcodeTemp, Color.GreenYellow);
                        UpdateItemTotal(true);
                        AppendText("[PROCESS] ĐÃ HOÀN THÀNH IN THÙNG" + BarcodeTemp + "\r");
                        devHandler.cPLCHandler.SetTrafficLightByM(1); //Green
                    }
                    else
                    {
                        SetLabelText(labelStatus, "NG", Color.Red);
                        SetLabelText(lblPushInformation, "KHÔNG ĐỌC ĐƯỢC MÃ VẠCH - BỎ RA KHỎI CÂN", Color.Red);
                        devHandler.cPLCHandler.SetTrafficLightByM(0); //RED
                        UpdateItemTotal(false);
                        PrintQueueHelper.MarkFail(lvPrintList);
                        _isPrinting = false;
                        WaitForZeroWeight();
                        continue;
                    }

                    PrintQueueHelper.MarkSuccess(lvPrintList);
                    PrintQueueHelper.UpdateStatusInDatabase(parcelCode, 1);

                    bool isLastParcel = PrintQueueHelper.IsLastPrintedParcel(lvPrintList);
                    Thread.Sleep(100);

                    if (GLb.nParcelDone >= GLb.nTotalParcel || (isLastParcel && GLb.nParcelDone > 0))
                    {
                        try
                        {
                            TryPrintAndSendPalletManual(palletCode);
                        }
                        catch (Exception e)
                        {
                            SetLabelText(lblPushInformation, "PALLET KHÔNG XỬ LÝ ĐƯỢC: " + e.Message, Color.Red);
                            devHandler.cPLCHandler.SetTrafficLightByM(0); // Red
                            GLb.PalletScanCompleted = false;
                        }
                    }

                    _isPrinting = false;
                    Thread.Sleep(2000);
                    WaitForZeroWeight();
                }
            }
            catch (Exception ex)
            {
                SetLabelText(lblPushInformation, "LỖI HỆ THỐNG: " + ex.Message, Color.Red);
                devHandler.cPLCHandler.SetTrafficLightByM(0); //Red
                GLb.PalletScanCompleted = false;
            }
        }

        private bool FinishProduction()
        {
            try
            {
                string json = "";
                var res = TryBuildPalletJsonEndOfTurn(GLb.CurrentPalletID, out json);
                string err;
                using (var cts = new CancellationTokenSource())
                {
                    bool postOK = TryPostJsonOnce(
                        baseUrl + ":8060/identify-service/ids/pallet_identify_done",
                        json,
                        out err
                    );

                    if (!postOK)
                    {
                        MessageBox.Show("GỬI DỮ LIỆU PALLET VỀ SERVER LỖI!", "WARNING", MessageBoxButtons.OKCancel);
                        return false;
                    }
                }

                // 1) Reset biến nghiệp vụ
                GLb.nTotalParcel = 0;
                GLb.nParcelDone = 0;
                GLb.PalletInProgress = false;
                GLb.PalletScanCompleted = false;
                GLb.CurrentPalletID = "";
                GLb.IsInTask = false;

                //// 2) Clear task tạm trong DB
                //ClearTaskTable();

                // 3) Update UI + STOP (thread-safe)
                Action uiAction = () =>
                {
                    try
                    {
                        // Stop process
                        buttonSTOP_Click(null, null);

                        // Reload dữ liệu
                        LoadDataFromSqlToListView();
                        var plan = LoadPlanConfig();
                        LoadProductionData();
                        InitPlan();

                        SetLabelText(lblPushInformation, "ĐÃ HOÀN TẤT LỆNH SẢN XUẤT", Color.Green);
                        SetLabelText(lblCountParcel, "0/0", Color.Aqua);
                        SetLabelText(lblCountPallet,
                            GLb.nPalletDone + "/" + GLb.nTotalPallet,
                            Color.Aqua
                        );

                        devHandler.cPLCHandler.SetTrafficLightByM(1);
                    }
                    catch (Exception ex)
                    {
                        SetLabelText(lblPushInformation, "LỖI KẾT THÚC SX: " + ex.Message, Color.Red);
                        devHandler.cPLCHandler.SetTrafficLightByM(0);
                    }
                };

                if (this.InvokeRequired)
                    this.BeginInvoke(uiAction);
                else
                    uiAction();

                return true;
            }
            catch (Exception ex)
            {
                SetLabelText(lblPushInformation, "LỖI HỆ THỐNG (FinishProduction): " + ex.Message, Color.Red);
                devHandler.cPLCHandler.SetTrafficLightByM(0);
                return false;
            }
        }

        private void TryPrintAndSendPalletManual(string palletCode)
        {
            if (string.IsNullOrWhiteSpace(palletCode))
                throw new Exception("MÃ PALLET KHÔNG HỢP LỆ");

            const int MAX_READ_TRY = 3;

            try
            {
                /* ================= INIT PALLET ================= */
                GLb.PalletInProgress = true;
                GLb.PalletScanCompleted = false;

                /* ================= PRINT + SCAN ================= */
                if (GLb.g_tSysCfg.bPrintPallet)
                {
                    SetLabelText(lblPushInformation, "IN MÃ PALLET: " + palletCode, Color.Orange);

                    if (!TryPrintPalletSafe(palletCode))
                        throw new Exception("IN PALLET LỖI");

                    string readCode;
                    if (!TryReadPalletSafe(palletCode, MAX_READ_TRY, out readCode))
                        throw new Exception("LỖI ĐỌC MÃ PALLET");
                }

                /* ================= UPDATE STATUS: WAIT → SEND ================= */
                PrintQueueHelper.UpdateStatusByPalletId(palletCode, 4);

                /* ================= BUILD JSON ================= */
                string json;
                if (!TryBuildPalletJsonEndOfTurn(palletCode, out json))
                    throw new Exception("LỖI BUILD JSON PALLET");

                /* ================= POST SERVER (BLOCK) ================= */
                string err;
                using (var cts = new CancellationTokenSource())
                {
                    bool ok = PostJsonBlockUntilSuccess(
                        baseUrl + ":8060/identify-service/ids/pallet_identify_done",
                        json,
                        30000,
                        cts.Token,
                        out err
                    );

                    if (!ok)
                        throw new Exception("GỬI PALLET LỖI: " + err);
                }

                /* ================= UPDATE DONE ================= */
                PrintQueueHelper.UpdateStatusByPalletId(palletCode, 5);

                GLb.nPalletDone++;
                GLb.nParcelDone = 0;

                UpdatePlanRealtime(GLb.nParcelDone, GLb.nPalletDone);

                /* ================= UI + RESET ================= */
                Action ui = () =>
                {
                    try
                    {
                        buttonSTOP_Click(null, null);
                        SetLabelText(lblPushInformation, "PALLET OK – TIẾP TỤC SẢN XUẤT", Color.Green);
                        SetLabelText(lblCountParcel, "0/" + GLb.nTotalParcel, Color.Aqua);
                        SetLabelText(lblCountPallet, GLb.nPalletDone + "/" + GLb.nTotalPallet, Color.Aqua);
                        devHandler.cPLCHandler.SetTrafficLightByM(1);
                    }
                    catch (Exception ex)
                    {
                        SetLabelText(lblPushInformation, "LỖI UI: " + ex.Message, Color.Red);
                        devHandler.cPLCHandler.SetTrafficLightByM(0);
                    }
                    finally
                    {
                        /* RESET STATE – 1 CHỖ DUY NHẤT */
                        GLb.PalletInProgress = false;
                        GLb.PalletScanCompleted = false;
                        GLb.nParcelDone = 0;
                        GLb.nTotalParcel = 0;
                    }
                };

                if (InvokeRequired) BeginInvoke(ui);
                else ui();
            }
            catch (Exception ex)
            {
                SetLabelText(lblPushInformation, ex.Message, Color.Red);
                devHandler.cPLCHandler.SetTrafficLightByM(0);
                GLb.PalletInProgress = false;
                GLb.PalletScanCompleted = false;
                throw new Exception(ex.Message);
            }
        }

        private bool TryPrintAndSendOnce(string palletCode)
        {
            if (string.IsNullOrWhiteSpace(palletCode))
                return false;

            const int MAX_READ_TRY = 3;

            try
            {
                /* ================= INIT PALLET ================= */
                GLb.PalletInProgress = true;
                GLb.PalletScanCompleted = false;

                /* ================= PRINT + SCAN ================= */
                if (GLb.g_tSysCfg.bPrintPallet)
                {
                    SetLabelText(lblPushInformation, "IN MÃ PALLET: " + palletCode, Color.Orange);

                    if (!TryPrintPalletSafe(palletCode))
                        return false;

                    string readCode;
                    if (!TryReadPalletSafe(palletCode, MAX_READ_TRY, out readCode))
                        return false;
                }

                /* ================= UPDATE STATUS: WAIT → SEND ================= */
                PrintQueueHelper.UpdateStatusByPalletId(palletCode, 4);

                /* ================= BUILD JSON ================= */
                string json;
                if (!TryBuildPalletJsonEndOfTurn(palletCode, out json))
                    return false;

                /* ================= POST SERVER (BLOCK) ================= */
                string err;
                using (var cts = new CancellationTokenSource())
                {
                    bool ok = TryPostJsonOnce(
                        baseUrl + ":8060/identify-service/ids/pallet_identify_done",
                        json,
                        out err
                    );

                    if (!ok)
                        return false;
                }

                /* ================= UPDATE DONE ================= */
                PrintQueueHelper.UpdateStatusByPalletId(palletCode, 5);

                GLb.nPalletDone++;
                GLb.nParcelDone = 0;

                UpdatePlanRealtime(GLb.nParcelDone, GLb.nPalletDone);

                /* ================= UI + RESET ================= */
                Action ui = () =>
                {
                    try
                    {
                        buttonSTOP_Click(null, null);

                        SetLabelText(lblPushInformation, "PALLET OK – TIẾP TỤC SẢN XUẤT", Color.Green);
                        SetLabelText(lblCountParcel, "0/" + GLb.nTotalParcel, Color.Aqua);
                        SetLabelText(lblCountPallet, GLb.nPalletDone + "/" + GLb.nTotalPallet, Color.Aqua);
                        devHandler.cPLCHandler.SetTrafficLightByM(1);
                    }
                    catch (Exception ex)
                    {
                        SetLabelText(lblPushInformation, "LỖI UI: " + ex.Message, Color.Red);
                        devHandler.cPLCHandler.SetTrafficLightByM(0);
                    }
                    finally
                    {
                        /* RESET STATE – 1 CHỖ DUY NHẤT */
                        GLb.PalletInProgress = false;
                        GLb.PalletScanCompleted = false;
                        GLb.nParcelDone = 0;
                        GLb.nTotalParcel = 0;
                    }
                };

                if (InvokeRequired) BeginInvoke(ui);
                else ui();

                return true;
            }
            catch (Exception ex)
            {
                SetLabelText(lblPushInformation, ex.Message, Color.Red);
                devHandler.cPLCHandler.SetTrafficLightByM(0);
                GLb.PalletInProgress = false;
                GLb.PalletScanCompleted = false;
                return false;
            }
        }

        private bool TryPrintPalletSafe(string palletCode)
        {
            try
            {
                if (devHandler == null || devHandler.cPrinterGodex == null)
                    return false;

                bool ok = devHandler.cPrinterGodex.PrintPallet(palletCode);

                Thread.Sleep(100);
                return ok;
            }
            catch
            {
                return false;
            }
        }

        private bool TryReadPalletSafe(string expected, int maxTry, out string readCode)
        {
            readCode = "";

            for (int i = 1; i <= maxTry; i++)
            {
                try
                {
                    SetLabelText(lblPushInformation, "ĐANG ĐỌC MÃ PALLET... (" + i + "/" + maxTry + ")", Color.Orange);

                    string tmp;
                    bool ok = WaitForBarcodeRead(out tmp, 5000);

                    if (ok && string.Equals(tmp, expected, StringComparison.OrdinalIgnoreCase))
                    {
                        readCode = tmp;
                        return true;
                    }
                }
                catch
                {
                    // nuốt lỗi để thử lại
                }

                Thread.Sleep(100);
            }

            return false;
        }

        private bool TryBuildPalletJsonEndOfTurn(string palletId, out string json)
        {
            try
            {
                string query = @"SELECT TOP 1 Task_ID, PO_ID, Line_ID, WH_Code FROM WCS_Pallet_Prod Where Pallet_ID = @PalletId";
                string wh_Code = "";
                long po_ID = 0;
                string lineId = "";
                string taskId = "";
                using (SqlConnection conn = new SqlConnection(GLb.g_tSQLConfig.SqlString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PalletId", palletId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                taskId = reader["Task_ID"].ToString().Trim();
                                po_ID =  Convert.ToInt64(reader["PO_ID"]);
                                lineId = reader["Line_ID"].ToString();
                                wh_Code = reader["WH_Code"].ToString();
                            }
                        }
                    }
                }

                var jsonObject = new
                {
                    Task_ID = taskId,
                    PO_ID = po_ID,
                    WH_Code = wh_Code,
                    Line_ID = lineId,
                    palletID = palletId,
                };

                json = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
                return true;
            }
            catch (Exception ex)
            {
                SetLabelText(lblPushInformation, "LỖI TẠO JSON KẾT THÚC PHIÊN: " + ex.Message, Color.Red);
                json = "";
                return false;
            }
        }

        private static readonly HttpClient _client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        private bool TryPostJsonOnce(string url, string json, out string err)
        {
            err = "";
            try
            {
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    // KHÔNG dùng .Result để tránh AggregateException
                    var resp = _client.PostAsync(url, content)
                                      .GetAwaiter()
                                      .GetResult();

                    if (resp == null)
                    {
                        err = "No response";
                        return false;
                    }

                    if (!resp.IsSuccessStatusCode)
                    {
                        err = ((int)resp.StatusCode) + " " + resp.ReasonPhrase;
                        return false;
                    }

                    return true;
                }
            }
            catch (TaskCanceledException ex)
            {
                // Gần như chắc chắn là timeout (HttpClient.Timeout)
                err = "Timeout / TaskCanceled: " + ex.Message;
                return false;
            }
            catch (HttpRequestException ex)
            {
                err = "HttpRequestException: " + ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
        }

        private bool PostJsonBlockUntilSuccess(string url,string json,int retryDelayMs,CancellationToken token,out string lastErr)
        {
            lastErr = "";

            while (!token.IsCancellationRequested)
            {
                string err;
                bool ok = TryPostJsonOnce(url, json, out err);

                if (ok)
                    return true;

                lastErr = err;

                // Log + UI cảnh báo
                this.BeginInvoke((Action)(() =>
                {
                    SetLabelText(lblPushInformation, "KHÔNG GỬI ĐƯỢC SERVER – TẠM DỪNG DÂY CHUYỀN", Color.Yellow);
                    //devHandler.cPLCHandler.SetTrafficLightByM(0); // RED
                }));

                Thread.Sleep(retryDelayMs);
            }

            lastErr = "Cancelled";
            return false;
        }

        private void WaitForZeroWeight()
        {
            // CẤU HÌNH
            const int THRESHOLD = 20;      // Ngưỡng an toàn (g) - Dưới mức này coi như đã lấy hàng
            const int REQUIRED_COUNT = 5;  // Số lần liên tiếp cần đạt (đếm 5 lần cho chắc)
            const int SLEEP_TIME = 100;    // Thời gian nghỉ giữa các lần đọc (ms)

            // Tổng thời gian xác nhận = 5 * 100ms = 0.5 giây (Rất nhanh nhưng cực an toàn)

            int stableCount = 0; // Biến đếm số lần thỏa mãn

            while (true)
            {
                // 1. Đọc dữ liệu từ cân
                string scaleRaw = devHandler.cScale.ReadScaleDataFormCom();

                // Lọc lấy số
                string digits = new string(scaleRaw.Where(char.IsDigit).ToArray());
                int weight = 0;

                // 2. Phân tích
                if (int.TryParse(digits, out weight))
                {
                    // Cập nhật UI (Để an toàn, dùng Invoke)
                    if (lblScaleValue.InvokeRequired) lblScaleValue.Invoke(new Action(() => lblScaleValue.Text = weight + "g"));
                    else lblScaleValue.Text = weight + "g";

                    // --- LOGIC CHÍNH ---
                    if (weight < THRESHOLD)
                    {
                        stableCount++; // Tăng biến đếm nếu dưới 20g

                        // Cập nhật trạng thái cho công nhân thấy
                        SetLabelText(lblPushInformation, "ĐANG LẤY HÀNG... (" + stableCount + "/" + REQUIRED_COUNT + ")", Color.Orange);

                        // Nếu đã đếm đủ số lần liên tiếp -> XÁC NHẬN ĐÃ LẤY RA
                        if (stableCount >= REQUIRED_COUNT)
                        {
                            SetLabelText(lblPushInformation, "SẴN SÀNG CHO SẢN PHẨM MỚI", Color.Green);
                            break; // Thoát vòng lặp
                        }
                    }
                    else
                    {
                        // QUAN TRỌNG: Nếu tự nhiên cân vọt lên > 20g (do chạm tay vào hoặc chưa lấy hẳn)
                        // Phải Reset biến đếm về 0 ngay lập tức để đếm lại từ đầu.
                        stableCount = 0;
                        SetLabelText(lblPushInformation, "VUI LÒNG LẤY HÀNG RA KHỎI CÂN", Color.Red);
                    }
                }
                else
                {
                    // Trường hợp đọc lỗi (chuỗi rỗng...), coi như không tính, không tăng đếm nhưng cũng không reset
                    // Hoặc tùy bạn, có thể reset stableCount = 0 nếu muốn khắt khe.
                }

                // Nghỉ một chút để không chiếm dụng CPU và đợi cân ổn định
                Thread.Sleep(SLEEP_TIME);
            }
        }

        private bool WaitForBarcodeRead(out string barcode, int timeoutMs = 6000)
        {
            barcode = "";
            var sw = Stopwatch.StartNew();

            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                string data = devHandler.cBarcode.ReadBarcoder().Trim();

                if (!string.IsNullOrEmpty(data))
                {
                    barcode = data;
                    return true;
                }

                Thread.Sleep(100);
            }

            return false;
        }

        private void SetLabelText(Label lbl, string text, Color color)
        {
            if (lbl.InvokeRequired)
            {
                lbl.Invoke(
                    new Action(() =>
                    {
                        lbl.Text = text;
                        lbl.ForeColor = color;
                    })
                );
            }
            else
            {
                lbl.Text = text;
                lbl.ForeColor = color;
            }
        }

        private bool IsStableWeight(out int finalWeight, int standardWeight, int allowedDiff, int durationSec = 2,int intervalMs = 200, int stabilityJitter = 15)
        {
            finalWeight = 0;

            int requiredStableSamples = Math.Max(1, (durationSec * 1000) / intervalMs);
            int stableCount = 0;
            int? previous = null;
            int maxTries = requiredStableSamples * 10;

            for (int i = 0; i < maxTries; i++)
            {
                string raw = devHandler.cScale.ReadScaleDataFormCom();

                if (string.IsNullOrWhiteSpace(raw))
                {
                    Thread.Sleep(intervalMs);
                    continue;
                }

                raw = raw.Trim();

                int weight; // <-- khai báo trước để dùng với out

                // Parse trực tiếp
                if (!int.TryParse(raw, out weight))
                {
                    // Fallback: lọc chỉ còn chữ số rồi parse
                    string digitsOnly = new string(raw.Where(char.IsDigit).ToArray());
                    if (!int.TryParse(digitsOnly, out weight))
                    {
                        Thread.Sleep(intervalMs);
                        continue;
                    }
                }

                // Cập nhật UI
                if (lblScaleValue.InvokeRequired)
                {
                    lblScaleValue.Invoke(new Action(() => lblScaleValue.Text = weight + " g"));
                }
                else
                {
                    lblScaleValue.Text = weight + " g";
                }

                // Kiểm tra ổn định liên tiếp
                if (previous.HasValue && Math.Abs(previous.Value - weight) <= stabilityJitter)
                {
                    stableCount++;
                }
                else
                {
                    stableCount = 1; // bắt đầu chuỗi ổn định mới
                }

                previous = weight;
                finalWeight = weight; // luôn giữ giá trị mới nhất

                // Đủ số mẫu ổn định để bao phủ durationSec
                if (stableCount >= requiredStableSamples)
                {
                    return Math.Abs(finalWeight - standardWeight) <= allowedDiff;
                }

                Thread.Sleep(intervalMs);
            }

            return false; // không đạt ổn định trong thời gian cho phép
        }

        public void UpdateDeviceStatus()
        {
            foreach (panelDevice item in panelDev1.Controls)
            {
                item.RefreshDev();
            }
            if (GLb.g_tDevCfg.tWMS.bActive)
            {
                if (devHandler.cWMS.m_bConnection)
                {
                    buttonWMSOnOff.BackColor = Color.GreenYellow;
                }
            }
            else
            {
                buttonWMSOnOff.BackColor = Color.OrangeRed;
            }

            #region RunMode
            if (buttonSTART.Tag.ToString() == "Running")
            {
                TimerInsp.Start();
                buttonInsp.Text = "Run Insp";
                buttonInsp.ForeColor = Color.GreenYellow;
            }
            else
            {
                TimerInsp.Stop();
                buttonInsp.Text = "Stop Insp";
                buttonInsp.Image = Properties.Resources.Indicator9;
                buttonInsp.ForeColor = Color.Gray;
                nCountImageInsp = 2;
            }
            #endregion
        }

        public void InitAllPanelDevDiagnostics()
        {
            labelNameSoftware.Text = GLb.g_SoftwareNameVersion;
            panelDev1.Controls.Clear();
            List<CDevice> lDevice = new List<CDevice>();

            foreach (CDevice item in devHandler.detectorHandler)
            {
                lDevice.Add(item);
            }
            if (GLb.g_tDevCfg.tPLC.bActive)
            {
                panelDevice dev = new panelDevice(
                    devHandler.cPLCHandler,
                    VTP_Induction.Properties.Resources.IoOff32x32,
                    VTP_Induction.Properties.Resources.IoOn32x32
                );
                dev.Dock = DockStyle.Right;
                panelDev1.Controls.Add(dev);
            }
            if (GLb.g_tDevCfg.tPrinterGodex.bActive)
            {
                panelDevice dev = new panelDevice(
                    devHandler.cPrinterGodex,
                    VTP_Induction.Properties.Resources.CameraOff32x32,
                    VTP_Induction.Properties.Resources.CameraOn32x32
                );
                dev.Dock = DockStyle.Right;
                panelDev1.Controls.Add(dev);
            }
            if (GLb.g_tDevCfg.tScale.bActive)
            {
                panelDevice dev = new panelDevice(
                    devHandler.cScale,
                    VTP_Induction.Properties.Resources.scale_off,
                    VTP_Induction.Properties.Resources.scale_onn
                );
                dev.Dock = DockStyle.Right;
                panelDev1.Controls.Add(dev);
            }
            if (GLb.g_tDevCfg.tBarcode.bActive)
            {
                panelDevice dev = new panelDevice(
                    devHandler.cBarcode,
                    VTP_Induction.Properties.Resources.BarcodeOff32x32,
                    VTP_Induction.Properties.Resources.BarcodeOn32x32
                );
                dev.Dock = DockStyle.Right;
                panelDev1.Controls.Add(dev);
            }
        }

        private int UpdateItemTotal(bool bSuccessRead)
        {
            GLb.nTotalItems++;

            if (bSuccessRead)
            {
                GLb.nPassItems++;
                GLb.nParcelDone++;
                //UpdateParcelRealtime(GLb.nParcelDone);
                UpdatePlanRealtime(GLb.nParcelDone, GLb.nPalletDone);
                SetLabelText(labelStatus, "OK", Color.LimeGreen);
                SetLabelText(lblPushInformation, "TEM IN THÀNH CÔNG", Color.LimeGreen);
            }
            else
            {
                GLb.nFailItems++;
                SetLabelText(labelStatus, "NG", Color.Red);
                SetLabelText(lblPushInformation, "LỖI THỰC HIỆN KHÔNG THÀNH CÔNG", Color.Red);
            }

            // Cập nhật tổng
            SetLabelText(
                lblPass,
                GLb.nPassItems.ToString() + "/" + GLb.nTotalItems.ToString(),
                Color.LimeGreen
            );
            SetLabelText(
                lblFail,
                GLb.nFailItems.ToString() + "/" + GLb.nTotalItems.ToString(),
                Color.Red
            );

            SetLabelText(
                lblCountParcel,
                GLb.nParcelDone.ToString() + "/" + GLb.nTotalParcel,
                Color.Aqua
            );

            //SetLabelText(lblCountPallet, GLb.nPalletDone.ToString() + "/" + GLb.nTotalPallet.ToString(), Color.Aqua);

            double percentPass = GLb.nPassItems * 100.0 / GLb.nTotalItems;
            double percentFail = GLb.nFailItems * 100.0 / GLb.nTotalItems;

            SetLabelText(lblPerPass, percentPass.ToString("F1") + "%", Color.LimeGreen);
            SetLabelText(lblPerFail, percentFail.ToString("F1") + "%", Color.Red);

            CounterService.Save(GLb.nTotalItems, GLb.nPassItems, GLb.nFailItems);
            Thread.Sleep(10);
            return bSuccessRead ? 1 : 0;
        }

        private void UpdatePlanRealtime(int parcelDone, int palletDone)
        {
            try
            {
                string query = @"UPDATE dbo.WCS_PLAN_CONFIG_Temp SET RealtimeParcel = @parcel, RealtimePallet = @pallet WHERE id = (SELECT MAX(id) FROM dbo.WCS_PLAN_CONFIG_Temp);";

                using (SqlConnection conn = new SqlConnection(GLb.g_tSQLConfig.SqlString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@parcel", parcelDone);
                        cmd.Parameters.AddWithValue("@pallet", palletDone);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật Realtime kế hoạch: " + ex.Message);
            }
        }

        private void LoadDataFromSqlToListView()
        {
            lvPrintList.Items.Clear();
            GLb.IsInTask = false;

            LoadCurrentPalletIdFromDb();

            if(string.IsNullOrWhiteSpace(GLb.CurrentPalletID))
                AppendText("[INFO] KHÔNG TÌM THẤY PALLET ĐANG XỬ LÝ TRONG CSDL\r");

            string query = @"SELECT ParcelCode, Pallet_ID, Status FROM dbo.WCS_Parcels_Prod
                            WHERE Pallet_ID = @Pallet_ID AND Status <> -1 ORDER BY Id ASC;";

            using (SqlConnection conn = new SqlConnection(GLb.g_tSQLConfig.SqlString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Pallet_ID", SqlDbType.NVarChar, 50).Value = GLb.CurrentPalletID;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int stt = 1;

                        while (reader.Read())
                        {
                            GLb.IsInTask = true;

                            string parcelCode = reader["ParcelCode"] == DBNull.Value ? "" : reader["ParcelCode"].ToString();
                            string palletIdDb = reader["Pallet_ID"] == DBNull.Value ? "" : reader["Pallet_ID"].ToString();
                            int status = reader["Status"] == DBNull.Value ? -1 : Convert.ToInt32(reader["Status"]);

                            string statusText;
                            switch (status)
                            {
                                case 0: statusText = "Chờ in"; break;
                                case 1: statusText = "Đã in"; break;
                                case 2: statusText = "Đã lên bảng kê"; break;
                                case 3: statusText = "Chờ in Palet"; break;
                                case 4: statusText = "Chờ gửi Palet"; break;
                                case 5: statusText = "Hoàn tất Palet"; break;
                                default: statusText = "Không xác định"; break;
                            }

                            ListViewItem item = new ListViewItem(stt.ToString());
                            item.SubItems.Add(parcelCode);
                            item.SubItems.Add(statusText);
                            item.SubItems.Add(palletIdDb);

                            lvPrintList.Items.Add(item);
                            stt++;
                        }
                    }
                }
            }
        }
        private void LoadCurrentPalletIdFromDb()
        {
            GLb.CurrentPalletID = "";

            string sql = @"
SELECT TOP 1 Pallet_ID
FROM dbo.WCS_Pallet_Prod
WHERE Status = 'PROCESSING'
ORDER BY Id ASC;

IF (@@ROWCOUNT = 0)
BEGIN
    SELECT TOP 1 Pallet_ID
    FROM dbo.WCS_Pallet_Prod
    WHERE Status = 'WAIT'
    ORDER BY Id ASC;
END";

            using (SqlConnection conn = new SqlConnection(GLb.g_tSQLConfig.SqlString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    object o = cmd.ExecuteScalar();
                    if (o != null && o != DBNull.Value)
                        GLb.CurrentPalletID = o.ToString();
                }
            }
        }

        private bool DataToUI_PLCConfig()
        {
            bool bRet = false;

            try
            {
                m_ptDevConfig = GLb.g_tDevCfg.tDeviceList[nIndexDev];
                propertyGridPower.SelectedObject = m_ptDevConfig;
                m_ptDevConfig1 = GLb.g_tDevCfg.tDeviceList[nIndexDev + 1];
                propertyGridPrinter.SelectedObject = m_ptDevConfig1;

                bRet = true;
            }
            catch //(System.Exception ex)
            {
                bRet = false;
            }

            CDevice dt = devHandler.GetDeviceHandler(m_ptDevConfig.nLCISDeviceID);
            if (dt != null)
            {
                if (dt.m_bConnection)
                {
                    simpleButtonConnect.Text = "&Disconnect";
                    // EnabledUI(false);
                }
                else
                {
                    simpleButtonConnect.Text = "&Connect";
                    //EnabledUI(true);
                }
            }
            else { }
            return bRet;
        }

        private bool UItoData_PLCConfig()
        {
            bool bRet = false;
            m_ptDevConfig = GLb.g_tDevCfg.tDeviceList[nIndexDev];
            propertyGridPower.SelectedObject = m_ptDevConfig;

            m_ptDevConfig1 = GLb.g_tDevCfg.tDeviceList[nIndexDev + 1];
            propertyGridPrinter.SelectedObject = m_ptDevConfig1;

            bRet = true;
            return bRet;
        }

        public bool DataToUI_MotionPLCControl()
        {
            bool bRet = false;
            try
            {
                #region IO
                for (int i = 0; i < (int)Globals.k_InputConfig.INRow_End; i++)
                {
                    if (i < GLb.g_tInputCfg.Length)
                    {
                        recordsDataInputConfig1[i].nNo = i.ToString("00");
                        recordsDataInputConfig1[i].sType = GLb.g_tInputCfg[i].sType;
                        recordsDataInputConfig1[i].nModule = GLb.g_tInputCfg[i].nModule;
                        recordsDataInputConfig1[i].nBitIndex = GLb.g_tInputCfg[i].nBitIndex;
                        recordsDataInputConfig1[i].nValue = GLb.g_tInputCfg[i].nValue;
                    }
                    else
                    {
                        recordsDataInputConfig1[i].nNo = i.ToString("00");
                    }
                }
                for (int i = 0; i < (int)Globals.k_OutputConfig.OUTRow_End; i++)
                {
                    if (i < GLb.g_tOutputCfg.Length)
                    {
                        recordsDataOutputConfig1[i].nNo = i.ToString("00");
                        recordsDataOutputConfig1[i].sType = GLb.g_tOutputCfg[i].sType;
                        recordsDataOutputConfig1[i].nModule = GLb.g_tOutputCfg[i].nModule;
                        recordsDataOutputConfig1[i].nBitIndex = GLb.g_tOutputCfg[i].nBitIndex;
                        recordsDataOutputConfig1[i].nValue = GLb.g_tOutputCfg[i].nValue;
                    }
                    else
                    {
                        recordsDataOutputConfig1[i].nNo = i.ToString("00");
                    }
                }
                #endregion
                //this.comboBoxLanguage.SelectedIndex = GLb.g_tSysCfg.nLanguage;
                bRet = true;
            }
            catch (System.Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                MessageBox.Show("Config data is broken!");
            }
            return bRet;
        }

        public bool UIToData_MotionPLCControl()
        {
            bool bRet = false;
            try
            {
                #region IO
                //GLb.g_tInputCfg = new Globals.TAxisIO[(int)Globals.k_InputConfig.INRow_End];
                for (int i = 0; i < (int)Globals.k_InputConfig.INRow_End; i++)
                {
                    if (GLb.g_tInputCfg[i] == null)
                    {
                        GLb.g_tInputCfg[i] = new Globals.TAxisIO();
                    }
                    GLb.g_tInputCfg[i].nNo = i.ToString("00");
                    GLb.g_tInputCfg[i].nModule = recordsDataInputConfig1[i].nModule;
                    GLb.g_tInputCfg[i].sType = recordsDataInputConfig1[i].sType;
                    GLb.g_tInputCfg[i].nBitIndex = recordsDataInputConfig1[i].nBitIndex;
                    GLb.g_tInputCfg[i].nValue = recordsDataInputConfig1[i].nValue;
                    GLb.g_tInputCfg[i].sItem = recordsDataInputConfig1[i].sItem;
                }
                //GLb.g_tOutputCfg = new Globals.TAxisIO[(int)Globals.k_OutputConfig.OUTRow_End];
                for (int i = 0; i < (int)Globals.k_OutputConfig.OUTRow_End; i++)
                {
                    if (GLb.g_tOutputCfg[i] == null)
                    {
                        GLb.g_tOutputCfg[i] = new Globals.TAxisIO();
                    }
                    GLb.g_tOutputCfg[i].nNo = i.ToString("00");
                    GLb.g_tOutputCfg[i].sItem = recordsDataOutputConfig1[i].sItem;
                    GLb.g_tOutputCfg[i].nModule = recordsDataOutputConfig1[i].nModule;
                    GLb.g_tOutputCfg[i].sType = recordsDataOutputConfig1[i].sType;
                    GLb.g_tOutputCfg[i].nBitIndex = recordsDataOutputConfig1[i].nBitIndex;
                    GLb.g_tOutputCfg[i].nValue = recordsDataOutputConfig1[i].nValue;
                }
                #endregion
                //GLb.g_tSysCfg.nLanguage = this.comboBoxLanguage.SelectedIndex;
                bRet = true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Correct all input data first!!! " + ex.Message);
                bRet = false;
            }
            return bRet;
        }

        public void SetButton(BtnState btnstate)
        {
            if (InvokeRequired)
            {
                if (btnstate == BtnState.RUNNING)
                {
                    buttonSTOP.Enabled = true;
                    buttonSTART.Enabled = false;
                    for (int i = 1; i < xtraTabControlTForm.TabPages.Count; i++)
                    {
                        if (xtraTabControlTForm.TabPages[i].Name == "Clear")
                        {
                            continue;
                        }
                        if (xtraTabControlTForm.TabPages[i].Name == "xtraTabPageSystem")
                        {
                            continue;
                        }
                        xtraTabControlTForm.TabPages[i].PageEnabled = false;
                    }
                    labelStatus.Text = "RUN";
                    labelStatus.ForeColor = Color.Orange;
                    buttonSTART.Text = "Running";
                    buttonSTART.Tag = "Running";
                }
                else if (btnstate == BtnState.STOP)
                {
                    buttonSTOP.Enabled = true;
                    buttonSTART.Enabled = true;
                    labelStatus.Text = "STOP";
                    labelStatus.ForeColor = Color.BlueViolet;
                    buttonSTART.Text = "Start";
                    buttonSTART.Tag = "Start";
                    for (int i = 1; i < xtraTabControlTForm.TabPages.Count; i++)
                    {
                        xtraTabControlTForm.TabPages[i].PageEnabled = true;
                    }
                }
                else if (btnstate == BtnState.MANUAL)
                {
                    buttonSTOP.Enabled = false;
                    buttonSTART.Enabled = false;
                }
            }
            else
            {
                if (btnstate == BtnState.RUNNING)
                {
                    buttonSTOP.Enabled = true;
                    buttonSTART.Enabled = false;

                    labelStatus.Text = "RUN";
                    labelStatus.ForeColor = Color.Orange;

                    for (int i = 1; i < xtraTabControlTForm.TabPages.Count; i++)
                    {
                        if (xtraTabControlTForm.TabPages[i].Name == "tabPageClearBarcode")
                        {
                            continue;
                        }
                        if (xtraTabControlTForm.TabPages[i].Name == "xtraTabPageSystem")
                        {
                            continue;
                        }
                        xtraTabControlTForm.TabPages[i].PageEnabled = false;
                    }
                    buttonSTART.Text = "Running";
                    buttonSTART.Tag = "Running";
                }
                else if (btnstate == BtnState.STOP)
                {
                    buttonSTART.Enabled = true;
                    buttonSTOP.Enabled = true;

                    labelStatus.Text = "STOP";
                    labelStatus.ForeColor = Color.Blue;

                    for (int i = 1; i < xtraTabControlTForm.TabPages.Count; i++)
                    {
                        xtraTabControlTForm.TabPages[i].PageEnabled = true;
                    }

                    buttonSTART.Text = "Start";
                    buttonSTART.Tag = "Start";
                }
                else if (btnstate == BtnState.MANUAL)
                {
                    buttonSTOP.Enabled = false;
                    buttonSTART.Enabled = false;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateDeviceStatus();
        }

        private void TForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            GLb.g_bGrabbing = false;
            GLb.g_bSQLCheck = false;
            devHandler.StopAllDev(true);
            CIOInterface.DestroyThread(this);
        }

        private void TForm_FormClosing(object sender, FormClosingEventArgs e) { }

        private void timer2_Tick(object sender, EventArgs e)
        {
            labelTime.Text =
                DateTime.Now.ToString("yyyy/MM/dd") + " " + DateTime.Now.ToShortTimeString();
            if (lvlog.Items.Count > 200)
            {
                lvlog.Items.Clear();
            }

            string folderNameNew = DateTime.Now.ToString("yyyy-MM-dd");
            if (GLb.fileNameSaveImage != folderNameNew)
            {
                GLb.fileNameSaveImage = folderNameNew;
                startupFolder();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(Convert.ToInt32(e.Argument));
        }

        private void backgroundWorker1_RunWorkerCompleted_1(object sender, RunWorkerCompletedEventArgs e)
        { }

        private int nCountImageInsp = 2;

        private void TimerInsp_Tick(object sender, EventArgs e)
        {
            switch (nCountImageInsp)
            {
                case 1:
                    buttonInsp.Image = Properties.Resources.Indicator1;
                    break;
                case 2:
                    buttonInsp.Image = Properties.Resources.Indicator2;
                    break;
                case 3:
                    buttonInsp.Image = Properties.Resources.Indicator3;
                    break;
                case 4:
                    buttonInsp.Image = Properties.Resources.Indicator4;
                    break;
                case 5:
                    buttonInsp.Image = Properties.Resources.Indicator5;
                    break;
                case 6:
                    buttonInsp.Image = Properties.Resources.Indicator6;
                    break;
                case 7:
                    buttonInsp.Image = Properties.Resources.Indicator7;
                    break;
                case 8:
                    buttonInsp.Image = Properties.Resources.Indicator8;
                    break;
                default:
                    break;
            }
            nCountImageInsp++;
            if (nCountImageInsp > 8)
            {
                nCountImageInsp = 1;
            }
        }

        private BindingList<Globals.TAxisIO> recordsDataInputConfig1 = new BindingList<Globals.TAxisIO>();
        private BindingList<Globals.TAxisIO> recordsDataOutputConfig1 = new BindingList<Globals.TAxisIO>();

        private void InitGridView()
        {
            ContextMenu IOContextMenu = new ContextMenu();
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            #region Axis config    ( VGrid - Vertical)
            var fields = typeof(DataAxisConfig)
                .GetProperties(bindingFlags)
                .Select(f => f.Name)
                .ToList();

            #endregion

            #region IO  ( Gridview - Horizotal)
            //gridControlInputS1.ContextMenu = IOContextMenu;
            gridControlInputS1.DataSource = recordsDataInputConfig1;

            fields = typeof(Globals.TAxisIO)
                .GetProperties(bindingFlags)
                .Select(f => f.Name)
                .ToList();
            for (int i = 0; i < fields.Count; i++)
            {
                if (i < gridViewInputS1.Columns.Count)
                {
                    gridViewInputS1.Columns[i].FieldName = fields[i];
                }
            }

            for (int i = 0; i < (int)Globals.k_InputConfig.INRow_End; i++)
            {
                recordsDataInputConfig1.Add(new Globals.TAxisIO());
                recordsDataInputConfig1[i].sItem = Globals.EnumExtensions.GetEnumDescription(
                    (Globals.k_InputConfig)i
                );
            }

            //gridControlOutput.ContextMenu = IOContextMenu;
            gridControlOutputS1.DataSource = recordsDataOutputConfig1;

            for (int i = 0; i < fields.Count; i++)
            {
                if (i < gridViewOutputS1.Columns.Count)
                {
                    gridViewOutputS1.Columns[i].FieldName = fields[i];
                }
            }
            for (int i = 0; i < (int)Globals.k_OutputConfig.OUTRow_End; i++)
            {
                recordsDataOutputConfig1.Add(new Globals.TAxisIO());
                recordsDataOutputConfig1[i].sItem = Globals.EnumExtensions.GetEnumDescription(
                    (Globals.k_OutputConfig)i
                );
            }

            #endregion
        }

        private void InitComboBox()
        {
            ComboBoxType.Items.Clear();
            ComboBoxType.Items.AddRange(Enum.GetValues(typeof(Globals.TEnumIOType)));
        }

        private bool bInitDone = false;

        private void ChangeButtonDisp(bool bChanged)
        {
            if (bInitDone)
            {
                if (!bChanged)
                {
                    this.simpleButton1.Appearance.BackColor = System.Drawing.Color.DodgerBlue;
                }
                else
                {
                    this.simpleButton1.Appearance.BackColor = System.Drawing.Color.Tomato;
                }
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            SplashScreenManager.ShowForm(null, typeof(WaitForm1), true, true, false);
            try
            {
                if (UIToData_MotionPLCControl())
                {
                    GLb.SaveAllData(true);
                    ChangeButtonDisp(false);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            finally
            {
                Thread.Sleep(500);
                SplashScreenManager.CloseForm(false);
            }
        }

        private void simpleButtonConnect_Click(object sender, EventArgs e)
        {
            CDevice dt = devHandler.GetDeviceHandler(m_ptDevConfig.nLCISDeviceID);

            if (dt != null)
            {
                if (simpleButtonConnect.Text == "&Connect")
                {
                    // Update ui to database
                    try
                    {
                        UItoData_PLCConfig();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    bool bTemp = false;
                    if (m_ptDevConfig.bActive)
                    {
                        try
                        {
                            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(
                                null,
                                typeof(WaitForm1),
                                true,
                                true,
                                false
                            );
                            bTemp = dt.Connect();
                        }
                        catch (Exception ex)
                        {
                            Log.LogWrite(Globals.LogLv.Information, ex);
                            Log.LogWrite(Globals.LogLv.Information, ex.Message);
                        }
                        finally
                        {
                            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                        }

                        //Connect
                        if (!bTemp)
                        {
                            // FAIL
                            string msg = string.Empty;
                            if (dt.m_bConnection)
                            {
                                msg +=
                                    "Serial port open successfully, but detector can not remote.";
                            }
                            else
                            {
                                msg += "Can not open serial port. Check again!";
                            }

                            MessageBox.Show(msg);
                        }
                        else
                        {
                            simpleButtonConnect.Text = "&Disconnect";
                            //EnabledUI(false);
                        }
                    }
                }
                else
                {
                    dt.Disconnect();
                    simpleButtonConnect.Text = "&Connect";
                    //EnabledUI(true);
                }
            }
        }

        private void TForm_Resize(object sender, EventArgs e)
        {
            //flowLayoutPanelCameraPictureSize  = flowLayoutPanelCameraPicture.ClientSize;
        }

        private void xtraTabControlTForm_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                SplashScreenManager.ShowForm(null, typeof(WaitForm1), true, true, false);
                DevExpress.XtraTab.XtraTabControl xTab =
                    sender as DevExpress.XtraTab.XtraTabControl;

                if (xTab.SelectedTabPage.Tag.ToString().IndexOf("Config") >= 0)
                {
                    if (this.frmCfg == null)
                    {
                        this.frmCfg = new frmConfig(this);
                        panelConfig.Controls.Add(frmCfg);
                    }
                    else
                    {
                        panelConfig.Controls.Clear();
                        this.frmCfg = new frmConfig(this);
                        panelConfig.Controls.Add(frmCfg);
                    }

                    frmCfg.Dock = DockStyle.Fill;
                }
                else if (xTab.SelectedTabPage.Tag.ToString().IndexOf("Main") >= 0)
                {
                    // LoadProductionData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                SplashScreenManager.CloseForm(false);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            SplashScreenManager.ShowForm(null, typeof(WaitForm1), true, true, false);

            try
            {
                if (UIToData_MotionPLCControl())
                {
                    GLb.SaveAllData(true);
                    ChangeButtonDisp(false);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            finally
            {
                Thread.Sleep(500);
                SplashScreenManager.CloseForm(false);
            }
        }

        private void labelNameSoftware_Click(object sender, EventArgs e)
        {
            //scaledemo = "0";
            SetLabelText(lblScaleValue, "0 g", Color.OrangeRed);
            // Cho phép chỉnh sửa lại DataGridView
            //GvJobFile.ReadOnly = false;

            //GvJobFile.Columns["Time"].ReadOnly = false;
            //GvJobFile.Columns["Action"].ReadOnly = false;
        }

        private void buttonSTART_Click_1(object sender, EventArgs e)
        {
            Log.LogWrite(Globals.LogLv.Information, "Click START button ");
            AppendText("Click START button \r");

            //if (!devHandler.cPrinterGodex.m_bConnection)
            //{
            //    MessageBox.Show("KIỂM TRA LẠI KẾT NỐI MÁY IN!!", "LỖI KẾT NỐI!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //if (!devHandler.cBarcode.m_bConnection)
            //{
            //    MessageBox.Show("KIỂM TRA LẠI KẾT NỐI PDA!!", "LỖI KẾT NỐI!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //if (!devHandler.cPLCHandler.m_bConnection)
            //{
            //    var res = MessageBox.Show("KẾT NỐI ĐÈN LỖI. TIẾP TỤC HAY KHÔNG?", "CẢNH BÁO", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            //    if(res == DialogResult.Cancel)
            //    {
            //        return;
            //    }
            //}

            //if (!devHandler.cScale.m_bConnection)
            //{
            //    MessageBox.Show("KIỂM TRA LẠI KẾT NỐI TỚI CÂN!!", "LỖI KẾT NỐI!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            if (GLb.IsInTask == false)
            {
                MessageBox.Show("KHÔNG CÓ LỆNH SẢN XUẤT. CHỜ PHẢN HỒI HỆ THỐNG!");
                return;
            }

            string palletOld = PrintQueueHelper.GetPalletIDFromDatabaseS2("4");
            if (!string.IsNullOrEmpty(palletOld))
            {
                GLb.CurrentPalletID = palletOld;
                LoadDataFromSqlToListView();
                if (!TryPrintAndSendOnce(palletOld))
                {
                    SetLabelText(lblPushInformation, "PALLET KHÔNG XỬ LÝ ĐƯỢC", Color.Red);
                    GLb.PalletScanCompleted = false;
                    MessageBox.Show("HÃY GỬI LẠI THÔNG TIN PALLET CŨ NHÉ!", "NOTIFICATION", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }
            }

            PrintQueueHelper.ResetToFirstWaitingItem(lvPrintList);

            LoadProductionData();
            InitPlan();

            AppendText("TẢI LÊN KẾ HOẠCH SẢN XUẤT \r ");

            TimerInsp.Enabled = true;
            this.buttonSTART.Enabled = false;

            if (this.buttonSTART.Tag.ToString() == "Start")
            {
                this.buttonSTART.Tag = "Running";
            }
            SetButton(BtnState.RUNNING);

            RunProcessCaptureImage();
            writeLog("[SYSTEM] System is Working");
            AppendText("[SYSTEM] System is Working \r");
        }

        private void buttonSTOP_Click(object sender, EventArgs e)
        {
            if (GLb.PalletInProgress)
            {
                AppendText("PALLET CHƯA HOÀN TẤT – HỆ THỐNG SẼ YÊU CẦU QUÉT TIẾP KHI START");
            }

            GLb.g_bGrabbing = false;

            ScaleRaw = "0";
            if (lblScaleValue.InvokeRequired)
            {
                lblScaleValue.Invoke(new Action(() => lblScaleValue.Text = ScaleRaw + "g"));
            }
            else
            {
                lblScaleValue.Text = ScaleRaw + "g";
            }
            if (m_hReceiveThread != null)
            {
                m_hReceiveThread.Abort();
            }
            TimerInsp.Enabled = false;
            buttonInsp.Image = Properties.Resources.Indicator9;
            SetButton(BtnState.STOP);

            devHandler.cPLCHandler.SetTrafficLightByM(-1);

            Log.LogWrite(Globals.LogLv.Information, "STOP SYSTEM");
            writeLog("[SYSTEM] System is STOP");
            try
            {
                foreach (var item in devHandler.detectorHandler) { }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void labelTime_Click(object sender, EventArgs e) { }

        private void buttonUserMan_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Bạn có chắc muốn đặt lại toàn bộ bộ đếm không?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            try
            {
                if (result == DialogResult.Yes)
                {
                    // Đặt lại giá trị toàn cục
                    GLb.nTotalItems = 0;
                    GLb.nPassItems = 0;
                    GLb.nFailItems = 0;

                    // Lưu lại file rỗng cho ngày hôm nay
                    CounterService.Save(0, 0, 0);

                    // Cập nhật lại giao diện
                    UpdateCounterUI();
                    LoadDataFromSqlToListView();
                    labelNameSoftware.Text = GLb.g_SoftwareNameVersion;
                    PrintQueueHelper.ResetToFirstWaitingItem(lvPrintList);
                    LoadProductionData();
                    InitPlan();

                    RCSbutton.Appearance.BackColor = Color.Orange;

                    MessageBox.Show(
                        "Đã đặt lại bộ đếm!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                AppendText("ĐÃ RESET CHU TRÌNH THÀNH CÔNG \r");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void HandleOrder(OrderTaskRequest data)
        {
            if (data == null || data.InforDetail == null || data.InforDetail.Count == 0)
                return;

            writeLog("XỬ LÝ LỆNH SẢN XUẤT: " + data.PO_ID);

            if (MessageBox.Show(
                "Có tiếp nhận lệnh sản xuất mới không?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(GLb.g_tSQLConfig.SqlString))
                {
                    conn.Open();

                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        /* =====================================================
                         * 1. INSERT TẤT CẢ PALLET (WAIT)
                         * =====================================================*/
                        foreach (var item in data.InforDetail)
                        {
                            using (SqlCommand cmd = new SqlCommand(@"
                                INSERT INTO dbo.WCS_Pallet_Prod (
                                    Pallet_ID, Location, Item_Code,
                                    Ctn, Qty, Pcs,
                                    Inner_Carton, Inner_Pallet,
                                    PO_ID, WH_Code, Line_ID, Task_ID,
                                    From_System, Status
                                ) VALUES (
                                    @Pallet_ID, @Location, @Item_Code,
                                    @Ctn, @Qty, @Pcs,
                                    @Inner_Carton, @Inner_Pallet,
                                    @PO_ID, @WH_Code, @Line_ID, @Task_ID,
                                    @From_System, 'WAIT'
                                );", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@Pallet_ID", item.PalletId);
                                cmd.Parameters.AddWithValue("@Location", item.Location ?? "");
                                cmd.Parameters.AddWithValue("@Item_Code", item.ItemCode ?? "");
                                cmd.Parameters.AddWithValue("@Ctn", item.Ctn);
                                cmd.Parameters.AddWithValue("@Qty", item.Qty);
                                cmd.Parameters.AddWithValue("@Pcs", item.Pcs);
                                cmd.Parameters.AddWithValue("@Inner_Carton", item.Inner_Carton);
                                cmd.Parameters.AddWithValue("@Inner_Pallet", item.Inner_Pallet);
                                cmd.Parameters.AddWithValue("@PO_ID", data.PO_ID);
                                cmd.Parameters.AddWithValue("@WH_Code", data.WH_Code ?? "");
                                cmd.Parameters.AddWithValue("@Line_ID", data.LineId ?? "LINE_01");
                                cmd.Parameters.AddWithValue("@Task_ID", data.Task_ID ?? "");
                                cmd.Parameters.AddWithValue("@From_System", data.FromSystem ?? "");

                                cmd.ExecuteNonQuery();
                            }

                            /* =====================================================
                             * 2. INSERT PARCEL (-1)
                             * =====================================================*/
                            foreach (var carton in item.CartonList)
                            {
                                string parcelCode, receivedCode;
                                ParseCartonCode(carton, out parcelCode, out receivedCode);

                                using (SqlCommand cmd = new SqlCommand(@"
                                    INSERT INTO dbo.WCS_Parcels_Prod (
                                        ParcelCode, Pallet_ID, Location,
                                        Status, Line_ID, ReceivedCode
                                    ) VALUES (
                                        @ParcelCode, @Pallet_ID, @Location,
                                        -1, @Line_ID, @ReceivedCode
                                    );", conn, tran))
                                {
                                    cmd.Parameters.AddWithValue("@ParcelCode", parcelCode);
                                    cmd.Parameters.AddWithValue("@Pallet_ID", item.PalletId);
                                    cmd.Parameters.AddWithValue("@Location", item.Location ?? "");
                                    cmd.Parameters.AddWithValue("@Line_ID", data.LineId ?? "LINE_01");
                                    cmd.Parameters.AddWithValue("@ReceivedCode", carton);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        if (!GLb.IsInTask)
                        {
                            /* =====================================================
                            * 3. CHỌN 1 PALLET ĐẦU TIÊN → PROCESSING
                            * =====================================================*/
                            var palletRun = data.InforDetail.First();
                            GLb.CurrentPalletID = palletRun.PalletId;
                            GLb.CurrentItemCode = palletRun.ItemCode;
                            GLb.CurrentWH_Code = data.WH_Code;

                            using (SqlCommand cmd = new SqlCommand(@"
                            UPDATE dbo.WCS_Pallet_Prod
                            SET Status = 'PROCESSING'
                            WHERE Pallet_ID = @Pallet_ID;", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@Pallet_ID", palletRun.PalletId);
                                cmd.ExecuteNonQuery();
                            }

                            using (SqlCommand cmd = new SqlCommand(@"
                            UPDATE dbo.WCS_Parcels_Prod
                            SET Status = 0
                            WHERE Pallet_ID = @Pallet_ID;", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@Pallet_ID", palletRun.PalletId);
                                cmd.ExecuteNonQuery();
                            }

                            /* =====================================================
                             * 4. INSERT PLAN (CHỈ 1 PALLET)
                             * =====================================================*/
                            using (SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO dbo.WCS_PLAN_CONFIG_Temp (
                                Line_ID, Item_Code,
                                Inner_Pallet, Inner_Carton,
                                TotalParcel, TotalPallet,
                                RealtimeParcel, RealTimePallet,
                                Ctn, Position
                            ) VALUES (
                                @Line_ID, @Item_Code,
                                @Inner_Pallet, @Inner_Carton,
                                @TotalParcel, 1,
                                0, 0,
                                @Ctn, @Position
                            );", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@Line_ID", data.LineId ?? "LINE_01");
                                cmd.Parameters.AddWithValue("@Item_Code", palletRun.ItemCode ?? "");
                                cmd.Parameters.AddWithValue("@Inner_Pallet", palletRun.Inner_Pallet);
                                cmd.Parameters.AddWithValue("@Inner_Carton", palletRun.Inner_Carton);
                                cmd.Parameters.AddWithValue("@TotalParcel", palletRun.CartonList.Count);
                                cmd.Parameters.AddWithValue("@Ctn", palletRun.Ctn);
                                cmd.Parameters.AddWithValue("@Position", palletRun.Location ?? "");

                                cmd.ExecuteNonQuery();
                            }
                        }
                        tran.Commit();
                    }
                }

                GLb.IsInTask = true;

                BeginInvoke(new Action(() =>
                {
                    LoadDataFromSqlToListView();
                    LoadProductionData();
                    InitPlan();
                }));
            }
            catch (Exception ex)
            {
                writeLog("Lỗi HandleOrder: " + ex.Message);
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HandleDonePallet(DonePalletRequest data)
        {
            if (data == null || string.IsNullOrEmpty(GLb.CurrentPalletID))
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(GLb.g_tSQLConfig.SqlString))
                {
                    conn.Open();

                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        string donePalletId = GLb.CurrentPalletID;
                        string nextPalletId = null;

                        /* =====================================================
                         * 1. XOÁ PARCEL CỦA PALLET HIỆN TẠI
                         *    (TRIGGER → WCS_Parcels_His)
                         * =====================================================*/
                        using (SqlCommand cmd = new SqlCommand(@"
DELETE FROM dbo.WCS_Parcels_Prod
WHERE Pallet_ID = @Pallet_ID;", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@Pallet_ID", donePalletId);
                            cmd.ExecuteNonQuery();
                        }

                        /* =====================================================
                         * 2. XOÁ PALLET HIỆN TẠI
                         *    (TRIGGER → WCS_Pallet_His)
                         * =====================================================*/
                        using (SqlCommand cmd = new SqlCommand(@"
DELETE FROM dbo.WCS_Pallet_Prod
WHERE Pallet_ID = @Pallet_ID;", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@Pallet_ID", donePalletId);
                            cmd.ExecuteNonQuery();
                        }

                        /* =====================================================
                         * 3. LẤY PALLET TIẾP THEO (FIFO)
                         * =====================================================*/
                        using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 Pallet_ID
FROM dbo.WCS_Pallet_Prod
WHERE Status = 'WAIT'
ORDER BY Id ASC;", conn, tran))
                        {
                            object o = cmd.ExecuteScalar();
                            if (o != null)
                                nextPalletId = o.ToString();
                        }

                        GLb.CurrentPalletID = nextPalletId;

                        /* =====================================================
                         * 4. KHÔNG CÒN PALLET → CLEAR PLAN
                         * =====================================================*/
                        if (string.IsNullOrEmpty(nextPalletId))
                        {
                            using (SqlCommand cmd = new SqlCommand(
                                "DELETE FROM dbo.WCS_PLAN_CONFIG_Temp;", conn, tran))
                            {
                                cmd.ExecuteNonQuery();
                            }
                            GLb.IsInTask = false;
                            tran.Commit();
                            return;
                        }

                        /* =====================================================
                         * 5. LOAD INFO PALLET MỚI
                         * =====================================================*/
                        string itemCode = "", location = "", lineId = "";
                        int innerPallet = 0, innerCarton = 0, ctn = 0, totalParcel = 0;

                        using (SqlCommand cmd = new SqlCommand(@"
SELECT Item_Code, Location, Inner_Pallet, Inner_Carton, Ctn, Line_ID
FROM dbo.WCS_Pallet_Prod
WHERE Pallet_ID = @Pallet_ID;", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@Pallet_ID", nextPalletId);
                            using (SqlDataReader rd = cmd.ExecuteReader())
                            {
                                if (rd.Read())
                                {
                                    itemCode = rd["Item_Code"].ToString();
                                    location = rd["Location"].ToString();
                                    innerPallet = Convert.ToInt32(rd["Inner_Pallet"]);
                                    innerCarton = Convert.ToInt32(rd["Inner_Carton"]);
                                    ctn = Convert.ToInt32(rd["Ctn"]);
                                    lineId = rd["Line_ID"].ToString();
                                }
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand(@"
SELECT COUNT(*)
FROM dbo.WCS_Parcels_Prod
WHERE Pallet_ID = @Pallet_ID;", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@Pallet_ID", nextPalletId);
                            totalParcel = (int)cmd.ExecuteScalar();
                        }

                        /* =====================================================
                         * 6. GHI ĐÈ CONFIG_PLAN_Temp
                         * =====================================================*/
                        using (SqlCommand cmd = new SqlCommand(@"
DELETE FROM dbo.WCS_PLAN_CONFIG_Temp;

INSERT INTO dbo.WCS_PLAN_CONFIG_Temp (
    Line_ID, Item_Code,
    Inner_Pallet, Inner_Carton,
    TotalParcel, TotalPallet,
    RealtimeParcel, RealTimePallet,
    Ctn, Position
) VALUES (
    @Line_ID, @Item_Code,
    @Inner_Pallet, @Inner_Carton,
    @TotalParcel, 1,
    0, 0,
    @Ctn, @Position
);", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@Line_ID", lineId ?? "LINE_01");
                            cmd.Parameters.AddWithValue("@Item_Code", itemCode);
                            cmd.Parameters.AddWithValue("@Inner_Pallet", innerPallet);
                            cmd.Parameters.AddWithValue("@Inner_Carton", innerCarton);
                            cmd.Parameters.AddWithValue("@TotalParcel", totalParcel);
                            cmd.Parameters.AddWithValue("@Ctn", ctn);
                            cmd.Parameters.AddWithValue("@Position", location);
                            cmd.ExecuteNonQuery();
                        }

                        /* =====================================================
                         * 7. PALLET MỚI → PROCESSING
                         * =====================================================*/
                        using (SqlCommand cmd = new SqlCommand(@"
UPDATE dbo.WCS_Pallet_Prod
SET Status = 'PROCESSING'
WHERE Pallet_ID = @Pallet_ID;", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@Pallet_ID", nextPalletId);
                            cmd.ExecuteNonQuery();
                        }

                        /* =====================================================
                         * 8. PARCEL: -1 → 0
                         * =====================================================*/
                        using (SqlCommand cmd = new SqlCommand(@"
UPDATE dbo.WCS_Parcels_Prod
SET Status = 0
WHERE Pallet_ID = @Pallet_ID
  AND Status = -1;", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@Pallet_ID", nextPalletId);
                            cmd.ExecuteNonQuery();
                        }

                        tran.Commit();
                    }
                }

                /* =====================================================
                 * 9. REFRESH UI
                 * =====================================================*/
                Action ui = () =>
                {
                    LoadDataFromSqlToListView();
                    LoadProductionData();
                    InitPlan();
                };

                if (InvokeRequired)
                    BeginInvoke(ui);
                else
                    ui();
            }
            catch (Exception ex)
            {
                MessageBox.Show("LỖI XỬ LÝ PALLET DONE: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ParseCartonCode(string cartonRaw, out string parcelCode, out string receivedCode)
        {
            parcelCode = "";
            receivedCode = "";

            if (string.IsNullOrWhiteSpace(cartonRaw))
                return;

            receivedCode = cartonRaw.Trim();

            var parts = receivedCode.Split('|');
            if (parts.Length > 10)
                parcelCode = parts[9];
        }
        #endregion

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (!FinishProduction())
            {
                MessageBox.Show("Gửi kết thúc lệnh sản xuất lỗi!");
                return;
            }
        }
        private bool _blinkOn = false;
        private Panel PosPanTemp = new Panel();
        private void timerBlink_Tick(object sender, EventArgs e)
        {
            _blinkOn = !_blinkOn;
            PosPanTemp.BackColor = _blinkOn ? Color.Green : Color.DarkGray;
        }
    }


    #region Helper Classes

    public class DailyCounter
    {
        public string Date { get; set; } // định dạng "yyyy-MM-dd"
        public int Total { get; set; }
        public int Pass { get; set; }
        public int Fail { get; set; }
    }

    public static class CounterService
    {
        private static string counterFile = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "counter.json"
        );

        public static void Save(int total, int pass, int fail)
        {
            var counter = new DailyCounter
            {
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                Total = total,
                Pass = pass,
                Fail = fail,
            };

            string json = JsonConvert.SerializeObject(counter, Formatting.Indented);
            File.WriteAllText(counterFile, json);
        }

        public static void Load(out int total, out int pass, out int fail)
        {
            total = 0;
            pass = 0;
            fail = 0;

            if (File.Exists(counterFile))
            {
                string json = File.ReadAllText(counterFile);
                var counter = JsonConvert.DeserializeObject<DailyCounter>(json);

                if (counter != null && counter.Date == DateTime.Now.ToString("yyyy-MM-dd"))
                {
                    total = counter.Total;
                    pass = counter.Pass;
                    fail = counter.Fail;
                }
            }
        }
    }

    public static class PrintQueueHelper
    {
        public static int CurrentIndex = 0;

        public static string GetNextParcelCode(ListView listView)
        {
            if (listView == null || CurrentIndex >= listView.Items.Count)
            {
                return null;
            }

            string result = null;

            if (listView.InvokeRequired)
            {
                listView.Invoke(
                    new Action(() =>
                    {
                        result = GetCodeAndHighlight(listView);
                    })
                );
            }
            else
            {
                result = GetCodeAndHighlight(listView);
            }

            return result;
        }

        public static string GetPalletIDFromDatabaseS2(string status)
        {
            string connectionString = Globals.getInstance().g_tSQLConfig.SqlString;
            string palletID = null;

            string query =
                @"SELECT TOP 1 Pallet_ID FROM WCS_Parcels_Prod WHERE Status = @status ORDER BY Id DESC";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", status);
                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            palletID = result.ToString();
                        }
                    }
                }
            }
            catch { }

            return palletID;
        }

        private static string GetCodeAndHighlight(ListView listView)
        {
            var item = listView.Items[CurrentIndex];
            string status = item.SubItems[2].Text;

            if (status != "Chờ in" && status != "In lỗi")
            {
                return null;
            }

            foreach (ListViewItem i in listView.Items)
            {
                i.BackColor = Color.Gray;
            }

            item.BackColor = Color.Lime;
            listView.EnsureVisible(CurrentIndex);

            return item.SubItems[1].Text;
        }

        public static void MarkSuccess(ListView listView)
        {
            if (listView.InvokeRequired)
            {
                listView.Invoke(new Action(() => MarkSuccessInternal(listView)));
            }
            else
            {
                MarkSuccessInternal(listView);
            }
        }

        private static void MarkSuccessInternal(ListView listView)
        {
            if (listView == null || CurrentIndex >= listView.Items.Count)
            {
                return;
            }

            listView.Items[CurrentIndex].SubItems[2].Text = "Đã lên bảng kê";
            //listView.Items[CurrentIndex].BackColor = Color.Gray;

            CurrentIndex++;
        }

        public static void MarkFail(ListView listView)
        {
            if (listView.InvokeRequired)
            {
                listView.Invoke(new Action(() => MarkFailInternal(listView)));
            }
            else
            {
                MarkFailInternal(listView);
            }
        }

        public static void MarkFailInternal(ListView listView)
        {
            if (listView == null || CurrentIndex >= listView.Items.Count)
            {
                return;
            }

            listView.Items[CurrentIndex].SubItems[2].Text = "In lỗi";
            listView.Items[CurrentIndex].BackColor = Color.LightCoral;

            // Không tăng index => chờ xử lý lại
        }

        public static void UpdateStatusInDatabase(string parcelCode, int newStatus)
        {
            string connectionString =
                Globals.getInstance().g_tSQLConfig.SqlString;
            string query =
                "UPDATE dbo.WCS_Parcels_Prod SET Status = @status WHERE ParcelCode = @code";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@status", newStatus);
                    cmd.Parameters.AddWithValue("@code", parcelCode);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static int UpdateStatusByPalletId(string palletId, int newStatus)
        {
            if (string.IsNullOrWhiteSpace(palletId))
                return 0;

            string connectionString = Globals.getInstance().g_tSQLConfig.SqlString;

            const string query = @"UPDATE dbo.WCS_Parcels_Prod SET Status = @status WHERE Pallet_ID = @palletId;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@status", newStatus);
                cmd.Parameters.AddWithValue("@palletId", palletId);

                conn.Open();
                return cmd.ExecuteNonQuery(); // số dòng đã update
            }
        }

        public static void ResetToFirstWaitingItem(ListView listView)
        {
            if (listView == null)
            {
                return;
            }

            for (int i = 0; i < listView.Items.Count; i++)
            {
                if (listView.Items[i].SubItems[2].Text == "Chờ in")
                {
                    CurrentIndex = i;
                    return;
                }
            }

            // Nếu không có dòng nào chờ in, đặt CurrentIndex ngoài phạm vi
            CurrentIndex = listView.Items.Count;
        }

        public static bool IsLastPrintedParcel(ListView lv)
        {
            if (lv == null || lv.IsDisposed || lv.Items.Count == 0)
            {
                return true;
            }

            if (lv.InvokeRequired)
            {
                return (bool)lv.Invoke(new Func<bool>(() => IsLastPrintedParcel(lv)));
            }

            foreach (ListViewItem item in lv.Items)
            {
                if (
                    (item.Tag != null && item.Tag.ToString() == "0")
                    || (item.SubItems.Count > 2 && item.SubItems[2].Text.Trim() == "Chờ in")
                )
                {
                    return false;
                }
            }

            return true;
        }
    }
    #endregion
}
