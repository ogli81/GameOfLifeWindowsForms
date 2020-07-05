using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameOfLifeWindowsForms.RingBuffer3D;

namespace GameOfLifeWindowsForms.SimulationTypes
{
    public abstract class AbstractCellSimulation : ICellSimulation
    {
        //state:

        protected int currentGen = 0;
        protected AbstractRingBuffer3D aring;
        public SimulationSettings Settings { get; }
        protected SimulationParameter? param1 = null;
        protected SimulationParameter? param2 = null;

        //now this is still an open qeustion:
        //protected LinkedList<decimal> stats1;
        protected List<decimal> stats1;
        //- should we use a normal List<T> (which uses arrays - way more efficient)
        //ideas here:
        https://www.c-sharpcorner.com/UploadFile/b942f9/implementing-a-double-ended-queue-or-deque-in-C-Sharp/


        //c'tors:

        public AbstractCellSimulation(SimulationSettings settings)
        {
            Settings = settings;
            stats1 = settings.TrackLifeStats ? new List<decimal>() : null;
        }
        /// <summary>
        /// Call this in the last line of your c'tor. When doing complex subclasses: 
        /// Overwrite this method with an empty body and make your own injection method.
        /// </summary>
        protected void InjectSettings() { Settings.Sim = this; }


        //helper methods:

        protected bool DoCreateNewGeneration()
        {
            if (aring.Full)
            {
                switch (Settings.MemSlotsFullBehavior)
                {
                    case MemFullBehavior.FORGET_SILENTLY:
                        break;
                    case MemFullBehavior.STOP_SILENTLY:
                        return false;
                    case MemFullBehavior.THROW_EXCEPTION:
                        throw new OutOfMemoryException("not enough memory slots for new cell fields!");
                        //throw new InsufficientMemoryException("not enough memory slots for new cell fields!");
                }
            }
            aring.AddGeneration();
            return true;
        }

        protected abstract bool DoCalculateNextGen();

        protected virtual void DoResizeRingBuffer(int mem, int x, int y)
        {
            AbstractRingBuffer3D.SafetyCheckNewRingBuffer(mem, x, y);
        }

        protected void LifeStatsEnsureCapacity()
        {
            if (stats1 != null)
                if (Settings.LifeStatsMem < 1)
                    stats1.Clear();
                else
                    if (Settings.LifeStatsMem < stats1.Count)
                        stats1.RemoveRange(0, stats1.Count - Settings.LifeStatsMem);
        }

        protected void LifeStatsRemove(int howMany)
        {
            if (stats1 != null)
                if (howMany >= stats1.Count)
                    stats1.Clear();
                else
                    stats1.RemoveRange(stats1.Count - howMany, howMany);
        }


        //abstract public methods:

        public abstract string Info { get; }

        public abstract void SetCellValue(int x, int y, decimal val);

        public abstract decimal GetCellValue(int x, int y);


        //public methods:

        public void SettingsChanged()
        {
            //Settings.MemSlots
            //Settings.SizeX
            //Settings.SizeY
            if (Settings.MemSlots != aring.MemSlots ||
                Settings.SizeX != aring.CellsX ||
                Settings.SizeY != aring.CellsY)
                DoResizeRingBuffer(Settings.MemSlots, Settings.SizeX, Settings.SizeY);

            //Settings.IsWrap
            //(nothing to do - future calculations will be performed with/without wrap)

            //Settings.TrackLifeStats
            if (Settings.TrackLifeStats == true && stats1 == null)
                stats1 = new List<decimal>(Settings.LifeStatsMem);
            else
            if (Settings.TrackLifeStats == false && stats1 != null)
                stats1 = null;

            //Settings.LifeStatsMem


            //Settings.MemSlotsFullBehavior
            //Settings.LifeStatsFullBehavior
            //(nothing to do - behavior will change for next "mem full" situation)
        }

        public void GoToOldestGen()
        {
            aring.RemoveAllExceptOldest();
        }

        public void RelabelCurrentAsZero()
        {
            aring.RemoveAllExceptNewest();
        }

        public bool CalculateNextGen()
        {
            if (!DoCreateNewGeneration())
                return false;
            DoCalculateNextGen();
            return true;
        }

        public int CurrentGen => currentGen;

        public int OldestGen => currentGen - aring.Length + 1;

        public int NumGens => aring.Length;

        public bool GoBackOneGen() => aring.RemoveGeneration();

        public bool GoToGen(int genId)
        {
            if (genId < 0)
                return false;
            int remove = currentGen - genId;
            if (remove <= 0)
                return aring.RemoveGenerations(remove);
            else
                for (int i = -1; i >= remove; i--)
                    if (!CalculateNextGen())
                        return false;
            return true;
        }

        public SimulationParameter? Param1 => param1;

        public SimulationParameter? Param2 => param2;
    }
}
