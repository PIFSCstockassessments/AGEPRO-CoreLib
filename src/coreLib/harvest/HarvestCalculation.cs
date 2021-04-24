﻿using System;
using System.Collections.Generic;
using System.IO;


namespace Nmfs.Agepro.CoreLib
{
  /// <summary>
  /// Generalized, abstract represetion for (Non-Standard) Harvest Scenario Calculations
  /// </summary>
  abstract public class HarvestCalculation : AgeproCoreLibProperty, IValidatable
    {
        public HarvestScenarioAnalysis calculationType;
        public int[] obsYears;
        public abstract void ReadCalculationDataLines(StreamReader sr);
        public abstract List<string> WriteCalculationDataLines();



        public virtual ValidationResult ValidateInput()
        {
            throw new NotImplementedException();
        }
    }
}
