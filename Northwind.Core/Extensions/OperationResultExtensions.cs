using Northwind.Core.Models.Results;

namespace Northwind.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for OperationResult.
    /// </summary>
    public static class OperationResultExtensions
    {
        public static async Task<T?> GetDataAsync<T>(this Task<OperationResult<T?>> operationTask)
        {
            var result = await operationTask;
            return result.NewData;
        }
    }
}
