using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TruckTest.Entities
{
    public class Result : ICloneable
    {

        private List<KeyValuePair<int, int>> _truckIdJobIdList;

        private List<KeyValuePair<int, int>> _truckIdRepeatedUse;

        private bool _abandoned;

        public int TruckIdJobIdListPairCount
        {
            get { return _truckIdJobIdList.Count; }
        }

        public bool Abandoned
        {
            get { return _abandoned; }
            set { _abandoned = value; }
        }

        public Result()
        {
            _truckIdJobIdList = new List<KeyValuePair<int, int>>();
            _truckIdRepeatedUse = new List<KeyValuePair<int, int>>();
            _abandoned = false;
        }

        public void AddNewTruckIdJobIdPair(KeyValuePair<int, int> truckIdJobIdPair)
        {
            _truckIdJobIdList.Add(truckIdJobIdPair);
        }

        public void AddNewTruckIdRepeatedUsePair(KeyValuePair<int, int> truckIdRepeatedUse)
        {
            _truckIdRepeatedUse.Add(truckIdRepeatedUse);
        }

        public void FillTruckIdRepeatUseList(List<Truck> trucks)
        {
            for (int i = 0; i < trucks.Count; i++)
            {
                _truckIdRepeatedUse.Add(new KeyValuePair<int, int>(trucks[i].Id, 0));
            }
        }

        public object Clone()
        {
            var resultClone = new Result();

            for (int i = 0; i < _truckIdJobIdList.Count; i++)
            {
                var pair = _truckIdJobIdList[i];
                resultClone.AddNewTruckIdJobIdPair(new KeyValuePair<int, int>(pair.Key, pair.Value));
            }

            for (int i = 0; i < _truckIdRepeatedUse.Count; i++)
            {
                var pair = _truckIdRepeatedUse[i];
                resultClone.AddNewTruckIdRepeatedUsePair(new KeyValuePair<int, int>(pair.Key, pair.Value));
            }

            return resultClone;
        }

        public void AddPlusRepeatedUse(int truckId)
        {
            KeyValuePair<int, int> truckIdRepeated = _truckIdRepeatedUse.FirstOrDefault(t => t.Key == truckId);
        
            int repeated = truckIdRepeated.Value;
            repeated++;
            _truckIdRepeatedUse.Remove(truckIdRepeated);
            _truckIdRepeatedUse.Add(new KeyValuePair<int, int>(truckId, repeated));     
        }

        public List<int> GetTruckIdsByRepeatedUse(int repeatedUseNumber)
        {
            return _truckIdRepeatedUse.Where(tj => tj.Value == repeatedUseNumber).Select(tr => tr.Key).ToList();
        }

        public List<int> GetPreviouslyUsedTruckIds()
        {
            return _truckIdJobIdList.Select(tj => tj.Key).ToList();
        }

        public string PrintToText()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (KeyValuePair<int, int> truckAndJob in _truckIdJobIdList)
            {
                var truckId = truckAndJob.Key;
                var jobId = truckAndJob.Value;
                stringBuilder.AppendLine(truckId + " " + jobId);
            }

            return stringBuilder.ToString();
        }
    }
}
