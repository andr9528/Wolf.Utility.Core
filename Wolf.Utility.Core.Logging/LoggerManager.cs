using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wolf.Utility.Core.Logging
{
    /// <summary>
    /// https://code-maze.com/net-core-web-development-part3/
    /// </summary>
    public class LoggerManager : ILoggerManager
    {
        private ILogger logger = LogManager.GetCurrentClassLogger();
        private string Caller = "UNSET";

        public void SetCaller(string caller)
        {
            var temp = Caller;
            Caller = caller;
            logger = LogManager.GetLogger(Caller);

            LogInfo($"Changing Logger to be from {Caller}; Was from {temp} Previously.");
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
