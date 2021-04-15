namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// A Specfic (custom) Percentile for the AGEPRO Output Report
  /// </summary>
  public class ReportPercentile : AgeproOptionsProperty
  {
    private double _Precentile;

    public double Percentile
    {
      get => _Precentile;
      set => SetProperty(ref _Precentile, value);
    }

    public ReportPercentile()
    {
      //Set Defaults to 0.0
      Percentile = 0.0;
    }
  }

}
