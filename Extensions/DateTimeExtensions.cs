using System;
using Yash.FluentDataPipelines.Configuration;
using Yash.FluentDataPipelines.Core;

namespace Yash.FluentDataPipelines.Extensions
{
    /// <summary>
    /// Extension methods for DateTime transformation operations on PipelineValue.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Transforms a DateTime PipelineValue using the provided configuration.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a DateTime.</param>
        /// <param name="config">The transformation configuration.</param>
        /// <param name="transformer">Function that performs the transformation.</param>
        /// <returns>A PipelineValue with the transformed DateTime.</returns>
        public static PipelineValue<DateTime> Transform(
            this PipelineValue<DateTime> pipelineValue,
            TransformationConfig config,
            Func<DateTime, TransformationConfig, DateTime> transformer)
        {
            if (pipelineValue == null)
            {
                return new PipelineValue<DateTime>(default(DateTime), false, new[] { new PipelineError("PipelineValue is null", "Transform") });
            }

            if (!pipelineValue.IsValid)
            {
                // Preserve failed state but don't transform
                return pipelineValue;
            }

            try
            {
                var transformedValue = transformer(pipelineValue.Value, config);
                return pipelineValue.WithValue(transformedValue);
            }
            catch (Exception ex)
            {
                return pipelineValue.WithError($"Transformation error: {ex.Message}", "Transform");
            }
        }

        /// <summary>
        /// Adds the specified number of days to the DateTime.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a DateTime.</param>
        /// <param name="days">The number of days to add.</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue with the updated DateTime.</returns>
        public static PipelineValue<DateTime> AddDays(
            this PipelineValue<DateTime> pipelineValue,
            int days,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (value, cfg) => value.AddDays(days));
        }

        /// <summary>
        /// Adds the specified number of days to the DateTime.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a DateTime.</param>
        /// <param name="days">The number of days to add (can be fractional).</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue with the updated DateTime.</returns>
        public static PipelineValue<DateTime> AddDays(
            this PipelineValue<DateTime> pipelineValue,
            double days,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (value, cfg) => value.AddDays(days));
        }

        /// <summary>
        /// Adds the specified number of months to the DateTime.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a DateTime.</param>
        /// <param name="months">The number of months to add.</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue with the updated DateTime.</returns>
        public static PipelineValue<DateTime> AddMonths(
            this PipelineValue<DateTime> pipelineValue,
            int months,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (value, cfg) => value.AddMonths(months));
        }

        /// <summary>
        /// Adds the specified number of years to the DateTime.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a DateTime.</param>
        /// <param name="years">The number of years to add.</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue with the updated DateTime.</returns>
        public static PipelineValue<DateTime> AddYears(
            this PipelineValue<DateTime> pipelineValue,
            int years,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (value, cfg) => value.AddYears(years));
        }

        /// <summary>
        /// Adds the specified TimeSpan to the DateTime.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a DateTime.</param>
        /// <param name="timeSpan">The TimeSpan to add.</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue with the updated DateTime.</returns>
        public static PipelineValue<DateTime> Add(
            this PipelineValue<DateTime> pipelineValue,
            TimeSpan timeSpan,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (value, cfg) => value.Add(timeSpan));
        }
    }
}

