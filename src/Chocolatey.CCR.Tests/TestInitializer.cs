using NUnit.Framework;

// This is just to force the english culture to be
// used even on non-english systems.
[assembly: SetCulture("en")]
[assembly: SetUICulture("en")]

namespace Chocolatey.CCR.Tests
{
    using NUnit.Framework;
    using VerifyTests;

    [SetUpFixture]
    public class TestInitializer
    {
        [OneTimeSetUp]
        public void Initialize()
        {
            ClipboardAccept.Enable();
        }
    }
}
