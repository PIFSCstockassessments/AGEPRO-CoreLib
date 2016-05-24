using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace AGEPRO_struct
{
    /// <summary>
    /// Emperical Recruitment
    /// </summary>
    public class EmpericalRecruitment : RecruitmentModel
    {
        public int numObs { get; set; }
        public DataTable obsTable { get; set; }
        public bool withSSB { get; set; }
        
        public EmpericalRecruitment(int modelNum)
        {
            this.recruitModelNum = modelNum;
            this.recruitCategory = 1;
            this.withSSB = false;
        }

        public EmpericalRecruitment(int modelNum, bool useSSB) : this(modelNum)
        {
            this.withSSB = useSSB;
        }

        /// <summary>
        /// Reads in AGEPRO Input File Stream For Emperical Recruitment Specfic Parameters & Data 
        /// </summary>
        /// <param name="sr">AGEPRO Input File StreamReader</param>
        public override void ReadRecruitmentModel(StreamReader sr)
        {
            string line;
            
            //numObs
            line = sr.ReadLine();
            this.numObs = Convert.ToInt32(line);

            //obsTable
            this.obsTable = ReadObsTable(sr, this.numObs, this.withSSB);


        }

        protected DataTable ReadObsTable(StreamReader sr, int nObs, bool useSSB)
        {
            
            string line;
            line = sr.ReadLine();
            string[] nobsRecruits = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            //inputTable
            DataTable inputTable = new DataTable("Observation Table");
            inputTable.Columns.Add("Recruits", typeof(double));

            if (useSSB)
            {
                inputTable.Columns.Add("SSB", typeof(double));

                line = sr.ReadLine();
                string[] nobsSSB = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < nObs; i++)
                {
                    inputTable.Rows.Add(Convert.ToDouble(nobsRecruits[i]), Convert.ToDouble(nobsSSB[i]));
                }
                
            }
            else //useSSB is false
            {
                for (int i = 0; i < nObs; i++)
                {
                    inputTable.Rows.Add(Convert.ToDouble(nobsRecruits[i]));
                }
                
            }
            return inputTable;
        }

        
    }

    /// <summary>
    /// Two-Stage Emperical Recruitment. Parameters & Observations for two levels (stages).
    /// </summary>
    public class TwoStageEmpericalRecruitment : EmpericalRecruitment
    {
        public int lv1NumObs { get; set; }
        public int lv2NumObs { get; set; }
        public int SSBBreakVal { get; set; }
        public DataTable lv1Obs { get; set; }
        public DataTable lv2Obs { get; set; }

        public TwoStageEmpericalRecruitment(int modelNum)
            : base(modelNum)
        {
            this.recruitModelNum = modelNum;
            this.recruitCategory = 1;
            this.withSSB = true; //TODO: Should this be default?
        }

        public TwoStageEmpericalRecruitment(int modelNum, bool useSSB)
            : this(modelNum)
        {
            this.withSSB = useSSB;
        }

        public override void ReadRecruitmentModel(StreamReader sr)
        {
            string line;
            DataTable inputTable = new DataTable("Observation Table");

            //lv1NumObs, lv2NumObs
            line = sr.ReadLine();
            string[] lineNumObsLvl = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            this.lv1NumObs = Convert.ToInt32(lineNumObsLvl[0]);
            this.lv2NumObs = Convert.ToInt32(lineNumObsLvl[1]);

            //lv1Obs 
            this.lv1Obs = base.ReadObsTable(sr, this.lv1NumObs, this.withSSB);
            //lv2Obs
            this.lv2Obs = base.ReadObsTable(sr, this.lv2NumObs, this.withSSB);

            //SSBBReakVal
            line = sr.ReadLine();
            this.SSBBreakVal = Convert.ToInt32(line);
        }
    }

    /// <summary>
    /// Emperical CDF of Recruitment w/ Linear Decline to Zero.
    /// </summary>
    public class EmpericalCDFZero : EmpericalRecruitment
    {
        public double? SSBHinge { get; set; }

        public EmpericalCDFZero(int modelNum) : base(modelNum) { }

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