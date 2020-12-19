using System;
using System.IO;
using System.Text.Json;
using RabbitMqInActionCsharp.Chapter4.AlertConsumer.Models;

namespace RabbitMqInActionCsharp.Chapter4.AlertConsumer
{
    public static class Setup
    {
        private static Configuration _configuration;
        
        public static Configuration GetConfiguration()
        {
            if (_configuration != null)
            {
                return _configuration;
            }

            var configurationString = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}/appsettings.local.json");
            _configuration = JsonSerializer.Deserialize<Configuration>(configurationString);

            return _configuration;
        }
    }
}