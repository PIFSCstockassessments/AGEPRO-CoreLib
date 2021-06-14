using System;
using System.Collections.Generic;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// Scaling Factors for the AGEPRO Output Report
  /// </summary>
  public class ScaleFactors : AgeproOptionsProperty
  {
    private double _ScaleBio;
    private double _ScaleRec;
    private double _ScaleStockSum;

    public double ScaleBio
    {
      get => _ScaleBio;
      set => SetProperty(ref _ScaleBio, value);
    }
    public double ScaleRec
    {
      get => _ScaleRec;
      set => SetProperty(ref _ScaleRec, value);
    }
    public double ScaleStockNum
    {
      get => _ScaleStockSum;
      set => SetProperty(ref _ScaleStockSum, value);
    }

    public ScaleFactors()
    {
      //Set Defaults to 0
      ScaleBio = 0;
      ScaleRec = 0;
      ScaleStockNum = 0;
    }

    public string ReadScaleFactors(StreamReader sr)
    {
      string line = sr.ReadLine();
      string[] scaleOpt = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      ScaleBio = Convert.ToDouble(scaleOpt[0]);
      ScaleRec = Convert.ToDouble(scaleOpt[1]);
      ScaleStockNum = Convert.ToDouble(scaleOpt[2]);
      return line;
    }

    public List<string> WriteScaleFactors()
    {
      return new List<string>
      {
        "[SCALE]",
        ScaleBio + new string(' ', 2) 
        + ScaleRec + new string(' ', 2) 
        + ScaleStockNum + new string(' ', 2)
      };
    }
  }

}
