using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AGEPRO.CoreLib
{
    /// <summary>
    /// General Paramemeters of AGEPRO
    /// </summary>
    public class AGEPRO_General
    {
        public int projYearStart { get; set; }    //First Year in Projection
        public int projYearEnd { get; set; }     //Last Year in Projection
        public int ageBegin { get; set; }        //First Age Class
        public int ageEnd { get; set; }           //Last Age Class
        public int numFleets { get; set; }       //Number of Fleets
        public int numRecModels { get; set; }    //Number of Recruit Models
        public int numPopSims { get; set; }      //Number of Population Simulations
        public bool hasDiscards { get; set; }     //Discards are Present
        public int seed { get; set; }            //Random Number Seed
        public string inputFile { get; set; }

        public AGEPRO_General()
        {
        }

        public AGEPRO_General(string file)
        {
            this.inputFile = file;
            //readin file contents
        }
        
        /// <summary>
        /// Determine number of years in projection by the (absolulte) diffefence between the 
        /// last and first year of projection. 
        /// </summary>
        /// <returns>The difference stored in 'nYears'</returns>
        public int NumYears()
        {
            int nYears = Math.Abs(this.projYearEnd - this.projYearStart) + 1;
            return nYears;
        }

        /// <summary>
        /// Determine number of ages in projection by the (absolulte) diffefence between last age 
        /// class and first age class of projection. 
        /// </summary>
        /// <returns>The difference in stored in 'nAges'</returns>
        public int NumAges()
        {
            int nAges = Math.Abs(this.ageBegin - this.ageEnd) + 1;
            return nAges;
        }

        /// <summary>
        /// Returns a sequence of years from First year of projection
        /// </summary>
        /// <returns>Returns a int array from <paramref name="projYearStart"/> by <paramref name="NumYears"/></returns>
        public int[] SeqYears()
        {
            return Enumerable.Range(this.projYearStart, this.NumYears()).ToArray();
        }

        
    }
}
