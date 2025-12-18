using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
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
                timerUpdateIOT.Enabled = true;
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
                _server = new WcsHttpServer(8500, "/ids/orderTask/");
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

                _server.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server start error! Please check again! " + ex.Message);
            }

            ResizeListViewColumns();
            
            //no used
            InitMenuStrip();
            InitGridViewData();
            //

            LoadDataFromSqlToListView();
            labelNameSoftware.Text = GLb.g_SoftwareNameVersion;
            PrintQueueHelper.ResetToFirstWaitingItem(lvPrintList);

            var plan = LoadPlanConfig();
            LoadProductionData();
            InitPlan();
            devHandler.DevConnect(true);

            //// ====== TRẠNG THÁI 1: ĐỦ THÙNG NHƯNG CHƯA IN PALLET ======
            //if (realtimeParcel == innerPallet)
            //{
            //    SetLabelText(lblPushInformation,
            //        "Pallet trước chưa được in. Hệ thống sẽ tự động in pallet.",
            //        Color.Yellow);

            //    TryPrintAndReadPallet(palletCode);
            //    return;
            //}

            //// ====== TRẠNG THÁI 2: PALLET ĐÃ IN NHƯNG CHƯA QUÉT ======
            //if (realtimeParcel == innerPallet + 1)
            //{
            //    SetLabelText(lblPushInformation,
            //        "Pallet đã in nhưng chưa quét mã. Vui lòng quét pallet.",
            //        Color.Orange);

            //    TryReadPalletBarcode();
            //    return;
            //}

            //// ====== TRẠNG THÁI 3: BÌNH THƯỜNG ======
            //SetLabelText(lblPushInformation,
            //    "Hệ thống sẵn sàng chạy dây chuyền.",
            //    Color.Lime);
            //ServerWCS.Start();
        }
        
        private bool TryReadPalletBarcode()
        {
            for (int i = 0; i < 10; i++)
            {
                string barcode = devHandler.cBarcode.ReadBarcoder();

                if (!string.IsNullOrEmpty(barcode))
                {
                    AppendText("Quét pallet OK: " + barcode);
                    return true;
                }

                Thread.Sleep(500);
            }

            AppendText("Không quét được pallet. Giữ trạng thái để quét lại.");
            return false;
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

        public void InitMenuStrip()
        {
            //contextMenuStrip1 = new ContextMenuStrip();

            //// Thêm mục "Add Row"
            //menuItemAddRow = new ToolStripMenuItem("Add");
            //menuItemAddRow.Click += menuItemToolStripMenuItem_Click;

            //// Thêm mục "Delete Row"
            //menuItemDeleteRow = new ToolStripMenuItem("Delete");
            //menuItemDeleteRow.Click += DeleteToolStripMenuItem_Click;

            //// Thêm mục "Insert Row"
            //menuItemInsertRow = new ToolStripMenuItem("Insert");
            //menuItemInsertRow.Click += InsertToolStripMenuItem_Click;

            //// Thêm các mục vào ContextMenuStrip
            //contextMenuStrip1.Items.AddRange(new ToolStripItem[] { menuItemAddRow, menuItemDeleteRow, menuItemInsertRow });

            //// Gắn sự kiện chuột phải cho DataGridView
            //GvJobFile.CellMouseDown += dataGridView1_CellMouseDown;
            ////
            ////comboBoxMode.Items.AddRange(Enum.GetNames(typeof(SystemMode)));
        }

        public void InitGridViewData()
        {
            //GvJobFile.Columns["Action"].SortMode = DataGridViewColumnSortMode.NotSortable;
            //GvJobFile.Columns["Time"].SortMode = DataGridViewColumnSortMode.NotSortable;
            ////comboBoxMode.SelectedIndex = 1;
            //GvJobFile.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            //// Đặt DataGridView thành chế độ chỉ đọc (không sửa)
            //GvJobFile.ReadOnly = true;
            //GvJobFile.Columns["Time"].ReadOnly = true;
            //GvJobFile.Columns["Action"].ReadOnly = true;
            //LoadDataGridViewFromFile(g_sHomeDir);
        }

        private void ResizeListViewColumns()
        {
            int totalWidth = lvPrintList.ClientSize.Width;
            int columnCount = lvPrintList.Columns.Count;

            // int totalWidth = listView1.ClientSize.Width;

            lvPrintList.Columns[0].Width = totalWidth * 10 / 100; // Cột STT
            lvPrintList.Columns[1].Width = totalWidth * 30 / 100; // Cột Mã Định Danh
            lvPrintList.Columns[2].Width = totalWidth * 30 / 100; // Cột MÃ Pallet
            lvPrintList.Columns[3].Width = totalWidth * 20 / 100; // Cột Trạng Thái
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

                MakeLogFile.WriteSystemEvent(text); // Giả định rằng phương thức này không cần truy cập vào control UI và có thể an toàn khi chạy trên background thread
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
        }

        public PlanConfig LoadPlanConfig()
        {
            PlanConfig config = new PlanConfig();

            string query = "SELECT TOP 1 Line_ID,Item_Code, Inner_Pallet, TotalParcel,TotalPallet,RealTimeParcel,RealTimePallet FROM dbo.WCS_PLAN_CONFIG_Temp ORDER BY id DESC";

            using (SqlConnection conn = new SqlConnection(GLb.g_tSQLConfig.SqlString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    config.ParcelQuantity = Convert.ToInt32(reader["Inner_Pallet"]);
                    config.ProductionName = reader["Item_Code"].ToString();
                    config.TotalParcel =
                        reader["TotalParcel"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(reader["TotalParcel"]);
                    config.PalletQuantity =
                        reader["TotalPallet"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(reader["TotalPallet"]);
                    config.ParcelRealTimeNumber =
                        reader["RealTimeParcel"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(reader["RealTimeParcel"]);
                    config.PalletRealTimeNumber =
                        reader["RealTimePallet"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(reader["RealTimePallet"]);
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

            // Parcel target theo pallet hiện tại (có xử lý pallet cuối)
            GLb.nTotalParcel = CalculateTargetParcelThisPallet(
                GLb.nPalletDone,
                GLb.nTotalPallet,
                GLb.nTotalParcelAll,
                plan.ParcelQuantity
            );

            lblProductionName.Text = plan.ProductionName ?? string.Empty;

            SetLabelText(lblCountParcel, GLb.nParcelDone + "/" + GLb.nTotalParcel, Color.Aqua);
            SetLabelText(lblCountPallet, GLb.nPalletDone + "/" + GLb.nTotalPallet, Color.Aqua);
        }

        private static int CalculateTargetParcelThisPallet(int palletDone, int totalPallet, int totalParcelAll, int parcelPerPallet)
        {
            if (totalPallet <= 0 || parcelPerPallet <= 0)
                return 0;

            if (palletDone >= totalPallet)
                return 0;

            // Pallet cuối: có thể không đủ "inner pallet" chuẩn
            if (palletDone == totalPallet - 1)
            {
                int remaining = totalParcelAll - (parcelPerPallet * palletDone);
                return Math.Max(0, remaining);
            }

            return parcelPerPallet;
        }

        private void AppendText(string text)
        {
            try
            {
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
            timerUpdateIOT.Enabled = true;
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
                        devHandler.cPLCHandler.SetTrafficLightByM(1); //Yellow
                        Thread.Sleep(100);
                        _isPrinting = false;
                        continue;
                    }

                    if (_isPrinting)
                    {
                        Thread.Sleep(100);
                        continue; // Wait for printing to finish
                    }

                    _isPrinting = true;
                    SetLabelText(lblPushInformation, "CÂN SẢN PHẨM...", Color.OrangeRed);


                    // Check weight stability
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
                    Thread.Sleep(1000);

                    // Step 2: Print label

                    SetLabelText(lblPushInformation, "ĐANG IN TEM...", Color.OrangeRed);

                    string parcelCode = PrintQueueHelper.GetNextParcelCode(lvPrintList);
                    string palletCode = PrintQueueHelper.GetPalletIDFromDatabase(parcelCode);
                    string ItemName = PrintQueueHelper.GetItemNameFromDatabase(parcelCode);
                    string WH_Code = PrintQueueHelper.GetWH_CodeFromDatabase(palletCode);

                    SetLabelText(lblPalletCode, palletCode, Color.LightYellow);
                    if (string.IsNullOrEmpty(parcelCode))
                    {
                        SetLabelText(lblPushInformation, "HẾT DANH SÁCH IN / CHƯA CÓ THÙNG CHỜ IN", Color.Gray);
                        _isPrinting = false;
                        Thread.Sleep(500);
                        continue;
                    }

                    bool printOK = devHandler.cPrinterGodex.PrintBarcode(parcelCode, ItemName, palletCode, WH_Code, finalWeight.ToString());
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
                    {
                        _isPrinting = false;
                        SetLabelText(labelStatus, "OK", Color.Lime);
                        SetLabelText(lblPushInformation, "MÃ VẠCH:" + BarcodeTemp, Color.GreenYellow);
                        UpdateItemTotal(true);
                        AppendText("[PROCESS] ĐÃ HOÀN THÀNH IN THÙNG" + BarcodeTemp + "\r");
                        devHandler.cPLCHandler.SetTrafficLightByM(2); //Green
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
                        if (!TryPrintAndSendPalletManual(palletCode))
                        {     
                            SetLabelText(lblPushInformation, "PALLET KHÔNG XỬ LÝ ĐƯỢC", Color.Red);
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

        private void FinishProduction()
        {
            try
            {
                // 1) Reset biến nghiệp vụ
                GLb.nTotalParcel = 0;
                GLb.nParcelDone = 0;
                GLb.PalletInProgress = false;
                GLb.PalletScanCompleted = false;
                GLb.CurrentPalletID = "";
                GLb.IsInTask = false;

                //// 2) Clear task tạm trong DB
                //ClearTaskTable();
                string json = "";
                var res = TryBuildPalletJsonEndOfTurn(out json);
                string err;
                using (var cts = new CancellationTokenSource())
                {
                    bool postOK = TryPostJsonOnce(
                        "http://192.168.110.189:8060/identify-service/ids/taskStatusReport",
                        json,
                        out err
                    );

                    if (!postOK)
                    {
                        MessageBox.Show("GỬI DỮ LIỆU PALLET VỀ SERVER LỖI!", "WARNING", MessageBoxButtons.OKCancel);
                    }
                }


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

                        // UI thông báo
                        SetLabelText(lblPushInformation, "ĐÃ HOÀN TẤT LỆNH SẢN XUẤT", Color.Green);
                        SetLabelText(lblCountParcel, "0/0", Color.Aqua);
                        SetLabelText(lblCountPallet,
                            GLb.nPalletDone + "/" + GLb.nTotalPallet,
                            Color.Aqua
                        );

                        // Đèn xanh (tuỳ quy ước)
                        devHandler.cPLCHandler.SetTrafficLightByM(2);
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
            }
            catch (Exception ex)
            {
                SetLabelText(lblPushInformation, "LỖI HỆ THỐNG (FinishProduction): " + ex.Message, Color.Red);
                devHandler.cPLCHandler.SetTrafficLightByM(0);
            }
        }

        private bool IsReadyToSendPalletCode = false;
        private bool TryPrintAndSendPalletManual(string palletCode)
        {
            if (string.IsNullOrWhiteSpace(palletCode))
                return false;

            const int MAX_READ_TRY = 3;

            try
            {
                GLb.PalletInProgress = true;
                GLb.PalletScanCompleted = false;
                GLb.CurrentPalletID = palletCode;

                /* 1. IN PALLET */
                SetLabelText(lblPushInformation, "IN MÃ PALLET: " + palletCode, Color.Orange);
                if (!TryPrintPalletSafe(palletCode))
                    return false;

                PrintQueueHelper.UpdateStatusByPalletId(palletCode, 4);

                /* 2. QUÉT PALLET */
                string readCode;
                if (!TryReadPalletSafe(palletCode, MAX_READ_TRY, out readCode))
                {
                    SetLabelText(lblPushInformation, "LỖI ĐỌC MÃ PALLET", Color.Red);
                    devHandler.cPLCHandler.SetTrafficLightByM(0);
                    return false;
                }

                IsReadyToSendPalletCode = true;

                return true;
            }
            catch (Exception ex)
            {
                SetLabelText(lblPushInformation, "LỖI PALLET: " + ex.Message, Color.Red);
                devHandler.cPLCHandler.SetTrafficLightByM(0);
                return false;
            }
        }

        private bool TryPrintAndReadPallet(string palletCode)
        {
            if (string.IsNullOrWhiteSpace(palletCode))
                return false;

            const int MAX_READ_TRY = 3;

            try
            {
                GLb.PalletInProgress = true;
                GLb.PalletScanCompleted = false;
                GLb.CurrentPalletID = palletCode;

                /* 1. IN PALLET */
                SetLabelText(lblPushInformation, "IN MÃ PALLET: " + palletCode, Color.Orange);
                if (!TryPrintPalletSafe(palletCode))
                    return false;

                PrintQueueHelper.UpdateStatusByPalletId(palletCode, 4);

                /* 2. QUÉT PALLET */
                string readCode;
                if (!TryReadPalletSafe(palletCode, MAX_READ_TRY, out readCode))
                    return false;

                /* 3. BUILD JSON */
                string json;
                if (!TryBuildPalletJsonSafe(palletCode, out json))
                    return false;

                /* 4. POST SERVER – BLOCK */
                string err;
                using (var cts = new CancellationTokenSource())
                {
                    bool postOK = PostJsonBlockUntilSuccess(
                        "http://192.168.110.177:7000/API/G8/WCS/post",
                        json,
                        3000,
                        cts.Token,
                        out err
                    );

                    if (!postOK)
                        return false;
                }

                GLb.nPalletDone ++ ;
                GLb.nParcelDone = 0;
                GLb.nTotalParcel = CalculateTargetParcelThisPallet(GLb.nPalletDone, GLb.nTotalPallet, GLb.nTotalParcelAll, _parcelPerPallet);

                /* 5. DONE PALLET */
                PrintQueueHelper.UpdateStatusByPalletId(palletCode, 5); // DONE + SENT

                SetLabelText(lblCountParcel, GLb.nParcelDone + "/" + GLb.nTotalParcel, Color.Aqua);
                SetLabelText(lblCountPallet, GLb.nPalletDone + "/" + GLb.nTotalPallet, Color.Aqua);
                UpdatePlanRealtime(GLb.nParcelDone, GLb.nPalletDone);

                SetLabelText(lblPushInformation, "PALLET OK – TIẾP TỤC SẢN XUẤT", Color.Green);
                devHandler.cPLCHandler.SetTrafficLightByM(2); // GREEN

                if (GLb.nPalletDone == GLb.nTotalPallet)
                {
                    FinishProduction();
                }

                GLb.PalletScanCompleted = true;
                GLb.PalletInProgress = false;
                GLb.CurrentPalletID = "";

                return true;
            }
            catch (Exception ex)
            {
                SetLabelText(lblPushInformation, "LỖI PALLET: " + ex.Message, Color.Red);
                devHandler.cPLCHandler.SetTrafficLightByM(0);
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

        private bool TryBuildPalletJsonSafe(string palletCode, out string json)
        {
            json = "";
            try
            {
                PalletLocationInfo palletInfo = PrintQueueHelper.GetPalletLocationInfo(palletCode);
                if (palletInfo == null)
                    return false;

                json = SendAllPalletsDetail(palletInfo, palletCode);
                return !string.IsNullOrWhiteSpace(json);
            }
            catch
            {
                return false;
            }
        }

        private bool TryBuildPalletJsonManual(string locationNew, string palletCode, out string json)
        {
            json = "";
            try
            {
                PalletLocationInfo result = new PalletLocationInfo();

                string query = @"SELECT TOP 1 LineProduction, Location FROM WCS_Task_Temp WHERE Pallet_ID = @PalletID";
                try
                {
                    using (
                        SqlConnection conn = new SqlConnection(Globals.getInstance().g_tSQLConfig.SqlString)
                    )
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@PalletID", palletCode);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string location = reader["Location"].ToString();
                                    result.LocationOld = location;
                                }
                            }
                        }
                    }
                }
                catch //(Exception ex)
                {
                    result.LocationOld = "";
                }
                result.LocationNew = locationNew;

                json = SendAllPalletsDetail(result, palletCode);
                return !string.IsNullOrWhiteSpace(json);
            }
            catch
            {
                return false;
            }
        }

        private bool TryBuildPalletJsonEndOfTurn(out string json)
        {
            try
            {
                string query = @"SELECT TOP 1 PO_ID, LineProduction, ProductionCode FROM WCS_Task_Temp";
                string wh_Code = "";
                string po_ID = "";
                string lineId = "";
                using (SqlConnection conn = new SqlConnection(GLb.g_tSQLConfig.SqlString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                po_ID = reader["PO_ID"].ToString();
                                lineId = reader["LineProduction"].ToString();
                                wh_Code = reader["ProductionCode"].ToString();
                            }
                        }
                    }
                }

                var jsonObject = new
                {
                    PO_ID = po_ID,
                    WH_Code = wh_Code,
                    Line_ID = lineId,
                    status = 8,
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

        private bool TryPostJsonOnce(string url, string json, out string err)
        {
            err = "";
            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);

                    HttpResponseMessage resp = client.PostAsync(url, content).Result;

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
                }

                return true;
            }
            catch (AggregateException ae)
            {
                Exception e = ae.Flatten().InnerException ?? ae;
                err = e.Message;
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
                    SetLabelText(
                        lblPushInformation,
                        "KHÔNG GỬI ĐƯỢC SERVER – TẠM DỪNG DÂY CHUYỀN",
                        Color.Yellow
                    );
                    devHandler.cPLCHandler.SetTrafficLightByM(0); // RED
                }));

                Thread.Sleep(retryDelayMs);
            }

            lastErr = "Cancelled";
            return false;
        }

        private string SendAllPalletsDetail(PalletLocationInfo palletInfo, string palletID)
        {
            try
            {
                List<object> itemList = new List<object>();

                string query = @"SELECT ParcelCode, Ctn, ItemCode, Pallet_ID, PO_ID, ReceivedCode, LineProduction
                                    FROM WCS_Task_Temp WHERE Status = @status AND Pallet_ID = @Pallet_ID";

                string WH_Code = "";
                string PO_ID = "";
                string lineId = "";

                using (SqlConnection conn = new SqlConnection(GLb.g_tSQLConfig.SqlString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", "4");
                        cmd.Parameters.AddWithValue("@Pallet_ID", palletID);
         

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            int stt = 1;
                            while (reader.Read())
                            {
                                string itemCode = reader["ItemCode"].ToString();
                                string cartonID = reader["ReceivedCode"].ToString();
                                int ctn = Convert.ToInt32(reader["Ctn"]);
                                PO_ID = reader["PO_ID"].ToString();
                                lineId = reader["LineProduction"].ToString();

                                string innerCarton = PrintQueueHelper.GetInnerCartonFromDatabase();
                                WH_Code = PrintQueueHelper.GetWH_CodeFromDatabase(palletID);

                                itemList.Add(new
                                    {
                                        STT = stt++,
                                        Item_Code = itemCode,
                                        Carton_ID = cartonID,
                                        Ctn = ctn,
                                        Qty = ctn * Convert.ToInt32(innerCarton),
                                        Pcs = 0,
                                    }
                                );
                            }
                        }
                    }
                }

                var jsonObject = new
                {
                    PO_ID = PO_ID,
                    WH_Code = WH_Code,
                    Line_ID = lineId,
                    palletID = palletID,
                    Location_Old = palletInfo.LocationOld,
                    Location_New = palletInfo.LocationNew,
                    //Item_Information = itemList,
                };

                string json = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
                return json;
            }
            catch (Exception ex)
            {
                SetLabelText(lblPushInformation, "LỖI GỬI JSON TOÀN BỘ: " + ex.Message, Color.Red);
                return "";
            }
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

        private bool IsStableWeight(out int finalWeight, int standardWeight, int allowedDiff, int durationSec = 2,
            int intervalMs = 200, // nên 200–300ms
            int stabilityJitter = 15 // độ lệch cho phép giữa các lần đọc
        )
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

        private void UpdateParcelRealtime(int value)
        {
            try
            {
                string query = @"UPDATE dbo.WCS_PLAN_CONFIG_Temp SET RealtimeParcel = @count WHERE id = (SELECT MAX(id) FROM dbo.WCS_PLAN_CONFIG_Temp);";

                using (SqlConnection conn = new SqlConnection(GLb.g_tSQLConfig.SqlString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@count", value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật ParcelNumberRealtime: " + ex.Message);
            }
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

        private void ClearTaskTable()
        {
            try
            {
                string query = @"DELETE FROM dbo.WCS_Task_Temp; DELETE FROM dbo.WCS_PLAN_CONFIG_Temp;";

                using (SqlConnection conn = new SqlConnection(GLb.g_tSQLConfig.SqlString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi xóa lệnh sản xuất đã xong: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void LoadDataFromSqlToListView()
        {
            lvPrintList.Items.Clear();
            GLb.IsInTask = false;

            string query = @"SELECT id, ParcelCode, Pallet_ID, Ctn, Status, LineProduction, ImportTime FROM dbo.WCS_Task_Temp";

            using (SqlConnection conn = new SqlConnection(GLb.g_tSQLConfig.SqlString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    int stt = 1;

                    while (reader.Read())
                    {
                        if (!GLb.IsInTask)
                            GLb.IsInTask = true;

                        string parcelCode = reader["ParcelCode"].ToString();
                        int status = Convert.ToInt32(reader["Status"]);
                        var palletId = reader["Pallet_ID"].ToString();

                        string statusText;
                        switch (status)
                        {
                            case 0:
                                statusText = "Chờ in";
                                break;
                            case 1:
                                statusText = "Đã in";
                                break;
                            case 2:
                                statusText = "Đã lên bảng kê";
                                break;
                            case 3:
                                statusText = "Chờ in Palet";
                                break;
                            case 4:
                                statusText = "Chờ gửi Palet";
                                break;
                            case 5:
                                statusText = "Hoàn tất Palet";
                                break;
                            default:
                                statusText = "Không xác định";
                                break;
                        }

                        ListViewItem item = new ListViewItem(stt.ToString());
                        item.SubItems.Add(parcelCode);
                        item.SubItems.Add(palletId);
                        item.SubItems.Add(statusText);

                        lvPrintList.Items.Add(item);
                        stt++;
                    }
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

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            //sendData("123456");
            string palletCode = PrintQueueHelper.GetPalletIDFromDatabaseS2("1");

            if (TryPrintAndReadPallet(palletCode))
            {
                GLb.nParcelDone = 0;
                SetLabelText(lblCountParcel, GLb.nParcelDone + "/" + GLb.nTotalParcel, Color.Aqua);
                SetLabelText(lblCountPallet, GLb.nPalletDone + "/" + GLb.nTotalPallet, Color.Aqua);
                UpdatePlanRealtime(GLb.nParcelDone, GLb.nPalletDone);
                SetLabelText(lblPushInformation, "ĐÃ HOÀN TẤT PALLET: " + palletCode, Color.Green);
                AppendText("[PROCESS] ĐÃ HOÀN THÀNH IN PALLET" + palletCode + "\r");

                //SendAllPalletsDetail();
            }
            else
            {
                SetLabelText(lblPushInformation, "PALLET KHÔNG XỬ LÝ ĐƯỢC", Color.Red);
            }
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

        private void timer3_Tick(object sender, EventArgs e)
        {
            //InitWarehouseParameter();
            //labelTotalItem.Text = GLb.performanceCount.ToString();
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

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e) { }

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

        private void panel2_Paint(object sender, PaintEventArgs e) { }

        private void panel49_Paint(object sender, PaintEventArgs e) { }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //scaledemo = "150";
            //using (StreamWriter sw = new StreamWriter(g_sHomeDir))
            //{
            //    // Lưu header của cột
            //    for (int i = 0; i < GvJobFile.Columns.Count; i++)
            //    {
            //        sw.Write(GvJobFile.Columns[i].HeaderText);
            //        if (i < GvJobFile.Columns.Count - 1) sw.Write(",");
            //    }
            //    sw.WriteLine();

            //    // Lưu dữ liệu các hàng
            //    foreach (DataGridViewRow row in GvJobFile.Rows)
            //    {
            //        if (!row.IsNewRow) // Bỏ qua hàng trống
            //        {
            //            for (int i = 0; i < GvJobFile.Columns.Count; i++)
            //            {
            //                sw.Write(row.Cells[i].Value);
            //                if (i < GvJobFile.Columns.Count - 1) sw.Write(",");
            //            }
            //            sw.WriteLine();
            //        }
            //    }
            //}
            //InitGridViewData();
            //MessageBox.Show("Data saved successfully!");
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

            if (GLb.IsInTask == false)
            {
                MessageBox.Show("KHÔNG CÓ LỆNH SẢN XUẤT. CHỜ PHẢN HỒI HỆ THỐNG!");
                return;
            }

            string palletOld = PrintQueueHelper.GetPalletIDFromDatabaseS2("4");
            if(!string.IsNullOrEmpty(palletOld))
            {
                var res = TryPrintAndReadPallet(palletOld);
                if (!res)
                {
                    MessageBox.Show("Pallet trước chưa hoàn tất. Vui lòng quét mã pallet hoặc hủy pallet trước khi bắt đầu mới.");
                    return;
                }
                LoadDataFromSqlToListView();
            }    


            if (GLb.PalletInProgress && !GLb.PalletScanCompleted)
            {
                MessageBox.Show("Pallet trước chưa hoàn tất. Vui lòng quét mã pallet hoặc hủy pallet trước khi bắt đầu mới.");
                return;
            }

            PrintQueueHelper.ResetToFirstWaitingItem(lvPrintList);

            LoadProductionData();
            InitPlan();

            AppendText("TẢI LÊN KẾ HOẠCH SẢN XUẤT \r ");

            //StartUpdateSqlIotWareHouse();

            TimerInsp.Enabled = true;
            //timer1.Enabled = true;
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
            devHandler.cPLCHandler.SetTrafficLightByM(9); //Yellow
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

            devHandler.cPLCHandler.SetTrafficLightByM(-1); //Red

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
                    InitGridViewData();
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

        private void simpleButton3_Click(object sender, EventArgs e) { }

        private void simpleButton3_DoubleClick(object sender, EventArgs e)
        {
            SendMovePalletCommand();
        }

        private async void SendMovePalletCommand()
        {
            //RCSbutton.Appearance.BackColor = Color.GreenYellow;

            try
            {
                var data_sample = new { call_amr = "robot_move_pallet" };

                string json = JsonConvert.SerializeObject(data_sample);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5); // timeout 5 giây

                    var response = await client.PostAsync(
                        "http://192.168.110.2:9999/signal",
                        content
                    );

                    if (response.IsSuccessStatusCode)
                    {
                        AppendText("ĐÃ GỬI LỆNH MOVE PALLET CHO ROBOT \r ");
                    }
                    else
                    {
                        AppendText(" LỆNH DI CHUYỂN GỬI THẤT BẠI:" + response.StatusCode + "\r");
                    }
                }
                RCSbutton.Appearance.BackColor = Color.GreenYellow;
            }
            catch (Exception ex)
            {
                RCSbutton.Appearance.BackColor = Color.Orange;
                AppendText(
                    "[FAIL] LỆNH DI CHUYỂN ĐẶC BIỆT GỬI KHÔNG THÀNH CÔNG:" + ex.Message + "\r"
                );
            }
        }

        private void HandleOrder(OrderTaskRequest data)
        {
            if (data == null)
                return;

            writeLog("Process order: " + data.OrderId);

            if (GLb.IsInTask)
                return;

            var result = MessageBox.Show(
                "Có tiếp nhận lệnh sản xuất mới không?",
                "Xác nhận tiếp nhận lệnh",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
                return;

            if (data.InforDetail == null || data.InforDetail.Count == 0)
            {
                MessageBox.Show("Lệnh sản xuất không có dữ liệu chi tiết.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Xóa dữ liệu cũ (Task + Plan)
                ClearTaskTable();

                using (SqlConnection conn = new SqlConnection(GLb.g_tSQLConfig.SqlString))
                {
                    conn.Open();

                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        int totalParcel = data.InforDetail.Sum(d => d.CartonList.Count);
                        int totalPallet = data.InforDetail.Select(d => d.PalletId).Where(p => !string.IsNullOrWhiteSpace(p)).Distinct().Count();

                        var first = data.InforDetail.First();

                        // 1) Insert Plan
                        using (SqlCommand cmdPlan = new SqlCommand(@"INSERT INTO dbo.WCS_PLAN_CONFIG_Temp (
                                                                        Line_ID, 
                                                                        Item_Code,
                                                                        Inner_Pallet,
                                                                        Inner_Carton,
                                                                        TotalParcel,
                                                                        TotalPallet,
                                                                        RealtimeParcel,
                                                                        RealTimePallet
                                                                    ) VALUES (
                                                                        @Line_ID,
                                                                        @Item_Code,
                                                                        @Inner_Pallet,
                                                                        @Inner_Carton,
                                                                        @TotalParcel,
                                                                        @TotalPallet,
                                                                        0,
                                                                        0);", conn, tran))
                        {
                            cmdPlan.Parameters.AddWithValue("@Line_ID", data.FromSystem ?? "LINE_01");
                            cmdPlan.Parameters.AddWithValue("@Item_Code", first.ItemCode ?? string.Empty);
                            cmdPlan.Parameters.AddWithValue("@Inner_Pallet", first.Inner_Pallet);
                            cmdPlan.Parameters.AddWithValue("@Inner_Carton", first.Inner_Carton);
                            cmdPlan.Parameters.AddWithValue("@TotalParcel", totalParcel);
                            cmdPlan.Parameters.AddWithValue("@TotalPallet", totalPallet);
                            cmdPlan.ExecuteNonQuery();
                        }

                        // 2) Insert Task Temp
                        foreach (var detail in data.InforDetail)
                        {
                            if (detail.CartonList == null) continue;

                            foreach (var carton in detail.CartonList)
                            {
                                string parcelCode, receivedCode;
                                ParseCartonCode(carton, out parcelCode, out receivedCode);

                                using (SqlCommand cmd = new SqlCommand(@"INSERT INTO dbo.WCS_Task_Temp (
                                        ParcelCode,
                                        ReceivedCode,
                                        Pallet_ID,
                                        Ctn,
                                        Status,
                                        LineProduction,
                                        ProductionCode,
                                        [Location],
                                        ItemCode,
                                        PO_ID,
                                        ImportTime
                                    ) VALUES (
                                        @ParcelCode,
                                        @ReceivedCode,
                                        @Pallet_ID,
                                        @Ctn,
                                        @Status,
                                        @LineProduction,
                                        @ProductionCode,
                                        @Location,
                                        @ItemCode,
                                        @PO_ID,
                                        @ImportTime );", conn, tran))
                                {
                                    cmd.Parameters.AddWithValue("@ParcelCode", parcelCode);
                                    cmd.Parameters.AddWithValue("@ReceivedCode", receivedCode);
                                    cmd.Parameters.AddWithValue("@Pallet_ID", detail.PalletId);
                                    cmd.Parameters.AddWithValue("@Ctn", detail.Ctn);
                                    cmd.Parameters.AddWithValue("@Status", 0);
                                    cmd.Parameters.AddWithValue("@LineProduction", 01);
                                    cmd.Parameters.AddWithValue("@ProductionCode", data.OrderId);
                                    cmd.Parameters.AddWithValue("@Location", detail.Location);
                                    cmd.Parameters.AddWithValue("@ItemCode", detail.ItemCode);
                                    cmd.Parameters.AddWithValue("@PO_ID", data.OrderId);
                                    cmd.Parameters.AddWithValue("@ImportTime", DateTime.Now);

                                    cmd.ExecuteNonQuery();
                                }
                            }

                        }

                        tran.Commit();
                    }
                }

                MessageBox.Show(
                    "Tiếp nhận lệnh sản xuất thành công.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // Refresh UI
                if (InvokeRequired)
                    BeginInvoke(new Action(() => LoadDataFromSqlToListView()));
                else
                    LoadDataFromSqlToListView();

                LoadProductionData();
                InitPlan();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xử lý lệnh sản xuất: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ParseCartonCode(string cartonRaw, out string parcelCode, out string receivedCode)
        {
            parcelCode = "";
            receivedCode = "";

            if (string.IsNullOrWhiteSpace(cartonRaw))
                return;

            receivedCode = cartonRaw.Trim();

            // Tách theo ký tự '|', token rỗng vẫn được giữ nguyên
            var parts = receivedCode.Split('|');

            // Với ví dụ: TRU...|||||||12|0|TTFKHV2NZ001||001||||||||01
            // => TTFKHV2NZ001 nằm ở index 10
            if (parts.Length > 10)
                parcelCode = parts[9];
        }
        #endregion

        private string Location01 = "A1.1.1";
        private string Location02 = "A1.1.2";
        private void btnSendPallet01_Click(object sender, EventArgs e)
        {
            if (!IsReadyToSendPalletCode)
            {
                MessageBox.Show("CHƯA THỂ GỬI MÃ PALLET, VUI LÒNG KIỂM TRA LẠI", "WARNING", MessageBoxButtons.OK);
                return;
            }

            /* 3. BUILD JSON */
            string json;
            if (!TryBuildPalletJsonManual(Location01, GLb.CurrentPalletID, out json))
                return;

            /* 4. POST SERVER – BLOCK */
            string err;
            using (var cts = new CancellationTokenSource())
            {
                bool postOK = TryPostJsonOnce(
                    "http://192.168.110.189:8070/storage-service/task/ids/storage_product",
                    json,
                    out err
                );

                if (!postOK)
                {
                    MessageBox.Show("GỬI DỮ LIỆU PALLET VỀ SERVER LỖI!", "WARNING", MessageBoxButtons.OK);
                }
            }

            GLb.nPalletDone++;
            GLb.nParcelDone = 0;
            GLb.nTotalParcel = CalculateTargetParcelThisPallet(GLb.nPalletDone, GLb.nTotalPallet, GLb.nTotalParcelAll, _parcelPerPallet);

            /* 5. DONE PALLET */
            PrintQueueHelper.UpdateStatusByPalletId(GLb.CurrentPalletID, 5); // DONE + SENT

            SetLabelText(lblCountParcel, GLb.nParcelDone + "/" + GLb.nTotalParcel, Color.Aqua);
            SetLabelText(lblCountPallet, GLb.nPalletDone + "/" + GLb.nTotalPallet, Color.Aqua);
            UpdatePlanRealtime(GLb.nParcelDone, GLb.nPalletDone);

            SetLabelText(lblPushInformation, "PALLET OK – TIẾP TỤC SẢN XUẤT", Color.Green);
            devHandler.cPLCHandler.SetTrafficLightByM(2); // GREEN

            if (GLb.nPalletDone == GLb.nTotalPallet)
            {
                FinishProduction();
            }

            GLb.PalletScanCompleted = true;
            GLb.PalletInProgress = false;
            GLb.CurrentPalletID = "";

            IsReadyToSendPalletCode = false ;
        }

        private void btnSendPallet02_Click(object sender, EventArgs e)
        {
            if (!IsReadyToSendPalletCode)
            {
                MessageBox.Show("CHƯA THỂ GỬI MÃ PALLET, VUI LÒNG KIỂM TRA LẠI", "WARNING", MessageBoxButtons.OK);
                return;
            }

            /* 3. BUILD JSON */
            string json;
            if (!TryBuildPalletJsonManual(Location02, GLb.CurrentPalletID, out json))
                return;

            /* 4. POST SERVER – BLOCK */
            string err;
            using (var cts = new CancellationTokenSource())
            {
                bool postOK = TryPostJsonOnce(
                    "http://192.168.110.189:8070/storage-service/task/ids/storage_product",
                    json,
                    out err
                );

                if (!postOK)
                {
                    MessageBox.Show("GỬI DỮ LIỆU PALLET VỀ SERVER LỖI!", "WARNING", MessageBoxButtons.OK);
                }
            }

            GLb.nPalletDone++;
            GLb.nParcelDone = 0;
            GLb.nTotalParcel = CalculateTargetParcelThisPallet(GLb.nPalletDone, GLb.nTotalPallet, GLb.nTotalParcelAll, _parcelPerPallet);

            /* 5. DONE PALLET */
            PrintQueueHelper.UpdateStatusByPalletId(GLb.CurrentPalletID, 5); // DONE + SENT

            SetLabelText(lblCountParcel, GLb.nParcelDone + "/" + GLb.nTotalParcel, Color.Aqua);
            SetLabelText(lblCountPallet, GLb.nPalletDone + "/" + GLb.nTotalPallet, Color.Aqua);
            UpdatePlanRealtime(GLb.nParcelDone, GLb.nPalletDone);

            SetLabelText(lblPushInformation, "PALLET OK – TIẾP TỤC SẢN XUẤT", Color.Green);
            devHandler.cPLCHandler.SetTrafficLightByM(2); // GREEN

            if (GLb.nPalletDone == GLb.nTotalPallet)
            {
                FinishProduction();
            }

            GLb.PalletScanCompleted = true;
            GLb.PalletInProgress = false;
            GLb.CurrentPalletID = "";

            IsReadyToSendPalletCode = false;
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

        static Globals GLb = Globals.getInstance();


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

        public static string GetPalletIDFromDatabase(string parcelCode)
        {
            string connectionString = GLb.g_tSQLConfig.SqlString;
            string palletID = null;

            string query = @"SELECT TOP 1 Pallet_ID FROM WCS_Task_Temp WHERE ParcelCode = @parcelCode";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@parcelCode", parcelCode);
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

        public static string GetPalletIDFromDatabaseS2(string status)
        {
            string connectionString = Globals.getInstance().g_tSQLConfig.SqlString;
            string palletID = null;

            string query =
                @"SELECT TOP 1 Pallet_ID FROM WCS_Task_Temp WHERE Status = @status ORDER BY ID DESC";
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

        public static string GetItemNameFromDatabase(string parcelCode)
        {
            string connectionString = Globals.getInstance().g_tSQLConfig.SqlString;
            string ItemName = null;

            string query =
                @"SELECT TOP 1 ItemCode FROM WCS_Task_Temp WHERE ParcelCode = @parcelCode";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@parcelCode", parcelCode);
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        ItemName = result.ToString();
                    }
                }
            }

            return ItemName;
        }

        public static string GetWH_CodeFromDatabase(string pallet_ID)
        {
            string connectionString =
                Globals.getInstance().g_tSQLConfig.SqlString;
            string WH_Code = null;

            string query =
                @"SELECT TOP 1 ProductionCode FROM WCS_Task_Temp WHERE Pallet_ID = @pallet_ID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pallet_ID", pallet_ID);
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        WH_Code = result.ToString();
                    }
                }
            }

            return WH_Code;
        }

        public static string GetCtnFromDatabase(string palletcode)
        {
            string connectionString =
                Globals.getInstance().g_tSQLConfig.SqlString;
            string Ctn = null;

            string query = @"SELECT TOP 1 Ctn FROM WCS_Task_Temp WHERE Pallet_ID= @palletcode";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@palletcode", palletcode);
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        Ctn = result.ToString();
                    }
                }
            }

            return Ctn;
        }

        public static string GetInnerCartonFromDatabase()
        {
            string connectionString =
                Globals.getInstance().g_tSQLConfig.SqlString;
            string innerCarton = null;

            string query =
                @"SELECT TOP 1 Inner_Carton FROM WCS_PLAN_CONFIG_Temp ORDER BY id DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        innerCarton = result.ToString();
                    }
                }
            }

            return innerCarton;
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
                "UPDATE dbo.WCS_Task_Temp SET Status = @status WHERE ParcelCode = @code";

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

            const string query = @"UPDATE dbo.WCS_Task_Temp SET Status = @status WHERE Pallet_ID = @palletId;";

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

        public static PalletLocationInfo GetPalletLocationInfo(string palletID)
        {
            PalletLocationInfo result = new PalletLocationInfo();

            string query =
                @"SELECT TOP 1 LineProduction, Location FROM WCS_Task_Temp WHERE Pallet_ID = @PalletID";
            try
            {
                using (
                    SqlConnection conn = new SqlConnection(Globals.getInstance().g_tSQLConfig.SqlString)
                )
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PalletID", palletID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //string line = reader["Line"].ToString();
                                string lineProd = reader["LineProduction"].ToString();
                                string location = reader["Location"].ToString();

                                result.LocationOld = "line" + lineProd;
                                result.LocationNew = location;
                            }
                        }
                    }
                }
            }
            catch //(Exception ex)
            {
                result.LocationOld = "";
                result.LocationNew = "";
            }
            return result;
        }
    }

    public class JsonServer
    {
        private HttpListener listener;
        private string connectionString = Globals.getInstance().g_tSQLConfig.SqlString;

        public void Start()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:9100/wcs/callback/"); // chấp nhận POST đến /import
            listener.Start();

            //Console.WriteLine("Server started. Listening on http://127.0.0.1:9100/wcs/callback");
            listener.BeginGetContext(OnRequest, null);
        }

        private void OnRequest(IAsyncResult result)
        {
            try
            {
                var context = listener.EndGetContext(result);
                listener.BeginGetContext(OnRequest, null); // tiếp tục lắng nghe

                var request = context.Request;
                var response = context.Response;

                if (request.HttpMethod == "POST")
                {
                    using (var reader = new StreamReader(request.InputStream))
                    {
                        string jsonData = reader.ReadToEnd();
                        SaveJsonToWCS(jsonData); // Gọi hàm đã viết
                    }

                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes("Success");
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    response.StatusCode = 405; // Method Not Allowed
                }

                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void SaveJsonToWCS(string jsonString)
        {
            var jsonObj = JObject.Parse(jsonString);
            var items = jsonObj["Item_Information"];
            int poId = jsonObj.Value<int>("PO_ID");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                foreach (var item in items)
                {
                    string palletCode = item.Value<string>("Pallet_ID");
                    int ctn = item.Value<int>("Ctn");

                    var cartonList = item["Carton_List"].ToObject<List<string>>();
                    foreach (var cartonRaw in cartonList)
                    {
                        string[] parts = cartonRaw.Split('|');
                        if (parts.Length >= 9)
                        {
                            string parcelCode = parts[8];

                            string insertQuery =
                                @"INSERT INTO WCS_Task_Temp
                                (ParcelCode, Pallet_ID, Ctn, Status, LineProduction, ImportTime)
                                VALUES (@ParcelCode, @Pallet_ID, @Ctn, @Status, @LineProduction, @ImportTime)";

                            using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@ParcelCode", parcelCode);
                                cmd.Parameters.AddWithValue("@Pallet_ID", palletCode);
                                cmd.Parameters.AddWithValue("@Ctn", ctn);
                                cmd.Parameters.AddWithValue("@Status", 0);
                                cmd.Parameters.AddWithValue("@LineProduction", 1);
                                cmd.Parameters.AddWithValue("@ImportTime", DateTime.Now);

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                conn.Close();
            }
        }
    }
    #endregion

    public class PalletLocationInfo
    {
        public string LocationOld { get; set; }
        public string LocationNew { get; set; }
    }
}
