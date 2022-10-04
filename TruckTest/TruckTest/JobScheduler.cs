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


            List<Result> resultsWithoutRepeat = CalculateWithoutRepeatableTrucks(jobs, trucks, maximumCompatibleJobTypeListSize);

            Console.WriteLine("Results (without repeatable trucks):" + resultsWithoutRepeat.Count);

            if (resultsWithoutRepeat.Count > 0 && resultsWithoutRepeat.Count <= 2)
            {
                Console.WriteLine("");
                WriteResultToFile(resultsWithoutRepeat[0], "output_without_repeat.txt");
                Console.WriteLine("");
            }


            int maximumRepeat = 1;

            List<Result> resultsWithRepeat = CalculateRepeatableTrucks(jobs, trucks, maximumRepeat, maximumCompatibleJobTypeListSize);

            Console.WriteLine("Results with repeatable trucks (Maximum repeat:" + maximumRepeat + ") " + resultsWithRepeat.Count);

            if (resultsWithRepeat.Count > 0 && resultsWithRepeat.Count <= 2)
            {
                Console.WriteLine("");
                WriteResultToFile(resultsWithRepeat[0], "output.txt");
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

            Console.WriteLine("Result (" + result.TruckIdJobIdList.Count.ToString() + " line) is written to file ");
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
            List<Result> resultList = new List<Result>();
            var firstResult = new Result();
            firstResult.FillTruckIdRepeatUseList(trucks);
            resultList.Add(firstResult);

            for (int j = 0; j < jobs.Count; j++)
            {
                List<Result> newBranches = new List<Result>();

                var job = jobs[j];

                foreach (Result result in resultList)
                {
                    var availableTruckIds = GetAvailableTruckIdsByJobTypeWithRepeat(maximumCompatibleJobTypeListSize, trucks, result, maximumRepeat, job.Type);

                    if (availableTruckIds.Count == 0) // dead branch
                    {
                        result.Abandoned = true;
                    }
                    else if (availableTruckIds.Count == 1) // we have only one option, we move on
                    {
                        var searchedId = availableTruckIds.First();
                        var truck = trucks.Single(t => t.Id == searchedId);
                        result.AddPlusRepeatedUse(truck.Id);

                        result.TruckIdJobIdList.Add(new KeyValuePair<int, int>(truck.Id, job.Id));
                    }
                    else // There are many options so we need to generate new branches with the previously used truckId and jobId pairs
                    {
                        foreach (int truckid in availableTruckIds)
                        {
                            var truck = trucks.Single(t => t.Id == truckid);
                            result.AddPlusRepeatedUse(truck.Id);

                            var resultClone = result.Clone() as Result;
                            resultClone.TruckIdJobIdList.Add(new KeyValuePair<int, int>(truckid, job.Id));
                            newBranches.Add(resultClone);
                        }
                        result.Abandoned = true;
                    }

                }

            }

            return resultList;
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
            List<Result> resultList = new List<Result>();

            resultList.Add(new Result());

            for (int j = 0; j < jobs.Count; j++)
            {
                List<Result> newBranches = new List<Result>();

                var job = jobs[j];

                foreach (Result result in resultList)
                {
                    var previouslyUsedTruckIds = result.GetPreviouslyUsedTruckIds();
                    var availableTruckIds = GetAvailableTruckIdsByJobTypeWithoutRepeat(maximumCompatibleJobTypeListSize, trucks, previouslyUsedTruckIds, job.Type);

                    if (availableTruckIds.Count == 0) // dead branch
                    {
                        result.Abandoned = true;
                    }
                    else if (availableTruckIds.Count == 1) // we have only one option, we move on
                    {
                        var searchedId = availableTruckIds.First();
                        var truck = trucks.Single(t => t.Id == searchedId);

                        result.TruckIdJobIdList.Add(new KeyValuePair<int, int>(truck.Id, job.Id));
                    }
                    else // There are many options so we need to generate new branches with the previously used truckId and jobId pairs
                    {
                        foreach (int truckid in availableTruckIds)
                        {
                            var resultClone = result.Clone() as Result;
                            resultClone.TruckIdJobIdList.Add(new KeyValuePair<int, int>(truckid, job.Id));
                            newBranches.Add(resultClone);
                        }
                        result.Abandoned = true;
                    }
                }

                if (newBranches.Count > 0)
                {
                    resultList.AddRange(newBranches);
                }

                resultList = resultList.Where(r => r.Abandoned == false).ToList();
            }

            return resultList;
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

            for (int maxSize = 1; maxSize <= maximumCompatibleJobTypeListSize; maxSize++)
            {
                for (int repeat = 0; repeat <= maximumRepeat; repeat++)
                {
                    List<int> truckIdsWithRepeatNumber = new List<int>();

                    var truckIdsWithRepeat = result.GetTruckIdsByRepeatedUse(repeat);

                    var firstAvailableTruckWithThisSize = trucks.FirstOrDefault(t => t.CompatibleJobTypes.Length == maxSize && t.CompatibleJobTypes.Contains(searchedJobType)
                                                                                         && truckIdsWithRepeat.Contains(t.Id));
                    if (firstAvailableTruckWithThisSize != null)
                    {
                        return new List<int> { firstAvailableTruckWithThisSize.Id };
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
            for (int maxSize = 1; maxSize <= maximumCompatibleJobTypeListSize; maxSize++)
            {
                var firstAvailableTruckWithThisSize = trucks.FirstOrDefault(t => previouslyUsedTruckIds.Contains(t.Id) == false &&
                                        t.CompatibleJobTypes.Length == maxSize && t.CompatibleJobTypes.Contains(searchedJobType));
                if (firstAvailableTruckWithThisSize != null)
                {
                    return new List<int> { firstAvailableTruckWithThisSize.Id };
                }
            }

            return new List<int>();
        }

    }

}