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
