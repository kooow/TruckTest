using TruckTest.Entities;

namespace TruckTest;

public interface IFileReader
{
    FileData LoadAndCreateEntities(string inputFile);
}
