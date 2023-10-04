namespace Chocolatey.CCR.Tests.Rules
{
    using System.Globalization;
    using System.Threading.Tasks;
    using Chocolatey.CCR.Rules;
    using NUnit.Framework;

    [Category("Requirements")]
    public class DescriptionElementRulesTests : RuleTestBase<DescriptionElementRules>
    {
        [Test]
        public async Task ShouldFlagWhenDescriptionElementIsNotPresent()
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

        [TestCase(null)]
        [TestCase("")]
        [TestCase("     ")]
        [TestCase("\r\n   \n")]
        public async Task ShouldFlagWhenDescriptionIsEmpty(string? value)
        {
            var testContent = GetTestContent(value);

            await VerifyNuspec(testContent);
        }

        [Test]
        public async Task ShouldNotFlagDescriptionRuleOnValidDescription()
        {
            var testContent = GetTestContent("Some kind of description needs to be specified. We make it long in case we reuse the same class for a minimum length Rule.");

            await VerifyEmptyResults(testContent);
        }

        private static string GetTestContent(string? copyright)
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
    <description>{0}</description>
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
