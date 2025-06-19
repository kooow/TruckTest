using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TruckTest.Entities;

public class Result : ICloneable
{
    private List<KeyValuePair<int, int>> m_truckIdJobIdList;
    private List<KeyValuePair<int, int>> m_truckIdRepeatedUse;
    private bool m_abandoned;

    public int TruckIdJobIdListPairCount => m_truckIdJobIdList.Count;

    public bool Abandoned
    {
        get { return m_abandoned; }
        set { m_abandoned = value; }
    }

    public Result()
    {
        m_truckIdJobIdList = new List<KeyValuePair<int, int>>();
        m_truckIdRepeatedUse = new List<KeyValuePair<int, int>>();
        m_abandoned = false;
    }

    public void AddNewTruckIdJobIdPair(KeyValuePair<int, int> truckIdJobIdPair)
    {
        m_truckIdJobIdList.Add(truckIdJobIdPair);
    }

    public void AddNewTruckIdRepeatedUsePair(KeyValuePair<int, int> truckIdRepeatedUse)
    {
        m_truckIdRepeatedUse.Add(truckIdRepeatedUse);
    }

    public void FillTruckIdRepeatUseList(List<Truck> trucks)
    {
        for (int i = 0; i < trucks.Count; i++)
        {
            m_truckIdRepeatedUse.Add(new KeyValuePair<int, int>(trucks[i].Id, 0));
        }
    }

    public object Clone()
    {
        var resultClone = new Result();

        for (int i = 0; i < m_truckIdJobIdList.Count; i++)
        {
            var pair = m_truckIdJobIdList[i];
            resultClone.AddNewTruckIdJobIdPair(new KeyValuePair<int, int>(pair.Key, pair.Value));
        }

        for (int i = 0; i < m_truckIdRepeatedUse.Count; i++)
        {
            var pair = m_truckIdRepeatedUse[i];
            resultClone.AddNewTruckIdRepeatedUsePair(new KeyValuePair<int, int>(pair.Key, pair.Value));
        }

        return resultClone;
    }

    public void AddPlusRepeatedUse(int truckId)
    {
        KeyValuePair<int, int> truckIdRepeated = m_truckIdRepeatedUse.FirstOrDefault(t => t.Key == truckId);

        int repeated = truckIdRepeated.Value;
        repeated++;
        m_truckIdRepeatedUse.Remove(truckIdRepeated);
        m_truckIdRepeatedUse.Add(new KeyValuePair<int, int>(truckId, repeated));
    }

    public List<int> GetTruckIdsByRepeatedUse(int repeatedUseNumber)
    {
        return m_truckIdRepeatedUse.Where(tj => tj.Value == repeatedUseNumber).Select(tr => tr.Key).ToList();
    }

    public List<int> GetPreviouslyUsedTruckIds()
    {
        return m_truckIdJobIdList.Select(tj => tj.Key).ToList();
    }

    public string PrintToText()
    {
        StringBuilder stringBuilder = new();

        foreach (KeyValuePair<int, int> truckAndJob in m_truckIdJobIdList)
        {
            var truckId = truckAndJob.Key;
            var jobId = truckAndJob.Value;
            stringBuilder.AppendLine($"{truckId} {jobId}");
        }

        return stringBuilder.ToString();
    }
}
