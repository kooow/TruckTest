using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TruckTest.Entities;

namespace TruckTest
{
    public class FileReader
    {
        private static string[] ReadInputFile(string inputFile)
        {
            string input_file_path = Path.Combine(AppContext.BaseDirectory, inputFile);
            if (!File.Exists(input_file_path))
            {
                throw new Exception("Error: missing input file: " + inputFile + " in this directory: " + AppContext.BaseDirectory);
            }

            Console.WriteLine("Loaded file:" + input_file_path);

            string[] input_file_content = File.ReadAllLines(input_file_path);
            if (input_file_content.Length == 0)
            {
                throw new Exception("Error: empty file - path: " + input_file_path);
            }
            else if (input_file_content.Length <= 4)
            {
                throw new Exception("Error: wrong input format! We need minimum four lines!");
            }

            return input_file_content;
        }

        public static FileData LoadAndCreateEntities(string inputFile)
        {
            string[] input_file_lines = ReadInputFile(inputFile);
            Console.WriteLine("Loaded lines from file: " + input_file_lines.Length);

            string number_of_vehicles_string = input_file_lines[0];
            uint number_of_vehicles = uint.Parse(number_of_vehicles_string);

            if (number_of_vehicles + 1 >= input_file_lines.Length)
            {
                throw new Exception("Error: wrong input format! Not enough lines!");
            }

            string number_of_jobs_string = input_file_lines[number_of_vehicles + 1];
            uint number_of_jobs = uint.Parse(number_of_jobs_string);

            if ((number_of_vehicles + number_of_jobs + 2) != input_file_lines.Length)
            {
                throw new Exception("Error: wrong input format! Not enough lines!");
            }

            var jobTypes = ReadJobTypesFromLines(number_of_vehicles, input_file_lines);
            Console.WriteLine("Processed job types: " + jobTypes.Count);

            var trucks = ReadTrucksFromLines(number_of_vehicles, input_file_lines);
            Console.WriteLine("Processed trucks: " + trucks.Count);

            ValidateDatas(jobTypes, trucks);

            FileData fileData = new FileData
            {
                Jobs = jobTypes,
                Trucks = trucks
            };

            return fileData;
        }

        private static List<Job> ReadJobTypesFromLines(uint numberOfVehicles, string[] inputFileLines)
        {
            List<Job> jobTypes = new List<Job>();

            for (uint i = numberOfVehicles + 2; i < inputFileLines.Length; i++)
            {
                var jobid_jobtype = inputFileLines[i].Split(' ');
                if (jobid_jobtype.Length != 2)
                {
                    throw new Exception("Error: wrong input format!");
                }

                int jobid = int.Parse(jobid_jobtype[0]);

                Job job_type = new Job
                {
                    Id = jobid,
                    Type = jobid_jobtype[1].FirstOrDefault()
                };
                jobTypes.Add(job_type);
            }

            return jobTypes;
        }

        private static List<Truck> ReadTrucksFromLines(uint numberOfVehicles, string[] inputFileLines)
        {
            List<Truck> trucks = new List<Truck>();

            for (int i = 1; i < numberOfVehicles + 1; i++)
            {         
                var truckid_compatibles = inputFileLines[i].Split(' ');
                if (truckid_compatibles.Length < 2)
                {
                    throw new Exception("Error: wrong input format!");
                }

                int truckid = int.Parse(truckid_compatibles[0]);

                List<char> compatible_job_types = new List<char>();

                for (int j = 1; j < truckid_compatibles.Length; j++)
                {
                    compatible_job_types.Add(truckid_compatibles[j].FirstOrDefault());
                }

                Truck newTruck = new Truck
                {
                    Id = truckid,
                    CompatibleJobTypes = compatible_job_types.ToArray()
                };
                trucks.Add(newTruck);
            }

            return trucks;
        }

        private static void ValidateDatas(List<Job> jobTypes, List<Truck> trucks)
        {
            var allJobTypes = jobTypes.Select(jt => jt.Type).Distinct().ToList();

            for (int i = 0; i < trucks.Count; i++)
            {
                for (int j = 0; j < trucks[i].CompatibleJobTypes.Length; j++)
                {
                    char jobtype_char = trucks[i].CompatibleJobTypes[j];

                    if (!allJobTypes.Contains(jobtype_char))
                    {
                        throw new Exception("Error: wrong input format! " + jobtype_char + " not found in " + allJobTypes.ToString());
                    }
                }
            }
        }

    }

}