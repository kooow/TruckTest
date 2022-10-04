﻿using System;
using System.Collections.Generic;
using System.Text;
using TruckTest.Entities;

namespace TruckTest
{
    public interface IJobScheduler
    {

        List<Result> CalculateResultsWithoutRepeatableTrucks(FileData fileData);

        List<Result> CalculateResultsWithRepeatable(FileData fileData, int maximumRepeat);

        void WriteResultToFile(List<Result> result, string fileName);

    }
}
