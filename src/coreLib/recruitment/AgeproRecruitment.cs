using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace Nmfs.Agepro.CoreLib
{
    /// <summary>
    /// General AGEPRO Recruitment Parameters. 
    /// </summary>
    public class AgeproRecruitment : AgeproCoreLibProperty
    {
        private double _recruitScalingFactor;
        private double _SSBScalingFactor;
       
        public int maxRecruitObs { get; set; }
        public int[] recruitType { get; set; }
        public DataTable recruitProb; // = new DataTable("Recruitment Probability");
        public int recruitmentCategory { get; set; }
        public List<RecruitmentModelProperty> recruitList { get; set; }
        public int[] observationYears { get; set; }

 
        public AgeproRecruitment()
        {
            recruitScalingFactor = 0;
            SSBScalingFactor = 0;

        }

        public double recruitScalingFactor
        {
            get { return _recruitScalingFactor; }
            set { SetProperty(ref _recruitScalingFactor, value); }
        }
        public double SSBScalingFactor
        {
            get { return _SSBScalingFactor; }
            set { SetProperty(ref _SSBScalingFactor, value); }
        }
       

        /// <summary>
        /// Sets up AgeoroRecruitment data based on user generated AGEPRO parameter new cases.
        /// </summary>
        /// <param name="nrecruits">Number of Recruits</param>
        /// <param name="seqYears">List of Projection Year names</param>
        public void newCaseRecruitment(int nrecruits, string[] seqYears)
        {

            this.maxRecruitObs = 500;
            //TODO: safer int.tryParse
            this.observationYears = Array.ConvertAll<string, int>(seqYears, int.Parse);

            //NullSelectRecuitment is default for NewCases.
            recruitList = new List<RecruitmentModelProperty>();
            recruitType = new int[nrecruits];
            for (int irecruit = 0; irecruit < nrecruits; irecruit++)
            {
                recruitList.Add(GetNewRecruitModel(0));
                recruitType[irecruit] = recruitList[irecruit].recruitModelNum; //0
                recruitList[irecruit].obsYears = this.observationYears;
            }

            //Set Recruitment Probabilty Values
            List<string[]> recruitProbYear = new List<string[]>();
            for (int iyear = 0; iyear < seqYears.Count(); iyear++)
            {
                string[] yrRecruitProb = new string[nrecruits];
                for (int jrecruit = 0; jrecruit < nrecruits; jrecruit++)
                {
                    double newCaseProb = Math.Round(1.0 / nrecruits);
                    yrRecruitProb[jrecruit] = newCaseProb.ToString("0.000");
                }
                recruitProbYear.Add(yrRecruitProb);
            }
            
            CreateRecruitmentProbabilityTable(recruitProbYear);
            
            
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
                throw new InvalidAgeproParameterException("numRecruitModels does not match input file recruitModel count");
            }

            List<string[]> recrProbYear = new List<string[]>();
            //Recruitment Probability
            for (int i = 0; i < nyears; i++)
            {
                line = sr.ReadLine();
                //string[] nyearRecruitProb = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                //recruitProb.Rows.Add(nyearRecruitProb);
                recrProbYear.Add(line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            }

            //Set Recruitment Probabilty Values
            CreateRecruitmentProbabilityTable(recrProbYear);

            //Instanciate recruitList as new list
            recruitList = new List<RecruitmentModelProperty>();
            
            //Recruitment type
            for (int i = 0; i < numRecruitModels; i++)
            {
                //AddToRecruitList(this.recruitType[i], recruitList);
                recruitList.Add(GetNewRecruitModel(this.recruitType[i]));
                //Set the observation years(Even if this Recruit Model is not a Predictor Recruitment Type).
                this.recruitList[i].obsYears = this.observationYears;
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
        /// Creates/Initalizes the Recruitment Proabablity Data Table
        /// </summary>
        /// <param name="listRecruitProbYr">List containing Recruitment Distribtions per Observaion Year</param>
        public void CreateRecruitmentProbabilityTable(List<string[]> listRecruitProbYr)
        {

            //Clean off potentially existing data 
            if (this.recruitProb != null)
            {
                this.recruitProb.Clear();
            }
            //Setup Recruitmet Probabilty Table
            this.recruitProb = new DataTable();
            this.recruitProb.TableName = "Recruitment Probability";


            //Set Recruit Prob Columns
            for (int nselection = 0; nselection < this.recruitType.Count(); nselection++)
            {
                String recruitProbColumnName = "Selection " + (nselection + 1).ToString();

                if (!this.recruitProb.Columns.Contains(recruitProbColumnName))
                {
                    this.recruitProb.Columns.Add(recruitProbColumnName, typeof(double));
                }
            }
            //If current Recruitment Probability table has more columns than actual count, trim it
            if (this.recruitProb.Columns.Count > this.recruitType.Count())
            {
                for (int index = this.recruitProb.Columns.Count - 1; index > 0; index--)
                {
                    this.recruitProb.Columns.RemoveAt(index);
                }
            }

            for (int irow = 0; irow < this.observationYears.Count(); irow++ )
            {
                try
                {
                    //Check Recruitment Probability for all selections of each year sums to 1.0
                    CheckRecruitProbabilitySum(listRecruitProbYr[irow]);
                }
                catch (Exception ex)
                {
                    throw new InvalidAgeproParameterException("At row " + (irow + 1).ToString() +
                        " of recruitment probablity:" + Environment.NewLine + ex.InnerException.Message
                        , ex);
                }

                this.recruitProb.Rows.Add(listRecruitProbYr[irow]);

            }




        }

        /// <summary>
        /// Checks the Selected Recruitment Probabilitiy row if it sums up to 1.0.  
        /// </summary>
        /// <param name="recruitProbRow">String array representing the row of the Recruitment 
        /// probability data grid.</param>
        /// <returns>Returns false if the row does not sum up to 1.0. Otherwise, true.</returns>
        public static bool CheckRecruitProbabilitySum(String[] recruitProbRow)
        {
            double precisionDiff;
            double rowSumRecruitProb;

            //Check Recruitment Probability for all selections of each year sums to 1.0
            rowSumRecruitProb = Array.ConvertAll<string, double>(recruitProbRow, double.Parse).Sum();
            precisionDiff = Math.Abs(rowSumRecruitProb * 0.00001);
            // To Handle Floating-Point precision issues when "sumRowRecruitProb != 1.0" comparisons
            if (!(Math.Abs(rowSumRecruitProb - (double)1) <= precisionDiff))
            {
                Console.WriteLine(
                    "Recruitment probablity sum does not equal 1.0: Probability sum is " + 
                    rowSumRecruitProb.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a new recruitment model, based on type (model number). 
        /// </summary>
        /// <param name="rtype">Recruitment Model Number</param>
        public static RecruitmentModelProperty GetNewRecruitModel(int rtype)
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
                    return(new FixedEmpiricalRecruitment(rtype));
                case 4:
                    return(new TwoStageEmpiricalRecruitment(rtype, useSSB: true));
                case 5:
                case 6:
                    return(new ParametricCurve(rtype, isAutocorrelated: false));
                case 7:
                    return(new ParametricShepherdCurve(rtype, isAutocorrelated: false));
                case 8:
                    return(new ParametricLognormal(rtype, isAutocorrelated: false));
                case 10:
                case 11:
                    return(new ParametricCurve(rtype, isAutocorrelated: true));
                case 12:
                    return(new ParametricShepherdCurve(rtype, isAutocorrelated: true));
                case 13:
                    return(new ParametricLognormal(rtype, isAutocorrelated: true));
                case 15:
                    return(new TwoStageEmpiricalRecruitment(rtype, useSSB: false));
                case 16:
                case 17:
                case 18:
                case 19:
                    return(new PredictorRecruitment(rtype));
                case 21:
                    return(new EmpiricalCDFZero(rtype));
                case 0:
                    return(new NullSelectRecruitment()); 
                default:
                    throw new InvalidAgeproParameterException("Invalid Recruitment Model Number: " + rtype);
            }//end switch

        }//end GetNewRecruitModel

        public List<string> WriteRecruitmentDataLines()
        {
            List<string> outputLines = new List<string>();

            outputLines.Add("[RECRUIT]");
            outputLines.Add(this.recruitScalingFactor.ToString() + new string(' ', 2) +
                this.SSBScalingFactor.ToString() + new string(' ', 2) +
                this.maxRecruitObs.ToString());
            
            //Gathering recruitNum from RecruitList because its more sturctured
            List<string> modelNumArrayFromRecruitList = new List<string>();
            foreach (RecruitmentModelProperty recruit in this.recruitList)
            {
                modelNumArrayFromRecruitList.Add(recruit.recruitModelNum.ToString());
            }
            outputLines.Add(string.Join(new string(' ', 2), modelNumArrayFromRecruitList));

            foreach(DataRow yearRow in this.recruitProb.Rows)
            {
                outputLines.Add(string.Join(new string(' ', 2), yearRow.ItemArray));
            }

            //Recruit Model(s)
            foreach (RecruitmentModelProperty recruitModel in recruitList)
            {
                outputLines.AddRange(recruitModel.WriteRecruitmentDataModelData());
            }


            return outputLines;
        }

        /// <summary>
        /// Creates a Recruitment Probability DataTable.
        /// </summary>
        /// <param name="yCol">Number of columns</param>
        /// <param name="xRows">Number of Rows</param>
        /// <param name="colName">Column Names</param>
        /// <returns></returns>
        public static DataTable CreateRecruitProbTable(int yCol, int xRows, string colName)
        {
            DataTable recruitProbTable = new DataTable();

            for (int icol = 0; icol < yCol; icol++)
            {
                recruitProbTable.Columns.Add(colName + " " + (icol + 1));
            }
            for (int row = 0; row < xRows; row++)
            {
                recruitProbTable.Rows.Add();
            }

            //Fill Recruit Probability table with default set of values.
            //Assume each new case recruit selection prob is spread evenly.
            double recruitProbVal = 1 / Convert.ToDouble(yCol);

            for (int irow = 0; irow < xRows; irow++)
            {
                for (int jcol = 0; jcol < yCol; jcol++)
                {
                    if (recruitProbTable.Rows[irow][jcol] == DBNull.Value)
                    {
                        recruitProbTable.Rows[irow][jcol] = recruitProbVal;
                    }
                }
            }

            return recruitProbTable;
        }
     
                
    }


}
