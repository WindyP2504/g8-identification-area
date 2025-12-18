using System;
using System.Net.Sockets;
using System.Threading;

namespace VTP_Induction.Device
{
    class PLCSocketConnect
    {
        public Globals GLb = Globals.getInstance();
        protected Thread PLCsocketProcess;
        public TForm Tform;

        public PLCSocketConnect(TForm Form)
        {
            Tform = Form;

        }
        public void Connect()
        {
            try
            {
                if (!GLb.tcpclnt.Client.Connected)
                {
                    GLb.tcpclnt = new TcpClient("127.0.0.1", 8001);
                    Tform.writeLog("[PLC] Connecting to 127.0.0.1 ");
                }
                else
                {
                    Tform.writeLog("[PLC] is Connected");
                }
                GLb.stm = GLb.tcpclnt.GetStream();
                byte[] ba = new byte[]
                {
                    252,
                    252,
                    2,
                    2,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    253,
                    253
                };
                GLb.stm.Write(ba, 0, ba.Length);
                Thread.Sleep(20000);
            }
            catch (Exception ex)
            {
                Tform.writeLog("ERROR: " + ex.Message);
                Thread.Sleep(5000);
            }
        }
        public int CheckPLCstatus()
        {
            int bRet = -99;
            if (GLb.tcpclnt.Client.Connected)
            {
                bRet = 1;
            }
            return bRet;
        }
        public void connectFeeder()
        {
            while (true)
            {
                try
                {

                    if (!GLb.tcpclnt.Client.Connected)
                    {
                        Tform.writeLog("[PLC] : Connecting..... ");
                        GLb.tcpclnt = new TcpClient(GLb.getIPadd(GLb.n_InductionNumber), 2300);
                        Tform.writeLog("[PLC] : PLC is Connected ");
                        GLb.stm = GLb.tcpclnt.GetStream();
                        GLb.stm.WriteTimeout = 100;
                        byte[] ba = new byte[]
                        {
                            252,
                            252,
                            2,
                            2,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            253,
                            253
                        };
                        GLb.stm.Write(ba, 0, ba.Length);
                    }
                    else
                    {
                        Tform.writeLog("[PLC] is Connected");
                    }
                    Thread.Sleep(15000);
                }
                catch (Exception ex)
                {
                    //MainForm.SetText(this, this.lblPlcStatus, "Disconnected");
                    Tform.writeLog("ERROR: CONNECT " + ex.Message);
                    Thread.Sleep(5000);
                }
            }
        }

        public void StartThread()
        {
            PLCsocketProcess = new Thread(new ThreadStart(this.connectFeeder));
            PLCsocketProcess.IsBackground = true;
            PLCsocketProcess.Start();
        }

    }
}
