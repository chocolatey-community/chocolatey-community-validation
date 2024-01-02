namespace Chocolatey.Community.Validation.Tests.Rules
{
    using System.Globalization;
    using System.Threading.Tasks;
    using Chocolatey.Community.Validation.Rules;
    using NUnit.Framework;

    public class IdElementRulesTests : RuleTestBase<IdElementRules>
    {
        [TestCase("testalpha")]
        [TestCase("test-ALPha")]
        [TestCase("testBETA")]
        [TestCase("beta-test")]
        [TestCase("prerelease")]
        [TestCase("my prerelease")]
        [TestCase("my-package.CONFIG")]
        [TestCase("pkg.config")]
        public async Task ShouldFlagIdentifier(string id)
        {
            var testContent = GetTestContent(id);

            await VerifyNuspec(testContent);
        }

        [Test]
        public async Task ShouldNotFlagIdentifierWithoutPrereleaseName()
        {
            var testContent = GetTestContent("some-id");

            await VerifyEmptyResults(testContent);
        }

        [Test]
        public async Task ShouldNotFlagAnEmptyIdentifier()
        {
            var testContent = GetTestContent(string.Empty);

            await VerifyEmptyResults(testContent);
        }

        private static string GetTestContent(string? id)
        {
            const string format = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Do not remove this test for UTF-8: if “Ω” doesn’t appear as greek uppercase omega letter enclosed in quotation marks, you should use an editor that supports UTF-8, not this one. -->
<package xmlns=""http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"">
  <metadata>
    <id>{0}</id>
    <version>1.0.0</version>
    <authors>Author</authors>
    <packageSourceUrl>https://test-url.com/</packageSourceUrl>
    <copyright>Copyright 2023</copyright>
    <dependencies>
      <dependency id=""basic"" />
    </dependencies>
  </metadata>
  <files />
</package>";

            return string.Format(CultureInfo.InvariantCulture, format, id);
        }
    }
}
