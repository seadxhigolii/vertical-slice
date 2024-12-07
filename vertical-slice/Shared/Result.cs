namespace vertical_slice.Shared
{
    public class Result<T>
    {
        protected internal Result(bool isSuccess, T value, Error error)
        {
            if (isSuccess && error != Error.None)
            {
                throw new InvalidOperationException("A successful result cannot have an error.");
            }

            if (!isSuccess && error == Error.None)
            {
                throw new InvalidOperationException("A failed result must have an error.");
            }

            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public T Value { get; }
        public Error Error { get; }

        public static Result<T> Success(T value) => new Result<T>(true, value, Error.None);
        public static Result<T> Failure(Error error) => new Result<T>(false, default, error);
    }

    public class Error
    {
        public static readonly Error None = new Error(string.Empty, string.Empty);

        public string Title { get; }
        public string Message { get; }

        public Error(string title, string message)
        {
            Title = title;
            Message = message;
        }

        public static Error Create(string title, string message) => new Error(title, message);

        public override string ToString() => $"{Title}: {Message}";
    }
}
