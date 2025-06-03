namespace FastFood.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException() : base("Um ou mais erros de validação ocorreram.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> errors) : this()
    {
        Errors = errors;
    }
    
    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>
        {
            ["Error"] = new[] { message }
        };
    }
}
