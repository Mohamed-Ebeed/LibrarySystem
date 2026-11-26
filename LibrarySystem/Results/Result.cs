namespace LibrarySystem.Results
{
    public enum ErrorType
    {
        None,
        NotFound,
        BadRequest
    }

    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; }
        public ErrorType ErrorType { get; }

        protected Result(bool isSuccess, string error, ErrorType errorType)
        {
            IsSuccess = isSuccess;
            Error = error;
            ErrorType = errorType;
        }

        public static Result Success() => new Result(true, string.Empty, ErrorType.None);
        public static Result NotFound(string error) => new Result(false, error, ErrorType.NotFound);
        public static Result Failure(string error) => new Result(false, error, ErrorType.BadRequest);
    }
}