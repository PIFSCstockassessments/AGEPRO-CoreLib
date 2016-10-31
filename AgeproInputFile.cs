using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace AGEPRO.CoreLib
{   
    /// <summary>
    /// AGEPRO Input File 
    /// </summary>
    public class AgeproInputFile
    {
        public string version { get; set; }
        public double numVer { get; set; }
        public string caseID { get; set; }
        public AgeproGeneral general = new AgeproGeneral();
        public AgeproBootstrap bootstrap = new AgeproBootstrap();
        public AgeproRecruitment recruitment = new AgeproRecruitment();
        public AgeproWeightAgeTable jan1Weight = new AgeproWeightAgeTable(new int[] {1,0}); //STOCK_WEIGHT
        public AgeproWeightAgeTable SSBWeight = new AgeproWeightAgeTable (new int[] {1,0,-1});
        public AgeproWeightAgeTable meanWeight = new AgeproWeightAgeTable (new int[] {1,0,-1,-2});
        public AgeproWeightAgeTable catchWeight = new AgeproWeightAgeTable (new int[] {1,0,-1,-2,-3} );
        public AgeproWeightAgeTable discardWeight = new AgeproWeightAgeTable(new int[] { 1, 0, -1, -2, -3, -4 }); //discard weight
        public AgeproBiological biological = new AgeproBiological(); //Fraction Mortality in Biological
        public AgeproStochasticAgeTable maturity = new AgeproStochasticAgeTable(); //Maturity in Biological
        public AgeproStochasticAgeTable fishery = new AgeproStochasticAgeTable();
        public AgeproStochasticAgeTable naturalMortality = new AgeproStochasticAgeTable(); 
        public AgeproMiscOptions.retroAdjustmentFactors retroAdjustOption = new AgeproMiscOptions.retroAdjustmentFactors(); //retroAdjust
        public AgeproHarvestScenario harvestScenario = new AgeproHarvestScenario();
        public AgeproStochasticAgeTable discardFraction = new AgeproStochasticAgeTable(); //discard fraction
        public AgeproMiscOptions.Bounds bounds = new AgeproMiscOptions.Bounds(); //bounds
        public AgeproMiscOptions options = new AgeproMiscOptions(); //options
        public AgeproMiscOptions.ScaleFactors scale = new AgeproMiscOptions.ScaleFactors(); //scale
        public AgeproMiscOptions.ReportPercentile reportPercentile = new AgeproMiscOptions.ReportPercentile(); //reportPercentile
        public AgeproMiscOptions.Refpoint refpoint = new AgeproMiscOptions.Refpoint(); //refpoint
        public RebuilderTargetCalculation rebuild = new RebuilderTargetCalculation(); //rebuilder
        public PStarCalculation pstar = new PStarCalculation();

        public AgeproInputFile()
        {
            caseID = "";
        }

        /// <summary>
        /// Initates the <paramref name="System.IO.StreamReader"/> function to read the AGEPRO Input file.
        /// </summary>
        /// <param name="file"> AGEPRO Input Filename Location</param>
        /// <remarks> Contains Try-Catch Execption Handler</remarks>
        public void ReadInputFile(string file)
        {
            
            try
            {
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

        /// <summary>
        /// Reads in the AGERPRO Input file values by line
        /// </summary>
        /// <param name="sr">SreamReader Object</param>
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
                //TODO: Throw Error/Warning for incompatiability
            }

            while(!sr.EndOfStream)
            {
                line = sr.ReadLine();
                //Case ID
                if (line.Equals("[CASEID]"))
                {
                    caseID = sr.ReadLine();
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
                    this.jan1Weight.ReadStochasticAgeData(sr, this.general.NumYears(), this.general.NumAges());
                }
                else if (line.Equals("[SSB_WEIGHT]"))
                {
                    this.SSBWeight.ReadStochasticAgeData(sr, this.general.NumYears(), this.general.NumAges());
                }
                else if (line.Equals("[MEAN_WEIGHT]"))
                {
                    this.meanWeight.ReadStochasticAgeData(sr, this.general.NumYears(), this.general.NumAges());
                }
                else if (line.Equals("[CATCH_WEIGHT]"))
                {
                    this.catchWeight.ReadStochasticAgeData(sr, this.general.NumYears(), this.general.NumAges(), this.general.numFleets);
                }
                else if (line.Equals("[DISC_WEIGHT]"))
                {
                    this.discardWeight.ReadStochasticAgeData(sr, this.general.NumYears(), this.general.NumAges(), this.general.numFleets);
                }
                else if (line.Equals("[NATMORT]"))
                {
                    this.naturalMortality.ReadStochasticAgeData(sr, this.general.NumYears(), this.general.NumAges());
                }
                else if (line.Equals("[MATURITY]"))
                {
                    this.maturity.ReadStochasticAgeData(sr, this.general.NumYears(), this.general.NumAges());
                }
                else if (line.Equals("[FISHERY]"))
                {
                    this.fishery.ReadStochasticAgeData(sr, this.general.NumYears(), this.general.NumAges(), this.general.numFleets);
                }
                else if (line.Equals("[DISCARD]"))
                {
                    this.discardFraction.ReadStochasticAgeData(sr, this.general.NumYears(), this.general.NumAges(), this.general.numFleets);
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
                    this.rebuild.ReadCalculationDataLines(sr);
                    this.harvestScenario.analysisType = HarvestScenarioAnalysis.Rebuilder;
                }
                else if (line.Equals("[REFPOINT]"))
                {
                    this.options.enableRefpoint = true;
                    line = sr.ReadLine();
                    string[] refpointOpt = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    this.refpoint.refSpawnBio = Convert.ToDouble(refpointOpt[0]);
                    this.refpoint.refJan1Bio = Convert.ToDouble(refpointOpt[1]);
                    this.refpoint.refMeanBio = Convert.ToDouble(refpointOpt[2]);
                    this.refpoint.refFMort = Convert.ToDouble(refpointOpt[3]);
                }
                else if (line.Equals("[BOUNDS]"))
                {
                    this.options.enableBounds = true;
                    line = sr.ReadLine();
                    string[] boundsOpt = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    this.bounds.maxWeight = Convert.ToDouble(boundsOpt[0]);
                    this.bounds.maxNatMort = Convert.ToDouble(boundsOpt[1]);
                }
                else if (line.Equals("[RETROADJUST]"))
                {
                    this.options.enableRetroAdjustmentFactors = true;
                    line = sr.ReadLine();
                    string[] rafLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    DataTable rafTable = new DataTable("Retro Adjustment Factors");
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
                    this.options.enableAuxStochasticFiles = Convert.ToBoolean(Convert.ToInt32(optionOpt[1]));
                    this.options.enableExportR = Convert.ToBoolean(Convert.ToInt32(optionOpt[2]));
                }
                else if (line.Equals("[SCALE]"))
                {
                    this.options.enableScaleFactors = true;
                    line = sr.ReadLine();
                    string[] scaleOpt = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    this.scale.scaleBio = Convert.ToDouble(scaleOpt[0]);
                    this.scale.scaleRec = Convert.ToDouble(scaleOpt[1]);
                    this.scale.scaleStockNum = Convert.ToDouble(scaleOpt[2]);
                }
                else if (line.Equals("[PERC]"))
                {
                    this.options.enablePercentileReport = true;
                    this.reportPercentile.percentile = Convert.ToDouble(sr.ReadLine());
                }
                else if (line.Equals("[PSTAR]"))
                {
                    this.pstar.ReadCalculationDataLines(sr);
                    this.harvestScenario.analysisType = HarvestScenarioAnalysis.PStar;
                }
            }

        }


        public void WriteInputFile(string file)
        {
            try
            {
                List<string> outLines = this.WriteInputFileLines();
                File.WriteAllLines(file, outLines);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private List<string> WriteInputFileLines()
        {
            List<string> inpFile = new List<string>();
            inpFile.Add(this.version); //AGEPRO VERSION 4.0
            
            //CASEID
            inpFile.Add("[CASEID]");
            inpFile.Add(this.caseID);

            //GENERAL
            inpFile.Add("[GENERAL]");
            inpFile.Add(
                this.general.projYearStart.ToString() + "  " +
                this.general.projYearEnd.ToString() + "  " +
                this.general.ageBegin.ToString() + "  " +
                this.general.ageEnd.ToString() + "  " + 
                this.general.numPopSims.ToString() + "  " + 
                this.general.numFleets.ToString() + "  " + 
                this.general.numRecModels.ToString() + "  " +
                Convert.ToInt32(this.general.hasDiscards).ToString() + "  " +
                this.general.seed.ToString());

            //BOOTSTRAP
            inpFile.Add("[BOOTSTRAP]");
            inpFile.Add(this.bootstrap.numBootstraps.ToString() + "  " + this.bootstrap.popScaleFactor.ToString());
            inpFile.Add(this.bootstrap.bootstrapFile);

            //STOCK WEIGHT
            inpFile.Add("[STOCK_WEIGHT]");
            inpFile.Add(Convert.ToInt32(this.jan1Weight.fromFile).ToString() + "  " +
                Convert.ToInt32(this.jan1Weight.timeVarying).ToString());
            if (this.jan1Weight.fromFile == true)
            {
                //Read Data Files
                inpFile.Add(this.jan1Weight.dataFile);
            }
            else
            {
                //WeightsAtAge (per year (row))
                //Can be TimeVarying(Multiple years) or Not
                foreach(DataRow yearRow in this.jan1Weight.byAgeData.Rows)
                {
                    inpFile.Add(string.Join("  ",yearRow.ItemArray)+"  ");
                }
                
                //CV
                foreach (DataRow cvRow in this.jan1Weight.byAgeCV.Rows)
                {
                    inpFile.Add(string.Join("  ", cvRow.ItemArray)+"  ");
                }

            }

            //SSB_WEIGHT
            inpFile.AddRange(SSBWeight.WriteStochasticAgeDataLines("[SSB_WEIGHT]"));

            //MEAN_WEIGHT
            inpFile.AddRange(meanWeight.WriteStochasticAgeDataLines("[MEAN_WEIGHT]"));

            //CATCH_WEIDHT
            inpFile.AddRange(catchWeight.WriteStochasticAgeDataLines("[CATCH_WEIGHT]"));

            if (this.general.hasDiscards)
            {
                //DISC_WEIGHT
                inpFile.AddRange(discardWeight.WriteStochasticAgeDataLines("[DISC_WEIGHT]"));
            }
            
            //NATMORT
            inpFile.AddRange(naturalMortality.WriteStochasticAgeDataLines("[NATMORT]"));

            //BIOLOGICAL (Bioloical:Fraction Mortality Prior to Spawning) 
            inpFile.AddRange(biological.WriteBiologicalDataLines());

            //MATURITY (Biological:Maturity At Age)
            inpFile.AddRange(maturity.WriteStochasticAgeDataLines("[MATURITY]"));

            //FISHERY
            inpFile.AddRange(fishery.WriteStochasticAgeDataLines("[FISHERY]"));

            if (this.general.hasDiscards)
            {
                //DISCARDS
                inpFile.AddRange(discardFraction.WriteStochasticAgeDataLines("[DISCARD]"));
            }

            //RECRUIT (Recruitment)
            inpFile.AddRange(recruitment.WriteRecruitmentDataLines());

            //HARVEST
            if (this.harvestScenario.analysisType == HarvestScenarioAnalysis.HarvestScenario)
            {
                inpFile.AddRange(harvestScenario.WriteHarvestTableDataLines());
            }
            
            //REBUILD
            if (this.harvestScenario.analysisType == HarvestScenarioAnalysis.Rebuilder)
            {
                //inpFile.AddRange(harvestScenario.WriteHarvestTableDataLines());
                inpFile.AddRange(rebuild.WriteCalculationDataLines());
            }
            
            //PSTAR
            if (this.harvestScenario.analysisType == HarvestScenarioAnalysis.PStar)
            {
                //inpFile.AddRange(harvestScenario.WriteHarvestTableDataLines());
                inpFile.AddRange(pstar.WriteCalculationDataLines());
            }
            
            //REFPOINT (Misc Options: Refpoint)
            if (this.options.enableRefpoint)
            {
                inpFile.Add("[REFPOINT]");
                inpFile.Add(
                    this.refpoint.refSpawnBio.ToString() + new string(' ', 2) +
                    this.refpoint.refJan1Bio.ToString() + new string(' ', 2) +
                    this.refpoint.refMeanBio.ToString() + new string(' ', 2) +
                    this.refpoint.refFMort.ToString());
            }

            //BOUNDS (Misc Options: Bounds)
            if (this.options.enableBounds)
            {
                inpFile.Add("[BOUNDS]");
                inpFile.Add(this.bounds.maxWeight + new string(' ',2) + this.bounds.maxNatMort);
            }

            //RETROADJUST (Misc Options: Retro Adjustment Factors)
            if (this.options.enableRetroAdjustmentFactors)
            {
                inpFile.Add("[RETROADJUST]");
                List<string> rafCol = new List<string>();
                foreach (DataRow ageRow in this.retroAdjustOption.retroAdjust.Rows)
                {
                    rafCol.Add(ageRow[0].ToString());
                }
                inpFile.Add(string.Join(new string(' ', 2), rafCol));

            }

            //OPTIONS (Misc Options)
            inpFile.Add("[OPTIONS]");
            inpFile.Add(
                Convert.ToInt32(this.options.enableSummaryReport).ToString() + new string(' ',2) +
                Convert.ToInt32(this.options.enableAuxStochasticFiles).ToString() + new string(' ',2) +
                Convert.ToInt32(this.options.enableExportR).ToString());

            if (this.options.enableScaleFactors)
            {
                inpFile.Add("[SCALE]");
                inpFile.Add(
                    this.scale.scaleBio + new string(' ',2) + 
                    this.scale.scaleRec + new string(' ',2) +
                    this.scale.scaleStockNum + new string(' ',2));
            }
            if (this.options.enablePercentileReport)
            {
                inpFile.Add("[PERC]");
                inpFile.Add(this.reportPercentile.ToString());
            }

            return inpFile;
        }

        /// <summary>
        /// Creates and sets the Recruitment Model Dictionary Object
        /// </summary>
        /// <returns>Returns the Recruitment Model Dictionary</returns>
        private Dictionary<int, string> RecruitDictionary() 
        {
            //Future Feature: Generizse/Automate this Dictionary?
            Dictionary<int, string> recruitModelDictionary = new Dictionary<int, string>();

            recruitModelDictionary.Add(0, "None Selected");
            recruitModelDictionary.Add(1, "Model 1: Markov Matrix");
            recruitModelDictionary.Add(2, "Model 2: Empirical Recruits per Spawning Biomass Distribution");
            recruitModelDictionary.Add(3, "Model 3: Empirical Recruitment Distributiion");
            recruitModelDictionary.Add(4, "Model 4: Two-Stage Empirical Recruits per Spawning Biomass Distribution");
            recruitModelDictionary.Add(5, "Model 5: Beverton-Holt Curve w/ Lognormal Error");
            recruitModelDictionary.Add(6, "Model 6: Ricker Curve w/ Lognormal Error");
            recruitModelDictionary.Add(7, "Model 7: Shepherd Curve w/ Lognormal Error");
            recruitModelDictionary.Add(8, "Model 8: Lognormal Distribution");
            //Model 9 was removed in AGEPRO 4.0
            recruitModelDictionary.Add(10, "Model 10: Beverton-Holt Curve w/ Autocorrected Lognormal Error");
            recruitModelDictionary.Add(11, "Model 11: Ricker Curve w/ Autocorrected Lognormal Error");
            recruitModelDictionary.Add(12, "Model 12: Shepherd Curve w/ Autocorrected Lognormal Error");
            recruitModelDictionary.Add(13, "Model 13: Autocorrected Lognormal Distribution");
            recruitModelDictionary.Add(14, "Model 14: Empirical Cumulative Distribution Function of Recruitment");
            recruitModelDictionary.Add(15, "Model 15: Two-Stage Empirical Cumulative Distribution Function of Recruitment");
            recruitModelDictionary.Add(16, "Model 16: Linear Recruits per Spawning Biomass Predictor w/ Normal Error");
            recruitModelDictionary.Add(17, "Model 17: Loglinear Recruits per Spawning Biomass Predictor w/ Lognormal Error");
            recruitModelDictionary.Add(18, "Model 18: Linear Recruitment Predictor w/ Normal Error");
            recruitModelDictionary.Add(19, "Model 19: Loglinear Recruitment Predictor w/ Lognormal Error");
            recruitModelDictionary.Add(20, "Model 20: Fixed Recruitment");
            recruitModelDictionary.Add(21, "Model 21: Empirical Cumulative Distribution Function of Recruitment w/ Linear Decline to Zero");

            return recruitModelDictionary;
        }
    }
}
