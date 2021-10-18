using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Wolf.Utility.Core.Logging;

namespace Wolf.Utility.Core.Startup.Modules
{
    public class NLogStartupModule : IStartupModule
    {
        /// <summary>
        /// Creates a Startup module that enables logging, using NLog
        /// </summary>
        /// <param name="console">Adds Console logging if true; Default true;</param>
        /// <param name="file">Adds Logging to file, beside the .exe file; Default true;</param>
        public NLogStartupModule(bool console = true, bool file = true)
        {            
            Console = console;
            File = file;
        }

        public NLogLoggerProvider Provider { get; }
        public bool Console { get; }
        public bool File { get; }

        public void ConfigureApplication(IApplicationBuilder app)
        {
            
        }
        /// <summary>
        /// https://code-maze.com/net-core-web-development-part3/
        /// Used https://stackoverflow.com/questions/61553415/using-nlog-console-logging-together-in-net-core-console-app
        /// </summary>
        /// <param name="services"></param>
        public void SetupServices(IServiceCollection services)
        {
            services.AddScoped<ILoggerManager, LoggerManager>();

            var config = new LoggingConfiguration();
            if (File)
            {
                var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "log.txt" };
                config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            }

            if (Console)
            {
                var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
                config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            }

            // Apply config           
            NLog.LogManager.Configuration = config;

            var logger = LogManager.GetCurrentClassLogger();
            logger.Info($"Completed Setup of NLog");
        }       
        
    }
}
