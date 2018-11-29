using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace Nmfs.Agepro.CoreLib 
{
    public class AgeproBiological
    {
        public bool timeVarying { get; set; }
        public DataTable TSpawn { get; set; } //Fraction Mortality Prior To Spawning
        public int numYears { get; set; }

        public AgeproBiological()
        {

        }


        /// <summary>
        /// Creates Fraction Mortality Prior to Spawn DataTable with non-null values.
        /// </summary>
        /// <param name="yearSeq">Array of subsequent years in the projection</param>
        public void CreateFallbackTSpawnTable(string [] yearSeq)
        {
            
            if(this.TSpawn != null)
            {
                //Clear out data 
                this.TSpawn.Reset();
            }
            
            DataTable fallbackTSpawn = new DataTable();

            if (this.timeVarying)
            {
                for (int icol = 0; icol < yearSeq.Count(); icol++)
                {
                    fallbackTSpawn.Columns.Add(yearSeq[icol]);
                }
            }
            else
            {
                fallbackTSpawn.Columns.Add("All Years");
            }

            //Add the Fraction Prior to Spawn
            fallbackTSpawn.Rows.Add();
            fallbackTSpawn.Rows.Add();

            this.TSpawn = Extensions.FillDBNullCellsWithZero(fallbackTSpawn);

        }

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

        /// <summary>
        /// Translates the Fraction Mortality Prior to Spawning DataTable into the
        /// AGEPRO input file data format.
        /// </summary>
        /// <returns>List of strings. Each string repesents a line from the input file.</returns>
        public List<string> WriteBiologicalDataLines()
        {
            if (this.TSpawn == null)
            {
                throw new NullReferenceException("Fraction Mortality Prior to Spawn ([BIOLOGICAL]) is NULL.");
            }

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
