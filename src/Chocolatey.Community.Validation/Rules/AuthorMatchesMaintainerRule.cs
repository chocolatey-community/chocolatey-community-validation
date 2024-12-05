namespace Chocolatey.Community.Validation.Rules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using chocolatey.infrastructure.rules;

    public sealed class AuthorMatchesMaintainerRule : CCRMetadataRuleBase
    {
        private const string RuleId = "CPMR0068";

        public override IEnumerable<RuleResult> Validate(global::NuGet.Packaging.NuspecReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var owners = GetNormalized(reader.GetOwners()).ToArray();
            var authors = GetNormalized(reader.GetAuthors()).ToArray();

            if (owners.Length != authors.Length || owners.Length == 0)
            {
                yield break;
            }

            for (var i = 0; i < owners.Length; i++)
            {
                if (string.Compare(owners[i], authors[i], StringComparison.OrdinalIgnoreCase) != 0)
                {
                    yield break;
                }
            }

            yield return GetRule(RuleId);
        }

        protected internal override IEnumerable<(RuleType severity, string? id, string summary)> GetRulesInformation()
        {
            yield return (RuleType.Note, RuleId, "The package maintainer field (owners) matches the software author field (authors) in the nuspec.");
        }

        private static IEnumerable<string> GetNormalized(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                yield break;
            }

            foreach (var owner in content.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!string.IsNullOrWhiteSpace(owner))
                {
                    yield return owner;
                }
            }
        }
    }
}
