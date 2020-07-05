namespace GameOfLifeWindowsForms
{
    /// <summary>
    /// How should the program react when memory is full?
    /// </summary>
    public enum MemFullBehavior
    {
        /// <summary>
        /// Forget old data, don't report anything.
        /// </summary>
        FORGET_SILENTLY,
        
        //FORGET_AND_LOG, //write to logfile
        
        /// <summary>
        /// Stop working (further attempts to continue the work will be ignored silently).
        /// </summary>
        STOP_SILENTLY,

        //STOP_AND_LOG, //write to logfile

        /// <summary>
        /// An exception is thrown (and usually the program is terminated due to that exception).
        /// </summary>
        THROW_EXCEPTION
    }
}