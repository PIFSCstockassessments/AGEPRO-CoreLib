using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// General AGEPRO Recruitment Parameters. 
  /// </summary>
  public class AgeproRecruitment : AgeproCoreLibProperty
  {
    private double _RecruitScalingFactor;
    private double _SSBScalingFactor;

    public int MaxRecruitObs { get; set; }
    public int[] RecruitType { get; set; }
    public int RecruitmentCategory { get; set; }
    public List<RecruitmentModelProperty> RecruitCollection { get; set; }
    public int[] ObservationYears { get; set; }
    public DataTable RecruitProb { get; set; }

    public double RecruitScalingFactor
    {
      get => _RecruitScalingFactor;
      set => SetProperty(ref _RecruitScalingFactor, value);
    }
    public double SSBScalingFactor
    {
      get => _SSBScalingFactor;
      set => SetProperty(ref _SSBScalingFactor, value);
    }
    
    public AgeproRecruitment()
    {
      RecruitScalingFactor = 0;
      SSBScalingFactor = 0;
    }



    /// <summary>
    /// Sets up AgeoroRecruitment data based on user generated AGEPRO parameter new cases.
    /// </summary>
    /// <param name="nrecruits">Number of Recruits</param>
    /// <param name="seqYears">List of Projection Year names</param>
    public void NewCaseRecruitment(int nrecruits, string[] seqYears)
    {
      if (seqYears is null)
      {
        throw new ArgumentNullException(nameof(seqYears));
      }

      MaxRecruitObs = 500;
      ObservationYears = Array.ConvertAll(seqYears, syr => int.TryParse(syr, out int x) ? x : 0);

      //NullSelectRecuitment is default for NewCases.
      RecruitCollection = new List<RecruitmentModelProperty>();
      RecruitType = new int[nrecruits];
      for (int irecruit = 0; irecruit < nrecruits; irecruit++)
      {
        RecruitCollection.Add(GetRecruitmentModel(0));
        RecruitType[irecruit] = RecruitCollection[irecruit].recruitModelNum; //0
        RecruitCollection[irecruit].obsYears = ObservationYears;
      }

      //Set Recruitment Probabilty Values
      List<string[]> recruitProbYear = new List<string[]>();
      for (int iyear = 0; iyear < seqYears.Count(); iyear++)
      {
        string[] yrRecruitProb = new string[nrecruits];
        for (int jrecruit = 0; jrecruit < nrecruits; jrecruit++)
        {
          double newCaseProb = Math.Round(1.0 / nrecruits);
          yrRecruitProb[jrecruit] = newCaseProb.ToString("0.000");
        }
        recruitProbYear.Add(yrRecruitProb);
      }

      CreateRecruitmentProbabilityTable(recruitProbYear);


    }

    /// <summary>
    /// Reads in AGEPRO Input File for Recruitment Model Data
    /// </summary>
    /// <param name="sr">AGEPRO Input File StreamReader</param>
    /// <param name="nyears">Number of Years in projection</param>
    /// <param name="numRecruitModels">Number of Recruitment models</param>
    public void ReadRecruitmentData(StreamReader sr, int nyears, int numRecruitModels)
    {
      if (sr is null)
      {
        throw new ArgumentNullException(nameof(sr));
      }

      string line;

      Console.WriteLine("Reading Recuitment Data ... ");

      line = sr.ReadLine();
      string[] recruitOpt = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      RecruitScalingFactor = Convert.ToInt32(recruitOpt[0]);  //Recruitment Scaling Factor
      SSBScalingFactor = Convert.ToInt32(recruitOpt[1]);      //SSB Scaling Factor
      MaxRecruitObs = Convert.ToInt32(recruitOpt[2]);

      //Recruit Methods
      line = sr.ReadLine().Trim();
      string[] recruitModels = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      //Keep Recruit Type a int array for switch-case
      RecruitType = Array.ConvertAll(recruitModels, smodel => int.TryParse(smodel, out int x) ? x : 0);

      //Check numRecruitModels matches actual count
      if (RecruitType.Count() != numRecruitModels)
      {
        throw new InvalidAgeproParameterException("numRecruitModels does not match input file recruitModel count");
      }
      
      //Recruitment Probability
      List<string[]> recrProbYear = new List<string[]>();
      for (int i = 0; i < nyears; i++)
      {
        line = sr.ReadLine();
        recrProbYear.Add(line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
      }
      CreateRecruitmentProbabilityTable(recrProbYear); //Set Recruitment Probabilty Values

      //Recruitment type
      //Set new RecruitCollection of Recruitment Models 
      RecruitCollection = new List<RecruitmentModelProperty>(); 
      for (int i = 0; i < numRecruitModels; i++)
      {
        RecruitCollection.Add(GetRecruitmentModel(RecruitType[i]));
        RecruitCollection[i].obsYears = ObservationYears;   //Set observation years
      }

      //Check for multiple Markov Matrix Recuitments. (Only one is allowed)
      if (RecruitCollection.Count(r => r.recruitModelNum == 1) > 1)
      {
        throw new ArgumentException("Multiple Markov Matrix Recruitment models found. " +
          "Please use a single Markov Matrix Recruitment model.");
      }

      Console.WriteLine("Reading Recuitment Model Data ... ");

      //Read Recruitment Data
      foreach (var nrecruit in RecruitCollection)
      {
        nrecruit.ReadRecruitmentModel(sr);
      }

      Console.WriteLine("Done.");
    }


    /// <summary>
    /// Creates/Initalizes the Recruitment Proabablity Data Table
    /// </summary>
    /// <param name="listRecruitProbYr">List containing Recruitment Distribtions per Observaion Year</param>
    public void CreateRecruitmentProbabilityTable(List<string[]> listRecruitProbYr)
    {

      //Clean off potentially existing data 
      if (RecruitProb != null)
      {
        RecruitProb.Clear();
      }
      //Setup Recruitmet Probabilty Table
      RecruitProb = new DataTable
      {
        TableName = "Recruitment Probability"
      };


      //Set Recruit Prob Columns
      for (int nselection = 0; nselection < RecruitType.Count(); nselection++)
      {
        string recruitProbColumnName = "Selection " + (nselection + 1).ToString();

        if (!RecruitProb.Columns.Contains(recruitProbColumnName))
        {
          RecruitProb.Columns.Add(recruitProbColumnName, typeof(double));
        }
      }
      //If current Recruitment Probability table has more columns than actual count, trim it
      if (RecruitProb.Columns.Count > RecruitType.Count())
      {
        for (int index = RecruitProb.Columns.Count - 1; index > 0; index--)
        {
          RecruitProb.Columns.RemoveAt(index);
        }
      }

      for (int irow = 0; irow < ObservationYears.Count(); irow++)
      {
        try
        {
          //Check Recruitment Probability for all selections of each year sums to 1.0
          CheckRecruitProbabilitySum(listRecruitProbYr[irow]);
        }
        catch (Exception ex)
        {
          throw new InvalidAgeproParameterException("At row " + (irow + 1).ToString() +
              " of recruitment probablity:" + Environment.NewLine + ex.InnerException.Message
              , ex);
        }

        RecruitProb.Rows.Add(listRecruitProbYr[irow]);

      }

    }

    /// <summary>
    /// Checks the Selected Recruitment Probabilitiy row if it sums up to 1.0.  
    /// </summary>
    /// <param name="recruitProbRow">String array representing the row of the Recruitment 
    /// probability data grid.</param>
    /// <returns>Returns false if the row does not sum up to 1.0. Otherwise, true.</returns>
    public static bool CheckRecruitProbabilitySum(String[] recruitProbRow)
    {
      if (recruitProbRow is null)
      {
        throw new ArgumentNullException(nameof(recruitProbRow));
      }

      double precisionDiff;
      double rowSumRecruitProb;

      //Check Recruitment Probability for all selections of each year sums to 1.0
      rowSumRecruitProb = Array.ConvertAll(recruitProbRow, s => double.TryParse(s, out double x) ? x : 0).Sum();
      precisionDiff = Math.Abs(rowSumRecruitProb * 0.00001);
      //Handle Floating-Point precision issues when "sumRowRecruitProb != 1.0" comparisons
      if (!(Math.Abs(rowSumRecruitProb - 1) <= precisionDiff))
      {
        Console.WriteLine(
            "Recruitment probablity sum does not equal 1.0: Probability sum is " +
            rowSumRecruitProb.ToString());
        return false;
      }

      return true;
    }

    /// <summary>
    /// Returns a new recruitment model, based on type (model number). 
    /// </summary>
    /// <param name="rtype">Recruitment Model Number</param>
    public static RecruitmentModelProperty GetRecruitmentModel(int rtype)
    {
      switch (rtype)
      {
        case 1:
          return new MarkovMatrixRecruitment();
        case 2:
          return new EmpiricalRecruitment(rtype, useSSB: true, subType: EmpiricalType.Empirical);
        case 3:
        case 14:
          return new EmpiricalRecruitment(rtype, useSSB: false, subType: EmpiricalType.Empirical);
        case 20:
          return new FixedEmpiricalRecruitment(rtype);
        case 4:
          return new TwoStageEmpiricalRecruitment(rtype, useSSB: true);
        case 5:
        case 6:
          return new ParametricCurve(rtype, isAutocorrelated: false);
        case 7:
          return new ParametricShepherdCurve(rtype, isAutocorrelated: false);
        case 8:
          return new ParametricLognormal(rtype, isAutocorrelated: false);
        case 10:
        case 11:
          return new ParametricCurve(rtype, isAutocorrelated: true);
        case 12:
          return new ParametricShepherdCurve(rtype, isAutocorrelated: true);
        case 13:
          return new ParametricLognormal(rtype, isAutocorrelated: true);
        case 15:
          return new TwoStageEmpiricalRecruitment(rtype, useSSB: false);
        case 16:
        case 17:
        case 18:
        case 19:
          return new PredictorRecruitment(rtype);
        case 21:
          return new EmpiricalCDFZero(rtype);
        case 0:
          return new NullSelectRecruitment();
        default:
          throw new InvalidAgeproParameterException("Invalid Recruitment Model Number: " + rtype);
      }//end switch

    }//end GetNewRecruitModel

    /// <summary>
    /// Stores AGEPRO Recruitment data under the AGEPRO Input Data Recruitment format
    /// </summary>
    /// <returns>String list formmatted under the AGEPRO Input Data Recruitment specification.</returns>
    public List<string> WriteRecruitmentDataLines()
    {
      string delimiter = new string(' ', 2);
      List<string> outputLines = new List<string>
      {
        "[RECRUIT]",
        RecruitScalingFactor.ToString() + delimiter + SSBScalingFactor.ToString() + delimiter + MaxRecruitObs.ToString()
      };

      //Write Recruit Model Number(s) used for projection
      List<string> modelNumArrayFromRecruitCollection = new List<string>();
      foreach (RecruitmentModelProperty recruit in RecruitCollection)
      {
        modelNumArrayFromRecruitCollection.Add(recruit.recruitModelNum.ToString());
      }
      outputLines.Add(string.Join(delimiter, modelNumArrayFromRecruitCollection));
      
      //Write Recruit Probability
      foreach (DataRow yearRow in RecruitProb.Rows)
      {
        outputLines.Add(string.Join(delimiter, yearRow.ItemArray));
      }

      //Write Recruit Model(s) Data
      foreach (RecruitmentModelProperty recruitModel in RecruitCollection)
      {
        outputLines.AddRange(recruitModel.WriteRecruitmentDataModelData());
      }


      return outputLines;
    }

    /// <summary>
    /// Creates a Recruitment Probability DataTable.
    /// </summary>
    /// <param name="yCol">Number of columns</param>
    /// <param name="xRows">Number of Rows</param>
    /// <param name="colName">Column Names</param>
    /// <returns></returns>
    public static DataTable CreateRecruitProbTable(int yCol, int xRows, string colName)
    {
      DataTable recruitProbTable = new DataTable();

      for (int icol = 0; icol < yCol; icol++)
      {
        recruitProbTable.Columns.Add(colName + " " + (icol + 1));
      }
      for (int row = 0; row < xRows; row++)
      {
        recruitProbTable.Rows.Add();
      }

      //Fill Recruit Probability table with default set of values.
      //Assume each new case recruit selection prob is spread evenly.
      double recruitProbVal = 1 / Convert.ToDouble(yCol);

      for (int irow = 0; irow < xRows; irow++)
      {
        for (int jcol = 0; jcol < yCol; jcol++)
        {
          if (recruitProbTable.Rows[irow][jcol] == DBNull.Value)
          {
            recruitProbTable.Rows[irow][jcol] = recruitProbVal;
          }
        }
      }

      return recruitProbTable;
    }
  }
}
