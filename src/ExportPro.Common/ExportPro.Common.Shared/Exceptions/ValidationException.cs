

using FluentValidation.Results;

namespace ExportPro.Common.Shared.Exceptions
{
    public class ValidationException: Exception
    {
        public ValidationException() : base("One or more validation errors.")
        {
            Failures = new Dictionary<string, string[]>();
        }

        public ValidationException(List<ValidationFailure> failures) : this()
        {
            var propNames = failures.Select(e => e.PropertyName).Distinct();

            foreach (var propName in propNames)
            {
                var propFailures = failures
                    .Where(e => e.PropertyName == propName)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                Failures.Add(propName, propFailures);

            }
        }

        public IDictionary<string, string[]> Failures { get; }
    }
}
