using Cobalt.AbstractSyntaxTree.Nodes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Cobalt.Optimization
{
    /// <summary>
    /// Provides Cobalt program AST optimization functionality.
    /// </summary>
    public class CobaltOptimizer
    {
        /// <summary>
        /// Tracks the available optimizations, and whether they are enabled.
        /// </summary>
        private Dictionary<Optimization, bool> Optimizations { get; }

        /// <summary>
        /// A logger for logging inside the optimizer.
        /// </summary>
        private ILogger Logger { get; }
        // TODO: Add debug logging to the Optimizer.

        /// <summary>
        /// Initializes a new instance of the <see cref="CobaltOptimizer"/> class.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        public CobaltOptimizer(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<CobaltOptimizer>();
            Optimizations = new Dictionary<Optimization, bool>();

            // Enable all optimizations by default
            foreach (Optimization optimization in Enum.GetValues(typeof(Optimization)))
            {
                Optimizations.Add(optimization, true);
            }
        }

        /// <summary>
        /// Optimizes the specified program AST using all enabled optimizations.
        /// </summary>
        /// <param name="program">The program to optimize.</param>
        /// <returns>Returns the optimized program</returns>
        public CobaltProgram Optimize(CobaltProgram program)
        {
            if (Optimizations[Optimization.SimplifyNegativeNumbers])
            {
                // TODO: implement
            }
            return program;
        }

        /// <summary>
        /// Enable or disable a specific optimization.
        /// </summary>
        /// <param name="optimization">The optimization to enable or disable.</param>
        /// <param name="enabled">Whether the optimization is to be enabled.</param>
        public void SetOptimizationEnabled(Optimization optimization, bool enabled)
        {
            Optimizations[optimization] = enabled;
        }
    }
}
