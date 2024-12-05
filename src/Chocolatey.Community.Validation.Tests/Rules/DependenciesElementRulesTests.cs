namespace Chocolatey.Community.Validation.Tests.Rules
{
    using System.Text;
    using System.Threading.Tasks;
    using Chocolatey.Community.Validation.Rules;
    using NUnit.Framework;

    [Category("Requirements")]
    public class DependenciesElementRulesTests : RuleTestBase<DependenciesElementRules>
    {
        [TestCase("chocolatey", null)]
        [TestCase("chocolatey", "2.0.0")]
        [TestCase("chocolatey", "[,2.0.0)")]
        [TestCase("chocolatey", "[2.0.0, 3.0.0)")]
        [TestCase("chocolatey", "[2.0.0,]")]
        [TestCase("test-package.hook", null)]
        [TestCase("test-package.hook", "1.0.0")]
        public async Task ShouldFlagDependency(string id, string version)
        {
            var testContent = GetTestContent("Test Package", (id, version));

            await VerifyNuspec(testContent);
        }

        [Test]
        public async Task ShouldFlagWhenTitleContainsDeprecatedWhileMissingDependenciesElement()
        {
            const string testContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Do not remove this test for UTF-8: if “Ω” doesn’t appear as greek uppercase omega letter enclosed in quotation marks, you should use an editor that supports UTF-8, not this one. -->
<package xmlns=""http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"">
  <metadata>
    <id>short-copyright</id>
    <title>[Deprecated] Test PKG</title>
    <version>1.0.0</version>
    <authors>Author</authors>
    <packageSourceUrl>https://test-url.com/</packageSourceUrl>
  </metadata>
  <files />
</package>";

            await VerifyNuspec(testContent);
        }

        [Test]
        public async Task ShouldFlagWhenTitleContainsDeprecatedWithoutDependenciesListed()
        {
            var testContent = GetTestContent("[Deprecated] Test package");

            await VerifyNuspec(testContent);
        }

        [Test]
        public async Task ShouldFlagWhenTitleContainsDeprecatedWithoutDependenciesThatIsSelfClosed()
        {
            const string testContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Do not remove this test for UTF-8: if “Ω” doesn’t appear as greek uppercase omega letter enclosed in quotation marks, you should use an editor that supports UTF-8, not this one. -->
<package xmlns=""http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"">
  <metadata>
    <id>short-copyright</id>
    <title>[Deprecated] Test PKG</title>
    <version>1.0.0</version>
    <authors>Author</authors>
    <packageSourceUrl>https://test-url.com/</packageSourceUrl>
    <dependencies />
  </metadata>
  <files />
</package>";

            await VerifyNuspec(testContent);
        }

        [Test]
        public async Task ShouldNotFlagDependencyNotUsingCorrectHookExtension()
        {
            var testContent = GetTestContent("Test Package", ("test-package-hook", null));

            await VerifyEmptyResults(testContent);
        }

        [Test]
        public async Task ShouldNotFlagWhenTitleDoesNotContainDeprecatedNotice()
        {
            var testContent = GetTestContent("Test Pakcage");

            await VerifyEmptyResults(testContent);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("1.0.0")]
        public async Task ShouldNotFlagWhenTitleIsDeprecatedAndHasDependency(string version)
        {
            var testContent = GetTestContent("[Deprecated] TPKG", ("dep", version));

            await VerifyEmptyResults(testContent);
        }

        private static string GetTestContent(string packagetitle, params (string id, string? versionRange)[] dependencies)
        {
            var sb = new StringBuilder()
                .AppendFormat(@"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Do not remove this test for UTF-8: if “Ω” doesn’t appear as greek uppercase omega letter enclosed in quotation marks, you should use an editor that supports UTF-8, not this one. -->
<package xmlns=""http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"">
  <metadata>
    <id>short-copyright</id>
    <title>{0}</title>
    <version>1.0.0</version>
    <authors>Author</authors>
    <packageSourceUrl>https://test-url.com/</packageSourceUrl>
    <dependencies>
", packagetitle);
            foreach (var (id, versionRange) in dependencies)
            {
                sb.AppendFormat("      <dependency id=\"{0}\"", id);

                if (versionRange != null)
                {
                    sb.AppendFormat(" version=\"{0}\"", versionRange);
                }

                sb.AppendFormat(" />\n");
            }

            sb.Append(@"    </dependencies>
  </metadata>
  <files />
</package>");

            return sb.ToString();
        }
    }
}
