using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Nmfs.Agepro.CoreLib
{
    [Serializable]
    public class InvalidAgeproParameterException : Exception
    {
        public InvalidAgeproParameterException()
        {

        }
        public InvalidAgeproParameterException(string message)
            : base(message)
        {

        }
        public InvalidAgeproParameterException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
        public InvalidAgeproParameterException(SerializationInfo info, StreamingContext c)
            : base(info, c)
        {

        }
    }
}
