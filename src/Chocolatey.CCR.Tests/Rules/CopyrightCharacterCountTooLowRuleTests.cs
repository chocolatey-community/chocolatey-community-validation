namespace Chocolatey.CCR.Tests.Rules
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Chocolatey.CCR.Rules;
    using FluentAssertions;
    using NUnit.Framework;
    using static VerifyNUnit.Verifier;

    [Category("Requirements")]
    public class CopyrightCharacterCountTooLowRuleTests : RuleTestBase<CopyrightCharacterCountTooLowRule>
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("            ")]
        [TestCase("a")]
        [TestCase("abc")]
        [TestCase("  uba   ")]
        public async Task ShouldFlagWhenCopyrightIsBelow4Characters(string? copyright)
        {
            var testContent = GetContent(copyright);

            await VerifyNuspec(testContent);
        }

        [TestCase("2024")]
        [TestCase("Copyright Someone")]
        public async Task ShouldNotFlagWhenCopyrightIs4OrMoreCharacters(string copyright)
        {
            var testContent = GetContent(copyright);

            await VerifyEmptyResults(testContent);
        }

        [Test]
        public async Task ShouldNotFlagWhenCopyrightIsMissing()
        {
            var testContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Do not remove this test for UTF-8: if “Ω” doesn’t appear as greek uppercase omega letter enclosed in quotation marks, you should use an editor that supports UTF-8, not this one. -->
<package xmlns=""http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"">
  <metadata>
    <id>short-copyright</id>
    <version>1.0.0</version>
    <authors>Author</authors>
    <packageSourceUrl>https://test-url.com/</packageSourceUrl>
    <dependencies>
      <dependency id=""basic"" />
    </dependencies>
  </metadata>
  <files />
</package>";

            await VerifyEmptyResults(testContent);
        }

        [Test]
        public Task ShouldReturnAvailableRulesForImplementation()
        {
            return Verify(Rule.GetAvailableRules());
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionIfReaderIsNull()
        {
            Action act = () => Rule.Validate(null!).ToList();

            act.Should().Throw<ArgumentNullException>().WithParameterName("reader");
        }

        private static string GetContent(string? copyright)
        {
            const string format = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Do not remove this test for UTF-8: if “Ω” doesn’t appear as greek uppercase omega letter enclosed in quotation marks, you should use an editor that supports UTF-8, not this one. -->
<package xmlns=""http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"">
  <metadata>
    <id>short-copyright</id>
    <version>1.0.0</version>
    <authors>Author</authors>
    <packageSourceUrl>https://test-url.com/</packageSourceUrl>
    <copyright>{0}</copyright>
    <dependencies>
      <dependency id=""basic"" />
    </dependencies>
  </metadata>
  <files />
</package>";

            return string.Format(CultureInfo.InvariantCulture, format, copyright);
        }
    }
}
