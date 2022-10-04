using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TruckTest.Entities;

namespace TruckTest
{
    public class FileReader : IFileReader
    {
        private string[] ReadInputFile(string inputFile)
        {
            string inputFilePath = Path.Combine(AppContext.BaseDirectory, inputFile);
            if (!File.Exists(inputFilePath))
            {
                throw new Exception("Error: missing input file: " + inputFile + " in this directory: " + AppContext.BaseDirectory);
            }

            Console.WriteLine("Loaded file:" + inputFilePath);

            string[] inputFileContent = File.ReadAllLines(inputFilePath);
            if (inputFileContent.Length == 0)
            {
                throw new Exception("Error: empty file - path: " + inputFilePath);
            }
            else if (inputFileContent.Length <= 4)
            {
                throw new Exception("Error: wrong input format! We need minimum four lines!");
            }

            return inputFileContent;
        }

        public FileData LoadAndCreateEntities(string inputFile)
        {
            string[] inputFileLines = ReadInputFile(inputFile);
            Console.WriteLine("Loaded lines from file: " + inputFileLines.Length);

            string numberOfVehiclesString = inputFileLines[0];
            uint numberOfVehicles = uint.Parse(numberOfVehiclesString);

            if (numberOfVehicles + 1 >= inputFileLines.Length)
            {
                throw new Exception("Error: wrong input format! Not enough lines!");
            }

            string numberOfJobsString = inputFileLines[numberOfVehicles + 1];
            uint numberOfJobs = uint.Parse(numberOfJobsString);

            if ((numberOfVehicles + numberOfJobs + 2) != inputFileLines.Length)
            {
                throw new Exception("Error: wrong input format! Not enough lines!");
            }

            var jobs = ReadJobTypesFromLines(numberOfVehicles, inputFileLines);
            Console.WriteLine("Processed job types: " + jobs.Count);

            var trucks = ReadTrucksFromLines(numberOfVehicles, inputFileLines);
            Console.WriteLine("Processed trucks: " + trucks.Count);

            ValidateDatas(jobs, trucks);

            FileData fileData = new FileData(trucks, jobs);

            return fileData;
        }

        private List<Job> ReadJobTypesFromLines(uint numberOfVehicles, string[] inputFileLines)
        {
            List<Job> jobTypes = new List<Job>();

            for (uint i = numberOfVehicles + 2; i < inputFileLines.Length; i++)
            {
                var jobIdAndJobType = inputFileLines[i].Split(' ');
                if (jobIdAndJobType.Length != 2)
                {
                    throw new Exception("Error: wrong input format!");
                }

                int jobId = int.Parse(jobIdAndJobType[0]);

                Job jobType = new Job
                {
                    Id = jobId,
                    Type = jobIdAndJobType[1].FirstOrDefault()
                };
                jobTypes.Add(jobType);
            }

            return jobTypes;
        }

        private List<Truck> ReadTrucksFromLines(uint numberOfVehicles, string[] inputFileLines)
        {
            List<Truck> trucks = new List<Truck>();

            for (int i = 1; i < numberOfVehicles + 1; i++)
            {         
                var truckIdCompatibles = inputFileLines[i].Split(' ');
                if (truckIdCompatibles.Length < 2)
                {
                    throw new Exception("Error: wrong input format!");
                }

                int truckid = int.Parse(truckIdCompatibles[0]);

                List<char> compatibleJobTypes = new List<char>();

                for (int j = 1; j < truckIdCompatibles.Length; j++)
                {
                    compatibleJobTypes.Add(truckIdCompatibles[j].FirstOrDefault());
                }

                Truck newTruck = new Truck
                {
                    Id = truckid,
                    CompatibleJobTypes = compatibleJobTypes.ToArray()
                };
                trucks.Add(newTruck);
            }

            return trucks;
        }

        private void ValidateDatas(List<Job> jobTypes, List<Truck> trucks)
        {
            var allJobTypes = jobTypes.Select(jt => jt.Type).Distinct().ToList();

            for (int i = 0; i < trucks.Count; i++)
            {
                for (int j = 0; j < trucks[i].CompatibleJobTypes.Length; j++)
                {
                    char jobTypeChar = trucks[i].CompatibleJobTypes[j];

                    if (!allJobTypes.Contains(jobTypeChar))
                    {
                        throw new Exception("Error: wrong input format! " + jobTypeChar + " not found in " + allJobTypes.ToString());
                    }
                }
            }
        }

    }

}