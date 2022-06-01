using Newtonsoft.Json;
using System;

namespace ChristmasRandomizerV2.Core
{
    public class Person
    {
        [JsonProperty()]
        public string Name { get; set; }

        [JsonProperty()]
        public string EmailAddress { get; set; }

        public Person()
        {

        }

        public Person(string name, string emailAddress)
        {
            this.Name = name;
            this.EmailAddress = emailAddress;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (obj.GetType() != typeof(Person)) return false;

            Person other = (Person)obj;

            return this.Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
