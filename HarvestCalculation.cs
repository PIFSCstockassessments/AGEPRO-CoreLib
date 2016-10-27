using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace AGEPRO.CoreLib
{
    public enum HarvestScenarioAnalysis
    {
        HarvestScenario,
        Rebuilder,
        PStar,
    };

    /// <summary>
    /// Generalized, abstract represetion for (Non-Standard) Harvest Scenario Calculations
    /// </summary>
    abstract public class HarvestCalculation : INotifyPropertyChanged
    {
        public HarvestScenarioAnalysis calculationType;
        public abstract void ReadCalculationDataLines(StreamReader sr);
        public abstract List<string> WriteCalculationDataLines();


        public event PropertyChangedEventHandler PropertyChanged;


        //DanRigby
        /// <summary>
        /// Allows two-way data binding to GUI.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="name"></param>
        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }
        }
    }
}
