using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeWindowsForms.RingBuffer3D
{
    /// <summary>
    /// A 3-dimensional ringbuffer (stores a '2-dimensional cell field' with a 'memory of the past'). 
    /// </summary>
    public abstract class AbstractRingBuffer3D
    {
        //state:

        protected int newestGenPos; //where in the ringBuffer is the newest generation (maximum)?
        protected int oldestGenPos; //where in the ringBuffer is the oldest generation (minimum)?

        /// <summary>
        /// The number of cells in x direction (2nd dim).
        /// </summary>
        public int CellsX { get; }

        /// <summary>
        /// The number of cells in y direction (3rd dim).
        /// </summary>
        public int CellsY { get; }


        //c'tors:

        /// <summary>
        /// Make a new instance (first c'tor variant).
        /// </summary>
        /// <param name="mem">number of memory slots in the ring buffer (1st dimension)</param>
        /// <param name="x">number of cells in x direction (2nd dimension)</param>
        /// <param name="y">number of cells in y direction (3rd dimension)</param>
        protected AbstractRingBuffer3D(int mem, int x, int y)
        {
            if (mem < 2)
                throw new ArgumentException("please give us at least 2 memory slots!");
            if (x < 1)
                throw new ArgumentException("x can't be less than 1!");
            if (y < 1)
                throw new ArgumentException("y can't be less than 1!");
            newestGenPos = 0;
            oldestGenPos = 0;

            //current idea: reserve memory for the first generation only
            CellsX = x;
            CellsY = y;
        }


        //helper methods:

        protected void MoveForward()
        {
            //newestGen increased by one, possibly oldestGen needs to move +1 too
            newestGenPos++;
            newestGenPos %= MemSlots;
            if (newestGenPos == oldestGenPos)
            {
                oldestGenPos++;
                oldestGenPos %= MemSlots;
            }
        }

        protected bool MoveBack()
        {
            if (newestGenPos == oldestGenPos)
                return false;
            newestGenPos--;
            if (newestGenPos < 0)
                newestGenPos = MemSlots - 1;
            return true;
        }

        protected abstract void FreeBuffer(int pos);


        //public methods:

        /// <summary>
        /// The number of memory slots in this ring buffer (1st dim).
        /// </summary>
        public abstract int MemSlots { get; }

        /// <summary>
        /// Add another entry (generation) in the ring buffer. You can chose if you want to clear the cells 
        /// or if you want to use them as they are (possibly containing old data from previous allocations). 
        /// Clearing the cells will cost a bit computation time (writes to memory) and after that process 
        /// the cells will all contain the default value of the type T. 
        /// </summary>
        /// <param name="initialValue"></param>
        public abstract void AddGeneration(bool clearWithDefault = false);

        /// <summary>
        /// Will try to remove one generation. This attempt will fail, if only 1 generation was left when you 
        /// tried to remove one generation.
        /// </summary>
        /// <returns>Will return false, if there was only 1 generation left when you tried to remove one (and true otherwise).</returns>
        public bool RemoveGeneration() => MoveBack();

        /// <summary>
        /// Will try to remove as many generations as you want to remove. If not that many 
        /// generations can be removed (so if less than 'howMany + 1' generations are available), 
        /// false will be returned (and otherwise true).
        /// </summary>
        /// <param name="howMany">The number of generations that we want to remove.</param>
        /// <returns>Will return false, if not that many generations were removed (so when false: 
        /// it's either zero or less than intended were removed)</returns>
        public bool RemoveGenerations(int howMany)
        {
            if (howMany < 0)
                throw new ArgumentException("can't be less than zero!");

            int todo = howMany;
            while (todo > 0)
            {
                if (!RemoveGeneration())
                    return false;
                todo--;
            }
            return true;
        }

        /// <summary>
        /// Discard everything except the newest generation.
        /// </summary>
        public void RemoveAllExceptNewest()
        {
            oldestGenPos = newestGenPos;
        }

        /// <summary>
        /// Discard everything except the oldest generation.
        /// </summary>
        public void RemoveAllExceptOldest()
        {
            newestGenPos = oldestGenPos;
        }

        /// <summary>
        /// Only those cell fields will be kept in memory that are actually used (they are between oldest and newest). 
        /// Using this method might free some memory but it might also cause more memory fragmentation! 
        /// If the full ring buffer is being used right now then we can't free any memory.
        /// </summary>
        public void FreeUnusedMemory()
        {
            int pos = newestGenPos;
            int n = MemSlots;
            bool done = false;

            do
            {
                pos++;
                if (pos == n) //<--- question: are these two lines 
                    pos = 0; //<--- faster than:  pos %= n  ???
                if (pos == oldestGenPos)
                    done = true;
                else
                    FreeBuffer(pos); //clear the array here
            }
            while (!done);
        }

        /// <summary>
        /// The number of valid data entries (generations) in this ring buffer. 
        /// Directly after creating a new instance (first c'tor variant), this value is 1. 
        /// The value can never be more than the number of memory slots (first dimension).
        /// </summary>
        public int Length
        {
            get
            {
                //returns the number of generations (oldest and newest included)
                if (newestGenPos > oldestGenPos)
                {
                    return newestGenPos - oldestGenPos + 1;
                }
                else
                {
                    //return newestGenPos + 1 + MemSlots - 1 - oldestGenPos + 1;
                    //---
                    //example:
                    //[0,1,2,3,4], oldestGenPos = 3, newestGenPos = 1
                    //-> MemSlots is 5
                    //-> expected return value is 4
                    //return 1 + 1 + 5 - 1 - 3 + 1
                    //return 4

                    return newestGenPos + MemSlots - oldestGenPos + 1;
                }
            }
        }

        /// <summary>
        /// Is this ring buffer fully filled? (Length is same as number of MemSlots => ring buffer is full)
        /// </summary>
        public bool Full => Length == MemSlots;
    }
}
