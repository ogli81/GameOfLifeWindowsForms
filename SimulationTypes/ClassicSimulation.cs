using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeWindowsForms.SimulationTypes
{
    /// <summary>
    /// The classic game of life simulation by John Horton Conway - simulates birth and death of a population of micro organisms.
    /// </summary>
    public class ClassicSimulation : BinaryCellSimulation
    {
        public ClassicSimulation(SimulationSettings settings) : base(settings)
        {
            InjectSettings();
        }

        public override string Info => "The classic game of life simulation by John Horton Conway - simulates birth and death of a population of micro organisms.";

        protected override bool DoCalculateNextGen()
        {
            throw new NotImplementedException();
        }
    }



}
