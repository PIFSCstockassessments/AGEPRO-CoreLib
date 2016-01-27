using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace AGEPRO_struct
{
    public class PredictorRecruitment : RecruitmentModel
    {
        public int numRecruitPredictors { get; set; }
        public double variance { get; set; }
        public double intercept { get; set; }
        public DataTable coefficientTable { get; set; }
        public DataTable observationTable { get; set; }
        public int[] obsYears { get; set; }
        

        public PredictorRecruitment(int modelNum) 
        {
            this.recruitModelNum = modelNum;
            this.recruitCategory = 3;
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

            //TODO:Check numRecruitPredictors <= 5 (max is 5)

            //Coefficents
            DataTable inputTable = new DataTable();
            inputTable.Columns.Add("Coefficient", typeof(double));

            line = sr.ReadLine();
            string[] predictorCoefficents = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < this.numRecruitPredictors; i++)
            {
                inputTable.Rows.Add(Convert.ToDouble(predictorCoefficents[i]));
            }
            this.coefficientTable = inputTable;

            //Observations
            inputTable = new DataTable();
            for (int j = 0; j < obsYears.Count(); j++)
            {
                inputTable.Columns.Add( obsYears[j].ToString(), typeof(double));
            }
            for (int i = 0; i < this.numRecruitPredictors; i++)
            {
                line = sr.ReadLine();
                string[] observationsLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                inputTable.Rows.Add(observationsLine);

            }
            this.observationTable = inputTable;

        }

        
    }
}
