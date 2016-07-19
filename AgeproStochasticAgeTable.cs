using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace AGEPRO.CoreLib
{
    /// <summary>
    /// Generalized Stochastic Weights-at-age AGEPRO parameter class.
    /// </summary>
    public class AgeproStochasticAgeTable 
    {
        public bool? fromFile { get; set; }
        public bool timeVarying { get; set; }
        public DataTable byAgeData { get; set; }
        public DataTable byAgeCV { get; set; }
        public string dataFile { get; set; }
        public int numYears { get; set; }
        public int numFleets { get; set; }

        public AgeproStochasticAgeTable()
        {
          
        }
        public AgeproStochasticAgeTable(string file) 
        {

        }
        
        /// <summary>
        /// Read in AGEPRO Input Data File for stochastic Weights-at-age AGEPRO Parameters 
        /// </summary>
        /// <param name="sr">AGEPRO Input Data File StreamReader</param>
        /// <param name="numYears">Number of Years in Projection</param>
        /// <param name="numAges">Number of Ages in Projection</param>
        /// <param name="numFleets">Number of Fleets. Default to <c>1</c></param>
        public void ReadStochasticAgeData(StreamReader sr, int numYears, int numAges, int numFleets = 1)
        {
            
            this.numYears = numYears;
            this.numFleets = numFleets; //Defaults to 1 for Non fleet-dependent InputAge Data
            int N;

            //Input Age Option
            string line = sr.ReadLine();
            string[] swLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string optStock = swLine[0];
            SetStochasticAgeOption(optStock);  //Input Age Option may vary if this a weightAgeTable

            //Time Varying
            string optTimeVarying = swLine[1];
            if (optTimeVarying.Equals("1"))
            {
                this.timeVarying = true;
            }
            else
            {
                this.timeVarying = false;
            }

            //check fromFile has non-null value. Do nothing if so.
            if (this.fromFile.HasValue)
            {
                //Create Ages Header
                string[] ageHeader = new string[numAges];
                for (int i = 0; i < numAges; i++)
                {
                    ageHeader[i] = "Age " + (i + 1).ToString();
                }


                if (!(bool)this.fromFile) //User spec by user
                {
                    //byAge
                    DataTable ageTable = new DataTable("Age Value");
                    if (this.timeVarying)
                    {
                        N = this.numYears * numFleets;
                    }
                    else
                    {
                        N = numFleets;
                    }

                    //Set Age Columns for ageTable
                    foreach(var nage in ageHeader){
                        ageTable.Columns.Add(nage, typeof(double));
                    }

                    //TODO:Rowheader/add-'Fleet-Year Column' to DataTable??
                    //i:N rows (years (and fleets))
                    for (int i = 0; i < N; i++)
                    {
                        line = sr.ReadLine();
                        DataRow dr = ageTable.NewRow();
                        string[] ageLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);   
                        ageTable.Rows.Add(ageLine);
                    }
                    this.byAgeData = ageTable;
                    
                    //byCV
                    DataTable cvTable = new DataTable("Age CV");

                    //Set Age Columns for cvTable
                    foreach (var nage in ageHeader)
                    {
                        cvTable.Columns.Add(nage, typeof(double));
                    }

                    for (int K = 0; K < numFleets; K++)
                    {
                        line = sr.ReadLine();
                        DataRow dr = cvTable.NewRow();
                        string[] cvLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        for (int j = 0; j < numAges; j++)
                        {
                            dr[j] = cvLine[j];
                        }
                        cvTable.Rows.Add(dr);
                    }
                    this.byAgeCV = cvTable;
                }
                else
                {
                    //From File: Reads filepath string
                    ReadStochasticAgeFromFileOption(sr);
                }
            }

        }

        /// <summary>
        /// Sets option for stochastic weights-at-age to be inputted maunually or read from the AGEPRO Input File
        /// </summary>
        /// <param name="optParam">String Character with '0' or '1'</param>
        protected virtual void SetStochasticAgeOption(string optParam)
        {
            if (optParam.Equals("0"))
            {
                this.fromFile = false; //User Spec by Age
            }
            else if (optParam.Equals("1"))
            {
                this.fromFile = true; //From File   
            }

        }

        /// <summary>
        /// Reads in AGEPRO Input File for the 'Read Weights From File' option 
        /// </summary>
        /// <param name="sr">AGEPRO Input File StreamReader</param>
        protected virtual void ReadStochasticAgeFromFileOption(StreamReader sr)
        {
            //Reads dataflie path
            string line = sr.ReadLine();
            this.dataFile = line;
        }
    }
}
