using System;
using System.Collections.Generic;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// Parameteric Recruitment Using the Berverton-Holt, Ricker, or Shepherd Curve
  /// </summary>
  public class ParametricCurve : ParametricRecruitment
  {
    private double _alpha;
    private double _beta;
    private double _variance;

    public double alpha
    {
      get => _alpha;
      set => SetProperty(ref _alpha, value);
    }
    public double beta
    {
      get => _beta;
      set => SetProperty(ref _beta, value);
    }
    public double variance
    {
      get => _variance;
      set => SetProperty(ref _variance, value);
    }

    public ParametricCurve(int modelNum, bool isAutocorrelated) : base(modelNum, isAutocorrelated)
    {
      Subtype = ParametricType.Curve;
    }

    /// <summary>
    /// Reads in AGEPRO Input File Stream For Parametric Recruitment Specfic Parameters & Data
    /// </summary>
    /// <param name="sr">AGEPRO Input File StreamReader</param>
    public override void ReadRecruitmentModel(StreamReader sr)
    {
      string line;
      line = sr.ReadLine();
      string[] parametricLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

      //Verify if this Parametric Model has 3 parameters
      //Shepherd Curve Models do not have 3 parameters
      if (parametricLine.Length == 3)
      {
        alpha = Convert.ToDouble(parametricLine[0]);
        beta = Convert.ToDouble(parametricLine[1]);
        variance = Convert.ToDouble(parametricLine[2]);
      }
      else
      {
        //Throw error
        throw new InvalidAgeproParameterException("Number of parametric curve parameters must be 3." +
            Environment.NewLine + "Number of parameters found: " + parametricLine.Length + ".");
      }

      if (Autocorrelated)
      {
        ReadAutocorrelatedValues(sr);
      }
    }


    /// <summary>
    /// Translates Parametric Curve Recruitment input data and parameters into the
    /// AGEPRO input file data format.
    /// </summary>
    /// <returns>List of strings. Each string repesents a line from the input file.</returns>
    public override List<string> WriteRecruitmentDataModelData()
    {
      List<string> outputLines = new List<string>();

      outputLines.Add(alpha.ToString().PadRight(12) +
          beta.ToString().PadRight(12) +
          variance.ToString().PadRight(12));

      if (Autocorrelated)
      {
        outputLines.Add(Phi.ToString().PadRight(12) + LastResidual.ToString().PadRight(12));
      }

      return outputLines;
    }

    /// <summary>
    /// Parametric curve parameter validation.
    /// </summary>
    /// <returns>Vaildation Result Object</returns>
    public override ValidationResult ValidateInput()
    {
      var msgList = new List<string>();

      msgList.AddRange(ValidateParametricParameter(alpha, "Alpha"));
      msgList.AddRange(ValidateParametricParameter(beta, "Beta"));
      msgList.AddRange(ValidateParametricParameter(variance, "Variance"));

      if (Autocorrelated)
      {
        msgList.AddRange(ValidateParametricParameter(Phi, "Phi"));
        msgList.AddRange(ValidateParametricParameter(LastResidual,
            "Last Residual"));
      }
      var results = msgList.EnumerateValidationResults();

      return results;
    }
  }
}
