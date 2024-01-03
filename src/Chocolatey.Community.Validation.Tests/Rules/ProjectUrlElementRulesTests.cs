namespace Chocolatey.Community.Validation.Tests.Rules
{
    using System.Globalization;
    using System.Threading.Tasks;
    using Chocolatey.Community.Validation.Rules;
    using NUnit.Framework;

    [Category("Requirements")]
    public class ProjectUrlElementRulesTests : RuleTestBase<ProjectUrlElementRules>
    {
        [Test]
        public async Task ShouldFlagWhenProjectUrlElementIsNotPresent()
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

            await VerifyNuspec(testContent);
        }

        [TestCaseSource(nameof(EmptyTestValues))]
        public async Task ShouldFlagWhenProjectUrlIsEmpty(string? value)
        {
            var testContent = GetTestContent(value);

            await VerifyNuspec(testContent);
        }

        [TestCaseSource(nameof(InvalidUrlValues))]
        [TestCaseSource(nameof(ValidUrlValues))]
        public async Task ShouldNotFlagWhenProjectUrlIsSpecified(string value)
        {
            var testContent = GetTestContent(value);

            await VerifyEmptyResults(testContent);
        }

        private static string GetTestContent(string? projectUrl)
        {
            const string format = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Do not remove this test for UTF-8: if “Ω” doesn’t appear as greek uppercase omega letter enclosed in quotation marks, you should use an editor that supports UTF-8, not this one. -->
<package xmlns=""http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"">
  <metadata>
    <id>short-copyright</id>
    <version>1.0.0</version>
    <authors>Author</authors>
    <packageSourceUrl>https://test-url.com/</packageSourceUrl>
    <copyright>Copyright 2023</copyright>
    <projectUrl>{0}</projectUrl>
    <dependencies>
      <dependency id=""basic"" />
    </dependencies>
  </metadata>
  <files />
</package>";

            return string.Format(CultureInfo.InvariantCulture, format, projectUrl);
        }
    }
}
