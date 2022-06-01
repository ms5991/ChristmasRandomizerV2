using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChristmasRandomizerV2.Core
{
    public class MappingManager
    {
        private ILogger _logger;
        private int _maxIterations;

        public MappingManager(
            ILogger logger,
            int maxIterations = 20)
        {
            this._logger = logger;
            this._maxIterations = maxIterations;
        }

        public Mapping Generate(
            ISet<Person> people,
            Restrictions restrictions)
        {
            Mapping result = null;
            int attempt = 0;

            Random random = new Random();

            ISet<Person> needAssignment = new HashSet<Person>(people);
            ISet<Person> validToAssign = new HashSet<Person>(people);

            foreach (KeyValuePair<Person, Person> requiredMapping in restrictions.RequiredMappings)
            {
                this._logger.Log($"Adding required mapping from [{requiredMapping.Key.Name}] to [{requiredMapping.Value.Name}]");
                result.Add(requiredMapping.Key, requiredMapping.Value);

                needAssignment.Remove(requiredMapping.Key);
                validToAssign.Remove(requiredMapping.Value);
            }

            bool success = false;

            while (!success && attempt < this._maxIterations)
            {
                result = new Mapping();

                Queue<Person> assignmentPool = new Queue<Person>(validToAssign.OrderBy(s => random.Next()));

                foreach (Person toAssign in needAssignment)
                {
                    int numToCheck = assignmentPool.Count;

                    bool assigned = false;

                    // check numToCheck things from the queue,
                    // i.e. check everything in the queue once
                    // and if none of them work, start over
                    for (int i = 0; i < numToCheck; i++)
                    {
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
                            result.Add(toAssign, toTest);
                            this._logger.Log($"Assigning [{toAssign.Name}] to [{toTest.Name}]");

                            assigned = true;

                            break;
                        }
                    }

                    if (!assigned)
                    {
                        this._logger.Log($"Could not make assignment for [{toAssign.Name}]");
                        break;
                    }
                }

                if (result.NumMappings == people.Count)
                {
                    success = true;
                }
                else
                {
                    this._logger.Log($"Unsuccessful attempt on try [{attempt + 1}]/[{this._maxIterations}]");
                    result = null;
                }

                attempt++;
            }


            return result;
        }

    }
}
