using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace Nmfs.Agepro.CoreLib
{
    public class MarkovMatrixRecruitment : RecruitmentModel
    {
        private int _numRecruitLevels;
        private int _numSSBLevels;
        
        public int numRecruitLevels 
        {
            get { return _numRecruitLevels; }
            set { SetProperty(ref _numRecruitLevels, value);}
        }
        public int numSSBLevels 
        {
            get { return _numSSBLevels; }
            set { SetProperty(ref _numSSBLevels, value); }
        }
        
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

                //Probability
                inputTable = new DataTable("Probability");
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

        private DataTable NewMarkovLevelTable(string tableName, int numLevels, string colName, int numCols = 1)
        {
            DataTable tableT = new DataTable(tableName);
            if (numCols == 1)
            {
                tableT.Columns.Add(colName, typeof(int));    
            }
            else if (numCols > 1)
            {   
                //Assumming 'Probabiliy' is the only multi-column Markov Matrix level Datatable
                for (int j = 0; j < numCols; j++)
                {
                    tableT.Columns.Add(colName+"("+ (j+1).ToString()+")", typeof(double));
                }
            }
            else
            {
                throw new InvalidAgeproParameterException("Markov "+ tableName +
                    " Table has invalid number of columns: "+ numCols);
            }

            for (int i = 0; i < numLevels; i++)
            {
                tableT.Rows.Add();
            }
            return tableT;
        }
        public DataTable NewRecruitLevelTable(int numLevels = 1)
        {
            return NewMarkovLevelTable("Recruitment", numLevels, "Recruitment");
        }
        public DataTable NewSSBLevelTable(int numLevels)
        {
            return NewMarkovLevelTable("SSB", numLevels, "SSB Cut Points");
        }
        public DataTable NewProbabilityTable(int lvlSSB, int lvlRecruits = 1)
        {
            return NewMarkovLevelTable("Probability", lvlSSB, "PR", lvlRecruits);
        }

        public override List<string> WriteRecruitmentDataModelData()
        {
            List<string> outputLines = new List<string>();

            outputLines.Add(this.numRecruitLevels + new string(' ', 2) + this.numSSBLevels);
            foreach (DataTable markovTable in markovRecruitment.Tables)
            {
                if (markovTable.TableName == "Probability")
                {
                    foreach (DataRow ssbRow in markovTable.Rows)
                    {
                        outputLines.Add(string.Join(new string(' ', 2), ssbRow.ItemArray));
                    }
                }
                else
                {
                    if (markovTable.Columns.Count > 1)
                    {
                        throw new InvalidAgeproParameterException("Non-Probability Table has more than one column");
                    }
                    List<string> markovParamCol = new List<string>();
                    foreach (DataRow dtRow in markovTable.Rows)
                    {
                        markovParamCol.Add(dtRow[0].ToString());
                    }
                    outputLines.Add(string.Join(new string(' ', 2), markovParamCol));
                }
            }

            return outputLines;
        }

    }
}
