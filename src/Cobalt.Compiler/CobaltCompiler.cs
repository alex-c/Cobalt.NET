using Cobalt.AbstractSyntaxTree.Nodes;
using Cobalt.Compiler.Exceptions;
using Cobalt.Compiler.Lexer;
using Cobalt.Compiler.Parser;
using Cobalt.Compiler.Tokens;
using Cobalt.Optimization;
using Cobalt.SemanticAnalysis;
using Cobalt.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Cobalt.Compiler
{
    /// <summary>
    /// A compiler for the Cobalt programming language.
    /// </summary>
    public class CobaltCompiler
    {
        /// <summary>
        /// A logger instance for logging on the compiler level.
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// The lexer instance used to tokenize input Cobalt code.
        /// </summary>
        private CobaltLexer Lexer { get; }

        /// <summary>
        /// The parser instance used to parse tokenized Cobalt code.
        /// </summary>
        private CobaltParser Parser { get; }

        /// <summary>
        /// The optimizer used to optimize a Cobalt program's AST.
        /// </summary>
        private CobaltOptimizer Optimizer { get; }

        private CobaltSemanticAnalyzer Analyzer { get; }

        private ICompilerBackend TargetCodeGenerator { get; }

        /// <summary>
        /// Sets up the compiler instance with all needed components.
        /// </summary>
        /// <param name="loggerFactory">A logger factory to use to instantiate loggers.</param>
        public CobaltCompiler(ILoggerFactory loggerFactory, ICompilerBackend compilerBackend)
        {
            Logger = loggerFactory.CreateLogger<CobaltCompiler>();
            Lexer = new CobaltLexer(loggerFactory);
            Parser = new CobaltParser(loggerFactory);
            Optimizer = new CobaltOptimizer(loggerFactory);
            Analyzer = new CobaltSemanticAnalyzer(loggerFactory);
            TargetCodeGenerator = compilerBackend;
        }

        /// <summary>
        /// Compiles a Cobalt program.
        /// </summary>
        /// <param name="sourceCode">The input Cobalt code.</param>
        /// <returns>Returns the compiled target code.</returns>
        public string Compile(string sourceCode, bool disableOptimization = false)
        {
            try
            {
                // Lexical analysis
                List<Token> tokens = Lexer.Tokenize(sourceCode);

                // Parsing
                CobaltProgram ast = Parser.Parse(tokens);

                // Semantic analysis
                Analyzer.Analyze(ast);

                // Optimization
                if (!disableOptimization)
                {
                    ast = Optimizer.Optimize(ast);
                }

                // Target code generation
                string targetCode = TargetCodeGenerator.GenerateTargetCode(ast);
                return targetCode;
            }
            catch (CobaltSyntaxError exception)
            {
                Logger.LogError(exception.Message, exception);
                throw;
            }
            catch (CompilerException exception)
            {
                Logger.LogCritical("A compiler error occured! Please report this error at https://github.com/alex-c/Cobalt.NET/issues.", exception);
                throw;
            }
            catch (Exception exception)
            {
                Logger.LogCritical("An unexpected error occured.", exception);
                throw;
            }
        }
    }
}
