﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cobalt
{
    public class Program
    {
        private static readonly string ARG_INPUT_FILE = "inputFile";
        private static readonly string ARG_TARGET = "target";

        /// <summary>
        /// The compiler entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            // Set up configuration from command line arguments
            IConfiguration configuration = new ConfigurationBuilder().AddCommandLine(args, new Dictionary<string, string>()
            {
                { "-i", ARG_INPUT_FILE },
                { "-t", ARG_TARGET },
            }).Build();

            // Check for verbose mode
            bool verbose = args.Contains("-v");

            // Set up logging providers
            ILoggerFactory loggerFactory = LoggingBootstrapper.CreateLoggerFactory(verbose);

            // Logger for local logging
            ILogger logger = loggerFactory.CreateLogger<Program>();

            // Display help if requested
            if (args.Contains("-h"))
            {
                PrintHelpAndExit(logger);
            }

            // Parse command line arguments
            string inputFile = configuration.GetValue<string>(ARG_INPUT_FILE);
            string target = configuration.GetValue<string>(ARG_TARGET);

            // Validate required parameters
            if (string.IsNullOrWhiteSpace(inputFile) || string.IsNullOrWhiteSpace(target))
            {
                logger.LogCritical("One or more required parameter is missing!");
                PrintHelpAndExit(logger);
            }

            // Display information in verbose mode
            logger.LogDebug("Compiler parameters:");
            logger.LogDebug($" - Input file: {inputFile}");
            logger.LogDebug($" - Target: {target}");

            // TODO: read code from file
            // TODO: tokenize code
            // TODO: parse tokens to AST
            // TODO: type check
            // TODO: optimize
            // TODO: initialize a backend
            // TODO: generate target code
            // TODO: write target code to file
            
            Console.ReadLine();
        }

        /// <summary>
        /// Prints a help message and waits for the user to press any key to terminate.
        /// </summary>
        /// <param name="logger">The logger to use for printing the help message.</param>
        private static void PrintHelpAndExit(ILogger logger)
        {
            logger.LogInformation("Welcome to the Cobalt compiler version 0.1.0 for Cobalt 0.1.0!");
            logger.LogInformation(" - Usage: cobaltc -i <inputFile> -t <targetPlatform> [-v]");
            logger.LogInformation(" - Available target platforms: <none>");
            logger.LogInformation("Press any key to terminate...");
            Console.Read();
            Environment.Exit(0);
        }
    }
}
