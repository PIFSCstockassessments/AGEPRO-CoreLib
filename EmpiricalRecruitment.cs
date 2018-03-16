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
        private int _numObs;

        public DataTable obsTable { get; set; }
        public bool withSSB { get; set; }
        public EmpiricalType subType { get; set; }
        protected double lowBound { get; set; }

        public int numObs 
        {
            get { return _numObs; }
            set { SetProperty(ref _numObs, value); }
        }

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

        /// <summary>
        /// Gets the Observed Values DataTable from the input stream. 
        /// </summary>
        /// <param name="sr">AGEPRO Input File StreamReader</param>
        /// <param name="numObs">Number of Observations</param>
        /// <returns>Observed Values DataTable Object</returns>
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
        /// <param name="numObs">Number of Obervations</param>
        /// <param name="obsRecruits">Observations values vector</param>
        /// <param name="obsSSB">Spawning Stock Biomass (SSB) vector</param>
        /// <returns>Returns a DataTable with the Observed (and Spawning Stock Biomass) values</returns>
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

        /// <summary>
        /// Translates Empirical Recruitment and parameters into the
        /// AGEPRO input file data format.
        /// </summary>
        /// <returns>List of strings. Each string repesents a line from the input file.</returns>
        public override List<string> WriteRecruitmentDataModelData()
        {
            List<string> outputLines = new List<string>();
            outputLines.Add(this.numObs.ToString());
            
            //obsTable
            outputLines.AddRange(this.WriteObsTableLines(this.obsTable,this.withSSB));
            
            return outputLines;
        }

        /// <summary>
        /// Translates Observed Values data table object into the
        /// AGEPRO input file data format. 
        /// </summary>
        /// <param name="recruitObsTable">Recruitment Observation DataTable Object</param>
        /// <param name="hasSSBCols">DataTable has Spawning Stock Biomass (SSB) Columns</param>
        /// <returns>List of strings. Each string repesents a line from the input file.</returns>
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

        /// <summary>
        /// Checks the values in the Observed Values DataTable Object are valid.
        /// </summary>
        /// <returns>Vaildation Result Object</returns>
        public override ValidationResult ValidateInput()
        {
            if (this.HasBlankOrNullCells(this.obsTable))
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
        private int _lv1NumObs;
        private int _lv2NumObs;
        private int _SSBBreakVal;

        public DataTable lv1Obs { get; set; }
        public DataTable lv2Obs { get; set; }
        
        public int lv1NumObs 
        {
            get { return _lv1NumObs; }
            set { SetProperty(ref _lv1NumObs, value); }
        }
        public int lv2NumObs 
        {
            get { return _lv2NumObs; }
            set { SetProperty(ref _lv2NumObs, value); }
        }
        public int SSBBreakVal 
        {
            get { return _SSBBreakVal; }
            set { SetProperty(ref _SSBBreakVal, value); }
        }
        
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

        /// <summary>
        /// Reads in AGEPRO Input File Stream For Two-Stage Empirical Recruitment Specfic Parameters & Data
        /// </summary>
        /// <param name="sr">AGEPRO Input File StreamReader</param>
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

        /// <summary>
        /// Translates Two-Stage Empirical Recruitment input data and parameters into the
        /// AGEPRO input file data format.
        /// </summary>
        /// <returns>List of strings. Each string repesents a line from the input file.</returns>
        public override List<string> WriteRecruitmentDataModelData()
        {
            List<string> outputLines = new List<string>();
            outputLines.Add(this.lv1NumObs + new string(' ', 2) + this.lv2NumObs);

            outputLines.AddRange(this.WriteObsTableLines(this.lv1Obs, this.withSSB));
            outputLines.AddRange(this.WriteObsTableLines(this.lv2Obs, this.withSSB));

            outputLines.Add(this.SSBBreakVal.ToString());

            return outputLines;
        }

        /// <summary>
        /// Single stage Observation Value DataTable validation check.
        /// </summary>
        /// <param name="twoStageObsTable">Observation DataTable from single stage</param>
        /// <param name="tableName">Observation Table Name</param>
        /// <returns>
        /// If DataTable passes all validation checks, nothing will be returned.
        /// All validations not met will be recorded to a list of "Error Messages" to return.
        /// </returns>
        private List<string> CheckTwoStageObsTable(DataTable twoStageObsTable, string tableName)
        {
            List<string> errorMsgList = new List<string>();

            if (twoStageObsTable.Rows.Count <= 0)
            {
                errorMsgList.Add(tableName + " table has 0 rows");
            }

            if (this.HasBlankOrNullCells(twoStageObsTable))
            {
                errorMsgList.Add("Missing Data in "+ tableName +" table");
            }            
            else
            {
                if (this.TableHasAllSignificantValues(twoStageObsTable, this.lowBound) == false)
                {
                    errorMsgList.Add("Insignificant values or values lower than "
                        + this.lowBound + " found in " + tableName + " table");
                }
            }

            return errorMsgList;
        }

        /// <summary>
        /// Checks the values in the Observed Values DataTable Object are valid.
        /// </summary>
        /// <returns>Vaildation Result Object</returns>
        public override ValidationResult ValidateInput()
        {
            List<string> errorMsgList = new List<string>();

            errorMsgList.AddRange(CheckTwoStageObsTable(this.lv1Obs, "Level 1 Observation"));
            errorMsgList.AddRange(CheckTwoStageObsTable(this.lv2Obs, "Level 2 Observation"));
            if (string.IsNullOrWhiteSpace(this.SSBBreakVal.ToString()))
            {
                errorMsgList.Add("Missing SSB Break Value.");
            }

            var results = errorMsgList.EnumerateValidationResults();
            return results;
        }
    }

    /// <summary>
    /// Empirical CDF of Recruitment w/ Linear Decline to Zero.
    /// </summary>
    public class EmpiricalCDFZero : EmpiricalRecruitment
    {
        private double? _SSBHinge;

        public double? SSBHinge 
        {
            get { return _SSBHinge; }
            set { SetProperty(ref _SSBHinge, value); }
        }

        public EmpiricalCDFZero(int modelNum) : base(modelNum) 
        {
            this.subType = EmpiricalType.CDFZero;
            this.SSBHinge = 0;  //Fallback Default
        }

        /// <summary>
        /// Reads in AGEPRO Input File Stream For Empirical CDF Recruitment w/ Linear 
        /// Decline to Zero Specfic Parameters & Data 
        /// </summary>
        /// <param name="sr">AGEPRO Input File StreamReader</param>
        public override void ReadRecruitmentModel(StreamReader sr)
        {
            string line;

            //numObs and obsTable w/ base EmpiricalRecruitment function
            base.ReadRecruitmentModel(sr);

            //SSB Hinge (MT*1000)
            line = sr.ReadLine();
            this.SSBHinge = Convert.ToDouble(line);

        }

        /// <summary>
        /// Translates Empirical CDF Recruitment w/ Linear Decline to Zero input data 
        /// and parameters into the AGEPRO input file data format.
        /// </summary>
        /// <returns>List of strings. Each string repesents a line from the input file.</returns>
        public override List<string> WriteRecruitmentDataModelData()
        {
            List<string> outputLine = new List<string>();
            outputLine.AddRange(base.WriteRecruitmentDataModelData());
            outputLine.Add(this.SSBHinge.ToString());
            return outputLine;
        }

        /// <summary>
        /// Checks the values in the Observed Values DataTable Object are valid.
        /// </summary>
        /// <returns>Vaildation Result Object</returns>
        public override ValidationResult ValidateInput()
        {
            //SSB Hinge
            if (string.IsNullOrWhiteSpace(this.SSBHinge.ToString()))
            {
                return new ValidationResult(false, "Missing SSB Hinge Value");
            }
            else
            {
                if (this.SSBHinge < 0.001)
                {
                    return new ValidationResult(false,
                        "SSB Hinge Value is less than lower limit of 0.001");
                }
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

        /// <summary>
        /// Checks the values in the Observed Values DataTable Object are valid.
        /// </summary>
        /// <returns>Vaildation Result Object</returns>
        public override ValidationResult ValidateInput()
        {
            //Check that number of rows match umber of years minus year one
            if (this.obsTable.Rows.Count != (this.obsYears.Count() - 1))
            {
                return new ValidationResult(false, 
                    "Number of recruitment rows does not equal to the number of years minus year one.");
            }

            return base.ValidateInput();
        }

    }
}