using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nmfs.Agepro.CoreLib
{
    public class NullSelectRecruitment : RecruitmentModel
    {
        /// <summary>
        /// A empty RecruitmentModel class. Only meant for Recruit List intialization purposes.
        /// </summary>
        public NullSelectRecruitment()
        {
            this.recruitModelNum = 0;
            this.recruitCategory = 0;
        }

        public override void ReadRecruitmentModel(System.IO.StreamReader sr)
        {
            throw new NotImplementedException();
        }

        public override List<string> WriteRecruitmentDataModelData()
        {
            throw new NotImplementedException();
        }

        public override bool ValidateRecruitmentData(int selectionIndex)
        {
            throw new NotImplementedException();
        }
    }
}
