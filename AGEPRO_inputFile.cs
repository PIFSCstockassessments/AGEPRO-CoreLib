using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace AGEPRO_struct
{
    public class AGEPRO_InputFile
    {
        public string version { get; set; }
        public string caseID { get; set; }
        public AGEPRO_General general = new AGEPRO_General();
        public AGEPRO_Bootstrap bootstrap = new AGEPRO_Bootstrap();
        public AGEPRO_Recruitment recruitment = new AGEPRO_Recruitment();
        public AGEPRO_InputAgeTable stockWeight = new AGEPRO_InputAgeTable();
        public AGEPRO_WeightAgeTable SSBWeight = new AGEPRO_WeightAgeTable (new int[] {1,0,-1});
        public AGEPRO_WeightAgeTable meanWeight = new AGEPRO_WeightAgeTable (new int[] {1,0,-1,-2});
        public AGEPRO_WeightAgeTable catchWeight = new AGEPRO_WeightAgeTable (new int[] {1,0,-1,-2,-3} );
        public AGEPRO_WeightAgeTable naturalMortality = new AGEPRO_WeightAgeTable(new int[] { 1, 0, -1, -2, -3, -4 });
        public AGEPRO_Biological biological = new AGEPRO_Biological();
        public AGEPRO_InputAgeTable maturity = new AGEPRO_InputAgeTable();
        public AGEPRO_InputAgeTable fishery = new AGEPRO_InputAgeTable();
        public AGEPRO_InputAgeTable discardWeight = new AGEPRO_InputAgeTable(); //discard weight
        public AGEPRO_MiscOptions.retroAdjustmentFactors retroAdjustOption = new AGEPRO_MiscOptions.retroAdjustmentFactors(); //retroAdjust
        public AGEPRO_HarvestScenario harvestScenario = new AGEPRO_HarvestScenario();
        public AGEPRO_InputAgeTable discardFraction = new AGEPRO_InputAgeTable(); //discard fraction
        public AGEPRO_MiscOptions.Bounds bounds = new AGEPRO_MiscOptions.Bounds(); //bounds
        public AGEPRO_MiscOptions options = new AGEPRO_MiscOptions(); //options
        public AGEPRO_MiscOptions.ScaleFactors scale = new AGEPRO_MiscOptions.ScaleFactors(); //scale
        public AGEPRO_MiscOptions.ReportPercentile reportPercentile = new AGEPRO_MiscOptions.ReportPercentile(); //reportPercentile
        public AGEPRO_MiscOptions.Refpoint refpoint = new AGEPRO_MiscOptions.Refpoint(); //refpoint
        public RebuilderTarget rebuild = new RebuilderTarget(); //rebuilder
        public PStar pstar = new PStar(); 
        
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
                    string[] generalLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    this.general.projYearStart = Convert.ToInt32(generalLine[0]);
                    this.general.projYearEnd = Convert.ToInt32(generalLine[1]);
                    this.general.ageBegin = Convert.ToInt32(generalLine[2]);
                    this.general.ageEnd = Convert.ToInt32(generalLine[3]);
                    this.general.numPopSims = Convert.ToInt32(generalLine[4]);
                    this.general.numFleets = Convert.ToInt32(generalLine[5]);
                    this.general.numRecModels = Convert.ToInt32(generalLine[6]);
                    this.general.seed = Convert.ToInt32(generalLine[8]);
                    if (generalLine[7].Equals("1"))
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
                    this.recruitment.observationYears = this.general.SeqYears();
                    this.recruitment.ReadRecruitmentData(sr,general.NumYears(),general.numRecModels);
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
                    AGEPRO_MiscOptions.enableRefpoint = true;
                    line = sr.ReadLine();
                    string[] refpointOpt = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    this.refpoint.refSSB = Convert.ToDouble(refpointOpt[0]);
                    this.refpoint.refStockBio = Convert.ToDouble(refpointOpt[1]);
                    this.refpoint.refMeanBio = Convert.ToDouble(refpointOpt[2]);
                    this.refpoint.refFMort = Convert.ToDouble(refpointOpt[3]);
                }
                else if (line.Equals("[BOUNDS]"))
                {
                    AGEPRO_MiscOptions.enableBounds = true;
                    line = sr.ReadLine();
                    string[] boundsOpt = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    this.bounds.maxWeight = Convert.ToDouble(boundsOpt[0]);
                    this.bounds.maxNatMort = Convert.ToDouble(boundsOpt[1]);
                }
                else if (line.Equals("[RETROADJUST]"))
                {
                    AGEPRO_MiscOptions.enableRetroAdjustmentFactors = true;
                    line = sr.ReadLine();
                    string[] rafLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    DataTable rafTable = new DataTable();
                    rafTable.Columns.Add(); //set column without name
                    //TODO: throw warning/error if 'rafLine' length doesn't match number of Ages

                    for (int i = 0; i < this.general.NumAges(); i++)
                    {
                        rafTable.Rows.Add(rafLine[i]);
                    }

                    this.retroAdjustOption.retroAdjust = rafTable;
                }
                else if (line.Equals("[OPTIONS]"))
                {
                    line = sr.ReadLine();
                    string[] optionOpt = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    this.options.enableSummaryReport = Convert.ToBoolean(Convert.ToInt32(optionOpt[0]));
                    this.options.enableDataFiles = Convert.ToBoolean(Convert.ToInt32(optionOpt[1]));
                    this.options.enableExportR = Convert.ToBoolean(Convert.ToInt32(optionOpt[2]));
                }
                else if (line.Equals("[SCALE]"))
                {
                    AGEPRO_MiscOptions.enableScaleFactors = true;
                    line = sr.ReadLine();
                    string[] scaleOpt = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    this.scale.scaleBio = Convert.ToDouble(scaleOpt[0]);
                    this.scale.scaleRec = Convert.ToDouble(scaleOpt[1]);
                    this.scale.scaleStockNum = Convert.ToDouble(scaleOpt[2]);
                }
                else if (line.Equals("[PERC]"))
                {
                    AGEPRO_MiscOptions.enablePercentileReport = true;
                    this.reportPercentile.percentile = Convert.ToDouble(sr.ReadLine());
                }
                else if (line.Equals("[PSTAR]"))
                {
                    this.pstar.ReadPStarData(sr);
                }
            }

        }

        private Dictionary<int, string> RecruitDictionary() 
        {
            //Future Feature: Generizse/Automate this Dictionary?
            Dictionary<int, string> recruitModelDictionary = new Dictionary<int, string>();

            recruitModelDictionary.Add(0, "None Selected");
            recruitModelDictionary.Add(1, "Model 1: Markov Matrix");
            recruitModelDictionary.Add(2, "Model 2: Emperical Recruits per Spawning Biomass Distribution");
            recruitModelDictionary.Add(3, "Model 3: Emperical Recruitment Distributiion");
            recruitModelDictionary.Add(4, "Model 4: Two-Stage Emperical Recruits per Spawning Biomass Distribution");
            recruitModelDictionary.Add(5, "Model 5: Beverton-Holt Curve w/ Lognormal Error");
            recruitModelDictionary.Add(6, "Model 6: Ricker Curve w/ Lognormal Error");
            recruitModelDictionary.Add(7, "Model 7: Shepherd Curve w/ Lognormal Error");
            recruitModelDictionary.Add(8, "Model 8: Lognormal Distribution");
            //Model 9 was removed in AGEPRO 4.0
            recruitModelDictionary.Add(10, "Model 10: Beverton-Holt Curve w/ Autocorrected Lognormal Error");
            recruitModelDictionary.Add(11, "Model 11: Ricker Curve w/ Autocorrected Lognormal Error");
            recruitModelDictionary.Add(12, "Model 12: Shepherd Curve w/ Autocorrected Lognormal Error");
            recruitModelDictionary.Add(13, "Model 13: Autocorrected Lognormal Distribution");
            recruitModelDictionary.Add(14, "Model 14: Emperical Cumulative Distribution Function of Recruitment");
            recruitModelDictionary.Add(15, "Model 15: Two-Stage Emperical Cumulative Distribution Function of Recruitment");
            recruitModelDictionary.Add(16, "Model 16: Linear Recruits per Spawning Biomass Predictor w/ Normal Error");
            recruitModelDictionary.Add(17, "Model 17: Loglinear Recruits per Spawning Biomass Predictor w/ Lognormal Error");
            recruitModelDictionary.Add(18, "Model 18: Linear Recruitment Predictor w/ Normal Error");
            recruitModelDictionary.Add(19, "Model 19: Loglinear Recruitment Predictor w/ Lognormal Error");
            recruitModelDictionary.Add(20, "Model 20: Fixed Recruitment");
            recruitModelDictionary.Add(21, "Model 21: Emperical Cumulative Distribution Function of Recruitment w/ Linear Decline to Zero");

            return recruitModelDictionary;
        }
    }
}
