using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameOfLifeWindowsForms.RingBuffer3D;

namespace GameOfLifeWindowsForms.SimulationTypes
{
    /// <summary>
    /// Abstract base class for all "generic" cell simulations, meaning we have a type 'C' for the 
    /// content of a cell in our cell field.
    /// </summary>
    /// <typeparam name="C"></typeparam>
    public abstract class GenericCellSimulation<C> : AbstractCellSimulation
    {
        //state:

        protected GenericRingBuffer3D<C> ring;


        //c'tors:

        public GenericCellSimulation(SimulationSettings settings) : base(settings)
        {
            ring = new GenericRingBuffer3D<C>(settings.MemSlots, settings.SizeX, settings.SizeY);
            aring = ring;
        }


        //helper methods:

        protected override void DoResizeRingBuffer(int mem, int x, int y)
        {
            base.DoResizeRingBuffer(mem, x, y);
            ring = new GenericRingBuffer3D<C>(mem, x, y, ring);
            aring = ring;
        }


        //public methods:

        //-
    }
}
