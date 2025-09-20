using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nmfs.Agepro.CoreLib
{
  public enum AuxiliaryOutputFlag
  {
    NoStockAge_ExcludeStockNumAuxFile = 0,
    StockAge_AllAuxFiles = 1,
    NoStockAge_NoAuxFiles = 2,
    StockAge_NoAuxFiles = 3,
    StockAge_ExcludeStockNumAuxFile = 4
  }

  /// <summary>
  /// Interface to the AuxiliaryOutputFlag Enum. Refernenced by ControlMiscOptions to set Auxiliary Output Flags
  /// </summary>
  public interface IStockSummary
  {
    AuxiliaryOutputFlag SummaryAuxFileOutputFlag { get; }
  }
}
