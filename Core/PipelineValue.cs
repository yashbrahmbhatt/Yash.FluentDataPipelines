using System;
using System.Collections.Generic;
using System.Linq;

namespace Yash.FluentDataPipelines.Core
{
    /// <summary>
    /// Represents a value in a data pipeline with validation state tracking.
    /// This type is immutable and all operations return new instances.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public class PipelineValue<T>
    {
        /// <summary>
        /// Gets the wrapped value.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Gets a value indicating whether the current value is valid.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Gets the collection of errors that have occurred during processing.
        /// </summary>
        public IReadOnlyList<PipelineError> Errors { get; }

        /// <summary>
        /// Initializes a new instance of the PipelineValue class.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="isValid">Whether the value is valid. Defaults to true.</param>
        /// <param name="errors">Collection of errors. Defaults to empty list.</param>
        public PipelineValue(T value, bool isValid = true, IEnumerable<PipelineError> errors = null)
        {
            Value = value;
            IsValid = isValid;
            Errors = errors?.ToList().AsReadOnly() ?? new List<PipelineError>().AsReadOnly();
        }

        /// <summary>
        /// Creates a new PipelineValue with the same validation state but a new value.
        /// </summary>
        /// <typeparam name="U">The type of the new value.</typeparam>
        /// <param name="newValue">The new value.</param>
        /// <returns>A new PipelineValue with the new value and same validation state.</returns>
        public PipelineValue<U> WithValue<U>(U newValue)
        {
            return new PipelineValue<U>(newValue, IsValid, Errors);
        }

        /// <summary>
        /// Creates a new PipelineValue with an updated validation state.
        /// </summary>
        /// <param name="isValid">The new validation state.</param>
        /// <param name="error">Optional error to add if validation fails.</param>
        /// <returns>A new PipelineValue with updated validation state.</returns>
        public PipelineValue<T> WithValidation(bool isValid, PipelineError error = null)
        {
            var newErrors = Errors.ToList();
            if (!isValid && error != null)
            {
                newErrors.Add(error);
            }
            return new PipelineValue<T>(Value, isValid && IsValid, newErrors);
        }

        /// <summary>
        /// Creates a new PipelineValue with an added error.
        /// </summary>
        /// <param name="error">The error to add.</param>
        /// <returns>A new PipelineValue with the added error and IsValid set to false.</returns>
        public PipelineValue<T> WithError(PipelineError error)
        {
            var newErrors = Errors.ToList();
            newErrors.Add(error);
            return new PipelineValue<T>(Value, false, newErrors);
        }

        /// <summary>
        /// Creates a new PipelineValue with an added error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="operation">The operation where the error occurred.</param>
        /// <returns>A new PipelineValue with the added error and IsValid set to false.</returns>
        public PipelineValue<T> WithError(string message, string operation = null)
        {
            return WithError(new PipelineError(message, operation));
        }

        /// <summary>
        /// Implicitly converts a PipelineValue to its underlying value type.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue to unwrap.</param>
        public static implicit operator T(PipelineValue<T> pipelineValue)
        {
            return pipelineValue != null ? pipelineValue.Value : default(T);
        }

        /// <summary>
        /// Implicitly converts a value to a PipelineValue.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        public static implicit operator PipelineValue<T>(T value)
        {
            return new PipelineValue<T>(value);
        }
    }
}

