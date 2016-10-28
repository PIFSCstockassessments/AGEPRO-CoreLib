using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AGEPRO.CoreLib
{
    public class PStarCalculation : HarvestCalculation
    {
        private int _targetYear;
        private double _pStarF;
        private DataTable _pStarTable;
        private int _pStarLevels;

        public int targetYear
        {
            get { return _targetYear; }
            set { SetProperty(ref _targetYear, value); }
        }
        public double pStarF
        {
            get { return _pStarF; }
            set { SetProperty(ref _pStarF, value); }
        }
        public DataTable pStarTable
        {
            get { return _pStarTable; }
            set { SetProperty(ref _pStarTable, value); }
        }
        public int pStarLevels
        {
            get { return _pStarLevels; }
            set { SetProperty(ref _pStarLevels, value); }
        }

        public PStarCalculation()
        {
            calculationType = HarvestScenarioAnalysis.PStar;
        }

        /// <summary>
        /// Reads in AGEPRO Input Data File for P-Star Data Specifications
        /// </summary>
        /// <param name="sr">AGEPRO Input Data File StreamReader</param>
        public override void ReadCalculationDataLines(System.IO.StreamReader sr)
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
                pStarTable.Columns.Add("Level " + (i + 1).ToString(), typeof(double));
            }
            pStarTable.Rows.Add(pStarLevelData);

            //Overfishing F
            line = sr.ReadLine();
            this.pStarF = Convert.ToDouble(line);

            //Target Year
            line = sr.ReadLine();
            this.targetYear = Convert.ToInt32(line);

        }


        /// <summary>
        /// Generates AGEPRO Input Data file lines related to the [PSTAR] parameter
        /// </summary>
        /// <returns>A list of strings to be appended to the AGEPRO Input Data file.</returns>
        public override List<string> WriteCalculationDataLines()
        {
            List<string> outputLines = new List<string>();
            outputLines.Add("[PSTAR]");
            outputLines.Add(this.pStarLevels.ToString());

            if (this.pStarTable.Rows.Count > 1)
            {
                throw new ApplicationException("P-Star Data Table Has more than one row");
            }
            foreach (DataRow dtRow in this.pStarTable.Rows)
            {
                outputLines.Add(string.Join(new string(' ', 2), dtRow.ItemArray.ToString()));
            }
            outputLines.Add(this.pStarF.ToString());
            return outputLines;
        }
    }
}
