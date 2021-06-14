using System.Collections.Generic;

namespace Nmfs.Agepro.CoreLib
{

  public class RecruitModelDictionaryContainer
  {
    public Dictionary<int, string> RecruitDictionary { get; set; }

    public RecruitModelDictionaryContainer()
    {
      RecruitDictionary = GetRecruitModelDictionary();
    }

    /// <summary>
    /// Creates and sets the Recruitment Model Dictionary Object
    /// </summary>
    /// <returns>Returns the Recruitment Model Dictionary</returns>
    public Dictionary<int, string> GetRecruitModelDictionary()
    {
      //TODO: Generalize/Automate this Dictionary? (resx?)

      return new Dictionary<int, string>
      {
        { 0, "None Selected" },
        { 1, "Model 1: Markov Matrix" },
        { 2, "Model 2: Empirical Recruits per Spawning Biomass Distribution" },
        { 3, "Model 3: Empirical Recruitment Distributiion" },
        { 4, "Model 4: Two-Stage Empirical Recruits per Spawning Biomass Distribution" },
        { 5, "Model 5: Beverton-Holt Curve w/ Lognormal Error" },
        { 6, "Model 6: Ricker Curve w/ Lognormal Error" },
        { 7, "Model 7: Shepherd Curve w/ Lognormal Error" },
        { 8, "Model 8: Lognormal Distribution" },
        //Model 9 was removed in AGEPRO 4.0
        { 10, "Model 10: Beverton-Holt Curve w/ Autocorrected Lognormal Error" },
        { 11, "Model 11: Ricker Curve w/ Autocorrected Lognormal Error" },
        { 12, "Model 12: Shepherd Curve w/ Autocorrected Lognormal Error" },
        { 13, "Model 13: Autocorrected Lognormal Distribution" },
        { 14, "Model 14: Empirical Cumulative Distribution Function of Recruitment" },
        { 15, "Model 15: Two-Stage Empirical Cumulative Distribution Function of Recruitment" },
        { 16, "Model 16: Linear Recruits per Spawning Biomass Predictor w/ Normal Error" },
        { 17, "Model 17: Loglinear Recruits per Spawning Biomass Predictor w/ Lognormal Error" },
        { 18, "Model 18: Linear Recruitment Predictor w/ Normal Error" },
        { 19, "Model 19: Loglinear Recruitment Predictor w/ Lognormal Error" },
        { 20, "Model 20: Fixed Recruitment" },
        { 21, "Model 21: Empirical Cumulative Distribution Function of Recruitment w/ Linear Decline to Zero" }
      };
    }
  }
}