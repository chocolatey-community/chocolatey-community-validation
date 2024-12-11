namespace Chocolatey.Community.Validation.Tests.Rules
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Chocolatey.Community.Validation.Rules;
    using NUnit.Framework;
    using VerifyNUnit;

    public class IconUrlElementRulesTest : RuleTestBase<IconUrlElementRules>
    {
        [TestCase(nameof(InvalidUrlValues))]
        public async Task ShouldNotFlagInvalidUrls(string value)
        {
            var testContent = GetTestContent(value);

            await VerifyEmptyResults(testContent);
        }

        [TestCaseSource(nameof(EmptyTestValues))]
        public async Task ShouldNotFlagEmptyValues(string value)
        {
            var testContent = GetTestContent(value);

            await VerifyEmptyResults(testContent);
        }

        [TestCase("https://github.com/chocolatey-community/chocolatey-packages/blob/master/icons/7zip.svg")]
        [TestCase("https://github.com/chocolatey-community/chocolatey-packages/raw/refs/heads/master/icons/filezilla.svg")]
        [TestCase("https://raw.githubusercontent.com/chocolatey-community/chocolatey-packages/refs/heads/master/icons/1password4.png")]
        public async Task ShouldFlagUrlsUsingGitHubLinks(string value)
        {
            var testContent = GetTestContent(value);

            var results = GetRuleResults(testContent, Encoding.UTF8);

            await Verifier.Verify(results)
                // We ignore the parameter value, as it will result in
                // failure due to long paths not being supported.
                .IgnoreParametersForVerified(nameof(value))
                .DisableRequireUniquePrefix();
        }

        [Test]
        public async Task ShouldFlagUrlsUsingRawGitLinks()
        {
            var testContent = GetTestContent("https://cdn.rawgit.com/chocolatey/chocolatey-coreteampackages/049a3a3d/icons/winff.png");

            await VerifyNuspec(testContent);
        }

        private static string GetTestContent(string? iconUrl)
        {
            const string format = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Do not remove this test for UTF-8: if “Ω” doesn’t appear as greek uppercase omega letter enclosed in quotation marks, you should use an editor that supports UTF-8, not this one. -->
<package xmlns=""http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"">
  <metadata>
    <id>short-copyright</id>
    <version>1.0.0</version>
    <authors>Author</authors>
    <iconUrl>{0}</iconUrl>
    <packageSourceUrl>https://test-url.com/</packageSourceUrl>
    <tags>tag-1 tag-2 tag-3</tags>
    <dependencies>
      <dependency id=""basic"" />
    </dependencies>
  </metadata>
  <files />
</package>";

            return string.Format(CultureInfo.InvariantCulture, format, iconUrl);
        }
    }
}
