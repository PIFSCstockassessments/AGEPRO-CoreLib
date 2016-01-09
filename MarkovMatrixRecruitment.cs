using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace AGEPRO_struct
{
    public class MarkovMatrixRecruitment : RecruitmentModel
    {
        public int numRecruitLevels { get; set; }
        public int numSSBLevels { get; set; }
        public DataTable recruitmentTable { get; set; }
        public DataTable SSBTable { get; set; }
        public DataTable probabilityTable { get; set; }

        public MarkovMatrixRecruitment()
        {
            this.recruitCategory = 4; //TODO: Check if MarkovMatrix Category is 4
        }

        public override void ReadRecruitmentModel(StreamReader sr)
        {
            string line;
            line = sr.ReadLine();
            string[] MarkovMatrixOptions = line.Split(' ');

            this.numRecruitLevels = Convert.ToInt32(MarkovMatrixOptions[0]);
            this.numSSBLevels = Convert.ToInt32(MarkovMatrixOptions[1]);

            //Recruitment
            DataTable inputTable = new DataTable();
            line = sr.ReadLine();
            string[] inputTableLine = line.Split(' ');
            for (int i=0; i < this.numRecruitLevels; i++)
            {
                inputTable.Rows.Add(Convert.ToInt32(inputTableLine[i]));
            }
            this.recruitmentTable = inputTable;
            
            //SSB
            inputTable = new DataTable();
            line = sr.ReadLine();
            inputTableLine = line.Split(' ');
            for (int i = 0; i < this.numSSBLevels; i++)
            {
                inputTable.Rows.Add(Convert.ToInt32(inputTableLine[i]));
            }
            this.SSBTable = inputTable;
            
            //Probalility
            inputTable = new DataTable();
            for (int i = 0; i < this.numSSBLevels; i++)
            {
                line = sr.ReadLine();
                inputTableLine = line.Split(' ');
                for (int j = 0; j < this.numRecruitLevels; j++)
                {
                    inputTable.Rows[i][j] = Convert.ToDouble(inputTableLine[j]);
                }
            }
            this.probabilityTable = inputTable;
        }

        
        
    }
}
