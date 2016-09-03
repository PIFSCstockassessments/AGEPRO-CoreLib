using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace AGEPRO.CoreLib
{
    public enum EmpiricalType
    {
        Empirical,
        TwoStage,
        CDFZero,
        Fixed
    };

    /// <summary>
    /// Empirical Recruitment
    /// </summary>
    public class EmpiricalRecruitment : RecruitmentModel
    {
        public int numObs { get; set; }
        public DataTable obsTable { get; set; }
        public bool withSSB { get; set; }
        public EmpiricalType subType { get; set; }
        
        public EmpiricalRecruitment(int modelNum)
        {
            this.recruitModelNum = modelNum;
            this.recruitCategory = 1;
            this.withSSB = false;
            this.numObs = 0;      //Fallback Default
        }

        public EmpiricalRecruitment(int modelNum, bool useSSB, EmpiricalType subType) : this(modelNum)
        {
            this.withSSB = useSSB;
            this.subType = subType;
        }

        /// <summary>
        /// Reads in AGEPRO Input File Stream For Empirical Recruitment Specfic Parameters & Data 
        /// </summary>
        /// <param name="sr">AGEPRO Input File StreamReader</param>
        public override void ReadRecruitmentModel(StreamReader sr)
        {
            string line;
            string[] nobsRecruits;
            string[] nobsSSB;

            //numObs
            line = sr.ReadLine();
            this.numObs = Convert.ToInt32(line);

            //obsTable
            //obsRecruits
            line = sr.ReadLine();
            nobsRecruits = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //obsSSB           
            if (this.withSSB == true)
            {
                line = sr.ReadLine();
                nobsSSB = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                this.obsTable = SetObsTable(this.numObs, nobsRecruits, nobsSSB);
            }
            else
            {
                this.obsTable = SetObsTable(this.numObs, nobsRecruits);
            }

            

        }

        protected DataTable SetObsTable(int nObs, string[] obsRecruits, string[] obsSSB = null)
        {
            bool useSSB = this.withSSB;
            
            //inputTable
            DataTable inputTable = new DataTable("Observation Table");
            inputTable.Columns.Add("Recruits", typeof(double));

            if (obsSSB != null)
            {
                inputTable.Columns.Add("SSB", typeof(double));

                for (int i = 0; i < nObs; i++)
                {
                    inputTable.Rows.Add(Convert.ToDouble(obsRecruits[i]), Convert.ToDouble(obsSSB[i]));
                }
                
            }
            else //obsSSB is null, therefore no SSB column
            {
                for (int i = 0; i < nObs; i++)
                {
                    inputTable.Rows.Add(Convert.ToDouble(obsRecruits[i]));
                }
                
            }
            return inputTable;
        }

        
    }

    /// <summary>
    /// Two-Stage Empirical Recruitment. Parameters & Observations for two levels (stages).
    /// </summary>
    public class TwoStageEmpiricalRecruitment : EmpiricalRecruitment
    {
        public int lv1NumObs { get; set; }
        public int lv2NumObs { get; set; }
        public int SSBBreakVal { get; set; }
        public DataTable lv1Obs { get; set; }
        public DataTable lv2Obs { get; set; }

        public TwoStageEmpiricalRecruitment(int modelNum)
            : base(modelNum)
        {
            this.recruitModelNum = modelNum;
            this.recruitCategory = 1;
            this.withSSB = true; //TODO: Should this be default?
            this.subType = EmpiricalType.TwoStage;

            //Fallback Defaults
            this.lv1NumObs = 0;
            this.lv2NumObs = 0;
            this.SSBBreakVal = 0;

        }

        public TwoStageEmpiricalRecruitment(int modelNum, bool useSSB)
            : this(modelNum)
        {
            this.withSSB = useSSB;
        }

        public override void ReadRecruitmentModel(StreamReader sr)
        {
            string line;
            string[] lv1RecruitObs;
            string[] lv2RecruitObs;
            
            //lv1NumObs, lv2NumObs
            line = sr.ReadLine();
            string[] lineNumObsLvl = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            this.lv1NumObs = Convert.ToInt32(lineNumObsLvl[0]);
            this.lv2NumObs = Convert.ToInt32(lineNumObsLvl[1]);

            //lv1Obs 
            line = sr.ReadLine();
            lv1RecruitObs = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            this.lv1Obs = base.SetObsTable(this.lv1NumObs, lv1RecruitObs);
            //lv2Obs
            line = sr.ReadLine();
            lv2RecruitObs = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            this.lv2Obs = base.SetObsTable(this.lv2NumObs, lv2RecruitObs);

            //SSBBReakVal
            line = sr.ReadLine();
            this.SSBBreakVal = Convert.ToInt32(line);
        }
    }

    /// <summary>
    /// Empirical CDF of Recruitment w/ Linear Decline to Zero.
    /// </summary>
    public class EmpiricalCDFZero : EmpiricalRecruitment
    {
        public double? SSBHinge { get; set; }

        public EmpiricalCDFZero(int modelNum) : base(modelNum) 
        {
            this.subType = EmpiricalType.CDFZero;
            this.SSBHinge = 0;  //Fallback Default
        }

        public override void ReadRecruitmentModel(StreamReader sr)
        {
            string line;

            //numObs and obsTable w/ base EmpiricalRecruitment function
            base.ReadRecruitmentModel(sr);

            //SSB Hinge (MT*1000)
            line = sr.ReadLine();
            this.SSBHinge = Convert.ToDouble(line);

        }

    }
}