/*
 * File: CronJobSettingService.cs
 * Description: This file defines the CronJobSettingService class responsible for managing cron job settings 
 *              in the UrbanMart application. It provides methods to get, update, and modify the interval 
 *              for executing cron jobs.
 * Author: Imesh Vitharana
 * Date: 11/10/24
 */

using MongoDB.Driver;
using urbanmart.Models;

namespace urbanmart.Services
{
    public class CronJobSettingService
    {
        private readonly IMongoCollection<CronJobSetting> _cronJobSettings;

        public CronJobSettingService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _cronJobSettings = database.GetCollection<CronJobSetting>("CronJobSettings");
        }

        // Get cron job settings by ID
        public CronJobSetting Get(string id) =>
            _cronJobSettings.Find(c => c.Id == id).FirstOrDefault();

        // Get cron job settings by Name
        public CronJobSetting GetByName(string name) =>
            _cronJobSettings.Find(c => c.Name == name).FirstOrDefault();

        // Update interval in minutes (PATCH equivalent)
        public void UpdateInterval(string id, int intervalInMinutes)
        {
            var update = Builders<CronJobSetting>.Update.Set(c => c.IntervalInMinutes, intervalInMinutes);
            _cronJobSettings.UpdateOne(c => c.Id == id, update);
        }

        // Update interval in minutes by Name
        public void UpdateIntervalByName(string name, int intervalInMinutes)
        {
            var update = Builders<CronJobSetting>.Update.Set(c => c.IntervalInMinutes, intervalInMinutes);
            _cronJobSettings.UpdateOne(c => c.Name == name, update);
        }

        // Create a new cron job setting
        public void Create(CronJobSetting cronJobSetting)
        {
            _cronJobSettings.InsertOne(cronJobSetting);
        }
    }
}
