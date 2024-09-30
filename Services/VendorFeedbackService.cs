using MongoDB.Driver;
using System.Collections.Generic;
using urbanmart.Models;

namespace urbanmart.Services
{
    public class VendorFeedbackService
    {
        private readonly IMongoCollection<VendorFeedback> _feedbacks;
        private readonly IMongoCollection<User> _users; // Access to User collection to get vendor role

        public VendorFeedbackService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _feedbacks = database.GetCollection<VendorFeedback>("VendorFeedbacks");
            _users = database.GetCollection<User>("Users"); // Assuming users are stored in "Users" collection
        }

        // Get feedback for a specific vendor
        public List<VendorFeedback> GetFeedbacksForVendor(string vendorId)
        {
            var vendor = _users.Find(user => user.Id == vendorId && user.Role == "Vendor").FirstOrDefault();
            if (vendor == null)
            {
                return null; // Vendor not found or user is not a vendor
            }

            return _feedbacks.Find(feedback => feedback.VendorId == vendorId).ToList();
        }

        // Get all feedback comments
        public List<string> GetAllComments()
        {
            return _feedbacks.Find(feedback => true).Project(feedback => feedback.Comment).ToList();
        }

        // Add new feedback
        public VendorFeedback Create(VendorFeedback feedback)
        {
            var vendor = _users.Find(user => user.Id == feedback.VendorId && user.Role == "Vendor").FirstOrDefault();
            if (vendor == null)
            {
                throw new KeyNotFoundException("Vendor not found");
            }

            _feedbacks.InsertOne(feedback);
            return feedback;
        }

        // Update comment only (not the rating)
        public void UpdateComment(string feedbackId, string newComment)
        {
            var update = Builders<VendorFeedback>.Update
                .Set(f => f.Comment, newComment);

            _feedbacks.UpdateOne(feedback => feedback.Id == feedbackId, update);
        }
    }
}
