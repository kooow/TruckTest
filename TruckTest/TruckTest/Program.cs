using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using TruckTest.Entities;

namespace TruckTest
{
    class Program
    {

        private static readonly string s_inputFile = "_input.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("Base dir: " + AppContext.BaseDirectory);

            IFileReader fileReader = new FileReader();
            var fileData = fileReader.LoadAndCreateEntities(s_inputFile);

            IJobScheduler jobScheduler = new JobScheduler();

            var resultsWithoutRepeatableTrucks = jobScheduler.CalculateResultsWithoutRepeatableTrucks(fileData);
            jobScheduler.WriteResultToFile(resultsWithoutRepeatableTrucks, "output_without_repeat.txt");

            int maximumRepeat = 1;

            var resultsWithRepeatable = jobScheduler.CalculateResultsWithRepeatable(fileData, maximumRepeat);
            jobScheduler.WriteResultToFile(resultsWithRepeatable, "output.txt");

            Console.ReadKey();
        }

    }
}
