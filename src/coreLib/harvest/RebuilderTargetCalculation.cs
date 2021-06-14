using System;
using System.Collections.Generic;
using System.Linq;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// Specifications for Stock Rebuilder Targets
  /// </summary>
  public class RebuilderTargetCalculation : AgeproHarvestScenario
  {
    //private int _targetYear;
    private double _TargetValue;
    private int _TargetType;        //rebuild target type (cboRebuild.SelectedIndex)
    private double _TargetPercent;  //Percent Confidence (Rebulider Confidence Level)

    public double TargetValue
    {
      get => _TargetValue;
      set => SetProperty(ref _TargetValue, value);
    }
    public int TargetType
    {
      get => _TargetType;
      set => SetProperty(ref _TargetType, value);
    }
    public double TargetPercent
    {
      get => _TargetPercent;
      set => SetProperty(ref _TargetPercent, value);
    }
    public RebuilderTargetCalculation()
    {
      //Default Rebuilder Target Values
      calculationType = HarvestScenarioAnalysis.Rebuilder;
      TargetYear = 0;
      TargetValue = 0;
      TargetType = 0;
      TargetPercent = 0;

      //Default Obs years
      ObsYears = new int[] { 1 };
    }

    /// <summary>
    /// Readin AGEPRO Input Data File for Rebuild Specification Parameters
    /// </summary>
    /// <param name="sr">AGEPRO Input Data File StreamReader</param>
    public override void ReadCalculationDataLines(System.IO.StreamReader sr)
    {
      if (sr is null)
      {
        throw new ArgumentNullException(nameof(sr));
      }
      string line = sr.ReadLine();
      string[] rebuildOptionLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

      TargetYear = Convert.ToInt32(rebuildOptionLine[0]);
      TargetValue = Convert.ToDouble(rebuildOptionLine[1]);
      TargetType = Convert.ToInt32(rebuildOptionLine[2]);
      TargetPercent = Convert.ToDouble(rebuildOptionLine[3]);

    }

    /// <summary>
    /// Generates AGEPRO Input Data file lines related to the [REBUILD] parameter
    /// </summary>
    /// <returns>A list of strings to be appended to the AGEPRO Input Data file.</returns>
    public override List<string> WriteCalculationDataLines()
    {
      List<string> outputLines = new List<string>
      {
        "[REBUILD]",
        TargetYear
        + new string(' ', 2)
        + TargetValue
        + new string(' ', 2)
        + TargetType
        + new string(' ', 2)
        + TargetPercent
      };

      return outputLines;
    }

    public override ValidationResult ValidateInput()
    {
      List<string> errorMsgList = new List<string>();
      int yrStart = ObsYears[0];
      int yrEnd = ObsYears[(ObsYears.Count() - 1)];

      //Rebulider Year
      if (TargetYear < yrStart || TargetYear > yrEnd)
      {
        errorMsgList.Add("Invalid Rebuilder Year Specification.");
      }
      //Rebuilder Target
      if (string.IsNullOrWhiteSpace(TargetValue.ToString()))
      {
        errorMsgList.Add("Invalid or missing rebuilder target value.");
      }
      //Rebuilder Confidence Level
      if (string.IsNullOrWhiteSpace(TargetPercent.ToString()))
      {
        errorMsgList.Add("Invalid or missing rebuilder confidence level.");
      }

      ValidationResult results = errorMsgList.EnumerateValidationResults();
      return results;
    }
  }
}
