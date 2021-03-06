﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// At-age vaiues to applied to initial population numbers to correct for retrospective bias.
  /// </summary>
  public class RetroAdjustmentFactors : AgeproOptionsProperty
  {
    public RetroAdjustmentFactors()
    {
      RetroAdjust = new DataTable("Retro Adjustment Factors");
    }

    public DataTable RetroAdjust { get; set; }

    public string ReadRetroAdjustmentFactorsTable(StreamReader sr, AgeproGeneral General)
    {
      string line = sr.ReadLine();
      string[] rafLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      DataTable rafTable = new DataTable("Retro Adjustment Factors");
      _ = rafTable.Columns.Add(); //set column without name

      //Throw warning/error if 'rafLine' length doesn't match number of Ages
      if (rafLine.Length != General.NumAges())
      {
        throw new InvalidAgeproParameterException("Number of retro adjustment factors does not match number of ages");
      }

      for (int i = 0; i < General.NumAges(); i++)
      {
        _ = rafTable.Rows.Add(rafLine[i]);
      }

      RetroAdjust = rafTable;
      return line;
    }

    public List<string> WriteRetroAdjustmentFactorsTable()
    {
      List<string> outputLines = new List<string>
      {
        "[RETROADJUST]"
      };
      List<string> rafCol = new List<string>();
      foreach (DataRow ageRow in RetroAdjust.Rows)
      {
        rafCol.Add(ageRow[0].ToString());
      }
      outputLines.Add(string.Join(new string(' ', 2), rafCol));

      return outputLines;
    }

  }

}
