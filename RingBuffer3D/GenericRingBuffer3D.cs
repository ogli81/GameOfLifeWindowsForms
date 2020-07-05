using System;
using System.Collections;

namespace GameOfLifeWindowsForms.RingBuffer3D
{
    /// <summary>
    /// A 3-dimensional ringbuffer (stores a '2-dimensional cell field' with a 'memory of the past'). 
    /// </summary>
    /// <typeparam name="T"> The type that we store in a cell, e.g. byte or double or decimal.
    /// Note: If T is 'bool' then we don't save our data in the most efficient manner (that would be the usage of 
    /// BinaryRingBuffer3D).
    /// </typeparam>
    public class GenericRingBuffer3D<T> : AbstractRingBuffer3D
    {
        //state:

        //1st dim = "ring buffer position"
        //2nd dim = "X"
        //3rd dim = "Y"
        //(X,Y) = (0,0) = "upper left corner"
        protected T[][,] ringBuffer;


        //c'tors:

        /// <summary>
        /// Make a new instance (first c'tor variant).
        /// </summary>
        /// <param name="mem">number of memory slots in the ring buffer (1st dimension)</param>
        /// <param name="x">number of cells in x direction (2nd dimension)</param>
        /// <param name="y">number of cells in y direction (3rd dimension)</param>
        public GenericRingBuffer3D(int mem, int x, int y) : base(mem, x, y)
        {
            ringBuffer = new T[mem][,];

            //earlier idea: reserve memory now for the entire ring buffer
            //-----------------------------------------------------------
            // for(int i = 0; i < mem; i++)
            //     ringBuffer[i] = new T[x, y];
            //
            // public int CellsX => ringBuffer[0].GetLength(0);
            // public int CellsY => ringBuffer[0].GetLength(1);
            //-----------------------------------------------------------

            //current idea: reserve memory for the first generation only
            MakeGeneration(0, false);
        }

        /// <summary>
        /// Make a new instance, copy from other instance (copy c'tor A, second c'tor variant). 
        /// Makes a copy of the other ring buffer (but only copies useful data, to safe time).
        /// </summary>
        /// <param name="other">the other instance</param>
        public GenericRingBuffer3D(GenericRingBuffer3D<T> other) : this(other.MemSlots, other.CellsX, other.CellsY)
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
                MakeGeneration(i, false);

                Array.Copy(other.ringBuffer[i], ringBuffer[i], ringBuffer[i].Length);

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
        public GenericRingBuffer3D(int mem, int x, int y, GenericRingBuffer3D<T> other) : this(mem, x, y)
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
                MakeGeneration(i, true);

                int ox = other.CellsX;
                int oy = other.CellsY;
                T[,] arr = ringBuffer[i];
                T[,] oarr = other.ringBuffer[i];
                for (int a = 0; a < x; a++)
                {
                    if (a >= ox)
                        break;
                    for (int b = 0; b < y; b++)
                    {
                        if (b >= oy)
                            break;
                        arr[a, b] = oarr[a, b];
                    }
                }

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
        public GenericRingBuffer3D(int mem, int x, int y, GenericRingBuffer3D<T> other, T defaultValue) : this(mem, x, y)
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
                MakeGeneration(i, defaultValue);

                int ox = other.CellsX;
                int oy = other.CellsY;
                T[,] arr = ringBuffer[i];
                T[,] oarr = other.ringBuffer[i];
                for (int a = 0; a < x; a++)
                {
                    if (a >= ox)
                        break;
                    for (int b = 0; b < y; b++)
                    {
                        if (b >= oy)
                            break;
                        arr[a, b] = oarr[a, b];
                    }
                }

                i++;
                if (i >= MemSlots)
                    i = 0;
            }
            while (i != newestGenPos);
        }


        //helper methods:

        private void MakeGeneration(int where, bool clearWithDefault)
        {
            if (ringBuffer[where] == null)
                ringBuffer[where] = new T[CellsX, CellsY];
            else
                if(clearWithDefault)
                    Array.Clear(ringBuffer[where], 0, ringBuffer[where].Length);
        }

        private void MakeGeneration(int where, T initialValue)
        {
            if (ringBuffer[where] == null)
                ringBuffer[where] = new T[CellsX, CellsY];

            //Array.Fill<T>(ringBuffer[where], initialValue, 0, ringBuffer[where].Length); //<--- not available in my version of .Net :-(

            for (int i = 0; i < CellsX; i++)
                for (int j = 0; j < CellsY; j++)
                    ringBuffer[where][i, j] = initialValue;
        }

        protected override void FreeBuffer(int pos)
        {
            ringBuffer[pos] = null;
        }


        //public methods:

        public override int MemSlots => ringBuffer.GetLength(0); //TODO: is this correct?

        public override void AddGeneration(bool clearWithDefault = false)
        {
            MoveForward();
            MakeGeneration(newestGenPos, clearWithDefault);
        }

        /// <summary>
        /// Add another entry (generation) in the ring buffer, using the specified initialValue for all cells.
        /// </summary>
        /// <param name="initialValue"></param>
        public void AddGeneration(T initialValue)
        {
            MoveForward();
            MakeGeneration(newestGenPos, initialValue);
        }

        /// <summary>
        /// Get the latest (newest) data from the ring buffer.
        /// </summary>
        public T[,] Last => ringBuffer[newestGenPos];

        /// <summary>
        /// Get the earliest (oldest) data from the ring buffer.
        /// </summary>
        public T[,] First => ringBuffer[oldestGenPos];

        /// <summary>
        /// Get the data entry right before the latest (newest) entry.
        /// </summary>
        public T[,] Previous => this[Length - 2];

        /// <summary>
        /// The indexer can be used like this: myRingBuffer[0] <--- this will return the oldest data from the ring buffer! 
        /// 0 = oldest = First  and  Length-1 = newest = Last
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public T[,] this[int i]
        {
            get {
                if (i > Length - 1)
                    throw new ArgumentException("can't be more than Length-1!");
                if (i < 0)
                    throw new ArgumentException("can't be less than zero!");
                int pos = oldestGenPos + i;
                pos %= MemSlots;
                return ringBuffer[pos];
            }
        }

        /// <summary>
        /// Try to get the content of the neighboring cell in that specific direction. 
        /// This behavior uses the "wrap-around at world boundaries" semantics.
        /// </summary>
        /// <param name="arr">the cell field</param>
        /// <param name="x">the x coordinate of the cell you are interested in</param>
        /// <param name="y">thy y coordinate of the cell you are interested in</param>
        /// <param name="direction">the direction where we find the neighbor of our cell</param>
        /// <returns>the content of the neighbor cell</returns>
        public T GetCellValueWithWrap(T[,] arr, int x, int y, Direction direction)
        {
            BoundsCheck(x, y);
            int ix = x, iy = y;
            switch (direction)
            {
                case Direction.N: if (y == 0) iy = CellsY - 1; break;
                case Direction.NE: if (x == CellsX - 1) ix = 0; if (y == 0) iy = CellsY - 1; break;
                case Direction.E: if (x == CellsX - 1) ix = 0; break;
                case Direction.SE: if (x == CellsX - 1) ix = 0; if (y == CellsY - 1) iy = 0; break;
                case Direction.S: if (y == CellsY - 1) iy = 0; break;
                case Direction.SW: if (x == 0) ix = CellsX - 1; if (y == CellsY - 1) iy = 0; break;
                case Direction.W: if (x == 0) ix = CellsX - 1; break;
                case Direction.NW: if (x == 0) ix = CellsX - 1; if (y == 0) iy = CellsY - 1; break;
            }
            return arr[ix, iy];
        }

        /// <summary>
        /// Try to get the content of the neighboring cell in that specific direction. 
        /// When there is no neighbor due to missing "wrap around" semantics, a default value is 
        /// needed, which you must provide.
        /// </summary>
        /// <param name="arr">the cell field</param>
        /// <param name="x">the x coordinate of the cell you are interested in</param>
        /// <param name="y">thy y coordinate of the cell you are interested in</param>
        /// <param name="direction">the direction where we find the neighbor of our cell</param>
        /// <param name="outsideVal">the default value in case our world ends here and so there is no neighboring cell</param>
        /// <returns>the content of the neighbor cell (or the default value 'outsideVal')</returns>
        public T GetCellValueWithoutWrap(T[,] arr, int x, int y, Direction direction, T outsideVal)
        {
            BoundsCheck(x, y);
            switch (direction)
            {
                case Direction.N: if (y == 0) return outsideVal; break;
                case Direction.NE: if (x == CellsX - 1) return outsideVal; if (y == 0) return outsideVal; break;
                case Direction.E: if (x == CellsX - 1) return outsideVal; break;
                case Direction.SE: if (x == CellsX - 1) return outsideVal; if (y == CellsY - 1) return outsideVal; break;
                case Direction.S: if (y == CellsY - 1) return outsideVal; break;
                case Direction.SW: if (x == 0) return outsideVal; if (y == CellsY - 1) return outsideVal; break;
                case Direction.W: if (x == 0) return outsideVal; break;
                case Direction.NW: if (x == 0) return outsideVal; if (y == 0) return outsideVal; break;
            }
            return arr[x, y];
        }
    }
}
