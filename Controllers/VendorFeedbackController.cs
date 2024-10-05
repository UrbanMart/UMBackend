using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using urbanmart.Models;
using urbanmart.Services;

namespace urbanmart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorFeedbackController : ControllerBase
    {
        private readonly VendorFeedbackService _feedbackService;

        public VendorFeedbackController(VendorFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        // GET: api/VendorFeedback/vendor/{vendorId}
        [HttpGet("vendor/{vendorId:length(24)}", Name = "GetFeedbacksForVendor")]
        public ActionResult<List<VendorFeedback>> GetFeedbacksForVendor(string vendorId)
        {
            var feedbacks = _feedbackService.GetFeedbacksForVendor(vendorId);
            if (feedbacks == null)
            {
                return NotFound("Vendor not found or no feedback available");
            }
            return feedbacks;
        }
        // GET: api/VendorFeedback/all
        [HttpGet("all", Name = "GetAllFeedbacks")]
        public ActionResult<List<VendorFeedback>> GetAllFeedbacks()
        {
            var feedbacks = _feedbackService.GetAllFeedbacks();
            return feedbacks;
        }

        // GET: api/VendorFeedback/comments
        [HttpGet("comments", Name = "GetAllComments")]
        public ActionResult<List<string>> GetAllComments()
        {
            var comments = _feedbackService.GetAllComments();
            return comments;
        }

        // POST: api/VendorFeedback
        [HttpPost]
        public ActionResult<VendorFeedback> Create(VendorFeedback feedback)
        {
            try
            {
                _feedbackService.Create(feedback);
                // Return the created feedback with correct route name and parameter
                return CreatedAtRoute("GetFeedbacksForVendor", new { vendorId = feedback.VendorId }, feedback);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/VendorFeedback/{id}/comment
        [HttpPut("{id:length(24)}/comment")]
        public IActionResult UpdateComment(string id, [FromBody] string newComment)
        {
            _feedbackService.UpdateComment(id, newComment);
            return NoContent();
        }
    }
}
