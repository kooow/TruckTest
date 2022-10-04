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

            var fileData = FileReader.LoadAndCreateEntities(s_inputFile);

            JobScheduler.CalculateResultsAndWriteToFile(fileData);
     
            Console.ReadKey();
        }

    }
}
