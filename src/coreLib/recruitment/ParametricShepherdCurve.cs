using System;
using System.Collections.Generic;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
  public class ParametricShepherdCurve : ParametricCurve
  {
    private double _kParm;

    public double kParm
    {
      get => _kParm;
      set => SetProperty(ref _kParm, value);
    }

    public ParametricShepherdCurve(int modelNum, bool isAutocorrelated)
        : base(modelNum, isAutocorrelated)
    {

    }

    /// <summary>
    /// Reads in AGEPRO Input File Stream For Shepherd Curve Recruitment Specfic Parameters & Data
    /// </summary>
    /// <param name="sr">AGEPRO Input File StreamReader</param>
    public override void ReadRecruitmentModel(StreamReader sr)
    {
      string line;
      line = sr.ReadLine();
      string[] parametricLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

      //Check parametricLine was split into 4 parameters. Only Shepherd Models have 4 parameters.
      if (parametricLine.Length != 4)
      {
        throw new InvalidAgeproParameterException("Shepherd Curve must have 4 parameters." +
            Environment.NewLine + "Number of parameters found: " + parametricLine.Length + ".");
      }

      alpha = Convert.ToDouble(parametricLine[0]);
      beta = Convert.ToDouble(parametricLine[1]);
      kParm = Convert.ToDouble(parametricLine[2]);
      variance = Convert.ToDouble(parametricLine[3]);

      if (Autocorrelated)
      {
        ReadAutocorrelatedValues(sr);
      }
    }

    /// <summary>
    /// Translates Shepherd Curve Recruitment input data and parameters into the
    /// AGEPRO input file data format.
    /// </summary>
    /// <returns>List of strings. Each string repesents a line from the input file.</returns>
    public override List<string> WriteRecruitmentDataModelData()
    {
      List<string> outputLines = new List<string>();

      outputLines.Add(alpha.ToString().PadRight(12) +
          beta.ToString().PadRight(12) +
          kParm.ToString().PadRight(12) +
          variance.ToString().PadRight(12));

      if (Autocorrelated)
      {
        outputLines.Add(Phi.ToString().PadRight(12) + LastResidual.ToString().PadRight(12));
      }
      return outputLines;
    }

    /// <summary>
    /// Shepherd Curve parameter validation.
    /// </summary>
    /// <returns>Vaildation Result Object</returns>
    public override ValidationResult ValidateInput()
    {
      var msgList = new List<string>();

      msgList.AddRange(ValidateParametricParameter(alpha, "Alpha"));
      msgList.AddRange(ValidateParametricParameter(beta, "Beta"));
      msgList.AddRange(ValidateParametricParameter(kParm, "KParm"));
      msgList.AddRange(ValidateParametricParameter(variance, "Variance"));

      if (Autocorrelated)
      {
        msgList.AddRange(ValidateParametricParameter(Phi, "Phi"));
        msgList.AddRange(ValidateParametricParameter(LastResidual, "Last Residual"));
      }
      var results = msgList.EnumerateValidationResults();

      return results;
    }
  }
}
