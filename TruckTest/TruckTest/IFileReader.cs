using System;
using System.Collections.Generic;
using System.Text;
using TruckTest.Entities;

namespace TruckTest
{
    public interface IFileReader
    {
        FileData LoadAndCreateEntities(string inputFile);
    }
}
