using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// Two-Stage Empirical Recruitment. Parameters & Observations for two levels (stages).
  /// </summary>
  public class EmpiricalTwoStageRecruitment : EmpiricalRecruitment
  {
    private int _Lv1NumObs;
    private int _Lv2NumObs;
    private int _SSBBreakVal;

    public DataTable lv1Obs { get; set; }
    public DataTable lv2Obs { get; set; }

    public int Lv1NumObs
    {
      get => _Lv1NumObs;
      set => SetProperty(ref _Lv1NumObs, value);
    }
    public int Lv2NumObs
    {
      get => _Lv2NumObs;
      set => SetProperty(ref _Lv2NumObs, value);
    }
    public int SSBBreakVal
    {
      get => _SSBBreakVal;
      set => SetProperty(ref _SSBBreakVal, value);
    }

    public EmpiricalTwoStageRecruitment(int modelNum)
        : base(modelNum)
    {
      recruitModelNum = modelNum;
      recruitCategory = 1;
      WithSSB = true;
      SubType = EmpiricalType.TwoStage;
      LowBound = 0.0001;

      //Fallback Defaults
      Lv1NumObs = 0;
      Lv2NumObs = 0;
      SSBBreakVal = 0;

    }

    public EmpiricalTwoStageRecruitment(int modelNum, bool useSSB)
        : this(modelNum)
    {
      WithSSB = useSSB;
    }

    /// <summary>
    /// Reads in AGEPRO Input File Stream For Two-Stage Empirical Recruitment Specfic Parameters & Data
    /// </summary>
    /// <param name="sr">AGEPRO Input File StreamReader</param>
    public override void ReadRecruitmentModel(StreamReader sr)
    {
      string line;

      //lv1NumObs, lv2NumObs
      line = sr.ReadLine();
      string[] lineNumObsLvl = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      Lv1NumObs = Convert.ToInt32(lineNumObsLvl[0]);
      Lv2NumObs = Convert.ToInt32(lineNumObsLvl[1]);

      //lv1Obs 
      lv1Obs = ReadObsTable(sr, Lv1NumObs);
      //lv2Obs
      lv2Obs = ReadObsTable(sr, Lv2NumObs);

      //SSBBReakVal
      line = sr.ReadLine();
      SSBBreakVal = Convert.ToInt32(line);
    }

    /// <summary>
    /// Translates Two-Stage Empirical Recruitment input data and parameters into the
    /// AGEPRO input file data format.
    /// </summary>
    /// <returns>List of strings. Each string repesents a line from the input file.</returns>
    public override List<string> WriteRecruitmentDataModelData()
    {
      List<string> outputLines = new List<string>
      {
        Lv1NumObs + new string(' ', 2) + Lv2NumObs
      };

      outputLines.AddRange(WriteObsTableLines(lv1Obs, WithSSB));
      outputLines.AddRange(WriteObsTableLines(lv2Obs, WithSSB));

      outputLines.Add(SSBBreakVal.ToString());

      return outputLines;
    }

    /// <summary>
    /// Single stage Observation Value DataTable validation check.
    /// </summary>
    /// <param name="twoStageObsTable">Observation DataTable from single stage</param>
    /// <param name="tableName">Observation Table Name</param>
    /// <returns>
    /// If DataTable passes all validation checks, nothing will be returned.
    /// All validations not met will be recorded to a list of "Error Messages" to return.
    /// </returns>
    private List<string> CheckTwoStageObsTable(DataTable twoStageObsTable, string tableName)
    {
      List<string> errorMsgList = new List<string>();

      if (twoStageObsTable.Rows.Count <= 0)
      {
        errorMsgList.Add(tableName + " table has 0 rows");
      }

      if (HasBlankOrNullCells(twoStageObsTable))
      {
        errorMsgList.Add("Missing Data in " + tableName + " table");
      }
      else
      {
        if (TableHasAllSignificantValues(twoStageObsTable, LowBound) == false)
        {
          errorMsgList.Add("Insignificant values or values lower than "
              + LowBound + " found in " + tableName + " table");
        }
      }

      return errorMsgList;
    }

    /// <summary>
    /// Checks the values in the Observed Values DataTable Object are valid.
    /// </summary>
    /// <returns>Vaildation Result Object</returns>
    public override ValidationResult ValidateInput()
    {
      List<string> errorMsgList = new List<string>();

      errorMsgList.AddRange(CheckTwoStageObsTable(lv1Obs, "Level 1 Observation"));
      errorMsgList.AddRange(CheckTwoStageObsTable(lv2Obs, "Level 2 Observation"));
      if (string.IsNullOrWhiteSpace(SSBBreakVal.ToString()))
      {
        errorMsgList.Add("Missing SSB Break Value.");
      }

      return errorMsgList.EnumerateValidationResults();
    }
  }
}