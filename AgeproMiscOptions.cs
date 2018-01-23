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
                //Set Defaults to 0
                refSpawnBio = 0;
                refJan1Bio = 0;
                refMeanBio = 0;
                refFMort = 0;
            }

        }

        /// <summary>
        /// A Specfic (custom) Percentile for the AGEPRO Output Report
        /// </summary>
        public class ReportPercentile : MiscOptionsParameter
        {
            private double _precentile;

            public double percentile 
            {
                get { return _precentile; }
                set { SetProperty(ref _precentile, value); }
            }

            public ReportPercentile()
            {
                //Set Defaults to 0.0
                percentile = 0.0;
            }
        }

        /// <summary>
        /// Scaling Factors for the AGEPRO Output Report
        /// </summary>
        public class ScaleFactors : MiscOptionsParameter
        {
            private double _scaleBio;
            private double _scaleRec;
            private double _scaleStockSum;

            public double scaleBio
            {
                get { return _scaleBio; }
                set { SetProperty(ref _scaleBio, value); }
            }
            public double scaleRec
            {
                get { return _scaleRec; }
                set { SetProperty(ref _scaleRec, value); }
            }
            public double scaleStockNum
            {
                get { return _scaleStockSum; }
                set { SetProperty(ref _scaleStockSum, value); }
            }

            public ScaleFactors()
            {
                //Set Defaults to 0
                scaleBio = 0;
                scaleRec = 0;
                scaleStockNum = 0;
            }
        }

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

        /// <summary>
        /// At-age vaiues to applied to initial population numbers to correct for retrospective bias.
        /// </summary>
        public class retroAdjustmentFactors : MiscOptionsParameter
        {
            //Data binded to AGEPRO GUI's dataGridRetroAdjustment (DataGridView)
            public DataTable retroAdjust; 

            public retroAdjustmentFactors()
            {
                retroAdjust = new DataTable("Retro Adjustment Factors");
            }
        }
    }

    /// <summary>
    /// Wrapper class for AGEPRO's MiscOptions paramerers that includes two-way data binding. 
    /// </summary>
    public class MiscOptionsParameter : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public MiscOptionsParameter()
        { 
        }

        //DanRigby 
        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }
        }
    }
}
