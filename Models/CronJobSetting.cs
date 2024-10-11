/*
 * File: CronJobSetting.cs
 * Description: This file defines the CronJobSetting model for the UrbanMart application.
 *              It stores the configuration settings for cron jobs, such as the interval 
 *              in minutes for the NotificationCronJobService. The model is stored in a MongoDB collection.
 * Author: Imesh Vitharana
 * Date: 11/10/24
 * 
 * The CronJobSetting class is used to dynamically configure the execution interval of cron jobs.
 */

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace urbanmart.Models
{
    public class CronJobSetting
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; } 

        [BsonElement("IntervalInMinutes")]
        public int IntervalInMinutes { get; set; }
    }
}
