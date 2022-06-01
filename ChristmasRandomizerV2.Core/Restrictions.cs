using System;
using System.Collections.Generic;
using System.Text;

namespace ChristmasRandomizerV2.Core
{
    public class Restrictions
    {
        private IDictionary<Person, ISet<Person>> invalidMappings;

        private IDictionary<Person, Person> requiredMappings;

        public IDictionary<Person, ISet<Person>> InvalidMappings
        {
            get { return invalidMappings; }
        }

        public IDictionary<Person, Person> RequiredMappings
        {
            get { return requiredMappings; }
        }

        public Restrictions(ISet<Person> people)
        {
            this.invalidMappings = new Dictionary<Person, ISet<Person>>();

            foreach (Person person in people)
            {
                this.invalidMappings.Add(person, new HashSet<Person>());
            }

            this.requiredMappings = new Dictionary<Person, Person>();
        }

        public void RestrictAll(ISet<Person> people, Mapping lastYearsMapping)
        {
            foreach (KeyValuePair<Person, Person> mapping in lastYearsMapping)
            {
                if (!people.Contains(mapping.Key) ||
                    !people.Contains(mapping.Value))
                {
                    continue;
                }

                this.Restrict(mapping.Key, mapping.Value);
            }
        }

        public void Restrict(Person from, Person to)
        {
            if (this.RequiredMappings.TryGetValue(from, out Person required) &&
                required.Equals(to))
            {
                throw new InvalidOperationException($"Person [{from.Name}] must be mapped to [{to.Name}] but attempted to add as restriction");
            }

            this.InvalidMappings[from].Add(to);
        }

        public void Require(Person from, Person to)
        {
            if (this.InvalidMappings.TryGetValue(from, out ISet<Person> invalidList) &&
                invalidList.Contains(to))
            {
                throw new InvalidOperationException($"Person [{from.Name}] cannot be mapped to [{to.Name}] but attempted to add as requirement");
            }

            if (this.RequiredMappings.TryGetValue(from, out Person alreadyMapped))
            {
                throw new InvalidOperationException($"Person [{from.Name}] already has a required mapping to [{alreadyMapped.Name}], cannot add to [{to.Name}]");
            }

            this.RequiredMappings[from] = to;
        }
    }
}
