using ErrorOr;

namespace ExportPro.Common.Core.Library
{
    public class Result
    {
        protected internal Result(bool isSuccess, Error? error)
        {
            IsSuccess = isSuccess;
            Errors = [error];
        }
        protected internal Result(bool isSuccess, Error?[] errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }
        public bool IsSuccess { get; }
        public Error?[] Errors { get; }
        public bool IsFailure => !IsSuccess;

        public static Result Success() => new(true, (Error?)null);
        public static Result<TValue> Success<TValue>(TValue value) => new(true, value, (Error?)null);
        public static Result Failure(Error error) => new(false, error);
        public static Result Failure(Error?[] errors) => new(false, errors);
        public static Result<TValue> Failure<TValue>(Error error) => new(false, default, error);
        public static Result<TValue> Create<TValue>(TValue? value) => Success(value);

    }
}
