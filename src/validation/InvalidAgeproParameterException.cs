using System;
using System.Runtime.Serialization;

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
