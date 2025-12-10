using System;

namespace Yash.FluentDataPipelines.Core
{
    /// <summary>
    /// Represents an error that occurred during pipeline processing.
    /// </summary>
    public class PipelineError
    {
        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the operation or context where the error occurred.
        /// </summary>
        public string Operation { get; }

        /// <summary>
        /// Gets the timestamp when the error occurred.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Initializes a new instance of the PipelineError class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="operation">The operation or context where the error occurred.</param>
        public PipelineError(string message, string operation = null)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Operation = operation;
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Returns a string representation of the error.
        /// </summary>
        public override string ToString()
        {
            return string.IsNullOrEmpty(Operation)
                ? $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Message}"
                : $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Operation}: {Message}";
        }
    }
}

