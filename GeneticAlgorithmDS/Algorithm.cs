﻿using System;
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
            Console.WriteLine("Average fitness of the last population: " + finalFitnesses.Average());
            Console.WriteLine("Best fitness of the last population: " + finalFitnesses.Max());
            Console.WriteLine("Best individual of the last population: " + winnerIndividual.Item1);

            return currentPopulation.Select((individual, index) => new Tuple<Individual, double>(individual, finalFitnesses[index])).OrderByDescending(tuple => tuple.Item2).First().Item1;
        }

        private Individual createIndividual()
        {
            Individual newIndividual = new Individual(5, r.Next(0, 32));
            return newIndividual;
        }

        private double computeFitness(Individual individual)
        {
            var intToBits = individual.ToBinary();
            var fitness = -Math.Pow(intToBits, 2) + (7 * intToBits);

            return fitness;
        }

        private Func<Tuple<Individual, Individual>> selectTwoParents(Individual[] currentPopulation, double[] fitnesses)
        {
            var parents = new List<Individual>();
            var worstFitness = fitnesses.Min();
            fitnesses = fitnesses.Select(item => item + Math.Abs(worstFitness)).ToArray();
            var totalProbability = fitnesses.Sum();

            while (parents.Count < 2)
            {
                var randomvalue = new Random().NextDouble() * totalProbability;
                for(var j = 0; j < fitnesses.Length; j++)
                {
                    randomvalue -= fitnesses[j];
                    if (randomvalue <= 0)
                    {
                        parents.Add(currentPopulation[j]);
                        break;
                    }
                }
            }
            return () => Tuple.Create(parents[0], parents[1]);
        }

        private Tuple<Individual, Individual> crossover(Tuple<Individual, Individual> parents)
        {
            var offspring1 = parents.Item1;
            var offspring2 = parents.Item2;
            var tmp = parents.Item2;

            var singleCrossoverPoint = r.Next(0, offspring1._size);

            offspring1 = new Individual(offspring1.GetPartOfParent(0, singleCrossoverPoint).Merge(offspring2.GetPartOfParent(singleCrossoverPoint, offspring2._size)));
            offspring2 = new Individual(offspring2.GetPartOfParent(0, singleCrossoverPoint).Merge(tmp.GetPartOfParent(singleCrossoverPoint, tmp._size)));

            var newChildren = new Tuple<Individual, Individual>(offspring1, offspring2);
            return newChildren;
        }

        private Individual mutation(Individual individual, double mutationRate)
        {
            for (var i = 0; i < individual._size; i++)
            {
                if (r.NextDouble() < mutationRate)
                {
                    individual.SwitchPosition(i);
                }
            }
            return individual;
        }
    }
}
