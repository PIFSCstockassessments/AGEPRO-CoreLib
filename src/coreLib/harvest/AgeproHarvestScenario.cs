using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{

  /// <summary>
  /// AGEPRO Harvest Scenario Parameters. 
  /// </summary>
  public class AgeproHarvestScenario : HarvestCalculation
  {
    private int _TargetYear;
    private HarvestScenarioAnalysis _analysisType;
    public DataTable HarvestScenarioTable { get; set; }

    public int TargetYear
    {
      get => _TargetYear;
      set => SetProperty(ref _TargetYear, value);
    }
    public HarvestScenarioAnalysis AnalysisType
    {
      get => _analysisType;
      set => SetProperty(ref _analysisType, value);
    }

    public AgeproHarvestScenario()
    {
      AnalysisType = HarvestScenarioAnalysis.HarvestScenario;
      TargetYear = 0;
    }


    public override List<string> WriteCalculationDataLines()
    {
      return WriteHarvestTableDataLines();
    }

    public override void ReadCalculationDataLines(StreamReader sr)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Reads in Harvest Specficiation basis for each year from the AGEPRO input file. 
    /// </summary>
    /// <param name="sr">AGEPRO Input File StreamReader</param>
    /// <param name="nyears">Number of years.</param>
    /// <param name="nfleet">Number of Fleets. Default is <c>1</c>.</param>
    public void ReadHarvestTable(StreamReader sr, int nyears, int nfleet = 1)
    {
      string line;
      DataTable G = new DataTable("Harvest Scenario");
      //Read Harvest Specficiation
      line = sr.ReadLine();
      string[] harvestSpecLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

      //Set Columns and save HarvestSpec/Nfleet Values to list
      G.Columns.Add("Harvest Spec", typeof(string));
      List<string[]> harvestNFleetLines = new List<string[]>(nfleet);
      if (nfleet.Equals(1))
      {
        line = sr.ReadLine();
        string[] harvestValueLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        harvestNFleetLines.Add(harvestValueLine);

        G.Columns.Add("HARVEST VALUE", typeof(double));
      }
      else
      {
        for (int i = 0; i < nfleet; i++)
        {
          //Readin next line for next nfleet
          line = sr.ReadLine();
          string[] harvestValueLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
          harvestNFleetLines.Add(harvestValueLine);

          G.Columns.Add("FLEET-" + (i + 1).ToString(), typeof(double));

        }
      }


      for (int i = 0; i < nyears; i++)
      {
        DataRow dr = G.NewRow();
        string iyearHarvestSpec;
        int ihs = Convert.ToInt32(harvestSpecLine[i]);
        switch (ihs)
        {
          case 0:
            iyearHarvestSpec = "F-MULT";
            break;
          case 1:
            iyearHarvestSpec = "LANDINGS";
            break;
          default:  //or 2
            iyearHarvestSpec = "REMOVALS";
            break;
        }
        //Harvest Spec
        dr[0] = iyearHarvestSpec;

        //Column Harvest Value/Fleet-N
        for (int j = 0; j < harvestNFleetLines.Count(); j++)
        {
          string[] nfleetValue = harvestNFleetLines[j];

          dr[j + 1] = Convert.ToDouble(nfleetValue[i]);
        }

        G.Rows.Add(dr);
      }

      HarvestScenarioTable = G;
    }

    /// <summary>
    /// Creates New Harvest Scenario Table
    /// </summary>
    /// <param name="nyears">Number of Years</param>
    /// <param name="nfleet">Number of Fleets</param>
    /// <returns>Returns 1-fleet matrix table w/ defaults harvest specification LANDINGS and rate of 0.0. </returns>
    public static DataTable NewHarvestTable(int nyears, int nfleet = 1)
    {
      object[] harvestFleetRow;
      DataTable G = new DataTable("Harvest Scenario");
      G.Columns.Add("Harvest Spec", typeof(string));
      //Set Harvest Table Columns
      if (nfleet == 1)
      {
        G.Columns.Add("HARVEST VALUE", typeof(double));

        //Use Defaults ("LANDINGS" and 0.0) for values
        harvestFleetRow = new object[2];
        harvestFleetRow[0] = "LANDINGS";
        harvestFleetRow[1] = 0.0;
      }
      else
      {
        harvestFleetRow = new object[1 + nfleet];

        for (int colFleet = 0; colFleet < nfleet; colFleet++)
        {
          G.Columns.Add("FLEET-" + (colFleet + 1).ToString(), typeof(double));
        }

        //Use Defaults ("LANDINGS" and 0.0) for values
        harvestFleetRow[0] = "LANDINGS";
        for (int arrayIndex = 1; arrayIndex < harvestFleetRow.Length; arrayIndex++)
        {
          harvestFleetRow[arrayIndex] = 0.0;
        }
      }
      //Add the Rows 
      for (int irow = 0; irow < nyears; irow++)
      {
        G.Rows.Add(harvestFleetRow);
      }

      return G;
    }

    /// <summary>
    /// Translates the Harvest Table input data and parameters into the
    /// AGEPRO input file data format.
    /// </summary>
    /// <returns>List of strings. Each string repesents a line from the input file.</returns>
    public List<string> WriteHarvestTableDataLines()
    {
      //Check if Harvest Table has blank/null cells, and if it does fill it w/ a zero
      HarvestScenarioTable = Extensions.FillDBNullCellsWithZero(HarvestScenarioTable);


      //HARVEST SPEC
      Dictionary<string, string> harvestSpecDict = new Dictionary<string, string>
      {
        { "F-MULT", "0" },
        { "LANDINGS", "1" },
        { "REMOVALS", "2" }
      };

      List<string> specCol = new List<string>();
      foreach (DataRow dtRow in HarvestScenarioTable.Rows)
      {
        if (harvestSpecDict.TryGetValue(dtRow["Harvest Spec"].ToString(), out string specOptNum))
        {
          specCol.Add(specOptNum);
        }
      }

      List<string> outputLines = new List<string>
      {
        "[HARVEST]"
      };
      outputLines.Add(string.Join(new string(' ', 2), specCol));

      //HARVEST FLEET-N
      for (int fleetCol = 0; fleetCol < (HarvestScenarioTable.Columns.Count - 1); fleetCol++)
      {
        List<string> harvestValCol = new List<string>();
        foreach (DataRow dtRow in HarvestScenarioTable.Rows)
        {
          harvestValCol.Add(dtRow[fleetCol + 1].ToString());
        }
        outputLines.Add(string.Join(new string(' ', 2), harvestValCol));

      }

      return outputLines;
    }

  }

}
