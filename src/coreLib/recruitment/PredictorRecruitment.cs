﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nmfs.Agepro.CoreLib
{
    /// <summary>
    /// Predictor Recruitment
    /// </summary>
    public class PredictorRecruitment : RecruitmentModel
    {
        private int _numRecruitPredictors;
        private double _variance;
        private double _intercept;
        
        public int numRecruitPredictors
        { 
            get { return _numRecruitPredictors; }
            set { SetProperty(ref _numRecruitPredictors, value); }
        }
        public double variance
        { 
            get { return _variance; }
            set { SetProperty(ref _variance, value); }
        }
        public double intercept 
        {
            get { return _intercept; }
            set { SetProperty(ref _intercept, value); }
        }
        public DataTable coefficientTable { get; set; }
        public DataTable observationTable { get; set; }
        //public int[] obsYears { get; set; }
        public int maxNumPredictors { get; set; }

        public PredictorRecruitment(int modelNum) 
        {
            this.recruitModelNum = modelNum;
            this.recruitCategory = 3;
            this.maxNumPredictors = 5;
        }

        /// <summary>
        /// Reads in AGEPRO Input File Stream for Predictor Recruitment parameters & data.
        /// </summary>
        /// <param name="sr">AGEPRO Input File StreamReader</param>
        public override void ReadRecruitmentModel(StreamReader sr) 
        {
            //16, 17, 18, 19
            string line;

            line = sr.ReadLine();
            string[] predictorParamLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            this.numRecruitPredictors = Convert.ToInt32(predictorParamLine[0]);
            this.variance = Convert.ToDouble(predictorParamLine[1]);
            this.intercept = Convert.ToDouble(predictorParamLine[2]);

            //Check numRecruitPredictors <= 5 (max is 5)
            if (this.numRecruitPredictors > this.maxNumPredictors)
            {
                throw new ArgumentOutOfRangeException("numRecruitPredictors",this.numRecruitPredictors,
                    "Number of Recruitment Predictors of Recruit Model "+this.recruitModelNum+
                    " is greater than the Maximum of "+this.maxNumPredictors );
            }


            //Coefficents
            this.coefficientTable = SetNewCoefficientTable(this.numRecruitPredictors);
            
            line = sr.ReadLine();
            string[] predictorCoefficents = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            
            int ipredictor = 0;
            foreach (DataRow coefRow in coefficientTable.Rows)
            {
                coefRow["Coefficient"] = (Convert.ToDouble(predictorCoefficents[ipredictor]));
                ipredictor++;
            }
            
            //Observations
            this.observationTable = 
                SetNewObsTable(this.numRecruitPredictors, Array.ConvertAll(this.obsYears, element => element.ToString()) );
            
            foreach (DataRow predictorRow in this.observationTable.Rows)
            {
                line = sr.ReadLine();
                string[] observationsPerYear = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                predictorRow.ItemArray = observationsPerYear;
            }
            
        }

        /// <summary>
        /// Accessor to creates a new Coefficient Table.
        /// </summary>
        /// <remarks> Allows the interface moduule to setup this table.</remarks>
        /// <param name="numPredictors">Number of Predictors</param>
        /// <returns>New, Blank CoefficientTable</returns>
        public static DataTable SetNewCoefficientTable(int numPredictors)
        {
            DataTable coefficientTable = new DataTable("Coefficients");
            coefficientTable.Columns.Add("Coefficient", typeof(double));
            for (int i = 0; i < numPredictors; i++)
            {
                coefficientTable.Rows.Add();
            }
            return coefficientTable;
        }

        /// <summary>
        /// Accessor to create a new Observation DataTable
        /// </summary>
        /// <remarks> Allows the interface moduule to setup this table.</remarks>
        /// <param name="numPredictors">Number of Predictors</param>
        /// <param name="obsYears">Observed Years</param>
        /// <returns>Blank Obervation Table</returns>
        public static DataTable SetNewObsTable(int numPredictors, string[] obsYears)
        {
            DataTable obsTable = new DataTable("Observations");
            for (int j = 0; j < obsYears.Count(); j++)
            {
                obsTable.Columns.Add(obsYears[j], typeof(double));
            }
            for (int i = 0; i < numPredictors; i++)
            {
                obsTable.Rows.Add();
            }
            
            return obsTable;
        }

        /// <summary>
        /// Translates Predictor Recruitment input data and parameters into the
        /// AGEPRO input file data format.
        /// </summary>
        /// <returns>List of strings. Each string repesents a line from the input file.</returns>
        public override List<string> WriteRecruitmentDataModelData()
        {
            List<string> outputLines = new List<string>();

            outputLines.Add(this.numRecruitPredictors.ToString() + new string(' ', 2) +
                this.variance.ToString().PadRight(12) + this.intercept.ToString());

            List<string> coefficientCol = new List<string>();
            foreach (DataRow predictorRow in coefficientTable.Rows)
            {
                coefficientCol.Add(predictorRow[0].ToString());
            }
            outputLines.Add(string.Join(new String(' ',2), coefficientCol).PadRight(12));
            foreach (DataRow predictorRow in observationTable.Rows)
            {
                outputLines.Add(string.Join(new string(' ', 2), predictorRow.ItemArray.ToString()));
            }

            return outputLines;
        }

        /// <summary>
        /// Predictor Recuitment Validation
        /// </summary>
        /// <returns>
        /// If all validation checks have been met, nothing will be returned.
        /// All validations not met will be recorded to a list of "Error Messages" to return.
        /// </returns>
        public override ValidationResult ValidateInput()
        {
            List<string> errorMsgList = new List<string>();
            //Make Sure Number of Recruitment Predictors is not zero
            if (this.numRecruitPredictors == 0)
            {
                errorMsgList.Add("Zero number of recruitment predictors.");
            }

            if (this.coefficientTable.Rows.Count == 0)
            {
                errorMsgList.Add("Coefficient Table has 0 rows.");
            }
            else
            {
                if (this.HasBlankOrNullCells(this.coefficientTable))
                {
                    errorMsgList.Add("Coefficient table has missing or null data.");
                }
            }

            if (this.observationTable.Columns.Count == 0)
            {
                errorMsgList.Add("Missing year projection time series. Observation Table has 0 Columns.");
            }
            if (this.observationTable.Rows.Count == 0)
            {
                errorMsgList.Add("Observation Table has 0 rows.");
            }
            else
            {
                if (this.HasBlankOrNullCells(this.observationTable))
                {
                    errorMsgList.Add("Observation table has missing or null data.");
                }
            }

            var results = errorMsgList.EnumerateValidationResults();

            return results;
        }
    }
}