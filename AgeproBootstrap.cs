using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AGEPRO_struct
{
    /// <summary>
    /// AGEPRO Bootstrapping Options
    /// </summary>
    /// <remarks>
    /// -Number of data values of each row must equal to the number of age classes.
    /// -The number of rows in a bootstrap file must be at least equal to the number of bootstrap 
    /// iterations containing the popluation of the first year in the projection
    /// </remarks>
    public class AGEPRO_Bootstrap
    {
        public int numBootstraps { get; set; }
        public double popScaleFactor { get; set; }
        public string bootstrapFile { get; set; }

        public AGEPRO_Bootstrap()
        {

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
    }
}
