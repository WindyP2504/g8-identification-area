
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using VTP_Induction.Common;
using VTP_Induction.Device;

namespace VTP_Induction
{
    public class MacroFunc : INotifyPropertyChanged, ICloneable
    {
        private CMDListCtrl cmdStr = CMDListCtrl.getInstance();
        private Globals GLb = Globals.getInstance();

        private CMDProperty cmd = new CMDProperty();

        public int nSeqNo = 0;

        public bool isSkip = false;


        public bool isPrevNG = false;

        public bool isBeforeNG = false;

        public string measResult = string.Empty;

        public string mesData = string.Empty;

        public bool isBlank = false;

        public bool isSetRetAsCheck = false;

        public bool isSetRetEndSeq = false;

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessors of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        /*
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {            
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }*/
        private void NotifyPropertyChanged(string property = "")
        {
            synchronizationContext.Send(SendRaisePropertyChanged, property);
        }

        void SendRaisePropertyChanged(object state)
        {
            RaisePropertyChanged((string)state);
        }

        private void RaisePropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public MacroFunc()
        {
            //displayName = "[" + type + "]" + cmd.ToString();
        }

        public string MeasResult
        {
            get { return measResult; }
            set
            {
                if (value != measResult)
                {
                    measResult = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string sOKNG = string.Empty; // OK or NG

        public string SOKNG
        {
            get { return sOKNG; }
            set
            {
                if (value != sOKNG)
                {
                    sOKNG = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public CMDProperty Cmd
        {
            get { return cmd; }
            set
            {
                if (value.cmpType != cmd.cmpType ||
                    value.mainData != cmd.mainData ||
                    value.name != cmd.name ||
                    value.parserString != cmd.parserString)
                {
                    cmd.cmpType = value.cmpType;
                    cmd.mainData = value.mainData;
                    cmd.name = value.name;
                    cmd.parserString = value.parserString;

                    NotifyPropertyChanged();
                }
            }
        }

        private string displayName = string.Empty;

        public string DisplayName
        {
            get { return displayName; }
            set
            {
                if (value != displayName)
                {
                    displayName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string mesCode = string.Empty;

        public string MesCode
        {
            get { return mesCode.Trim(); }
            set
            {
                if (value != mesCode)
                {
                    mesCode = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsSkip
        {
            get { return isSkip; }
            set
            {
                if (value != isSkip)
                {
                    isSkip = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string label = string.Empty;

        public string CmdLabel
        {
            get { return label; }
            set
            {
                if (value != label)
                {
                    label = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string labelCnt = string.Empty;

        public string LabelCnt
        {
            get { return labelCnt; }
            set
            {
                int temp = 0;
                if (value != labelCnt && Int32.TryParse(value, out temp) && temp > 0)
                {
                    labelCnt = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string delayTime = string.Empty;
        public string DelayTime
        {
            get { return delayTime; }
            set
            {
                int ntemp = 0;
                if (value != delayTime && Int32.TryParse(value, out ntemp) && ntemp >= 0)
                {
                    delayTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int n_delayTime
        {
            get
            {
                int ntemp = 0;
                Int32.TryParse(delayTime, out ntemp);
                return ntemp;
            }
        }

        private string timeout = string.Empty;
        public string Timeout
        {
            get { return timeout; }
            set
            {
                int ntemp = 0;
                if (value != timeout && Int32.TryParse(value, out ntemp) && ntemp >= 0)
                {
                    timeout = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int n_timeout
        {
            get
            {
                int ntemp = 0;
                Int32.TryParse(timeout, out ntemp);
                return ntemp;
            }
        }

        private string retry = string.Empty;
        public string Retry
        {
            get { return retry; }
            set
            {
                int ntemp = 0;
                if (value != retry && Int32.TryParse(value, out ntemp) && ntemp >= 0)
                {
                    retry = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int n_retry
        {
            get
            {
                int ntemp = 0;
                Int32.TryParse(retry, out ntemp);
                return ntemp;
            }
        }

        private long timeInMs = 0;

        public long TimeInMs
        {
            set
            {
                if (value != timeInMs)
                {
                    timeInMs = value;
                    NotifyPropertyChanged();
                }
            }
            get
            {
                return timeInMs;
            }
        }
        //public string compare = string.Empty;

        public string Compare
        {
            get { return cmdStr.CompareTypeStrList[(int)cmd.cmpType]; }
            set
            {
                if (value != cmdStr.CompareTypeStrList[(int)cmd.cmpType])
                {
                    int idx = cmdStr.CompareTypeStrList.IndexOf(value);
                    if (idx != -1)
                    {
                        cmd.cmpType = (COMPARE_TYPE)idx;
                        NotifyPropertyChanged();
                    }
                }
            }
        }

        private string min = string.Empty;

        public string Min
        {
            get { return min; }
            set
            {
                if (value != min)
                {
                    min = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string max = string.Empty;

        public string Max
        {
            get { return max; }
            set
            {
                if (value != max)
                {
                    max = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string option = string.Empty;

        //public string Option
        //{
        //    get { return cmd.parserString; }
        //    set
        //    {
        //        if (value != cmd.parserString)
        //        {
        //            string filePar = GLb.g_sParserStrDir + value + ".par";
        //            if (File.Exists(filePar))
        //            {
        //                cmd.parserString = value;
        //                option = File.ReadAllText(filePar);
        //                NotifyPropertyChanged();
        //            }
        //        }
        //    }
        //}
        public string Option
        {
            get { return option; }
            set
            {
                if (value != option)
                {
                    option = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string par1 = string.Empty;
        public string Par1
        {
            get { return par1; }
            set
            {
                if (value != par1)
                {
                    par1 = value;
                    NotifyPropertyChanged();
                }

            }
        }
        public int n_Par1
        {
            get
            {
                int ntemp = 7;
                Int32.TryParse(par1, out ntemp);
                return ntemp;
            }
        }
        public double d_Par1Val(int nPos)
        {
            double dtemp = 0;
            if (nPos < 0)
            {
                return dtemp;
            }

            if (par1.Trim() == "")
            {
                return dtemp;
            }

            string[] sArr = par1.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (sArr != null && sArr.Length > nPos)
            {
                double.TryParse(sArr[nPos], out dtemp);
            }
            return dtemp;
        }
        public int n_Par1Val(int nPos)
        {
            return (int)d_Par1Val(nPos);
        }


        private string par2 = string.Empty;

        public string Par2
        {
            get { return par2; }
            set
            {
                if (value != par2)
                {
                    par2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string MeasItemsName = string.Empty;

        private string expr = string.Empty;

        public string Expr
        {
            get { return expr; }
            set
            {
                if (value != expr)
                {
                    expr = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string type = string.Empty;

        public string CMDType
        {
            get { return type; }
            set
            {
                if (value != type)
                {
                    type = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override string ToString()
        {
            return displayName;
        }

        public bool bRes = false;

        public List<string> sListMeasData = new List<string>();

        public List<bool> bListMeasRes = new List<bool>();

        //public List<PinvisionLib.ResultPackage>  PinvisionData = new List<PinvisionLib.ResultPackage>();

        //public int PinvisionRes = Globals.RESULT_OK;

        //public bool PinvisionPrevNG = false;


        public object Clone()
        {
            MacroFunc ret = new MacroFunc();
            ret.type = this.type;

            ret.cmd.cmpType = this.cmd.cmpType;
            ret.cmd.mainData = this.cmd.mainData;
            ret.cmd.name = this.cmd.name;
            ret.cmd.parserString = this.cmd.parserString;

            ret.displayName = this.displayName;
            ret.mesCode = this.mesCode;
            ret.isSkip = this.isSkip;
            ret.label = this.label;
            ret.labelCnt = this.labelCnt;
            //ret.caseNG = this.caseNG;
            ret.delayTime = this.delayTime;
            ret.timeout = this.timeout;
            ret.retry = this.retry;
            ret.min = this.min;
            ret.max = this.max;
            ret.option = this.option;
            ret.par1 = this.par1;
            ret.par2 = this.par2;
            ret.expr = this.expr;
            ret.bRes = this.bRes;
            ret.MeasItemsName = this.MeasItemsName;
            ret.sListMeasData = this.sListMeasData;
            return ret;
        }

        public void ResetData()
        {
            // Not reset type

            this.cmd.cmpType = COMPARE_TYPE.MIN_LOWEROREQUAL_VALUE_LOWEROREQUAL_MAX;
            this.cmd.mainData = string.Empty;
            this.cmd.name = string.Empty;
            this.cmd.parserString = string.Empty;

            this.displayName = "[" + type + "]";
            this.mesCode = string.Empty;
            this.isSkip = false;
            this.label = string.Empty;
            this.labelCnt = string.Empty;
            //this.caseNG = string.Empty;
            this.delayTime = string.Empty;
            this.timeout = string.Empty;
            this.retry = string.Empty;
            this.min = string.Empty;
            this.max = string.Empty;
            this.option = string.Empty;
            this.par1 = string.Empty;
            this.par2 = string.Empty;
            this.expr = string.Empty;
            this.MeasItemsName = string.Empty;
        }


        private static string[] ManualSplitData(string lineStr, string comma)
        {
            List<string> retStr = new List<string>();
            int startIdx = 0;
            int endIdx = 0;

            if (lineStr == string.Empty)
            {
                return null;
            }
            while (true)
            {
                if (lineStr.Substring(startIdx).Trim()[0] == '"') // ,   here"xxxxx"  ,
                {
                    endIdx = startIdx + lineStr.Substring(startIdx + 1).IndexOf("\"") + 1;

                    if (endIdx <= startIdx)
                    {
                        throw new Exception("Data error: " + lineStr);
                    }

                    if (endIdx < lineStr.Length)
                    {
                        if (lineStr.Substring(endIdx).IndexOf(comma) != -1)
                        {
                            endIdx = endIdx + lineStr.Substring(endIdx).IndexOf(comma);
                        }
                        else
                        {
                            endIdx = lineStr.Length;
                        }
                    }
                }
                else
                {
                    endIdx = startIdx + lineStr.Substring(startIdx).IndexOf(comma);
                }

                if (endIdx == -1)
                {
                    endIdx = lineStr.Length;
                }

                string temp = lineStr.Substring(startIdx, endIdx - startIdx);

                retStr.Add(temp);

                startIdx = endIdx + 1;

                if (startIdx >= lineStr.Length)
                {
                    break;
                }
            }

            return retStr.ToArray();
        }

        public static MacroFunc StringParse(DataRow dataRow)
        {
            // Template
            //No,TYPE,COMMAND,DISPLAY NAME,MES CODE,ACTION,LABEL,LABEL COUNT,CASE NG,DELAY,TIME OUT,RETRY,COMPARE,MIN,MAX,OPTION,PAR1,PAR2,EXPR,""
            CMDListCtrl CMDStore = CMDListCtrl.getInstance();
            Globals GL = Globals.getInstance();
            MacroFunc m = new MacroFunc();

            object[] itemsObj = dataRow.ItemArray;

            if (itemsObj.Length != 19)
            {
                return null;
            }
            List<string> items = itemsObj.Select(s => s.ToString().Trim()).ToList();
            try
            {
                //item 0 ignore
                //item 1 TYPE
                m.CMDType = items[1];


                if (items[1] == "" && items[2] == "" && items[3] == "" && items[4] == "")
                {
                    m.cmd = new CMDProperty();
                    m.isBlank = true;
                }
                else
                {
                    //item 2 COMMAND
                    CMDProperty cp = CMDStore.findElement(items[1], items[2]);
                    m.cmd = cp;
                }
                //item 3 DISPLAY NAME
                m.displayName = items[3];

                //item 4 MES CODE
                m.mesCode = items[4];

                // item 5 ACTION
                m.isSkip = (items[5].ToUpper() == "TRUE");

                //item 6 LABEL
                m.label = items[6];

                //item 7 LABEL COUNT
                int iTemplc = 0;
                if (Int32.TryParse(items[7], out iTemplc))
                {
                    m.labelCnt = iTemplc.ToString();
                }
                else
                {
                    m.labelCnt = "";
                }
                //item 8 CASE NG
                //List<string> caseNGList = new List<string>(GL.g_sCaseNgItemList);
                //if (caseNGList.IndexOf(items[8].Substring(0, items[8].IndexOf(":") == -1 ? items[8].Length : items[8].IndexOf(":")).Trim()) >= 0)
                //{
                //    m.caseNG = items[8].Trim();
                //}
                //else
                //{
                //    throw new Exception("Wrong CASE NG value : " + items[8]);
                //}

                //item 9 DELAY
                int iTempdl = 0;
                if (Int32.TryParse(items[9], out iTempdl))
                {
                    m.delayTime = iTempdl.ToString();
                }
                else
                {
                    m.delayTime = "";
                }

                //item 10 TIME OUT
                int iTempto = 0;
                if (Int32.TryParse(items[10], out iTempto))
                {
                    m.timeout = iTempto.ToString();
                }
                else
                {
                    m.timeout = "";
                }

                //item 11 RETRY
                int iTemprt = 0;
                if (Int32.TryParse(items[11], out iTemprt))
                {
                    m.retry = iTemprt.ToString();
                }
                else
                {
                    m.retry = "";
                }

                //item 12 compare
                m.Compare = items[12];

                //item 13 MIN
                m.min = items[13];

                //item 14 MIN
                m.max = items[14];


                /*
                //item 13 MIN
                double dTempmin = 0;
                if (Double.TryParse(items[13], out dTempmin))
                {
                    m.min = dTempmin.ToString();
                }
                else
                {
                    if (items[13] == "\"\"")
                    {
                        m.min = "";
                    }
                    else
                        throw new Exception("Wrong MIN value : " + items[13]);
                }

                //item 14 MAX
                double dTempmax = 0;
                if (Double.TryParse(items[14], out dTempmax))
                {
                    m.max = dTempmax.ToString();
                }
                else
                {
                    if (items[14] == "\"\"")
                    {
                        m.max = "";
                    }
                    else
                        throw new Exception("Wrong MAX value : " + items[14]);
                }
                */


                //item 15 OPTION
                m.option = items[15];

                //if(m.option != string.Empty)
                //{
                //    string filePar = GL.g_sParserStrDir + m.option + ".par";
                //    if(!File.Exists(filePar))
                //    {
                //        throw new Exception("Parser string define file: " + items[15] + " does not found!");
                //    }
                //    else
                //    {
                //        m.option = File.ReadAllText(filePar);
                //    }
                //}

                //item 16 PAR1
                m.par1 = items[16];

                //item 17 PAR2
                m.par2 = items[17];

                //item 18 EXPR
                m.expr = items[18];

                return m;
            }
            catch (Exception ex)
            {
                Log.LogWrite(Globals.LogLv.Debug, ex.Message);
                // NOTE: Log

                return null;
            }

        }
    }

}
