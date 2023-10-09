namespace Chocolatey.CCR.Tests.Rules
{
    using System.Globalization;
    using System.Threading.Tasks;
    using Chocolatey.CCR.Rules;
    using NUnit.Framework;

    public class TagsMissingOrEmptyRuleTests : RuleTestBase<TagsMissingOrEmptyRule>
    {
        [TestCaseSource(nameof(EmptyTestValues))]
        public async Task ShouldFlagEmptyTags(string tags)
        {
            var testContent = GetTestContent(tags);

            await VerifyNuspec(testContent);
        }

        [Test]
        public async Task ShouldFlagMissingTagsElement()
        {
            const string testContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
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

            await VerifyNuspec(testContent);
        }

        [Test]
        public async Task ShouldNotFlagWhenTagsIsNotEmpty()
        {
            var testContent = GetTestContent("some awesome tags");

            await VerifyEmptyResults(testContent);
        }

        private static string GetTestContent(string? tags)
        {
            const string format = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Do not remove this test for UTF-8: if “Ω” doesn’t appear as greek uppercase omega letter enclosed in quotation marks, you should use an editor that supports UTF-8, not this one. -->
<package xmlns=""http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"">
  <metadata>
    <id>short-copyright</id>
    <version>1.0.0</version>
    <authors>Author</authors>
    <packageSourceUrl>https://test-url.com/</packageSourceUrl>
    <tags>{0}</tags>
    <dependencies>
      <dependency id=""basic"" />
    </dependencies>
  </metadata>
  <files />
</package>";

            return string.Format(CultureInfo.InvariantCulture, format, tags);
        }
    }
}
