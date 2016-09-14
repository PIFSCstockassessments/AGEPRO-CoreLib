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
    /// Predictor Recruitment
    /// </summary>
    public class PredictorRecruitment : RecruitmentModel
    {
        public int numRecruitPredictors { get; set; }
        public double variance { get; set; }
        public double intercept { get; set; }
        public DataTable coefficientTable { get; set; }
        public DataTable observationTable { get; set; }
        public int[] obsYears { get; set; }
        public int maxNumPredictors { get; set; }

        public PredictorRecruitment(int modelNum) 
        {
            this.recruitModelNum = modelNum;
            this.recruitCategory = 3;
            this.maxNumPredictors = 5;
        }

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
            //DataTable inputTable = new DataTable("Coefficients");
            //inputTable.Columns.Add("Coefficient", typeof(double));

            line = sr.ReadLine();
            string[] predictorCoefficents = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //for (int i = 0; i < this.numRecruitPredictors; i++)
            //{
            //    inputTable.Rows.Add(Convert.ToDouble(predictorCoefficents[i]));
            //}
            //this.coefficientTable = inputTable;
            this.coefficientTable = SetNewCoefficientTable(this.numRecruitPredictors);
            int ipredictor = 0;
            foreach (DataRow coefRow in coefficientTable.Rows)
            {
                coefRow["Coefficient"] = (Convert.ToDouble(predictorCoefficents[ipredictor]));
                ipredictor++;
            }
            
            //Observations
            //inputTable = new DataTable("Observations");
            //for (int j = 0; j < obsYears.Count(); j++)
            //{
            //    inputTable.Columns.Add( obsYears[j].ToString(), typeof(double));
            //}
            this.observationTable = 
                SetNewObsTable(this.numRecruitPredictors, Array.ConvertAll(this.obsYears, element => element.ToString()) );
            //for (int i = 0; i < this.numRecruitPredictors; i++)
            //{
            //    line = sr.ReadLine();
            //    string[] observationsLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //    inputTable.Rows.Add(observationsLine);
            //}
            foreach (DataRow obsRow in this.observationTable.Rows)
            {
                line = sr.ReadLine();
                string[] observationsLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                obsRow.ItemArray = observationsLine;
            }
            //this.observationTable = inputTable;

        }

       
        public DataTable SetNewCoefficientTable(int numPredictors)
        {
            DataTable coefficientTable = new DataTable("Coefficients");
            coefficientTable.Columns.Add("Coefficient", typeof(double));
            for (int i = 0; i < numPredictors; i++)
            {
                coefficientTable.Rows.Add();
            }
            return coefficientTable;
        }
        public DataTable SetNewObsTable(int numPredictors, string[] obsYears)
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
    }
}
