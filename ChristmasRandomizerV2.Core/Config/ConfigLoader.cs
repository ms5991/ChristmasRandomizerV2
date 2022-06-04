using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ChristmasRandomizerV2.Core.Serialization
{
    public class ConfigLoader
    {
        /// <summary>
        /// Mapping used last year
        /// </summary>
        private Mapping LastYearMapping { get; set; }

        /// <summary>
        /// Restrictions loaded from file
        /// </summary>
        public Restrictions Restrictions { get; private set; }

        /// <summary>
        /// List of people loaded from file
        /// </summary>
        public ISet<Person> People { get; private set; }

        /// <summary>
        /// Config for sending email loaded from file
        /// </summary>
        internal ConfigEmail EmailConfig { get; private set; }

        /// <summary>
        /// Map of string name to Person object
        /// </summary>
        private Dictionary<string, Person> _nameMapping;

        public ConfigLoader(
            string configFilePath, 
            string lastYearsMappingFilePath)
        {
            // first load the config file
            this.LoadConfigFile(configFilePath);

            // Load last year's mapping and add restrictions
            // if necessary
            if (!string.IsNullOrEmpty(lastYearsMappingFilePath))
            {
                this.LoadLastYearConfig(lastYearsMappingFilePath);
                this.Restrictions.RestrictAll(this.People, this.LastYearMapping);
            }
        }

        /// <summary>
        /// Loads the config file
        /// </summary>
        /// <param name="filePath"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private void LoadConfigFile(string filePath)
        {
            // deserialize the config file
            Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(filePath));

            this.EmailConfig = config.Email;

            this._nameMapping = new Dictionary<string, Person>(config.Names.Count);

            foreach (ConfigPerson p in config.Names)
            {
                if (this._nameMapping.ContainsKey(p.Name))
                {
                    throw new InvalidOperationException($"Config file [{filePath}] has duplicate person [{p.Name}]");
                }

                this._nameMapping.Add(p.Name, new Person(p.Name, p.Email));
            }

            this.People = this._nameMapping.Values.ToHashSet();

            this.Restrictions = new Restrictions(this.People);

            foreach (ConfigRestriction restriction in config.Restrictions)
            {
                this.Restrictions.Restrict(this._nameMapping[restriction.Person], this._nameMapping[restriction.CannotHave]);
            }
        }

        /// <summary>
        /// Load last year's config file.
        /// </summary>
        /// <param name="lastYearConfigFilePath"></param>
        private void LoadLastYearConfig(string lastYearConfigFilePath)
        {
            ConfigMapping lastYear = JsonConvert.DeserializeObject<ConfigMapping>(File.ReadAllText(lastYearConfigFilePath));

            this.LastYearMapping = new Mapping();
            foreach (ConfigPersonHas map in lastYear.Mapping)
            {
                if (!this._nameMapping.ContainsKey(map.Person) || !this._nameMapping.ContainsKey(map.Has))
                {
                    continue;
                }

                this.LastYearMapping.Add(this._nameMapping[map.Person], this._nameMapping[map.Has]);
            }
        }
    }
}
