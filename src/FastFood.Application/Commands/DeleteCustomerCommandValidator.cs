using FastFood.Domain.Customers.Repositories;
using FastFood.Domain.Orders.Repositories;
using FluentValidation;

namespace FastFood.Application.Commands;

/// <summary>
/// Validador para o command de exclusão de cliente
/// </summary>
public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderRepository _orderRepository;

    public DeleteCustomerCommandValidator(
        ICustomerRepository customerRepository, 
        IOrderRepository orderRepository)
    {
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("O ID do cliente é obrigatório")
            .MustAsync(CustomerExist)
            .WithMessage("Cliente não encontrado")
            .MustAsync(NotHaveOrders)
            .WithMessage("Não é possível excluir um cliente que possui pedidos");
    }

    private async Task<bool> CustomerExist(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        return customer != null;
    }

    private async Task<bool> NotHaveOrders(Guid id, CancellationToken cancellationToken)
    {
        var hasOrders = await _orderRepository.CustomerHasOrdersAsync(id, cancellationToken);
        return !hasOrders;
    }
}
