using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nmfs.Agepro.CoreLib
{
  public enum StockSummaryFlag
  {
    NoStockDistAllAux = 0,
    StockDistAllAux = 1,
    StockDistNoAux = 2,
    StockDistExceptAuxStockVector = 3
  }

  public interface IStockSummary
  {
    StockSummaryFlag SummaryAuxFileOutputFlag { get; }
  }
}
