using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Nmfs.Agepro.CoreLib
{
    /// <summary>
    /// Generalized, abstract represetion for (Non-Standard) Harvest Scenario Calculations
    /// </summary>
    abstract public class HarvestCalculation : INotifyPropertyChanged, IValidatable
    {
        public HarvestScenarioAnalysis calculationType;
        public int[] obsYears;
        public abstract void ReadCalculationDataLines(StreamReader sr);
        public abstract List<string> WriteCalculationDataLines();


        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        ///     Checks if a property already matches a desired value.  Sets the property and
        ///     notifies listeners only when necessary. In AGEPRO, allows a GUI object to set
        ///     values with its the data-binded CoreLib property.
        /// </summary>
        /// <remarks>
        ///     Derived From DanRigby
        /// </remarks>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="field">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="name">
        ///     Name of the property used to notify listeners.  This
        ///     value is optional and can be provided automatically when invoked from compilers that
        ///     support CallerMemberName.
        /// </param>
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

        public virtual ValidationResult ValidateInput()
        {
            throw new NotImplementedException();
        }
    }
}
