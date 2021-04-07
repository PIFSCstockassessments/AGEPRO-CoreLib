using System;
using System.Collections.Generic;

namespace Nmfs.Agepro.CoreLib
{
  public class NullSelectRecruitment : RecruitmentModelProperty
  {
    /// <summary>
    /// A empty RecruitmentModel class. Only meant for Recruit List intialization purposes.
    /// </summary>
    public NullSelectRecruitment()
    {
      recruitModelNum = 0;
      recruitCategory = 0;
    }

    public override void ReadRecruitmentModel(System.IO.StreamReader sr)
    {
      throw new InvalidAgeproParameterException(
          "Error loading AGEPRO input file:" + Environment.NewLine +
          "NullSelectRecruitment is an invalid recruitment model type" +
          "(Recruit Model # is " + recruitModelNum + ").");
    }

    public override List<string> WriteRecruitmentDataModelData()
    {
      throw new InvalidAgeproParameterException(
          "Error saving AGEPRO inputs to file:" + Environment.NewLine +
          "NullSelectRecruitment is an invalid recruitment model type" +
          "(Recruit Model # is " + recruitModelNum + "). " +
          "Select a valid recruitment model."
      );

    }

    public override ValidationResult ValidateInput()
    {
      throw new InvalidAgeproParameterException(
          "NullSelectRecruitment is an invalid recruitment model type" +
          "(Recruit Model # is " + recruitModelNum + "). " +
          "Select a valid recruitment model."
      );
    }

  }
}
