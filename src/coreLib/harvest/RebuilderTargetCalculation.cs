﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nmfs.Agepro.CoreLib
{
    /// <summary>
    /// Specifications for Stock Rebuilder Targets
    /// </summary>
    public class RebuilderTargetCalculation : AgeproHarvestScenario
    {
        //private int _targetYear;
        private double _targetValue;
        private int _targetType;        //rebuild target type (cboRebuild.SelectedIndex)
        private double _targetPercent;  //Percent Confidence (Rebulider Confidence Level)

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
            this.targetYear = 0;
            this.targetValue = 0;
            this.targetType = 0;
            this.targetPercent = 0;
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

        public override ValidationResult ValidateInput()
        {
            List<string> errorMsgList = new List<string>();
            int yrStart = obsYears[0];
            int yrEnd = obsYears[(obsYears.Count()-1)];

            //Rebulider Year
            if (this.targetYear < yrStart || this.targetYear > yrEnd)
            {
                errorMsgList.Add("Invalid Rebuilder Year Specification.");
            }
            //Rebuilder Target
            if (string.IsNullOrWhiteSpace(this.targetValue.ToString()))
            {
                errorMsgList.Add("Invalid or missing rebuilder target value.");
            }
            //Rebuilder Confidence Level
            if (string.IsNullOrWhiteSpace(this.targetPercent.ToString()))
            {
                errorMsgList.Add("Invalid or missing rebuilder confidence level.");
            }
            //

            var results = errorMsgList.EnumerateValidationResults();
            return results;
        }
    }
}