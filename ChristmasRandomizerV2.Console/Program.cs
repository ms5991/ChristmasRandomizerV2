using ChristmasRandomizerV2.Core;
using ChristmasRandomizerV2.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace ChristmasRandomizerV2.ConsoleApp
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Program program = new Program(args);

            program.Execute();
        }

        /// <summary>
        /// Whether to send an email
        /// </summary>
        private bool _notify = false;
        
        /// <summary>
        /// Whether to log results to the logger
        /// </summary>
        private bool _log = false;

        /// <summary>
        /// Max number of retries before quitting
        /// </summary>
        private int _maxIterations = 20;

        internal Program(string[] args)
        {
            this.ParseArgs(args);
        }

        /// <summary>
        /// Parses command line args
        /// </summary>
        /// <param name="args"></param>
        /// <exception cref="ArgumentException"></exception>
        private void ParseArgs(string[] args)
        {
            if (args.Length < 1)
            {
                throw new ArgumentException($"Invalid number of args, need at least one");
            }

            if (args[0].Equals("notify", StringComparison.OrdinalIgnoreCase))
            {
                this._notify = true;
                this._log = false;
            }
            else if (args[0].Equals("log", StringComparison.OrdinalIgnoreCase))
            {
                this._notify = false;
                this._log = true;
            }
            else
            {
                throw new ArgumentException($"Argument should be either [notify] or [log]");
            }

            if (args.Length > 1)
            {
                foreach (string option in args)
                {
                    switch (option)
                    {
                        case "-l":
                        case "-L":
                            this._log = true;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Executes the cmdline app
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Execute()
        {
            ILogger logger = new Logger(this._log);

            ConfigLoader loader = new ConfigLoader(
                configFilePath: @"",
                lastYearsMappingFilePath: @"");

            Mapping result = new MappingManager(logger, this._maxIterations).Generate(loader.People, loader.Restrictions);

            if (result.NumMappings == 0)
            {
                logger.Log("Unable to build mapping");
                throw new Exception($"Unable to build mapping with given parameters");
            }

            if (this._notify)
            {
                result.Notify(loader);
            }

            if (this._log)
            {
                foreach (var m in result)
                {
                    logger.Log($"[{m.Key.Name}] -> [{m.Value.Name}]");
                }
            }
        }
    }
}
