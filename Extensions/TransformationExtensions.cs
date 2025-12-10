using System;
using Yash.FluentDataPipelines.Configuration;
using Yash.FluentDataPipelines.Core;

namespace Yash.FluentDataPipelines.Extensions
{
    /// <summary>
    /// Extension methods for string transformation operations on PipelineValue.
    /// </summary>
    public static class TransformationExtensions
    {
        /// <summary>
        /// Transforms a string PipelineValue using the provided configuration.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a string.</param>
        /// <param name="config">The transformation configuration.</param>
        /// <param name="transformer">Function that performs the transformation.</param>
        /// <returns>A PipelineValue with the transformed string.</returns>
        public static PipelineValue<string> Transform(
            this PipelineValue<string> pipelineValue,
            TransformationConfig config,
            Func<string, TransformationConfig, string> transformer)
        {
            if (pipelineValue == null)
            {
                return new PipelineValue<string>(default(string), false, new[] { new PipelineError("PipelineValue is null", "Transform") });
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
        /// Trims whitespace from the string.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a string.</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue with the trimmed string.</returns>
        public static PipelineValue<string> Trim(
            this PipelineValue<string> pipelineValue,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (value, cfg) =>
            {
                if (value == null)
                {
                    return value;
                }
                return cfg.TrimChars != null ? value.Trim(cfg.TrimChars) : value.Trim();
            });
        }

        /// <summary>
        /// Converts the string to uppercase.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a string.</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue with the uppercase string.</returns>
        public static PipelineValue<string> ToUpper(
            this PipelineValue<string> pipelineValue,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (value, cfg) =>
            {
                return value?.ToUpper(cfg.Culture) ?? value;
            });
        }

        /// <summary>
        /// Converts the string to lowercase.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a string.</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue with the lowercase string.</returns>
        public static PipelineValue<string> ToLower(
            this PipelineValue<string> pipelineValue,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (value, cfg) =>
            {
                return value?.ToLower(cfg.Culture) ?? value;
            });
        }

        /// <summary>
        /// Replaces occurrences of a string with another string.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a string.</param>
        /// <param name="oldValue">The string to replace.</param>
        /// <param name="newValue">The replacement string.</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue with the replaced string.</returns>
        public static PipelineValue<string> Replace(
            this PipelineValue<string> pipelineValue,
            string oldValue,
            string newValue,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (value, cfg) =>
            {
                return value?.Replace(oldValue, newValue) ?? value;
            });
        }
    }
}

