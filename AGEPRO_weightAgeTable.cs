using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace AGEPRO_struct
{
    public class AGEPRO_weightAgeTable : AGEPRO_inputAgeTable
    {
        public int weightOpt { get; set; }  //Weight Option
        public int [] validOpt { get; set;  }

        public AGEPRO_weightAgeTable()
        {
            validOpt = new int[] {-1,-2,-3,-4};
        }
        public AGEPRO_weightAgeTable(int[] validOptions)
        {
            validOpt = validOptions;
        }

        protected override void ReadInputAgeOption(string optParam)
        {
            if (optParam.Equals("1"))
            {
                this.fromFile = false; //1=User Spec by Age
                this.weightOpt = Convert.ToInt32(optParam);
            }
            else if (optParam.Equals("0"))
            {
                this.fromFile = true; //0=From File
                this.weightOpt = Convert.ToInt32(optParam);
            }
            else
            {
                if (this.validOpt.Contains(Convert.ToInt32(optParam)))
                {
                    this.weightOpt = Convert.ToInt32(optParam);
                }
                else
                {
                    //Throw warning/error
                }
            }
        }

        protected override void ReadInputAgeFromFileOption(System.IO.StreamReader sr)
        {
            //If Option 0=fromFile, read fromFile options
            //otherwise ignore and finish reading the current AGEPRO parameter
            if (this.weightOpt == 0) 
            {
                base.ReadInputAgeFromFileOption(sr);
            }
            
            
        }
            
    }
}
