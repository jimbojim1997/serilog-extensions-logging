﻿using System;
using Microsoft.Framework.Logging;
using ILogger = Microsoft.Framework.Logging.ILogger;
using Serilog;

namespace Sample
{
    public class Program
    {
        private readonly ILogger _logger;

        public Program()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
#if DNXCORE50
                .WriteTo.TextWriter(Console.Out)
#else
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .WriteTo.LiterateConsole()
#endif
                .CreateLogger();

            _logger = new LoggerFactory()
                .AddSerilog()
                .CreateLogger(typeof(Program).FullName);
        }

        public void Main(string[] args)
        {
            _logger.LogInformation("Starting");

            var startTime = DateTimeOffset.UtcNow;
            _logger.LogInformation(1, "Started at '{StartTime}' and 0x{Hello:X} is hex of 42", startTime, 42);

            try
            {
                throw new Exception("Boom");
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Unexpected critical error starting application", ex);
                _logger.Log(LogLevel.Critical, 0, "Unexpected critical error", ex, null);
                // This write should not log anything
                _logger.Log(LogLevel.Critical, 0, null, null, null);
                _logger.LogError("Unexpected error", ex);
                _logger.LogWarning("Unexpected warning", ex);
            }

            using (_logger.BeginScope("Main"))
            {
                Console.WriteLine("Hello World");

                _logger.LogInformation("Waiting for user input");
                Console.ReadLine();
            }

            var endTime = DateTimeOffset.UtcNow;
            _logger.LogInformation(2, "Stopping at '{StopTime}'", endTime);

            _logger.LogInformation("Stopping");

            _logger.LogInformation(Environment.NewLine);
            _logger.LogInformation("{Result,-10}{StartTime,15}{EndTime,15}{Duration,15}", "RESULT", "START TIME", "END TIME", "DURATION(ms)");
            _logger.LogInformation("{Result,-10}{StartTime,15}{EndTime,15}{Duration,15}", "------", "----- ----", "--- ----", "------------");
            _logger.LogInformation("{Result,-10}{StartTime,15:mm:s tt}{EndTime,15:mm:s tt}{Duration,15}", "SUCCESS", startTime, endTime, (endTime - startTime).TotalMilliseconds);
        }
    }
}
