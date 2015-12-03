using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AGEPRO_struct
{
    public class RecruitmentModel
    {
        public int recruitModelNum { get; set; }
        public int recruitCategory { get; set; }

        public RecruitmentModel()
        {

        }

        public string GetRecruitModelName(int modelType)
        {
            //TODO:Use Dictionary to get Model Name  

            return "";
        }

        protected virtual void ReadRecruitmentModel(StreamReader sr)
        {
            //Generic Recruitment Model does not have inupt file strucutre. 
            //TODO: Should throw exception or do nothing?
            throw new Exception("Recruitment Model is not a valid AGEPRO Recruitment model.");
        }
    }
}
