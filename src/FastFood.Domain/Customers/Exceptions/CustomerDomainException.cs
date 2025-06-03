using System;
using FastFood.Domain.Shared.Exceptions;

namespace FastFood.Domain.Customers.Exceptions;

/// <summary>
/// Exceção de domínio para regras de negócio relacionadas a clientes.
/// </summary>
public class CustomerDomainException : DomainException
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="CustomerDomainException"/>.
    /// </summary>
    /// <param name="message">A mensagem que descreve o erro.</param>
    public CustomerDomainException(string message) : base(message)
    {
    }

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="CustomerDomainException"/>.
    /// </summary>
    /// <param name="message">A mensagem que descreve o erro.</param>
    /// <param name="innerException">A exceção que causou esta exceção.</param>
    public CustomerDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
