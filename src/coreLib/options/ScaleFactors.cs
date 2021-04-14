namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// Scaling Factors for the AGEPRO Output Report
  /// </summary>
  public class ScaleFactors : AgeproOptionsProperty
  {
    private double _scaleBio;
    private double _scaleRec;
    private double _scaleStockSum;

    public double scaleBio
    {
      get { return _scaleBio; }
      set { SetProperty(ref _scaleBio, value); }
    }
    public double scaleRec
    {
      get { return _scaleRec; }
      set { SetProperty(ref _scaleRec, value); }
    }
    public double scaleStockNum
    {
      get { return _scaleStockSum; }
      set { SetProperty(ref _scaleStockSum, value); }
    }

    public ScaleFactors()
    {
      //Set Defaults to 0
      scaleBio = 0;
      scaleRec = 0;
      scaleStockNum = 0;
    }
  }

}
