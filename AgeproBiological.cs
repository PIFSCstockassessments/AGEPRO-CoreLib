using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace AGEPRO.CoreLib 
{
    public class AgeproBiological
    {
        public bool timeVarying { get; set; }
        public DataTable TSpawn { get; set; } //Fraction Mortality Prior To Spawning
        public int numYears { get; set; }

        public AgeproBiological()
        {

        }

        //TODO: Test if Function is sucessful.
        /// <summary>
        /// Read in AGEPRO Biological Options from the AGEPRO Input File StreamReader
        /// </summary>
        /// <param name="sr">AGEPRO Input Data File StreamReader</param>
        /// <param name="yearSeq">Year Sequence</param>
        public void ReadBiologicalData(StreamReader sr, int [] yearSeq)
        {
            this.numYears = yearSeq.Count();
            //time varying
            string line = sr.ReadLine();
            string[] bioLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
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
            DataTable tSpawnTable = new DataTable("tSpawnTable");

            if (this.timeVarying)
            {
                //TFemale
                line = sr.ReadLine();
                string[] TFLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                //TMale
                line = sr.ReadLine();
                string[] TMLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < this.numYears; i++)
                {
                    tSpawnTable.Columns.Add(yearSeq[i].ToString(), typeof(double));
                }
                tSpawnTable.Rows.Add(TFLine);
                tSpawnTable.Rows.Add(TMLine);
            }
            else //All Years
            {
                tSpawnTable.Columns.Add("All Years", typeof(double));

                line = sr.ReadLine();
                string TFLine = line;
                tSpawnTable.Rows.Add(TFLine);

                line = sr.ReadLine();
                string TMLine = line;
                tSpawnTable.Rows.Add(TMLine);
            }
            this.TSpawn = tSpawnTable;
        }

        public List<string> WriteBiologicalDataLines()
        {
            List<string> outputLines = new List<string>();
            outputLines.Add("[BIOLOGICAL]");
            outputLines.Add(Convert.ToInt32(this.timeVarying).ToString());
            foreach (DataRow gRow in this.TSpawn.Rows)
            {
                outputLines.Add(string.Join(new string(' ', 2), gRow.ItemArray));
            }
            return outputLines;
        }
    }
}
