namespace Chocolatey.CCR.Tests.Rules
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using chocolatey.infrastructure.rules;
    using Chocolatey.CCR.Rules;
    using FluentAssertions;
    using static VerifyNUnit.Verifier;

    public abstract class RuleTestBase<TestRule>
        where TestRule : CCRMetadataRuleBase, new()
    {
        protected readonly TestRule Rule = new TestRule();

        protected async Task<IEnumerable<RuleResult>> GetRuleResults(string nuspecContent, Encoding encoding)
        {
            using var memStream = new MemoryStream();
            var content = encoding.GetBytes(nuspecContent);
            await memStream.WriteAsync(content, 0, content.Length);
            memStream.Position = 0;

            // We do not reference NuGet.Packaging ourself, as such we need to use the explicit global namespace.
            var reader = new global::NuGet.Packaging.NuspecReader(memStream, null, leaveStreamOpen: true);

            return Rule.Validate(reader);
        }

        protected Task VerifyEmptyResults(string nuspecContent)
        {
            return VerifyEmptyResults(nuspecContent, Encoding.UTF8);
        }

        protected async Task VerifyEmptyResults(string nuspecContent, Encoding encoding)
        {
            var result = await GetRuleResults(nuspecContent, encoding);

            result.Should().NotBeNull().And.BeEmpty();
        }

        protected Task VerifyNuspec(string nuspecContent)
        {
            return VerifyNuspec(nuspecContent, Encoding.UTF8);
        }

        protected async Task VerifyNuspec(string nuspecContent, Encoding encoding)
        {
            var result = await GetRuleResults(nuspecContent, encoding);

            await Verify(result);
        }
    }
}
