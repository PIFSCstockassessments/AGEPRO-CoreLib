using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AGEPRO.CoreLib
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

        public class Refpoint
        {
            public double refSpawnBio { get; set; } //SSBThresh
            public double refJan1Bio { get; set; }  //StockBioThresh
            public double refMeanBio { get; set; }  //MeanBioThresh
            public double refFMort { get; set; }    //FMortThresh

        }

        public class ReportPercentile
        {
            public double percentile { get; set; }
        }

        public class ScaleFactors
        {
            public double scaleBio { get; set; }
            public double scaleRec { get; set; }
            public double scaleStockNum { get; set; }
        }

        public class Bounds
        {
            public double maxWeight { get; set; }
            public double maxNatMort { get; set; }
        }

        public class retroAdjustmentFactors
        {
            public DataTable retroAdjust; 

            public retroAdjustmentFactors()
            {
                retroAdjust = new DataTable("Retro Adjustment Factors");
            }
        }
    }
}
