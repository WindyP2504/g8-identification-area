using System;
using System.IO;
using System.Net;
using VTP_Induction.Common;

namespace VTP_Induction.Device
{
    public class WMS : CDevice
    {
        public Globals GLb = Globals.getInstance();
        public TForm m_From;

        public string URL = "http://127.0.0.1:3600/inference_batch?ima`ge_path=";

        public int m_nTimeOutTm = 4500;

        public int m_nDelayTm = 0;

        public bool m_bRunning = false;

        private string m_sNameDevice = string.Empty;

        public int nIndex = -1;

        public WMS(int index)

        {
            this.nIndex = index;
            m_sNameDevice = GLb.g_tDevCfg.tWMS.sDevName;
        }
        public string m_sName
        {
            get
            {
                return m_sNameDevice;
            }
        }
        private bool m_bConnect = false;
        private object syncObj = new object();

        public bool m_bConnection
        {
            get
            {
                if (GLb.g_tSysCfg.bSimulationMode || GLb.g_tSysCfg.bDemoMode)
                {
                    return true;
                }
                return m_bConnect;
            }
        }
        public bool RunProcess(string sInput, out string reponseFromServer)
        {
            m_bRunning = true;
            string sLog = "AIApi::Process ";
            int nTimeOut = m_nTimeOutTm;
            if (nTimeOut < 1000)
            {
                nTimeOut = 1000;
            }

            string select_path = sInput;
            bool bRet = false;
            reponseFromServer = "EMPTY";

            try
            {
                select_path = select_path.Replace(":", "%3A");
                select_path = select_path.Replace("\\", "%5C");
                Log.LogWrite(Globals.LogLv.Information, URL + "... Start ");
                lock (syncObj)
                {
                    WebRequest request = WebRequest.Create(URL + select_path);
                    request.Method = "POST";
                    request.Timeout = nTimeOut;
                    using (WebResponse reponse = request.GetResponse())
                    {
                        using (Stream dataStream = reponse.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(dataStream))
                            {
                                reponseFromServer = reader.ReadToEnd();

                                reader.Close();
                                dataStream.Close();
                                reponse.Close();
                            }
                        }
                    }
                    //reponseFromServer = "[[[1138,103,1150,116],[308,619,319,630],[646,479,657,491]],[1,0,0]]";
                }
                sLog += "<<" + reponseFromServer;
                if (reponseFromServer.IndexOf("[") >= 0)
                {
                    bRet = true;
                }
                else
                {
                    bRet = false;
                }
            }
            catch (Exception ex)
            {
                bRet = false;
                sLog += ex.Message;
                Log.LogWrite(Globals.LogLv.Verbose, ex);

            }
            finally
            {
                m_bRunning = false;
                if (reponseFromServer.Trim() == "")
                {
                    reponseFromServer = "EMPTY";
                }
            }
            Log.LogWrite(Globals.LogLv.Information, sLog);
            return bRet;
        }
        public bool Connect()
        {
            string sOut = "";
            bool bRet = false;
            string sLog = "WMS::Connect ";
            try
            {
                if (GLb.g_tDevCfg.tWMS.bActive)
                {
                    string sFile = string.Empty;
                    //string sFile = GLb.g_tDevCfg.tAIapi[nIndex].sTestImage;
                    if (File.Exists(sFile))
                    {
                        bRet = RunProcess(sFile, out sOut);
                        bRet = bRet && (sOut.IndexOf("[") >= 0);
                    }
                    else
                    {
                        bRet = true;
                    }

                }

                if (bRet)
                {
                    bRet = true;
                    m_bConnect = true;
                }
                else
                {
                    bRet = false;
                    m_bConnect = false;
                }

            }
            catch (Exception ex)
            {
                sLog += ex.Message;
                bRet = false;
                m_bConnect = false;
                Log.LogWrite(Globals.LogLv.Verbose, ex);
            }
            Log.LogWrite(Globals.LogLv.Information, sLog, bRet);
            return bRet;
        }

        public void Disconnect()
        {

        }

    }
}
