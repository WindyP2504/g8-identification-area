using EzioDll;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace VTP_Induction.Device
{
    public class CPrinterGodex : CDevice
    {
        public Globals GLb = Globals.getInstance();
        protected TForm m_Form;
        private string p_sNameDevice = string.Empty;
        GodexPrinter Printer = new GodexPrinter();
        protected bool p_bConnection = false;

        public Globals.TPrinterGodex m_ptDeviceConfig
        {
            get
            {
                return (Globals.TPrinterGodex)GLb.g_tDevCfg.tPrinterGodex.Clone();
            }
        }

        public CPrinterGodex(TForm form)
        {
            m_Form = form;
            p_sNameDevice = "PRINTER";
        }

        public virtual bool m_bConnection
        {
            get
            {
                return p_bConnection;
            }
        }

        public string m_sName
        {
            get
            {
                return p_sNameDevice;
            }
        }

        public bool Connect()
        {
            try
            {
                p_bConnection = false;
                if (m_ptDeviceConfig.connectType.Equals(Globals.TPrinterConnectType.USB))
                {
                    if (m_ptDeviceConfig.sUsbName != null)
                        p_bConnection = Printer.bOpenUSB(m_ptDeviceConfig.sUsbName);
                    else
                        Printer.Open(PortType.USB);
                }
                else if (m_ptDeviceConfig.connectType.Equals(Globals.TPrinterConnectType.COM))
                {
                    if (m_ptDeviceConfig.sCom != null)
                    {
                        Printer.Open(m_ptDeviceConfig.sCom);
                        Printer.SetBaudrate(int.Parse(m_ptDeviceConfig.sBaud));
                    }
                }
                else if (m_ptDeviceConfig.connectType.Equals(Globals.TPrinterConnectType.LAN))
                {
                    Printer.Open(m_ptDeviceConfig.sLanIp, int.Parse(m_ptDeviceConfig.sPort));
                }
                else if (m_ptDeviceConfig.connectType.Equals(Globals.TPrinterConnectType.DRIVER))
                {
                    Printer.Open(m_ptDeviceConfig.sDriver);
                }
                return p_bConnection;
            }
            catch
            {
                return p_bConnection;
            }
        }

        public void Disconnect()
        {
            p_bConnection = false;
            Printer.Close();
        }

        // ======= cấu hình NHÃN & MÁY IN (không dùng UI) =======
        const int LABEL_W_MM = 70;
        const int LABEL_H_MM = 40;
        const int GAP_MM = 3; // ~ 2 mm ~ 15 dots cho giấy GAP phổ biến
        const int COPIES = 1;   // số bản in mỗi lệnh
        const int PAGES = 1;   // số "trang" trong một lệnh

        // tốc độ & độ đậm tham khảo; chỉnh theo giấy/tem của bạn
        const int SPEED_IPS = 4;   // 1..8 tuỳ model
        const int DARKNESS = 10;  // 0..30 tuỳ model/giấy

        public bool PrintBarcode(string ParcelCode, string ItemName, string PalletCode, string WH_code, string Khoiluong)
        {
            string currentTime = DateTime.Now.ToString("HH\\hmm\\pss\\s dd/MM/yyyy");

            try
            {
                //Connect();

                // ---- ÁP CẤU HÌNH GIẤY/IN TRỰC TIẾP ----
                Printer.Config.LabelMode(PaperMode.GapLabel, LABEL_H_MM, GAP_MM);
                Printer.Config.LabelWidth(LABEL_W_MM);
                Printer.Config.PageNo(PAGES);
                Printer.Config.CopyNo(COPIES);
                Printer.Config.Speed(SPEED_IPS);
                Printer.Config.Dark(DARKNESS);

                // Bắt đầu in
                Printer.Command.Start();
                int fontH_small = 18;
                int fontH_med = 18;

                int x = 10, y = 20;
                
                string imagePathLogo = Path.Combine(Application.StartupPath, "Images\\G8_Label.bmp");
                Printer.Command.PrintImage(x, y, imagePathLogo, 0);

                y = 90;
                Printer.Command.PrintText(x, y, fontH_small, "Arial", currentTime, 0, FontWeight.FW_200_EXTRALIGHT, RotateMode.Angle_0);

                y = 130;
                int step = 30;
                Printer.Command.PrintText_Unicode(x, y         , fontH_med, "Arial", "Ten SP    : " + ItemName, 0, FontWeight.FW_600_FW_SEMIBOLD, RotateMode.Angle_0);
                Printer.Command.PrintText_Unicode(x , y += step, fontH_med, "Arial", "Ma SP     : " + ParcelCode, 0, FontWeight.FW_600_FW_SEMIBOLD, RotateMode.Angle_0);
                Printer.Command.PrintText_Unicode(x , y += step, fontH_med, "Arial", "Ma Pallet : " + PalletCode, 0, FontWeight.FW_600_FW_SEMIBOLD, RotateMode.Angle_0);
                Printer.Command.PrintText_Unicode(x , y += step, fontH_med, "Arial", "Ma Lenh   : " + WH_code, 0, FontWeight.FW_600_FW_SEMIBOLD, RotateMode.Angle_0);
                Printer.Command.PrintText_Unicode(x , y += step, fontH_med, "Arial", "K.Luong   : " + Khoiluong + " gam", 0, FontWeight.FW_600_FW_SEMIBOLD, RotateMode.Angle_0);

                // QR từ ParcelCode
                int qrX = 280;
                int qrY = 40;
                bool qrPrinted = false;

                // In mã QR với kích thước gấp đôi (Mul = 2)
                int Mul = 10; // Thay đổi giá trị này để tăng giảm kích thước
                int Mode = 3;  // Mức độ mã hóa (Thông thường là 0)
                int Type = 2;  // Loại mã QR (Thông thường là 0)
                string ErrorLevel = "L"; // Mức độ lỗi (L, M, Q, H)
                int Mask = 0; // Mặt nạ mã QR
                int Rotation = 0; // Xoay mã QR (0, 90, 180, 270)

                try
                {
                    // Thử in QR từ SDK của GoDEX (tên tham số có thể thay đổi tùy theo SDK của bạn)
                    //Printer.Command.PrintQRCode(qrX, qrY, ParcelCode);
                    Printer.Command.PrintQRCode(qrX, qrY, Mode, Type, ErrorLevel, Mask, Mul, Rotation, ParcelCode);
                    qrPrinted = true;
                }
                catch { qrPrinted = false; }

                string code = "QR: " + ParcelCode;
                // Nếu QR không in được, in mã ParcelCode thay thế
                if (!qrPrinted)
                {
                    Printer.Command.PrintText_Unicode(
                        qrX, qrY,
                        fontH_small, "Arial",
                        code,
                        0, FontWeight.FW_400_NORMAL, RotateMode.Angle_180);
                }

                // Kết thúc lệnh in
                Printer.Command.End();
                //Disconnect();
                return true;
            }
            catch //(Exception ex)
            {
                return false;
            }
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
                Printer.Config.LabelMode(PaperMode.GapLabel, LABEL_H_MM, GAP_MM);
                Printer.Config.LabelWidth(LABEL_W_MM);
                Printer.Config.PageNo(PAGES);
                Printer.Config.CopyNo(COPIES);
                Printer.Config.Speed(SPEED_IPS);
                Printer.Config.Dark(DARKNESS);

                // Bắt đầu in
                Printer.Command.Start();
                int qrX = 100;
                int qrY = 20;
                //bool qrPrinted = false;

                // In mã QR với kích thước gấp đôi (Mul = 2)
                int Mul = 10; // Thay đổi giá trị này để tăng giảm kích thước
                int Mode = 2;  // Mức độ mã hóa (Thông thường là 0)
                int Type = 1;  // Loại mã QR (Thông thường là 0)
                string ErrorLevel = "L"; // Mức độ lỗi (L, M, Q, H)
                int Mask = 0; // Mặt nạ mã QR
                int Rotation = 0; // Xoay mã QR (0, 90, 180, 270)

                Printer.Command.PrintQRCode(qrX, qrY, Mode, Type, ErrorLevel, Mask, Mul, Rotation, PalletCode);
                Printer.Command.End();
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
