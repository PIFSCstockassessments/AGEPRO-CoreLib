using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace Nmfs.Agepro.CoreLib
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
    public class EmpiricalRecruitment : RecruitmentModel, IValidatable
    {
        public int numObs { get; set; }
        public DataTable obsTable { get; set; }
        public bool withSSB { get; set; }
        public EmpiricalType subType { get; set; }
        protected double lowBound { get; set; }
        
        public EmpiricalRecruitment(int modelNum)
        {
            this.recruitModelNum = modelNum;
            this.recruitCategory = 1;
            this.withSSB = false;
            this.numObs = 0;      //Fallback Default
            this.lowBound = 0.0001;
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
            
            //numObs
            line = sr.ReadLine();
            this.numObs = Convert.ToInt32(line);

            //obsTable
            this.obsTable = ReadObsTable(sr, this.numObs);
            
        }

        protected DataTable ReadObsTable(StreamReader sr, int numObs)
        {
            string line;
            string[] nobsRecruits;
            string[] nobsSSB;

            //obsRecruits
            line = sr.ReadLine();
            nobsRecruits = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //obsSSB           
            if (this.withSSB == true)
            {
                line = sr.ReadLine();
                nobsSSB = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                return SetObsTableData(numObs, nobsRecruits, nobsSSB);
            }
            else
            {
                return SetObsTableData(numObs, nobsRecruits);
            }

            
        }

        /// <summary>
        /// Sets a new Observation Datatable and populates it with string array paramters. 
        /// </summary>
        /// <param name="numObs"></param>
        /// <param name="obsRecruits"></param>
        /// <param name="obsSSB"></param>
        /// <returns></returns>
        protected DataTable SetObsTableData(int numObs, string[] obsRecruits, string[] obsSSB = null)
        {
            //inputTable
            DataTable inputTable = SetNewObsTable(numObs);
            int i = 0;
            foreach (DataRow obsRow in inputTable.Rows)
            {
                //TODO: Add a check if stated "numObs" is more than inputTable row count  
                obsRow["Recruits"] = Convert.ToDouble(obsRecruits[i]);
                if (this.withSSB)
                {
                    obsRow["SSB"] = Convert.ToDouble(obsSSB[i]);
                }
                i++;
            }
            

            return inputTable;
        }

        /// <summary>
        /// Creates an empty observation data table.
        /// </summary>
        /// <param name="numObs">Number of Observation Rows.</param>
        /// <returns>Returns a Empty Data Table</returns>
        public DataTable SetNewObsTable(int numObs)
        {
            //inputTable
            DataTable obsTable = new DataTable("Observation Table");
            obsTable.Columns.Add("Recruits", typeof(double));
            if (this.withSSB)
            {
                obsTable.Columns.Add("SSB", typeof(double));
            }
            
            for (int i = 0; i < numObs; i++)
            {
                obsTable.Rows.Add();
            }

            return obsTable;
        }

        public override List<string> WriteRecruitmentDataModelData()
        {
            List<string> outputLines = new List<string>();
            outputLines.Add(this.numObs.ToString());
            
            //obsTable
            outputLines.AddRange(this.WriteObsTableLines(this.obsTable,this.withSSB));
            
            return outputLines;
        }

        protected List<string> WriteObsTableLines(DataTable recruitObsTable, bool hasSSBCols)
        {
            List<string> obsTableLines = new List<string>();

            List<string> obsRecritCol = new List<string>();
            List<string> obsSSBCol = new List<string>();
            foreach (DataRow obsRow in recruitObsTable.Rows)
            {
                obsRecritCol.Add(obsRow["Recruits"].ToString());
                if (hasSSBCols)
                {
                    obsSSBCol.Add(obsRow["SSB"].ToString());
                }
            }
            obsTableLines.Add(string.Join(new string(' ', 2), obsRecritCol));
            
            if (hasSSBCols)
            {
                obsTableLines.Add(string.Join(new string(' ', 2), obsSSBCol));
            }

            return obsTableLines;
        }

        public virtual ValidationResult ValidateInput()
        {
            if (this.HasBlankOrNullCells(this.obsTable) == false)
            {
                return new ValidationResult(false, "Missing Data in observation table");
            }
            if (this.TableHasAllSignificantValues(this.obsTable, this.lowBound) == false)
            {
                return new ValidationResult(false, "Insignificant values or values lower than " 
                    + this.lowBound + " found in observation table");
            }

            return new ValidationResult(true, "Validation Successful");

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
            this.lowBound = 0.0001;

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

            //lv1NumObs, lv2NumObs
            line = sr.ReadLine();
            string[] lineNumObsLvl = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            this.lv1NumObs = Convert.ToInt32(lineNumObsLvl[0]);
            this.lv2NumObs = Convert.ToInt32(lineNumObsLvl[1]);

            //lv1Obs 
            this.lv1Obs = base.ReadObsTable(sr, this.lv1NumObs);
            //lv2Obs
            this.lv2Obs = base.ReadObsTable(sr, this.lv2NumObs);
            
            //SSBBReakVal
            line = sr.ReadLine();
            this.SSBBreakVal = Convert.ToInt32(line);
        }

        public override List<string> WriteRecruitmentDataModelData()
        {
            List<string> outputLines = new List<string>();
            outputLines.Add(this.lv1NumObs + new string(' ', 2) + this.lv2NumObs);

            outputLines.AddRange(this.WriteObsTableLines(this.lv1Obs, this.withSSB));
            outputLines.AddRange(this.WriteObsTableLines(this.lv2Obs, this.withSSB));

            outputLines.Add(this.SSBBreakVal.ToString());

            return outputLines;
        }

        public override ValidationResult ValidateInput()
        {

            if (this.HasBlankOrNullCells(this.lv1Obs) == false)
            {
                return new ValidationResult(false, "Missing Data in Level 1 observation table");
            }
            if (this.HasBlankOrNullCells(this.lv2Obs) == false)
            {
                return new ValidationResult(false, "Missing Data in Level 2 observation table");
            }

            if (this.TableHasAllSignificantValues(this.lv1Obs, this.lowBound) == false)
            {
                return new ValidationResult(false, "Insignificant values or values lower than "
                    + this.lowBound + " found in Level 1 observation table");
            }
            if (this.TableHasAllSignificantValues(this.lv2Obs, this.lowBound) == false)
            {
                return new ValidationResult(false, "Insignificant values or values lower than "
                    + this.lowBound + " found in Level 2 observation table");
            }

            return new ValidationResult(true, "Validation Successful");
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

        public override List<string> WriteRecruitmentDataModelData()
        {
            List<string> outputLine = new List<string>();
            outputLine.AddRange(base.WriteRecruitmentDataModelData());
            outputLine.Add(this.SSBHinge.ToString());
            return outputLine;
        }

        public override ValidationResult ValidateInput()
        {
            //SSB Hinge
            if (this.SSBHinge < 0.001)
            {
                return new ValidationResult(false,
                    "SSB Hinge Value is less than lower limit of 0.001");
            }
            
            return base.ValidateInput();
        }
    }

    /// <summary>
    /// Fixed Recruitment
    /// </summary>
    public class FixedEmpiricalRecruitment : EmpiricalRecruitment
    {
        public FixedEmpiricalRecruitment(int modelNum = 20)
            : base(modelNum)
        {
            this.subType = EmpiricalType.Fixed;
        }

        public override ValidationResult ValidateInput()
        {
            //TODO: Have FixedEmpiricalRecruitment check that number of row match 
            //number of years minus year one

            return base.ValidateInput();
        }

    }
}