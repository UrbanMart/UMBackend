/*
 * File: CronJobSettingController.cs
 * Description: This file defines the CronJobSettingController class for managing API requests 
 *              related to updating cron job settings in the UrbanMart application.
 *              Specifically, it allows the updating of the interval time for cron jobs.
 * Author: Imesh Vitharana
 * Date: 11/10/24
 */

using Microsoft.AspNetCore.Mvc;
using urbanmart.Services;
using urbanmart.Models;

namespace urbanmart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CronJobSettingController : ControllerBase
    {
        private readonly CronJobSettingService _cronJobSettingService;

        public CronJobSettingController(CronJobSettingService cronJobSettingService)
        {
            _cronJobSettingService = cronJobSettingService;
        }

        // PATCH: api/CronJobSetting/name/{name}
        [HttpPatch("name/{name}")]
        public IActionResult UpdateIntervalByName(string name, [FromBody] int intervalInMinutes)
        {
            var cronJobSetting = _cronJobSettingService.GetByName(name);

            if (cronJobSetting == null)
            {
                return NotFound(new { Message = "CronJobSetting not found." });
            }

            _cronJobSettingService.UpdateIntervalByName(name, intervalInMinutes);

            return Ok(new { Message = "Interval updated successfully.", NewInterval = intervalInMinutes });
        }

        // POST: api/CronJobSetting
        [HttpPost]
        public IActionResult CreateCronJobSetting([FromBody] CronJobSetting cronJobSetting)
        {
            if (cronJobSetting == null || cronJobSetting.IntervalInMinutes <= 0 || string.IsNullOrWhiteSpace(cronJobSetting.Name))
            {
                return BadRequest(new { Message = "Invalid cron job setting. Name and interval must be provided." });
            }

            _cronJobSettingService.Create(cronJobSetting);
            return CreatedAtAction(nameof(CreateCronJobSetting), new { id = cronJobSetting.Id }, cronJobSetting);
        }
    }
}
