using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// Two-Stage Empirical Recruitment. Parameters & Observations for two levels (stages).
  /// </summary>
  public class TwoStageEmpiricalRecruitment : EmpiricalRecruitment
  {
    private int _lv1NumObs;
    private int _lv2NumObs;
    private int _SSBBreakVal;

    public DataTable lv1Obs { get; set; }
    public DataTable lv2Obs { get; set; }

    public int lv1NumObs
    {
      get => _lv1NumObs;
      set => SetProperty(ref _lv1NumObs, value);
    }
    public int lv2NumObs
    {
      get => _lv2NumObs;
      set => SetProperty(ref _lv2NumObs, value);
    }
    public int SSBBreakVal
    {
      get => _SSBBreakVal;
      set => SetProperty(ref _SSBBreakVal, value);
    }

    public TwoStageEmpiricalRecruitment(int modelNum)
        : base(modelNum)
    {
      recruitModelNum = modelNum;
      recruitCategory = 1;
      WithSSB = true;
      SubType = EmpiricalType.TwoStage;
      LowBound = 0.0001;

      //Fallback Defaults
      lv1NumObs = 0;
      lv2NumObs = 0;
      SSBBreakVal = 0;

    }

    public TwoStageEmpiricalRecruitment(int modelNum, bool useSSB)
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
      lv1NumObs = Convert.ToInt32(lineNumObsLvl[0]);
      lv2NumObs = Convert.ToInt32(lineNumObsLvl[1]);

      //lv1Obs 
      lv1Obs = base.ReadObsTable(sr, lv1NumObs);
      //lv2Obs
      lv2Obs = base.ReadObsTable(sr, lv2NumObs);

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
      List<string> outputLines = new List<string>();
      outputLines.Add(lv1NumObs + new string(' ', 2) + lv2NumObs);

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

      var results = errorMsgList.EnumerateValidationResults();
      return results;
    }
  }
}