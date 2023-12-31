﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CompanyInfoTrackingSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("upload", Name = "upload")]
        public async Task<IActionResult> UploadFile(
         IFormFile Photo)
        {
            if (CheckIfExcelFile(Photo))
            {
                await WriteFile(Photo);
            }
            else
            {
                return BadRequest(new { message = "Invalid Photo extension" });
            }

            return Ok();
        }

        private bool CheckIfExcelFile(IFormFile Photo)
        {
            var extension = "." + Photo.FileName.Split('.')[Photo.FileName.Split('.').Length - 1];
            return (extension == ".jpg" || extension == ".png"); // Change the extension based on your need
        }
        private async Task<bool> WriteFile(IFormFile Photo)
        {
            bool isSaveSuccess = false;
            string fileName;
            try
            {
                var extension = "." + Photo.FileName.Split('.')[Photo.FileName.Split('.').Length - 1];
                fileName = DateTime.Now.Ticks + extension; //Create a new Name for the Photo due to security reasons.

                var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");

                if (!Directory.Exists(pathBuilt))
                {
                    Directory.CreateDirectory(pathBuilt);
                }

                var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files",
                   fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await Photo.CopyToAsync(stream);
                }

                isSaveSuccess = true;
            }
            catch (Exception e)
            {
               //log
            }
            return isSaveSuccess;
        }
    }
}
