namespace Chocolatey.Community.Validation.Rules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using chocolatey;
    using chocolatey.infrastructure.rules;

    public sealed class DependenciesElementRules : CCRMetadataRuleBase
    {
        private const string DeprecatedNoDependencyRuleId = "CPMR0017";

        public override IEnumerable<RuleResult> Validate(global::NuGet.Packaging.NuspecReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var dependencies = reader.GetDependencyGroups().SelectMany(dg => dg.Packages).ToArray();

            if (reader.GetTitle().ContainsSafe("deprecated", StringComparison.OrdinalIgnoreCase) &&
                dependencies.Length == 0)
            {
                yield return GetRule(DeprecatedNoDependencyRuleId, "A dependency is required for deprecated packages.");
            }
        }

        protected internal override IEnumerable<(RuleType severity, string? id, string summary)> GetRulesInformation()
        {
            yield return (RuleType.Error, DeprecatedNoDependencyRuleId, "Deprecated packages must have a dependency.");
        }
    }
}
