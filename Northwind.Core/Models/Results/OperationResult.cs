namespace Northwind.Core.Models.Results
{
    /// <summary>
    /// Represents the result of an operation.
    /// </summary>
    /// <typeparam name="T">The type of the document involved in the operation.</typeparam>
    public class OperationResult<T>
    {
        /// <summary>
        /// Gets or sets the operation message.
        /// </summary>
        public string Message { get; set; } = "";
        /// <summary>
        /// Gets or sets the state of the operation.
        /// </summary>
        public OperationState State { get; set; }
        /// <summary>
        /// Gets or sets the status code representing the outcome of the operation.
        /// </summary>
        public string StatusCode { get; set; } = "";
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool Success => State == OperationState.Success;
        /// <summary>
        /// Gets or sets the previous document state if applicable.
        /// </summary>
        public T? OldData { get; set; }
        /// <summary>
        /// Gets or sets the new document state if applicable.
        /// </summary>
        public T? NewData { get; set; }
    }

    /// <summary>
    /// Specifies the possible states of an operation.
    /// </summary>
    public enum OperationState
    {
        /// <summary>
        /// The operation was successful.
        /// </summary>
        Success,
        /// <summary>
        /// The operation failed.
        /// </summary>
        Failure
    }
}
