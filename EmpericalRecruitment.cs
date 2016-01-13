using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace AGEPRO_struct
{
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
            DataTable inputTable = new DataTable();

            //obsTable
            line = sr.ReadLine();
            string[] nobsRecruits = line.Split(' ');
            if (useSSB)
            {
                line = sr.ReadLine();
                string[] nobsSSB = line.Split(' ');
                for (int i = 0; i < nObs; i++)
                {
                    inputTable.Rows.Add((i + 1), Convert.ToDouble(nobsRecruits[i]), Convert.ToDouble(nobsSSB[i]));
                }
                
            }
            else
            {
                for (int i = 0; i < nObs; i++)
                {
                    inputTable.Rows.Add((i + 1), Convert.ToDouble(nobsRecruits[i]));
                }
                
            }

            return inputTable;
        }

        
    }

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
            DataTable inputTable = new DataTable();

            //lv1NumObs, lv2NumObs
            line = sr.ReadLine();
            string[] lineNumObsLvl = line.Split(' ');
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