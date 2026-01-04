//using Serilog;
//using Serilog.Core;
//using Serilog.Events;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using VTP_Induction.Device;

namespace VTP_Induction.Common
{
    public delegate void AddLogEvent(LogRecord record);

    public static class Log
    {
        private static ILogger _LoggerSystem;
        private static ILogger _LoggerGmes;
        private static LoggingLevelSwitch levelSwitchSystem;
        private static string m_sFilePathSystem = @"D:\VIETTELPOST\Log\Test\SystemLog_";
        private static bool m_bInit = true;
        public static Globals.LogLv Loglv
        {
            get
            {
                return (Globals.LogLv)levelSwitchSystem.MinimumLevel;
            }
            set
            {
                levelSwitchSystem.MinimumLevel = (LogEventLevel)value;
            }
        }
        public static bool bInit
        {
            get
            {
                return m_bInit;
            }

            set
            {
                m_bInit = value;
            }

        }
        public static string FilePathSystem
        {
            get
            {
                return m_sFilePathSystem + ".log";
            }

            set
            {
                m_sFilePathSystem = value;
            }
        }

        private static string m_sFilePathGmes = @"D:\VIETTELPOST\Log\Test\GmesLog_";
        public static string FilePathGmes
        {
            get
            {
                return m_sFilePathGmes + ".log";
            }

            set
            {
                m_sFilePathGmes = value;
            }
        }

        private static Globals GLb = Globals.getInstance();

        //        public static void LoadData()
        //        {
        //            levelSwitchSystem = new LoggingLevelSwitch();
        //            levelSwitchSystem.MinimumLevel = Serilog.Events.LogEventLevel.Information;

        //            DateTime dt = DateTime.Now;
        //            string sLogTemp = string.Empty;

        //            m_sFilePathSystem = Globals.g_sSystemLog +
        //            dt.Year.ToString("0000") +
        //            "\\" + dt.Month.ToString("00") +
        //            "\\" + dt.Day.ToString("00") +
        //            "\\SystemLog_";

        //            _LoggerSystem = new LoggerConfiguration()
        //                    .MinimumLevel.ControlledBy(levelSwitchSystem)
        //            #if DEBUG
        //                .WriteTo.Console(outputTemplate: "{Timestamp:yyMMdd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        //            #endif
        //            .WriteTo.File(path: FilePathSystem,
        //                      rollingInterval: RollingInterval.Day,
        //                      rollOnFileSizeLimit: true,
        //                      fileSizeLimitBytes: 1024 * 1024 * 5,
        //                      outputTemplate: "{Timestamp:yyMMdd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        //                    .CreateLogger();

        //            #region GMES
        //            levelSwitchGmes = new LoggingLevelSwitch();
        //            levelSwitchGmes.MinimumLevel = Serilog.Events.LogEventLevel.Information;


        //            dt = DateTime.Now;

        ////            FilePathGmes = Globals.g_sGmesLogDir +
        ////            dt.Year.ToString("0000") +
        ////            "\\" + dt.Month.ToString("00") +
        ////            "\\" + dt.Day.ToString("00") +
        ////            "\\GmesLog_";

        ////            _LoggerGmes = new LoggerConfiguration()
        ////                    .MinimumLevel.ControlledBy(levelSwitchGmes)
        ////#if DEBUG
        ////.WriteTo.Console(outputTemplate: "{Timestamp:yyMMdd HH:mm:ss.fff [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        ////#endif
        ////.WriteTo.File(path: FilePathGmes,
        ////                      rollingInterval: RollingInterval.Day,
        ////                      rollOnFileSizeLimit: true,
        ////                      fileSizeLimitBytes: 1024 * 1024 * 5,
        ////                      outputTemplate: "{Timestamp:yyMMdd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        ////                    .CreateLogger();
        ////            #endregion
        //        }
        public static void LoadData()
        {
            levelSwitchSystem = new LoggingLevelSwitch();
            levelSwitchSystem.MinimumLevel = Serilog.Events.LogEventLevel.Information;

            DateTime dt = DateTime.Now;

            m_sFilePathSystem = Globals.g_sSystemLog +
            dt.Year.ToString("0000") +
            "\\" + dt.Month.ToString("00") +
            "\\" + dt.Day.ToString("00") +
            "\\SystemLog_";

            _LoggerSystem = new LoggerConfiguration()
                    .MinimumLevel.ControlledBy(levelSwitchSystem)
#if DEBUG
.WriteTo.Console(outputTemplate: "{Timestamp:yyMMdd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
#endif
.WriteTo.File(path: FilePathSystem,
                      rollingInterval: RollingInterval.Day,
                      rollOnFileSizeLimit: true,
                      fileSizeLimitBytes: 1024 * 1024 * 10,
                      outputTemplate: "{Timestamp:yyMMdd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();

            //            #region GMES
            //            levelSwitchGmes = new LoggingLevelSwitch();
            //            levelSwitchGmes.MinimumLevel = Serilog.Events.LogEventLevel.Information;


            //            dt = DateTime.Now;

            //            FilePathGmes = Globals.g_sGmesLogDir +
            //                            dt.Year.ToString("0000") +
            //            "\\" + dt.Month.ToString("00") +
            //            "\\" + dt.Day.ToString("00") +
            //            "\\GmesLog_";

            //            _LoggerGmes = new LoggerConfiguration()
            //                    .MinimumLevel.ControlledBy(levelSwitchGmes)
            //#if DEBUG
            //.WriteTo.Console(outputTemplate: "{Timestamp:yyMMdd HH:mm:ss.fff [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            //#endif
            //.WriteTo.File(path: FilePathGmes,
            //                      rollingInterval: RollingInterval.Day,
            //                      rollOnFileSizeLimit: true,
            //                      fileSizeLimitBytes: 1024 * 1024 * 5,
            //                      outputTemplate: "{Timestamp:yyMMdd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            //                    .CreateLogger();
            //            #endregion
        }
        public static AddLogEvent LogEvent;


        public static void LogWrite(Globals.LogLv level, string str, Globals.LogType type = Globals.LogType.SYSTEM)
        {
            switch (type)
            {
                case Globals.LogType.SYSTEM:
                    {
                        _LoggerSystem.Write((LogEventLevel)level, str);
                        if (m_bInit)
                        {
                            if ((int)level >= (int)Loglv)
                            {
                                LogRecord r = new LogRecord();
                                r.LogLevel = level;
                                r.logData = str.Trim();
                                r.time = DateTime.Now.ToString("yyMMdd HH:mm:ss.fff");

                                if (LogEvent != null)
                                {
                                    LogEvent(r);
                                }
                            }
                        }
                    }
                    break;
                case Globals.LogType.GMES:
                    _LoggerGmes.Write((LogEventLevel)level, str);
                    break;
                default:
                    break;
            }
        }
        public static void LogWrite(Globals.LogLv level, double str, Globals.LogType type = Globals.LogType.SYSTEM)
        {
            switch (type)
            {
                case Globals.LogType.SYSTEM:
                    _LoggerSystem.Write((LogEventLevel)level, str.ToString());
                    break;
                case Globals.LogType.GMES:
                    _LoggerGmes.Write((LogEventLevel)level, str.ToString());
                    break;
                default:
                    break;
            }
        }

        public static void LogWrite(Globals.LogLv level, byte[] bytes, Globals.LogType type = Globals.LogType.SYSTEM)
        {
            string str = Util.ByteArrayToStrHexa(bytes);
            switch (type)
            {
                case Globals.LogType.SYSTEM:
                    _LoggerSystem.Write((LogEventLevel)level, str.ToString());
                    break;
                case Globals.LogType.GMES:
                    _LoggerGmes.Write((LogEventLevel)level, str.ToString());
                    break;
                default:
                    break;
            }
        }
        public static void LogWrite(Globals.LogLv level, string str, bool b, Globals.LogType type = Globals.LogType.SYSTEM)
        {
            switch (type)
            {
                case Globals.LogType.SYSTEM:
                    {
                        str += (b ? " [TRUE]" : " [FALSE]");
                        _LoggerSystem.Write((LogEventLevel)level, str);

                        if (m_bInit)
                        {
                            if ((int)level >= (int)Loglv)
                            {
                                LogRecord r = new LogRecord();
                                r.LogLevel = level;
                                r.logData = str.Trim();
                                r.time = DateTime.Now.ToString("yyMMdd HH:mm:ss.fff");

                                if (LogEvent != null)
                                {
                                    LogEvent(r);
                                }
                            }
                        }
                    }
                    break;
                case Globals.LogType.GMES:
                    _LoggerGmes.Write((LogEventLevel)level, str + (b ? " [TRUE]" : " [FALSE]"));
                    break;
                default:
                    break;
            }
        }

        public static void LogWrite(int level, string str, Globals.LogType type = Globals.LogType.SYSTEM)
        {
            switch (type)
            {
                case Globals.LogType.SYSTEM:
                    _LoggerSystem.Write((LogEventLevel)level, str);
                    break;
                case Globals.LogType.GMES:
                    _LoggerGmes.Write((LogEventLevel)level, str);
                    break;
                default:
                    break;
            }
            // Add record to list

        }
        public static void LogWrite(Globals.LogLv level, Exception exception, Globals.LogType type = Globals.LogType.SYSTEM)
        {
            switch (type)
            {
                case Globals.LogType.SYSTEM:
                    {
                        _LoggerSystem.Write((LogEventLevel)level, exception, "");
                        if (m_bInit)
                        {
                            if ((int)level >= (int)Loglv)
                            {
                                LogRecord r = new LogRecord();
                                r.LogLevel = level;
                                r.logData = exception.ToString();
                                r.time = DateTime.Now.ToString("yyMMdd HH:mm:ss.fff");

                                if (LogEvent != null)
                                {
                                    LogEvent(r);
                                }
                            }
                        }
                    }
                    break;
                case Globals.LogType.GMES:
                    _LoggerGmes.Write((LogEventLevel)level, exception, "");
                    break;
                default:
                    break;
            }// Add record to list
        }
    }
    public class LogRecord
    {
        public Globals.LogLv LogLevel = Globals.LogLv.Information;
        public string logData;

        public string time;

        public override string ToString()
        {
            return ("[" + time + "] " + logData + "\r\n");
        }
    }
}
