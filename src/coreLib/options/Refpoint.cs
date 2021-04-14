namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// Parameters for the Reference Point Threshold Report
  /// </summary>
  public class Refpoint : MiscOptionsParameter
  {
    private double _refSpawnBio; //SSBThresh
    private double _refJan1Bio;  //StockBioThresh
    private double _refMeanBio;  //MeanBioThresh
    private double _refFMort;    //FMortThresh

    //SSBThresh
    public double refSpawnBio
    {
      get { return _refSpawnBio; }
      set { SetProperty(ref _refSpawnBio, value); }
    }
    //StockBioThresh
    public double refJan1Bio
    {
      get { return _refJan1Bio; }
      set { SetProperty(ref _refJan1Bio, value); }
    }
    //MeanBioThresh
    public double refMeanBio
    {
      get { return _refMeanBio; }
      set { SetProperty(ref _refMeanBio, value); }
    }
    //FMortThresh
    public double refFMort
    {
      get { return _refFMort; }
      set { SetProperty(ref _refFMort, value); }
    }

    public Refpoint()
    {
      //Set Defaults to 0.0
      refSpawnBio = 0.0;
      refJan1Bio = 0.0;
      refMeanBio = 0.0;
      refFMort = 0.0;
    }

  }

}
