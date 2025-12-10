using System;
using System.Collections.Generic;
using System.Linq;
using Yash.FluentDataPipelines.Configuration;
using Yash.FluentDataPipelines.Core;

namespace Yash.FluentDataPipelines.Extensions
{
    /// <summary>
    /// Extension methods for collection operations on PipelineValue.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Transforms a collection PipelineValue using the provided configuration.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <typeparam name="U">The result type.</typeparam>
        /// <param name="pipelineValue">The PipelineValue containing a collection.</param>
        /// <param name="config">The transformation configuration.</param>
        /// <param name="transformer">Function that performs the transformation.</param>
        /// <returns>A PipelineValue with the transformed collection.</returns>
        public static PipelineValue<U> Transform<T, U>(
            this PipelineValue<IEnumerable<T>> pipelineValue,
            TransformationConfig config,
            Func<IEnumerable<T>, TransformationConfig, U> transformer)
        {
            if (pipelineValue == null)
            {
                return new PipelineValue<U>(default(U), false, new[] { new PipelineError("PipelineValue is null", "Transform") });
            }

            if (!pipelineValue.IsValid)
            {
                // Preserve failed state but don't transform
                return new PipelineValue<U>(default(U), false, pipelineValue.Errors);
            }

            try
            {
                var transformedValue = transformer(pipelineValue.Value, config);
                return new PipelineValue<U>(transformedValue, pipelineValue.IsValid, pipelineValue.Errors);
            }
            catch (Exception ex)
            {
                return new PipelineValue<U>(default(U), false, pipelineValue.Errors.Concat(new[] { new PipelineError($"Transformation error: {ex.Message}", "Transform") }));
            }
        }

        /// <summary>
        /// Gets the first element of the collection.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="pipelineValue">The PipelineValue containing a collection.</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue containing the first element or an error if empty.</returns>
        public static PipelineValue<T> First<T>(
            this PipelineValue<IEnumerable<T>> pipelineValue,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (collection, cfg) =>
            {
                try
                {
                    return collection.First();
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
            });
        }

        /// <summary>
        /// Gets the first element of the collection, or a default value if empty.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="pipelineValue">The PipelineValue containing a collection.</param>
        /// <param name="defaultValue">The default value to return if collection is empty.</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue containing the first element or the default value.</returns>
        public static PipelineValue<T> FirstOrDefault<T>(
            this PipelineValue<IEnumerable<T>> pipelineValue,
            T defaultValue = default(T),
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (collection, cfg) => collection.FirstOrDefault());
        }

        /// <summary>
        /// Gets the last element of the collection.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="pipelineValue">The PipelineValue containing a collection.</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue containing the last element or an error if empty.</returns>
        public static PipelineValue<T> Last<T>(
            this PipelineValue<IEnumerable<T>> pipelineValue,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (collection, cfg) =>
            {
                try
                {
                    return collection.Last();
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
            });
        }

        /// <summary>
        /// Gets the last element of the collection, or a default value if empty.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="pipelineValue">The PipelineValue containing a collection.</param>
        /// <param name="defaultValue">The default value to return if collection is empty.</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue containing the last element or the default value.</returns>
        public static PipelineValue<T> LastOrDefault<T>(
            this PipelineValue<IEnumerable<T>> pipelineValue,
            T defaultValue = default(T),
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (collection, cfg) => collection.LastOrDefault());
        }

        /// <summary>
        /// Filters the collection using the specified predicate.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="pipelineValue">The PipelineValue containing a collection.</param>
        /// <param name="predicate">The predicate function to filter by.</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue containing the filtered collection.</returns>
        public static PipelineValue<IEnumerable<T>> Where<T>(
            this PipelineValue<IEnumerable<T>> pipelineValue,
            Func<T, bool> predicate,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (collection, cfg) => collection.Where(predicate));
        }

        /// <summary>
        /// Projects each element of the collection into a new form.
        /// </summary>
        /// <typeparam name="T">The source element type.</typeparam>
        /// <typeparam name="U">The result element type.</typeparam>
        /// <param name="pipelineValue">The PipelineValue containing a collection.</param>
        /// <param name="selector">The selector function.</param>
        /// <param name="config">Optional transformation configuration.</param>
        /// <returns>A PipelineValue containing the projected collection.</returns>
        public static PipelineValue<IEnumerable<U>> Select<T, U>(
            this PipelineValue<IEnumerable<T>> pipelineValue,
            Func<T, U> selector,
            TransformationConfig config = null)
        {
            config = config ?? TransformationConfig.Default;
            return pipelineValue.Transform(config, (collection, cfg) => collection.Select(selector));
        }
    }
}

