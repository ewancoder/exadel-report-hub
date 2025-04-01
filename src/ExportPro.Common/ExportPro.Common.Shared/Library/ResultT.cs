
using ErrorOr;

namespace ExportPro.Common.Shared.Library
{
    public class Result<TValue> : Result
    {
        private readonly TValue? _value;
        protected internal Result(bool isSuccess, TValue? value, Error? error) : base(isSuccess, error)
        {
            _value = value;
        }

        protected internal Result(TValue? value, bool isSuccess, Error?[] errors) : base(isSuccess, errors)
        {
            _value = value;
        }

        public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("Value cant be accessed");
        public static implicit operator Result<TValue>(TValue? value) => Create(value);
    }
}
