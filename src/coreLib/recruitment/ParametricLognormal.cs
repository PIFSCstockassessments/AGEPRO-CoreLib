using System;
using System.Collections.Generic;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// Parmetric Recruitment in Lognormal Distribution
  /// </summary>
  public class ParametricLognormal : ParametricRecruitment
  {
    private double _Mean;
    private double _StdDev;

    public double Mean
    {
      get => _Mean;
      set => SetProperty(ref _Mean, value);
    }
    public double StdDev
    {
      get => _StdDev;
      set => SetProperty(ref _StdDev, value);
    }

    public ParametricLognormal(int modelNum, bool isAutocorrelated) : base(modelNum, isAutocorrelated)
    {
      Subtype = ParametricType.Lognormal;
    }

    /// <summary>
    /// Reads in AGEPRO Input File Stream For Parametric Lognornal Recruitment Specfic Parameters & Data
    /// </summary>
    /// <param name="sr">AGEPRO Input File StreamReader</param>
    public override void ReadRecruitmentModel(StreamReader sr)
    {
      string line = sr.ReadLine();
      string[] logParamLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

      Mean = Convert.ToDouble(logParamLine[0]);
      StdDev = Convert.ToDouble(logParamLine[1]);

      if (Autocorrelated)
      {
        ReadAutocorrelatedValues(sr);
      }
    }

    /// <summary>
    /// Translates Parametric Logmormal Recruitment input data and parameters into the
    /// AGEPRO input file data format.
    /// </summary>
    /// <returns>List of strings. Each string repesents a line from the input file.</returns>
    public override List<string> WriteRecruitmentDataModelData()
    {
      List<string> outputLines = new List<string>
      {
        Mean.ToString().PadRight(12) + StdDev.ToString().PadRight(12)
      };
      if (Autocorrelated)
      {
        outputLines.Add(Phi.ToString().PadRight(12) + LastResidual.ToString().PadRight(12));
      }
      return outputLines;
    }

    /// <summary>
    /// Parametric Lognormal parameter validation.
    /// </summary>
    /// <returns>Vaildation Result Object</returns>
    public override ValidationResult ValidateInput()
    {
      List<string> msgList = new List<string>();

      msgList.AddRange(ValidateParametricParameter(Mean, "Mean"));
      msgList.AddRange(ValidateParametricParameter(StdDev, "Std. Deviaition"));

      if (Autocorrelated)
      {
        msgList.AddRange(ValidateParametricParameter(Phi.Value, "Phi"));
        msgList.AddRange(ValidateParametricParameter(LastResidual.Value,
            "Last Residual"));
      }

      return msgList.EnumerateValidationResults();
    }
  }
}
