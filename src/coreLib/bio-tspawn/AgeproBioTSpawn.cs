using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// Biologcal: Fraction Mortality Prior to Spawning
  /// </summary>
  public class AgeproBioTSpawn
  {
    public bool TimeVarying { get; set; }
    public DataTable TSpawn { get; set; } //Fraction Mortality Prior To Spawning
    public int NumYears { get; set; }

    public AgeproBioTSpawn()
    {

    }


    /// <summary>
    /// Creates Fraction Mortality Prior to Spawn DataTable with non-null values.
    /// </summary>
    /// <param name="yearSeq">Array of subsequent years in the projection</param>
    public void CreateFallbackTSpawnTable(string[] yearSeq)
    {

      if (TSpawn != null)
      {
        //Clear out data 
        TSpawn.Reset();
      }

      DataTable fallbackTSpawn = new DataTable();

      if (TimeVarying)
      {
        for (int icol = 0; icol < yearSeq.Count(); icol++)
        {
          _ = fallbackTSpawn.Columns.Add(yearSeq[icol]);
        }
      }
      else
      {
        _ = fallbackTSpawn.Columns.Add("All Years");
      }

      //Add the Fraction Prior to Spawn
      _ = fallbackTSpawn.Rows.Add();
      _ = fallbackTSpawn.Rows.Add();

      TSpawn = Extensions.FillDBNullCellsWithZero(fallbackTSpawn);

    }

    /// <summary>
    /// Read in AGEPRO Biological Options from the AGEPRO Input File StreamReader
    /// </summary>
    /// <param name="sr">AGEPRO Input Data File StreamReader</param>
    /// <param name="yearSeq">Year Sequence</param>
    public void ReadBiologicalData(StreamReader sr, int[] yearSeq)
    {
      NumYears = yearSeq.Count();
      //time varying
      string line = sr.ReadLine();
      string[] bioLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      string optTSpawn = bioLine.First();
      TimeVarying = optTSpawn.Equals("1");

      //Fraction mortality prior to spawning 
      DataTable tSpawnTable = new DataTable("tSpawnTable");

      if (TimeVarying)
      {
        //TFemale
        line = sr.ReadLine();
        string[] TFLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        //TMale
        line = sr.ReadLine();
        string[] TMLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < NumYears; i++)
        {
          _ = tSpawnTable.Columns.Add(yearSeq[i].ToString(), typeof(double));
        }
        _ = tSpawnTable.Rows.Add(TFLine);
        _ = tSpawnTable.Rows.Add(TMLine);
      }
      else //All Years
      {
        _ = tSpawnTable.Columns.Add("All Years", typeof(double));

        line = sr.ReadLine();
        _ = tSpawnTable.Rows.Add((string)line);

        line = sr.ReadLine();
        _ = tSpawnTable.Rows.Add((string)line);
      }
      TSpawn = tSpawnTable;
    }

    /// <summary>
    /// Translates the Fraction Mortality Prior to Spawning DataTable into the
    /// AGEPRO input file data format.
    /// </summary>
    /// <returns>List of strings. Each string repesents a line from the input file.</returns>
    public List<string> WriteBiologicalDataLines()
    {
      if (TSpawn == null)
      {
        throw new NullReferenceException("Fraction Mortality Prior to Spawn ([BIOLOGICAL]) is NULL.");
      }

      List<string> outputLines = new List<string>
      {
        "[BIOLOGICAL]",
        Convert.ToInt32(TimeVarying).ToString()
      };
      foreach (DataRow gRow in TSpawn.Rows)
      {
        outputLines.Add(string.Join(new string(' ', 2), gRow.ItemArray));
      }
      return outputLines;
    }
  }
}
