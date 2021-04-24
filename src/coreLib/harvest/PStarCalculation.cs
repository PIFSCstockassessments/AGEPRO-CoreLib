using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Nmfs.Agepro.CoreLib
{
  public class PStarCalculation : AgeproHarvestScenario
  {

    private double _PStarF;
    private DataTable _PStarTable;
    private int _PStarLevels;

    public double PStarF
    {
      get => _PStarF;
      set => SetProperty(ref _PStarF, value);
    }
    public DataTable PStarTable
    {
      get => _PStarTable;
      set => SetProperty(ref _PStarTable, value);
    }
    public int PStarLevels
    {
      get => _PStarLevels;
      set => SetProperty(ref _PStarLevels, value);
    }

    public PStarCalculation()
    {
      calculationType = HarvestScenarioAnalysis.PStar;
      PStarLevels = 1;
      PStarF = 0;
      TargetYear = 0;
      
      //Create PStar Table
      PStarTable = CreateNewPStarTable();
      PStarTable.Rows.Add();
      Extensions.FillDBNullCellsWithZero(PStarTable);
    }

    /// <summary>
    /// Creates a new instance of a PStar Levels DataTable
    /// </summary>
    /// <param name="levels">Number of columns to create. Defaults to 1.</param>
    /// <returns>A row-less PStar DataTable Instance.</returns>
    public DataTable CreateNewPStarTable(int levels = 1)
    {
      DataTable newPStarTable = new DataTable("pStar");

      for (int i = 0; i < levels; i++)
      {
        newPStarTable.Columns.Add("Level " + (i + 1).ToString(), typeof(double));
      }

      return newPStarTable;
    }


    /// <summary>
    /// Reads in AGEPRO Input Data File for P-Star Data Specifications
    /// </summary>
    /// <param name="sr">AGEPRO Input Data File StreamReader</param>
    public override void ReadCalculationDataLines(System.IO.StreamReader sr)
    {
      string line;

      //Number of pStar Levels
      line = sr.ReadLine();
      PStarLevels = Convert.ToInt32(line);

      //pStar Level Values
      line = sr.ReadLine();
      string[] pStarLevelData = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      PStarTable = CreateNewPStarTable(PStarLevels);
      PStarTable.Rows.Add(pStarLevelData);

      //Overfishing F
      line = sr.ReadLine();
      PStarF = Convert.ToDouble(line);

      //Target Year
      line = sr.ReadLine();
      TargetYear = Convert.ToInt32(line);

    }


    /// <summary>
    /// Generates AGEPRO Input Data file lines related to the [PSTAR] parameter
    /// </summary>
    /// <returns>A list of strings to be appended to the AGEPRO Input Data file.</returns>
    public override List<string> WriteCalculationDataLines()
    {
      List<string> outputLines = new List<string>
      {
        "[PSTAR]",
        PStarLevels.ToString()
      };

      if (PStarTable.Rows.Count > 1)
      {
        throw new ApplicationException("P-Star Data Table Has more than one row");
      }
      foreach (DataRow dtRow in PStarTable.Rows)
      {
        outputLines.Add(string.Join(new string(' ', 2), dtRow.ItemArray));
      }
      outputLines.Add(PStarF.ToString());
      outputLines.Add(TargetYear.ToString());
      return outputLines;
    }


    public override ValidationResult ValidateInput()
    {
      List<string> errorMsgList = new List<string>();
      int yrStart = ObsYears[0];
      int yrEnd = ObsYears[ObsYears.Count() - 1];
      //Target Year
      if (TargetYear < yrStart || TargetYear > yrEnd)
      {
        errorMsgList.Add("Invalid P-Star Year Specification.");
      }
      //Pstar Levels
      if (string.IsNullOrWhiteSpace(PStarLevels.ToString()))
      {
        errorMsgList.Add("Invalid or missing number of P-Star Levels.");
      }
      //F
      if (string.IsNullOrWhiteSpace(PStarF.ToString()))
      {
        errorMsgList.Add("Invalid or missing overfishing value.");
      }

      double xPstar = 0.0;
      foreach (DataRow pstarLvl in PStarTable.Rows)
      {
        foreach (var item in pstarLvl.ItemArray)
        {
          double currentPstar = Convert.ToDouble(item);
          if (xPstar > currentPstar)
          {
            errorMsgList.Add("P-Star Levels must be in ascending order.");
            break;
          }
          else
          {
            xPstar = currentPstar;
          }
        }
      }

      ValidationResult results = errorMsgList.EnumerateValidationResults();
      return results;
    }
  }
}
