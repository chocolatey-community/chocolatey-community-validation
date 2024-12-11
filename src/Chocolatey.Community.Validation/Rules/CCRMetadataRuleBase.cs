namespace Chocolatey.Community.Validation.Rules
{
    using System.Collections.Generic;
    using chocolatey.infrastructure.app.rules;
    using chocolatey.infrastructure.rules;

    public abstract class CCRMetadataRuleBase : MetadataRuleBase
    {
        protected internal abstract IEnumerable<(RuleType severity, string? id, string summary)> GetRulesInformation();

        protected sealed override IEnumerable<ImmutableRule> GetRules()
        {
            foreach (var (severity, id, summary) in GetRulesInformation())
            {
                if (string.IsNullOrEmpty(id))
                {
                    yield return new ImmutableRule(severity, string.Empty, summary);
                }
                else
                {
                    var helpUrl = $"https://ch0.co/rules/{id!.ToLowerInvariant()}";
                    yield return new ImmutableRule(severity, id, summary, helpUrl);
                }
            }
        }
    }
}
