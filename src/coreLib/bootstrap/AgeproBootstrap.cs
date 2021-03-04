using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Nmfs.Agepro.CoreLib
{
    /// <summary>
    /// AGEPRO Bootstrapping Options
    /// </summary>
    /// <remarks>
    /// -Number of data values of each row must equal to the number of age classes.
    /// -The number of rows in a bootstrap file must be at least equal to the number of bootstrap 
    /// iterations containing the popluation of the first year in the projection
    /// </remarks>
    public class AgeproBootstrap
    {
        private int _numBootstraps;
        private double _popScaleFactor;
        private string _bootstrapFile;

        public int numBootstraps
        { 
            get { return _numBootstraps; }
            set { SetProperty(ref _numBootstraps, value); }
        }
        public double popScaleFactor 
        {
            get { return _popScaleFactor; }
            set { SetProperty(ref _popScaleFactor, value); }
        }
        public string bootstrapFile 
        {
            get { return _bootstrapFile; }
            set { SetProperty(ref _bootstrapFile, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AgeproBootstrap()
        {
            numBootstraps = 0; 
            popScaleFactor = 0;
        }

        /// <summary>
        /// Reading Bootstrap Options from AGEPRO Input File Stream
        /// </summary>
        /// <param name="sr">AGEPRO Input File StreamReader</param>
        public void ReadBootstrapData(StreamReader sr)
        {
            string line;
            
            line = sr.ReadLine();
            string[] bootstrapOptions = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //Number of Bootstraps
            this.numBootstraps = Convert.ToInt32(bootstrapOptions[0]); 
            //Pop. Scale Factor
            this.popScaleFactor = Convert.ToDouble(bootstrapOptions[1]);
            //Bootstrap File
            line = sr.ReadLine();
            this.bootstrapFile = line;

            //TODO: Send error/warning if bootstrapFile is not found in system

        }

         
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
        public void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
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
