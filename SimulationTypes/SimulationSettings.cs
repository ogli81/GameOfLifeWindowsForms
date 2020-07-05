using System.Drawing;

namespace GameOfLifeWindowsForms
{
    /// <summary>
    /// The settings of a simulation comprises things like "how many cells" and 
    /// "does the world wrap at its borders" and "how many states from the past 
    /// should we keep as history" and things like that.
    /// </summary>
    public class SimulationSettings
    {
        /// <summary>
        /// The simulation for which we will call 'SettingsChanged' after changes to our state.
        /// </summary>
        public ICellSimulation Sim { get => sim; set { sim = value; sim.SettingsChanged(); } }
        protected ICellSimulation sim;

        //c'tor
        public SimulationSettings()
        {
        }

        /// <summary>
        /// Number of field instances that we remember. Example: remember 100 cell fields from the past => MemSlots = 100.
        /// </summary>
        public int MemSlots { get => memSlots; set { memSlots = value; sim.SettingsChanged(); } }
        protected int memSlots = 100;
        /// <summary>
        /// How many cells in x direction? Example: 32 x 32 field => SizeX = 32.
        /// </summary>
        public int SizeX { get => sizeX; set { sizeX = value; sim.SettingsChanged(); } }
        protected int sizeX = 32;
        /// <summary>
        /// How many cells in y direction? Example: 32 x 32 field => SizeY = 32.
        /// </summary>
        public int SizeY { get => sizeY; set { sizeY = value; sim.SettingsChanged(); } }
        protected int sizeY = 32;

        /// <summary>
        /// Do we have a wrap-around? (wrap = "if I leave east border, I enter from west border again...")
        /// </summary>
        public bool IsWrap { get => isWrap; set { isWrap = value; sim.SettingsChanged(); } }
        protected bool isWrap = true;

        /// <summary>
        /// Should we track the overall life sum as statistics?
        /// </summary>
        public bool TrackLifeStats { get => trackLifeStats; set { trackLifeStats = value; sim.SettingsChanged(); } }
        protected bool trackLifeStats = false;
        /// <summary>
        /// How many data slots does the life sum statistics have?
        /// </summary>
        public int LifeStatsMem { get => lifeStatsMem; set { lifeStatsMem = value; sim.SettingsChanged(); } }
        protected int lifeStatsMem = 1_000_000;

        /// <summary>
        /// How should the program react when MemSlots is exceeded?
        /// </summary>
        public MemFullBehavior MemSlotsFullBehavior { get => memSlotsFullBehavior; set { memSlotsFullBehavior = value; sim.SettingsChanged(); } }
        protected MemFullBehavior memSlotsFullBehavior = MemFullBehavior.FORGET_SILENTLY;
        /// <summary>
        /// How should the program react when LifeStatsMem is exceeded?
        /// </summary>
        public MemFullBehavior LifeStatsFullBehavior { get => lifeStatsFullBehavior; set { lifeStatsFullBehavior = value; sim.SettingsChanged(); } }
        protected MemFullBehavior lifeStatsFullBehavior = MemFullBehavior.FORGET_SILENTLY;
    }
}