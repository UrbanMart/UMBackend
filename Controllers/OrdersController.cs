using urbanmart.Models;
using urbanmart.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace urbanmart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersService _orderService;

        public OrdersController(OrdersService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public ActionResult<List<Order>> Get() => _orderService.Get();

        [HttpGet("{id:length(24)}", Name = "GetOrder")]
        public ActionResult<Order> Get(string id)
        {
            var order = _orderService.Get(id);
            if (order == null)
            {
                return NotFound();
            }
            return order;
        }

        [HttpPost]
        public ActionResult<Order> Create(Order order)
        {
            _orderService.Create(order);
            return CreatedAtRoute("GetOrder", new { id = order.Id }, order);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Order orderIn)
        {
            var order = _orderService.Get(id);
            if (order == null)
            {
                return NotFound();
            }
            _orderService.Update(id, orderIn);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var order = _orderService.Get(id);
            if (order == null)
            {
                return NotFound();
            }
            _orderService.Delete(id);
            return NoContent();
        }
    }
}
