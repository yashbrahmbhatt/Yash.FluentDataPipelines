using System;
using Yash.FluentDataPipelines.Configuration;
using Yash.FluentDataPipelines.Core;

namespace Yash.FluentDataPipelines.Extensions
{
    /// <summary>
    /// Extension methods for numeric transformation operations on PipelineValue.
    /// </summary>
    public static class NumericExtensions
    {
        /// <summary>
        /// Transforms a numeric PipelineValue using the provided configuration.
        /// </summary>
        /// <typeparam name="T">The numeric type.</typeparam>
        /// <param name="pipelineValue">The PipelineValue containing a numeric value.</param>
        /// <param name="config">The transformation configuration.</param>
        /// <param name="transformer">Function that performs the transformation.</param>
        /// <returns>A PipelineValue with the transformed value.</returns>
        public static PipelineValue<T> Transform<T>(
            this PipelineValue<T> pipelineValue,
            TransformationConfig config,
            Func<T, TransformationConfig, T> transformer)
        {
            if (pipelineValue == null)
            {
                return new PipelineValue<T>(default(T), false, new[] { new PipelineError("PipelineValue is null", "Transform") });
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
        /// Multiplies the integer value by the specified factor.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing an integer.</param>
        /// <param name="factor">The factor to multiply by.</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue with the multiplied value.</returns>
        public static PipelineValue<int> Multiply(
            this PipelineValue<int> pipelineValue,
            int factor,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (value, cfg) => value * factor);
        }

        /// <summary>
        /// Multiplies the double value by the specified factor.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a double.</param>
        /// <param name="factor">The factor to multiply by.</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue with the multiplied value.</returns>
        public static PipelineValue<double> Multiply(
            this PipelineValue<double> pipelineValue,
            double factor,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (value, cfg) => value * factor);
        }

        /// <summary>
        /// Multiplies the decimal value by the specified factor.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a decimal.</param>
        /// <param name="factor">The factor to multiply by.</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue with the multiplied value.</returns>
        public static PipelineValue<decimal> Multiply(
            this PipelineValue<decimal> pipelineValue,
            decimal factor,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (value, cfg) => value * factor);
        }

        /// <summary>
        /// Divides the integer value by the specified divisor.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing an integer.</param>
        /// <param name="divisor">The divisor.</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue with the divided value.</returns>
        public static PipelineValue<int> Divide(
            this PipelineValue<int> pipelineValue,
            int divisor,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (value, cfg) => value / divisor);
        }

        /// <summary>
        /// Divides the double value by the specified divisor.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a double.</param>
        /// <param name="divisor">The divisor.</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue with the divided value.</returns>
        public static PipelineValue<double> Divide(
            this PipelineValue<double> pipelineValue,
            double divisor,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (value, cfg) => value / divisor);
        }

        /// <summary>
        /// Divides the decimal value by the specified divisor.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a decimal.</param>
        /// <param name="divisor">The divisor.</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue with the divided value.</returns>
        public static PipelineValue<decimal> Divide(
            this PipelineValue<decimal> pipelineValue,
            decimal divisor,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (value, cfg) => value / divisor);
        }

        /// <summary>
        /// Rounds the double value to the specified number of decimal places.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a double.</param>
        /// <param name="decimals">The number of decimal places.</param>
        /// <param name="config">Optional transformation configuration (rounding mode).</param>
        /// <returns>A PipelineValue with the rounded value.</returns>
        public static PipelineValue<double> Round(
            this PipelineValue<double> pipelineValue,
            int decimals,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (value, cfg) => Math.Round(value, decimals, cfg.RoundingMode));
        }

        /// <summary>
        /// Rounds the decimal value to the specified number of decimal places.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a decimal.</param>
        /// <param name="decimals">The number of decimal places.</param>
        /// <param name="config">Optional transformation configuration (rounding mode).</param>
        /// <returns>A PipelineValue with the rounded value.</returns>
        public static PipelineValue<decimal> Round(
            this PipelineValue<decimal> pipelineValue,
            int decimals,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (value, cfg) => Math.Round(value, decimals, config.RoundingMode));
        }
    }
}

