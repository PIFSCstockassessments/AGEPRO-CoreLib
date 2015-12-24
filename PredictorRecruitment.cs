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
        public int variance { get; set; }
        public int intercept { get; set; }
        public DataTable coefficientTable { get; set; }
        public DataTable observationTable { get; set; }

        protected override void ReadRecruitmentModel(StreamReader sr, int nyears)
        {
            //16, 17, 18, 19
            string line;

            line = sr.ReadLine();
            string[] predictorParamLine = line.Split(' ');
            this.numRecruitPredictors = Convert.ToInt32(predictorParamLine[0]);
            this.variance = Convert.ToInt32(predictorParamLine[1]);
            this.intercept = Convert.ToInt32(predictorParamLine[2]);

            //Coefficents
            DataTable inputTable = new DataTable();
            line = sr.ReadLine();
            string[] predictorCoefficents = line.Split(' ');
            for (int i = 1; i < this.numRecruitPredictors; i++)
            {
                //TODO? How to handle multiple recruits? Or oo class is sufficencent?
                inputTable.Rows.Add(Convert.ToDouble(predictorCoefficents[i]));
            }
            this.coefficientTable = inputTable;

            //Observations
            inputTable = new DataTable();
            for (int i = 0; i < this.numRecruitPredictors; i++)
            {
                line = sr.ReadLine();
                string[] observationsLine = line.Split(' ');
                for (int j = 0; j < nyears; j++)
                {
                    inputTable.Rows[i][j] = Convert.ToDouble(observationsLine[i]);
                }


            }


        }

        
    }
}
