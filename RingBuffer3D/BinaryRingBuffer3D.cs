using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeWindowsForms.RingBuffer3D
{
    /// <summary>
    /// A 3-dimensional ringbuffer (stores a '2-dimensional cell field' with a 'memory of the past'). 
    /// This version of the 3-dimensional ring buffer idea uses a BitArray (just a huge series of bits, 
    /// each representing the content of a cell in our cell field).<br></br>
    /// (IDEA for later: We might want to save even more memory space by using some kind of in-memory compression)
    /// </summary>
    public class BinaryRingBuffer3D : AbstractRingBuffer3D
    {
        //state:

        //1st dim = "ring buffer position"
        //2nd dim = "X"
        //3rd dim = "Y"
        //(X,Y) = (0,0) = "upper left corner"
        protected BitArray[] ringBuffer;


        //c'tors:
        /// <summary>
        /// Make a new instance (first c'tor variant).
        /// </summary>
        /// <param name="mem">number of memory slots in the ring buffer (1st dimension)</param>
        /// <param name="x">number of cells in x direction (2nd dimension)</param>
        /// <param name="y">number of cells in y direction (3rd dimension)</param>
        public BinaryRingBuffer3D(int mem, int x, int y) : base(mem, x, y)
        {
            ringBuffer = new BitArray[mem];

            MakeGeneration(0, false, false);
        }

        /// <summary>
        /// Make a new instance, copy from other instance (copy c'tor A, second c'tor variant). 
        /// Makes a copy of the other ring buffer (but only copies useful data, to safe time).
        /// </summary>
        /// <param name="other">the other instance</param>
        public BinaryRingBuffer3D(BinaryRingBuffer3D other) : this(other.MemSlots, other.CellsX, other.CellsY)
        {
            //we copy as much as we can from the other
            //use 'newestGenPos', work into the direction of 'oldestGenPos' (fit as much as we can)
            //use top-left corner (0,0), work into x and y direction as far as we can
            //if this new ringbuffer is bigger than the other: fill with 'defaultValue'

            this.newestGenPos = other.newestGenPos;
            this.oldestGenPos = other.oldestGenPos;

            int i = oldestGenPos;
            do
            {
                MakeGeneration(i, false, false);

                ringBuffer[i] = (BitArray)other.ringBuffer[i].Clone(); //TODO: hopefully, this works as intended? (shallow copy should work?)

                i++;
                if (i >= MemSlots)
                    i = 0;
            }
            while (i != newestGenPos);
        }

        /// <summary>
        /// Make a new instance, copy from other instance (copy c'tor B, third c'tor variant). 
        /// Tries to grab as much data from the other instance as possible.
        /// </summary>
        /// <param name="mem">number of memory slots in the ring buffer (1st dimension)</param>
        /// <param name="x">number of cells in x direction (2nd dimension)</param>
        /// <param name="y">number of cells in y direction (3rd dimension)</param>
        /// <param name="other">the other instance</param>
        /// <param name="defaultValue">default value for excess cells (if new instance has more space than the other instance)</param>
        public BinaryRingBuffer3D(int mem, int x, int y, BinaryRingBuffer3D other, bool defaultValue) : this(mem, x, y)
        {
            //we copy as much as we can from the other
            //use 'newestGenPos', work into the direction of 'oldestGenPos' (fit as much as we can)
            //use top-left corner (0,0), work into x and y direction as far as we can
            //if this new ringbuffer is bigger than the other: fill with 'defaultValue'

            this.newestGenPos = other.newestGenPos;
            this.oldestGenPos = other.oldestGenPos;

            int i = oldestGenPos;
            do
            {
                MakeGeneration(i, true, defaultValue);

                int ox = other.CellsX;
                int oy = other.CellsY;
                BitArray arr = ringBuffer[i];
                BitArray oarr = other.ringBuffer[i];
                int pos, opos;
                bool value;
                for (int a = 0; a < x; a++)
                {
                    if (a >= ox)
                        break;
                    for (int b = 0; b < y; b++)
                    {
                        if (b >= oy)
                            break;
                        pos = b * CellsX + a;
                        opos = b * other.CellsX + a;
                        value = oarr.Get(opos);
                        arr.Set(pos, value);
                    }
                }

                i++;
                if (i >= MemSlots)
                    i = 0;
            }
            while (i != newestGenPos);
        }


        //helper methods:

        private void MakeGeneration(int where, bool clearWithValue, bool value)
        {
            if (ringBuffer[where] == null)
            {
                ringBuffer[where] = new BitArray(CellsX * CellsY);
                if (clearWithValue && value == false)
                    return; //because it's already all set to false (the default of type 'bool')
            }
            if (clearWithValue)
                ringBuffer[where].SetAll(value);
        }

        protected override void FreeBuffer(int pos)
        {
            ringBuffer[pos] = null;
        }


        //public methods:

        public override int MemSlots => ringBuffer.Length;

        public override void AddGeneration(bool clearWithDefault = false)
        {
            MoveForward();
            MakeGeneration(newestGenPos, clearWithDefault, false);
        }

        /// <summary>
        /// Add another entry (generation) in the ring buffer, using the specified initialValue for all cells.
        /// </summary>
        /// <param name="initialValue"></param>
        public void AddGeneration(bool setInitialValue, bool initialValue = true)
        {
            MoveForward();
            MakeGeneration(newestGenPos, setInitialValue, initialValue);
        }

        /// <summary>
        /// Get the latest (newest) data from the ring buffer.
        /// </summary>
        public BitArray Last => ringBuffer[newestGenPos];

        /// <summary>
        /// Get the earliest (oldest) data from the ring buffer.
        /// </summary>
        public BitArray First => ringBuffer[oldestGenPos];

        /// <summary>
        /// Get the data entry right before the latest (newest) entry.
        /// </summary>
        public BitArray Previous => this[Length - 2];

        /// <summary>
        /// The indexer can be used like this: myRingBuffer[0] <--- this will return the oldest data from the ring buffer! 
        /// 0 = oldest = First  and  Length-1 = newest = Last
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public BitArray this[int i]
        {
            get
            {
                if (i > Length - 1)
                    throw new ArgumentException("can't be more than Length-1!");
                if (i < 0)
                    throw new ArgumentException("can't be less than zero!");
                int pos = oldestGenPos + i;
                pos %= MemSlots;
                return ringBuffer[pos];
            }
        }
    }
}
