using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace AGEPRO.CoreLib
{
    public class MarkovMatrixRecruitment : RecruitmentModel
    {
        public int numRecruitLevels { get; set; }
        public int numSSBLevels { get; set; }
        public DataSet markovRecruitment { get; set; }
        
        public MarkovMatrixRecruitment()
        {
            this.recruitModelNum = 1; //Model 1 only for MarkovMatrix
            this.recruitCategory = 4; //TODO: Check if MarkovMatrix Category is 4
        }

        public override void ReadRecruitmentModel(StreamReader sr)
        {
            using (DataSet markov = new DataSet("markovRecruitmentTables"))
            {

                string line;
                double SSBLevelProbSum;
                double precisionDiff;

                line = sr.ReadLine();
                string[] MarkovMatrixOptions = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                this.numRecruitLevels = Convert.ToInt32(MarkovMatrixOptions[0]);
                this.numSSBLevels = Convert.ToInt32(MarkovMatrixOptions[1]);

                //Recruitment
                DataTable inputTable = new DataTable("Recruitment");
                line = sr.ReadLine();
                string[] inputTableLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                inputTable.Columns.Add("Recruitment", typeof(int));
                for (int i = 0; i < this.numRecruitLevels; i++)
                {
                    inputTable.Rows.Add(Convert.ToInt32(inputTableLine[i]));
                }
                markov.Tables.Add(inputTable);

                //SSB
                inputTable = new DataTable("SSB");
                line = sr.ReadLine();
                inputTableLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                inputTable.Columns.Add("SSB Cut Points", typeof(int));
                for (int i = 0; i < this.numSSBLevels; i++)
                {
                    inputTable.Rows.Add(Convert.ToInt32(inputTableLine[i]));
                }
                markov.Tables.Add(inputTable);

                //Probalility
                inputTable = new DataTable("Probalitity");
                for (int j = 0; j < this.numRecruitLevels; j++)
                {
                    inputTable.Columns.Add("PR(" + (j + 1).ToString() + ")", typeof(double));
                }
                for (int i = 0; i < this.numSSBLevels; i++)
                {
                    line = sr.ReadLine();
                    inputTableLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    //Check Probability of each SSB Level (row) must sum to 1.0 
                    SSBLevelProbSum = Array.ConvertAll<string, double>(inputTableLine, double.Parse).Sum();
                    precisionDiff = Math.Abs(SSBLevelProbSum * .000001);
                    //To Handle Floating-Point Precision issues in "SSBLevelProbSum != 1" Comparision
                    if (!(Math.Abs(SSBLevelProbSum - (double)1.0) <= precisionDiff))
                    {
                        throw new InvalidOperationException("SSB level " + (i + 1).ToString() + " probability sum does not equal to 1.0: " +
                            "Probability sum is " + SSBLevelProbSum.ToString());
                    }
                    inputTable.Rows.Add(inputTableLine);
                }
                markov.Tables.Add(inputTable);

                this.markovRecruitment = markov;
            }
        }

        
    }
}
