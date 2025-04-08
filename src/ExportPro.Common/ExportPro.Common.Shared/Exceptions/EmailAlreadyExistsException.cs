namespace ExportPro.Common.Shared.Exceptions;

public class EmailAlreadyExistsException : Exception
{
    public EmailAlreadyExistsException() : base("Email is already registered") { }

    public EmailAlreadyExistsException(string message) : base(message) { }
}
