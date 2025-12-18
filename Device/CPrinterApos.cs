using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using VTP_Induction.Common;

namespace VTP_Induction.Device
{
    public class CPrinterApos : CDevice
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
        public Globals.TPrinterConfig m_ptDeviceConfig
        {
            get
            {
                return (Globals.TPrinterConfig)GLb.g_tDevCfg.tPRINTER.Clone();
            }
        }
        public Globals.TDevUtilDeviceConfig m_ptConfig
        {
            get
            {
                return m_ptDeviceConfig;
            }
        }
        public Globals.TCommConfig m_ptCommCfg
        {
            get
            {
                return m_ptConfig.tCommCfg;
            }
        }

        public CPrinterApos(TForm Form)
        {
            m_Form = Form;
            this.p_sNameDevice = m_ptCommCfg.sDevName;
        }
        public string printePort { get; set; }
        IntPtr printer;
        int openStatus = 100;

        public bool Connect()
        {
            //Disconnect();

            bool bRet = false;
            string sLog = "{ Printer " + m_sName + "::Connect ";


            if (openStatus == 0)
            {
                CPCLPrint.ClosePort(printer);
                CPCLPrint.ReleasePrinter(printer);
            }
            printer = CPCLPrint.InitPrinter("");
            openStatus = CPCLPrint.OpenPort(printer, "USB,");
            if (openStatus == 0)
            {
                bRet = true;
            }
            else
            {
                bRet = false;
            }
            //bRet = Printer.openport("TSC PEX-2360R");


            if (!bRet)
            {
                return bRet;
            }

            p_bConnection = bRet;
            Log.LogWrite(Globals.LogLv.Information, sLog, bRet);
            return bRet;
        }
        public void Disconnect()
        {
            string sLog = "{" + m_sName + "::Disconnect} ";
            Log.LogWrite(Globals.LogLv.Information, sLog);
            if (openStatus == 0)
            {
                CPCLPrint.ClosePort(printer);
                CPCLPrint.ReleasePrinter(printer);
            }
            openStatus = 100;

        }

        //public bool PrintBarcode(string TextString)
        //{
        //    bool bRet = false;
        //    string imagePath1 = Path.Combine(Application.StartupPath, "Images\\anh3.bmp");
        //    string imagePath2 = Path.Combine(Application.StartupPath, "Images\\G8.bmp");
        //    string currentTime = DateTime.Now.ToString("HH'h'mm'p' dd/MM/yyyy");
        //    try
        //    {
        //        if (openStatus != 0)
        //        {
        //            bRet = false;
        //            //tb_Msg.Text += DateTime.Now.ToString("G") + " : " + "Please connect the printer first!\r\n";
        //            //return;
        //        }
        //        CPCLPrint.CPCL_AddLabel(printer, 15, 1200, 1);
        //        CPCLPrint.CPCL_SetAlign(printer, 0);
        //        CPCLPrint.CPCL_AddBox(printer, 0, 0, 600, 250, 5); //Khung 1
        //        CPCLPrint.CPCL_AddBox(printer, 0, 250, 600, 550, 5); //khung 2
        //        CPCLPrint.CPCL_AddBox(printer, 0, 550, 600, 1000, 5); //khung 3
        //        CPCLPrint.CPCL_AddImage(printer, 0, 300, 0, imagePath2); //in ảnh 1
        //        CPCLPrint.CPCL_AddText(printer, 0, "4", 7, 10, 20, currentTime); // thoi gian
        //        CPCLPrint.CPCL_AddText(printer, 0, "5", 7, 10, 100, "G8 HOME"); //text 1
        //        CPCLPrint.CPCL_AddBarCodeText(printer, 1, 7, 0, 0);
        //        CPCLPrint.CPCL_AddBarCode(printer, 0, 20, 3, 2, 100, 30, 350, TextString); // barcode
        //        CPCLPrint.CPCL_AddQRCode(printer, 0, 300, 650, 2, 10, 0, TextString); // QRcode
        //        CPCLPrint.CPCL_AddText(printer, 0, "4", 7, 220, 900, "Total: 5kg"); // text 2
        //        CPCLPrint.CPCL_AddImage(printer, 0, 20, 600, imagePath1); //in ảnh 2
        //        CPCLPrint.CPCL_Print(printer);
        //        bRet = true;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //        bRet = false;
        //    }
        //    return bRet;
        //}
        public bool PrintBarcode(string ParcelCode, string ItemName, string PalletCode, string WH_code, string Khoiluong)
        {
            bool bRet = false;
            // Đường dẫn đến 2 ảnh
            string imagePath1 = Path.Combine(Application.StartupPath, "Images\\anh3.bmp");
            string imagePath2 = Path.Combine(Application.StartupPath, "Images\\G8.bmp");

            // Lấy thời gian hiện tại để in
            string currentTime = DateTime.Now.ToString("HH'h'mm'p' dd/MM/yyyy");

            try
            {
                if (openStatus != 0)
                {
                    // Nếu chưa mở kết nối máy in
                    return false;
                }
                // CPCLPrint.
                // Chuyển khổ in sang 5cm x 5cm:
                //  1 inch = 25.4mm, máy in 200 dpi ⇒ 1 cm ≈ 78.74 dots
                //  ⇒ 5 cm ≈ 394 dots
                int labelSize = 394;
                CPCLPrint.CPCL_AddLabel(printer, /*gap*/15, /*height*/labelSize, /*copies*/1);
                CPCLPrint.CPCL_SetAlign(printer, 0);

                // Kẻ sườn tham khảo (bạn có thể bỏ nếu không cần)
                // CPCLPrint.CPCL_AddBox(printer, 0, 0, labelSize, labelSize, 1);

                // In logo G8 ở góc trên trái
                //  CPCLPrint.CPCL_AddImage(printer, /*mode*/0,
                //    /*x*/10, /*y*/10,
                //                         imagePath2);

                // In thời gian ngay dưới logo
                CPCLPrint.CPCL_AddText(printer, /*direct*/0, /*font*/"4", /*size*/3,
                    /*x*/20, /*y*/10,
                                       currentTime);

                // In dòng “G8 HOME” phía dưới
                CPCLPrint.CPCL_AddText(printer, 0, "5", 3,
                                       20, 40,
                                       "G8 HOME");

                // In barcode chính giữa, dài 120 dots, cao 40 dots
                //CPCLPrint.CPCL_AddBarCodeText(printer, 1, 7, 0, 0);
                //CPCLPrint.CPCL_AddBarCode(printer, /*dir*/0,
                //    /*x*/10, /*narrow*/3, /*wide*/2,
                //    /*height*/120, /*whitespace*/40,
                //    /*y*/10 + 140,
                //                         TextString);

                //In QR-code ở nửa phải, cỡ module 7 dots
                CPCLPrint.CPCL_AddQRCode(printer, /*dir*/0,
                    /*x*/120, /*y*/10 + 50,
                    /*cellSize*/2, /*widthCells*/5, /*rot*/2,
                                        ParcelCode);

                // In dòng tổng khối lượng ở đáy trái

                CPCLPrint.CPCL_AddText(printer, 0, "5", 2,
                       20, 190,
                       "Ten SP: " + ItemName);
                CPCLPrint.CPCL_AddText(printer, 0, "5", 2,
                       20, 220,
                       "Ma SP: " + ParcelCode);
                CPCLPrint.CPCL_AddText(printer, 0, "5", 2,
                       20, 250,
                       "Ma Pallet: " + PalletCode);
                CPCLPrint.CPCL_AddText(printer, 0, "5", 2,
                       20, 280,
                       "Ma Lenh: " + WH_code);
                CPCLPrint.CPCL_AddText(printer, 0, "5", 2,
                                     20, 310,
                                       "Khoi Luong:" + Khoiluong + "g");
                // In ảnh thứ hai ở đáy phải
                //CPCLPrint.CPCL_AddImage(printer, 0,
                //    /*x*/200, /*y*/labelSize - 80,
                //                         imagePath1);

                // Cuối cùng: gửi lệnh in
                CPCLPrint.CPCL_Print(printer);
                Thread.Sleep(100);
                bRet = true;

            }
            catch
            {
                bRet = false;
                throw;
            }

            return bRet;
        }

        public bool PrintPallet(string PalletCode)
        {
            bool bRet = false;
            // Đường dẫn đến 2 ảnh
            //string imagePath1 = Path.Combine(Application.StartupPath, "Images\\anh3.bmp");
            //string imagePath2 = Path.Combine(Application.StartupPath, "Images\\G8.bmp");

            Thread.Sleep(10000);
            // Lấy thời gian hiện tại để in
            string currentTime = DateTime.Now.ToString("HH'h'mm'p' dd/MM/yyyy");

            try
            {
                if (openStatus != 0)
                {
                    // Nếu chưa mở kết nối máy in
                    return false;
                }

                // Chuyển khổ in sang 5cm x 5cm:
                //  1 inch = 25.4mm, máy in 200 dpi ⇒ 1 cm ≈ 78.74 dots
                //  ⇒ 5 cm ≈ 394 dots
                int labelSize = 394;
                CPCLPrint.CPCL_AddLabel(printer, /*gap*/15, /*height*/labelSize, /*copies*/1);
                CPCLPrint.CPCL_SetAlign(printer, 0);

                // Kẻ sườn tham khảo (bạn có thể bỏ nếu không cần)
                // CPCLPrint.CPCL_AddBox(printer, 0, 0, labelSize, labelSize, 1);

                // In logo G8 ở góc trên trái
                //  CPCLPrint.CPCL_AddImage(printer, /*mode*/0,
                //    /*x*/10, /*y*/10,
                //                         imagePath2);

                // In thời gian ngay dưới logo
                CPCLPrint.CPCL_AddText(printer, /*direct*/0, /*font*/"4", /*size*/3,
                    /*x*/100, /*y*/10,
                                       currentTime);

                // In dòng “G8 HOME” phía dưới
                CPCLPrint.CPCL_AddText(printer, 0, "5", 3,
                                       100, 40,
                                       "G8 HOME");

                // In barcode chính giữa, dài 120 dots, cao 40 dots
                //CPCLPrint.CPCL_AddBarCodeText(printer, 1, 7, 0, 0);
                //CPCLPrint.CPCL_AddBarCode(printer, /*dir*/0,
                //    /*x*/10, /*narrow*/3, /*wide*/2,
                //    /*height*/120, /*whitespace*/40,
                //    /*y*/10 + 140,
                //                         TextString);

                //In QR-code ở nửa phải, cỡ module 7 dots
                CPCLPrint.CPCL_AddQRCode(printer, /*dir*/0,
                    /*x*/250, /*y*/10 + 50,
                    /*cellSize*/2, /*widthCells*/6, /*rot*/2,
                                        PalletCode);

                // In dòng tổng khối lượng ở đáy trái

                CPCLPrint.CPCL_AddText(printer, 0, "5", 4,
                       100, 260,
                       "MA PALLET: " + PalletCode);






                // In ảnh thứ hai ở đáy phải
                //CPCLPrint.CPCL_AddImage(printer, 0,
                //    /*x*/200, /*y*/labelSize - 80,
                //                         imagePath1);

                // Cuối cùng: gửi lệnh in
                CPCLPrint.CPCL_Print(printer);
                bRet = true;
            }
            catch
            {
                bRet = false;
                throw;
            }

            return bRet;
        }

    }
}
