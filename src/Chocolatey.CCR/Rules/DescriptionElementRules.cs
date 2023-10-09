namespace Chocolatey.CCR.Rules
{
    using System;
    using System.Collections.Generic;
    using chocolatey.infrastructure.rules;

    public sealed class DescriptionElementRules : CCRMetadataRuleBase
    {
        private const string RuleId = "CPMR0002";

        public override IEnumerable<RuleResult> Validate(global::NuGet.Packaging.NuspecReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var description = reader.GetDescription();

            if (string.IsNullOrEmpty(description))
            {
                yield return GetRule(RuleId);
            }
        }

        protected internal override IEnumerable<(RuleType severity, string? id, string summary)> GetRulesInformation()
        {
            yield return (RuleType.Error, RuleId, "A description of the package is either missing or empty.");
        }
    }
}
