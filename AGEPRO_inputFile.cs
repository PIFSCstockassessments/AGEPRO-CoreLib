using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AGEPRO_struct
{
    public class AGEPRO_inputFile
    {
        public string version { get; set; }
        public string caseID { get; set; }
        public AGEPRO_general general { get; set; }
        //bootstrap
        public List <Recruitment> recruitList { get; set; }
        public AGEPRO_inputAgeTable stockWeight { get; set; }
        public AGEPRO_weightAgeTable SSBWeight { get; set; }
        public AGEPRO_weightAgeTable meanWeight { get; set; }
        public AGEPRO_weightAgeTable catchWeight { get; set; }
        public AGEPRO_weightAgeTable naturalMortality { get; set; }
        //biologoical
        public AGEPRO_inputAgeTable maturity { get; set; }
        public AGEPRO_inputAgeTable fishery { get; set; }
        public AGEPRO_inputAgeTable discardWeight { get; set; }
        //retroAdjust
        public AGEPRO_inputAgeTable harvestScenario { get; set; }
        public AGEPRO_inputAgeTable discardFraction { get; set; }
        //bounds
        //options
        //scale
        //reportPercentile
        //refpoint
        //rebuild
        //pstar

        public void ReadInputFile(string file)
        {
            try
            {
                //string[] lines = File.ReadAllLines(file);
                //ReadInputFileLineValues(lines);
                using (StreamReader inReader = new StreamReader(file))
                {
                    ReadInputFileLineValues(inReader);
                }

            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ReadInputFileLineValues(StreamReader sr)
        {
            string line;
            

            line = sr.ReadLine();
            //AGEPRO (Input File) Version
            if (line.Equals("AGEPRO VERSION 4.0"))
            {
                this.version = line;
            }
            else
            {
                //Throw Error/Warning for incompatiability
            }

            while(!sr.EndOfStream)
            {
                line = sr.ReadLine();
                //Case ID
                if (line.Equals("[CASEID]")) 
                {
                    this.caseID = sr.ReadLine();
                }
                //General
                else if (line.Equals("[GENERAL]"))
                {
                    line = sr.ReadLine();
                    string [] generalLine = line.Split(' ');
                    this.general.projYearStart = Convert.ToInt32(generalLine[0]);
                    this.general.projYearEnd = Convert.ToInt32(generalLine[1]);
                    this.general.ageBegin = Convert.ToInt32(generalLine[2]);
                    this.general.ageEnd = Convert.ToInt32(generalLine[3]);
                    this.general.numPopSims = Convert.ToInt32(generalLine[4]);
                    this.general.numFleets = Convert.ToInt32(generalLine[5]);
                    this.general.numRecModels = Convert.ToInt32(generalLine[6]);
                    this.general.seed = Convert.ToInt32(generalLine[8]);
                    if (generalLine[7].Equals('1'))
                    {
                        this.general.hasDiscards = true;
                    }
                    else
                    {
                        this.general.hasDiscards = false;
                    }

                    
                    
                }
                else if(line.Equals("[RECRUIT]"))
                {
                    //Read Recruit Data
                }
                else if (line.Equals("[STOCK_WEIGHT]"))
                {
                    this.stockWeight.ReadInputAgeData(sr, this.general.numYears(), this.general.numAges());
                }
                else if (line.Equals("[SSB_WEIGHT]"))
                {
                    this.SSBWeight.ReadInputAgeData(sr, this.general.numYears(), this.general.numAges());
                }
                else if (line.Equals("[MEAN_WEIGHT]"))
                {
                    this.meanWeight.ReadInputAgeData(sr, this.general.numYears(), this.general.numAges());
                }
            }

        }
    }
}
