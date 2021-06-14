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
    private double _Alpha;
    private double _Beta;
    private double _Variance;

    public double Alpha
    {
      get => _Alpha;
      set => SetProperty(ref _Alpha, value);
    }
    public double Beta
    {
      get => _Beta;
      set => SetProperty(ref _Beta, value);
    }
    public double Variance
    {
      get => _Variance;
      set => SetProperty(ref _Variance, value);
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
      if (sr is null)
      {
        throw new ArgumentNullException(nameof(sr));
      }

      string line = sr.ReadLine();
      string[] parametricLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

      //Verify if this Parametric Model has 3 parameters
      //Shepherd Curve Models do not have 3 parameters
      if (parametricLine.Length != 3)
      {
        //Throw error
        throw new InvalidAgeproParameterException("Number of parametric curve parameters must be 3." +
            Environment.NewLine + "Number of parameters found: " + parametricLine.Length + ".");
      }

      Alpha = Convert.ToDouble(parametricLine[0]);
      Beta = Convert.ToDouble(parametricLine[1]);
      Variance = Convert.ToDouble(parametricLine[2]);
      

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
      List<string> outputLines = new List<string>
      {
        Alpha.ToString().PadRight(12) +
          Beta.ToString().PadRight(12) +
          Variance.ToString().PadRight(12)
      };

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
      List<string> msgList = new List<string>();

      msgList.AddRange(ValidateParametricParameter(Alpha, "Alpha"));
      msgList.AddRange(ValidateParametricParameter(Beta, "Beta"));
      msgList.AddRange(ValidateParametricParameter(Variance, "Variance"));

      if (Autocorrelated)
      {
        msgList.AddRange(ValidateParametricParameter(Phi, "Phi"));
        msgList.AddRange(ValidateParametricParameter(LastResidual,
            "Last Residual"));
      }

      return msgList.EnumerateValidationResults();
    }
  }
}
