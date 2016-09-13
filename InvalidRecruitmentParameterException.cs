using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace AGEPRO.CoreLib
{
    [Serializable]
    public class InvalidRecruitmentParameterException : Exception
    {
        public InvalidRecruitmentParameterException()
        {

        }
        public InvalidRecruitmentParameterException(string message)
            : base(message)
        {

        }
        public InvalidRecruitmentParameterException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
        public InvalidRecruitmentParameterException(SerializationInfo info, StreamingContext c)
            : base(info, c)
        {

        }
    }
}
