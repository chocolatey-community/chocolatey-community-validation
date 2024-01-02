namespace Chocolatey.Community.Validation.Tests.Rules
{
    using System.Globalization;
    using System.Threading.Tasks;
    using Chocolatey.Community.Validation.Rules;
    using NUnit.Framework;

    [Category("Requirements")]
    public class TagsElementRulesTests : RuleTestBase<TagsElementRules>
    {
        [TestCase(",taggie")]
        [TestCase("taggie,")]
        [TestCase("tag1, tag2 tag3")]
        public async Task ShouldFlagCommaSeparatedTags(string tags)
        {
            var testContent = GetTestContent(tags);

            await VerifyNuspec(testContent);
        }

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
        public async Task ShouldNotFlagTagsNotContainingAComma()
        {
            var testContent = GetTestContent("awesome-tag with space separated");

            await VerifyEmptyResults(testContent);
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
