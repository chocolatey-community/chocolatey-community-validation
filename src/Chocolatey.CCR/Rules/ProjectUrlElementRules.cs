namespace Chocolatey.CCR.Rules
{
    using System;
    using System.Collections.Generic;
    using chocolatey.infrastructure.rules;

    public sealed class ProjectUrlElementRules : CCRMetadataRuleBase
    {
        private const string RuleId = "CPMR0009";

        public override IEnumerable<RuleResult> Validate(global::NuGet.Packaging.NuspecReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (string.IsNullOrEmpty(reader.GetProjectUrl()))
            {
                yield return GetRule(RuleId);
            }
        }

        protected internal override IEnumerable<(RuleType severity, string? id, string summary)> GetRulesInformation()
        {
            yield return (RuleType.Error, RuleId, "A project URL for the software is either missing or empty.");
        }
    }
}
