using System;
using System.Collections.Generic;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// Empirical CDF of Recruitment w/ Linear Decline to Zero.
  /// </summary>
  public class EmpiricalCDFZero : EmpiricalRecruitment
  {
    private double? _SSBHinge;

    public double? SSBHinge
    {
      get => _SSBHinge;
      set => SetProperty(ref _SSBHinge, value);
    }

    public EmpiricalCDFZero(int modelNum) : base(modelNum)
    {
      SubType = EmpiricalType.CDFZero;
      SSBHinge = 0;  //Fallback Default
    }

    /// <summary>
    /// Reads in AGEPRO Input File Stream For Empirical CDF Recruitment w/ Linear 
    /// Decline to Zero Specfic Parameters & Data 
    /// </summary>
    /// <param name="sr">AGEPRO Input File StreamReader</param>
    public override void ReadRecruitmentModel(StreamReader sr)
    {
      string line;

      //numObs and obsTable w/ base EmpiricalRecruitment function
      base.ReadRecruitmentModel(sr);

      //SSB Hinge (MT*1000)
      line = sr.ReadLine();
      SSBHinge = Convert.ToDouble(line);

    }

    /// <summary>
    /// Translates Empirical CDF Recruitment w/ Linear Decline to Zero input data 
    /// and parameters into the AGEPRO input file data format.
    /// </summary>
    /// <returns>List of strings. Each string repesents a line from the input file.</returns>
    public override List<string> WriteRecruitmentDataModelData()
    {
      List<string> outputLine = new List<string>();
      outputLine.AddRange(base.WriteRecruitmentDataModelData());
      outputLine.Add(SSBHinge.ToString());
      return outputLine;
    }

    /// <summary>
    /// Checks the values in the Observed Values DataTable Object are valid.
    /// </summary>
    /// <returns>Vaildation Result Object</returns>
    public override ValidationResult ValidateInput()
    {
      //SSB Hinge
      if (string.IsNullOrWhiteSpace(SSBHinge.ToString()))
      {
        return new ValidationResult(false, "Missing SSB Hinge Value");
      }
      else
      {
        if (SSBHinge < 0.001)
        {
          return new ValidationResult(false,
              "SSB Hinge Value is less than lower limit of 0.001");
        }
      }



      return base.ValidateInput();
    }
  }
}