using INDProAssignments.Data;
using INDProAssignments.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace INDProAssignments.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            order.Id = 0;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrderTotal), new { id = order.Id }, order);
        }

        [HttpPost("{id}/items")]
        public async Task<IActionResult> AddOrderItem(int id, [FromBody] OrderItem item)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            item.OrderId = id;
            _context.OrderItems.Add(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}/total")]
        public async Task<IActionResult> GetOrderTotal(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            var total = order.OrderItems.Sum(i => i.Quantity * i.Price);
            return Ok(new { order_id = id, total_price = total });
        }
    }

}
