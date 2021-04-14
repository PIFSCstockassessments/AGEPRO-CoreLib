using System.Data;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// At-age vaiues to applied to initial population numbers to correct for retrospective bias.
  /// </summary>
  public class RetroAdjustmentFactors : MiscOptionsParameter
  {
    //Data binded to AGEPRO GUI's dataGridRetroAdjustment (DataGridView)
    public DataTable retroAdjust;

    public RetroAdjustmentFactors()
    {
      retroAdjust = new DataTable("Retro Adjustment Factors");
    }
  }

}
