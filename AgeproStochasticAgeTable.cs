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
                    ReadStochasticAgeTableFilename(sr);
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
        /// Reads in AGEPRO Input Filename for the 'Read Weights From File' option 
        /// </summary>
        /// <param name="sr">AGEPRO Input File StreamReader</param>
        protected virtual void ReadStochasticAgeTableFilename(StreamReader sr)
        {
            //Reads dataflie path
            string line = sr.ReadLine();
            this.dataFile = line;
        }

        /// <summary>
        /// Creates data line strings to append to the Data Writer for this Stochastic Parameter.
        /// </summary>
        /// <param name="keyword">Stochasatic Parameter name. Should be written in 
        /// all caps, and enclosed in square parenthesis. Example: [PARAMETER] </param>
        /// <returns>Returns a list of strings. </returns>
        public virtual List<string> WriteStochasticAgeDataLines(string keyword)
        {
            List<string> outputLines = new List<string>();

            outputLines.Add(keyword); //[PARAMETER]
            if (this.fromFile == true)
            {
                outputLines.Add("1" + new string(' ',2) + Convert.ToInt32(this.timeVarying).ToString());
                outputLines.Add(this.dataFile);
            }
            else
            {
                outputLines.Add("0" + new string(' ', 2) + Convert.ToInt32(this.timeVarying).ToString());
                foreach (DataRow yearRow in this.byAgeData.Rows)
                {
                    outputLines.Add(string.Join(new string(' ', 2), yearRow.ItemArray));
                }
                foreach (DataRow cvRow in this.byAgeCV.Rows)
                {
                    outputLines.Add(string.Join(new string(' ', 2), cvRow.ItemArray));
                }
            }
            
            return outputLines;
        }

        /// <summary>
        /// Reads in a Stochastic Table Files to memory. 
        /// </summary>
        /// <param name="fn">Stochastic Table Filename</param>
        /// <param name="ncol">Number of Data Columns. Typically it is the number of Age 
        /// classes. For mulit-fleet cases, it is: Number of ages * Number of fleets.</param>
        /// <returns>Returns a DataTable object.</returns>
        public static DataTable ReadStochasticTableFile(string fn, int ncol)
        {
            DataTable fromFileAgeTable = new DataTable("Age From File");
            string line;

            //Setup Columns
            for (int i = 0; i < ncol; i++)
            {
                fromFileAgeTable.Columns.Add( "Age " + (i + 1).ToString(), typeof(double) );
            }

            try
            {
                using (StreamReader inReader = new StreamReader(fn))
                {
                    while (!inReader.EndOfStream)
                    {
                        line = inReader.ReadLine();
                        string[] lineRow = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (lineRow.Count() == ncol)
                        {
                            throw new IOException("Stochastic Table File does not equal this "+
                                "stochastic parameter's number of columns: "+ ncol + 
                                "(Counted in file : " + lineRow.Count() + ")");
                        }
                        DataRow dr = fromFileAgeTable.NewRow();
                        dr.ItemArray = lineRow;
                        fromFileAgeTable.Rows.Add(dr);

                    }
                    
                }

            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return fromFileAgeTable;
        }
    }
}
