namespace FastFood.Domain.Users.Exceptions;

/// <summary>
/// Exceção lançada quando ocorre um erro relacionado ao domínio de usuários.
/// </summary>
public class UserDomainException : Exception
{
    /// <summary>
    /// Inicializa uma nova instância da exceção.
    /// </summary>
    public UserDomainException() : base() { }

    /// <summary>
    /// Inicializa uma nova instância da exceção com uma mensagem.
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    public UserDomainException(string message) : base(message) { }

    /// <summary>
    /// Inicializa uma nova instância da exceção com uma mensagem e uma exceção interna.
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    /// <param name="innerException">Exceção interna</param>
    public UserDomainException(string message, Exception innerException) : base(message, innerException) { }
}
