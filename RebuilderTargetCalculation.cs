using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGEPRO.CoreLib
{
    /// <summary>
    /// Specifications for Stock Rebuilder Targets
    /// </summary>
    class RebuilderTargetCalculation : HarvestCalculation
    {
        private int _targetYear;
        private double _targetValue;
        private int _targetType;        //rebuild target type (cboRebuild.SelectedIndex)
        private double _targetPercent;  //Percent Confidence

        public int targetYear
        {
            get { return _targetYear; }
            set { SetProperty(ref _targetYear, value); }
        }
        public double targetValue
        {
            get { return _targetValue; }
            set { SetProperty(ref _targetValue, value); }
        }
        public int targetType
        {
            get { return _targetType; }
            set { SetProperty(ref _targetType, value); }
        }
        public double targetPercent
        {
            get { return _targetPercent; }
            set { SetProperty(ref _targetPercent, value); }
        }
        public RebuilderTargetCalculation()
        {
            calculationType = HarvestScenarioAnalysis.Rebuilder;
        }

        /// <summary>
        /// Readin AGEPRO Input Data File for Rebuild Specification Parameters
        /// </summary>
        /// <param name="sr">AGEPRO Input Data File StreamReader</param>
        public override void ReadCalculationDataLines(System.IO.StreamReader sr)
        {
            string line;
            line = sr.ReadLine();
            string[] rebuildOptionLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            this.targetYear = Convert.ToInt32(rebuildOptionLine[0]);
            this.targetValue = Convert.ToDouble(rebuildOptionLine[1]);
            this.targetType = Convert.ToInt32(rebuildOptionLine[2]);
            this.targetPercent = Convert.ToDouble(rebuildOptionLine[3]);

        }

        /// <summary>
        /// Generates AGEPRO Input Data file lines related to the [REBUILD] parameter
        /// </summary>
        /// <returns>A list of strings to be appended to the AGEPRO Input Data file.</returns>
        public override List<string> WriteCalculationDataLines()
        {
            List<string> outputLines = new List<string>();
            outputLines.Add("[REBUILD]");
            outputLines.Add(this.targetYear + new string(' ', 2) +
                this.targetValue + new string(' ', 2) +
                this.targetType + new string(' ', 2) +
                this.targetPercent);

            return outputLines;
        }
    }
}
