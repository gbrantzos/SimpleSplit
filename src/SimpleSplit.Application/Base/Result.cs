namespace SimpleSplit.Application.Base
{
    /// <summary>
    /// Helper class to wrap an operation or method result.
    /// </summary>
    public class Result
    {
        private static readonly List<string> EmptyList = Array.Empty<string>().ToList();

        public enum ResultStatus
        {
            Success,
            Failure,
            Exception
        }

        public ResultStatus Status { get; protected init; } = ResultStatus.Success;
        public bool HasErrors => Status != ResultStatus.Success;

        protected List<string> ErrorList = EmptyList;
        public IReadOnlyCollection<string> Errors => ErrorList.AsReadOnly();
        public string AllErrors(string separator = null) => String.Join(separator ?? Environment.NewLine, ErrorList);

        public Exception Exception { get; protected init; }
        public bool HasException => Exception is not null;

        // We shall not be able to create base class instance from anywhere!
        protected Result()
        {
        }

        // Factory methods
        public static Result<TData> FromError<TData>(string error) => FromError<TData>(new[] { error });

        public static Result<TData> FromError<TData>(IEnumerable<string> errors)
        {
            var messages = (errors ?? Array.Empty<string>()).Distinct();
            return new Result<TData>(default, messages);
        }

        public static Result<TData> FromException<TData>(Exception exception) => FromException<TData>(null, exception);

        public static Result<TData> FromException<TData>(string message, Exception exception)
        {
            var errorItems = new List<string>();
            if (!string.IsNullOrEmpty(message))
                errorItems.Add(message);

            var current = exception;
            errorItems.Add(current.Message);
            while (current.InnerException != null)
            {
                current = current.InnerException;
                errorItems.Add(current.Message);
            }

            return new Result<TData>(default, errorItems, exception);
        }
    }

    /// <summary>
    /// Helper class to wrap an operation or method result. Result is of type <typeparamref name="TValue"/>
    /// </summary>
    public sealed class Result<TValue> : Result
    {
        // The actual data returned with response
        public TValue Value { get; }

        public Result(TValue value, IEnumerable<string> errors = null, Exception exception = null)
        {
            Value     = value;
            Exception = exception;
            ErrorList = (errors ?? Array.Empty<string>()).ToList();

            if (errors?.Any() == true)
                Status = ResultStatus.Failure;

            if (exception is not null)
                Status = ResultStatus.Exception;
        }

        // Implicit operators, for lazy people
        public static implicit operator Result<TValue>(TValue data) => new(data);
    }
}