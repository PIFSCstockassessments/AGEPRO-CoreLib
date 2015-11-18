using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AGEPRO_struct
{
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
        
        public int NumYears()
        {
            int nYears = Math.Abs(this.projYearEnd - this.projYearStart) + 1;
            return nYears;
        }

        public int NumAges()
        {
            int nAges = Math.Abs(this.ageBegin - this.ageEnd) + 1;
            return nAges;
        }

        public int[] SeqYears()
        {
            return Enumerable.Range(this.projYearStart, this.projYearEnd).ToArray();
        }

        private void ReadInputFile(string file)
        {
            try
            {
                //open file
                using (StreamReader inFile = new StreamReader(file))
                {
                        
                }
            }
            catch(IOException ex)
            {
                Console.WriteLine(ex.Message);
            }

           

        }
    }
}
