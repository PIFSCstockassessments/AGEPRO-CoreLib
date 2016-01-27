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
        public DataTable recruitProb = new DataTable();
        public int recruitmentCategory { get; set; }
        public List<RecruitmentModel> recruitList { get; set; }
        public int[] observationYears { get; set; }

        public AGEPRO_Recruitment()
        {
            
        }
        
        public void ReadRecruitmentData(StreamReader sr, int nyears, int numRecruitModels)
        {
            string line;
            Console.WriteLine("Reading Recuitment Data ... ");

            line = sr.ReadLine();
            string[] recruitOpt = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            this.recruitScalingFactor = Convert.ToInt32(recruitOpt[0]); //Recruitment Scaling Factor
            this.SSBScalingFactor = Convert.ToInt32(recruitOpt[1]); //SSB Scaling Factor
            this.maxRecuitObs = Convert.ToInt32(recruitOpt[2]);

            //Recruit Methods
            line = sr.ReadLine().Trim();
            string[] recruitModels = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //TODO:Keep Recruit Type a int array for switch-case
            this.recruitType = Array.ConvertAll<string, int>(recruitModels, int.Parse);  

            //Check numRecruitModels matches actual count
            if (this.recruitType.Count() != numRecruitModels)
            {
                throw new System.InvalidOperationException("numRecruitModels does not match input file recruitModel count");
            }

            

            //Set Recruit Prob Columns
            for(int nselection = 0; nselection < recruitType.Count(); nselection++)
            {
                recruitProb.Columns.Add("Selection " + (nselection+1).ToString(), typeof(double));
            }
            //Recruitment Probability
            for (int i = 0; i < nyears; i++)
            {
                line = sr.ReadLine();
                string[] nyearRecruitProb = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                recruitProb.Rows.Add(nyearRecruitProb);
                
                //TODO: Check Recruitment Probability for all selections of each year sums to 1.0
                
                
            }

            Console.WriteLine("Reading Recuitment Model Data ... ");
            
            //Instanciate recruitList as new list
            recruitList = new List<RecruitmentModel>();
            
            //Recruitment type
            for (int i = 0; i < numRecruitModels; i++)
            {
                AddToRecruitList(this.recruitType[i], recruitList);
            }

            //Check for multiple Markov Matrix Recuitments. (Only one is allowed)
            if (recruitList.Count(r => r.recruitModelNum == 1) > 1)
            {
                throw new ArgumentException("Multiple Markov Matrix Recruitment not allowed");
            }   

            //Read Recruitment Data
            foreach (var nrecruit in recruitList)
            {
                nrecruit.ReadRecruitmentModel(sr);
            }

            Console.WriteLine("Done.");
        }
        
        
        private void AddToRecruitList(int recruitType, List<RecruitmentModel> recruitList)
        {
            
            switch (recruitType)
            {
                case 1:
                    recruitList.Add(new MarkovMatrixRecruitment());
                    break;
                case 2:
                    recruitList.Add(new EmpericalRecruitment(recruitType, useSSB: true)); 
                    break;
                case 3: 
                case 14:
                case 20:
                    recruitList.Add(new EmpericalRecruitment(recruitType, useSSB: false));
                    break;
                case 4:
                    
                    recruitList.Add(new TwoStageEmpericalRecruitment(recruitType, useSSB: true));
                    break;
                case 5:
                case 6:
                case 7:
                    recruitList.Add(new ParametricCurve(recruitType, isAutocorrelated: false));
                    break;
                case 8:
                    recruitList.Add(new ParametricLognormal(recruitType, isAutocorrelated: false));
                    break;
                case 10:
                case 11:
                case 12:
                    recruitList.Add(new ParametricCurve(recruitType, isAutocorrelated: true));
                    break;
                case 13:
                    recruitList.Add(new ParametricLognormal(recruitType, isAutocorrelated: true));
                    break;
                case 15:
                    recruitList.Add(new TwoStageEmpericalRecruitment(recruitType, useSSB: false));
                    break;
                case 16:
                case 17:
                case 18:
                case 19:
                    PredictorRecruitment predictorRecruitModel = new PredictorRecruitment(recruitType);
                    predictorRecruitModel.obsYears = this.observationYears;
                    recruitList.Add(predictorRecruitModel);
                    break;
                case 21:
                    recruitList.Add(new EmpericalCDFZero(recruitType));
                    break;
                case 0:
                default:
                    throw new ArgumentOutOfRangeException("recruitType","Must be a vaild recruitType model number.");  
            }//end switch

            
            //return RecruitmentModel
        }


    }


}
