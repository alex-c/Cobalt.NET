﻿using Microsoft.Extensions.Logging;
using Serilog;

namespace Cobalt
{
    /// <summary>
    /// A helper class to set up logging.
    /// </summary>
    public static class LoggingBootstrapper
    {
        /// <summary>
        /// Creates a <see cref="ILoggerFactory">logger factory</see>, abstracting away the use of Serilog.
        /// </summary>
        /// <param name="verbose">Sets whether the compiler should log verbosely.</param>
        /// <returns>Returns a ready-to-use logger factory.</returns>
        public static ILoggerFactory CreateLoggerFactory(bool verbose)
        {
            LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext();

            // Set minimum level depending on verbose mode
            if (verbose)
            {
                loggerConfiguration.MinimumLevel.Debug();
            }
            else
            {
                loggerConfiguration.MinimumLevel.Information();
            }

            // Set output template depending on verbose mode
            string outputTemplate = null;
            if (verbose)
            {
                outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}][{SourceContext}] {Message:lj}{NewLine}{Exception}";
            }
            else
            {
                outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
            }

            // Set up Serilog logger
            Log.Logger = loggerConfiguration
                .WriteTo.Console(outputTemplate: outputTemplate)
                .CreateLogger();

            // Return a .NET Core logging factory
            return LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });
        }
    }
}
