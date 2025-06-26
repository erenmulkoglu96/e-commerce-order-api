namespace ECommerceOrderApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<OrderItem> Items { get; set; }
    }
}
