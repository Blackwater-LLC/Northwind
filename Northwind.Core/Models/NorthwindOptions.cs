namespace Northwind.Core.Models
{
    /// <summary>
    /// Contains configuration options for Northwind.
    /// </summary>
    public class NorthwindOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether MongoDB transactions are used.
        /// </summary>
        public bool UseTransactions { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the previous and new document states should be returned.
        /// </summary>
        public bool ReturnDocumentState { get; set; } = false;
    }
}
