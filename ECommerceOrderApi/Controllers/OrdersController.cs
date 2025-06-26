using ECommerceOrderApi.Data;
using ECommerceOrderApi.DTOs;
using ECommerceOrderApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ECommerceOrderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {


        // Dependecy Injection, DI Fields
        private readonly AppDbContext _context;


        // Dependecy Injection, DI Constructors
        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateOrder(CreateOrderRequest request)
        {
            foreach (var item in request.Items)
            {
                var product = _context.Products.Find(item.ProductId);
                if (product == null || product.Stock < item.Quantity)
                {
                    return BadRequest($"Product {item.ProductId} is out of stock.");
                }
            }

            var order = new Order
            {
                UserId = request.UserId,
                Items = request.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            // Stok güncelleyelim.
            foreach (var item in order.Items)
            {
                var product = _context.Products.Find(item.ProductId);
                product.Stock -= item.Quantity;
            }

            _context.Orders.Add(order);
            _context.SaveChanges();

            return Ok(order);
        }

        [HttpGet("{userId}")]
        public IActionResult GetOrdersByUser(string userId)
        {
            var orders = _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.UserId == userId)
                .ToList();

            return Ok(orders);
        }

        [HttpGet("detail/{orderId}")]
        public IActionResult GetOrderDetail(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpDelete("{orderId}")]
        public IActionResult DeleteOrder(int orderId)
        {
            var order = _context.Orders.Include(o => o.Items).FirstOrDefault(o => o.Id == orderId);
            if (order == null) return NotFound();

            _context.OrderItems.RemoveRange(order.Items);
            _context.Orders.Remove(order);
            _context.SaveChanges();

            return NoContent();
        }
    }

}
