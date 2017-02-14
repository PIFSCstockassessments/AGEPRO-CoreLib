using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nmfs.Agepro.CoreLib
{
    public enum ParametricType
    {
        Lognormal,
        Curve
    }

    /// <summary>
    /// Parametric Recruitment
    /// </summary>
    public class ParametricRecruitment : RecruitmentModel
    {
        private double? _phi;
        private double? _lastResidual;
        private bool _autocorrelated;

        public double? phi 
        {
            get { return _phi; }
            set { SetProperty(ref _phi, value); }
        }
        public double? lastResidual 
        {
            get { return _lastResidual; }
            set { SetProperty(ref _lastResidual, value); } 
        }
        public bool autocorrelated
        {
            get { return _autocorrelated; }
            set { SetProperty(ref _autocorrelated, value); }
        }
        public ParametricType subtype { get; set; }

        public ParametricRecruitment(int modelNum)
        {
            this.recruitModelNum = modelNum;
            this.recruitCategory = 2;
            this.autocorrelated = false;

        }
        public ParametricRecruitment(int modelNum, bool isAutocorrelated) : this(modelNum)
        {
            this.autocorrelated = isAutocorrelated;

            if (this.autocorrelated)
            {
                //If enabled, these values will be set to '0' (instead of null)
                this.lastResidual = lastResidual.GetValueOrDefault();
                this.phi = phi.GetValueOrDefault();
            }
        }


        public override void ReadRecruitmentModel(StreamReader sr)
        {
            throw new NotImplementedException();
        }

        public override List<string> WriteRecruitmentDataModelData()
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

        protected List<string> ValidateParametricParameter(double param, string paramName, 
            double significantBound = 0.000000001)
        {
            var msgList = new List<string>();

            if (string.IsNullOrWhiteSpace(param.ToString()))
            {
                msgList.Add("Missing or empty " + paramName + " value.");
            }
            else
            {
                if (Math.Abs(param) < significantBound)
                {
                    msgList.Add(paramName + " value is zero or less significant than " + significantBound + ".");
                }
            }
            return msgList;
        }


        protected List<string> ValidateParametricParameter(double? param, string paramName,
            double significantBound = 0.000000001)
        {
            var msgList = new List<string>();

            if (param != null)
            {
                msgList.AddRange(ValidateParametricParameter(param.Value, paramName, significantBound));
            }
            else
            {
                msgList.Add("Missing " + paramName + " value.");
            }

            return msgList;
        }
      
    }

    /// <summary>
    /// Parameteric Recruitment Using the Berverton-Holt, Ricker, or Shepherd Curve
    /// </summary>
    public class ParametricCurve : ParametricRecruitment
    {
        private double _alpha;
        private double _beta;
        private double _variance;
        private double? _kParm;
        public bool isShepherdCurve;

        public double alpha 
        {
            get { return _alpha; }
            set { SetProperty(ref _alpha, value); }
        }
        public double beta
        {
            get { return _beta; }
            set { SetProperty(ref _beta, value); }
        }
        public double variance 
        {
            get { return _variance; }
            set { SetProperty(ref _variance, value); }
        }
        public double? kParm 
        {
            get { return _kParm; }
            set { SetProperty(ref _kParm, value); }
        }

        public ParametricCurve(int modelNum, bool isAutocorrelated) : base(modelNum, isAutocorrelated) 
        {
            this.subtype = ParametricType.Curve;
            this.isShepherdCurve = (this.recruitModelNum == 7 || this.recruitModelNum == 12);
            
            if (this.isShepherdCurve)
            {
                //If enabled, K-parm will be set to '0' (instead of null)
                this.kParm = kParm.GetValueOrDefault(); 
            }
            
        }

        public override void ReadRecruitmentModel(StreamReader sr)
        {
            string line;
            line = sr.ReadLine();
            string[] parametricLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);


            if (parametricLine.Length == 3)
            {
                //Verify if this Parametric Model should have 3 parameters
                //Shepherd Curve Models do not have 3 parameters
                if (this.isShepherdCurve)
                {
                    throw new InvalidAgeproParameterException("3 Parameter Shepherd Curve Found");
                }
                this.alpha = Convert.ToDouble(parametricLine[0]);
                this.beta = Convert.ToDouble(parametricLine[1]);
                this.variance = Convert.ToDouble(parametricLine[2]);
                this.kParm = null; //TODO:Leave null value?
            }
            else if (parametricLine.Length == 4)
            {
                //Verify if this Parametric Model should have 4 parameters
                //Only Shepherd Models have 4 parameters.
                if(!this.isShepherdCurve){
                    throw new InvalidAgeproParameterException("Beverton-Holt or Ricker Curve with 4 parameters found");
                }
                this.alpha = Convert.ToDouble(parametricLine[0]);
                this.beta = Convert.ToDouble(parametricLine[1]);
                this.kParm = Convert.ToDouble(parametricLine[2]);
                this.variance = Convert.ToDouble(parametricLine[3]);
            }
            else
            {
                //Throw error
                throw new System.ArgumentOutOfRangeException("Number of Parmetric parameters", parametricLine.Length, 
                    "Parmetric Curve must have 3 or 4 parameters.");
            }

            if (this.autocorrelated)
            {
                ReadAutocorrelatedValues(sr);
            }
        }

        

        public override List<string> WriteRecruitmentDataModelData()
        {
            List<string> outputLines = new List<string>();

            if (this.isShepherdCurve)
            {
                outputLines.Add(this.alpha.ToString().PadRight(12) +
                    this.beta.ToString().PadRight(12) +
                    this.kParm.ToString().PadRight(12) +
                    this.variance.ToString().PadRight(12));
            }
            else
            {
                outputLines.Add(this.alpha.ToString().PadRight(12) +
                    this.beta.ToString().PadRight(12) + 
                    this.variance.ToString().PadRight(12));
            }

            if (this.autocorrelated)
            {
                outputLines.Add(this.phi.ToString().PadRight(12) + this.lastResidual.ToString().PadRight(12));
            }

            return outputLines;
        }


        public override ValidationResult ValidateInput()
        {
            var msgList = new List<string>();
            
            msgList.AddRange(ValidateParametricParameter(this.alpha, "Alpha"));
            msgList.AddRange(ValidateParametricParameter(this.beta, "Beta"));
            msgList.AddRange(ValidateParametricParameter(this.variance, "Variance"));

            if (this.isShepherdCurve)
            {
                msgList.AddRange(ValidateParametricParameter(this.kParm, "KParm"));
            }
           
            if (this.autocorrelated)
            {
                msgList.AddRange(ValidateParametricParameter(this.phi, "Phi"));
                msgList.AddRange(ValidateParametricParameter(this.lastResidual, 
                    "Last Residual"));
            }
            var results = msgList.EnumerateValidationResults();

            return results;
        }
    }

    public class ParametricShepherdCurve : ParametricCurve
    {
        private double _kParm;

        public new double kParm   //TODO:Remove Base Class kParm field when finished implmenting this class.
        {
            get { return _kParm; }
            set { SetProperty(ref _kParm, value); }
        }

        public ParametricShepherdCurve(int modelNum, bool isAutocorrelated)
            : base(modelNum, isAutocorrelated)
        {
            
        }

        public override void ReadRecruitmentModel(StreamReader sr)
        {
            string line;
            line = sr.ReadLine();
            string[] parametricLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            //Check parametricLine was split into 4 parameters. Only Shepherd Models have 4 parameters.
            if (parametricLine.Length != 4)
            {
                throw new InvalidAgeproParameterException("Shepherd Curve must have 4 parameters." + 
                    Environment.NewLine + "Number of parameters found: " + parametricLine.Length);
            }

            this.alpha = Convert.ToDouble(parametricLine[0]);
            this.beta = Convert.ToDouble(parametricLine[1]);
            this.kParm = Convert.ToDouble(parametricLine[2]);
            this.variance = Convert.ToDouble(parametricLine[3]);
            
        }

        public override List<string> WriteRecruitmentDataModelData()
        {
            List<string> outputLines = new List<string>();

            outputLines.Add(this.alpha.ToString().PadRight(12) +
                this.beta.ToString().PadRight(12) +
                this.kParm.ToString().PadRight(12) +
                this.variance.ToString().PadRight(12));

            if (this.autocorrelated)
            {
                outputLines.Add(this.phi.ToString().PadRight(12) + this.lastResidual.ToString().PadRight(12));
            }
            return outputLines;
        }

        public override ValidationResult ValidateInput()
        {
            var msgList = new List<string>();

            msgList.AddRange(ValidateParametricParameter(this.alpha, "Alpha"));
            msgList.AddRange(ValidateParametricParameter(this.beta, "Beta"));
            msgList.AddRange(ValidateParametricParameter(this.kParm, "KParm"));
            msgList.AddRange(ValidateParametricParameter(this.variance, "Variance"));

            if (this.autocorrelated)
            {
                msgList.AddRange(ValidateParametricParameter(this.phi, "Phi"));
                msgList.AddRange(ValidateParametricParameter(this.lastResidual, "Last Residual"));
            }
            var results = msgList.EnumerateValidationResults();

            return results;
        }
    }

    /// <summary>
    /// Parmetric Recruitment in Lognormal Distribution
    /// </summary>
    public class ParametricLognormal : ParametricRecruitment
    {
        private double _mean;
        private double _stdDev;

        public double mean 
        {
            get { return _mean; }
            set { SetProperty(ref _mean, value); }
        }
        public double stdDev 
        {
            get { return _stdDev; }
            set { SetProperty(ref _stdDev, value); }
        }

        public ParametricLognormal(int modelNum, bool isAutocorrelated) : base(modelNum, isAutocorrelated) 
        {
            this.subtype = ParametricType.Lognormal;
        }

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

        public override List<string> WriteRecruitmentDataModelData()
        {
            List<string> outputLines = new List<string>();
            outputLines.Add(this.mean.ToString().PadRight(12) + this.stdDev.ToString().PadRight(12));
            if (this.autocorrelated)
            {
                outputLines.Add(this.phi.ToString().PadRight(12) + this.lastResidual.ToString().PadRight(12));
            }
            return outputLines;
        }

        public override ValidationResult ValidateInput()
        {
            List<string> msgList = new List<string>();

            msgList.AddRange(ValidateParametricParameter(this.mean, "Mean"));
            msgList.AddRange(ValidateParametricParameter(this.stdDev, "Std. Deviaition"));

            if (this.autocorrelated)
            {
                msgList.AddRange(ValidateParametricParameter(this.phi.Value, "Phi"));
                msgList.AddRange(ValidateParametricParameter(this.lastResidual.Value,
                    "Last Residual"));
            }
            var results = msgList.EnumerateValidationResults();

            return results;
        }
    }
}
