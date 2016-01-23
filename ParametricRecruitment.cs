using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace AGEPRO_struct
{
    public class ParametricRecruitment : RecruitmentModel
    {
        public double? phi { get; set; }
        public double? lastResidual { get; set; }
        public bool autocorrelated { get; set; }
        
        public ParametricRecruitment(int modelNum)
        {
            this.recruitModelNum = modelNum;
            this.recruitCategory = 2;
            this.autocorrelated = false;  
        }
        public ParametricRecruitment(int modelNum, bool isAutocorrelated) : this(modelNum)
        {
            this.autocorrelated = isAutocorrelated;
        }
        public override void ReadRecruitmentModel(StreamReader sr)
        {
            throw new NotImplementedException();
        }


        protected void ReadAutocorrelatedValues(StreamReader sr)
        {
            string line;
            line = sr.ReadLine();
            string[] autoCorrLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            this.phi = Convert.ToDouble(autoCorrLine[0]);
            this.lastResidual = Convert.ToDouble(autoCorrLine[1]);
        }

        
        
    }

    public class ParametricCurve : ParametricRecruitment
    {
        public double alpha { get; set; }
        public double beta { get; set; }
        public double variance { get; set; }
        public double? kParm { get; set; }

        public ParametricCurve(int modelNum, bool isAutocorrelated) : base(modelNum, isAutocorrelated) { }

        public override void ReadRecruitmentModel(StreamReader sr)
        {
            string line;
            line = sr.ReadLine();
            string[] parametricLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);


            if (parametricLine.Length == 3)
            {
                //TODO: Verify if this Parametric Model should have 3 parameters
                this.alpha = Convert.ToDouble(parametricLine[0]);
                this.beta = Convert.ToDouble(parametricLine[1]);
                this.variance = Convert.ToDouble(parametricLine[2]);
                this.kParm = null; //TODO:Leave null value?
            }
            else if (parametricLine.Length == 4)
            {
                //TODO: Verify if this Parametric Model should have 4 parameters 
                this.alpha = Convert.ToDouble(parametricLine[0]);
                this.beta = Convert.ToDouble(parametricLine[1]);
                this.kParm = Convert.ToDouble(parametricLine[2]);
                this.variance = Convert.ToDouble(parametricLine[3]);
            }
            else
            {
                //throw error
            }

            if (this.autocorrelated)
            {
                ReadAutocorrelatedValues(sr);
            }
        }
    }

    public class ParametricLognormal : ParametricRecruitment
    {
        public double mean { get; set; }
        public double stdDev { get; set; }

        public ParametricLognormal(int modelNum, bool isAutocorrelated) : base(modelNum, isAutocorrelated) { }

        public override void ReadRecruitmentModel(StreamReader sr)
        {
            string line;
            line = sr.ReadLine();
            string[] logParamLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            this.mean = Convert.ToDouble(logParamLine[0]);
            this.stdDev = Convert.ToDouble(logParamLine[1]);

            if (this.autocorrelated)
            {
                ReadAutocorrelatedValues(sr);
            }
        }
    }
}
