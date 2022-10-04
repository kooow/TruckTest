using System;
using System.Collections.Generic;
using System.Text;

namespace TruckTest.Entities
{
    public class Truck
    {
        private char[] _compatibleJobTypes;

        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public char[] CompatibleJobTypes
        {
            get { return _compatibleJobTypes; }
            set { _compatibleJobTypes = value; }
        }
    }
}
