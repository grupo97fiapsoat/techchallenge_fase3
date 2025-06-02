using FastFood.Domain.Customers.Entities;
using FastFood.Domain.Orders.Exceptions;
using FastFood.Domain.Orders.ValueObjects;
using FastFood.Domain.Products.Entities;
using FastFood.Domain.Shared.Entities;

namespace FastFood.Domain.Orders.Entities;

/// <summary>
/// Representa um pedido no sistema.
/// </summary>
public class Order : Entity
{    /// <summary>
    /// Cliente que realizou o pedido.
    /// </summary>
    public Customer? Customer { get; private set; }

    /// <summary>
    /// ID do cliente que realizou o pedido.
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// Itens do pedido.
    /// </summary>
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    private readonly List<OrderItem> _items;

    /// <summary>
    /// Status do pedido.
    /// </summary>
    public OrderStatus Status { get; private set; }    /// <summary>
    /// Valor total do pedido.
    /// </summary>
    public decimal TotalPrice { get; private set; }

    /// <summary>
    /// QR Code gerado para pagamento do pedido.
    /// </summary>
    public string? QrCode { get; private set; }

    /// <summary>
    /// ID da preferência no Mercado Pago.
    /// </summary>
    public string? PreferenceId { get; private set; }

    /// <summary>
    /// Construtor privado para uso do EF Core.
    /// </summary>
    private Order() : base()
    {
        _items = new List<OrderItem>();
        Status = OrderStatus.Pending;
        TotalPrice = 0;
    }

    /// <summary>
    /// Construtor interno para criação de pedido.
    /// </summary>
    /// <param name="customerId">ID do cliente que está realizando o pedido.</param>
    /// <param name="items">Itens do pedido.</param>
    private Order(Guid customerId, List<OrderItem> items) : base()
    {
        CustomerId = customerId;
        _items = items ?? new List<OrderItem>();
        Status = OrderStatus.Pending;
        CalculateTotalPrice();
    }

    /// <summary>
    /// Factory method para criar um novo pedido com validação de todos os campos.
    /// </summary>
    /// <param name="customerId">ID do cliente que está realizando o pedido.</param>
    /// <param name="items">Itens do pedido.</param>
    /// <returns>Uma nova instância de Order com os dados validados.</returns>
    /// <exception cref="OrderDomainException">Lançada quando algum dos campos é inválido.</exception>
    public static Order Create(Guid customerId, List<OrderItem> items)
    {
        if (customerId == Guid.Empty)
            throw new OrderDomainException("O ID do cliente é obrigatório");

        if (items == null || !items.Any())
            throw new OrderDomainException("O pedido deve ter pelo menos um item");

        return new Order(customerId, items);
    }

    /// <summary>
    /// Adiciona um item ao pedido.
    /// </summary>
    /// <param name="item">Item a ser adicionado.</param>
    /// <exception cref="OrderDomainException">Lançada quando o pedido não está mais em estado pendente.</exception>
    public void AddItem(OrderItem item)
    {
        if (Status != OrderStatus.Pending)
            throw new OrderDomainException("Não é possível adicionar itens a um pedido que não está pendente");

        if (item == null)
            throw new OrderDomainException("O item não pode ser nulo");

        _items.Add(item);
        CalculateTotalPrice();
        SetUpdatedAt();
    }

    /// <summary>
    /// Remove um item do pedido pelo ID do produto.
    /// </summary>
    /// <param name="productId">ID do produto a ser removido.</param>
    /// <returns>true se o item foi removido; false caso contrário.</returns>
    /// <exception cref="OrderDomainException">Lançada quando o pedido não está mais em estado pendente.</exception>
    public bool RemoveItem(Guid productId)
    {
        if (Status != OrderStatus.Pending)
            throw new OrderDomainException("Não é possível remover itens de um pedido que não está pendente");

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            return false;

        var removed = _items.Remove(item);
        if (removed)
        {
            CalculateTotalPrice();
            SetUpdatedAt();
        }
        return removed;
    }

    /// <summary>
    /// Atualiza o status do pedido.
    /// </summary>
    /// <param name="status">Novo status do pedido.</param>
    /// <exception cref="OrderDomainException">Lançada quando a transição de status não é permitida.</exception>
    public void UpdateStatus(OrderStatus status)
    {
        // Validar transições de status
        if (!IsValidStatusTransition(Status, status))
            throw new OrderDomainException($"A transição do status {Status} para {status} não é permitida");

        Status = status;
        SetUpdatedAt();
    }

    /// <summary>
    /// Define o QR Code do pedido.
    /// </summary>
    /// <param name="qrCode">Código QR gerado para pagamento.</param>
    public void SetQrCode(string qrCode)
    {
        if (string.IsNullOrWhiteSpace(qrCode))
            throw new OrderDomainException("O QR Code não pode ser vazio");

        QrCode = qrCode;
        SetUpdatedAt();
    }

    /// <summary>
    /// Define o ID da preferência do Mercado Pago para o pedido.
    /// </summary>
    /// <param name="preferenceId">ID da preferência no Mercado Pago.</param>
    public void SetPreferenceId(string preferenceId)
    {
        if (string.IsNullOrWhiteSpace(preferenceId))
            throw new OrderDomainException("O ID da preferência não pode ser vazio");

        PreferenceId = preferenceId;
        SetUpdatedAt();
    }

    /// <summary>
    /// Verifica se a transição de status é válida.
    /// </summary>
    /// <param name="currentStatus">Status atual.</param>
    /// <param name="newStatus">Novo status.</param>
    /// <returns>true se a transição é válida; false caso contrário.</returns>
    private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {        return (currentStatus, newStatus) switch
        {
            // Transições básicas do fluxo principal
            (OrderStatus.Pending, OrderStatus.Processing) => true,
            (OrderStatus.Processing, OrderStatus.Ready) => true,
            (OrderStatus.Ready, OrderStatus.Completed) => true,
            (OrderStatus.Pending, OrderStatus.Cancelled) => true,
            (OrderStatus.Processing, OrderStatus.Cancelled) => true,
            
            // Transições de pagamento - NOVO FLUXO
            (OrderStatus.Pending, OrderStatus.AwaitingPayment) => true,  // Checkout gera QR Code
            (OrderStatus.AwaitingPayment, OrderStatus.Paid) => true,     // Confirmação de pagamento
            (OrderStatus.AwaitingPayment, OrderStatus.Cancelled) => true, // Cancelamento durante espera
            (OrderStatus.Paid, OrderStatus.Processing) => true,          // Envio para cozinha
            
            // Transições do fluxo antigo (manter compatibilidade)
            (OrderStatus.Pending, OrderStatus.Paid) => true,
            
            // Transição reversa permitida (ex: problemas na cozinha, falta de ingredientes)
            (OrderStatus.Processing, OrderStatus.Pending) => true,
            
            // Status igual, permitido (idempotência)
            var (current, next) when current == next => true,
            
            // Qualquer outra transição é inválida
            _ => false
        };
    }

    /// <summary>
    /// Calcula o valor total do pedido com base nos itens.
    /// </summary>
    private void CalculateTotalPrice()
    {
        TotalPrice = _items.Sum(item => item.SubTotal);
    }
}
