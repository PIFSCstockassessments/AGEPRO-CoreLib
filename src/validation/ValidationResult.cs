namespace Nmfs.Agepro.CoreLib
{
  public interface IValidatable
  {
    ValidationResult ValidateInput();
  }

  public class ValidationResult
  {
    public bool isValid { get; private set; }
    public string message { get; private set; }

    public ValidationResult(bool valid, string message)
    {
      this.isValid = valid;
      this.message = message;
    }

  }
}
