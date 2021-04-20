using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// General Paramemeters of AGEPRO
  /// </summary>
  public class AgeproGeneral
  {
    public int ProjYearStart { get; set; }    //First Year in Projection
    public int ProjYearEnd { get; set; }     //Last Year in Projection
    public int AgeBegin { get; set; }        //First Age Class
    public int AgeEnd { get; set; }           //Last Age Class
    public int NumFleets { get; set; }       //Number of Fleets
    public int NumRecModels { get; set; }    //Number of Recruit Models
    public int NumPopSims { get; set; }      //Number of Population Simulations
    public bool HasDiscards { get; set; }     //Discards are Present
    public int Seed { get; set; }            //Random Number Seed
    public string InputFile { get; set; }

    public AgeproGeneral()
    {

    }

    public AgeproGeneral(string file)
    {
      InputFile = file; //readin file contents
    }

    /// <summary>
    /// Determine number of years in projection by the (absolulte) diffefence between the 
    /// last and first year of projection. 
    /// </summary>
    /// <returns>The difference stored in 'nYears'</returns>
    public int NumYears()
    {
      return Math.Abs(ProjYearEnd - ProjYearStart) + 1;
    }

    /// <summary>
    /// Determine number of ages in projection by the (absolulte) diffefence between last age 
    /// class and first age class of projection. 
    /// </summary>
    /// <returns>The difference in stored in 'nAges'</returns>
    public int NumAges()
    {
      return Math.Abs(AgeBegin - AgeEnd) + 1;
    }

    /// <summary>
    /// Returns a sequence of years from First year of projection
    /// </summary>
    /// <returns>Returns a int array from <paramref name="projYearStart"/> by <paramref name="NumYears"/></returns>
    public int[] SeqYears()
    {
      return Enumerable.Range(ProjYearStart, NumYears()).ToArray();
    }

    public string ReadGeneralModelParameters(StreamReader sr)
    {
      if (sr is null)
      {
        throw new ArgumentNullException(nameof(sr));
      }

      string line = sr.ReadLine();
      string[] generalLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      ProjYearStart = Convert.ToInt32(generalLine[0]);
      ProjYearEnd = Convert.ToInt32(generalLine[1]);
      AgeBegin = Convert.ToInt32(generalLine[2]);
      AgeEnd = Convert.ToInt32(generalLine[3]);
      NumPopSims = Convert.ToInt32(generalLine[4]);
      NumFleets = Convert.ToInt32(generalLine[5]);
      NumRecModels = Convert.ToInt32(generalLine[6]);
      Seed = Convert.ToInt32(generalLine[8]);
      HasDiscards = generalLine[7].Equals("1");

      return line;
    }

    public List<string> WriteAgeproGeneralParameters()
    {

      return new List<string>
      {
        "[GENERAL]",
        ProjYearStart.ToString() + "  " +
          ProjYearEnd.ToString() + "  " +
          AgeBegin.ToString() + "  " +
          AgeEnd.ToString() + "  " +
          NumPopSims.ToString() + "  " +
          NumFleets.ToString() + "  " +
          NumRecModels.ToString() + "  " +
          Convert.ToInt32(HasDiscards).ToString() + "  " +
          Seed.ToString()
      };
    }
  }
}
