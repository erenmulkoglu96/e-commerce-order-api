namespace ECommerceOrderApi.DTOs
{
    public class CreateOrderRequest
    {
        public string UserId { get; set; }
        public List<OrderItemRequest> Items { get; set; }

        public class OrderItemRequest
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }

    }
}
