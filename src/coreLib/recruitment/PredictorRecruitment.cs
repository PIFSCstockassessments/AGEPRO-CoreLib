using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// Predictor Recruitment
  /// </summary>
  public class PredictorRecruitment : RecruitmentModelProperty
  {
    private int _NumRecruitPredictors;
    private double _Variance;
    private double _Intercept;
 
    public DataTable CoefficientTable { get; set; }
    public DataTable ObservationTable { get; set; }
    public int MaxNumPredictors { get; set; }

    public int NumRecruitPredictors
    {
      get => _NumRecruitPredictors;
      set => SetProperty(ref _NumRecruitPredictors, value);
    }
    public double Variance
    {
      get => _Variance;
      set => SetProperty(ref _Variance, value);
    }
    public double Intercept
    {
      get => _Intercept;
      set => SetProperty(ref _Intercept, value);
    }

    public PredictorRecruitment(int modelNum)
    {
      recruitModelNum = modelNum;
      recruitCategory = 3;
      MaxNumPredictors = 5;
    }

    /// <summary>
    /// Reads in AGEPRO Input File Stream for Predictor Recruitment parameters & data.
    /// </summary>
    /// <param name="sr">AGEPRO Input File StreamReader</param>
    public override void ReadRecruitmentModel(StreamReader sr)
    {

      //16, 17, 18, 19
      string line = sr.ReadLine();
      string[] predictorParamLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      NumRecruitPredictors = Convert.ToInt32(predictorParamLine[0]);
      Variance = Convert.ToDouble(predictorParamLine[1]);
      Intercept = Convert.ToDouble(predictorParamLine[2]);

      //Check numRecruitPredictors <= 5 (max is 5)
      if (NumRecruitPredictors > MaxNumPredictors)
      {
        throw new ArgumentOutOfRangeException("numRecruitPredictors", NumRecruitPredictors,
          message: "Number of Recruitment Predictors of Recruit Model " + recruitModelNum +
          " is greater than the Maximum of " + MaxNumPredictors);
      }


      //Coefficents
      CoefficientTable = SetNewCoefficientTable(NumRecruitPredictors);

      line = sr.ReadLine();
      string[] predictorCoefficents = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

      int ipredictor = 0;
      foreach (DataRow coefRow in CoefficientTable.Rows)
      {
        coefRow["Coefficient"] = Convert.ToDouble(predictorCoefficents[ipredictor]);
        ipredictor++;
      }

      //Observations
      ObservationTable = SetNewObsTable(NumRecruitPredictors, Array.ConvertAll(obsYears, element => element.ToString()));

      foreach (DataRow predictorRow in ObservationTable.Rows)
      {
        line = sr.ReadLine();
        predictorRow.ItemArray = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      }

    }

    /// <summary>
    /// Accessor to creates a new Coefficient Table.
    /// </summary>
    /// <remarks> Allows the interface moduule to setup this table.</remarks>
    /// <param name="numPredictors">Number of Predictors</param>
    /// <returns>New, Blank CoefficientTable</returns>
    public static DataTable SetNewCoefficientTable(int numPredictors)
    {
      DataTable coefficientTable = new DataTable("Coefficients");
      _ = coefficientTable.Columns.Add("Coefficient", typeof(double));
      for (int i = 0; i < numPredictors; i++)
      {
        _ = coefficientTable.Rows.Add();
      }
      return coefficientTable;
    }

    /// <summary>
    /// Accessor to create a new Observation DataTable
    /// </summary>
    /// <remarks> Allows the interface moduule to setup this table.</remarks>
    /// <param name="numPredictors">Number of Predictors</param>
    /// <param name="obsYears">Observed Years</param>
    /// <returns>Blank Obervation Table</returns>
    public static DataTable SetNewObsTable(int numPredictors, string[] obsYears)
    {
      if (obsYears is null)
      {
        throw new ArgumentNullException(nameof(obsYears));
      }

      DataTable obsTable = new DataTable("Observations");
      for (int j = 0; j < obsYears.Count(); j++)
      {
        _ = obsTable.Columns.Add(obsYears[j], typeof(double));
      }
      for (int i = 0; i < numPredictors; i++)
      {
        _ = obsTable.Rows.Add();
      }

      return obsTable;
    }

    /// <summary>
    /// Translates Predictor Recruitment input data and parameters into the
    /// AGEPRO input file data format.
    /// </summary>
    /// <returns>List of strings. Each string repesents a line from the input file.</returns>
    public override List<string> WriteRecruitmentDataModelData()
    {
      List<string> outputLines = new List<string>
      {
        NumRecruitPredictors.ToString() + new string(' ', 2) +
          Variance.ToString().PadRight(12) + Intercept.ToString()
      };

      List<string> coefficientCol = new List<string>();
      foreach (DataRow predictorRow in CoefficientTable.Rows)
      {
        coefficientCol.Add(predictorRow[0].ToString());
      }
      outputLines.Add(string.Join(new string(' ', 2), coefficientCol).PadRight(12));
      foreach (DataRow predictorRow in ObservationTable.Rows)
      {
        outputLines.Add(string.Join(new string(' ', 2), predictorRow.ItemArray.ToString()));
      }

      return outputLines;
    }

    /// <summary>
    /// Predictor Recuitment Validation
    /// </summary>
    /// <returns>
    /// If all validation checks have been met, nothing will be returned.
    /// All validations not met will be recorded to a list of "Error Messages" to return.
    /// </returns>
    public override ValidationResult ValidateInput()
    {
      List<string> errorMsgList = new List<string>();
      //Make Sure Number of Recruitment Predictors is not zero
      if (NumRecruitPredictors == 0)
      {
        errorMsgList.Add("Zero number of recruitment predictors.");
      }

      if (CoefficientTable.Rows.Count == 0)
      {
        errorMsgList.Add("Coefficient Table has 0 rows.");
      }
      else
      {
        if (HasBlankOrNullCells(CoefficientTable))
        {
          errorMsgList.Add("Coefficient table has missing or null data.");
        }
      }

      if (ObservationTable.Columns.Count == 0)
      {
        errorMsgList.Add("Missing year projection time series. Observation Table has 0 Columns.");
      }
      if (ObservationTable.Rows.Count == 0)
      {
        errorMsgList.Add("Observation Table has 0 rows.");
      }
      else
      {
        if (HasBlankOrNullCells(ObservationTable))
        {
          errorMsgList.Add("Observation table has missing or null data.");
        }
      }


      return errorMsgList.EnumerateValidationResults();
    }
  }
}
