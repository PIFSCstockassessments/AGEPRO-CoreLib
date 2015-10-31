using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AGEPRO_struct
{
    public class Recruitment
    {
        public int recruitScalingFactor { get; set; }
        public int SSBScalingFactor { get; set; }
        public int maxRecuitObs { get; set; }
        public string recruitType { get; set; }
        public DataTable recruitProb { get; set; }

        public Recruitment()
        {
        }
        
        public void ReadRecruitmentData(string[] inputFile, int nyears)
        {
            //for (int i = 0; i < inputFile.Length; i++ )
            //{
                
            //    //line1
            //    if(i == 1)
            //    {
            //        string[] tokens = inputFile[i].Split(' ');
            //        this.recruitScalingFactor = Convert.ToInt32(tokens[0]); //Recruitment Scaling Factor
            //        this.SSBScalingFactor = Convert.ToInt32(tokens[1]); //SSB Scaling Factor
            //        this.maxRecuitObs = Convert.ToInt32(tokens[2]);
            //    }
            //    else if (i == 2)
            //    {
            //        //Recruitment Model(s)
            //        this.recruitType = inputFile[i];
            //    }
            //    else if (i >= 3 || i <= 3+nyears )
            //    {
            //        this.recruitProb.Rows.Add(inputFile[i]);
            //    }
  
            //    //Recruitment type

            //}

        }

        public bool ValidateModel()
        {
            return true;
        }
    }


}
