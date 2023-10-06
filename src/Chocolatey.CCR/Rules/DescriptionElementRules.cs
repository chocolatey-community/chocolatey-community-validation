namespace Chocolatey.CCR.Rules
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using chocolatey.infrastructure.rules;

    public sealed class DescriptionElementRules : CCRMetadataRuleBase
    {
        private const string RuleId = "CPMR0002";
        private const string TooManyCharsRuleId = "CPMR0026";

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
            else if (description.Length > 4000)
            {
                yield return GetRule(TooManyCharsRuleId, $"The description had a length of {description.Length:N0} characters. A description can not have a length of above {4000:N0} characters.");
            }
        }

        protected internal override IEnumerable<(RuleType severity, string? id, string summary)> GetRulesInformation()
        {
            yield return (RuleType.Error, RuleId, "A description of the package is either missing or empty.");
            yield return (RuleType.Error, TooManyCharsRuleId, $"A description can not have a length of above {4000:N0} characters.");
        }
    }
}
