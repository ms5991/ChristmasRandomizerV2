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
            bool notify = false;
            if (args.Length == 1)
            {
                notify = string.Equals("notify", args[0], StringComparison.OrdinalIgnoreCase);
            }

            ConfigLoader loader = new ConfigLoader(
                configFilePath: @"C:\Users\ms599\OneDrive\Documents\Home and Life\Christmas\V2\config.json",
                lastYearsMappingFilePath: @"C:\Users\ms599\OneDrive\Documents\Home and Life\Christmas\V2\2021.json");

            Mapping result = new MappingManager(new Logger()).Generate(loader.People, loader.Restrictions);

            if (result == null)
            {
                Console.WriteLine("Unable to build mapping");
            }
            else
            {
                if (notify)
                {
                    result.Notify(loader);
                }
                else
                {
                    foreach (var m in result)
                    {
                        Console.WriteLine($"[{m.Key.Name}] -> [{m.Value.Name}]");
                    }
                }
            }
        }

    }
}
