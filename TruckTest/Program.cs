using System;

namespace TruckTest;

/// <summary>
/// Program class serves as the main entry point for the TruckTest application.
/// </summary>
class Program
{
    private static readonly string s_inputFile = "_input.txt";
    private static readonly string s_outputFile = "output.txt";
    private static readonly string s_outputWithoutRepeatFile = "output_without_repeat.txt";

    /// <summary>
    /// Main entry point for the application.
    /// </summary>
    static void Main()
    {
        Console.WriteLine($"Base dir: {AppContext.BaseDirectory}");

        IFileReader fileReader = new FileReader();
        var fileData = fileReader.LoadAndCreateEntities(s_inputFile);

        IJobScheduler jobScheduler = new JobScheduler();

        var resultsWithoutRepeatableTrucks = jobScheduler.CalculateResultsWithoutRepeatableTrucks(fileData);
        jobScheduler.WriteResultToFile(resultsWithoutRepeatableTrucks, s_outputWithoutRepeatFile);

        int maximumRepeat = 1;

        var resultsWithRepeatable = jobScheduler.CalculateResultsWithRepeatable(fileData, maximumRepeat);
        jobScheduler.WriteResultToFile(resultsWithRepeatable, s_outputFile);

        Console.WriteLine("Press any key to exit the program...");
        Console.ReadKey();
    }
}
