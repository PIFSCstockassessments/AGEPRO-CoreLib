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
            line = sr.ReadLine();
            string[] harvestValueLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            
            DataRow dr = G.NewRow();
            
            //Harvest Spec
            for (int i = 0; i < nyears; i++)
            {
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
                dr[0] = iyearHarvestSpec;

                G.Rows.Add(dr);

            }
            ////Harvest Value (or Value per nfleet)
            for (int k = 0; k < nfleet; k++)
            {
                int kfleetIndexStart = 0 + (nyears * k);
                int kfleetIndexEnd = nyears + (nyears * k);
                string[] harvestValueLineFleet = harvestValueLine.SplitArray(kfleetIndexStart, kfleetIndexEnd);

                for (int i = 0; i < nyears; i++)
                {
                    //Add Harvest Value Column values to existing data table
                    G.Rows[i][k + 1] = Convert.ToDouble(harvestValueLineFleet[i]); 
                }
                
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
