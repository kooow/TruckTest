using System;
using System.Collections.Generic;
using System.Text;

namespace TruckTest.Entities
{
    public class FileData
    {
        private List<Truck> _trucks;

        private List<Job> _jobs;

        public List<Truck> Trucks
        {
            get { return _trucks; }
        }
        public List<Job> Jobs
        {
            get { return _jobs; }
        }

        public FileData(List<Truck> trucks, List<Job> jobs)
        {
            _trucks = trucks;
            _jobs = jobs;
        }
    }
}