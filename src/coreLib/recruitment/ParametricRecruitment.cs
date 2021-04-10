using System;
using System.Collections.Generic;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{

  /// <summary>
  /// Parametric Recruitment
  /// </summary>
  public class ParametricRecruitment : RecruitmentModelProperty
  {
    private double? _Phi;
    private double? _LastResidual;
    private bool _Autocorrelated;

    public double? Phi
    {
      get { return _Phi; }
      set { SetProperty(ref _Phi, value); }
    }
    public double? LastResidual
    {
      get { return _LastResidual; }
      set { SetProperty(ref _LastResidual, value); }
    }
    public bool Autocorrelated
    {
      get { return _Autocorrelated; }
      set { SetProperty(ref _Autocorrelated, value); }
    }
    public ParametricType Subtype { get; set; }

    public ParametricRecruitment(int modelNum)
    {
      this.recruitModelNum = modelNum;
      this.recruitCategory = 2;
      this.Autocorrelated = false;

    }
    public ParametricRecruitment(int modelNum, bool isAutocorrelated) : this(modelNum)
    {
      this.Autocorrelated = isAutocorrelated;

      if (this.Autocorrelated)
      {
        //If enabled, these values will be set to '0' (instead of null)
        this.LastResidual = LastResidual.GetValueOrDefault();
        this.Phi = Phi.GetValueOrDefault();
      }
    }


    public override void ReadRecruitmentModel(StreamReader sr)
    {
      throw new NotImplementedException();
    }

    public override List<string> WriteRecruitmentDataModelData()
    {
      throw new NotImplementedException();
    }
    public override ValidationResult ValidateInput()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Reads in AGEPRO Input File Stream for Autocorrelated Values: Phi, and Last Residual.
    /// </summary>
    /// <param name="sr">AGEPRO Input File StreamReader</param>
    protected void ReadAutocorrelatedValues(StreamReader sr)
    {
      string line;
      line = sr.ReadLine();
      string[] autoCorrLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

      this.Phi = Convert.ToDouble(autoCorrLine[0]);
      this.LastResidual = Convert.ToDouble(autoCorrLine[1]);
    }

    /// <summary>
    /// Parameteric Parameter Validation
    /// </summary>
    /// <param name="param">Parameter Value</param>
    /// <param name="paramName">Name of Parameter</param>
    /// <param name="significantBound">Significant Bound</param>
    /// <returns>
    /// If parameter clears all validation checks, nothing will be returned.
    /// All validations not met will be recorded to a list of "Error Messages" to return.
    /// </returns>
    protected List<string> ValidateParametricParameter(double param, string paramName,
        double significantBound = 0.000000001)
    {
      var msgList = new List<string>();

      if (string.IsNullOrWhiteSpace(param.ToString()))
      {
        msgList.Add("Missing or empty " + paramName + " value.");
      }
      else
      {
        if (Math.Abs(param) < significantBound)
        {
          msgList.Add(paramName + " value is zero or less significant than " + significantBound + ".");
        }
      }
      return msgList;
    }

    /// <summary>
    /// Parametric Parameter Validation
    /// </summary>
    /// <param name="param">Parameter Value. Can be NULL</param>
    /// <param name="paramName">Name of Parameter</param>
    /// <param name="significantBound">Significant Bound</param>
    /// <returns>       
    /// If parameter clears all validation checks, nothing will be returned.
    /// All validations not met will be recorded to a list of "Error Messages" to return.
    /// </returns>
    protected List<string> ValidateParametricParameter(double? param, string paramName,
        double significantBound = 0.000000001)
    {
      var msgList = new List<string>();

      if (param != null)
      {
        msgList.AddRange(ValidateParametricParameter(param.Value, paramName, significantBound));
      }
      else
      {
        msgList.Add("Missing " + paramName + " value.");
      }

      return msgList;
    }

  }

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
      get { return _alpha; }
      set { SetProperty(ref _alpha, value); }
    }
    public double beta
    {
      get { return _beta; }
      set { SetProperty(ref _beta, value); }
    }
    public double variance
    {
      get { return _variance; }
      set { SetProperty(ref _variance, value); }
    }

    public ParametricCurve(int modelNum, bool isAutocorrelated) : base(modelNum, isAutocorrelated)
    {
      this.Subtype = ParametricType.Curve;
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
        this.alpha = Convert.ToDouble(parametricLine[0]);
        this.beta = Convert.ToDouble(parametricLine[1]);
        this.variance = Convert.ToDouble(parametricLine[2]);
      }
      else
      {
        //Throw error
        throw new InvalidAgeproParameterException("Number of parametric curve parameters must be 3." +
            Environment.NewLine + "Number of parameters found: " + parametricLine.Length + ".");
      }

      if (this.Autocorrelated)
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

      outputLines.Add(this.alpha.ToString().PadRight(12) +
          this.beta.ToString().PadRight(12) +
          this.variance.ToString().PadRight(12));

      if (this.Autocorrelated)
      {
        outputLines.Add(this.Phi.ToString().PadRight(12) + this.LastResidual.ToString().PadRight(12));
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

      msgList.AddRange(ValidateParametricParameter(this.alpha, "Alpha"));
      msgList.AddRange(ValidateParametricParameter(this.beta, "Beta"));
      msgList.AddRange(ValidateParametricParameter(this.variance, "Variance"));

      if (this.Autocorrelated)
      {
        msgList.AddRange(ValidateParametricParameter(this.Phi, "Phi"));
        msgList.AddRange(ValidateParametricParameter(this.LastResidual,
            "Last Residual"));
      }
      var results = msgList.EnumerateValidationResults();

      return results;
    }
  }

  public class ParametricShepherdCurve : ParametricCurve
  {
    private double _kParm;

    public double kParm
    {
      get { return _kParm; }
      set { SetProperty(ref _kParm, value); }
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

      this.alpha = Convert.ToDouble(parametricLine[0]);
      this.beta = Convert.ToDouble(parametricLine[1]);
      this.kParm = Convert.ToDouble(parametricLine[2]);
      this.variance = Convert.ToDouble(parametricLine[3]);

      if (this.Autocorrelated)
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

      outputLines.Add(this.alpha.ToString().PadRight(12) +
          this.beta.ToString().PadRight(12) +
          this.kParm.ToString().PadRight(12) +
          this.variance.ToString().PadRight(12));

      if (this.Autocorrelated)
      {
        outputLines.Add(this.Phi.ToString().PadRight(12) + this.LastResidual.ToString().PadRight(12));
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

      msgList.AddRange(ValidateParametricParameter(this.alpha, "Alpha"));
      msgList.AddRange(ValidateParametricParameter(this.beta, "Beta"));
      msgList.AddRange(ValidateParametricParameter(this.kParm, "KParm"));
      msgList.AddRange(ValidateParametricParameter(this.variance, "Variance"));

      if (this.Autocorrelated)
      {
        msgList.AddRange(ValidateParametricParameter(this.Phi, "Phi"));
        msgList.AddRange(ValidateParametricParameter(this.LastResidual, "Last Residual"));
      }
      var results = msgList.EnumerateValidationResults();

      return results;
    }
  }

  /// <summary>
  /// Parmetric Recruitment in Lognormal Distribution
  /// </summary>
  public class ParametricLognormal : ParametricRecruitment
  {
    private double _mean;
    private double _stdDev;

    public double mean
    {
      get { return _mean; }
      set { SetProperty(ref _mean, value); }
    }
    public double stdDev
    {
      get { return _stdDev; }
      set { SetProperty(ref _stdDev, value); }
    }

    public ParametricLognormal(int modelNum, bool isAutocorrelated) : base(modelNum, isAutocorrelated)
    {
      this.Subtype = ParametricType.Lognormal;
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

      this.mean = Convert.ToDouble(logParamLine[0]);
      this.stdDev = Convert.ToDouble(logParamLine[1]);

      if (this.Autocorrelated)
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
      outputLines.Add(this.mean.ToString().PadRight(12) + this.stdDev.ToString().PadRight(12));
      if (this.Autocorrelated)
      {
        outputLines.Add(this.Phi.ToString().PadRight(12) + this.LastResidual.ToString().PadRight(12));
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

      msgList.AddRange(ValidateParametricParameter(this.mean, "Mean"));
      msgList.AddRange(ValidateParametricParameter(this.stdDev, "Std. Deviaition"));

      if (this.Autocorrelated)
      {
        msgList.AddRange(ValidateParametricParameter(this.Phi.Value, "Phi"));
        msgList.AddRange(ValidateParametricParameter(this.LastResidual.Value,
            "Last Residual"));
      }
      var results = msgList.EnumerateValidationResults();

      return results;
    }
  }
}
