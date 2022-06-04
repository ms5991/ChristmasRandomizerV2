using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChristmasRandomizerV2.Core
{
    public class MappingManager
    {
        private static Random random = new Random();

        private ILogger _logger;
        private int _maxIterations;

        public MappingManager(
            ILogger logger,
            int maxIterations = 20)
        {
            this._logger = logger;
            this._maxIterations = maxIterations;
        }

        /// <summary>
        /// Generate a Mapping given the set of people
        /// and the restrictions
        /// </summary>
        /// <param name="people"></param>
        /// <param name="restrictions"></param>
        /// <returns></returns>
        public Mapping Generate(
            ISet<Person> people,
            Restrictions restrictions)
        {
            // the result to be returned
            Mapping result = new Mapping();

            // indicates which people need assignment after processing
            // the required mappings
            ISet<Person> needAssignment = new HashSet<Person>(people);

            // Indicates which people can be assigned to other people
            // after processing required mappings
            ISet<Person> validToAssign = new HashSet<Person>(people);

            // Process required mappings
            foreach (KeyValuePair<Person, Person> requiredMapping in restrictions.RequiredMappings)
            {
                this._logger.Log($"Adding required mapping from [{requiredMapping.Key.Name}] to [{requiredMapping.Value.Name}]");
                result.Add(requiredMapping.Key, requiredMapping.Value);

                // update the other sets
                needAssignment.Remove(requiredMapping.Key);
                validToAssign.Remove(requiredMapping.Value);
            }

            bool success = false;
            int attempt = 0;

            // Continue until we've made valid assignments for 
            // everyone or until we run out of attempt iterations
            while (!success && attempt < this._maxIterations)
            {
                // clear the results since we're starting from scratch.
                result.Clear();

                // Create a queue that contains all of the valid
                // assignments in a random order
                Queue<Person> assignmentPool = new Queue<Person>(validToAssign.OrderBy(s => random.Next()));

                // Assign someone for each person
                foreach (Person toAssign in needAssignment)
                {
                    // check each Person from the queue if necessary
                    int numToCheck = assignmentPool.Count;

                    bool assigned = false;

                    // check numToCheck things from the queue,
                    // i.e. check everything in the queue once
                    // and if none of them work, start over
                    for (int i = 0; i < numToCheck; i++)
                    {
                        // Get a person to check
                        Person toTest = assignmentPool.Dequeue();

                        if (toTest.Equals(toAssign))
                        {
                            // cannot assign self
                            assignmentPool.Enqueue(toTest);
                            this._logger.Log($"Could not assign [{toAssign.Name}] to self");

                        }
                        else if (restrictions.InvalidMappings[toAssign].Contains(toTest))
                        {
                            // cannot assign restriction
                            assignmentPool.Enqueue(toTest);
                            this._logger.Log($"Could not assign [{toAssign.Name}] to restricted person [{toTest.Name}]");
                        }
                        else
                        {
                            // This Person is valid, so go ahead and assign
                            result.Add(toAssign, toTest);
                            this._logger.Log($"Assigning [{toAssign.Name}] to [{toTest.Name}]");

                            assigned = true;

                            // no need to check rest of queue
                            break;
                        }
                    }

                    // If we checked everyone and were unable to assign,
                    // break out of the foreach loop and start over.
                    if (!assigned)
                    {
                        this._logger.Log($"Could not make assignment for [{toAssign.Name}]");
                        break;
                    }
                }

                // If we assigned everyone, we're done.
                if (result.NumMappings == people.Count)
                {
                    success = true;
                }
                else
                {
                    this._logger.Log($"Unsuccessful attempt on try [{attempt + 1}]/[{this._maxIterations}]");
                    result.Clear();
                }

                attempt++;
            }

            return result;
        }
    }
}
