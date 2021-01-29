using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Globalization;
using System.IO;
using Topshelf;

namespace Omniverse.WinService
{
    // 1. How to create a windows service
    // 2. How to create a background job
    // 3. How to call a rest api to get some data

    public class Program
    {
        public static void Main()
        {
            int exitCode;
            var logger = ConfigureLogging();

            try
            {
                var rc = HostFactory.Run(x =>
                {
                    x.Service<BackgroundWorker>(s =>
                    {
                        s.ConstructUsing(name => new BackgroundWorker(logger, new BackendServiceClient("https://jsonplaceholder.typicode.com")));
                        s.WhenStarted(tc => tc.Start());
                        s.WhenStopped(tc => tc.Stop());
                    });
                    x.RunAsLocalSystem();

                    x.SetDescription("Omniverse Service Demo");
                    x.SetDisplayName("Omniverse");
                    x.SetServiceName("Omniverse");
                });

                exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            }
            catch (Exception ex)
            {
                exitCode = 1067;
                logger.Fatal(ex, ex.Message);
                LogManager.Shutdown();
            }

            Environment.ExitCode = exitCode;
        }

        private static ILogger ConfigureLogging()
        {

            var config = new LoggingConfiguration
            {

                // use invariant culture as default
                DefaultCultureInfo = CultureInfo.InvariantCulture
            };

            // create targets
            var basePath = Path.Combine("Logs");

            var fileTarget = new FileTarget("fileTarget")
            {
                FileName = Path.Combine(basePath, "Omniverse.WinService.log"),
                ArchiveFileName = Path.Combine(basePath, "Omniverse.WinService.{##}.log"),
                Layout = @"${date:universalTime=true:format=yyyy-MM-dd HH\:mm\:ss.fff} UTC | ${pad:padding=5:inner=${level:uppercase=true}} | ${var:assemblyInfoVersion} | ${right:length=40:inner=${callsite:CleanNamesOfAsyncContinuations=true}} | ${message}${onexception:inner=${newline}${exception:format=tostring}}",
                ArchiveAboveSize = 10000000, // max 10 MB per file
                ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
                ArchiveDateFormat = "yyyyMMdd",
                MaxArchiveFiles = 60,
                CreateDirs = true
            };

            var consoleTarget = new ColoredConsoleTarget()
            {
                Name = "coloredConsoleTarget",
                Layout = @"${date:format=HH\:mm\:ss.fff} | ${pad:padding=5:inner=${level:uppercase=true}} | ${right:length=30:inner=${callsite:CleanNamesOfAsyncContinuations=true}} | ${message}${onexception:inner=${newline}${exception:format=message}}"
            };

            config.AddTarget(fileTarget);
            config.AddTarget(consoleTarget);

            // define rules
            config.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget);
            config.AddRule(LogLevel.Info, LogLevel.Fatal, consoleTarget);

            // activate the configuration
            LogManager.Configuration = config;

            // create logger
            var logger = LogManager.GetLogger(typeof(Program).FullName);

            return logger;
        }
    }
}
