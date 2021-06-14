using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{

  /// <summary>
  /// Empirical Recruitment
  /// </summary>
  public class EmpiricalRecruitment : RecruitmentModelProperty, IValidatable
  {
    private int _NumObs;

    public DataTable ObsTable { get; set; }
    public bool WithSSB { get; set; }
    public EmpiricalType SubType { get; set; }
    protected double LowBound { get; set; }

    public int NumObs
    {
      get => _NumObs;
      set => SetProperty(ref _NumObs, value);
    }

    public EmpiricalRecruitment(int modelNum)
    {
      recruitModelNum = modelNum;
      recruitCategory = 1;
      WithSSB = false;
      NumObs = 0;      //Fallback Default
      LowBound = 0.0001;
    }

    public EmpiricalRecruitment(int modelNum, bool useSSB, EmpiricalType subType) : this(modelNum)
    {
      WithSSB = useSSB;
      SubType = subType;
    }

    /// <summary>
    /// Reads in AGEPRO Input File Stream For Empirical Recruitment Specfic Parameters & Data 
    /// </summary>
    /// <param name="sr">AGEPRO Input File StreamReader</param>
    public override void ReadRecruitmentModel(StreamReader sr)
    {
      if (sr is null)
      {
        throw new ArgumentNullException(nameof(sr));
      }

      //numObs
      string line = sr.ReadLine();
      NumObs = Convert.ToInt32(line);

      //obsTable
      ObsTable = ReadObsTable(sr, NumObs);

    }

    /// <summary>
    /// Gets the Observed Values DataTable from the input stream. 
    /// </summary>
    /// <param name="sr">AGEPRO Input File StreamReader</param>
    /// <param name="numObs">Number of Observations</param>
    /// <returns>Observed Values DataTable Object</returns>
    protected DataTable ReadObsTable(StreamReader sr, int numObs)
    {
      if (sr is null)
      {
        throw new ArgumentNullException(nameof(sr));
      }

      //obsRecruits
      string line = sr.ReadLine();
      string[] nobsRecruits = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      //obsSSB           
      if (WithSSB)
      {
        line = sr.ReadLine();  //read another line
        string[] nobsSSB = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        return SetObsTableData(numObs, nobsRecruits, nobsSSB);
      }
      else
      {
        return SetObsTableData(numObs, nobsRecruits);
      }


    }

    /// <summary>
    /// Sets a new Observation Datatable and populates it with string array paramters. 
    /// </summary>
    /// <param name="numObs">Number of Obervations</param>
    /// <param name="obsRecruits">Observations values vector</param>
    /// <param name="obsSSB">Spawning Stock Biomass (SSB) vector</param>
    /// <returns>Returns a DataTable with the Observed (and Spawning Stock Biomass) values</returns>
    protected DataTable SetObsTableData(int numObs, string[] obsRecruits, string[] obsSSB = null)
    {
      //inputTable
      DataTable inputTable = SetNewObsTable(numObs);
      int i = 0;
      foreach (DataRow obsRow in inputTable.Rows)
      {
        obsRow["Recruits"] = Convert.ToDouble(obsRecruits[i]);
        if (WithSSB)
        {
          obsRow["SSB"] = Convert.ToDouble(obsSSB[i]);
        }
        i++;
      }

      return inputTable;
    }

    /// <summary>
    /// Creates an empty observation data table.
    /// </summary>
    /// <param name="numObs">Number of Observation Rows.</param>
    /// <returns>Returns a Empty Data Table</returns>
    public DataTable SetNewObsTable(int numObs)
    {
      //inputTable
      DataTable obsTable = new DataTable("Observation Table");
      obsTable.Columns.Add("Recruits", typeof(double));
      if (WithSSB)
      {
        obsTable.Columns.Add("SSB", typeof(double));
      }

      for (int i = 0; i < numObs; i++)
      {
        obsTable.Rows.Add();
      }

      return obsTable;
    }

    /// <summary>
    /// Translates Empirical Recruitment and parameters into the
    /// AGEPRO input file data format.
    /// </summary>
    /// <returns>List of strings. Each string repesents a line from the input file.</returns>
    public override List<string> WriteRecruitmentDataModelData()
    {
      List<string> outputLines = new List<string>();
      outputLines.Add(NumObs.ToString());

      //obsTable
      outputLines.AddRange(WriteObsTableLines(ObsTable, WithSSB));

      return outputLines;
    }

    /// <summary>
    /// Translates Observed Values data table object into the
    /// AGEPRO input file data format. 
    /// </summary>
    /// <param name="recruitObsTable">Recruitment Observation DataTable Object</param>
    /// <param name="hasSSBCols">DataTable has Spawning Stock Biomass (SSB) Columns</param>
    /// <returns>List of strings. Each string repesents a line from the input file.</returns>
    protected List<string> WriteObsTableLines(DataTable recruitObsTable, bool hasSSBCols)
    {
      List<string> obsTableLines = new List<string>();

      List<string> obsRecritCol = new List<string>();
      List<string> obsSSBCol = new List<string>();
      foreach (DataRow obsRow in recruitObsTable.Rows)
      {
        obsRecritCol.Add(obsRow["Recruits"].ToString());
        if (hasSSBCols)
        {
          obsSSBCol.Add(obsRow["SSB"].ToString());
        }
      }
      obsTableLines.Add(string.Join(new string(' ', 2), obsRecritCol));

      if (hasSSBCols)
      {
        obsTableLines.Add(string.Join(new string(' ', 2), obsSSBCol));
      }

      return obsTableLines;
    }

    /// <summary>
    /// Checks the values in the Observed Values DataTable Object are valid.
    /// </summary>
    /// <returns>Vaildation Result Object</returns>
    public override ValidationResult ValidateInput()
    {
      if (HasBlankOrNullCells(ObsTable))
      {
        return new ValidationResult(false, "Missing Data in observation table");
      }
      if (TableHasAllSignificantValues(ObsTable, LowBound) == false)
      {
        return new ValidationResult(false, "Insignificant values or values lower than "
            + LowBound + " found in observation table");
      }

      return new ValidationResult(true, "Validation Successful");

    }
  }
}