using System;
using System.Collections.Generic;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// Customized maximum bounds for Weight and Natural Mortality   
  /// </summary>
  public class Bounds : AgeproOptionsProperty
  {
    private double _MaxWeight;
    private double _MaxNatMort;

    public double MaxWeight
    {
      get => _MaxWeight;
      set => SetProperty(ref _MaxWeight, value);
    }
    public double MaxNatMort
    {
      get => _MaxNatMort;
      set => SetProperty(ref _MaxNatMort, value);
    }

    public Bounds()
    {
      //Set defaults
      MaxWeight = 10.0;
      MaxNatMort = 1.0;
    }

    public string ReadBounds(StreamReader sr)
    {
      string line = sr.ReadLine();
      string[] boundsOpt = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      MaxWeight = Convert.ToDouble(boundsOpt[0]);
      MaxNatMort = Convert.ToDouble(boundsOpt[1]);
      return line;
    }

    public List<string> WriteBoundsLines()
    {
      return new List<string>
      {
        "[BOUNDS]",
        MaxWeight + new string(' ', 2) + MaxNatMort
      };
    }
  }

}
