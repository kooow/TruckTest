using System;
using System.Collections.Generic;
using System.Text;

namespace TruckTest.Entities
{
    public class Job
    {
        private int _id;

        private char _type;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Char Type
        {
            get { return _type; }
            set { _type = value; }
        }
    }
}
