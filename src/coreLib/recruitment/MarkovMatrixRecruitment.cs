﻿using System;
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

        /// <summary>
        /// Reads in AGEPRO Input File Stream for Markov Matrix Recruitment parameters & data.
        /// </summary>
        /// <param name="sr">AGEPRO Input File StreamReader</param>
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

        /// <summary>
        /// Creates a Recruitment Levels table based on the Number of Levels 
        /// </summary>
        /// <param name="numLevels">Numberof Recruitment Levels</param>
        /// <returns>Recruitment DataTable</returns>
        public DataTable NewRecruitLevelTable(int numLevels = 1)
        {
            return NewMarkovLevelTable("Recruitment", numLevels, "Recruitment");
        }
        /// <summary>
        /// Creates a SSB Cut Point table based on the Number of Levels. 
        /// </summary>
        /// <param name="numLevels">Number of Levels</param>
        /// <returns>SSB Cutpoints DataTable</returns>
        public DataTable NewSSBLevelTable(int numLevels)
        {
            return NewMarkovLevelTable("SSB", numLevels, "SSB Cut Points");
        }
        /// <summary>
        /// Creates a Proabablity table based on Spawning Stock Biomass 
        /// </summary>
        /// <param name="lvlSSB">Spawning Stock Biomass levels</param>
        /// <param name="lvlRecruits">Recruitment levels</param>
        /// <returns>Probability DataTable</returns>
        public DataTable NewProbabilityTable(int lvlSSB, int lvlRecruits = 1)
        {
            return NewMarkovLevelTable("Probability", lvlSSB, "PR", lvlRecruits);
        }

        /// <summary>
        /// Creates a new Markov Matrix DataTable 
        /// </summary>
        /// <param name="tableName">DataTable Name</param>
        /// <param name="numLevels">Number of Level rows</param>
        /// <param name="colName">Column Name(s)</param>
        /// <param name="numCols">Number of Columns. Default is 1.</param>
        /// <returns>A DataTable determined in numLevels by numCol. </returns>
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


        /// <summary>
        /// Translates Markov Matrix Recruitment input data and parameters into the
        /// AGEPRO input file data format.
        /// </summary>
        /// <returns>List of strings. Each string repesents a line from the input file.</returns>
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

        /// <summary>
        /// Checks to see parameter is 0
        /// </summary>
        /// <param name="markovParam">Parameter Value</param>
        /// <returns></returns>
        private bool ValidMarkovParameter(int markovParam)
        {
            List<string> errorMsgList = new List<string>();
            if (markovParam == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Markov Matrix Recuitment Validation
        /// </summary>
        /// <returns>
        /// If all validation checks have been met, nothing will be returned.
        /// All validations not met will be recorded to a list of "Error Messages" to return.
        /// </returns>
        public override ValidationResult ValidateInput()
        {
            List<string> errorMsgList = new List<string>();
            if (this.ValidMarkovParameter(this.numRecruitLevels) == false)
            {
                errorMsgList.Add("Zero or missing number of recruitment levels found.");
                errorMsgList.Add("Recruitment table has 0 rows, "+
                    "Probability table has 0 columns.");
            }
            if (this.ValidMarkovParameter(this.numSSBLevels) == false)
            {
                errorMsgList.Add("Zero or missing number of SSB levels found.");
                errorMsgList.Add("SSB table has 0 rows, Probability table has 0 rows.");
            }

            if (this.HasBlankOrNullCells(this.markovRecruitment.Tables["Recruitment"]))
            {
                errorMsgList.Add("Missing data in recruitment table.");
            }
            if (this.HasBlankOrNullCells(this.markovRecruitment.Tables["SSB"]))
            {
                errorMsgList.Add("Missing data in SSB Cut Points Table.");
            }
            if (this.HasBlankOrNullCells(this.markovRecruitment.Tables["Probability"]))
            {
                errorMsgList.Add("Missing data in Probability table.");
            }

            var results = errorMsgList.EnumerateValidationResults();
            return results;
        }

    }
}