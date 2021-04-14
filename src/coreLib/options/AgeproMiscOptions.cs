namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// AGEPRO Misc Options.
  /// </summary>
  public class AgeproMiscOptions
  {
    public bool EnableSummaryReport { get; set; }
    public bool EnableAuxStochasticFiles { get; set; }
    public bool EnableExportR { get; set; }
    //enabled if classes are called.
    public bool EnableRefpoint { get; set; }
    public bool EnablePercentileReport { get; set; }
    public bool EnableScaleFactors { get; set; }
    public bool EnableBounds { get; set; }
    public bool EnableRetroAdjustmentFactors { get; set; }

    public AgeproMiscOptions()
    {
      EnableRefpoint = false;
      EnablePercentileReport = false;
      EnableScaleFactors = false;
      EnableBounds = false;
      EnableRetroAdjustmentFactors = false;
    }

  }


}
