using System;

namespace VTP_Induction
{

    public class BCRClass
    {
        private struct buffer
        {
            public int state;

            public msgstruct msgObj;
        }

        public struct msgstruct
        {
            public string receiveTime;

            public string responseMsg;

            public string url;

            public string insertTime;

            public string getoutTime;

            public string msg;
        }

        private string sckIp;

        private int sckPort;

        private int ID;

        private int HWM;

        private int arrayState = 1;

        private int belongPLCid;

        private buffer[] array;

        public BCRClass(int plcid)
        {
            array = new buffer[100];
            HWM = 0;
            belongPLCid = plcid;
        }
        public enum CommType
        {
            RS232,
            TCPIP,
            UDP,
            USB
        }
        public ResultClass.resultStruct insertMsg(string msg, string receiveTime, string insertTime, string url, string responseMsg)
        {
            ResultClass.resultStruct result = default(ResultClass.resultStruct);
            try
            {
                int num = 0;
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].state == 0)
                    {
                        if (i >= HWM)
                        {
                            HWM = i;
                        }
                        array[i].state = 1;
                        array[i].msgObj.msg = msg;
                        array[i].msgObj.receiveTime = receiveTime;
                        array[i].msgObj.insertTime = insertTime;
                        array[i].msgObj.url = url;
                        array[i].msgObj.responseMsg = responseMsg;
                        num = 1;
                        break;
                    }
                }
                if (num == 0)
                {
                    result.result = 0;
                    result.ExceptionMsg = "覆盖报文:" + array[0].msgObj.msg;
                    array[0].msgObj.msg = msg;
                    array[0].msgObj.receiveTime = receiveTime;
                    array[0].msgObj.insertTime = insertTime;
                    array[0].msgObj.url = url;
                    array[0].msgObj.responseMsg = responseMsg;
                }
                else
                {
                    result.result = 1;
                    result.Rmsg = HWM.ToString();
                }
            }
            catch (Exception ex)
            {
                result.result = 0;
                result.ExceptionMsg = ex.Message;
            }
            return result;
        }

        public ResultClass.resultStruct getMsg(ResultClass.resultStruct result, int len)
        {
            try
            {
                int num = 0;
                for (int i = 0; i <= HWM; i++)
                {
                    if (len <= 0)
                    {
                        break;
                    }
                    if (result.Rmsg.Length >= 8000)
                    {
                        break;
                    }
                    if (array[i].state != 1)
                    {
                        continue;
                    }
                    for (int j = 0; j < result.msgArray.Length; j++)
                    {
                        if (result.msgArray[j].msg == null)
                        {
                            array[i].msgObj.getoutTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");


                            //ref msgstruct reference = ref result.msgArray[j];



                            msgstruct reference = result.msgArray[j];
                            reference = array[i].msgObj;
                            break;
                        }
                    }
                    result.len++;
                    result.Rmsg = result.Rmsg + array[i].msgObj.msg + "||";
                    array[i].state = 0;
                    array[i].msgObj.msg = null;
                    array[i].msgObj.receiveTime = null;
                    array[i].msgObj.insertTime = null;
                    array[i].msgObj.getoutTime = null;
                    array[i].msgObj.url = null;
                    array[i].msgObj.responseMsg = null;
                    num = i;
                    len--;
                }
                if (num >= HWM)
                {
                    int num2 = 0;
                    for (int i = num; i >= 0; i--)
                    {
                        if (array[i].state == 1)
                        {
                            HWM = i;
                            num2 = 1;
                            break;
                        }
                    }
                    if (num2 == 0)
                    {
                        HWM = 0;
                    }
                }
                result.result = 1;
            }
            catch (Exception ex)
            {
                result.result = 0;
                result.ExceptionMsg = ex.Message;
            }
            return result;
        }

        public ResultClass.resultStruct clearArray(ResultClass.resultStruct result)
        {
            try
            {
                arrayState = 0;
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].state == 1)
                    {
                        string rmsg = result.Rmsg;
                        result.Rmsg = rmsg + array[i].msgObj.msg + "|ReceiveTime:" + array[i].msgObj.receiveTime + "|insertTime:" + array[i].msgObj.insertTime + "||";
                        array[i].state = 0;
                        array[i].msgObj.msg = null;
                        array[i].msgObj.receiveTime = null;
                        array[i].msgObj.insertTime = null;
                        array[i].msgObj.getoutTime = null;
                        result.result = 1;
                    }
                }
                HWM = 0;
                arrayState = 1;
            }
            catch (Exception ex)
            {
                result.result = 0;
                result.ExceptionMsg = ex.Message;
            }
            return result;
        }

        public string getsckIp()
        {
            return sckIp;
        }

        public void setsckIp(string val)
        {
            sckIp = val;
        }

        public int getsckPort()
        {
            return sckPort;
        }

        public void setsckPort(int val)
        {
            sckPort = val;
        }

        public int getID()
        {
            return ID;
        }

        public void setID(int val)
        {
            ID = val;
        }

        public int getbelongPlcID()
        {
            return belongPLCid;
        }

        public void setbelongPlcID(int val)
        {
            belongPLCid = val;
        }

        public int getHWM()
        {
            return HWM;
        }

        public int getArrayState()
        {
            return arrayState;
        }
    }
}