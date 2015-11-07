using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace AGEPRO_struct 
{
    public class AGEPRO_biological : AGEPRO_input
    {
        public bool timeVarying { get; set; }
        public DataTable TSpawn { get; set; } //Fraction Mortality Prior To Spawning


        public AGEPRO_biological()
        {

        }

        //TODO: Test if Function is sucessful.
        public void ReadBiologicalData(StreamReader sr, int [] yearSeq)
        {
            this.nYears = yearSeq.Count();
            //time varying
            string line = sr.ReadLine();
            string[] bioLine = line.Split(' ');
            string optTSpawn = bioLine.First();
            if (optTSpawn.Equals("1"))
            {
                this.timeVarying = true;
            }
            else
            {
                this.timeVarying = false;
            }

            //Fraction mortality prior to spawning 
            DataTable tSpawnTable = new DataTable();

            if (this.timeVarying)
            {
                //TF
                line = sr.ReadLine();
                string[] TFLine = line.Split(' ');

                //TM
                line = sr.ReadLine();
                string[] TMLine = line.Split(' ');

                for (int i = 0; i < this.nYears; i++)
                {
                    tSpawnTable.Columns.Add(yearSeq[i].ToString(), typeof(double));
                }
                tSpawnTable.Rows.Add(TFLine);
                tSpawnTable.Rows.Add(TMLine);
            }
            else
            {
                line = sr.ReadLine();
                string TFLine = line;
                tSpawnTable.Rows.Add(TFLine);

                line = sr.ReadLine();
                string TMLine = line;
                tSpawnTable.Rows.Add(TMLine);
            }
            this.TSpawn = tSpawnTable;
        }
    }
}
