using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmDS
{
    class Program
    {
        private static int populationSize;
        private static double crossoverRate;
        private static double mutationRate;
        private static int iterations;
        private static bool elitism;
        private static Random r = new Random();

        static void Main(string[] args)
        {
            /* FUNCTIONS TO DEFINE (for each problem):
            Func<Ind> createIndividual;                                 ==> input is nothing, output is a new individual
            Func<Ind,double> computeFitness;                            ==> input is one individual, output is its fitness
            Func<Ind[],double[],Func<Tuple<Ind,Ind>>> selectTwoParents; ==> input is an array of individuals (population) and an array of corresponding fitnesses, output is a function which (without any input) returns a tuple with two individuals (parents)
            Func<Tuple<Ind, Ind>, Tuple<Ind, Ind>> crossover;           ==> input is a tuple with two individuals (parents), output is a tuple with two individuals (offspring/children)
            Func<Ind, double, Ind> mutation;                            ==> input is one individual and mutation rate, output is the mutated individual
            */

            getUserInput();

            var algorithm = new Algorithm(populationSize,crossoverRate,mutationRate,iterations,elitism); // CHANGE THE GENERIC TYPE (NOW IT'S INT AS AN EXAMPLE) AND THE PARAMETERS VALUES
            var solution = algorithm.Run();
            Console.WriteLine("Solution: ");
            Console.WriteLine(solution);
            Console.ReadLine();
        }

        public static void getUserInput()
        {
            Console.WriteLine("Specify the values for Population (20-50), Crossover Rate (0.8 - 0.95), Mutation Rate (0.01 - 0.1), Number of Iterations and Elitism (true / false)");
            var userInput = Console.ReadLine();

            try
            {
                var splitUserInput = userInput.Split(',');

                if (splitUserInput == null)
                {
                    throw new Exception("You didn't fill in all fields");
                }

                populationSize = int.Parse(splitUserInput[0]);
                crossoverRate = double.Parse(splitUserInput[1]);
                mutationRate = double.Parse(splitUserInput[2]);
                iterations = int.Parse(splitUserInput[3]);
                elitism = bool.Parse(splitUserInput[4]);
            }
            catch (Exception)
            {
                Console.WriteLine("Your input is invalid. You can try again");
                getUserInput();
            }
        }
    }
}
