using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nmfs.Agepro.CoreLib
{
  public enum StockSummaryFlag
  {
    None = 0,
    SummaryOnly = 1,
    SummaryPlusAux2_10 = 2,
    SummaryPlusAllAux = 3
  }

  public interface IStockSummary
  {
    void MyMethod(StockSummaryFlag flag);
  }
}
