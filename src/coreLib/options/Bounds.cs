namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// Customized maximum bounds for Weight and Natural Mortality   
  /// </summary>
  public class Bounds : MiscOptionsParameter
  {
    private double _maxWeight;
    private double _maxNatMort;

    public double maxWeight
    {
      get { return _maxWeight; }
      set { SetProperty(ref _maxWeight, value); }
    }
    public double maxNatMort
    {
      get { return _maxNatMort; }
      set { SetProperty(ref _maxNatMort, value); }
    }

    public Bounds()
    {
      //Set defaults
      maxWeight = 10.0;
      maxNatMort = 1.0;
    }
  }

}
