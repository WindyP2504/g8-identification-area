using System.Data;

namespace VTP_Induction
{

    public class ResultClass
    {
        public struct resultStruct
        {
            public int result;

            public string ExceptionMsg;

            public DataTable table;

            public string Rmsg;

            public BCRClass.msgstruct[] msgArray;

            public int len;
        }
    }
}