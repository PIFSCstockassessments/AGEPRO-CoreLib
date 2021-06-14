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
    public ParametricType Subtype { get; set; }
    public double? Phi
    {
      get => _Phi;
      set => SetProperty(ref _Phi, value);
    }
    public double? LastResidual
    {
      get => _LastResidual;
      set => SetProperty(ref _LastResidual, value);
    }
    public bool Autocorrelated
    {
      get => _Autocorrelated;
      set => SetProperty(ref _Autocorrelated, value);
    }

    public ParametricRecruitment(int modelNum)
    {
      recruitModelNum = modelNum;
      recruitCategory = 2;
      Autocorrelated = false;
    }

    public ParametricRecruitment(int modelNum, bool isAutocorrelated) : this(modelNum)
    {
      Autocorrelated = isAutocorrelated;

      if (Autocorrelated)
      {
        //If enabled, these values will be set to '0' (instead of null)
        LastResidual = LastResidual.GetValueOrDefault();
        Phi = Phi.GetValueOrDefault();
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
      string line = sr.ReadLine();
      string[] autoCorrLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

      Phi = Convert.ToDouble(autoCorrLine[0]);
      LastResidual = Convert.ToDouble(autoCorrLine[1]);
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
      List<string> msgList = new List<string>();

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
      List<string> msgList = new List<string>();

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
}
