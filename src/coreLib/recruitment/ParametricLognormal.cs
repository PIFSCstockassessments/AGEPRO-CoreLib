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
    private double _mean;
    private double _stdDev;

    public double mean
    {
      get => _mean;
      set => SetProperty(ref _mean, value);
    }
    public double stdDev
    {
      get => _stdDev;
      set => SetProperty(ref _stdDev, value);
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
      string line;
      line = sr.ReadLine();
      string[] logParamLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

      mean = Convert.ToDouble(logParamLine[0]);
      stdDev = Convert.ToDouble(logParamLine[1]);

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
      List<string> outputLines = new List<string>();
      outputLines.Add(mean.ToString().PadRight(12) + stdDev.ToString().PadRight(12));
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

      msgList.AddRange(ValidateParametricParameter(mean, "Mean"));
      msgList.AddRange(ValidateParametricParameter(stdDev, "Std. Deviaition"));

      if (Autocorrelated)
      {
        msgList.AddRange(ValidateParametricParameter(Phi.Value, "Phi"));
        msgList.AddRange(ValidateParametricParameter(LastResidual.Value,
            "Last Residual"));
      }
      var results = msgList.EnumerateValidationResults();

      return results;
    }
  }
}
