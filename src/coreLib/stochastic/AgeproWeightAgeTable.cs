using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections.ObjectModel;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// Weight Specfic Stochastic Weights-at-age AGEPRO parameter class.
  /// </summary>
  public class AgeproWeightAgeTable : AgeproStochasticAgeTable
  {
    public int weightOpt { get; set; }  //Weight Option
    public int[] validOpt { get; set; } //Valid weight options

    public AgeproWeightAgeTable()
    {
      validOpt = new int[] { 1, 0, -1, -2, -3, -4 };
    }
    public AgeproWeightAgeTable(int[] validOptions)
    {
      validOpt = validOptions;
    }

    /// <summary>
    /// Sets option for stochastic weights-at-age to either: inputted maunually, 
    /// read from the AGEPRO Input File, or use a valid weight option
    /// </summary>
    /// <param name="optParam">String Character from <paramref name="validOpt"/></param>
    protected override void SetStochasticAgeOption(string optParam)
    {
      if (string.IsNullOrWhiteSpace(optParam))
      {
        FromFile = null;
        throw new ArgumentException($"'{nameof(optParam)}' cannot be null or whitespace.", nameof(optParam));
      }

      if (optParam.Equals("0"))
      {
        FromFile = false; //0=User Spec by Age
        weightOpt = Convert.ToInt32(optParam);
      }
      else if (optParam.Equals("1"))
      {
        FromFile = true; //1=From File
        weightOpt = Convert.ToInt32(optParam);
      }
      else
      {
        FromFile = null;
        //Check if weightOpt is a valid one
        weightOpt = validOpt.Contains(Convert.ToInt32(optParam))
            ? Convert.ToInt32(optParam)
            : throw new InvalidOperationException("Weight option not valid for current Weights of Age Model");


      }
    }

    /// <summary>
    /// Reads in AGEPRO Input Filename for the 'Read Weights From File' option 
    /// </summary>
    /// <param name="sr">AGEPRO Input File StreamReader</param>
    protected override void ReadStochasticAgeTableFilename(System.IO.StreamReader sr)
    {
      //If Option 0=fromFile, read fromFile options
      //otherwise ignore and finish reading the current AGEPRO parameter
      if (weightOpt == 0)
      {
        base.ReadStochasticAgeTableFilename(sr);
      }

    }

    public override List<string> WriteStochasticAgeDataLines(string keyword)
    {

      List<string> outputLines = new List<string>();

      outputLines.Add(keyword); //[PARAMETER]
      outputLines.Add(weightOpt.ToString() + new string(' ', 2) + Convert.ToInt32(TimeVarying).ToString());
      //since fromFile is a nullable boolean, have to explicitly check if its true 
      if (FromFile == true)
      {
        outputLines.Add(DataFile);
      }
      // 0 == User Specfied Weights at Age
      else if (weightOpt == 0)
      {

        if (ByAgeData == null)
        {
          throw new NullReferenceException("Stochastic Age of " +
              keyword + " is NULL.");
        }
        if (ByAgeCV == null)
        {
          throw new NullReferenceException("Stochastic CV of " +
              keyword + " is NULL.");
        }

        foreach (DataRow yearRow in ByAgeData.Rows)
        {
          outputLines.Add(string.Join(new string(' ', 2), yearRow.ItemArray));
        }

        foreach (DataRow cvRow in ByAgeCV.Rows)
        {
          outputLines.Add(string.Join(new string(' ', 2), cvRow.ItemArray));
        }
      }
      else if (!(validOpt.Contains(weightOpt)))
      {
        throw new InvalidOperationException("Invalid Weight option.");
      }

      return outputLines;
    }

  }
}
