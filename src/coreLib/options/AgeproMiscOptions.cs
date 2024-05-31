using System;
using System.Collections.Generic;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// AGEPRO Misc Options.
  /// </summary>
  public class AgeproMiscOptions
  {
    public bool EnableSummaryReport { get; set; }
    public int OutputSummaryReport { get; set; }
    public bool EnableAuxStochasticFiles { get; set; }
    public bool EnableExportR { get; set; }
    //enabled if classes are called.
    public bool EnableRefpoint { get; set; }
    public bool EnablePercentileReport { get; set; }
    public bool EnableScaleFactors { get; set; }
    public bool EnableBounds { get; set; }
    public bool EnableRetroAdjustmentFactors { get; set; }

    public AgeproMiscOptions()
    {
      EnableRefpoint = false;
      EnablePercentileReport = false;
      EnableScaleFactors = false;
      EnableBounds = false;
      EnableRetroAdjustmentFactors = false;
    }

    public string ReadAgepro40Options(StreamReader sr)
    {
      if (sr is null)
      {
        throw new System.ArgumentNullException(nameof(sr));
      }

      string line = sr.ReadLine();
      string[] optionOpt = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      EnableSummaryReport = Convert.ToBoolean(Convert.ToInt32(optionOpt[0]));
      EnableAuxStochasticFiles = Convert.ToBoolean(Convert.ToInt32(optionOpt[1]));
      EnableExportR = Convert.ToBoolean(Convert.ToInt32(optionOpt[2]));
      return line;
    }

    /// <summary>
    /// Reads in the values from the keyword parameter OPTIONS from the 
    /// AGEPRO Input file
    /// </summary>
    /// <param name="sr">File reader</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public string ReadAgeproOutputOptions(StreamReader sr)
    {
      if (sr is null)
      {
        throw new System.ArgumentNullException(nameof(sr));
      }

      // Read an addtional line from the file connection and split it to 3 substrings to
      // assign as OutputSummaryReport, AuxStochasticFiles, EnableExportR
      string line = sr.ReadLine();
      string[] optionOpt = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      OutputSummaryReport = Convert.ToInt32(Convert.ToInt32(optionOpt[0]));
      EnableAuxStochasticFiles = Convert.ToBoolean(Convert.ToInt32(optionOpt[1]));
      EnableExportR = Convert.ToBoolean(Convert.ToInt32(optionOpt[2]));
      return line;
    }

    public List<string> WriteAgepro40Options()
    {
      return new List<string>
      {
        "[OPTIONS]",
        Convert.ToInt32(EnableSummaryReport).ToString() + new string(' ', 2) +
          Convert.ToInt32(EnableAuxStochasticFiles).ToString() + new string(' ', 2) +
          Convert.ToInt32(EnableExportR).ToString()
      };
    }

    public List<string> WriteAgeproOutputOptions()
    {
      return new List<string>
      {
        "[OPTIONS]",
        OutputSummaryReport.ToString() + new string(' ', 2) +
          Convert.ToInt32(EnableAuxStochasticFiles).ToString() + new string(' ', 2) +
          Convert.ToInt32(EnableExportR).ToString()
      };
    }

  }
}
