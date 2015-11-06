using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace AGEPRO_struct
{
    public class AGEPRO_inputAgeTable : AGEPRO_input
    {
        public bool? fromFile { get; set; }
        public bool timeVarying { get; set; }
        public DataTable byAgeData { get; set; }
        public DataTable byAgeCV { get; set; }
        public string dataFile { get; set; }

        public AGEPRO_inputAgeTable()
        {
          
        }
        public AGEPRO_inputAgeTable(string file) 
        {

        }
        
        public void ReadInputAgeData(StreamReader sr, int numYears, int numAges, int numFleets = 1)
        {
            
            this.nYears = numYears;
            this.nFleets = numFleets; //Defaults to 1 for Non fleet-dependent InputAge Data
            int N;

            //Create Ages Header?
            string [] ageHeader = new string[numAges];
            for (int i = 0; i < numAges; i++)
            {
                ageHeader[i] = "Age " + (i+1).ToString();
            }

            //Input Age Option
            string line = sr.ReadLine();
            string[] swLine = line.Split(' ');
            string optStock = swLine[0];
            ReadInputAgeOption(optStock);  //Input Age Option may vary if this a weightAgeTable

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
                if (!(bool)this.fromFile) //User spec by user
                {
                    DataTable ageTable = new DataTable();
                    if (this.timeVarying)
                    {
                        N = this.nYears * numFleets;
                    }
                    else
                    {
                        N = numFleets;
                    }
                    //byAge
                    for (int i = 0; i < N; i++)
                    {
                        line = sr.ReadLine();
                        DataRow dr = ageTable.NewRow();
                        string[] ageLine = line.Split(' ');
                        for (int j = 0; j < ageLine.Length; j++)
                        {
                            dr[j] = ageLine[j];
                        }
                        ageTable.Rows.Add(dr);
                    }
                    this.byAgeData = ageTable;
                    //CV
                    DataTable cvTable = new DataTable();
                    for (int K = 0; K < numFleets; K++)
                    {
                        line = sr.ReadLine();
                        DataRow dr = cvTable.NewRow();
                        string[] cvLine = line.Split(' ');
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
                    //From File
                    ReadInputAgeFromFileOption(sr);
                }
            }

        }

        protected virtual void ReadInputAgeOption(string optParam)
        {
            if (optParam.Equals("1"))
            {
                this.fromFile = false; //User Spec by Age
            }
            else
            {
                this.fromFile = true; //From File
            }

        }

        protected virtual void ReadInputAgeFromFileOption(StreamReader sr)
        {
            string line = sr.ReadLine();
            this.dataFile = line;
        }
    }
}
