using FastFood.Domain.Shared.Exceptions;

namespace FastFood.Domain.Orders.Exceptions;

/// <summary>
/// Exceção de domínio específica para entidades de pedido.
/// </summary>
public class OrderDomainException : DomainException
{
    public OrderDomainException(string message) : base(message)
    {
    }
}
