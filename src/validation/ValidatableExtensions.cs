﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Nmfs.Agepro.CoreLib
{
  //poco-validation
  public static class ValidatableExtensions
  {
    /// <summary>
    /// Returns bool value of the Validation Check 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsValid(this IValidatable input)
    {
      return input.ValidationCheck().IsValid;
    }

    /// <summary>
    /// Retruns Message of the Vaildation Check
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ValidationMessage(this IValidatable input)
    {
      return input.ValidationCheck().Message;
    }

    /// <summary>
    /// Null check for nullable validatable objects 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static ValidationResult ValidationCheck(this IValidatable input)
    {
      if (input == null)
      {
        return new ValidationResult(false, "Null or Missing input.");
      }
      else
      {
        return input.ValidateInput();
      }

    }

    /// <summary>
    /// Centralizes boilerplate needed to convert list of errors into single string.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static ValidationResult EnumerateValidationResults(this IEnumerable<string> input)
    {
      if (input == null)
      {
        return new ValidationResult(false, "Null or Missing input.");
      }

      //Enumerate the Errors
      var errors = input.ToList();
      var success = !errors.Any();
      string message;
      if (success)
      {
        message = "Validation Sucessful";
      }
      else
      {
        message = string.Join(Environment.NewLine, errors);
      }

      return new ValidationResult(success, message);
    }

  }
}
