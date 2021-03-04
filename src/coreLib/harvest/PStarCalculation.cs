using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Nmfs.Agepro.CoreLib
{
    public class PStarCalculation : AgeproHarvestScenario
    {

        private double _pStarF;
        private DataTable _pStarTable;
        private int _pStarLevels;

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
            this.pStarLevels = 1;
            this.pStarF = 0;
            this.targetYear = 0;
            //Create PStar Table
            this.pStarTable = this.CreateNewPStarTable();
            this.pStarTable.Rows.Add();
            Nmfs.Agepro.CoreLib.Extensions.FillDBNullCellsWithZero(this.pStarTable);
        }

        /// <summary>
        /// Creates a new instance of a PStar Levels DataTable
        /// </summary>
        /// <param name="levels">Number of columns to create. Defaults to 1.</param>
        /// <returns>A row-less PStar DataTable Instance.</returns>
        public DataTable CreateNewPStarTable(int levels = 1)
        {
            DataTable newPStarTable = new DataTable("pStar");

            for (int i = 0; i < levels; i++)
            {
                newPStarTable.Columns.Add("Level " + (i + 1).ToString(), typeof(double));
            }

            return newPStarTable;
        }


        /// <summary>
        /// Reads in AGEPRO Input Data File for P-Star Data Specifications
        /// </summary>
        /// <param name="sr">AGEPRO Input Data File StreamReader</param>
        public override void ReadCalculationDataLines(System.IO.StreamReader sr)
        {
            string line;
            
            //Number of pStar Levels
            line = sr.ReadLine();
            this.pStarLevels = Convert.ToInt32(line);

            //pStar Level Values
            line = sr.ReadLine();
            string[] pStarLevelData = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            pStarTable = CreateNewPStarTable(this.pStarLevels);
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
                outputLines.Add(string.Join(new string(' ', 2), dtRow.ItemArray));
            }
            outputLines.Add(this.pStarF.ToString());
            outputLines.Add(this.targetYear.ToString());
            return outputLines;
        }


        public override ValidationResult ValidateInput()
        {
            List<string> errorMsgList = new List<string>();
            int yrStart = obsYears[0];
            int yrEnd = obsYears[(obsYears.Count() - 1)];
            //Target Year
            if (this.targetYear < yrStart || this.targetYear > yrEnd)
            {
                errorMsgList.Add("Invalid P-Star Year Specification.");
            }
            //
            if (string.IsNullOrWhiteSpace(this.pStarLevels.ToString()))
            {
                errorMsgList.Add("Invalid or missing number of P-Star Levels.");
            }
            if (string.IsNullOrWhiteSpace(this.pStarF.ToString()))
            {
                errorMsgList.Add("Invalid or missing overfishing value.");
            }

            double xPstar = 0.0;
            double currentPstar;
            foreach (DataRow pstarLvl in this.pStarTable.Rows)
            {
                foreach (var item in pstarLvl.ItemArray)
                {
                    currentPstar = Convert.ToDouble(item);
                    if (xPstar > currentPstar)
                    {
                        errorMsgList.Add("P-Star Levels must be in ascending order.");
                        break;
                    }
                    else
                    {
                        xPstar = currentPstar;
                    }
                }
            }

            var results = errorMsgList.EnumerateValidationResults();
            return results;
        }
    }
}
