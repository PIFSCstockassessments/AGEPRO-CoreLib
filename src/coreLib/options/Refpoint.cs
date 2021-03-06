﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// Parameters for the Reference Point Threshold Report
  /// </summary>
  public class Refpoint : AgeproOptionsProperty
  {
    private double _RefSpawnBioThresh; //SSBThresh
    private double _RefJan1BioThresh;  //StockBioThresh
    private double _RefMeanBioThresh;  //MeanBioThresh
    private double _RefFMortThresh;    //FMortThresh

    //SSBThresh
    public double RefSpawnBio
    {
      get => _RefSpawnBioThresh;
      set => SetProperty(ref _RefSpawnBioThresh, value);
    }
    //StockBioThresh
    public double RefJan1Bio
    {
      get => _RefJan1BioThresh;
      set => SetProperty(ref _RefJan1BioThresh, value);
    }
    //MeanBioThresh
    public double RefMeanBio
    {
      get => _RefMeanBioThresh;
      set => SetProperty(ref _RefMeanBioThresh, value);
    }
    //FMortThresh
    public double RefFMort
    {
      get => _RefFMortThresh;
      set => SetProperty(ref _RefFMortThresh, value);
    }

    public Refpoint()
    {
      //Set Defaults to 0.0
      RefSpawnBio = 0.0;
      RefJan1Bio = 0.0;
      RefMeanBio = 0.0;
      RefFMort = 0.0;
    }

    public string ReadRefpointLines(StreamReader sr)
    {
      if (sr is null)
      {
        throw new System.ArgumentNullException(nameof(sr));
      }

      string line = sr.ReadLine();
      string[] refpointOpt = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      RefSpawnBio = Convert.ToDouble(refpointOpt[0]);
      RefJan1Bio = Convert.ToDouble(refpointOpt[1]);
      RefMeanBio = Convert.ToDouble(refpointOpt[2]);
      RefFMort = Convert.ToDouble(refpointOpt[3]);
      return line;
    }

    public List<string> WriteRefpointLines()
    {
      return new List<string>
      {
        "[REFPOINT]",
        RefSpawnBio.ToString() + new string(' ', 2)
        + RefJan1Bio.ToString() + new string(' ', 2)
        + RefMeanBio.ToString() + new string(' ', 2)
        + RefFMort.ToString()
      };
    }
  }

}
