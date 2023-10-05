namespace Chocolatey.CCR.Rules
{
    using System;
    using System.Collections.Generic;
    using chocolatey.infrastructure.rules;

    public sealed class TagsMissingOrEmptyRule : CCRMetadataRuleBase
    {
        private const string EmptyRuleId = "CPMR0023";

        public override IEnumerable<RuleResult> Validate(global::NuGet.Packaging.NuspecReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var tags = reader.GetTags();

            if (string.IsNullOrEmpty(tags))
            {
                yield return GetRule(EmptyRuleId);
            }
        }

        protected internal override IEnumerable<(RuleType severity, string? id, string summary)> GetRulesInformation()
        {
            yield return (RuleType.Error, EmptyRuleId, "Packages require at least one tag, and they must be separated by a space.");
        }
    }
}
