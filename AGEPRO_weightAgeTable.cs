using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace AGEPRO_struct
{
    /// <summary>
    /// Weight Specfic Stochastic Weights-at-age AGEPRO parameter class.
    /// </summary>
    public class AGEPRO_WeightAgeTable : AgeproStochasticAgeTable
    {
        public int weightOpt { get; set; }  //Weight Option
        public int [] validOpt { get; set;  } //Valid weight options

        public AGEPRO_WeightAgeTable()
        {
            validOpt = new int[] {1,0,-1,-2,-3,-4};
        }
        public AGEPRO_WeightAgeTable(int[] validOptions)
        {
            validOpt = validOptions;
        }

        /// <summary>
        /// Sets option for stochastic weights-at-age to either: inputted maunually, 
        /// read from the AGEPRO Input File, or use a valid weight option
        /// </summary>
        /// <param name="optParam">String Character from <paramref name="validOpt"/></param>
        protected override void SetInputAgeOption(string optParam)
        {
            if (optParam.Equals("0"))
            {
                this.fromFile = false; //0=User Spec by Age
                this.weightOpt = Convert.ToInt32(optParam);
            }
            else if (optParam.Equals("1"))
            {
                this.fromFile = true; //1=From File
                this.weightOpt = Convert.ToInt32(optParam);
            }
            else
            {
                this.fromFile = null;
                //Check if weightOpt is a valid one
                if (this.validOpt.Contains(Convert.ToInt32(optParam)))
                {
                    this.weightOpt = Convert.ToInt32(optParam);
                }
                else
                {
                    throw new InvalidOperationException("Weight option not valid for current Weights of Age Model");
                }
            }
        }

        /// <summary>
        /// Reads in AGEPRO Input File for the 'Read Weights From File' option 
        /// </summary>
        /// <param name="sr">AGEPRO Input File StreamReader</param>
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
