using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace io.github.ba32107.Chrome.NativeMessaging.Internal
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    internal class StringArrayRegularExpressionAttribute : ValidationAttribute
    {
        private readonly Regex _regex;

        internal StringArrayRegularExpressionAttribute(string regexPattern)
            : base(() => "At least one of the strings in the array does not match the regular " +
                         $"expression '{regexPattern}'.")
        {
            _regex = new Regex(regexPattern, RegexOptions.Compiled);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string[] array)
            {
                return array.Any(str => str == null || !_regex.IsMatch(str))
                    ? new ValidationResult(ErrorMessage)
                    : ValidationResult.Success;
            }

            return new ValidationResult($"The '{nameof(StringArrayRegularExpressionAttribute)}' attribute must be applied on a string array property");
        }
    }
}