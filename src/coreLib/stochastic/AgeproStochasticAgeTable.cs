﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// Generalized Stochastic Weights-at-age AGEPRO parameter class.
  /// </summary>
  public class AgeproStochasticAgeTable
  {
    public bool? FromFile { get; set; }
    public bool TimeVarying { get; set; }
    public DataTable ByAgeData { get; set; }
    public DataTable ByAgeCV { get; set; }
    public string DataFile { get; set; }
    public int NumYears { get; set; }
    public int NumFleets { get; set; }

    public AgeproStochasticAgeTable()
    {
      FromFile = false;
    }
    public AgeproStochasticAgeTable(string file)
    {
      FromFile = true;
    }

    /// <summary>
    /// Read in AGEPRO Input Data File for stochastic Weights-at-age AGEPRO Parameters 
    /// </summary>
    /// <param name="sr">AGEPRO Input Data File StreamReader</param>
    /// <param name="numYears">Number of Years in Projection</param>
    /// <param name="numAges">Number of Ages in Projection</param>
    /// <param name="numFleets">Number of Fleets. Default to <c>1</c></param>
    public void ReadStochasticAgeData(StreamReader sr, int numYears, int numAges, int numFleets = 1)
    {
      if (sr is null)
      {
        throw new ArgumentNullException(nameof(sr));
      }

      NumYears = numYears;
      NumFleets = numFleets; //Defaults to 1 for Non fleet-dependent InputAge Data
      int N;

      //Input Age Option
      string line = sr.ReadLine();
      string[] swLine = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
      string optStock = swLine[0];
      SetStochasticAgeOption(optStock);  //Input Age Option may vary if this a weightAgeTable

      //Time Varying
      string optTimeVarying = swLine[1];
      TimeVarying = optTimeVarying.Equals("1");

      //check fromFile has non-null value. Do nothing if so.
      if (FromFile.HasValue)
      {
        //Create Ages Header
        string[] ageHeader = new string[numAges];
        for (int i = 0; i < numAges; i++)
        {
          ageHeader[i] = "Age " + (i + 1).ToString();
        }


        if ((bool)FromFile) 
        {
          //From File: Reads filepath
          ReadStochasticAgeTableFilename(sr);
        }
        else 
        {
          //Reads in Stochastic Data specified from AGERPO Input File Format
          //byAge
          DataTable ageTable = new DataTable("Age Value");
          N = TimeVarying ? NumYears * numFleets : numFleets;

          //Set Age Columns for ageTable
          foreach (var nage in ageHeader)
          {
            ageTable.Columns.Add(nage, typeof(double));
          }

          //TODO:Rowheader/add-'Fleet-Year Column' to DataTable??
          //i:N rows (years (and fleets))
          for (int i = 0; i < N; i++)
          {
            line = sr.ReadLine();
            _ = ageTable.NewRow();
            ageTable.Rows.Add(line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
          }
          ByAgeData = ageTable;

          //byCV
          DataTable cvTable = new DataTable("Age CV");

          //Set Age Columns for cvTable
          foreach (var nage in ageHeader)
          {
            cvTable.Columns.Add(nage, typeof(double));
          }

          for (int K = 0; K < numFleets; K++)
          {
            line = sr.ReadLine();
            DataRow dr = cvTable.NewRow();
            string[] cvLine = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < numAges; j++)
            {
              dr[j] = cvLine[j];
            }
            cvTable.Rows.Add(dr);
          }
          ByAgeCV = cvTable;
        }
      }

    }

    /// <summary>
    /// Sets option for stochastic weights-at-age to be inputted maunually or read from the AGEPRO Input File
    /// </summary>
    /// <param name="optParam">String Character with '0' or '1'</param>
    protected virtual void SetStochasticAgeOption(string optParam)
    {
      if (optParam.Equals("0"))
      {
        FromFile = false; //User Spec by Age
      }
      else if (optParam.Equals("1"))
      {
        FromFile = true; //From File   
      }

    }

    /// <summary>
    /// Reads in AGEPRO Input Filename for the 'Read Weights From File' option 
    /// </summary>
    /// <param name="sr">AGEPRO Input File StreamReader</param>
    protected virtual void ReadStochasticAgeTableFilename(StreamReader sr)
    {
      //Reads dataflie path
      DataFile = sr.ReadLine();
    }

    /// <summary>
    /// Creates data line strings to append to the Data Writer for this Stochastic Parameter.
    /// </summary>
    /// <param name="keyword">Stochasatic Parameter name. Should be written in 
    /// all caps, and enclosed in square parenthesis. Example: [PARAMETER] </param>
    /// <returns>Returns a list of strings. </returns>
    public virtual List<string> WriteStochasticAgeDataLines(string keyword)
    {

      List<string> outputLines = new List<string>
      {
        keyword //[PARAMETER]
      };

      if (FromFile == null)
      {
        throw new NullReferenceException("Stochastic Table Filepath is NULL");
      } 
      else if (FromFile == true)
      {
        outputLines.Add("1" + new string(' ', 2) + Convert.ToInt32(TimeVarying).ToString());
        outputLines.Add(DataFile);
      }
      else
      {

        if (ByAgeData is null)
        {
          throw new NullReferenceException("Stochastic Age of " + keyword + " is NULL.");
        }
        if (ByAgeCV is null)
        {
          throw new NullReferenceException("Stochastic CV of " + keyword + " is NULL.");
        }


        outputLines.Add("0" + new string(' ', 2) + Convert.ToInt32(TimeVarying).ToString());
        foreach (DataRow yearRow in ByAgeData.Rows)
        {
          outputLines.Add(string.Join(new string(' ', 2), yearRow.ItemArray));
        }
        foreach (DataRow cvRow in ByAgeCV.Rows)
        {
          outputLines.Add(string.Join(new string(' ', 2), cvRow.ItemArray));
        }
      }

      return outputLines;
    }

    /// <summary>
    /// Reads in a Stochastic Table Files to memory. 
    /// </summary>
    /// <param name="fn">Stochastic Table Filename</param>
    /// <param name="ncol">Number of Data Columns. Typically it is the number of Age 
    /// classes. For mulit-fleet cases, it is: Number of ages * Number of fleets.</param>
    /// <returns>Returns a DataTable object.</returns>
    public static DataTable ReadStochasticTableFile(string fn, int ncol)
    {
      if (string.IsNullOrWhiteSpace(fn))
      {
        throw new ArgumentException($"'{nameof(fn)}' cannot be null or whitespace.", nameof(fn));
      }

      DataTable fromFileAgeTable = new DataTable("Age From File");
      string line;

      //Setup Columns
      for (int i = 0; i < ncol; i++)
      {
        fromFileAgeTable.Columns.Add("Age " + (i + 1).ToString(), typeof(double));
      }

      try
      {
        using (StreamReader inReader = new StreamReader(fn))
        {
          while (!inReader.EndOfStream)
          {
            line = inReader.ReadLine();
            string[] lineRow = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (lineRow.Count() == ncol)
            {
              throw new IOException("Stochastic Table File does not equal this " +
                  "stochastic parameter's number of columns: " + ncol +
                  "(Counted in file : " + lineRow.Count() + ")");
            }
            DataRow dr = fromFileAgeTable.NewRow();
            dr.ItemArray = lineRow;
            fromFileAgeTable.Rows.Add(dr);

          }

        }

      }
      catch (IOException ex)
      {
        Console.WriteLine(ex.Message);
      }

      return fromFileAgeTable;
    }
  }
}
