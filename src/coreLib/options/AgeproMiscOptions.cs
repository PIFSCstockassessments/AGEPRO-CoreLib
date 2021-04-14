using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// AGEPRO Misc Options.
  /// </summary>
  public class AgeproMiscOptions
  {
    public bool enableSummaryReport { get; set; }
    public bool enableAuxStochasticFiles { get; set; }
    public bool enableExportR { get; set; }
    //enabled if classes are called.
    public bool enableRefpoint { get; set; }
    public bool enablePercentileReport { get; set; }
    public bool enableScaleFactors { get; set; }
    public bool enableBounds { get; set; }
    public bool enableRetroAdjustmentFactors { get; set; }

    public AgeproMiscOptions()
    {
      enableRefpoint = false;
      enablePercentileReport = false;
      enableScaleFactors = false;
      enableBounds = false;
      enableRetroAdjustmentFactors = false;
    }

  }


}
