
using System;
using System.Collections.Generic;
using System.IO;
using VTP_Induction.Device;

namespace VTP_Induction
{
    [Flags]
    public enum COMPARE_TYPE // OK situation is active
    {
        //MIN_LOWER_VALUE_LOWER_MAX,               // Min <  Value < Max
        //MIN_LOWEROREQUAL_VALUE_LOWER_MAX,        // Min <= Value < Max
        //MIN_LOWER_VALUE_LOWEROREQUAL_MAX,        // Min <  Value <= Max
        MIN_LOWEROREQUAL_VALUE_LOWEROREQUAL_MAX, // Min <= Value <= Max
        //MIN_LOWER_VALUE,                         // Min <  Value
        //MIN_LOWEROREQUAL_VALUE,                  // Min <= Value
        //MAX_LARGER_VALUE,                        // Max >  Value
        //MAX_LARGEROREQUAL_VALUE,                 // Max >= Value
        EQUAL,                                   // Min ==
        NOTEQUAL,                                // Min !=

        //INDEX_OF,

        //NONE// none compare mode
    }

    public class CMDListCtrl
    {
        public static CMDListCtrl instance;

        public List<string> CompareTypeStrList = new List<string>(new string[]{
                                             
                                            //"MIN ≤ ■ ≤ MAX",  // UTF-8
                                            // "MIN == ■",
                                            // "MIN != ■",
                                            "MIN <= x <= MAX",
                                            "MIN=MAX == x",
                                            "MIN=MAX != x",

                                         });

        public static CMDListCtrl getInstance()
        {
            if (instance == null)
            {
                instance = new CMDListCtrl();
            }

            return instance;
        }

        public const string GROUP_DECLARE = "#";
        public const string CMD_FILE_EXT = ".clist";

        public List<CMDGroup> CMDStore = new List<CMDGroup>();
        public CMDListCtrl()
        {
            try
            {
                ScanCMDStore();
            }
            catch //(Exception ex)
            {
                //  Log.LogWrite(Globals.LogLv.Debug, ex.Message);
            }
        }

        public void ScanCMDStore()
        {
            string[] files = Directory.GetFiles(Globals.g_sCmdListDir);

            foreach (string filePath in files)
            {
                if (Path.GetExtension(filePath) == CMD_FILE_EXT)
                {
                    // Scan file to get CMD Properties
                    CMDGroup MainGroup = new CMDGroup();
                    MainGroup.GroupName = Path.GetFileNameWithoutExtension(filePath);

                    //Add subgroup
                    string[] lines = System.IO.File.ReadAllLines(filePath);

                    CMDGroup subGroup = null;

                    for (int i = 0; i < lines.Length; i++)
                    {
                        string curLine = lines[i];

                        // check sub-group declare
                        if (curLine.Trim().IndexOf(GROUP_DECLARE) == 0) // the first character is '#'
                        {
                            if (subGroup != null)//Add current subgroup to group
                            {
                                MainGroup.groups.Add(subGroup.Clone());
                            }
                            //Create new Sub-Group
                            subGroup = new CMDGroup();
                            subGroup.GroupName = curLine.Trim().Substring(1); // ignore '#'
                        }
                        else
                        {
                            //Add cmd properties to subGroup
                            string[] strs = curLine.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries);

                            if (strs.Length >= 2)
                            {
                                CMDProperty prop = new CMDProperty();
                                prop.name = strs[0].Trim();
                                prop.mainData = strs[1].Trim();

                                if (strs.Length >= 3 && strs[2].Trim() != string.Empty)
                                {
                                    //COMPARE_TYPE cmpTypeValue = COMPARE_TYPE.NONE;

                                    //Enum.TryParse(strs[2], true, out cmpTypeValue);
                                    //if (Enum.IsDefined(typeof(COMPARE_TYPE), cmpTypeValue) && cmpTypeValue.ToString() == strs[2].ToUpper())
                                    //{
                                    //    prop.cmpType = cmpTypeValue;
                                    //}
                                    //else
                                    //{
                                    //    throw new Exception(strs[2] + " is not defined!");
                                    //}


                                    //if(strs.Length >= 4 && strs[3].Trim() != string.Empty)
                                    //{
                                    //    if(File.Exists(GLb.g_sParserStrDir + strs[3].Trim() + ".par"))
                                    //    {
                                    //        prop.parserString = strs[3];
                                    //    }
                                    //    else
                                    //    {
                                    //        throw new Exception("Can not find parser string file: " + strs[3]);
                                    //    }
                                    //}
                                }

                                subGroup.CMDList.Add(prop);
                            }

                        }
                        if (i == lines.Length - 1)// last line
                        {
                            if (subGroup != null)//Add current subgroup to group
                            {
                                MainGroup.groups.Add(subGroup.Clone());
                            }
                        }
                    }

                    CMDStore.Add(MainGroup);
                }
            }
        }

        public List<string> ListOfType()
        {
            List<string> listTypes = new List<string>();

            foreach (CMDGroup g in CMDStore)
            {
                listTypes.Add(g.ToString());
            }

            return listTypes;
        }

        public bool isTrueType(string type)
        {
            if (ListOfType().IndexOf(type.Trim()) >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public CMDProperty findElement(string type, string CMDName)
        {
            //item 2 COMMAND
            int idx = ListOfType().IndexOf(type.Trim());

            if (idx < 0)
            {
                throw new Exception("Wrong param: TYPE: " + type + ", CMD: " + CMDName);
            }

            foreach (CMDGroup mainGroup in CMDStore[idx].groups)
            {
                foreach (CMDProperty cp in mainGroup.CMDList)
                {
                    if (cp.name == CMDName)
                    {
                        return cp.Clone();
                    }
                }
            }

            throw new Exception("Can not find CMD: " + CMDName + " with TYPE: " + type);
        }

        public List<string> ListOfExPara()
        {
            List<string> listExpara = new List<string>();
            //Add subgroup
            string[] lines = System.IO.File.ReadAllLines(Globals.g_sExPrStrFile);
            for (int i = 0; i < lines.Length; i++)
            {
                listExpara.Add(lines[i].Trim());
            }

            return listExpara;
        }

        public List<string> ListOfOption()
        {
            List<string> listOption = new List<string>();
            //Add subgroup
            string[] lines = System.IO.File.ReadAllLines(Globals.g_sOptionStrFile);
            for (int i = 0; i < lines.Length; i++)
            {
                listOption.Add(lines[i].Trim());
            }

            return listOption;
        }
    }

    public class CMDGroup
    {
        public string GroupName = string.Empty;
        public List<CMDGroup> groups = new List<CMDGroup>();
        public List<CMDProperty> CMDList = new List<CMDProperty>();

        public CMDGroup Clone()
        {
            CMDGroup temp = new CMDGroup();
            temp.GroupName = this.GroupName;

            foreach (CMDGroup cg in this.groups)
            {
                temp.groups.Add(cg.Clone());
            }

            foreach (CMDProperty cp in this.CMDList)
            {
                temp.CMDList.Add(cp.Clone());
            }

            return temp;
        }

        public override string ToString()
        {
            return GroupName;
        }
    }

    public class CMDProperty
    {
        public string name = string.Empty;
        public string mainData = string.Empty;
        public COMPARE_TYPE cmpType = COMPARE_TYPE.MIN_LOWEROREQUAL_VALUE_LOWEROREQUAL_MAX;
        public string parserString = string.Empty;

        public CMDProperty Clone()
        {
            CMDProperty temp = new CMDProperty();

            temp.name = this.name;
            temp.mainData = this.mainData;
            temp.cmpType = this.cmpType;
            temp.parserString = this.parserString;

            return temp;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
