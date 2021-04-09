using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;

namespace Nmfs.Agepro.CoreLib
{
  public class MarkovMatrixRecruitment : RecruitmentModelProperty
  {
    private int _NumRecruitLevels;
    private int _NumSSBLevels;
    
    public DataSet MarkovRecruitment { get; set; }

    public int NumRecruitLevels
    {
      get => _NumRecruitLevels;
      set => SetProperty(ref _NumRecruitLevels, value);
    }
    public int NumSSBLevels
    {
      get => _NumSSBLevels;
      set => SetProperty(ref _NumSSBLevels, value);
    }



    public MarkovMatrixRecruitment()
    {
      recruitModelNum = 1; //Model 1 only for MarkovMatrix
      recruitCategory = 4; //TODO: Check if MarkovMatrix Category is 4
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

        NumRecruitLevels = Convert.ToInt32(MarkovMatrixOptions[0]);
        NumSSBLevels = Convert.ToInt32(MarkovMatrixOptions[1]);


        //Recruitment
        line = sr.ReadLine();
        string[] inputTableLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        markov.Tables.Add(ReadRecruitmentDataTable(inputTableLine));

        //SSB
        line = sr.ReadLine();
        inputTableLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        markov.Tables.Add(ReadSSBLevelsDataTable(inputTableLine));

        //Probability
        using (DataTable inputTable = new DataTable("Probability"))
        {
          for (int j = 0; j < NumRecruitLevels; j++)
          {
            DataColumn RecruitLevel = inputTable.Columns.Add("PR(" + (j + 1).ToString() + ")", typeof(double));
          }
          for (int i = 0; i < NumSSBLevels; i++)
          {
            line = sr.ReadLine();
            inputTableLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            //Check Probability of each SSB Level (row) must sum to 1.0 
            SSBLevelProbSum = Array.ConvertAll(inputTableLine, double.Parse).Sum();
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
        }

        MarkovRecruitment = markov;
      }
    }



    /// <summary>
    /// Reads in Markov Matrix Recruitment's Recruitment Data Table 
    /// </summary>
    /// <param name="inputTableLine"></param>
    /// <returns></returns>
    private DataTable ReadRecruitmentDataTable(string[] inputTableLine)
    {
      DataTable inputTable = new DataTable("Recruitment");
      inputTable.Columns.Add("Recruitment", typeof(int));
      for (int i = 0; i < NumRecruitLevels; i++)
      {
        inputTable.Rows.Add(Convert.ToInt32(inputTableLine[i]));
      }

      return inputTable;
    }

    /// <summary>
    /// Reads in Markov Matrix Recuritment's SSB Levels Data Table 
    /// </summary>
    /// <param name="inputTableLine"></param>
    /// <returns></returns>
    private DataTable ReadSSBLevelsDataTable(string[] inputTableLine)
    {
      DataTable inputTable = new DataTable("SSB");
      inputTable.Columns.Add("SSB Cut Points", typeof(int));
      for (int i = 0; i < NumSSBLevels; i++)
      {
        inputTable.Rows.Add(Convert.ToInt32(inputTableLine[i]));
      }

      return inputTable;
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
          tableT.Columns.Add(colName + "(" + (j + 1).ToString() + ")", typeof(double));
        }
      }
      else
      {
        throw new InvalidAgeproParameterException("Markov " + tableName +
            " Table has invalid number of columns: " + numCols);
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

      string delimiter = new string(' ', 2);
      outputLines.Add(NumRecruitLevels + delimiter + NumSSBLevels);

      foreach (DataTable markovTable in MarkovRecruitment.Tables)
      {
        if (markovTable.TableName == "Probability")
        {
          foreach (DataRow ssbRow in markovTable.Rows)
          {
            outputLines.Add(string.Join(delimiter, ssbRow.ItemArray));
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
          outputLines.Add(string.Join(delimiter, markovParamCol));
        }
      }

      return outputLines;
    }

    /// <summary>
    /// Checks to see parameter is 0
    /// </summary>
    /// <param name="markovParam">Parameter Value</param>
    /// <returns></returns>
    private bool VerifyZeroLevels(int markovParam)
    {
      return markovParam == 0;
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
      if (VerifyZeroLevels(NumRecruitLevels))
      {
        errorMsgList.Add("Zero or missing number of recruitment levels found.");
        errorMsgList.Add("Recruitment table has 0 rows, " +
            "Probability table has 0 columns.");
      }
      if (VerifyZeroLevels(NumSSBLevels))
      {
        errorMsgList.Add("Zero or missing number of SSB levels found.");
        errorMsgList.Add("SSB table has 0 rows, Probability table has 0 rows.");
      }

      if (HasBlankOrNullCells(MarkovRecruitment.Tables["Recruitment"]))
      {
        errorMsgList.Add("Missing data in recruitment table.");
      }
      if (HasBlankOrNullCells(MarkovRecruitment.Tables["SSB"]))
      {
        errorMsgList.Add("Missing data in SSB Cut Points Table.");
      }
      if (HasBlankOrNullCells(MarkovRecruitment.Tables["Probability"]))
      {
        errorMsgList.Add("Missing data in Probability table.");
      }

      return errorMsgList.EnumerateValidationResults();
    }

  }
}
