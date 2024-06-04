using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// AGEPRO Misc Options.
  /// </summary>
  public class AgeproMiscOptions
  {
    private const string INP_keyword = "[OPTIONS]";

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
      OutputSummaryReport = 0;
      EnablePercentileReport = false;
      EnableScaleFactors = false;
      EnableBounds = false;
      EnableRetroAdjustmentFactors = false;
    }

    [Obsolete("This method is used for \"AGEPRO VERSION 4.0\" AGEPRO Input Files. " +
      "Please use method ReadAgeproOutputOptions for newer AGEPRO Input File versions")]
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
      string[] outputOptionsLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      // Parse String Array as Int
      int [] optionOpt = ParseInpLineToIntArray(outputOptionsLine);

      OutputSummaryReport = optionOpt[0];
      EnableAuxStochasticFiles = Convert.ToBoolean(optionOpt[1]);
      EnableExportR = Convert.ToBoolean(optionOpt[2]);
      return line;
    }

    [Obsolete("This method is used for \"AGEPRO VERSION 4.0\" AGEPRO Input Files. " +
      "Please use method WriteAgeproOutputOptions for newer AGEPRO Input File versions")]
    public List<string> WriteAgepro40Options()
    {
      return new List<string>
      {
        INP_keyword,
        Convert.ToInt32(EnableSummaryReport).ToString()
        + new string(' ', 2)
        + Convert.ToInt32(EnableAuxStochasticFiles).ToString()
        + new string(' ', 2)
        + Convert.ToInt32(EnableExportR).ToString()
      };
    }

    public List<string> WriteAgeproOutputOptions()
    {
      return new List<string>
      {
        INP_keyword,
        OutputSummaryReport.ToString()
        + new string(' ', 2)
        + Convert.ToInt32(EnableAuxStochasticFiles).ToString()
        + new string(' ', 2)
        + Convert.ToInt32(EnableExportR).ToString()
      };
    }

    /// <summary>
    /// Converts a array of numeric strings from the AGEPRO input file connection 
    /// as an intger array.
    /// </summary>
    /// <param name="InpLine">Line read from the AGEPRO Input File </param>
    /// <returns></returns>
    private int[] ParseInpLineToIntArray(String[] InpLine)
    {
      int[] NumericalValues = new int[InpLine.Length];

      for (int i = 0; i < InpLine.Length; i++) {
        NumericalValues[i] = int.Parse(InpLine[i]);
      }

      return NumericalValues;

    }

  }
}
