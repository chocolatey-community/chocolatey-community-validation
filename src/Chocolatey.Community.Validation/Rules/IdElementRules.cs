namespace Chocolatey.Community.Validation.Rules
{
    using System;
    using System.Collections.Generic;
    using chocolatey;
    using chocolatey.infrastructure.rules;

    public sealed class IdElementRules : CCRMetadataRuleBase
    {
        private const string ConfigRuleId = "CPMR0029";
        private const string DotsInIdentifierRuleId = "CPMR0061";
        private const string PreReleaseRuleId = "CPMR0024";

        public override IEnumerable<RuleResult> Validate(global::NuGet.Packaging.NuspecReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var id = reader.GetId();

            if (string.IsNullOrEmpty(id))
            {
                yield break;
            }

            if (id.ContainsSafe("alpha") ||
                id.ContainsSafe("beta") ||
                id.ContainsSafe("prerelease"))
            {
                yield return GetRule(PreReleaseRuleId);
            }

            if (id.EndsWith(".config", StringComparison.OrdinalIgnoreCase))
            {
                yield return GetRule(ConfigRuleId);
            }

            var subId = GetSubIdentifier(id);

            if (subId.IndexOf('.') > -1)
            {
                yield return GetRule(DotsInIdentifierRuleId);
            }
        }

        protected internal override IEnumerable<(RuleType severity, string? id, string summary)> GetRulesInformation()
        {
            yield return (RuleType.Error, PreReleaseRuleId, "Package ID includes a prerelease version name.");
            yield return (RuleType.Error, ConfigRuleId, "Package ID ends with the reserved suffix `.config`.");
            yield return (RuleType.Note, DotsInIdentifierRuleId, "Package ID contains dots (.), that is not part of the accepted suffixes.");
        }

        private static string GetSubIdentifier(string id)
        {
            var possibleSuffixes = new[]
            {
                ".portable",
                ".commandline",
                ".install",
                ".extension",
                ".template",
                ".powershell",
                // This is not an accepted suffix, but an existing rule aready handle this. So we ignore it.
                ".config"
            };

            foreach (var suffix in possibleSuffixes)
            {
                if (id.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                {
                    return id.Substring(0, id.Length - suffix.Length);
                }
            }

            return id;
        }
    }
}
