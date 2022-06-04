using ChristmasRandomizerV2.Core.Email;
using ChristmasRandomizerV2.Core.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChristmasRandomizerV2.Core
{
    public class Mapping : IEnumerable<KeyValuePair<Person, Person>>
    {
        /// <summary>
        /// The actual mapping data.
        /// </summary>
        private IDictionary<Person, Person> mappingData { get; set; }

        /// <summary>
        /// Indicates how many mappings we have added.
        /// </summary>
        public int NumMappings
        {
            get { return mappingData.Count; }
        }

        public Mapping()
        {
            this.mappingData =  new Dictionary<Person, Person>(); 
        }

        /// <summary>
        /// Add the given mapping to this Mapping.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void Add(Person from, Person to)
        {
            this.mappingData[from] = to;
        }

        /// <summary>
        /// Get the mapping for the given person.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public Person Get(Person from)
        {
            return this.mappingData[from];
        }

        /// <summary>
        /// Enumerate.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<Person, Person>> GetEnumerator()
        {
            return this.mappingData.GetEnumerator();
        }

        /// <summary>
        /// Enumerate.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.mappingData.GetEnumerator();
        }

        /// <summary>
        /// Clear this mapping to start from scratch.
        /// </summary>
        public void Clear()
        {
            this.mappingData.Clear();
        }

        /// <summary>
        /// Write this mapping to a json file at the given
        /// file path.
        /// </summary>
        /// <param name="fileName"></param>
        public void SerializeToFile(string fileName)
        {
            ConfigMapping toFile = new ConfigMapping() { Mapping = new List<ConfigPersonHas>(this.mappingData.Count) };

            foreach (KeyValuePair<Person, Person> map in this.mappingData)
            {
                toFile.Mapping.Add(new ConfigPersonHas() { Person = map.Key.Name, Has = map.Value.Name });
            }

            File.WriteAllText(fileName, JsonConvert.SerializeObject(toFile));
        }

        /// <summary>
        /// Send email to each of the people regarding
        /// their mappings
        /// </summary>
        /// <param name="configs"></param>
        public void Notify(ConfigLoader configs)
        {
            using (EmailManager emailManager = new EmailManager(
                configs.EmailConfig.Username, 
                configs.EmailConfig.Password,
                configs.EmailConfig.SmtpServer,
                configs.EmailConfig.Port))
            {
                foreach (KeyValuePair<Person, Person> map in this.mappingData)
                {
                    emailManager.Notify(map.Key, map.Value);
                }
            }
        }
    }
}
