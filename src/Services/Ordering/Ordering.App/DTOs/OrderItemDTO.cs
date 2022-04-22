namespace ECom.Services.Ordering.App.DTOs
#nullable disable
{
    public class OrderItemDTO
    {
        public string ProductName { get; init; }
        public string PictureUrl { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal Discount { get; init; }
        public int Units { get; init; }
        public int ProductId { get; init; }
    }
}
