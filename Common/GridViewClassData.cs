namespace VTP_Induction
{

    public class DataAxisConfig
    {
        /*00*/
        public string AxisName { get; set; }
        /*01*/
        public int AxisRow_No { get; set; }
        /*02*/
        public int AxisRow_Present { get; set; }
        /*03*/
        public int AxisRow_Type { get; set; }
        /*04*/
        public int AxisRow_Motor_Type { get; set; }
        /*05*/
        public int AxisRow_Methode_EncoderInput { get; set; }
        /*06*/
        public int AxisRow_Methode_PulseOut { get; set; }
        /*07*/
        public double AxisRow_Motor_Res { get; set; }
        /*08*/
        public int AxisRow_SensLevel_Pend { get; set; }
        /*09*/
        public int AxisRow_SensLevel_Mend { get; set; }
        /*10*/
        public int AxisRow_SensLevel_Pslow { get; set; }
        /*11*/
        public int AxisRow_SensLevel_Mslow { get; set; }
        /*12*/
        public int AxisRow_SensLevel_Alarm { get; set; }
        /*13*/
        public int AxisRow_SensLevel_Inposition { get; set; }
        /*14*/
        public int AxisRow_SensLevel_ServoOn { get; set; }
        /*15*/
        public int AxisRow_SensLevel_Origin { get; set; }
        /*16*/
        public double AxisRow_Storke_Min { get; set; }
        /*17*/
        public double AxisRow_Storke_Max { get; set; }
        /*18*/
        public double AxisRow_CurrentPosSet { get; set; }
        /*19*/
        public int AxisRow_Motion_DriveOper { get; set; }
        /*20*/
        public double AxisRow_Motion_Vel { get; set; }
        /*21*/
        public double AxisRow_Motion_Acc { get; set; }
        /*22*/
        public double AxisRow_Motion_Dec { get; set; }
        /*23*/
        public int AxisRow_Vel_DriveOper { get; set; }
        /*24*/
        public int AxisRow_Vel_StopMethode { get; set; }
        /*25*/
        public double AxisRow_Vel_Vel { get; set; }
        /*26*/
        public double AxisRow_Vel_Acc { get; set; }
        /*27*/
        public double AxisRow_Vel_Dec { get; set; }
        /*28*/
        public int AxisRow_Home_Order { get; set; }
        /*29*/
        public double AxisRow_Home_MovePos { get; set; }
        /*30*/
        public double AxisRow_Home_ResetPos { get; set; }
        /*31*/
        public int AxisRow_Home_Profile { get; set; }
        /*32*/
        public double AxisRow_Home_Vel { get; set; }
        /*33*/
        public double AxisRow_Home_Acc { get; set; }
        /*34*/
        public int AxisRow_Home_Dir { get; set; }
        /*35*/
        public double AxisRow_SW_Limit_P { get; set; }
        /*36*/
        public double AxisRow_SW_Limit_M { get; set; }
        /*37*/
        public double AxisRow_PanelAttatchPos { get; set; }
        /*38*/
        public double AxisRow_BaseSetPos { get; set; }
        /*39*/
        public double AxisRow_DetectorAttatchPos { get; set; }
        /*40*/
        public double AxisRow_UserPos1 { get; set; }
        /*41*/
        public double AxisRow_UserPos2 { get; set; }
        /*42*/
        public int AxisRow_ID { get; set; }

        public DataAxisConfig()
        {
            AxisName = " Unknown";
            AxisRow_No = 0;
            AxisRow_Present = 0;
            AxisRow_Type = 0;
            AxisRow_Motor_Type = 0;
            AxisRow_Methode_EncoderInput = 0;
            AxisRow_Methode_PulseOut = 0;
            AxisRow_Motor_Res = 0;
            AxisRow_SensLevel_Pend = 0;
            AxisRow_SensLevel_Mend = 0;
            AxisRow_SensLevel_Pslow = 0;
            AxisRow_SensLevel_Mslow = 0;
            AxisRow_SensLevel_Alarm = 0;
            AxisRow_SensLevel_Inposition = 0;
            AxisRow_SensLevel_ServoOn = 0;
            AxisRow_SensLevel_Origin = 0;
            AxisRow_Storke_Min = 0;
            AxisRow_Storke_Max = 0;
            AxisRow_CurrentPosSet = 0;
            AxisRow_Motion_DriveOper = 0;
            AxisRow_Motion_Vel = 0;
            AxisRow_Motion_Acc = 0;
            AxisRow_Motion_Dec = 0;
            AxisRow_Vel_DriveOper = 0;
            AxisRow_Vel_StopMethode = 0;
            AxisRow_Vel_Vel = 0;
            AxisRow_Vel_Acc = 0;
            AxisRow_Vel_Dec = 0;
            AxisRow_Home_Order = 0;
            AxisRow_Home_MovePos = 0;
            AxisRow_Home_ResetPos = 0;
            AxisRow_Home_Profile = 0;
            AxisRow_Home_Vel = 0;
            AxisRow_Home_Acc = 0;
            AxisRow_Home_Dir = 0;
            AxisRow_SW_Limit_P = 0;
            AxisRow_SW_Limit_M = 0;
            AxisRow_PanelAttatchPos = 0;
            AxisRow_BaseSetPos = 0;
            AxisRow_DetectorAttatchPos = 0;
            AxisRow_UserPos1 = 0;
            AxisRow_UserPos2 = 0;
        }

        public DataAxisConfig(string sAxisName, int nID)
        {
            AxisName = sAxisName;
            AxisRow_No = 0;
            AxisRow_Present = 0;
            AxisRow_Type = 0;
            AxisRow_Motor_Type = 0;
            AxisRow_Methode_EncoderInput = 0;
            AxisRow_Methode_PulseOut = 0;
            AxisRow_Motor_Res = 0;
            AxisRow_SensLevel_Pend = 0;
            AxisRow_SensLevel_Mend = 0;
            AxisRow_SensLevel_Pslow = 0;
            AxisRow_SensLevel_Mslow = 0;
            AxisRow_SensLevel_Alarm = 0;
            AxisRow_SensLevel_Inposition = 0;
            AxisRow_SensLevel_ServoOn = 0;
            AxisRow_SensLevel_Origin = 0;
            AxisRow_Storke_Min = 0;
            AxisRow_Storke_Max = 0;
            AxisRow_CurrentPosSet = 0;
            AxisRow_Motion_DriveOper = 0;
            AxisRow_Motion_Vel = 0;
            AxisRow_Motion_Acc = 0;
            AxisRow_Motion_Dec = 0;
            AxisRow_Vel_DriveOper = 0;
            AxisRow_Vel_StopMethode = 0;
            AxisRow_Vel_Vel = 0;
            AxisRow_Vel_Acc = 0;
            AxisRow_Vel_Dec = 0;
            AxisRow_Home_Order = 0;
            AxisRow_Home_MovePos = 0;
            AxisRow_Home_ResetPos = 0;
            AxisRow_Home_Profile = 0;
            AxisRow_Home_Vel = 0;
            AxisRow_Home_Acc = 0;
            AxisRow_Home_Dir = 0;
            AxisRow_SW_Limit_P = 0;
            AxisRow_SW_Limit_M = 0;
            AxisRow_PanelAttatchPos = 0;
            AxisRow_BaseSetPos = 0;
            AxisRow_DetectorAttatchPos = 0;
            AxisRow_UserPos1 = 0;
            AxisRow_UserPos2 = 0;
            AxisRow_ID = nID;
        }

    }

    public class DataJigConfig
    {
        /*00*/
        public string JigRow_Item { get; set; }
        /*01*/
        public string JigRow_AxisToPlane { get; set; }
        /*02*/
        public double JigRow_OrgDist { get; set; }
        /*03*/
        public string JigRow_JigTopRightpos { get; set; }
        /*04*/
        public string JigRow_JigBotLeftpos { get; set; }
        /*05*/
        public string JigRow_CenterPos { get; set; }
        /*06*/
        public string JigRow_CamSafeTopRight { get; set; }
        /*07*/
        public string JigRow_CamSafeBotLeft { get; set; }
        /*08*/
        public double JigRow_CamSafeZ { get; set; }
        /*09*/
        public bool JigRow_CrashGuardUse { get; set; }
        /*10*/
        public double JigRow_CrashMinDist { get; set; }
        /*11*/
        public double JigRow_MeasMinDist { get; set; }
        /*12*/
        public double JigRow_SafetyZAxisPos { get; set; }
        /*13*/
        public string JigRow_AziCenter { get; set; }


        public DataJigConfig()
        {
            JigRow_Item = "Value";
            JigRow_AxisToPlane = "0,0";
            JigRow_OrgDist = 0;
            JigRow_JigTopRightpos = "0,0";
            JigRow_JigBotLeftpos = "0,0";
            JigRow_CenterPos = "0,0,0";
            JigRow_CamSafeTopRight = "0,0";
            JigRow_CamSafeBotLeft = "0,0";
            JigRow_CamSafeZ = 0;
            JigRow_CrashGuardUse = false;
            JigRow_CrashMinDist = 0;
            JigRow_MeasMinDist = 0;
            JigRow_SafetyZAxisPos = 0;
            JigRow_AziCenter = "0,0";
        }

    }

    public class DataIOConfig  // Grid view
    {
        /*00*/
        public string IOCol_Item { get; set; }
        /*01*/
        public int IOCol_Axis { get; set; }
        /*02*/
        public int IOCol_Bit { get; set; }
        /*03*/
        public int IOCol_Value { get; set; }


        public DataIOConfig()
        {
            IOCol_Item = "Unknown";
            IOCol_Axis = 0;
            IOCol_Bit = 0;
            IOCol_Value = 0;
        }

    }
    public class DataDetectorPosConfig  // Gridview
    {
        /*00*/
        public string IOCol_Item { get; set; }
        /*01*/
        public string StandardPos_X_Y_Z { get; set; }
        /*02*/
        public string CamPos_d1x_d1y_d1z { get; set; }
        /*03*/
        public string CamPos_d2x_d2y_d2z { get; set; }

        public DataDetectorPosConfig()
        {
            IOCol_Item = "Unknown";
            StandardPos_X_Y_Z = "0,0,0";
            CamPos_d1x_d1y_d1z = "0,0,0";
            CamPos_d2x_d2y_d2z = "0,0,0";
        }
    }

    public class DataModelInfor  // Grid view
    {
        /*00*/
        public int NoInfor { get; set; }
        /*01*/
        public string CategoryInfor { get; set; }
        /*02*/
        public string InforInfor { get; set; }


        public DataModelInfor()
        {
            NoInfor = -1;
            CategoryInfor = "";
            InforInfor = "";

        }

    }



}
