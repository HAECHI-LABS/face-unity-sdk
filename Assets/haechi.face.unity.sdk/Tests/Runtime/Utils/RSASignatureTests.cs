using System;
using haechi.face.unity.sdk.Runtime.Utils;
using NUnit.Framework;

namespace haechi.face.unity.sdk.Tests.Runtime.Utils
{
    public class RSASignatureVerifierTests
    {
        [Test]
        public void VerifyTest()
        {
            bool result = RSASignatureVerifier.Verify("hello",
                "D2Y3OU9cEIkxML_Aghd4tcSl4oR3309BongVK6kfcjuokQYngDvLlQMN42MhwDjDwSe7jrK5RT7T2gsl5Jnobr-EcLaS2jvfMhS8Csk-5wtGDixGFP7HgLJpFRaUMisnD2zaVyPVFJi1XeRYFpTPSVZOOZSkfLpeFP7zPaOI3M0=",
                "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDLYvQgQYet97bJATHnaGIm1mieaKA8vgsnc1L0ssn-nerBFBgGCMMXl1YpHBsAIaZWbfsNnJHhLj-C81Y5wF2xjPlDSpsjQjTt9hmKkGS0qFdYJ14aWiqkkwfFewglKdxDJ7d4Uh-DX6wg_xDKAKNgZM8flF0YdWApLrQWtMQ0eQIDAQAB");
            Assert.IsTrue(result);
        }
    }
}