namespace Chocolatey.Community.Validation.Tests.Rules
{
    using System.Globalization;
    using System.Threading.Tasks;
    using Chocolatey.Community.Validation.Rules;
    using NUnit.Framework;

    public class IdElementRulesTests : RuleTestBase<IdElementRules>
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("some-id")]
        [TestCase("something.portable")]
        [TestCase("something.commandline")]
        [TestCase("something.powershell")]
        [TestCase("something.install")]
        [TestCase("something.template")]
        [TestCase("something.extension")]
        [TestCase("iamalongidentifier.extension")]
        [TestCase("i-am-a-long-identifier-separated-by-dashes")]
        public async Task ShouldNotFlagIdentifier(string id)
        {
            var testContent = GetTestContent(id);

            await VerifyEmptyResults(testContent);
        }

        [TestCase("testalpha")]
        [TestCase("test-ALPha")]
        [TestCase("testBETA")]
        [TestCase("beta-test")]
        [TestCase("prerelease")]
        [TestCase("my prerelease")]
        [TestCase("my-package.CONFIG")]
        [TestCase("pkg.config")]
        [TestCase("something.other")]
        [TestCase("something.other.portable")]
        [TestCase("different.something.commandline")]
        [TestCase("something.other.powershell")]
        [TestCase("something.other.install")]
        [TestCase("something.other.template")]
        [TestCase("something.other.extension")]
        [TestCase("with_underscores")]
        [TestCase("iamaverylongidentifierthatismorethan20characters")]
        [TestCase("i-haveasectionlongerthan20-characters.install")]
        [TestCase("IAmALongeAlpha.CharacterThatMatches_Multiple_Rules.config")]
        public async Task ShouldFlagIdentifier(string id)
        {
            var testContent = GetTestContent(id);

            await VerifyNuspec(testContent);
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
