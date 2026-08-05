using EzioDll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
        const int SPEED_IPS = 2;   // 1..8 tuỳ model
        const int DARKNESS = 10;  // 0..30 tuỳ model/giấy

        public bool PrintBarcode(string stt, string ParcelCode, string ItemName, string PalletCode, string Khoiluong, string InnerCtn)
        {

            string currentTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy");

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

                // QR từ ParcelCode
                int qrX = 10;
                int qrY = 10;
                bool qrPrinted = false;

                // In mã QR với kích thước gấp đôi (Mul = 2)
                int Mul = 7; // Thay đổi giá trị này để tăng giảm kích thước
                int Mode = 3;  // Mức độ mã hóa (Thông thường là 0)
                int Type = 2;  // Loại mã QR (Thông thường là 0)
                string ErrorLevel = "L";
                int Mask = 0;
                int Rotation = 0; // Xoay mã QR (0, 90, 180, 270)

                try
                {
                    var res = Printer.Command.PrintQRCode(qrX, qrY, Mode, Type, ErrorLevel, Mask, Mul, Rotation, ParcelCode);
                    if (res != 0)
                        qrPrinted = true;
                    else
                        qrPrinted = false;
                }
                catch { qrPrinted = false; }

                int fontH_med = 21;

                int x = 260, y = 20;
                int step = 40;
                Printer.Command.PrintText_Unicode(x, y, fontH_med, "Arial", "Tên sản phẩm: ", 0, FontWeight.FW_400_NORMAL, RotateMode.Angle_0);
                PrintItemNameAuto(x, ref y, ItemName);
                Printer.Command.PrintText_Unicode(x, y += step, fontH_med, "Arial", "Quy cách   : " + InnerCtn + " chiếc", 0, FontWeight.FW_400_NORMAL, RotateMode.Angle_0);
                Printer.Command.PrintText_Unicode(x, y += step, fontH_med, "Arial", "Ngày nhập : " + currentTime, 0, FontWeight.FW_400_NORMAL, RotateMode.Angle_0);
                Printer.Command.PrintText_Unicode(470, 10, 80, "Arial", stt, 0, FontWeight.FW_900_HEAVY, RotateMode.Angle_0);
                x = 10; y = 255;
                Printer.Command.PrintText_Unicode(x, y, 16, "Arial", "Mã định danh: " + ParcelCode, 0, FontWeight.FW_400_NORMAL, RotateMode.Angle_0);

                // Kết thúc lệnh in
                Printer.Command.End();
                //Printer.Close();
                //Disconnect();
                return qrPrinted;
            }
            catch //(Exception ex)
            {
                return false;
            }
        }

        private List<string> WrapText(string text, int maxCharsPerLine)
        {
            List<string> lines = new List<string>();

            while (text.Length > maxCharsPerLine)
            {
                int wrapAt = text.LastIndexOf(' ', maxCharsPerLine);
                if (wrapAt <= 0) wrapAt = maxCharsPerLine;

                lines.Add(text.Substring(0, wrapAt).Trim());
                text = text.Substring(wrapAt).Trim();
            }

            if (!string.IsNullOrEmpty(text))
                lines.Add(text);

            return lines;
        }

        private void PrintItemNameAuto(int x, ref int y, string itemName)
        {
            int baseFont = 22; // giảm font mặc định
            int fontSize = baseFont;

            // Auto scale font (nhỏ hơn so với bản gốc)
            if (itemName.Length > 24) fontSize = 20;
            if (itemName.Length > 36) fontSize = 18;
            if (itemName.Length > 48) fontSize = 16;

            // Step phụ thuộc font → KHÔNG bị đè chữ
            int step = (int)(fontSize * 1.8);

            // max ký tự theo font (giảm xuống)
            int maxChars = 20;
            if (fontSize == 20) maxChars = 22;
            if (fontSize == 18) maxChars = 24;
            if (fontSize == 16) maxChars = 26;

            var lines = WrapText(itemName, maxChars);

            // Giới hạn 3 dòng
            if (lines.Count > 3)
            {
                lines = lines.Take(3).ToList();
                lines[2] += "...";
            }

            foreach (var line in lines)
            {
                y += step;
                Printer.Command.PrintText_Unicode(
                    x, y,
                    fontSize,
                    "Arial",
                    line,
                    0,
                    FontWeight.FW_600_FW_SEMIBOLD,
                    RotateMode.Angle_0
                );
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
