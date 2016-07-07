﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace AGEPRO.CoreLib
{
    /// <summary>
    /// AGEPRO Harvest Scenario Parameters. 
    /// </summary>
    public class AGEPRO_HarvestScenario
    {
        public DataTable harvestScenarioTable { get; set; }
        public int targetYear { get; set; }

        public AGEPRO_HarvestScenario()
        {

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
                    default:
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


    }

    /// <summary>
    /// Specifications for Stock Rebuilder Targets
    /// </summary>
    public class RebuilderTarget : AGEPRO_HarvestScenario
    {
        public double targetValue { get; set; }
        public int targetType { get; set; } //rebuild target type (cboRebuild.SelectedIndex)
        public double targetPercent { get; set; } //Percent Confidence

        /// <summary>
        /// Readin AGEPRO Input Data File for Rebuild Specification Parameters
        /// </summary>
        /// <param name="sr">AGEPRO Input Data File StreamReader</param>
        public void ReadRebuildData(StreamReader sr)
        {
            string line;
            line = sr.ReadLine();
            string[] rebuildOptionLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            this.targetYear = Convert.ToInt32(rebuildOptionLine[0]);
            this.targetValue = Convert.ToDouble(rebuildOptionLine[1]);
            this.targetType = Convert.ToInt32(rebuildOptionLine[2]);
            this.targetPercent = Convert.ToDouble(rebuildOptionLine[3]);

        }
    }

    /// <summary>
    /// P-Star Analysis
    /// </summary>
    public class PStar : AGEPRO_HarvestScenario
    {
        public int pStarLevels { get; set; }
        public DataTable pStarTable; 
        public double pStarF { get; set; }

        /// <summary>
        /// Readin AGEPRO Input Data File for P-Star Data Specifications
        /// </summary>
        /// <param name="sr">AGEPRO Input Data File StreamReader</param>
        public void ReadPStarData(StreamReader sr)
        {
            string line;
            this.pStarTable = new DataTable("pStar");
            
            //Number of pStar Levels
            line = sr.ReadLine();
            this.pStarLevels = Convert.ToInt32(line);

            //pStar Level Values
            line = sr.ReadLine();
            string[] pStarLevelData = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < this.pStarLevels; i++)
            {
                pStarTable.Columns.Add("Level " + (i+1).ToString(), typeof(double));
            }
            pStarTable.Rows.Add(pStarLevelData);

            //Overfishing F
            line = sr.ReadLine();
            this.pStarF = Convert.ToDouble(line);

            //Target Year
            line = sr.ReadLine();
            this.targetYear = Convert.ToInt32(line);

        }
    }

}