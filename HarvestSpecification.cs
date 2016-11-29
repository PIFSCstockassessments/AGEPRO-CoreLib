using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGEPRO.CoreLib
{
    public class HarvestSpecification
    {
        //Used in ControlHarvestScenario for the Harvest Specification Combo Box Column
        public int Index { get; set; }
        public string HarvestScenario { get; set; }

        public HarvestSpecification(int i, string spec)
        {
            Index = i;
            HarvestScenario = spec;
        }

        private static readonly List<HarvestSpecification> harvestSpec = new List<HarvestSpecification>
        {
            { new HarvestSpecification(0, "F-MULT") },
            { new HarvestSpecification(1, "LANDINGS") },
            { new HarvestSpecification(2, "REMOVALS") }
        };

        public static List<HarvestSpecification> GetHarvestSpec()
        {
            return harvestSpec;
        }

    }
}
