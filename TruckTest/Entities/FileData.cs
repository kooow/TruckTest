using System.Collections.Generic;

namespace TruckTest.Entities;

public class FileData
{
    private readonly List<Truck> m_trucks;
    private readonly List<Job> m_jobs;

    public List<Truck> Trucks => m_trucks;
    public List<Job> Jobs => m_jobs;

    public FileData(List<Truck> trucks, List<Job> jobs)
    {
        m_trucks = trucks;
        m_jobs = jobs;
    }
}