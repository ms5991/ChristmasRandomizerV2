using System;
using System.Collections.Generic;
using System.Text;

namespace ChristmasRandomizerV2.Core
{
    /// <summary>
    /// Simple logging interface
    /// </summary>
    public interface ILogger
    {
        void Log(string message);
    }

    /// <summary>
    /// Simple logger which logs if 
    /// bool is true.
    /// </summary>
    public class Logger : ILogger
    {
        private bool _shouldLog;

        public Logger(bool shouldLog)
        {
            this._shouldLog = shouldLog;
        }

        public void Log(string message)
        {
            if (this._shouldLog)
            {
                Console.WriteLine(message);
            }
        }
    }
}
