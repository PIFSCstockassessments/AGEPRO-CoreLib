using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// AGEPRO Input File 
  /// </summary>
  public class AgeproInputFile
  {
    public string Version { get; set; } //AGEPRO Reference Manual-Calculation Engine Version
    public string GUI_Version { get; set; }
    public string CaseID { get; set; }

    // Version Constants
    private const string AGEPRO40Version = "AGEPRO VERSION 4.0"; //"AGEPRO VERSION 4.0" Version String 
    public const string CurrentVersion = "AGEPRO VERSION 4.25";
    public const string GUIVersion = "4.25-4.3.4";
    public static readonly string[] INPSupportedVersions = { AgeproInputFile.AGEPRO40Version, AgeproInputFile.CurrentVersion };
    
    public AgeproGeneral General = new AgeproGeneral();
    public AgeproBootstrap Bootstrap = new AgeproBootstrap();
    public AgeproRecruitment Recruitment = new AgeproRecruitment();
    public AgeproWeightAgeTable Jan1StockWeight = new AgeproWeightAgeTable(new int[] { 1, 0 }); //STOCK_WEIGHT
    public AgeproWeightAgeTable SSBWeight = new AgeproWeightAgeTable(new int[] { 1, 0, -1 });
    public AgeproWeightAgeTable MeanWeight = new AgeproWeightAgeTable(new int[] { 1, 0, -1, -2 });
    public AgeproWeightAgeTable CatchWeight = new AgeproWeightAgeTable(new int[] { 1, 0, -1, -2, -3 });
    public AgeproWeightAgeTable DiscardWeight = new AgeproWeightAgeTable(new int[] { 1, 0, -1, -2, -3, -4 }); //discard weight
    public AgeproBioTSpawn BiologicalTSpawn = new AgeproBioTSpawn(); //Fraction Mortality in Biological
    public AgeproStochasticAgeTable BiologicalMaturity = new AgeproStochasticAgeTable(); //Maturity in Biological
    public AgeproStochasticAgeTable Fishery = new AgeproStochasticAgeTable();
    public AgeproStochasticAgeTable NaturalMortality = new AgeproStochasticAgeTable();
    public RetroAdjustmentFactors RetroAdjustments = new RetroAdjustmentFactors(); //retroAdjust
    public AgeproHarvestScenario HarvestScenario = new AgeproHarvestScenario();
    public AgeproStochasticAgeTable DiscardFraction = new AgeproStochasticAgeTable(); //discard fraction
    public Bounds Bounds = new Bounds(); //bounds
    public AgeproMiscOptions Options = new AgeproMiscOptions(); //options
    public ScaleFactors Scale = new ScaleFactors(); //scale
    public ReportPercentile ReportPercentile = new ReportPercentile(); //reportPercentile
    public Refpoint Refpoint = new Refpoint(); //refpoint
    public RebuilderTargetCalculation Rebuild = new RebuilderTargetCalculation(); //rebuilder
    public PStarCalculation PStar = new PStarCalculation();

    public AgeproInputFile()
    {
      this.CaseID = "";
      this.Version = AgeproInputFile.CurrentVersion;
      this.GUI_Version = AgeproInputFile.GUIVersion;

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
      _ = CheckINPVersion(sr);

      while (!sr.EndOfStream)
      {
        line = sr.ReadLine();
        //Case ID
        if (line.Equals("[CASEID]"))
        {
          CaseID = sr.ReadLine();
        }
        //General
        else if (line.Equals("[GENERAL]"))
        {
          _ = General.ReadGeneralModelParameters(sr);

        }
        else if (line.Equals("[RECRUIT]"))
        {
          //Read Recruit Data
          Recruitment.ObservationYears = General.SeqYears();
          Recruitment.ReadRecruitmentData(sr, General.NumYears(), General.NumRecModels);
        }
        else if (line.Equals("[STOCK_WEIGHT]"))
        {
          Jan1StockWeight.ReadStochasticAgeData(sr, General.NumYears(), General.NumAges());
        }
        else if (line.Equals("[SSB_WEIGHT]"))
        {
          SSBWeight.ReadStochasticAgeData(sr, General.NumYears(), General.NumAges());
        }
        else if (line.Equals("[MEAN_WEIGHT]"))
        {
          MeanWeight.ReadStochasticAgeData(sr, General.NumYears(), General.NumAges());
        }
        else if (line.Equals("[CATCH_WEIGHT]"))
        {
          CatchWeight.ReadStochasticAgeData(sr, General.NumYears(), General.NumAges(), General.NumFleets);
        }
        else if (line.Equals("[DISC_WEIGHT]"))
        {
          DiscardWeight.ReadStochasticAgeData(sr, General.NumYears(), General.NumAges(), General.NumFleets);
        }
        else if (line.Equals("[NATMORT]"))
        {
          NaturalMortality.ReadStochasticAgeData(sr, General.NumYears(), General.NumAges());
        }
        else if (line.Equals("[MATURITY]"))
        {
          BiologicalMaturity.ReadStochasticAgeData(sr, General.NumYears(), General.NumAges());
        }
        else if (line.Equals("[FISHERY]"))
        {
          Fishery.ReadStochasticAgeData(sr, General.NumYears(), General.NumAges(), General.NumFleets);
        }
        else if (line.Equals("[DISCARD]"))
        {
          DiscardFraction.ReadStochasticAgeData(sr, General.NumYears(), General.NumAges(), General.NumFleets);
        }
        else if (line.Equals("[BIOLOGICAL]"))
        {
          BiologicalTSpawn.ReadBiologicalData(sr, General.SeqYears());
        }
        else if (line.Equals("[BOOTSTRAP]"))
        {
          Bootstrap.ReadBootstrapData(sr);
        }
        else if (line.Equals("[HARVEST]"))
        {
          HarvestScenario.ReadHarvestTable(sr, General.NumYears(), General.NumFleets);
        }
        else if (line.Equals("[REBUILD]"))
        {
          Rebuild.ObsYears = General.SeqYears();
          Rebuild.ReadCalculationDataLines(sr);
          HarvestScenario.AnalysisType = HarvestScenarioAnalysis.Rebuilder;
        }
        else if (line.Equals("[REFPOINT]"))
        {
          Options.EnableRefpoint = true;
          _ = Refpoint.ReadRefpointLines(sr);
        }
        else if (line.Equals("[BOUNDS]"))
        {
          Options.EnableBounds = true;
          _ = Bounds.ReadBounds(sr);
        }
        else if (line.Equals("[RETROADJUST]"))
        {
          Options.EnableRetroAdjustmentFactors = true;
          _ = RetroAdjustments.ReadRetroAdjustmentFactorsTable(sr, General);
        }
        else if (line.Equals("[OPTIONS]"))
        {
          ReadOutputOptions(sr);

        }
        else if (line.Equals("[SCALE]"))
        {
          Options.EnableScaleFactors = true;
          _ = Scale.ReadScaleFactors(sr);
        }
        else if (line.Equals("[PERC]"))
        {
          Options.EnablePercentileReport = true;
          ReportPercentile.Percentile = Convert.ToDouble(sr.ReadLine());
        }
        else if (line.Equals("[PSTAR]"))
        {
          PStar.ObsYears = General.SeqYears();
          PStar.ReadCalculationDataLines(sr);
          HarvestScenario.AnalysisType = HarvestScenarioAnalysis.PStar;
        }
      }
    }

    /// <summary>
    /// Helper function to read OPTIONS keyword parameter for the AGEPRO Input File, 
    /// depending on version. AGEPRO input file formated to version 4.0 will read stock 
    /// summary flags as a boolean. Stock summary flags for the AGEPRO input file 4.25 
    /// format will be read as an integer.
    /// </summary>
    /// <param name="sr">Streamreader object to the file connection</param>
    private void ReadOutputOptions(StreamReader sr)
    {
      if (this.Version == AgeproInputFile.AGEPRO40Version)
      {
        _ = Options.ReadAgepro40Options(sr);
      }
      else
      {
        _ = Options.ReadAgeproOutputOptions(sr);
      }
    }

    /// <summary>
    /// Checks Printed AGEPRO Input File Version
    /// </summary>
    /// <param name="sr">Streamreader object to the file connection</param>
    /// <returns></returns>
    private string CheckINPVersion(StreamReader sr)
    {
      string line = sr.ReadLine();

      //Version: AGEPRO (Input File) Version
      var supportedINPVer = new[] { AgeproInputFile.AGEPRO40Version, AgeproInputFile.CurrentVersion };
      var incompatibleINPVer = new[] { "AGEPRO VERSION 3.2", "AGEPRO VERSION 3.3" };
      if (supportedINPVer.Contains(line))
      {
        this.Version = line;
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

      return line;
    }


    /// <summary>
    /// Initiates the \code{WriteInputFileLines} function to write AGEPRO Input files.
    /// </summary>
    /// <param name="file">AGEPRO Input Filename Location</param>
    public void WriteInputFile(string file)
    {
      try
      {
        List<string> outLines = WriteInputFileLines();
        File.WriteAllLines(file, outLines);
      }
      catch (NullReferenceException)
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

      //VERSION
      inpFile.Add(this.Version); //New cases will have "AGEPRO VERSION 4.2"

      //CASEID
      inpFile.Add("[CASEID]");
      inpFile.Add(CaseID);

      //GENERAL
      inpFile.AddRange(General.WriteAgeproGeneralParameters());

      //BOOTSTRAP
      inpFile.Add("[BOOTSTRAP]");
      inpFile.Add(Bootstrap.NumBootstraps.ToString() + "  " + Bootstrap.PopScaleFactor.ToString());
      inpFile.Add(Bootstrap.BootstrapFile);

      //STOCK WEIGHT
      inpFile.AddRange(Jan1StockWeight.WriteStochasticAgeDataLines("[STOCK_WEIGHT]"));

      //SSB_WEIGHT
      inpFile.AddRange(SSBWeight.WriteStochasticAgeDataLines("[SSB_WEIGHT]"));

      //MEAN_WEIGHT
      inpFile.AddRange(MeanWeight.WriteStochasticAgeDataLines("[MEAN_WEIGHT]"));

      //CATCH_WEIGHT
      inpFile.AddRange(CatchWeight.WriteStochasticAgeDataLines("[CATCH_WEIGHT]"));

      if (General.HasDiscards)
      {
        //DISC_WEIGHT
        inpFile.AddRange(DiscardWeight.WriteStochasticAgeDataLines("[DISC_WEIGHT]"));
      }

      //NATMORT
      inpFile.AddRange(NaturalMortality.WriteStochasticAgeDataLines("[NATMORT]"));

      //BIOLOGICAL (Bioloical:Fraction Mortality Prior to Spawning) 
      inpFile.AddRange(BiologicalTSpawn.WriteBiologicalDataLines());

      //MATURITY (Biological:Maturity At Age)
      inpFile.AddRange(BiologicalMaturity.WriteStochasticAgeDataLines("[MATURITY]"));

      //FISHERY
      inpFile.AddRange(Fishery.WriteStochasticAgeDataLines("[FISHERY]"));

      if (General.HasDiscards)
      {
        //DISCARDS
        inpFile.AddRange(DiscardFraction.WriteStochasticAgeDataLines("[DISCARD]"));
      }

      //RECRUIT (Recruitment)
      inpFile.AddRange(Recruitment.WriteRecruitmentDataLines());

      //HARVEST
      inpFile.AddRange(HarvestScenario.WriteHarvestTableDataLines());

      //REBUILD
      if (HarvestScenario.AnalysisType == HarvestScenarioAnalysis.Rebuilder)
      {
        //inpFile.AddRange(harvestScenario.WriteHarvestTableDataLines());
        inpFile.AddRange(Rebuild.WriteCalculationDataLines());
      }

      //PSTAR
      if (HarvestScenario.AnalysisType == HarvestScenarioAnalysis.PStar)
      {
        //inpFile.AddRange(harvestScenario.WriteHarvestTableDataLines());
        inpFile.AddRange(PStar.WriteCalculationDataLines());
      }

      //REFPOINT (Misc Options: Refpoint)
      if (Options.EnableRefpoint)
      {
        inpFile.AddRange(Refpoint.WriteRefpointLines());
      }

      //BOUNDS (Misc Options: Bounds)
      if (Options.EnableBounds)
      {
        inpFile.AddRange(Bounds.WriteBoundsLines());
      }

      //RETROADJUST (Misc Options: Retro Adjustment Factors)
      if (Options.EnableRetroAdjustmentFactors)
      {
        inpFile.AddRange(RetroAdjustments.WriteRetroAdjustmentFactorsTable());
      }

      //OPTIONS (Misc Options)
      if (this.Version == AgeproInputFile.AGEPRO40Version)
      {
        inpFile.AddRange(Options.WriteAgepro40Options());
      }
      else
      {
        inpFile.AddRange(Options.WriteAgeproOutputOptions());
      }


      //SCALE FACTORS
      if (Options.EnableScaleFactors)
      {
        inpFile.AddRange(Scale.WriteScaleFactors());
      }

      //REPORT PERCENTILE
      if (Options.EnablePercentileReport)
      {
        inpFile.Add("[PERC]");
        inpFile.Add(ReportPercentile.Percentile.ToString());
      }

      return inpFile;
    }

  }
}
