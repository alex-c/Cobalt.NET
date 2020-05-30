using Cobalt.Compiler;
using Cobalt.Target.JavaScript;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cobalt
{
    /// <summary>
    /// The `cobaltc` Cobalt compiler main program.
    /// </summary>
    public class Program
    {
        private static readonly string ARG_INPUT_FILE = "inputFile";
        private static readonly string ARG_OUTPUT_DIR = "outputDir";
        private static readonly string ARG_TARGET_PLATFORM = "targetPlatform";

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
                { "-o", ARG_OUTPUT_DIR },
                { "-t", ARG_TARGET_PLATFORM }
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
            string outputDir = configuration.GetValue<string>(ARG_OUTPUT_DIR);
            string targetPlatform = configuration.GetValue<string>(ARG_TARGET_PLATFORM);

            // Validate required parameters
            if (string.IsNullOrWhiteSpace(inputFile) ||
                string.IsNullOrWhiteSpace(outputDir) ||
                string.IsNullOrWhiteSpace(targetPlatform))
            {
                logger.LogCritical("One or more required parameter is missing!");
                PrintHelpAndExit(logger);
            }

            // Display information in verbose mode
            logger.LogDebug("Compiler parameters:");
            logger.LogDebug($" - Input file: {inputFile}");
            logger.LogDebug($" - Output directory: {outputDir}");
            logger.LogDebug($" - Target platform: {targetPlatform}");

            // Set up compiler backend
            ITargetCodeGenerator backend = null;
            switch (targetPlatform.ToLowerInvariant())
            {
                case "js":
                case "javascript":
                    backend = new JavaScriptCodeGenerator();
                    break;
                default:
                    logger.LogError($"Unknown target platform `{targetPlatform}`.");
                    break;
            }

            if (backend != null)
            {
                try
                {
                    // Set up compiler
                    CobaltCompiler compiler = new CobaltCompiler(loggerFactory, backend);

                    // Read source code from input file
                    string sourceCode = File.ReadAllText(inputFile);

                    // Compile!
                    TargetProgram targetProgram = compiler.Compile(sourceCode);

                    // Write output files
                    if (!Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }
                    foreach (ITargetFile file in targetProgram.GetFiles())
                    {
                        string filePath = Path.Combine(outputDir, file.Name);
                        File.Create(filePath);
                        file.Writer.WriteTargetFile(file, filePath);
                    }
                }
                catch (Exception exception)
                {
                    if (verbose)
                    {
                        logger.LogError($"Compilation failed with {exception.Message}{Environment.NewLine}{exception.StackTrace}");
                    }
                    else
                    {
                        logger.LogError("Compilation failed.", exception);
                    }
                    logger.LogInformation("Press any key to terminate...");
                    Console.Read();
                }
            }

            // TODO: remove this
            Console.Read();
        }

        /// <summary>
        /// Prints a help message and waits for the user to press any key to terminate.
        /// </summary>
        /// <param name="logger">The logger to use for printing the help message.</param>
        private static void PrintHelpAndExit(ILogger logger)
        {
            logger.LogInformation("Welcome to the Cobalt compiler version 0.1.0 for Cobalt 0.1.0!");
            logger.LogInformation(" - Usage: cobaltc -i <inputFile> -o <outputDir> -t <targetPlatform> [-v]");
            logger.LogInformation(" - Available target platforms: <none>");
            logger.LogInformation("Press any key to terminate...");
            Console.Read();
            Environment.Exit(0);
        }
    }
}
