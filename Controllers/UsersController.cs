/*
 * File: UsersController.cs
 * Description: This file defines the UsersController class for the UrbanMart application. 
 *              It provides API endpoints for managing users, including operations like 
 *              fetching, creating, updating, deleting users, and user login authentication.
 * Author: Darshi Buddhini
 * Date: 16/09/24
 * 
 * The UsersController class uses the UsersService class to perform operations 
 * on user data stored in a MongoDB database.
 */

using urbanmart.Models;
using urbanmart.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace urbanmart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _userService;
        private readonly NotificationsService _notificationsService;

        public UsersController(UsersService userService, NotificationsService notificationsService)
        {
            _userService = userService;
            _notificationsService = notificationsService;
        }

        // GET: api/Users
        [HttpGet]
        public ActionResult<List<User>> Get()
        {
            try
            {
                var users = _userService.Get();
                return Ok(users); // Return 200 with the list of users
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Users/{id}
        [HttpGet("{id:length(24)}", Name = "GetUser")]
        public ActionResult<User> Get(string id)
        {
            try
            {
                var user = _userService.Get(id);

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/Users
        [HttpPost]
        public ActionResult<User> Create(User user)
        {
            if (user == null)
            {
                return BadRequest("User is null");
            }

            try
            {
                // Create a new user
                _userService.Create(user);

                // Check if the user is a customer before notifying CSR(s)
                if (user.Role == "Customer") 
                {
                    // Find CSR(s) in the system to notify
                    var csrs = _userService.GetUsersByRole("CSR");
                    foreach (var csr in csrs)
                    {
                        // Create a notification for each CSR about the new customer account
                        Notification notification = new Notification
                        {
                            UserId = csr.Id, 
                            Message = $"New customer account created for {user.Email}. Approval is required.",
                            IsRead = false,
                            Type = "NewCustomer" // Notification type for new customer account
                        };
                        _notificationsService.Create(notification);
                    }
                }

                // Return the newly created user details
                return CreatedAtRoute("GetUser", new { id = user.Id }, user);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Users/{id}
        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, User userIn)
        {
            if (userIn == null || userIn.Id != id)
            {
                return BadRequest("Invalid user data");
            }

            try
            {
                var user = _userService.Get(id);

                if (user == null)
                {
                    return NotFound();
                }

                _userService.Update(id, userIn);
                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/Users/{id}
        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            try
            {
                var user = _userService.Get(id);

                if (user == null)
                {
                    return NotFound();
                }

                _userService.Delete(id);
                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public ActionResult<User> Login([FromBody] UserLoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Invalid login details");
            }

            try
            {
                var user = _userService.Login(loginDto.Email, loginDto.Password);

                if (user == null)
                {
                    return Unauthorized("Invalid email or password");
                }

                return Ok(user); // Return authenticated user data
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // CSR approves customer account
        [HttpPut("{id:length(24)}/approve")]
        public IActionResult ApproveCustomer(string id)
        {
            try
            {
                var user = _userService.Get(id);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                // Activate the user
                user.IsActive = true;
                _userService.Update(id, user);

                // Notify the customer that their account is approved
                Notification notification = new Notification
                {
                    UserId = user.Id, // Customer user ID
                    Message = "Your account has been approved. You can now log in to the mobile app.",
                    IsRead = false,
                    Type = "AccountApproval" // Notification type for account approval
                };
                _notificationsService.Create(notification);

                return NoContent(); // Successful approval
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // Data Transfer Object (DTO) for login
        public class UserLoginDto
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
