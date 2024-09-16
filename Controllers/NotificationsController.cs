using urbanmart.Models;
using urbanmart.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace urbanmart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationsService _notificationService; // Updated service name

        public NotificationsController(NotificationsService notificationService) // Updated parameter type
        {
            _notificationService = notificationService;
        }

        // GET: api/notifications
        [HttpGet]
        public ActionResult<List<Notification>> Get() => _notificationService.Get();

        // GET: api/notifications/{id}
        [HttpGet("{id:length(24)}", Name = "GetNotification")]
        public ActionResult<Notification> Get(string id)
        {
            var notification = _notificationService.Get(id);
            if (notification == null)
            {
                return NotFound();
            }
            return notification;
        }

        // POST: api/notifications
        [HttpPost]
        public ActionResult<Notification> Create(Notification notification)
        {
            _notificationService.Create(notification);
            return CreatedAtRoute("GetNotification", new { id = notification.Id }, notification);
        }

        // PUT: api/notifications/{id}
        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Notification notificationIn)
        {
            var notification = _notificationService.Get(id);
            if (notification == null)
            {
                return NotFound();
            }
            _notificationService.Update(id, notificationIn);
            return NoContent();
        }

        // PATCH: api/notifications/{id}/markasread
        [HttpPatch("{id:length(24)}/markasread")]
        public IActionResult MarkAsRead(string id)
        {
            var notification = _notificationService.Get(id);
            if (notification == null)
            {
                return NotFound();
            }
            _notificationService.MarkAsRead(id);
            return NoContent();
        }

        // DELETE: api/notifications/{id}
        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var notification = _notificationService.Get(id);
            if (notification == null)
            {
                return NotFound();
            }
            _notificationService.Delete(id);
            return NoContent();
        }
    }
}
