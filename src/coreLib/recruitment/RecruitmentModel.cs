using System;
using System.Collections.Generic;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// Generalized, abstract representation of AGEPRO Recruitment Model property
  /// </summary>
  abstract public class RecruitmentModel : AgeproCoreLibProperty, IValidatable
  {
    public int recruitModelNum;
    public int recruitCategory;
    public int[] obsYears;
    public abstract void ReadRecruitmentModel(StreamReader sr);
    public abstract List<string> WriteRecruitmentDataModelData();
    public abstract ValidationResult ValidateInput();

    /// <summary>
    /// Checks if this DataTable has any cell that may be blank, null, or with only white-space 
    /// characters.
    /// </summary>
    /// <returns>If the function finds an single instance of a blank/null/white-space only cell,
    /// then it will return true. Otherwise, false.</returns>
    protected virtual bool HasBlankOrNullCells(System.Data.DataTable table)
    {
      bool blankNullsExist = false;

      //Return true if there is no table, or 0 row or columns.
      if (table == null)
      {
        return true;
      }

      foreach (System.Data.DataRow dtRow in table.Rows)
      {
        foreach (var item in dtRow.ItemArray)
        {
          if (string.IsNullOrWhiteSpace(item.ToString()))
          {
            blankNullsExist = true;
          }
        }
      }

      return blankNullsExist;
    }

    /// <summary>
    /// Verifies if all DataTable values are siginificantly above the lower bound.  
    /// </summary>
    /// <param name="dtable">Data table to compare.</param>
    /// <param name="lowBound">The lower bound that defines insigfiicance.</param>
    /// <returns>If the function finds any value that is smaller than the lower bound
    /// then it returns false.</returns>
    protected bool TableHasAllSignificantValues(System.Data.DataTable dtable, double lowBound = 0.0001)
    {
      foreach (System.Data.DataRow drow in dtable.Rows)
      {
        foreach (System.Data.DataColumn dcol in dtable.Columns)
        {
          if (Convert.ToDouble(drow[dcol]) < lowBound)
          {
            return false;
          }
        }
      }
      return true;
    }


  }
}
