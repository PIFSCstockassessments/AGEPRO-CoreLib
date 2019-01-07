using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
    public enum HarvestScenarioAnalysis
    {
        HarvestScenario,
        Rebuilder,
        PStar,
    };

    /// <summary>
    /// AGEPRO Harvest Scenario Parameters. 
    /// </summary>
    public class AgeproHarvestScenario
    {
        public DataTable harvestScenarioTable { get; set; }
        public int targetYear { get; set; }
        public HarvestScenarioAnalysis analysisType { get; set; }

        public AgeproHarvestScenario()
        {
            analysisType = HarvestScenarioAnalysis.HarvestScenario;
        }

        /// <summary>
        /// Reads in Harvest Specficiation basis for each year from the AGEPRO input file. 
        /// </summary>
        /// <param name="sr">AGEPRO Input File StreamReader</param>
        /// <param name="nyears">Number of years.</param>
        /// <param name="nfleet">Number of Fleets. Default is <c>1</c>.</param>
        public void ReadHarvestTable(StreamReader sr, int nyears, int nfleet=1)
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
                    
                    G.Columns.Add("FLEET-" + (i+1).ToString(), typeof(double));

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
                for (int j = 0 ; j < harvestNFleetLines.Count(); j++)
                {
                    string[] nfleetValue = harvestNFleetLines[j];

                    dr[j + 1] = Convert.ToDouble(nfleetValue[i]);
                }
           
                G.Rows.Add(dr);
            }
            
            this.harvestScenarioTable = G;
        }

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
            harvestScenarioTable = Extensions.FillDBNullCellsWithZero(harvestScenarioTable);

            List<string> outputLines = new List<string>();
            outputLines.Add("[HARVEST]");

            //HARVEST SPEC
            Dictionary<string, string> harvestSpecDict = new Dictionary<string, string>();
            harvestSpecDict.Add("F-MULT", "0");
            harvestSpecDict.Add("LANDINGS", "1");
            harvestSpecDict.Add("REMOVALS", "2");

            List<string> specCol = new List<string>();
            foreach (DataRow dtRow in this.harvestScenarioTable.Rows)
            {
                string specOptNum;
                if (harvestSpecDict.TryGetValue(dtRow["Harvest Spec"].ToString(), out specOptNum))
                {
                    specCol.Add(specOptNum);
                }
            }
            outputLines.Add(string.Join(new string(' ',2), specCol));

            //HARVEST FLEET-N
            for (int fleetCol = 0; fleetCol < (harvestScenarioTable.Columns.Count - 1); fleetCol++ )
            {
                List<string> harvestValCol = new List<string>();
                foreach (DataRow dtRow in this.harvestScenarioTable.Rows)
                {
                    harvestValCol.Add(dtRow[fleetCol + 1].ToString());
                }
                outputLines.Add(string.Join(new string(' ',2), harvestValCol));

            }

            return outputLines;
        }
    }

}
