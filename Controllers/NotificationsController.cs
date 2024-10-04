/*
 * File: NotificationsController.cs
 * Description: This file defines the NotificationsController for managing notifications in the UrbanMart application. 
 *              It provides HTTP endpoints for creating, retrieving, updating, and deleting notifications. 
 *              Additionally, it includes functionality to mark notifications as read.
 * Author: Dinithi Mendis
 * Date:16/09/24
 * 
 * The NotificationsController interacts with the NotificationsService to perform operations on notifications.
 * It handles CRUD operations and provides endpoints for various notification-related actions.
 */
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
        private readonly NotificationsService _notificationsService;

        public NotificationsController(NotificationsService notificationsService)
        {
            _notificationsService = notificationsService;
        }

        // GET: api/notifications
        [HttpGet]
        public ActionResult<List<Notification>> GetAll()
        {
            var notifications = _notificationsService.Get();
            return Ok(notifications);
        }

        // GET: api/notifications/user/{userId}
        [HttpGet("user/{userId:length(24)}")]
        public ActionResult<List<Notification>> GetByUserId(string userId)
        {
            var notifications = _notificationsService.GetByUserId(userId);
            if (notifications == null || notifications.Count == 0)
            {
                return NotFound("Notifications not found");
            }
            return Ok(notifications);
        }

        // GET: api/notifications/{id}
        [HttpGet("{id:length(24)}")]
        public ActionResult<Notification> Get(string id)
        {
            var notification = _notificationsService.Get(id);
            if (notification == null)
            {
                return NotFound("Notification not found");
            }
            return Ok(notification);
        }

        // POST: api/notifications
        [HttpPost]
        public ActionResult<Notification> Create(Notification notification)
        {
            if (notification == null)
            {
                return BadRequest("Notification is null");
            }
            _notificationsService.Create(notification);
            return CreatedAtRoute("GetNotification", new { id = notification.Id }, notification);
        }

        // PUT: api/notifications/{id}/read
        [HttpPut("{id:length(24)}/read")]
        public IActionResult MarkAsRead(string id)
        {
            var notification = _notificationsService.Get(id);
            if (notification == null)
            {
                return NotFound("Notification not found");
            }
            notification.IsRead = true;
            _notificationsService.Update(id, notification);
            return NoContent(); // Successful update
        }

        // DELETE: api/notifications/{id}
        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var notification = _notificationsService.Get(id);
            if (notification == null)
            {
                return NotFound("Notification not found");
            }
            _notificationsService.Delete(id);
            return NoContent(); // Successful deletion
        }
    }
}
