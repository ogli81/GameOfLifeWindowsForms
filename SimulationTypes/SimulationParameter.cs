using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeWindowsForms.SimulationTypes
{
    public struct SimulationParameter
    {
        public SimulationParameter(string name) : this(name, 0.0m, 1.0m)
        {

        }

        public SimulationParameter(string name, decimal min, decimal max) : this(name, min, max, (max+min)*0.5m)
        {

        }

        public SimulationParameter(string name, decimal min, decimal max, decimal current)
        {
            if (name == null)
                throw new ArgumentNullException("name must not be null!");
            if (min > max)
                throw new ArgumentException("min must be smaller than max!");
            if (current < min)
                throw new ArgumentException("current must be greater than or equal to min!");
            if (current > max)
                throw new ArgumentException("current musst be less than or equal to max!");

            Name = name;
            Min = min;
            Max = max;
            curr = current;
        }

        public string Name { get; }

        public decimal Min { get; }
        public decimal Max { get; }

        private decimal curr;
        public decimal Current 
        {
            get => curr;
            set
            {
                if (value < Min)
                    throw new ArgumentException("Current must be greater than or equal to Min!");
                if (value > Max)
                    throw new ArgumentException("Current musst be less than or equal to Max!");
                curr = value;
            }
        }
    }
}
