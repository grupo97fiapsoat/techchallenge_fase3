using FastFood.Domain.Shared.Exceptions;

namespace FastFood.Domain.Products.Exceptions;

/// <summary>
/// Exceção de domínio específica para entidades de produto.
/// </summary>
public class ProductDomainException : DomainException
{
    public ProductDomainException(string message) : base(message)
    {
    }
}
