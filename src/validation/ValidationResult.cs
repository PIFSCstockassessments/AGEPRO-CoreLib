namespace Nmfs.Agepro.CoreLib
{
  public interface IValidatable
  {
    ValidationResult ValidateInput();
  }

  public class ValidationResult
  {
    public bool IsValid { get; private set; }
    public string Message { get; private set; }

    public ValidationResult(bool valid, string message)
    {
      IsValid = valid;
      Message = message;
    }

  }
}
