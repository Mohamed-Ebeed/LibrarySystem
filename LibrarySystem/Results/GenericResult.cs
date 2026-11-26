namespace LibrarySystem.Results
{
    public class GenericResult<T> : Result
    {
        public T? Value { get; }

        protected GenericResult(bool isSuccess, T? value, string error, ErrorType errorType)
            : base(isSuccess, error, errorType)
            => Value = value;

        public static GenericResult<T> Success(T value)
            => new GenericResult<T>(true, value, string.Empty, ErrorType.None);

        public static new GenericResult<T> NotFound(string error)
            => new GenericResult<T>(false, default, error, ErrorType.NotFound);

        public static new GenericResult<T> Failure(string error)
            => new GenericResult<T>(false, default, error, ErrorType.BadRequest);
    }
}