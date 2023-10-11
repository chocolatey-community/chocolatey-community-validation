namespace Chocolatey.CCR.Rules
{
    using System;
    using System.Collections.Generic;
    using chocolatey;
    using chocolatey.infrastructure.rules;

    public sealed class IdElementRules : CCRMetadataRuleBase
    {
        private const string PreReleaseRuleId = "CPMR0024";
        private const string ConfigRuleId = "CPMR0029";

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
        }

        protected internal override IEnumerable<(RuleType severity, string? id, string summary)> GetRulesInformation()
        {
            yield return (RuleType.Error, PreReleaseRuleId, "Package ID includes a prerelease version name.");
            yield return (RuleType.Error, ConfigRuleId, "Package ID ends with the reserved suffix `.config`.");
        }
    }
}
