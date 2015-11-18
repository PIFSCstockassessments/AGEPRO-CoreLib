using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AGEPRO_struct
{
    public class AGEPRO_bootstrap
    {
        public int numBootstraps { get; set; }
        public double popScaleFactor { get; set; }
        public string bootstrapFile { get; set; }

        public AGEPRO_bootstrap()
        {

        }

        public void ReadBootstrapData(StreamReader sr)
        {
            string line;
            
            line = sr.ReadLine();
            string[] bootstrapOptions = line.Split(' ');
            //Number of Bootstraps
            this.numBootstraps = Convert.ToInt32(bootstrapOptions[0]); 
            //Pop. Scale Factor
            this.popScaleFactor = Convert.ToDouble(bootstrapOptions[1]);
            //Bootstrap File
            line = sr.ReadLine();
            this.bootstrapFile = line;

        }
    }
}
