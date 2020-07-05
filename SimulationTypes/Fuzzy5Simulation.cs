using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeWindowsForms.SimulationTypes
{
    public class Fuzzy5Simulation : GenericCellSimulation<byte>
    {
        //c'tors:

        public Fuzzy5Simulation(SimulationSettings settings) : base(settings)
        {
            InjectSettings();
        }


        //helper methods:

        protected override bool DoCalculateNextGen()
        {
            throw new NotImplementedException();
        }


        //public methods:

        public override string Info => "A modified version of the life simulation by John Horton Conway - we use 5 different states: 0,1,2,3,4 (0 = fully dead, 4 = fully alife).";

        public override decimal GetCellValue(int x, int y)
        {
            switch (ring.Last[x,y])
            {
                case 0: return 0.0m;
                case 1: return 0.25m;
                case 2: return 0.5m;
                case 3: return 0.75m;
                case 4: return 1.0m;
                default: throw new ArgumentException("Only values 0,1,2,3,4 are allowed here!");
            }
        }

        public override void SetCellValue(int x, int y, decimal val)
        {
            //there are multiple ways to achieve this:

            //// range-based switching (as of C#7), performance: ???
            //switch(val)
            //{
            //    case decimal n when (n <= 0.0m): { ring.Last[x, y] = 0; return; }
            //    case decimal n when (n <= 0.25m): { ring.Last[x, y] = 1; return; }
            //    case decimal n when (n <= 0.5m): { ring.Last[x, y] = 2; return; }
            //    case decimal n when (n <= 0.75m): { ring.Last[x, y] = 3; return; }
            //    default: { ring.Last[x, y] = 4; return; }
            //}

            //// linear checks (best case: 1 checks, worst case: 5 checks, average: 3 checks)
            //if (val <= 0.0m) { ring.Last[x, y] = 0; return; }
            //if (val <= 0.25m) { ring.Last[x, y] = 1; return; }
            //if (val <= 0.5m) { ring.Last[x, y] = 2; return; }
            //if (val <= 0.75m) { ring.Last[x, y] = 3; return; }
            //{ ring.Last[x, y] = 4; return; }
            
            // binary checks (best case: 2 checks, worst case: 3 checks, average: 2.4 checks)
            //if(val > 0.5m)
            //    if(val > 0.75m)
            //    { ring.Last[x, y] = 4; return; }
            //    else
            //    { ring.Last[x, y] = 3; return; }
            //else
            //    if(val > 0.25m)
            //    { ring.Last[x, y] = 2; return; }
            //    else
            //        if(val > 0.0m)
            //        { ring.Last[x, y] = 1; return; }
            //        else
            //        { ring.Last[x, y] = 0; return; }

            // binary checks written with ternary operator:
            ring.Last[x, y] = (val > 0.5m ? (val > 0.75m ? (byte)4 : (byte)3) : (val > 0.25m ? (byte)2 : (val > 0.0m ? (byte)1 : (byte)0)));
        }
    }
}
