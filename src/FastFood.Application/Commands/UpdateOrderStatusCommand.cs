using MediatR;

namespace FastFood.Application.Commands;

public class UpdateOrderStatusCommand : IRequest<UpdateOrderStatusCommandResult>
{
    public Guid Id { get; set; }
    public string Status { get; set; }
}

public class UpdateOrderStatusCommandResult
{
    public Guid Id { get; set; }
    public string Status { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool NotificationSent { get; set; }
}
