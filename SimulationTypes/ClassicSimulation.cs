using GameOfLifeWindowsForms.SimulationTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeWindowsForms
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

        public override decimal GetCellValue(int x, int y) => ring.Last[x, y] ? 1.0m : 0.0m;
    }

    TODO
    public abstract class AbstractCellSimulation : ICellSimulation
    {
        protected int currentGen = 0;
        protected LinkedList<decimal> stats1;

        TODO
    }

    TODO
    public abstract class BinaryCellSimulation : AbstractCellSimulation
    {
        TODO
    }

    TODO
    /// <summary>
    /// Abstract base class for all 
    /// </summary>
    /// <typeparam name="C"></typeparam>
    public abstract class GenericCellSimulation<C> : AbstractCellSimulation
    {
        //state:

        //TODO:
        //idea for later: replace by ring buffer that uses BitArray for the "binary" cell type (C is "bool" or "Boolean")
        //we want to have a 'GenericCellSimulation' and a 'BinaryCellSimulation'
        protected GenericRingBuffer3D<C> ring; 
        



        //c'tor
        public GenericCellSimulation(SimulationSettings settings)
        {
            Settings = settings;
            ring = new GenericRingBuffer3D<C>(settings.MemSlots, settings.SizeX, settings.SizeY);
            stats1 = settings.TrackLifeStats ? new LinkedList<decimal>() : null;
        }
        protected void InjectSettings() { Settings.Sim = this; } //call this in the last line of your c'tor


        //public methods:

        public string Info => "The classic game of life simulation by John Horton Conway - simulates birth and death of a population of micro organisms.";

        public SimulationSettings Settings { get; }

        public int CurrentGen => currentGen;

        public int OldestGen => currentGen - ring.Length + 1;

        public int NumGens => ring.Length;

        public SimulationParameter? Param1 => null;

        public SimulationParameter? Param2 => null;

        //[AllowNull] //<--- not available in my version of .Net :-(
        //public LinkedList<decimal> LifeStats => stats1;

        SimulationSettings ICellSimulation.Settings { get; }

        public bool CalculateNextGen()
        {
            if (ring.Full)
            {
                switch (Settings.MemSlotsFullBehavior)
                {
                    case MemFullBehavior.FORGET_SILENTLY:
                        ring.AddGeneration();
                        return true;
                    case MemFullBehavior.STOP_SILENTLY:
                        return false;
                    case MemFullBehavior.THROW_EXCEPTION:
                        throw new OutOfMemoryException("not enough memory slots for new cell fields!");
                        //throw new InsufficientMemoryException("not enough memory slots for new cell fields!");
                }
            }

            TODO

            return true;
        }

        public abstract decimal GetCellValue(int x, int y);

        public bool GoBackOneGen() => ring.RemoveGeneration();

        public bool GoToGen(int genId)
        {
            if (genId < 0)
                return false;
            int remove = currentGen - genId;
            if (remove <= 0)
                return ring.RemoveGenerations(remove);
            else
                for(int i = -1; i >= remove; i--)
                    if (!CalculateNextGen())
                        return false;
            return true;
        }

        public void GoToOldestGen()
        {
            throw new NotImplementedException();
        }

        public void RelabelCurrentAsZero()
        {
            throw new NotImplementedException();
        }

        public void SetCellValue(int x, int y, decimal val)
        {
            throw new NotImplementedException();
        }

        public void SettingsChanged()
        {
            throw new NotImplementedException();
        }
    }
}
