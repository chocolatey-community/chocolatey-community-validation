namespace Chocolatey.CCR.Tests.Rules
{
    using System.Threading.Tasks;
    using Chocolatey.CCR.Rules;
    using NUnit.Framework;

    [Category("Requirements")]
    public class RequireLicenceAcceptanceElementRulesTests : RuleTestBase<RequireLicenseAcceptanceElementRules>
    {
        [Test]
        public async Task ShouldFlagWhenLicenseAcceptanceIsTrueAndLicenseUrlIsMissing()
        {
            const string testContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Do not remove this test for UTF-8: if “Ω” doesn’t appear as greek uppercase omega letter enclosed in quotation marks, you should use an editor that supports UTF-8, not this one. -->
<package xmlns=""http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"">
  <metadata>
    <id>short-copyright</id>
    <version>1.0.0</version>
    <authors>Author</authors>
    <packageSourceUrl>https://test-url.com/</packageSourceUrl>
    <copyright>Copyright 2023</copyright>
    <requireLicenseAcceptance>true</requireLicenseAcceptance>
    <dependencies>
      <dependency id=""basic"" />
    </dependencies>
  </metadata>
  <files />
</package>";

            await VerifyNuspec(testContent);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("     ")]
        public async Task ShouldFlagWhenLicenseAcceptanceIsTrueAndNoLicenseUrl(string value)
        {
            var testContent = GetTestContent(value, true);

            await VerifyNuspec(testContent);
        }

        [Test]
        public async Task ShouldNotFlagWhenLicenseAcceptanceIsFalseAndLicenseUrlIsMissing()
        {
            const string testContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Do not remove this test for UTF-8: if “Ω” doesn’t appear as greek uppercase omega letter enclosed in quotation marks, you should use an editor that supports UTF-8, not this one. -->
<package xmlns=""http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"">
  <metadata>
    <id>short-copyright</id>
    <version>1.0.0</version>
    <authors>Author</authors>
    <packageSourceUrl>https://test-url.com/</packageSourceUrl>
    <copyright>Copyright 2023</copyright>
    <requireLicenseAcceptance>false</requireLicenseAcceptance>
    <dependencies>
      <dependency id=""basic"" />
    </dependencies>
  </metadata>
  <files />
</package>";

            await VerifyEmptyResults(testContent);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("     ")]
        public async Task ShouldNotFlagWhenLicenseAcceptanceIsFalseAndNoLicenseUrl(string value)
        {
            var testContent = GetTestContent(value, false);

            await VerifyEmptyResults(testContent);
        }

        [Test]
        public async Task ShouldNotFlagWhenLicenseIsProvided([Values] bool requireLicenseAcceptance)
        {
            var testContent = GetTestContent("https://license.com/my-license", requireLicenseAcceptance);

            await VerifyEmptyResults(testContent);
        }

        [Test]
        public async Task ShouldNotFlagWhenRequireLicenseAcceptanceAndLicenseUrlIsMissing()
        {
            const string testContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Do not remove this test for UTF-8: if “Ω” doesn’t appear as greek uppercase omega letter enclosed in quotation marks, you should use an editor that supports UTF-8, not this one. -->
<package xmlns=""http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"">
  <metadata>
    <id>short-copyright</id>
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

            await VerifyEmptyResults(testContent);
        }

        private string GetTestContent(string licenseUrl, bool requireAcceptance)
        {
            const string format = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Do not remove this test for UTF-8: if “Ω” doesn’t appear as greek uppercase omega letter enclosed in quotation marks, you should use an editor that supports UTF-8, not this one. -->
<package xmlns=""http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"">
  <metadata>
    <id>short-copyright</id>
    <version>1.0.0</version>
    <authors>Author</authors>
    <packageSourceUrl>https://test-url.com/</packageSourceUrl>
    <copyright>Some Copyright</copyright>
    <requireLicenseAcceptance>{0}</requireLicenseAcceptance>
    <licenseUrl>{1}</licenseUrl>
    <dependencies>
      <dependency id=""basic"" />
    </dependencies>
  </metadata>
  <files />
</package>";

            return string.Format(format, requireAcceptance, licenseUrl);
        }
    }
}
