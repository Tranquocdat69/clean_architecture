namespace ECom.Services.Ordering.App.Application.Validations
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(command => command.UserId).NotEmpty();
            RuleFor(command => command.Street).NotEmpty();
            RuleFor(command => command.City).NotEmpty();
            RuleFor(command => command.OrderItems).Must(HaveAnyItem).WithMessage("No item in order");
        }

        private bool HaveAnyItem(IEnumerable<OrderItemDTO> orderItems)
        {
            return orderItems.Any();
        }
    }
}
