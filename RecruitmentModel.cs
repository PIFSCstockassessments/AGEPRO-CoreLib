using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AGEPRO_struct
{
    abstract public class RecruitmentModel
    {
        public int recruitModelNum;
        public int recruitCategory;
        public abstract void ReadRecruitmentModel(StreamReader sr);


        public string GetRecruitModelName(int modelType)
        {
            //TODO:Use Dictionary to get Model Name  

            return "";
        }

        //public virtual void ReadRecruitmentModel(StreamReader sr)
        //{
        //    //Generic Recruitment Model does not have inupt file strucutre. 
        //    //TODO: Should throw exception or do nothing?
        //    throw new NotImplementedException();
        //}

        //public virtual void ReadRecruitmentModel(StreamReader sr, int nyears)
        //{
        //    //Throw to generic ReadRecruitmentModel method 
        //     throw new NotImplementedException();
        //}
    }
}
