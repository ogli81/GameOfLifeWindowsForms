using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameOfLifeWindowsForms.RingBuffer3D;

namespace GameOfLifeWindowsForms.SimulationTypes
{
    /// <summary>
    /// A 'BinaryCellSimulation' is one with only two possible states for each cell: 
    /// true and false (or 1 and 0) meaning true=alife and false=dead.
    /// </summary>
    public abstract class BinaryCellSimulation : AbstractCellSimulation
    {
        //state:

        protected BinaryRingBuffer3D ring;


        //c'tors:

        public BinaryCellSimulation(SimulationSettings settings) : base(settings)
        {
            ring = new BinaryRingBuffer3D(settings.MemSlots, settings.SizeX, settings.SizeY);
            aring = ring;
        }


        //helper methods:

        protected override void DoResizeRingBuffer(int mem, int x, int y)
        {
            base.DoResizeRingBuffer(mem, x, y);
            ring = new BinaryRingBuffer3D(mem, x, y, ring);
            aring = ring;
        }


        //public methods:

        public override decimal GetCellValue(int x, int y) => ring.GetCellValue(ring.Last, x, y) ? 1.0m : 0.0m;

        public override void SetCellValue(int x, int y, decimal val) { ring.SetCellValue(ring.Last, x, y, val >= 0.5m ? true : false); }
    }
}
