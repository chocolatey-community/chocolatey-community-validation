namespace Chocolatey.Community.Validation.Tests.Rules
{
    using System.Threading.Tasks;
    using chocolatey;
    using Chocolatey.Community.Validation.Rules;
    using NUnit.Framework;

    [Category("Notes")]
    public class AuthorMatchesMaintainerRuleTests : RuleTestBase<AuthorMatchesMaintainerRule>
    {
        [Test]
        public async Task ShouldFlagRuleWhenAuthorAndMaintainerIsSame()
        {
            const string testContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Do not remove this test for UTF-8: if “Ω” doesn’t appear as greek uppercase omega letter enclosed in quotation marks, you should use an editor that supports UTF-8, not this one. -->
<package xmlns=""http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"">
  <metadata>
    <id>short-copyright</id>
    <title>[Deprecated] Test PKG</title>
    <version>1.0.0</version>
    <authors>Package-Author</authors>
    <owners>Package-Author</owners>
    <packageSourceUrl>https://test-url.com/</packageSourceUrl>
  </metadata>
  <files />
</package>";

            await VerifyNuspec(testContent);
        }
        [Test]
        public async Task ShouldFlagRuleWhenAuthorAndMaintainerUsesDifferentCase()
        {
            const string testContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Do not remove this test for UTF-8: if “Ω” doesn’t appear as greek uppercase omega letter enclosed in quotation marks, you should use an editor that supports UTF-8, not this one. -->
<package xmlns=""http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"">
  <metadata>
    <id>short-copyright</id>
    <title>[Deprecated] Test PKG</title>
    <version>1.0.0</version>
    <authors>Package-Author</authors>
    <owners>package-author</owners>
    <packageSourceUrl>https://test-url.com/</packageSourceUrl>
  </metadata>
  <files />
</package>";

            await VerifyNuspec(testContent);
        }

        [Test]
        public async Task ShouldFlagRuleWhenMultipleSameAuthorsAndMaintainers()
        {
            const string testContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Do not remove this test for UTF-8: if “Ω” doesn’t appear as greek uppercase omega letter enclosed in quotation marks, you should use an editor that supports UTF-8, not this one. -->
<package xmlns=""http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"">
  <metadata>
    <id>short-copyright</id>
    <title>[Deprecated] Test PKG</title>
    <version>1.0.0</version>
    <authors>Package-Author Package-Maintainer</authors>
    <owners>Package-Author Package-Maintainer</owners>
    <packageSourceUrl>https://test-url.com/</packageSourceUrl>
  </metadata>
  <files />
</package>";

            await VerifyNuspec(testContent);
        }

        [TestCaseSource(nameof(EmptyTestValues))]
        public async Task ShouldNotFlagOnEmptyValues(string value)
        {
            const string testContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Do not remove this test for UTF-8: if “Ω” doesn’t appear as greek uppercase omega letter enclosed in quotation marks, you should use an editor that supports UTF-8, not this one. -->
<package xmlns=""http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"">
  <metadata>
    <id>short-copyright</id>
    <title>[Deprecated] Test PKG</title>
    <version>1.0.0</version>
    <authors>{0}</authors>
    <owners>{0}</owners>
    <packageSourceUrl>https://test-url.com/</packageSourceUrl>
  </metadata>
  <files />
</package>";

            await VerifyEmptyResults(testContent.FormatWith(value));
        }

        [Test]
        public async Task ShouldNotFlagWhenDifferentMaintanerCountThanAuthors()
        {
            const string testContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Do not remove this test for UTF-8: if “Ω” doesn’t appear as greek uppercase omega letter enclosed in quotation marks, you should use an editor that supports UTF-8, not this one. -->
<package xmlns=""http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"">
  <metadata>
    <id>short-copyright</id>
    <title>[Deprecated] Test PKG</title>
    <version>1.0.0</version>
    <authors>Package-Author</authors>
    <owners>Package-Author Package-Maintainer</owners>
    <packageSourceUrl>https://test-url.com/</packageSourceUrl>
  </metadata>
  <files />
</package>";

            await VerifyEmptyResults(testContent);
        }
    }
}
