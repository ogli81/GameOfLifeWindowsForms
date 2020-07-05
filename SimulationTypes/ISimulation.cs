using GameOfLifeWindowsForms.SimulationTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeWindowsForms
{
    /// <summary>
    /// This interface is used for our cellular automaton simulation classes. You can 
    /// set the values for each cell (with its x- and y-coordinates) and you can 
    /// execute the next simulation step (the next "generation") and do other things.
    /// </summary>
    public interface ICellSimulation
    {
        /// <summary>
        /// Gives some human-readable information on what this simulation is all about.
        /// </summary>
        string Info { get; }

        /// <summary>
        /// Get the settings of the simulation. These settings are compatible with all kinds 
        /// of cellular automaton simulations. Usually you inject the settings object via 
        /// the c'tor of the simulation class.
        /// </summary>
        SimulationSettings Settings { get; }
        /// <summary>
        /// Calling this method informs the simulation that settings may have changed.
        /// </summary>
        void SettingsChanged();

        /// <summary>
        /// Get the id-number of the current "generation".
        /// </summary>
        int CurrentGen { get; }
        /// <summary>
        /// Get the id-number of the oldest "generation".
        /// </summary>
        int OldestGen { get; }
        /// <summary>
        /// Get the overall number of "generations" (usually it's CurrentGen-OldestGen+1).
        /// </summary>
        int NumGens { get; }

        /// <summary>
        /// Each simulation may have up to 2 adjustable parameters - this is the first one. 
        /// If the simulation doesn't have a first parameter, null will be returned.
        /// </summary>
        SimulationParameter? Param1 { get; }
        /// <summary>
        /// Each simulation may have up to 2 adjustable parameters - this is the second one. 
        /// If the simulation doesn't have a second parameter, null will be returned.
        /// </summary>
        SimulationParameter? Param2 { get; }

        /// <summary>
        /// If tracking of life statistics is enabled, you can get the list of values which 
        /// is the statistic of "sum of life value over all cells of the simulation".<br></br>
        /// ATTENTION: May be null (if TrackLifeStats is false)!<br></br>
        /// 
        /// </summary>
        //List<decimal> LifeStats { get; }
        //TODO: should be compatible with LiveCharts!

        /// <summary>
        /// Calculate the next "generation". Will advance the CurrentGen by +1.
        /// </summary>
        /// <returns>true, if further calculation was not possible (and true otherwise)</returns>
        bool CalculateNextGen();
        /// <summary>
        /// Move back to the oldest "generation" which is stored in the history. The 
        /// CurrentGen will be set to OldestGen (and all other "generations" will be discarded).
        /// </summary>
        void GoToOldestGen();
        /// <summary>
        /// Move back along the history (if possible). CurrentGen will be reduced by 1 and true 
        /// will be returned (if possible). If we do not have more than one "generation" in our 
        /// history, then nothing will change and false will be returnded.
        /// possible) an
        /// </summary>
        /// <returns>true, if going back was possible (and false otherwise)</returns>
        bool GoBackOneGen();
        /// <summary>
        /// Gives the CurrentGen the id-number zero and discards the complete history (except 
        /// for the current "generation").
        /// </summary>
        void RelabelCurrentAsZero();
        /// <summary>
        /// Go back or forth to a "generation" with that specific id-number. Going back will only 
        /// be possible if an older "generation" with that id-number is available in the history. 
        /// Going forth will always be possible (but may take a long time to calculate).<br></br>
        /// ATTENTION: If you set a very high value, the simulation may run for a very long time 
        /// (and call 'CalculateNextGen' very often).
        /// </summary>
        /// <param name="genId">
        /// The id-number of the "generation" that you want to go to. May be an older id-number 
        /// (of a "generation" before CurrentGen). May be a newer id-number or the same as the 
        /// CurrentGen. Older "generations" must be looked up in the history. Newer "generations" 
        /// must be calculated (via 'CalculateNextGen').<br></br>
        /// ATTENTION: If you set a very high value, the simulation may run for a very long time 
        /// (and call 'CalculateNextGen' very often).
        /// </param>
        /// <returns>true, if going to the "generation" with that id-number was possible (and false otherwise)</returns>
        bool GoToGen(int genId);

        /// <summary>
        /// Gives you the value that is stored in that cell (with that x and y coordinates inside the cell field).
        /// A value between 0.0(inclusive) and 1.0(inclusive) is returned, so a value from [0.0..1.0] where 
        /// 0.0 is "fully dead" and 1.0 is "fully alife".
        /// </summary>
        /// <param name="x">The x coordinate of the cell (the cell at the "Left" or "West" border has a x coordinate of zero).</param>
        /// <param name="y">The y coordinate of the cell (the cell at the "Top" or "North" border has a y coordinate of zero).</param>
        /// <returns>A value between 0.0(inclusive) and 1.0(inclusive), so a value from [0.0..1.0] where 
        /// 0.0 is "fully dead" and 1.0 is "fully alife".</returns>
        decimal GetCellValue(int x, int y);
        /// <summary>
        /// You may change the value of a cell, using this method. A value between 0.0(inclusive) and 1.0(inclusive) may be stored
        /// in the cell as its "life value". Lower or higher values will be capped to 0.0 or 1.0 respectively.
        /// </summary>
        /// <param name="x">The x coordinate of the cell (the cell at the "Left" or "West" border has a x coordinate of zero).</param>
        /// <param name="y">The y coordinate of the cell (the cell at the "Top" or "North" border has a y coordinate of zero).</param>
        /// <param name="val">
        /// The "life value" that you want to set (should be from the interval [0.0..1.0]). 
        /// Lower or higher values will be capped to 0.0 or 1.0 respectively.
        /// </param>
        void SetCellValue(int x, int y, decimal val);
    }
}