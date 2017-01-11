using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return input.ValidationCheck().isValid;
        }

        /// <summary>
        /// Retruns Message of the Vaildation Check
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ValidationMessage(this IValidatable input)
        {
            return input.ValidationCheck().message;
        }   

        /// <summary>
        /// Call to validation function w/ bulit in null check 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static ValidationResult ValidationCheck(this IValidatable input)
        {
            if (input != null)
            {
                return input.Validate();
            }
            else
            {
                return new ValidationResult(false, "Null or Missing input");
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
                return new ValidationResult(false, "Null or Missing input");
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
                message = string.Join(" ", errors);
            }

            return new ValidationResult(success, message);
        }

    }
}
