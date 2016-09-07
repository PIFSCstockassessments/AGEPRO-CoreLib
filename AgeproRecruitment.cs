﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace AGEPRO.CoreLib
{
    /// <summary>
    /// General AGEPRO Recruitment Parameters. 
    /// </summary>
    public class AgeproRecruitment
    {
        public int recruitScalingFactor { get; set; }
        public int SSBScalingFactor { get; set; }
        public int maxRecruitObs { get; set; }
        public int[] recruitType { get; set; }
        public DataTable recruitProb = new DataTable("Recruitment Probability");
        public int recruitmentCategory { get; set; }
        public List<RecruitmentModel> recruitList { get; set; }
        public int[] observationYears { get; set; }

        public AgeproRecruitment()
        {
            
        }
        
        /// <summary>
        /// Reads in AGEPRO Input File for Recruitment Model Data
        /// </summary>
        /// <param name="sr">AGEPRO Input File StreamReader</param>
        /// <param name="nyears">Number of Years in projection</param>
        /// <param name="numRecruitModels">Number of Recruitment models</param>
        public void ReadRecruitmentData(StreamReader sr, int nyears, int numRecruitModels)
        {
            string line;
            double nyearProbSum;
            double precisionDiff;
            
            //Clean off any existing data on DataTables
            recruitProb.Clear();
            
            Console.WriteLine("Reading Recuitment Data ... ");

            line = sr.ReadLine();
            string[] recruitOpt = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            this.recruitScalingFactor = Convert.ToInt32(recruitOpt[0]); //Recruitment Scaling Factor
            this.SSBScalingFactor = Convert.ToInt32(recruitOpt[1]); //SSB Scaling Factor
            this.maxRecruitObs = Convert.ToInt32(recruitOpt[2]);

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
                String recruitProbColumnName = "Selection " + (nselection+1).ToString(); 
                
                if (!recruitProb.Columns.Contains(recruitProbColumnName))
                {
                    recruitProb.Columns.Add(recruitProbColumnName, typeof(double));
                }                
            }
            //If current Recruitment Probability table has more columns than actual count, trim it
            if (recruitProb.Columns.Count > recruitType.Count())
            {
                for (int index=recruitProb.Columns.Count-1; index > 0 ; index--){
                    recruitProb.Columns.RemoveAt(index);
                }
            }


            //Recruitment Probability
            for (int i = 0; i < nyears; i++)
            {
                line = sr.ReadLine();
                string[] nyearRecruitProb = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                recruitProb.Rows.Add(nyearRecruitProb);
                
                //Check Recruitment Probability for all selections of each year sums to 1.0
                nyearProbSum = Array.ConvertAll<string, double>(nyearRecruitProb, double.Parse).Sum();
                precisionDiff = Math.Abs(nyearProbSum * 0.00001);
                // To Handle Floating-Point precision issues when "nyearProbSum != 1.0" comparisons
                if(!(Math.Abs(nyearProbSum - (double)1) <= precisionDiff))
                {
                    throw new InvalidOperationException("Year " + (i + 1).ToString() + " recruitment probablity sum does not equal 1.0: " + 
                        "Probability sum is " + nyearProbSum.ToString());
                }
                
            }

            
            //Instanciate recruitList as new list
            recruitList = new List<RecruitmentModel>();
            
            //Recruitment type
            for (int i = 0; i < numRecruitModels; i++)
            {
                //AddToRecruitList(this.recruitType[i], recruitList);
                recruitList.Add(GetNewRecruitModel(this.recruitType[i]));
            }

            //Check for multiple Markov Matrix Recuitments. (Only one is allowed)
            if (recruitList.Count(r => r.recruitModelNum == 1) > 1)
            {
                throw new ArgumentException("Multiple Markov Matrix Recruitment not allowed");
            }
            
            Console.WriteLine("Reading Recuitment Model Data ... ");
            
            //Read Recruitment Data
            foreach (var nrecruit in recruitList)
            {
                nrecruit.ReadRecruitmentModel(sr);
            }

            Console.WriteLine("Done.");
        }

        /// <summary>
        /// Returns a new recruitment model, based on type (model number). 
        /// </summary>
        /// <param name="rtype">Recruitment Model Number</param>
        public RecruitmentModel GetNewRecruitModel(int rtype)
        {
            switch (rtype)
            {
                case 1:
                    return (new MarkovMatrixRecruitment());
                case 2:
                    return (new EmpiricalRecruitment(rtype, useSSB: true, subType: EmpiricalType.Empirical));
                case 3:
                case 14:
                    return(new EmpiricalRecruitment(rtype, useSSB: false, subType: EmpiricalType.Empirical));
                case 20:
                    return(new EmpiricalRecruitment(rtype, useSSB: false, subType: EmpiricalType.Fixed));
                case 4:
                    return(new TwoStageEmpiricalRecruitment(rtype, useSSB: true));
                case 5:
                case 6:
                case 7:
                    return(new ParametricCurve(rtype, isAutocorrelated: false));
                case 8:
                    return(new ParametricLognormal(rtype, isAutocorrelated: false));
                case 10:
                case 11:
                case 12:
                    return(new ParametricCurve(rtype, isAutocorrelated: true));
                case 13:
                    return(new ParametricLognormal(rtype, isAutocorrelated: true));
                case 15:
                    return(new TwoStageEmpiricalRecruitment(rtype, useSSB: false));
                case 16:
                case 17:
                case 18:
                case 19:
                    PredictorRecruitment predictorRecruitModel = new PredictorRecruitment(rtype);
                    predictorRecruitModel.obsYears = this.observationYears;
                    return(predictorRecruitModel);
                case 21:
                    return(new EmpiricalCDFZero(rtype));
                case 0:
                default:
                    throw new ArgumentOutOfRangeException("rtype", "Must be a vaild recruitType model number.");
            }//end switch

        }//end GetNewRecruitModel
                
    }


}
