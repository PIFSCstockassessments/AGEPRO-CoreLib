namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// A Specfic (custom) Percentile for the AGEPRO Output Report
  /// </summary>
  public class ReportPercentile : MiscOptionsParameter
  {
    private double _precentile;

    public double percentile
    {
      get { return _precentile; }
      set { SetProperty(ref _precentile, value); }
    }

    public ReportPercentile()
    {
      //Set Defaults to 0.0
      percentile = 0.0;
    }
  }

}
