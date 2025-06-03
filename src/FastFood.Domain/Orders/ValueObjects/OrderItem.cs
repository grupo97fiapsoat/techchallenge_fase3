using FastFood.Domain.Orders.Exceptions;
using FastFood.Domain.Shared.ValueObjects;

namespace FastFood.Domain.Orders.ValueObjects;

/// <summary>
/// Value Object que representa um item de um pedido.
/// </summary>
public sealed class OrderItem : ValueObject
{
    /// <summary>
    /// ID do item no pedido.
    /// </summary>
    public Guid Id { get; private set; }
    
    /// <summary>
    /// ID do produto.
    /// </summary>
    public Guid ProductId { get; private set; }
    
    /// <summary>
    /// Nome do produto no momento do pedido.
    /// </summary>
    public string ProductName { get; private set; }
    
    /// <summary>
    /// Preço unitário do produto no momento do pedido.
    /// </summary>
    public decimal UnitPrice { get; private set; }
    
    /// <summary>
    /// Quantidade do produto.
    /// </summary>
    public int Quantity { get; private set; }
    
    /// <summary>
    /// Valor total do item (quantidade * preço unitário).
    /// </summary>
    public decimal SubTotal => Quantity * UnitPrice;

    /// <summary>
    /// Construtor privado para uso do EF Core.
    /// </summary>
    private OrderItem()
    {
        ProductName = string.Empty;
    }

    /// <summary>
    /// Construtor interno para criação de um item de pedido.
    /// </summary>
    private OrderItem(Guid id, Guid productId, string productName, decimal unitPrice, int quantity)
    {
        Id = id;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    /// <summary>
    /// Factory method para criar um novo item de pedido com validação de todos os campos.
    /// </summary>
    /// <param name="productId">ID do produto.</param>
    /// <param name="productName">Nome do produto.</param>
    /// <param name="unitPrice">Preço unitário do produto.</param>
    /// <param name="quantity">Quantidade do produto.</param>
    /// <returns>Uma nova instância de OrderItem com os dados validados.</returns>
    /// <exception cref="OrderDomainException">Lançada quando algum dos campos é inválido.</exception>
    public static OrderItem Create(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (productId == Guid.Empty)
            throw new OrderDomainException("O ID do produto é obrigatório");

        if (string.IsNullOrWhiteSpace(productName))
            throw new OrderDomainException("O nome do produto é obrigatório");

        if (unitPrice <= 0)
            throw new OrderDomainException("O preço unitário deve ser maior que zero");

        if (quantity <= 0)
            throw new OrderDomainException("A quantidade deve ser maior que zero");

        return new OrderItem(Guid.NewGuid(), productId, productName, unitPrice, quantity);
    }

    /// <summary>
    /// Retorna um novo OrderItem com a quantidade atualizada.
    /// </summary>
    /// <param name="quantity">Nova quantidade.</param>
    /// <returns>Um novo OrderItem com a quantidade atualizada.</returns>
    /// <exception cref="OrderDomainException">Lançada quando a quantidade é inválida.</exception>
    public OrderItem WithQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new OrderDomainException("A quantidade deve ser maior que zero");

        return new OrderItem(Id, ProductId, ProductName, UnitPrice, quantity);
    }    /// <summary>
    /// Retorna os valores que compõem este value object.
    /// </summary>
    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return ProductId;
        yield return ProductName;
        yield return UnitPrice;
        yield return Quantity;
    }
}
