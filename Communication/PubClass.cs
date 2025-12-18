using System;
using System.IO;
using System.Xml;

namespace VTP_Induction
{

    public class PubClass
    {
        public struct Configure
        {
            public string ConnectDBString;

            public int MsgFileQty;

            public string Title;

            public string DBType;

            public int HBFrequency;

            public int ConnProbeFrequency;

            public int isSendHB;

            public string UrlWcs201;

            public int HttpTimeout;

            public string PLCDesc;

            public string BcrDesc;

            public string ProductName;

            public string pwd;
        }

        public struct writeLoginfo
        {
            public string Directory;

            public string FileName;

            public string loginfo;
        }

        public static Configure ConfigureParameter;

        public static void WriteLogFile(string DirName, string fileName, string info)
        {
            try
            {
                if (!Directory.Exists(DirName))
                {
                    Directory.CreateDirectory(DirName);
                }
                FileInfo fileInfo = new FileInfo(DirName + "\\" + fileName);
                if (!fileInfo.Exists)
                {
                    FileStream fileStream = File.Create(DirName + "\\" + fileName);
                    fileStream.Close();
                    fileInfo = new FileInfo(DirName + "\\" + fileName);
                    fileStream.Dispose();
                }
                FileStream fileStream2 = fileInfo.OpenWrite();
                StreamWriter streamWriter = new StreamWriter(fileStream2);
                streamWriter.BaseStream.Seek(0L, SeekOrigin.End);
                streamWriter.Write("{0} \n\r", info);
                streamWriter.Flush();
                streamWriter.Close();
                streamWriter.Dispose();
                fileStream2.Dispose();
                fileInfo = null;
            }
            catch
            {
            }
        }

        public static Configure ReadXMLfile(string FileName)
        {
            Configure result = default(Configure);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(FileName);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("configure/DBType");
            XmlNode xmlNode2 = xmlDocument.SelectSingleNode("configure/MySqlConnStr");
            XmlNode xmlNode3 = xmlDocument.SelectSingleNode("configure/SQLServerConnStr");
            XmlNode xmlNode4 = xmlDocument.SelectSingleNode("configure/MsgFileQty");
            XmlNode xmlNode5 = xmlDocument.SelectSingleNode("configure/Title");
            XmlNode xmlNode6 = xmlDocument.SelectSingleNode("configure/UrlWcs201");
            XmlNode xmlNode7 = xmlDocument.SelectSingleNode("configure/HttpTimeout");
            XmlNode xmlNode8 = xmlDocument.SelectSingleNode("configure/OBRDesc");
            XmlNode xmlNode9 = xmlDocument.SelectSingleNode("configure/ConnProbeFrequency");
            XmlNode xmlNode10 = xmlDocument.SelectSingleNode("configure/HBFrequency");
            XmlNode xmlNode11 = xmlDocument.SelectSingleNode("configure/isSendHB");
            XmlNode xmlNode12 = xmlDocument.SelectSingleNode("configure/OracleConnStr");
            XmlNode xmlNode13 = xmlDocument.SelectSingleNode("configure/PLCDesc");
            XmlNode xmlNode14 = xmlDocument.SelectSingleNode("configure/ProductName");
            if (xmlNode.InnerText == "SQLSERVER")
            {
                result.ConnectDBString = xmlNode3.InnerText;
            }
            else if (xmlNode.InnerText == "Oracle")
            {
                result.ConnectDBString = xmlNode12.InnerText;
            }
            result.DBType = xmlNode.InnerText;
            result.MsgFileQty = Convert.ToInt32(xmlNode4.InnerText);
            result.Title = xmlNode5.InnerText;
            result.ConnProbeFrequency = Convert.ToInt32(xmlNode9.InnerText);
            result.HBFrequency = Convert.ToInt32(xmlNode10.InnerText);
            result.isSendHB = Convert.ToInt32(xmlNode11.InnerText);
            result.PLCDesc = xmlNode13.InnerText;
            result.BcrDesc = xmlNode8.InnerText;
            result.ProductName = xmlNode14.InnerText;
            result.UrlWcs201 = xmlNode6.InnerText;
            result.HttpTimeout = Convert.ToInt16(xmlNode7.InnerText);
            return result;
        }

        public static string getBytesToHexStr(byte[] msg, int len)
        {
            string text = "";
            for (int i = 0; i < len; i++)
            {
                text += Convert.ToString(msg[i], 16).PadLeft(2, '0').ToUpper();
            }
            return text;
        }

        public static string getBytesToStr(byte[] msg, int len)
        {
            string text = "";
            for (int i = 0; i < len; i++)
            {
                text += Convert.ToChar(msg[i]).ToString().ToUpper();
            }
            return text;
        }

        public static byte[] getBytes(string msg)
        {
            byte[] array = new byte[msg.Length / 2];
            int num = 0;
            while (msg.Length > 0)
            {
                array[num++] = Convert.ToByte(Convert.ToInt32(msg.Substring(0, 2), 16));
                msg = ((msg.Length <= 2) ? "" : msg.Substring(2));
            }
            return array;
        }
    }
}