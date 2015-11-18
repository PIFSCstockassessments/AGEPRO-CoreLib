using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace AGEPRO_struct
{
    public class AGEPRO_inputFile
    {
        public string version { get; set; }
        public string caseID { get; set; }
        public AGEPRO_general general { get; set; }
        public AGEPRO_bootstrap bootstrap = new AGEPRO_bootstrap();
        public List <AGEPRO_Recruitment> recruitList { get; set; }
        public AGEPRO_inputAgeTable stockWeight { get; set; }
        public AGEPRO_weightAgeTable SSBWeight = new AGEPRO_weightAgeTable (new int[] {1,0,-1});
        public AGEPRO_weightAgeTable meanWeight = new AGEPRO_weightAgeTable (new int[] {1,0,-1,-2});
        public AGEPRO_weightAgeTable catchWeight = new AGEPRO_weightAgeTable (new int[] {1,0,-1,-2,-3} );
        public AGEPRO_weightAgeTable naturalMortality = new AGEPRO_weightAgeTable(new int[] { 1, 0, -1, -2, -3, -4 });
        public AGEPRO_biological biological = new AGEPRO_biological();
        public AGEPRO_inputAgeTable maturity { get; set; }
        public AGEPRO_inputAgeTable fishery { get; set; }
        public AGEPRO_inputAgeTable discardWeight { get; set; }
        public AGEPRO_miscOptions.retroAdjustmentFactors retroAdjust = new AGEPRO_miscOptions.retroAdjustmentFactors(); //retroAdjust
        public AGEPRO_harvestScenario harvestScenario { get; set; }
        public AGEPRO_inputAgeTable discardFraction { get; set; }
        public AGEPRO_miscOptions.Bounds bounds = new AGEPRO_miscOptions.Bounds(); //bounds
        public AGEPRO_miscOptions options = new AGEPRO_miscOptions(); //options
        public AGEPRO_miscOptions.ScaleFactors scale = new AGEPRO_miscOptions.ScaleFactors(); //scale
        public AGEPRO_miscOptions.ReportPercentile reportPercentile = new AGEPRO_miscOptions.ReportPercentile(); //reportPercentile
        public AGEPRO_miscOptions.Refpoint refpoint = new AGEPRO_miscOptions.Refpoint(); //refpoint
        public AGEPRO_harvestScenario.Rebuild rebuild = new AGEPRO_harvestScenario.Rebuild();
        public AGEPRO_harvestScenario.PStar pstar = new AGEPRO_harvestScenario.PStar(); 
        
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
                    string[] generalLine = line.Split(' ');
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
                else if (line.Equals("[RECRUIT]"))
                {
                    //Read Recruit Data
                }
                else if (line.Equals("[STOCK_WEIGHT]"))
                {
                    this.stockWeight.ReadInputAgeData(sr, this.general.NumYears(), this.general.NumAges());
                }
                else if (line.Equals("[SSB_WEIGHT]"))
                {
                    this.SSBWeight.ReadInputAgeData(sr, this.general.NumYears(), this.general.NumAges());
                }
                else if (line.Equals("[MEAN_WEIGHT]"))
                {
                    this.meanWeight.ReadInputAgeData(sr, this.general.NumYears(), this.general.NumAges());
                }
                else if (line.Equals("[CATCH_WEIGHT]"))
                {
                    this.catchWeight.ReadInputAgeData(sr, this.general.NumYears(), this.general.NumAges(), this.general.numFleets);
                }
                else if (line.Equals("[DISC_WEIGHT]"))
                {
                    this.discardWeight.ReadInputAgeData(sr, this.general.NumYears(), this.general.NumAges(), this.general.numFleets);
                }
                else if (line.Equals("[NATMORT]"))
                {
                    this.naturalMortality.ReadInputAgeData(sr, this.general.NumYears(), this.general.NumAges());
                }
                else if (line.Equals("[MATURITY]"))
                {
                    this.maturity.ReadInputAgeData(sr, this.general.NumYears(), this.general.NumAges());
                }
                else if (line.Equals("[FISHERY]"))
                {
                    this.fishery.ReadInputAgeData(sr, this.general.NumYears(), this.general.NumAges(), this.general.numFleets);
                }
                else if (line.Equals("[DISCARD]"))
                {
                    this.discardFraction.ReadInputAgeData(sr, this.general.NumYears(), this.general.NumAges(), this.general.numFleets);
                }
                else if (line.Equals("[BIOLOGICAL]"))
                {
                    this.biological.ReadBiologicalData(sr, this.general.SeqYears());
                }
                else if (line.Equals("[BOOTSTRAP]"))
                {
                    this.bootstrap.ReadBootstrapData(sr);
                }
                else if (line.Equals("[HARVEST]"))
                {
                    this.harvestScenario.ReadHarvestTable(sr, this.general.NumYears(), this.general.numFleets);
                }
                else if (line.Equals("[REBUILD]"))
                {
                    this.rebuild.ReadRebuildData(sr);
                }
                else if (line.Equals("[REFPOINT]"))
                {
                    AGEPRO_miscOptions.enableRefpoint = true;
                    line = sr.ReadLine();
                    string[] refpointOpt = line.Split(' ');
                    this.refpoint.refSSB = Convert.ToDouble(refpointOpt[0]);
                    this.refpoint.refStockBio = Convert.ToDouble(refpointOpt[1]);
                    this.refpoint.refMeanBio = Convert.ToDouble(refpointOpt[2]);
                    this.refpoint.refFMort = Convert.ToDouble(refpointOpt[3]);
                }
                else if (line.Equals("[BOUNDS]"))
                {
                    AGEPRO_miscOptions.enableBounds = true;
                    line = sr.ReadLine();
                    string[] boundsOpt = line.Split(' ');
                    this.bounds.maxWeight = Convert.ToDouble(boundsOpt[0]);
                    this.bounds.maxNatMort = Convert.ToDouble(boundsOpt[1]);
                }
                else if (line.Equals("[RETROADJUST]"))
                {
                    AGEPRO_miscOptions.enableRetroAdjustmentFactors = true;
                    line = sr.ReadLine();
                    string[] rafLine = line.Split(' ');
                    DataTable retroAdjTable = new DataTable();

                    //TODO: throw warning/error if 'rafLine' length doesn't match number of Ages

                    for (int i = 0; i < this.general.NumAges(); i++)
                    {
                        retroAdjTable.Rows.Add(rafLine[i]);
                    }
                }
                else if (line.Equals("[OPTIONS]"))
                {
                    line = sr.ReadLine();
                    string[] optionOpt = line.Split(' ');
                    this.options.enableSummaryReport = Convert.ToBoolean(Convert.ToInt32(optionOpt[0]));
                    this.options.enableDataFiles = Convert.ToBoolean(Convert.ToInt32(optionOpt[1]));
                    this.options.enableExportR = Convert.ToBoolean(Convert.ToInt32(optionOpt[2]));
                }
                else if (line.Equals("[SCALE]"))
                {
                    AGEPRO_miscOptions.enableScaleFactors = true;
                    line = sr.ReadLine();
                    string[] scaleOpt = line.Split(' ');
                    this.scale.scaleBio = Convert.ToDouble(scaleOpt[0]);
                    this.scale.scaleRec = Convert.ToDouble(scaleOpt[1]);
                    this.scale.scaleStockNum = Convert.ToDouble(scaleOpt[2]);
                }
                else if (line.Equals("[PERC]"))
                {
                    AGEPRO_miscOptions.enablePercentileReport = true;
                    this.reportPercentile.percentile = Convert.ToDouble(sr.ReadLine());
                }
                else if (line.Equals("[PSTAR]"))
                {
                    this.pstar.ReadPStarData(sr);
                }
            }

        }
    }
}
