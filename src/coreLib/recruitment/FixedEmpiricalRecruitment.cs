using System.Linq;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// Fixed Recruitment
  /// </summary>
  public class FixedEmpiricalRecruitment : EmpiricalRecruitment
  {
    public FixedEmpiricalRecruitment(int modelNum = 20)
        : base(modelNum)
    {
      SubType = EmpiricalType.Fixed;
    }

    /// <summary>
    /// Checks the values in the Observed Values DataTable Object are valid.
    /// </summary>
    /// <returns>Vaildation Result Object</returns>
    public override ValidationResult ValidateInput()
    {
      //Check that number of rows match umber of years minus year one
      if (ObsTable.Rows.Count != (obsYears.Count() - 1))
      {
        return new ValidationResult(false,
            "Number of recruitment rows does not equal to the number of years minus year one.");
      }

      return base.ValidateInput();
    }

  }
}