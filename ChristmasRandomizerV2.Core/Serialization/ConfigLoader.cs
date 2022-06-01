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
        private Mapping LastYearMapping { get; set; }

        public Restrictions Restrictions { get; private set; }

        public ISet<Person> People { get; private set; }

        internal ConfigEmail EmailConfig { get; private set; }

        private Dictionary<string, Person> _nameMapping;

        public ConfigLoader(
            string configFilePath, 
            string lastYearsMappingFilePath)
        {
            this.LoadConfigFile(configFilePath);

            if (!string.IsNullOrEmpty(lastYearsMappingFilePath))
            {
                this.LoadLastYearConfig(lastYearsMappingFilePath);
                this.Restrictions.RestrictAll(this.People, this.LastYearMapping);
            }
        }

        private void LoadConfigFile(string filePath)
        {
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
