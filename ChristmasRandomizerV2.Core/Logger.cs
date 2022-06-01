using System;
using System.Collections.Generic;
using System.Text;

namespace ChristmasRandomizerV2.Core
{
    public interface ILogger
    {
        void Log(string message);
    }


    public class Logger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
