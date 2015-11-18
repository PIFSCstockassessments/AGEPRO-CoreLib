using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AGEPRO_struct
{
    public class AGEPRO_MiscOptions
    {
        public bool enableSummaryReport { get; set; }
        public bool enableDataFiles { get; set; }
        public bool enableExportR { get; set; }
        //enabled if classes are called.
        public static bool enableRefpoint { get; set; }
        public static bool enablePercentileReport { get; set; }
        public static bool enableScaleFactors { get; set; }
        public static bool enableBounds { get; set; }
        public static bool enableRetroAdjustmentFactors { get; set; }

        public AGEPRO_MiscOptions()
        {
            enableRefpoint = false;
            enablePercentileReport = false;
            enableScaleFactors = true; //TODO: should this be enabled by default?
            enableBounds = false;
            enableRetroAdjustmentFactors = false;
        }

        public class Refpoint
        {
            public double refSSB { get; set; }
            public double refStockBio { get; set; }
            public double refMeanBio { get; set; }
            public double refFMort { get; set; }

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
            public DataTable retroAdjust = new DataTable();
        }
    }
}
