using System.ComponentModel.DataAnnotations;
using io.github.ba32107.Chrome.NativeMessaging.Internal;
using NUnit.Framework;

namespace io.github.ba32107.Chrome.NativeMessaging.Test.Internal
{
    internal class StringArrayRegularExpressionAttributeTest
    {
        private const string TestRegexPattern = @"^[A-Z][a-z][0-9]$";

        private static readonly string[][] ValidTestObjects =
        {
            new string[0],
            new [] { "Br5", "Ab1", "Yx4" },
            new [] { "Lo9", "Kq0", "Zi9" }
        };

        private static readonly string[][] InvalidTestObjects =
        {
            new [] { "Br5", "Ab1", "" },
            new [] { "Lo9", null, "Zi9" },
            new [] { "AA1", "Qc1", "Lk9" },
            new [] { "other test string" }
        };

        private StringArrayRegularExpressionAttribute _uut;

        [SetUp]
        public void SetUp()
        {
            _uut = new StringArrayRegularExpressionAttribute(TestRegexPattern);
        }

        [Test]
        public void TestValidationFailsForNonStringArray()
        {
            var result = PerformValidation(new object());
            VerifyValidationFailed(result, "attribute must be applied on a string array property");
        }

        [TestCaseSource(nameof(ValidTestObjects))]
        public void TestValidationSucceeds(string[] testObject)
        {
            var result = PerformValidation(testObject);
            Assert.AreEqual(ValidationResult.Success, result);
        }

        [TestCaseSource(nameof(InvalidTestObjects))]
        public void TestValidationFails(string[] testObject)
        {
            var result = PerformValidation(testObject);
            VerifyValidationFailed(result,
                "At least one of the strings in the array does not match the regular expression");
        }

        private ValidationResult PerformValidation(object testObject)
        {
            var validationContext = new ValidationContext(testObject);
            return _uut.GetValidationResult(testObject, validationContext);
        }

        private static void VerifyValidationFailed(ValidationResult result, string partialErrorMessage)
        {
            Assert.AreNotEqual(ValidationResult.Success, result);
            Assert.That(result.ErrorMessage, Does.Contain(partialErrorMessage));
        }
    }
}