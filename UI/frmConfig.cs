using DevExpress.XtraSplashScreen;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Forms;
using VTP_Induction.Common;
using VTP_Induction.Device;

namespace VTP_Induction.UI
{
    public partial class frmConfig : UserControl
    {
        private Globals GLb = Globals.getInstance();
        private string sLogMain = "frmConfig: ";

        private BindingList<DataAxisConfig> recordsDataAxisConfig = new BindingList<DataAxisConfig>();
        private BindingList<DataJigConfig> recordsDataJigConfig = new BindingList<DataJigConfig>();
        private BindingList<DataDetectorPosConfig> recordsDataDetectorPosConfig = new BindingList<DataDetectorPosConfig>();
        private TForm parentForm;

        public frmConfig(TForm form)
        {
            parentForm = form;
            InitializeComponent();
            //m_sPassword = GLb.g_tSysCfg.Password;

            InitDeviceCfgs();
            DataToUI();
            GLb.g_SoftwareNameVersion = textBoxSoftwareName.Text;

            LoadHisPallet();
        }
        public void InitDeviceCfgs()
        {
            if (true)
            {
                foreach (Globals.TDetectorConfig cfg in GLb.g_tDevCfg.tDetectorList)
                {
                    CameraConfig rp = new CameraConfig(cfg, cfg.sDevGroup);
                    if (rp != null)
                    {
                        rp.dataChangedEvent += new DataChangedNow(dataChangedHandler);
                        flowLayoutPanelDetector.Controls.Add(rp);
                    }
                }

                Globals.TScaleConfig Scalecfg = GLb.g_tDevCfg.tScale;
                ScaleRS232 rpScale = new ScaleRS232(Scalecfg, Scalecfg.sDevGroup);
                if (rpScale != null)
                {
                    rpScale.dataChangedEvent += new DataChangedNow(dataChangedHandler);

                    flowLayoutPanelDetector.Controls.Add(rpScale);
                }

                Globals.TBarcodeConfig BarcodeCfg = GLb.g_tDevCfg.tBarcode;
                BarcodeReader rpBarcode = new BarcodeReader(BarcodeCfg, BarcodeCfg.sDevGroup);
                if (rpBarcode != null)
                {
                    rpBarcode.dataChangedEvent += new DataChangedNow(dataChangedHandler);

                    flowLayoutPanelDetector.Controls.Add(rpBarcode);
                }

                Globals.TPrinterGodex printerGodex = GLb.g_tDevCfg.tPrinterGodex;
                PrinterGodex rpPrinterGodex = new PrinterGodex(printerGodex);
                if (rpPrinterGodex != null)
                {
                    rpPrinterGodex.dataChangedEvent += new DataChangedNow(dataChangedHandler);

                    flowLayoutPanelDetector.Controls.Add(rpPrinterGodex);
                }

                //Globals.TWMSConfig WMScfg = GLb.g_tDevCfg.tWMS;
                //Globals.RFIDConfig RFIDcfg = GLb.g_tDevCfg.tRFID;

                //APIwms rpWMS = new APIwms(WMScfg, WMScfg.sDevGroup);

                //if (rpWMS != null)
                //{
                //    rpWMS.dataChangedEvent += new DataChangedNow(dataChangedHandler);

                //    flowLayoutPanelWMS.Controls.Add(rpWMS);
                //}

                //APIrfid rpRFID = new APIrfid(RFIDcfg, RFIDcfg.sDevGroup);

                //if (rpWMS != null)
                //{
                //    rpWMS.dataChangedEvent += new DataChangedNow(dataChangedHandler);

                //    flowLayoutPanelWMS.Controls.Add(rpWMS);
                //}

                //if (rpRFID != null)
                //{
                //    rpRFID.dataChangedEvent += new DataChangedNow(dataChangedHandler);

                //    flowLayoutPanelWMS.Controls.Add(rpRFID);
                //}

                Globals.TSQLConfig tSQL = GLb.g_tSQLConfig;
                SQLConfig rpSQLConfig = new SQLConfig(tSQL);
                if (rpSQLConfig != null)
                {
                    rpSQLConfig.dataChangedEvent += new DataChangedNow(dataChangedHandler);
                    flowLayoutPanelDB.Controls.Add(rpSQLConfig);
                }
            }
            else
            {

            }
        }

        private void LoadHisPallet()
        {
            try
            {
                string sql = @"SELECT TOP (500) Item_Code AS [Item Code],Pallet_ID AS [Pallet ID],Inner_Pallet AS [Inner Pallet], CreatedAt AS [Created At] FROM dbo.WCS_Pallet_His ORDER BY CreatedAt DESC;";

                using (SqlConnection conn = new SqlConnection(GLb.g_tSQLConfig.SqlString))
                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvPalletHis.DataSource = dt;

                    dgvPalletHis.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvPalletHis.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgvPalletHis.ReadOnly = true;
                    dgvPalletHis.AllowUserToAddRows = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi load dữ liệu pallet history: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private string GetSelectedHisPalletId()
        {
            if (dgvPalletHis.CurrentRow == null)
                return "";

            object value = null;

            // Nếu DataGridView đang hiển thị cột tên là Pallet_ID
            if (dgvPalletHis.Columns.Contains("Pallet_ID"))
                value = dgvPalletHis.CurrentRow.Cells["Pallet_ID"].Value;

            // Nếu bạn đang đặt alias là Pallet ID
            else if (dgvPalletHis.Columns.Contains("Pallet ID"))
                value = dgvPalletHis.CurrentRow.Cells["Pallet ID"].Value;

            return value == null ? "" : value.ToString().Trim();
        }

        private bool bInitDone = false;
        private void dataChangedHandler()
        {
            if (bInitDone)
            {
                this.buttonRefresh.Appearance.BackColor = System.Drawing.Color.OrangeRed;
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            //if (buttonRefresh.BackColor == Color.Tomato)
            DialogResult dlg = MessageBox.Show("Do you want to reset all of the connections", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dlg == DialogResult.OK)
            {
                buttonRefresh.Enabled = false;
                GLb.SaveAllData(true);

                //Refresh all device
                parentForm.devHandler.StopAllDev(true);
                parentForm.devHandler.StartAllDev(true);
                parentForm.devHandler.DevConnect(true);

                this.buttonRefresh.Appearance.BackColor = System.Drawing.Color.DodgerBlue;
                buttonRefresh.Enabled = true;
            }
        }
        private bool DataToUI()
        {
            bool bRet = false;
            try
            {

                foreach (Control ctrl in flowLayoutPanelDetector.Controls)
                {
                    ScaleRS232 rpScale = ctrl as ScaleRS232;
                    if (rpScale != null)
                    {
                        rpScale.DataToUI();
                    }
                    else
                    {
                        BarcodeReader rpBarcode = ctrl as BarcodeReader;
                        if (rpBarcode != null)
                        {
                            rpBarcode.DataToUI();
                        }
                        else
                        {
                            PrinterGodex rpPrinterGodex = ctrl as PrinterGodex;
                            if (rpPrinterGodex != null)
                            {
                                rpPrinterGodex.DataToUI();
                            }
                        }
                    }

                }

                //foreach (var control in flowLayoutPanelWMS.Controls)
                //{
                //    APIwms apiwmsControl = control as APIwms;
                //    if (apiwmsControl != null)
                //    {
                //        apiwmsControl.DataToUI();
                //    }
                //}
                //foreach (var control in flowLayoutPanelWMS.Controls)
                //{
                //    APIrfid apiRFIDControl = control as APIrfid;
                //    if (apiRFIDControl != null)
                //    {
                //        apiRFIDControl.DataToUI();
                //    }
                //}

                foreach (var control in flowLayoutPanelDB.Controls)
                {
                    SQLConfig sQLConfig = control as SQLConfig;
                    if (sQLConfig != null)
                    {
                        sQLConfig.DataToUI();
                    }
                }

                txbWeightStandard.Text = GLb.g_tSysCfg.nWeightStandard.ToString();
                txtScaleError.Text = GLb.g_tSysCfg.nScaleError.ToString();
                txtTimeScale.Text = GLb.g_tSysCfg.nTimeScale.ToString();
                textBoxSoftwareName.Text = GLb.g_tSysCfg.sSoftwareName;
                txtPalletNumber.Text = GLb.g_tSysCfg.nPalletNumber.ToString();
                txtParcelNumber.Text = GLb.g_tSysCfg.nParcelNumber.ToString();
                txtParcelType.Text = GLb.g_tSysCfg.sParcelType;
                checkBoxPrintPallet.Checked = GLb.g_tSysCfg.bPrintPallet;

                txtPos1.Text = GLb.g_tSysCfg.sPositions[0].ToString();
                txtPos2.Text = GLb.g_tSysCfg.sPositions[1].ToString();
                txtPos3.Text = GLb.g_tSysCfg.sPositions[2].ToString();
                txtPos4.Text = GLb.g_tSysCfg.sPositions[3].ToString();
                txtPos5.Text = GLb.g_tSysCfg.sPositions[4].ToString();
                txtPos6.Text = GLb.g_tSysCfg.sPositions[5].ToString();

                bRet = true;

            }
            catch (Exception ex)
            {
                Log.LogWrite(Globals.LogLv.Verbose, ex);
                MessageBox.Show("Config data is broken!");
            }
            return bRet;
        }
        private bool UIToData()
        {
            bool bRet = false;
            try
            {
                //foreach (CameraConfig rp in flowLayoutPanelDetector.Controls)
                //{
                //    rp.UIToData();
                //}
                foreach (Control ctrl in flowLayoutPanelDetector.Controls)
                {
                    ScaleRS232 rpScale = ctrl as ScaleRS232;
                    if (rpScale != null)
                    {
                        rpScale.UIToData();
                    }
                    else
                    {
                        BarcodeReader rpBarcode = ctrl as BarcodeReader;
                        if (rpBarcode != null)
                        {
                            rpBarcode.UIToData();
                        }
                        else
                        {
                            PrinterGodex rpPrinterGodex = ctrl as PrinterGodex;
                            if (rpPrinterGodex != null)
                            {
                                rpPrinterGodex.UIToData();
                            }
                        }
                    }
                }

                //foreach (var control in flowLayoutPanelWMS.Controls)
                //{
                //    APIwms apiwmsControl = control as APIwms;
                //    if (apiwmsControl != null)
                //    {
                //        apiwmsControl.UIToData();
                //    }
                //}
                //foreach (var control in flowLayoutPanelWMS.Controls)
                //{
                //    APIrfid apiRFIDControl = control as APIrfid;
                //    if (apiRFIDControl != null)
                //    {
                //        apiRFIDControl.UIToData();
                //    }
                //}

                foreach (var control in flowLayoutPanelDB.Controls)
                {
                    SQLConfig sQLConfig = control as SQLConfig;
                    if (sQLConfig != null)
                    {
                        sQLConfig.UIToData();
                    }
                }

                //GLb.g_tSysCfg.n_MultipleCamera = int.Parse(txtScaleValue.Text);
                GLb.g_tSysCfg.nWeightStandard = int.Parse(txbWeightStandard.Text);
                GLb.g_tSysCfg.nScaleError = int.Parse(txtScaleError.Text);
                GLb.g_tSysCfg.nTimeScale = int.Parse(txtTimeScale.Text);
                GLb.g_tSysCfg.sSoftwareName = textBoxSoftwareName.Text;
                GLb.g_tSysCfg.nParcelNumber = int.Parse(txtParcelNumber.Text);
                GLb.g_tSysCfg.nPalletNumber = int.Parse(txtPalletNumber.Text);
                GLb.g_tSysCfg.sParcelType = txtParcelType.Text;
                GLb.g_tSysCfg.bPrintPallet = checkBoxPrintPallet.Checked;

                GLb.g_tSysCfg.sPositions[0] = txtPos1.Text;
                GLb.g_tSysCfg.sPositions[1] = txtPos2.Text;
                GLb.g_tSysCfg.sPositions[2] = txtPos3.Text;
                GLb.g_tSysCfg.sPositions[3] = txtPos4.Text;
                GLb.g_tSysCfg.sPositions[4] = txtPos5.Text;
                GLb.g_tSysCfg.sPositions[5] = txtPos6.Text;

                bRet = true;
            }
            catch (Exception ex)
            {
                Log.LogWrite(Globals.LogLv.Verbose, ex);
                MessageBox.Show("Correct all input data first!!! ");
                bRet = false;
            }
            return bRet;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            SplashScreenManager.ShowForm(null, typeof(WaitForm1), true, true, false);

            string sLog = sLogMain + "OK button is clicked.";

            try
            {
                if (UIToData())
                {
                    GLb.g_SoftwareNameVersion = textBoxSoftwareName.Text;

                    GLb.SaveAllData(true);
                    //Refresh all device
                    parentForm.devHandler.StopAllDev(false);
                    parentForm.devHandler.StartAllDev(false);
                    parentForm.devHandler.DevConnect(false);
                    parentForm.InitAllPanelDevDiagnostics();

                    //parentForm.SetPermission(GLb.currentUser);
                    this.buttonRefresh.Appearance.BackColor = System.Drawing.Color.DodgerBlue;

                    // ChangeButtonDisp(false);
                }
            }
            catch (Exception ex)
            {
                Log.LogWrite(Globals.LogLv.Verbose, ex);
            }
            finally
            {
                Thread.Sleep(500);
                SplashScreenManager.CloseForm(false);
            }
        }

        private void buttonPrintManual_Click(object sender, EventArgs e)
        {
            bool bRet = parentForm.devHandler.cPrinter.PrintBarcode(txtPrintManual.Text, "", "", "", "");
            if (bRet)
            {
                MessageBox.Show("IN THÀNH CÔNG " + txtPrintManual.Text);
            }
            else
            {
                MessageBox.Show("IN LỖI " + txtPrintManual.Text);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {

        }

        private void btnReloadTask_Click(object sender, EventArgs e)
        {
            string palletId = GetSelectedHisPalletId();

            if (string.IsNullOrWhiteSpace(palletId))
            {
                MessageBox.Show(
                    "Vui lòng chọn một pallet trong danh sách lịch sử.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (GLb.IsInTask)
            {
                MessageBox.Show(
                    "Đang có task chạy, không thể reload pallet lịch sử.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Bạn có chắc muốn reload pallet này không?\n\nPallet_ID: " + palletId,
                "Xác nhận reload",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes)
                return;

            ReloadPalletFromHis(palletId);
        }

        private void ReloadPalletFromHis(string palletId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GLb.g_tSQLConfig.SqlString))
                {
                    conn.Open();

                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            int innerPallet = 0;

                            /*
                                1. Lấy thông tin pallet trong HIS,
                                đồng thời lấy Inner_Pallet để biết cần reload bao nhiêu parcel
                            */
                            using (SqlCommand cmd = new SqlCommand(@"
                        SELECT TOP 1 
                            ISNULL(Inner_Pallet, 0)
                        FROM dbo.WCS_Pallet_His
                        WHERE Pallet_ID = @Pallet_ID;
                    ", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@Pallet_ID", palletId);

                                object result = cmd.ExecuteScalar();

                                if (result == null || result == DBNull.Value)
                                {
                                    tran.Rollback();

                                    MessageBox.Show(
                                        "Không tìm thấy pallet trong bảng WCS_Pallet_His.",
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning
                                    );
                                    return;
                                }

                                innerPallet = Convert.ToInt32(result);

                                if (innerPallet <= 0)
                                {
                                    tran.Rollback();

                                    MessageBox.Show(
                                        "Inner_Pallet của pallet này không hợp lệ.",
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning
                                    );
                                    return;
                                }
                            }

                            /*
                                2. Kiểm tra pallet đã tồn tại trong PROD chưa
                            */
                            using (SqlCommand cmd = new SqlCommand(@"
                        SELECT COUNT(1)
                        FROM dbo.WCS_Pallet_Prod
                        WHERE Pallet_ID = @Pallet_ID;
                    ", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@Pallet_ID", palletId);

                                int countProd = Convert.ToInt32(cmd.ExecuteScalar());

                                if (countProd > 0)
                                {
                                    tran.Rollback();

                                    MessageBox.Show(
                                        "Pallet này đã tồn tại trong WCS_Pallet_Prod, không thể reload.",
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning
                                    );
                                    return;
                                }
                            }

                            /*
                                3. Insert pallet từ HIS về PROD
                                Status set lại là PROCESSING
                            */
                            using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO dbo.WCS_Pallet_Prod
                        (
                            Pallet_ID,
                            Location,
                            Item_Code,
                            Ctn,
                            Qty,
                            Pcs,
                            Inner_Carton,
                            Inner_Pallet,
                            PO_ID,
                            WH_Code,
                            Line_ID,
                            Task_ID,
                            From_System,
                            CreatedAt,
                            Status,
                            Weight_ref,
                            Item_Name
                        )
                        SELECT TOP 1
                            Pallet_ID,
                            Location,
                            Item_Code,
                            Ctn,
                            Qty,
                            Pcs,
                            Inner_Carton,
                            Inner_Pallet,
                            PO_ID,
                            WH_Code,
                            Line_ID,
                            Task_ID,
                            From_System,
                            CreatedAt,
                            'PROCESSING' AS Status,
                            Weight_ref,
                            Item_Name
                        FROM dbo.WCS_Pallet_His
                        WHERE Pallet_ID = @Pallet_ID;
                    ", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@Pallet_ID", palletId);
                                cmd.ExecuteNonQuery();
                            }

                            /*
     4. Insert parcel từ HIS về PROD
     Order theo CreatedAt trước
     Sau đó lấy đúng số lượng = Inner_Pallet
     Đồng thời loại trùng ParcelCode để tránh lỗi unique
 */
                            int insertedParcel = 0;

                            using (SqlCommand cmd = new SqlCommand(@"
    ;WITH SourceParcel AS
    (
        SELECT
            h.ParcelCode,
            h.Pallet_ID,
            h.Location,
            h.Line_ID,
            h.ReceivedCode,
            h.CreatedAt,

            ROW_NUMBER() OVER
            (
                PARTITION BY h.ParcelCode
                ORDER BY h.CreatedAt ASC, h.ParcelCode ASC
            ) AS rn
        FROM dbo.WCS_Parcels_His h
        WHERE h.Pallet_ID = @Pallet_ID
    ),
    PickParcel AS
    (
        SELECT TOP (@InnerPallet)
            s.ParcelCode,
            s.Pallet_ID,
            s.Location,
            s.Line_ID,
            s.ReceivedCode,
            s.CreatedAt
        FROM SourceParcel s
        WHERE s.rn = 1
          AND NOT EXISTS
          (
              SELECT 1
              FROM dbo.WCS_Parcels_Prod p
              WHERE p.ParcelCode = s.ParcelCode
          )
        ORDER BY s.CreatedAt ASC, s.ParcelCode ASC
    )
    INSERT INTO dbo.WCS_Parcels_Prod
    (
        ParcelCode,
        Pallet_ID,
        Location,
        Status,
        Line_ID,
        ReceivedCode
    )
    SELECT
        ParcelCode,
        Pallet_ID,
        Location,
        0 AS Status,
        Line_ID,
        ReceivedCode
    FROM PickParcel;
", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@Pallet_ID", palletId);
                                cmd.Parameters.AddWithValue("@InnerPallet", innerPallet);

                                insertedParcel = cmd.ExecuteNonQuery();
                            }

                            /*
                                5. Nếu số parcel lấy được không đủ Inner_Pallet thì rollback
                            */
                            if (insertedParcel != innerPallet)
                            {
                                tran.Rollback();

                                MessageBox.Show(
                                    "Số parcel reload không đủ theo Inner_Pallet.\n\n" +
                                    "Pallet_ID: " + palletId + "\n" +
                                    "Inner_Pallet yêu cầu: " + innerPallet + "\n" +
                                    "Parcel reload được: " + insertedParcel,
                                    "Thông báo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning
                                );
                                return;
                            }

                            tran.Commit();
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }

                /*
                    6. Sau khi reload DB thành công:
                    - Set đang có task
                    - Gọi InitPlan ở form cha
                */
                GLb.IsInTask = true;

                CallInitPlanFromParentForm();

                MessageBox.Show(
                    "Reload pallet từ lịch sử thành công.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LoadHisPallet();
            }
            catch (Exception ex)
            {
                parentForm.writeLog("Lỗi ReloadPalletFromHis: " + ex.Message);
                Log.LogWrite(Globals.LogLv.Error, "Lỗi ReloadPalletFromHis: " + ex.Message);

                MessageBox.Show(
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void CallInitPlanFromParentForm()
        {
            if (parentForm == null || parentForm.IsDisposed)
            {
                MessageBox.Show(
                    "Không tìm thấy form chính để gọi InitPlan.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (parentForm.InvokeRequired)
            {
                parentForm.BeginInvoke(new Action(() =>
                {
                    parentForm.InitPlan();
                }));
            }
            else
            {
                parentForm.InitPlan();
            }
        }
    }
}
