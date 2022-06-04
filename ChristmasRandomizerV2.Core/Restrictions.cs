using System;
using System.Collections.Generic;
using System.Text;

namespace ChristmasRandomizerV2.Core
{
    public class Restrictions
    {
        /// <summary>
        /// Mappings indicating which Persons the given
        /// Person cannot be assigned to.
        /// </summary>
        private IDictionary<Person, ISet<Person>> invalidMappings;

        /// <summary>
        /// Mappings indicating which Person the given Person
        /// must be assigned to.
        /// </summary>
        private IDictionary<Person, Person> requiredMappings;

        /// <summary>
        /// Mappings indicating which Persons the given
        /// Person cannot be assigned to.
        /// </summary>
        public IDictionary<Person, ISet<Person>> InvalidMappings
        {
            get { return invalidMappings; }
        }

        /// <summary>
        /// Mappings indicating which Person the given Person
        /// must be assigned to.
        /// </summary>
        public IDictionary<Person, Person> RequiredMappings
        {
            get { return requiredMappings; }
        }

        public Restrictions(ISet<Person> people)
        {
            this.invalidMappings = new Dictionary<Person, ISet<Person>>();

            // each person starts with an empty set
            foreach (Person person in people)
            {
                this.invalidMappings.Add(person, new HashSet<Person>());
            }

            this.requiredMappings = new Dictionary<Person, Person>();
        }

        /// <summary>
        /// Given the current set of Persons and the Mapping
        /// from last year, add each of the mappings as a restriction
        /// </summary>
        /// <param name="people"></param>
        /// <param name="lastYearsMapping"></param>
        public void RestrictAll(ISet<Person> people, Mapping lastYearsMapping)
        {
            foreach (KeyValuePair<Person, Person> mapping in lastYearsMapping)
            {
                // If either person in this mapping is not a part
                // of this year, skip.
                if (!people.Contains(mapping.Key) ||
                    !people.Contains(mapping.Value))
                {
                    continue;
                }

                this.Restrict(mapping.Key, mapping.Value);
            }
        }

        /// <summary>
        /// Add a restriction such that the from person
        /// cannot have the to person.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Restrict(Person from, Person to)
        {
            // If we are trying to restrict a requirement, throw
            if (this.RequiredMappings.TryGetValue(from, out Person required) &&
                required.Equals(to))
            {
                throw new InvalidOperationException($"Person [{from.Name}] must be mapped to [{to.Name}] but attempted to add as restriction");
            }

            this.InvalidMappings[from].Add(to);
        }

        /// <summary>
        /// Add a requirement such that the from person
        /// must have the to person.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Require(Person from, Person to)
        {
            // If we already defined an invalid mapping, throw.
            if (this.InvalidMappings.TryGetValue(from, out ISet<Person> invalidList) &&
                invalidList.Contains(to))
            {
                throw new InvalidOperationException($"Person [{from.Name}] cannot be mapped to [{to.Name}] but attempted to add as requirement");
            }

            // If we already added a requirement for this 
            // mapping, throw.
            if (this.RequiredMappings.TryGetValue(from, out Person alreadyMapped))
            {
                throw new InvalidOperationException($"Person [{from.Name}] already has a required mapping to [{alreadyMapped.Name}], cannot add to [{to.Name}]");
            }

            this.RequiredMappings[from] = to;
        }
    }
}
