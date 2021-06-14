using System;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// AGEPRO Bootstrapping Options
  /// </summary>
  /// <remarks>
  /// -Number of data values of each row must equal to the number of age classes.
  /// -The number of rows in a bootstrap file must be at least equal to the number of bootstrap 
  /// iterations containing the popluation of the first year in the projection
  /// </remarks>
  public class AgeproBootstrap : AgeproCoreLibProperty
  {
    private int _NumBootstraps;
    private double _PopScaleFactor;
    private string _BootstrapFile;

    public int NumBootstraps
    {
      get => _NumBootstraps;
      set => SetProperty(ref _NumBootstraps, value);
    }
    public double PopScaleFactor
    {
      get => _PopScaleFactor;
      set => SetProperty(ref _PopScaleFactor, value);
    }
    public string BootstrapFile
    {
      get => _BootstrapFile;
      set => SetProperty(ref _BootstrapFile, value);
    }

    public AgeproBootstrap()
    {
      NumBootstraps = 0;
      PopScaleFactor = 0;
    }

    /// <summary>
    /// Reading Bootstrap Options from AGEPRO Input File Stream
    /// </summary>
    /// <param name="sr">AGEPRO Input File StreamReader</param>
    public void ReadBootstrapData(StreamReader sr)
    {
      //Send error/warning if bootstrapFile is not found in system
      if (sr is null)
      {
        throw new ArgumentNullException(nameof(sr));
      }

      string line;

      line = sr.ReadLine();
      string[] bootstrapOptions = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      //Number of Bootstraps
      NumBootstraps = Convert.ToInt32(bootstrapOptions[0]);
      //Pop. Scale Factor
      PopScaleFactor = Convert.ToDouble(bootstrapOptions[1]);
      //Bootstrap File
      line = sr.ReadLine();
      BootstrapFile = line;

    }
  }
}
