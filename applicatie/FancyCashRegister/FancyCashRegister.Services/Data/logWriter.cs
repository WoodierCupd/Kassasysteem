using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyCashRegister.Services.Data
{
    class logWriter
    {
        public int error { get; set; }
        public int info { get; set; }
        public int warning { get; set; }
        public int debug { get; set; }
        public int critical { get; set; }
        
        public void LogLevel()
        {
            debug = 1;
            info = 2;
            warning = 3;
            error = 4;
            critical = 5;
        }

        
    }
}
