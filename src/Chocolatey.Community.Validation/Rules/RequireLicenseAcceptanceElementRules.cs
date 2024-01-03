namespace Chocolatey.Community.Validation.Rules
{
    using System;
    using System.Collections.Generic;
    using chocolatey.infrastructure.rules;

    public sealed class RequireLicenseAcceptanceElementRules : CCRMetadataRuleBase
    {
        private const string RuleId = "CPMR0007";

        public override IEnumerable<RuleResult> Validate(global::NuGet.Packaging.NuspecReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (reader.GetRequireLicenseAcceptance() && string.IsNullOrEmpty(reader.GetLicenseUrl()))
            {
                yield return GetRule(RuleId);
            }
        }

        protected internal override IEnumerable<(RuleType severity, string? id, string summary)> GetRulesInformation()
        {
            yield return (RuleType.Error, RuleId, "The license URL is absent, despite the requirement for license acceptance.");
        }
    }
}
