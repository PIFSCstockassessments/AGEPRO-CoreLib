using System;
using System.Collections.Generic;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
  public class ParametricShepherdCurve : ParametricCurve
  {
    private double _KParm;

    public double KParm
    {
      get => _KParm;
      set => SetProperty(ref _KParm, value);
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
      if (sr is null)
      {
        throw new ArgumentNullException(nameof(sr));
      }
      string line = sr.ReadLine();
      string[] parametricLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

      //Check parametricLine was split into 4 parameters. Only Shepherd Models have 4 parameters.
      if (parametricLine.Length != 4)
      {
        throw new InvalidAgeproParameterException("Shepherd Curve must have 4 parameters." +
            Environment.NewLine + "Number of parameters found: " + parametricLine.Length + ".");
      }

      Alpha = Convert.ToDouble(parametricLine[0]);
      Beta = Convert.ToDouble(parametricLine[1]);
      KParm = Convert.ToDouble(parametricLine[2]);
      Variance = Convert.ToDouble(parametricLine[3]);

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

      outputLines.Add(Alpha.ToString().PadRight(12) +
          Beta.ToString().PadRight(12) +
          KParm.ToString().PadRight(12) +
          Variance.ToString().PadRight(12));

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
      List<string> msgList = new List<string>();

      msgList.AddRange(ValidateParametricParameter(Alpha, "Alpha"));
      msgList.AddRange(ValidateParametricParameter(Beta, "Beta"));
      msgList.AddRange(ValidateParametricParameter(KParm, "KParm"));
      msgList.AddRange(ValidateParametricParameter(Variance, "Variance"));

      if (Autocorrelated)
      {
        msgList.AddRange(ValidateParametricParameter(Phi, "Phi"));
        msgList.AddRange(ValidateParametricParameter(LastResidual, "Last Residual"));
      }

      return msgList.EnumerateValidationResults();
    }
  }
}
