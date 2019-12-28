using System.ComponentModel.DataAnnotations;
using NUnit.Framework;

namespace io.github.ba32107.Chrome.NativeMessaging.Test
{
    internal class NativeMessagingHostManifestTest
    {
        private static readonly string[][] ValidAllowedOriginTestData =
        {
            new [] { "chrome-extension://abc123/" }, new [] { "chrome-extension://abc123/", "def456" }
        };

        private static readonly string[][] InvalidAllowedOriginTestData =
        {
            null, new string[0], new [] { "", null }, new [] { "chrome-extension:abc123" }
        };

        private NativeMessagingHostManifest _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new NativeMessagingHostManifest
            {
                Name = "name",
                Description = "description",
                Path = "path",
                AllowedOrigins = new []
                {
                    "origin"
                }
            };
        }

        [TestCase("abc123")]
        [TestCase("abc.123")]
        [TestCase("abc.123.def.456")]
        [TestCase("abc_123_def_456")]
        [TestCase("_abc.123_")]
        public void TestValidName(string name)
        {
            _uut.Name = name;
            VerifyValidationSucceeds();
        }

        [TestCase("ABC123")]
        [TestCase("abc%123")]
        [TestCase("a#b$c123")]
        [TestCase(".abc123")]
        [TestCase("abc123.")]
        [TestCase("abc..123")]
        [TestCase("")]
        [TestCase(null)]
        public void TestInvalidName(string name)
        {
            _uut.Name = name;
            VerifyValidationFails();
        }

        [TestCase("")]
        [TestCase(null)]
        public void TestDescriptionIsRequired(string description)
        {
            _uut.Description = description;
            VerifyValidationFails();
        }

        [TestCase("")]
        [TestCase(null)]
        public void TestPathIsRequired(string path)
        {
            _uut.Path = path;
            VerifyValidationFails();
        }

        [TestCaseSource(nameof(ValidAllowedOriginTestData))]
        public void TestValidAllowedOrigins(string[] allowedOrigins)
        {
            _uut.AllowedOrigins = allowedOrigins;
            VerifyValidationSucceeds();
        }

        [TestCaseSource(nameof(InvalidAllowedOriginTestData))]
        public void TestInvalidAllowedOrigins(string[] allowedOrigins)
        {
            _uut.AllowedOrigins = allowedOrigins;
            VerifyValidationFails();
        }

        [Test]
        public void TestTypeIsSetByDefault()
        {
            Assert.AreEqual("stdio", _uut.Type);
        }

        private void VerifyValidationSucceeds()
        {
            var ctx = new ValidationContext(_uut);
            Assert.DoesNotThrow(() => Validator.ValidateObject(_uut, ctx, true));
        }
        private void VerifyValidationFails()
        {
            var ctx = new ValidationContext(_uut);
            Assert.Throws<ValidationException>(() => Validator.ValidateObject(_uut, ctx, true));
        }
    }
}