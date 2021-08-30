using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wolf.Utility.Core.Startup.Assist
{
    /// <summary>
    /// https://code-maze.com/net-core-web-development-part3/
    /// </summary>
    public class LoggerManager : ILoggerManager
    {
        private NLog.ILogger logger = LogManager.GetCurrentClassLogger();
        private string Caller = "";

        public void SetCaller(string caller) 
        {
            Caller = caller;
            logger = LogManager.GetLogger(Caller);

            LogInfo($"Set Logger to be from {Caller}");
        }

        public void LogDebug(string message)
        {
            logger.Debug(message);
        }

        public void LogError(string message)
        {
            logger.Error(message);
        }

        public void LogInfo(string message)
        {
            logger.Info(message);
        }

        public void LogWarn(string message)
        {
            logger.Warn(message);
        }
    }
}
