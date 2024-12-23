namespace Chocolatey.Community.Validation.Rules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using chocolatey.infrastructure.rules;

    public sealed class TagsElementRules : CCRMetadataRuleBase
    {
        private const string CommaRuleId = "CPMR0014";
        private const string EmptyRuleId = "CPMR0023";
        private const string NotSilentRuleId = "CPMR0067";

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
            else if (tags.Contains(','))
            {
                yield return GetRule(CommaRuleId);
            }

            if (tags.IndexOf("notSilent", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                yield return GetRule(NotSilentRuleId);
            }
        }

        protected internal override IEnumerable<(RuleType severity, string? id, string summary)> GetRulesInformation()
        {
            yield return (RuleType.Error, CommaRuleId, "The tags have been separated by a comma; they must be separated by a space.");
            yield return (RuleType.Error, EmptyRuleId, "Packages require at least one tag, and they must be separated by a space.");
            yield return (RuleType.Note, NotSilentRuleId, "The tag 'notSilent' is being used.");
        }
    }
}
