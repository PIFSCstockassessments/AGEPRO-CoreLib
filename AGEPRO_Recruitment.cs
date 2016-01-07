using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace AGEPRO_struct
{
    public class AGEPRO_Recruitment
    {
        public int recruitScalingFactor { get; set; }
        public int SSBScalingFactor { get; set; }
        public int maxRecuitObs { get; set; }
        public int[] recruitType { get; set; }
        public DataTable recruitProb { get; set; }
        public int recruitmentCategory { get; set; }

        public AGEPRO_Recruitment()
        {
        }
        
        public void ReadRecruitmentData(StreamReader sr, int nyears, int numRecruitModels)
        {
            string line;

            line = sr.ReadLine();
            string[] recruitOpt = line.Split(' ');
            this.recruitScalingFactor = Convert.ToInt32(recruitOpt[0]); //Recruitment Scaling Factor
            this.SSBScalingFactor = Convert.ToInt32(recruitOpt[1]); //SSB Scaling Factor
            this.maxRecuitObs = Convert.ToInt32(recruitOpt[2]);

            //Recruit Methods
            line = sr.ReadLine();
            string[] recruitModels = line.Split();
            //TODO:Keep Recruit Type int or string?
            this.recruitType = Array.ConvertAll<string, int>(recruitModels, int.Parse);  

            //Check numRecruitModels matches actual count
            if (this.recruitType.Count() != numRecruitModels)
            {
                throw new System.InvalidOperationException("numRecruitModels does not match input file recruitModel count");
            }


            //Recruit Prob
            for (int i = 0; i < nyears; i++)
            {
                line = sr.ReadLine();
                string[] nyearRecruitProb = line.Split(' ');
                
                //TODO: Check Recruitment Probability for all selections of each year sums to 1.0
                
                for (int j=0; j < numRecruitModels; j++)
                {
                    this.recruitProb.Rows[i][j] = nyearRecruitProb[j];
                }
                
            }
            
            
            //Recruitment type
            for (int i = 0; i < numRecruitModels; i++)
            {
                //TODO:Check for multiple Markov Matrix Recuitments. (Only one is allowed)

                try
                {

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            //return list of recruitments
            
        }

        public bool ValidateModel()
        {
            return true;
        }
    }


}
