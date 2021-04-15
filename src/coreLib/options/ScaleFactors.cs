namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// Scaling Factors for the AGEPRO Output Report
  /// </summary>
  public class ScaleFactors : AgeproOptionsProperty
  {
    private double _ScaleBio;
    private double _ScaleRec;
    private double _ScaleStockSum;

    public double ScaleBio
    {
      get => _ScaleBio;
      set => SetProperty(ref _ScaleBio, value);
    }
    public double ScaleRec
    {
      get => _ScaleRec;
      set => SetProperty(ref _ScaleRec, value);
    }
    public double ScaleStockNum
    {
      get => _ScaleStockSum;
      set => SetProperty(ref _ScaleStockSum, value);
    }

    public ScaleFactors()
    {
      //Set Defaults to 0
      ScaleBio = 0;
      ScaleRec = 0;
      ScaleStockNum = 0;
    }
  }

}
