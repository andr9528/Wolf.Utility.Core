using System;
using System.Collections.Generic;
using System.Text;

namespace Wolf.Utility.Core.Logging
{
    /// <summary>
    /// https://code-maze.com/net-core-web-development-part3/
    /// </summary>
    public interface ILoggerManager
    {
        void LogInfo(string message);
        void LogWarn(string message);
        void LogDebug(string message);
        void LogError(string message);
        void SetCaller(string caller);
    }
}
