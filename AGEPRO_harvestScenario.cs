using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace AGEPRO_struct
{
    public class AGEPRO_HarvestScenario
    {
        public DataTable harvestScenarioTable { get; set; }
        public int targetYear { get; set; }

        public AGEPRO_HarvestScenario()
        {

        }

        public DataTable ReadHarvestTable(StreamReader sr, int nyears, int nfleet=1)
        {
            string line;
            DataTable G = new DataTable();
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
            
            return G;
        }


    }

    public class RebuilderTarget : AGEPRO_HarvestScenario
    {
        public double targetValue { get; set; }
        public int targetType { get; set; } //rebuild target type (cboRebuild.SelectedIndex)
        public double targetPercent { get; set; } //Percent Confidence

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

    public class PStar : AGEPRO_HarvestScenario
    {
        public int pStarLevels { get; set; }
        public DataTable pStarTable = new DataTable();
        public double pStarF { get; set; }

        public void ReadPStarData(StreamReader sr)
        {
            string line;
            //Number of pStar Levels
            line = sr.ReadLine();
            this.pStarLevels = Convert.ToInt32(line);

            //pStar Level Values
            line = sr.ReadLine();
            string[] pStarLevelData = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            DataRow pStarLevelRow = pStarTable.NewRow();
            for (int i = 0; i < this.pStarLevels; i++)
            {
                pStarLevelRow[i] = Convert.ToDouble(pStarLevelData[i]);
            }
            pStarTable.Rows.Add(pStarLevelRow);

            //Overfishing F
            line = sr.ReadLine();
            this.pStarF = Convert.ToDouble(line);

            //Target Year
            line = sr.ReadLine();
            this.targetYear = Convert.ToInt32(line);

        }
    }

}
