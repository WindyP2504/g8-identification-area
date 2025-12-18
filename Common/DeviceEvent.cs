using System.Threading;

namespace VTP_Induction
{
    public enum DEV_EVENT_CMD : int
    {
        // USER CMD
        DEC_MOVE_XYZ_MM,
        DEC_MOVE_XYZ_PERCENT,
        DEC_MOVE_ABS_MM,
        DEC_MOVE_ABS_PERCENT,
        DEC_MOVE_REL,
        DEC_MOVE_RXY,
        DEC_MOVE_TXY,
        DEC_HOME,
        DEC_SERVO_ON_OFF,
        DEC_CAPTURE,


        //PRIVATE
        DEC_MOVE_VEL_ONE_AXIS,
        DEC_MOVE_REL_ONE_AXIS,
        DEC_MOVE_REL_TWO_AXIS,
        DEC_MOVE_ABS_MUL_AXIS,
        DEC_MOVE_ABS_ONE_AXIS,
        DEC_MOVE_ABS_MOTION_POINT,
        DEC_MOVE_POS,
        DEC_VSTOP_AXISES,
        DEC_ESTOP_ALL_AXISES,
        DEC_POS_SETTING,
        DEC_TOWER_LAMP,


        //LCIS MODIFY
        DEC_SEQ_CMD,
        DEC_SEQ_CMD_MANUAL,
        DEC_ERROR,

        // USER CMD ROBOT
        DEC_ROB_IO,
        DEC_MOVE_ABS_ROBOT_POINT,
        DEC_MOVE_REL_ROBOT_POINT,
        DEC_ESTOP_ROBOT,

    }

    public enum DEV_EVENT_TYPE : int
    {
        DET_EFFECT = 0,
        DET_EFFECT_SUB1 = 1,
        DET_EFFECT_SUB2 = 2,
        DET_EFFECT_SUB3 = 3,
        DET_NOT_EFFECT = 4,
    }

    public class DeviceEvent
    {
        public DEV_EVENT_CMD cmd;
        public DEV_EVENT_TYPE type = DEV_EVENT_TYPE.DET_EFFECT;
        public bool IsBreak = false;
        public ManualResetEvent DoneEvent = new System.Threading.ManualResetEvent(false);

        public object retVal;

        public object data0;
        public object data1;
        public object data2;
        public object data3;
        public object data4;
        public object data5;
        public object data6;
        public object data7;

        public DeviceEvent()
        {

        }

        public DeviceEvent(DEV_EVENT_CMD cmd, DEV_EVENT_TYPE type, ref ManualResetEvent doneEvent, object data)
        {
            this.cmd = cmd;
            this.type = type;
            this.data0 = data;
            this.DoneEvent = doneEvent;
        }
    }
}
