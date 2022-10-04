using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TruckTest.Entities
{
    public class Result : ICloneable
    {
        public List<KeyValuePair<int, int>> TruckIdJobIdList = new List<KeyValuePair<int, int>>();

        public List<KeyValuePair<int, int>> TruckIdRepeatedUse = new List<KeyValuePair<int, int>>();

        public bool Abandoned = false;

        public Result()
        {
        }

        public void FillTruckIdRepeatUseList(List<Truck> trucks)
        {
            for (int i = 0; i < trucks.Count; i++)
            {
                TruckIdRepeatedUse.Add(new KeyValuePair<int, int>(trucks[i].Id, 0));
            }
        }

        public object Clone()
        {
            var resultClone = new Result();

            for (int i = 0; i < TruckIdJobIdList.Count; i++)
            {
                var pair = TruckIdJobIdList[i];
                resultClone.TruckIdJobIdList.Add(new KeyValuePair<int, int>(pair.Key, pair.Value));
            }


            for (int i = 0; i < TruckIdRepeatedUse.Count; i++)
            {
                var pair = TruckIdRepeatedUse[i];
                resultClone.TruckIdRepeatedUse.Add(new KeyValuePair<int, int>(pair.Key, pair.Value));
            }

            return resultClone;
        }

        public void AddPlusRepeatedUse(int truckId)
        {
            KeyValuePair<int, int> truckIdRepeated = TruckIdRepeatedUse.FirstOrDefault(t => t.Key == truckId);
        
            int repeated = truckIdRepeated.Value;
            repeated++;
            TruckIdRepeatedUse.Remove(truckIdRepeated);
            TruckIdRepeatedUse.Add(new KeyValuePair<int, int>(truckId, repeated));     
        }

        public List<int> GetTruckIdsByRepeatedUse(int repeatedUseNumber)
        {
            return TruckIdRepeatedUse.Where(tj => tj.Value == repeatedUseNumber).Select(tr => tr.Key).ToList();
        }

        public List<int> GetPreviouslyUsedTruckIds()
        {
            return TruckIdJobIdList.Select(tj => tj.Key).ToList();
        }

        public string PrintToText()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (KeyValuePair<int, int> truckAndJob in TruckIdJobIdList)
            {
                var truckId = truckAndJob.Key;
                var jobId = truckAndJob.Value;
                stringBuilder.AppendLine(truckId + " " + jobId);
            }

            return stringBuilder.ToString();
        }
    }
}
