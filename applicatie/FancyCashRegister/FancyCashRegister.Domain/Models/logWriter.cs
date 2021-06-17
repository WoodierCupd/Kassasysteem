using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace FancyCashRegister.Services.Data
{
    class logWriter
    {
        public void LogStart() 
        {
            Log.Logger = new LoggerConfiguration()
                      .MinimumLevel.Debug()
                      .WriteTo.File (@"C:\Users\stefa\OneDrive\Desktop\amo-1e 2020-2021\blok-b jaar 1\pra\b5- KassaSysteem -\project\Kassasysteem\data\logs\logs.log", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}", rollingInterval: RollingInterval.Day)
                      .MinimumLevel.Debug()
                      .CreateLogger();
            Log.Information("============= Started Logging =============");
            
        } 

        public void logMoment()
        {

        }
    }
}
