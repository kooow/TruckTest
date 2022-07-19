using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TruckTest.Entities
{
    public class Result : ICloneable
    {
        public List<KeyValuePair<int, int>> TruckId_JobId_List = new List<KeyValuePair<int, int>>();

        public List<KeyValuePair<int, int>> TruckId_RepeatedUse = new List<KeyValuePair<int, int>>();

        public bool abandoned = false;

        public Result()
        {
        }

        public void FillTruckIdRepeatUseList(List<Truck> trucks)
        {
            for (int i = 0; i < trucks.Count; i++)
            {
                TruckId_RepeatedUse.Add(new KeyValuePair<int, int>(trucks[i].Id, 0));
            }
        }

        public object Clone()
        {
            var resultClone = new Result();

            for (int i = 0; i < TruckId_JobId_List.Count; i++)
            {
                var pair = TruckId_JobId_List[i];
                resultClone.TruckId_JobId_List.Add(new KeyValuePair<int, int>(pair.Key, pair.Value));
            }


            for (int i = 0; i < TruckId_RepeatedUse.Count; i++)
            {
                var pair = TruckId_RepeatedUse[i];
                resultClone.TruckId_RepeatedUse.Add(new KeyValuePair<int, int>(pair.Key, pair.Value));
            }

            return resultClone;
        }

        public void AddPlusRepeatedUse(int truckId)
        {
            KeyValuePair<int, int> truckId_repeated = TruckId_RepeatedUse.FirstOrDefault(t => t.Key == truckId);
        
            int repeated = truckId_repeated.Value;
            repeated++;
            TruckId_RepeatedUse.Remove(truckId_repeated);
            TruckId_RepeatedUse.Add(new KeyValuePair<int, int>(truckId, repeated));     
        }

        public List<int> GetTruckIdsByRepeatedUse(int repeatedUseNumber)
        {
            return TruckId_RepeatedUse.Where(tj => tj.Value == repeatedUseNumber).Select(tr => tr.Key).ToList();
        }

        public List<int> GetPreviouslyUsedTruckIds()
        {
            return TruckId_JobId_List.Select(tj => tj.Key).ToList();
        }

        public string PrintToText()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (KeyValuePair<int, int> t_j in TruckId_JobId_List)
            {
                var truck_id = t_j.Key;
                var job_id = t_j.Value;
                stringBuilder.AppendLine(truck_id + " " + job_id);
            }

            return stringBuilder.ToString();
        }
    }
}
