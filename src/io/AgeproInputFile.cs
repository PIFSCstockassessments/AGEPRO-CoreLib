using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// AGEPRO Input File 
  /// </summary>
  public class AgeproInputFile
  {
    public string Version { get; set; } //AGEPRO Reference Manual-Calculation Engine Version
    public double NumVer { get; set; }
    public string CaseID { get; set; }

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
      CaseID = "";
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
      var supportedINPVer = new[] { "AGEPRO VERSION 4.0", "AGEPRO VERSION 4.2" };
      var incompatibleINPVer = new[] { "AGEPRO VERSION 3.2", "AGEPRO VERSION 3.3" };
      if (supportedINPVer.Contains(line))
      {
        Version = line;
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
          line = sr.ReadLine();
          string[] generalLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
          General.ProjYearStart = Convert.ToInt32(generalLine[0]);
          General.ProjYearEnd = Convert.ToInt32(generalLine[1]);
          General.AgeBegin = Convert.ToInt32(generalLine[2]);
          General.AgeEnd = Convert.ToInt32(generalLine[3]);
          General.NumPopSims = Convert.ToInt32(generalLine[4]);
          General.NumFleets = Convert.ToInt32(generalLine[5]);
          General.NumRecModels = Convert.ToInt32(generalLine[6]);
          General.Seed = Convert.ToInt32(generalLine[8]);
          if (generalLine[7].Equals("1"))
          {
            General.HasDiscards = true;
          }
          else
          {
            General.HasDiscards = false;
          }

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
          Rebuild.obsYears = General.SeqYears();
          Rebuild.ReadCalculationDataLines(sr);
          HarvestScenario.AnalysisType = HarvestScenarioAnalysis.Rebuilder;
        }
        else if (line.Equals("[REFPOINT]"))
        {
          Options.EnableRefpoint = true;
          line = sr.ReadLine();
          string[] refpointOpt = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
          Refpoint.RefSpawnBio = Convert.ToDouble(refpointOpt[0]);
          Refpoint.RefJan1Bio = Convert.ToDouble(refpointOpt[1]);
          Refpoint.RefMeanBio = Convert.ToDouble(refpointOpt[2]);
          Refpoint.RefFMort = Convert.ToDouble(refpointOpt[3]);
        }
        else if (line.Equals("[BOUNDS]"))
        {
          Options.EnableBounds = true;
          line = sr.ReadLine();
          string[] boundsOpt = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
          Bounds.MaxWeight = Convert.ToDouble(boundsOpt[0]);
          Bounds.MaxNatMort = Convert.ToDouble(boundsOpt[1]);
        }
        else if (line.Equals("[RETROADJUST]"))
        {
          Options.EnableRetroAdjustmentFactors = true;
          line = sr.ReadLine();
          string[] rafLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
          DataTable rafTable = new DataTable("Retro Adjustment Factors");
          rafTable.Columns.Add(); //set column without name
                                  //TODO: throw warning/error if 'rafLine' length doesn't match number of Ages

          for (int i = 0; i < General.NumAges(); i++)
          {
            rafTable.Rows.Add(rafLine[i]);
          }

          RetroAdjustments.retroAdjust = rafTable;
        }
        else if (line.Equals("[OPTIONS]"))
        {
          line = sr.ReadLine();
          string[] optionOpt = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
          Options.EnableSummaryReport = Convert.ToBoolean(Convert.ToInt32(optionOpt[0]));
          Options.EnableAuxStochasticFiles = Convert.ToBoolean(Convert.ToInt32(optionOpt[1]));
          Options.EnableExportR = Convert.ToBoolean(Convert.ToInt32(optionOpt[2]));
        }
        else if (line.Equals("[SCALE]"))
        {
          Options.EnableScaleFactors = true;
          line = sr.ReadLine();
          string[] scaleOpt = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
          Scale.ScaleBio = Convert.ToDouble(scaleOpt[0]);
          Scale.ScaleRec = Convert.ToDouble(scaleOpt[1]);
          Scale.ScaleStockNum = Convert.ToDouble(scaleOpt[2]);
        }
        else if (line.Equals("[PERC]"))
        {
          Options.EnablePercentileReport = true;
          ReportPercentile.Percentile = Convert.ToDouble(sr.ReadLine());
        }
        else if (line.Equals("[PSTAR]"))
        {
          PStar.obsYears = General.SeqYears();
          PStar.ReadCalculationDataLines(sr);
          HarvestScenario.AnalysisType = HarvestScenarioAnalysis.PStar;
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
      inpFile.Add(Version); //New cases will have "AGEPRO VERSION 4.2"

      //CASEID
      inpFile.Add("[CASEID]");
      inpFile.Add(CaseID);

      //GENERAL
      inpFile.Add("[GENERAL]");
      inpFile.Add(
          General.ProjYearStart.ToString() + "  " +
          General.ProjYearEnd.ToString() + "  " +
          General.AgeBegin.ToString() + "  " +
          General.AgeEnd.ToString() + "  " +
          General.NumPopSims.ToString() + "  " +
          General.NumFleets.ToString() + "  " +
          General.NumRecModels.ToString() + "  " +
          Convert.ToInt32(General.HasDiscards).ToString() + "  " +
          General.Seed.ToString());

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
        inpFile.Add("[REFPOINT]");
        inpFile.Add(
            Refpoint.RefSpawnBio.ToString() + new string(' ', 2) +
            Refpoint.RefJan1Bio.ToString() + new string(' ', 2) +
            Refpoint.RefMeanBio.ToString() + new string(' ', 2) +
            Refpoint.RefFMort.ToString());
      }

      //BOUNDS (Misc Options: Bounds)
      if (Options.EnableBounds)
      {
        inpFile.Add("[BOUNDS]");
        inpFile.Add(Bounds.MaxWeight + new string(' ', 2) + Bounds.MaxNatMort);
      }

      //RETROADJUST (Misc Options: Retro Adjustment Factors)
      if (Options.EnableRetroAdjustmentFactors)
      {
        inpFile.Add("[RETROADJUST]");
        List<string> rafCol = new List<string>();
        foreach (DataRow ageRow in RetroAdjustments.retroAdjust.Rows)
        {
          rafCol.Add(ageRow[0].ToString());
        }
        inpFile.Add(string.Join(new string(' ', 2), rafCol));

      }

      //OPTIONS (Misc Options)
      inpFile.Add("[OPTIONS]");
      inpFile.Add(
          Convert.ToInt32(Options.EnableSummaryReport).ToString() + new string(' ', 2) +
          Convert.ToInt32(Options.EnableAuxStochasticFiles).ToString() + new string(' ', 2) +
          Convert.ToInt32(Options.EnableExportR).ToString());

      if (Options.EnableScaleFactors)
      {
        inpFile.Add("[SCALE]");
        inpFile.Add(
            Scale.ScaleBio + new string(' ', 2) +
            Scale.ScaleRec + new string(' ', 2) +
            Scale.ScaleStockNum + new string(' ', 2));
      }
      if (Options.EnablePercentileReport)
      {
        inpFile.Add("[PERC]");
        inpFile.Add(ReportPercentile.Percentile.ToString());
      }

      return inpFile;
    }
  }
}
