using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{   
    /// <summary>
    /// AGEPRO Input File 
    /// </summary>
    public class AgeproInputFile
  {
        public string version { get; set; } //AGEPRO Reference Manual-Calculation Engine Version
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
        public AgeproBioTSpawn biological = new AgeproBioTSpawn(); //Fraction Mortality in Biological
        public AgeproStochasticAgeTable maturity = new AgeproStochasticAgeTable(); //Maturity in Biological
        public AgeproStochasticAgeTable fishery = new AgeproStochasticAgeTable();
        public AgeproStochasticAgeTable naturalMortality = new AgeproStochasticAgeTable(); 
        public RetroAdjustmentFactors retroAdjustOption = new RetroAdjustmentFactors(); //retroAdjust
        public AgeproHarvestScenario harvestScenario = new AgeproHarvestScenario();
        public AgeproStochasticAgeTable discardFraction = new AgeproStochasticAgeTable(); //discard fraction
        public Bounds bounds = new Bounds(); //bounds
        public AgeproMiscOptions options = new AgeproMiscOptions(); //options
        public ScaleFactors scale = new ScaleFactors(); //scale
        public ReportPercentile reportPercentile = new ReportPercentile(); //reportPercentile
        public Refpoint refpoint = new Refpoint(); //refpoint
        public RebuilderTargetCalculation rebuild = new RebuilderTargetCalculation(); //rebuilder
        public PStarCalculation pstar = new PStarCalculation();

        public AgeproInputFile()
        {
            caseID = "";
        }

        /// <summary>
        /// Initiates the <paramref name="System.IO.StreamReader"/> function to read the AGEPRO Input file.
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
            
            //Version: AGEPRO (Input File) Version
            var supportedINPVer = new[]{ "AGEPRO VERSION 4.0", "AGEPRO VERSION 4.2" };
            var incompatibleINPVer = new[]{ "AGEPRO VERSION 3.2", "AGEPRO VERSION 3.3" };
            if (supportedINPVer.Contains(line))
            {
                this.version = line;
            }
            else if (incompatibleINPVer.Contains(line)) 
            {
                //Throw Error/Warning for incompatiability
                throw new InvalidAgeproParameterException("This file format version is incompatible.");
            }
            else
            {           
                throw new InvalidAgeproParameterException("Invaild AGEPRO input file.");
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
                    this.general.ProjYearStart = Convert.ToInt32(generalLine[0]);
                    this.general.ProjYearEnd = Convert.ToInt32(generalLine[1]);
                    this.general.AgeBegin = Convert.ToInt32(generalLine[2]);
                    this.general.AgeEnd = Convert.ToInt32(generalLine[3]);
                    this.general.NumPopSims = Convert.ToInt32(generalLine[4]);
                    this.general.NumFleets = Convert.ToInt32(generalLine[5]);
                    this.general.NumRecModels = Convert.ToInt32(generalLine[6]);
                    this.general.Seed = Convert.ToInt32(generalLine[8]);
                    if (generalLine[7].Equals("1"))
                    {
                        this.general.HasDiscards = true;
                    }
                    else
                    {
                        this.general.HasDiscards = false;
                    }

                }
                else if (line.Equals("[RECRUIT]"))
                {
                    //Read Recruit Data
                    this.recruitment.ObservationYears = this.general.SeqYears();
                    this.recruitment.ReadRecruitmentData(sr,general.NumYears(),general.NumRecModels);
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
                    this.catchWeight.ReadStochasticAgeData(sr, this.general.NumYears(), this.general.NumAges(), this.general.NumFleets);
                }
                else if (line.Equals("[DISC_WEIGHT]"))
                {
                    this.discardWeight.ReadStochasticAgeData(sr, this.general.NumYears(), this.general.NumAges(), this.general.NumFleets);
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
                    this.fishery.ReadStochasticAgeData(sr, this.general.NumYears(), this.general.NumAges(), this.general.NumFleets);
                }
                else if (line.Equals("[DISCARD]"))
                {
                    this.discardFraction.ReadStochasticAgeData(sr, this.general.NumYears(), this.general.NumAges(), this.general.NumFleets);
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
                    this.harvestScenario.ReadHarvestTable(sr, this.general.NumYears(), this.general.NumFleets);
                }
                else if (line.Equals("[REBUILD]"))
                {
                    this.rebuild.obsYears = this.general.SeqYears();
                    this.rebuild.ReadCalculationDataLines(sr);
                    this.harvestScenario.AnalysisType = HarvestScenarioAnalysis.Rebuilder;
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
                    this.pstar.obsYears = this.general.SeqYears();
                    this.pstar.ReadCalculationDataLines(sr);
                    this.harvestScenario.AnalysisType = HarvestScenarioAnalysis.PStar;
                }
            }

        }

        /// <summary>
        /// Initiates the \code{WriteInputFileLines} function to write AGEPRO Input files.
        /// </summary>
        /// <param name="file">AGEPRO Input Filename Location</param>
        public void WriteInputFile(string file)
        {
            try
            {
                List<string> outLines = this.WriteInputFileLines();
                File.WriteAllLines(file, outLines);
            }
            catch(NullReferenceException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Writes AGERPRO Input file values by line
        /// </summary>
        /// <returns>List of strings. Each string repesents a line from the input file.</returns>
        private List<string> WriteInputFileLines()
        {
            List<string> inpFile = new List<string>();
            inpFile.Add(this.version); //New cases will have "AGEPRO VERSION 4.2"
            
            //CASEID
            inpFile.Add("[CASEID]");
            inpFile.Add(this.caseID);

            //GENERAL
            inpFile.Add("[GENERAL]");
            inpFile.Add(
                this.general.ProjYearStart.ToString() + "  " +
                this.general.ProjYearEnd.ToString() + "  " +
                this.general.AgeBegin.ToString() + "  " +
                this.general.AgeEnd.ToString() + "  " + 
                this.general.NumPopSims.ToString() + "  " + 
                this.general.NumFleets.ToString() + "  " + 
                this.general.NumRecModels.ToString() + "  " +
                Convert.ToInt32(this.general.HasDiscards).ToString() + "  " +
                this.general.Seed.ToString());

            //BOOTSTRAP
            inpFile.Add("[BOOTSTRAP]");
            inpFile.Add(this.bootstrap.NumBootstraps.ToString() + "  " + this.bootstrap.PopScaleFactor.ToString());
            inpFile.Add(this.bootstrap.BootstrapFile);

            //STOCK WEIGHT
            inpFile.AddRange(jan1Weight.WriteStochasticAgeDataLines("[STOCK_WEIGHT]"));

            //SSB_WEIGHT
            inpFile.AddRange(SSBWeight.WriteStochasticAgeDataLines("[SSB_WEIGHT]"));

            //MEAN_WEIGHT
            inpFile.AddRange(meanWeight.WriteStochasticAgeDataLines("[MEAN_WEIGHT]"));

            //CATCH_WEIGHT
            inpFile.AddRange(catchWeight.WriteStochasticAgeDataLines("[CATCH_WEIGHT]"));

            if (this.general.HasDiscards)
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

            if (this.general.HasDiscards)
            {
                //DISCARDS
                inpFile.AddRange(discardFraction.WriteStochasticAgeDataLines("[DISCARD]"));
            }

            //RECRUIT (Recruitment)
            inpFile.AddRange(recruitment.WriteRecruitmentDataLines());

            //HARVEST
            inpFile.AddRange(harvestScenario.WriteHarvestTableDataLines());
            
            //REBUILD
            if (this.harvestScenario.AnalysisType == HarvestScenarioAnalysis.Rebuilder)
            {
                //inpFile.AddRange(harvestScenario.WriteHarvestTableDataLines());
                inpFile.AddRange(rebuild.WriteCalculationDataLines());
            }
            
            //PSTAR
            if (this.harvestScenario.AnalysisType == HarvestScenarioAnalysis.PStar)
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
                inpFile.Add(this.reportPercentile.percentile.ToString());
            }

            return inpFile;
        }
  }
}
