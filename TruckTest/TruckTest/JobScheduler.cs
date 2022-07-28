using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TruckTest.Entities;

namespace TruckTest
{

    public class JobScheduler
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileData"></param>
        public static void CalculateResultsAndWriteToFile(FileData fileData)
        {
            var jobs = fileData.Jobs;
            var trucks = fileData.Trucks;

            int maximumCompatibleJobTypeListSize = trucks.Max(t => t.CompatibleJobTypes.Length);
            Console.WriteLine("Truck with compatible job type list - Maximum length of list: " + maximumCompatibleJobTypeListSize);


            List<Result> results_without_repeat = CalculateWithoutRepeatableTrucks(jobs, trucks, maximumCompatibleJobTypeListSize);

            Console.WriteLine("Results (without repeatable trucks):" + results_without_repeat.Count);

            if (results_without_repeat.Count > 0 && results_without_repeat.Count <= 2)
            {
                Console.WriteLine("");
                WriteResultToFile(results_without_repeat[0], "output_without_repeat.txt");
                Console.WriteLine("");
            }


            int maximumRepeat = 1;

            List<Result> results_with_repeat = CalculateRepeatableTrucks(jobs, trucks, maximumRepeat, maximumCompatibleJobTypeListSize);

            Console.WriteLine("Results with repeatable trucks (Maximum repeat:" + maximumRepeat + ") " + results_with_repeat.Count);

            if (results_with_repeat.Count > 0 && results_with_repeat.Count <= 2)
            {
                Console.WriteLine("");
                WriteResultToFile(results_with_repeat[0], "output.txt");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="outputFileName"></param>
        private static void WriteResultToFile(Result result, string outputFileName)
        {
            string resultTest = result.PrintToText();

            File.WriteAllText(outputFileName, resultTest);

            Console.WriteLine("Result (" + result.TruckId_JobId_List.Count.ToString() + " line) is written to file ");
            Console.WriteLine(AppContext.BaseDirectory + "\\" + outputFileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobs"></param>
        /// <param name="trucks"></param>
        /// <param name="maximumRepeat"></param>
        /// <param name="maximumCompatibleJobTypeListSize"></param>
        /// <returns></returns>
        private static List<Result> CalculateRepeatableTrucks(List<Job> jobs, List<Truck> trucks, int maximumRepeat, int maximumCompatibleJobTypeListSize)
        {
            List<Result> result_list = new List<Result>();
            var first_result = new Result();
            first_result.FillTruckIdRepeatUseList(trucks);
            result_list.Add(first_result);

            for (int j = 0; j < jobs.Count; j++)
            {
                List<Result> newBranches = new List<Result>();

                var job = jobs[j];

                foreach (Result result in result_list)
                {
                    var available_truck_ids = GetAvailableTruckIdsByJobTypeWithRepeat(maximumCompatibleJobTypeListSize, trucks, result, maximumRepeat, job.Type);

                    if (available_truck_ids.Count == 0) // dead branch
                    {
                        result.Abandoned = true;
                    }
                    else if (available_truck_ids.Count == 1) // we have only one option, we move on
                    {
                        var searched_id = available_truck_ids.First();
                        var truck = trucks.Single(t => t.Id == searched_id);
                        result.AddPlusRepeatedUse(truck.Id);

                        result.TruckId_JobId_List.Add(new KeyValuePair<int, int>(truck.Id, job.Id));
                    }
                    else // There are many options so we need to generate new branches with the previously used truck_id and job_id pairs
                    {
                        foreach (int truckid in available_truck_ids)
                        {
                            var truck = trucks.Single(t => t.Id == truckid);
                            result.AddPlusRepeatedUse(truck.Id);

                            var resultClone = result.Clone() as Result;
                            resultClone.TruckId_JobId_List.Add(new KeyValuePair<int, int>(truckid, job.Id));
                            newBranches.Add(resultClone);
                        }
                        result.Abandoned = true;
                    }

                }

            }

            return result_list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobs"></param>
        /// <param name="trucks"></param>
        /// <param name="maximumCompatibleJobTypeListSize"></param>
        /// <returns></returns>
        private static List<Result> CalculateWithoutRepeatableTrucks(List<Job> jobs, List<Truck> trucks, int maximumCompatibleJobTypeListSize)
        {
            List<Result> result_list = new List<Result>();

            result_list.Add(new Result());

            for (int j = 0; j < jobs.Count; j++)
            {
                List<Result> newBranches = new List<Result>();

                var job = jobs[j];

                foreach (Result result in result_list)
                {
                    var previously_used_truck_ids = result.GetPreviouslyUsedTruckIds();
                    var available_truck_ids = GetAvailableTruckIdsByJobTypeWithoutRepeat(maximumCompatibleJobTypeListSize, trucks, previously_used_truck_ids, job.Type);

                    if (available_truck_ids.Count == 0) // dead branch
                    {
                        result.Abandoned = true;
                    }
                    else if (available_truck_ids.Count == 1) // we have only one option, we move on
                    {
                        var searched_id = available_truck_ids.First();
                        var truck = trucks.Single(t => t.Id == searched_id);

                        result.TruckId_JobId_List.Add(new KeyValuePair<int, int>(truck.Id, job.Id));
                    }
                    else // There are many options so we need to generate new branches with the previously used truck_id and job_id pairs
                    {
                        foreach (int truckid in available_truck_ids)
                        {
                            var resultClone = result.Clone() as Result;
                            resultClone.TruckId_JobId_List.Add(new KeyValuePair<int, int>(truckid, job.Id));
                            newBranches.Add(resultClone);
                        }
                        result.Abandoned = true;
                    }
                }

                if (newBranches.Count > 0)
                {
                    result_list.AddRange(newBranches);
                }

                result_list = result_list.Where(r => r.Abandoned == false).ToList();
            }

            return result_list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maximumCompatibleJobTypeListSize"></param>
        /// <param name="trucks"></param>
        /// <param name="result"></param>
        /// <param name="maximumRepeat"></param>
        /// <param name="searchedJobType"></param>
        /// <returns></returns>
        private static List<int> GetAvailableTruckIdsByJobTypeWithRepeat(int maximumCompatibleJobTypeListSize, List<Truck> trucks, Result result, int maximumRepeat, char searchedJobType)
        {

            for (int max_size = 1; max_size <= maximumCompatibleJobTypeListSize; max_size++)
            {
                for (int repeat = 0; repeat <= maximumRepeat; repeat++)
                {
                    List<int> truck_ids_with_repeat_number = new List<int>();

                    var truckids_with_repeat = result.GetTruckIdsByRepeatedUse(repeat);

                    var first_available_truck_with_this_size = trucks.FirstOrDefault(t => t.CompatibleJobTypes.Length == max_size && t.CompatibleJobTypes.Contains(searchedJobType)
                                                                                         && truckids_with_repeat.Contains(t.Id));
                    if (first_available_truck_with_this_size != null)
                    {
                        return new List<int> { first_available_truck_with_this_size.Id };
                    }
                }
            }

            return new List<int>();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maximumCompatibleJobTypeListSize"></param>
        /// <param name="trucks"></param>
        /// <param name="previouslyUsedTruckIds"></param>
        /// <param name="searchedJobType"></param>
        /// <returns></returns>
        private static List<int> GetAvailableTruckIdsByJobTypeWithoutRepeat(int maximumCompatibleJobTypeListSize, List<Truck> trucks, List<int> previouslyUsedTruckIds, char searchedJobType)
        {
            for (int max_size = 1; max_size <= maximumCompatibleJobTypeListSize; max_size++)
            {
                var first_available_truck_with_this_size = trucks.FirstOrDefault(t => previouslyUsedTruckIds.Contains(t.Id) == false &&
                                        t.CompatibleJobTypes.Length == max_size && t.CompatibleJobTypes.Contains(searchedJobType));
                if (first_available_truck_with_this_size != null)
                {
                    return new List<int> { first_available_truck_with_this_size.Id };
                }
            }

            return new List<int>();
        }

    }

}