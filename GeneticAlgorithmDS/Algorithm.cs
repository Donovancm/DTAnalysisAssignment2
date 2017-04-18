using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmDS
{
    public class Algorithm
    {
        double crossoverRate;
        double mutationRate;
        bool elitism;
        int populationSize;
        int numIterations;
        Random r = new Random();

        public Algorithm(int populationSize, double crossoverRate, double mutationRate, int numIterations, bool elitism)
        {
            this.crossoverRate = crossoverRate;
            this.mutationRate = mutationRate;
            this.elitism = elitism;
            this.populationSize = populationSize;
            this.numIterations = numIterations;
        }
        public Individual Run()
        {
            // initialize the first population
            var initialPopulation = Enumerable.Range(0, populationSize).Select(i => createIndividual()).ToArray();

            var currentPopulation = initialPopulation;

            for (int generation = 0; generation < numIterations; generation++)
            {
                // compute fitness of each individual in the population
                var fitnesses = Enumerable.Range(0, populationSize).Select(i => computeFitness(currentPopulation[i])).ToArray();

                var nextPopulation = new Individual[populationSize];

                // apply elitism
                int startIndex;
                if (elitism)
                {
                    startIndex = 1;
                    var populationWithFitness = currentPopulation.Select((individual, index) => new Tuple<Individual, double>(individual, fitnesses[index]));
                    var populationSorted = populationWithFitness.OrderByDescending(tuple => tuple.Item2); // item2 is the fitness
                    var bestIndividual = populationSorted.First();
                    nextPopulation[0] = bestIndividual.Item1;
                }
                else
                {
                    startIndex = 0;
                }

                // initialize the selection function given the current individuals and their fitnesses
                var getTwoParents = selectTwoParents(currentPopulation, fitnesses);

                // create the individuals of the next generation
                for (int newInd = startIndex; newInd < populationSize; newInd++)
                {
                    // select two parents
                    var parents = getTwoParents();

                    // do a crossover between the selected parents to generate two children (with a certain probability, crossover does not happen and the two parents are kept unchanged)
                    Tuple<Individual, Individual> offspring;
                    if (r.NextDouble() < crossoverRate)
                        offspring = crossover(parents);
                    else
                        offspring = parents;

                    // save the two children in the next population (after mutation)
                    nextPopulation[newInd++] = mutation(offspring.Item1, mutationRate);
                    if (newInd < populationSize) //there is still space for the second children inside the population
                        nextPopulation[newInd] = mutation(offspring.Item2, mutationRate);
                }

                // the new population becomes the current one
                currentPopulation = nextPopulation;

                // in case it's needed, check here some convergence condition to terminate the generations loop earlier
            }

            // recompute the fitnesses on the final population and return the best individual
            var finalFitnesses = Enumerable.Range(0, populationSize).Select(i => computeFitness(currentPopulation[i])).ToArray();
            var winnerIndividual = currentPopulation.Select((individual, index) => new Tuple<Individual, double>(individual, finalFitnesses[index])).OrderByDescending(tuple => tuple.Item2).First();

            Console.WriteLine("--------------------------------");
            Console.WriteLine("Genetic Algorithm completed");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Average fitness:" + finalFitnesses.Average());
            Console.WriteLine("Fitness of the best individual:" + winnerIndividual.Item2);
            Console.WriteLine("Best individual:" + winnerIndividual.Item1 + " " + winnerIndividual.Item1.ToString().BinaryConvert(2));

            return currentPopulation.Select((individual, index) => new Tuple<Individual, double>(individual, finalFitnesses[index])).OrderByDescending(tuple => tuple.Item2).First().Item1;
        }
        private Individual createIndividual()
        {
            return new Individual(5, r.Next(0, 32));
        }
        private double computeFitness(Individual individual)
        {
            var value = individual.ToInt();
            return -Math.Pow(value, 2) + 7 * value;
        }
        private Func<Tuple<Individual, Individual>> selectTwoParents(Individual[] individuals, double[] fitnesses)
        {
            var parents = new List<Individual>();
            var FitnessTotal = fitnesses.Sum();

            while (parents.Count < 2)
            {
                var randomvalue = new Random().NextDouble() * FitnessTotal;
                for(var j = 0; j < fitnesses.Length; j++)
                {
                    randomvalue -= fitnesses[j];
                    if (randomvalue <= 0)
                    {
                        parents.Add(individuals[j]);
                    }
                }
            }
            return () => Tuple.Create(parents[0], parents[1]);
        }

        private Tuple<Individual, Individual> crossover(Tuple<Individual, Individual> parents)
        {
            var par1 = parents.Item1;
            var par2 = parents.Item2;
            var tmp = parents.Item2;

            var crossoverPoint = r.Next(0, par1._size);

            par1 = new Individual(par1.GetPart(0, crossoverPoint).Merge(par2.GetPart(crossoverPoint, par2._size)));
            par2 = new Individual(par2.GetPart(0, crossoverPoint).Merge(tmp.GetPart(crossoverPoint, par1._size)));

            return Tuple.Create(par1, par2);
        }

        private Individual mutation(Individual individual, double mutationRate)
        {
            for (var i = 0; i < individual._size; i++)
            {
                if (r.Next(100) < mutationRate)
                {
                    individual.Switch(i);
                }
            }
            return individual;
        }
    }
}
