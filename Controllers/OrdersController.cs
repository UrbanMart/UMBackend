/*
 * File: OrdersController.cs
 * Description: This file defines the OrdersController for managing orders in the UrbanMart application. 
 *              It provides HTTP endpoints for creating, retrieving, updating, and deleting orders.
 * Author: Imesh Vitharana
 * Date: 15/09/24
 * 
 * The OrdersController interacts with the OrdersService to perform operations on orders.
 * It handles CRUD operations and provides endpoints for various order-related actions.
 */
using urbanmart.Models;
using urbanmart.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace urbanmart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersService _orderService;
        private readonly NotificationsService _notificationsService; 
        private readonly UsersService _usersService; // Injecting UserService

        public OrdersController(OrdersService orderService, NotificationsService notificationsService, UsersService usersService)
        {
            _orderService = orderService;
            _notificationsService = notificationsService;
            _usersService = usersService; // Assign injected UserService
        }

        // GET: api/orders
        [HttpGet]
        public ActionResult<List<Order>> Get() => _orderService.Get();

        // GET: api/orders/{id}
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

        // POST: api/orders
        [HttpPost]
        public ActionResult<Order> Create(Order order)
        {
            var createdOrder = _orderService.Create(order);
            return CreatedAtRoute("GetOrder", new { id = createdOrder.Id }, createdOrder);
        }

        // PUT: api/orders/{id}
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

        // PUT: api/Orders/{id}/request-cancellation
        [HttpPut("{id:length(24)}/request-cancellation")]
        public IActionResult RequestCancellation(string id)
        {
            try
            {
                var order = _orderService.Get(id);
                if (order == null)
                {
                    return NotFound("Order not found");
                }

                // Update order status to Cancellation Requested
                order.Status = "Cancellation Requested";
                _orderService.Update(id, order);

                // Notify both CSR(s) and Administrator about the cancellation request
                var usersToNotify = _usersService.GetUsersByRoles(new List<string> { "CSR", "Administrator" });
                foreach (var user in usersToNotify)
                {
                    Notification notification = new Notification
                    {
                        UserId = user.Id,
                        Message = $"Cancellation requested for order {id}.",
                        IsRead = false,
                        Type = "OrderCancellation" // Notification type for order cancellation
                    };
                    _notificationsService.Create(notification);
                }

                return NoContent(); // Successfully requested cancellation
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Orders/{id}/approve-cancellation
        [HttpPut("{id:length(24)}/approve-cancellation")]
        public IActionResult ApproveCancellation(string id)
        {
            try
            {
                var order = _orderService.Get(id);
                if (order == null)
                {
                    return NotFound("Order not found");
                }

                // Update order status to Cancelled
                order.Status = "Cancelled";
                _orderService.Update(id, order);

                // Notify customer about the cancellation approval
                Notification notification = new Notification
                {
                    UserId = order.CustomerId, // Now this should work as UserId is defined in Order
                    Message = $"Your cancellation request for order {id} has been approved.",
                    IsRead = false,
                    Type = "CancellationApproval" // Notification type for cancellation approval
                };
                _notificationsService.Create(notification);

                return NoContent(); // Successfully approved cancellation
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/orders/{id}
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

        // GET: api/orders/vendor/{vendorId}
        [HttpGet("vendor/{vendorId}")]
        public ActionResult<List<OrderItem>> GetVendorOrders(string vendorId)
        {
            var orderItems = _orderService.GetVendorOrderItems(vendorId);
            if (orderItems == null || !orderItems.Any())
            {
                return NotFound();
            }
            return orderItems;
        }

        // PATCH: api/orders/{orderId}/markitemasdelivered/{productId}/{vendorId}
        [HttpPatch("{orderId:length(24)}/markitemasdelivered/{productId}/{vendorId}")]
        public IActionResult MarkItemAsDelivered(string orderId, string productId, string vendorId)
        {
            var order = _orderService.Get(orderId);
            if (order == null)
            {
                return NotFound();
            }
            _orderService.MarkItemAsDelivered(orderId, productId, vendorId);

            // After marking, check the order status
            var updatedOrder = _orderService.Get(orderId); // Retrieve the updated order status

            // Check if the order status is now "Delivered"
            if (updatedOrder.Status == "Delivered")
            {
                // Notify customer about the delivery
                Notification notification = new Notification
                {
                    UserId = updatedOrder.CustomerId, // Get the customer ID from the order
                    Message = $"Your order {orderId} has been delivered.",
                    IsRead = false,
                    Type = "Delivery" // Notification type for delivery
                };
                _notificationsService.Create(notification);
            }

            return NoContent();
        }
    }
}
