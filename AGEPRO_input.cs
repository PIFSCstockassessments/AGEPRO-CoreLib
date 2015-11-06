using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGEPRO_struct
{
    public class AGEPRO_input
    {
        public int nYears { get; set; }
        public int nFleets { get; set; }

        public AGEPRO_input()
        {
           
        }
        public AGEPRO_input(int numYears, int numFleets)
        {
            this.nYears = numYears;
            this.nFleets = numFleets;
        }
    
    }

}
