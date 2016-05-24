using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AGEPRO_struct
{
    /// <summary>
    /// Generalized, abstract representation of various AGEPRO Recruitment Models
    /// </summary>
    abstract public class RecruitmentModel
    {
        public int recruitModelNum;
        public int recruitCategory;
        public abstract void ReadRecruitmentModel(StreamReader sr);

        /// <summary>
        /// Looks up the name of the AGEPRO Recruitment Model. Unimpmented at this time. 
        /// </summary>
        /// <param name="modelType">Desginated AGEPRO Model Numnber</param>
        /// <returns>String containng the model name</returns>
        public string GetRecruitModelName(int modelType)
        {
            //TODO:Use Dictionary to get Model Name  

            return "";
        }
    }
}
