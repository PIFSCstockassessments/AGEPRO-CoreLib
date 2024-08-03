using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nmfs.Agepro.CoreLib
{
  public enum AuxiliaryOutputFlag
  {
    OutfileNoStockExcludeStockAux = 0,
    OutfileAppendStockAllAux = 1,
    OnlyOutfileNoStock = 2,
    OnlyOutfileAppendStock = 3,
    OutfileAppendStockExcludeStockAux = 4
  }

  public interface IStockSummary
  {
    AuxiliaryOutputFlag SummaryAuxFileOutputFlag { get; }
  }
}
