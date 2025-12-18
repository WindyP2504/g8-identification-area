using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace VTP_Induction
{
    public static class MakeLogFile
    {
        private static FileStream fsSystemEvent = new FileStream(Application.StartupPath + "\\log\\" + DateTime.Now.ToString("yyyyMMdd") + "systemEvent.txt", FileMode.Append, FileAccess.Write);

        public static void WriteSystemEvent(string text)
        {
            if (fsSystemEvent.Name.Contains(DateTime.Now.ToString("yyyyMMdd")))
            {
                StringBuilder strb = new StringBuilder();
                strb.Append(DateTime.Now.ToString("HH:mm:ss-fff"));
                strb.Append("\t");
                strb.Append(text);
                strb.Append("\r\n");
                byte[] b = Encoding.Default.GetBytes(strb.ToString());
                fsSystemEvent.Write(b, 0, b.Length);
                fsSystemEvent.Flush();
            }
            else
            {
                fsSystemEvent = new FileStream(Application.StartupPath + "\\log\\" + DateTime.Now.ToString("yyyyMMdd") + "systemEvent.txt", FileMode.Append, FileAccess.Write);
                StringBuilder strb = new StringBuilder();
                strb.Append(DateTime.Now.ToString("HH:mm:ss-fff"));
                strb.Append("\t");
                strb.Append(text);
                strb.Append("\r\n");
                byte[] b = Encoding.Default.GetBytes(strb.ToString());
                fsSystemEvent.Write(b, 0, b.Length);
                fsSystemEvent.Flush();
            }
        }
    }
}
