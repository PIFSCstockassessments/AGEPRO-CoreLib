﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGEPRO.CoreLib
{
    public class GeneralizedRecruitment : RecruitmentModel
    {
        /// <summary>
        /// A empty RecruitmentModel class. Only meant for Recruit List intialization purposes.
        /// </summary>
        public GeneralizedRecruitment()
        {
            this.recruitModelNum = 0;
            this.recruitCategory = 0;
        }

        public override void ReadRecruitmentModel(System.IO.StreamReader sr)
        {
            throw new NotImplementedException();
        }
    }
}