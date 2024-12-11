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
        private const string ChocolateyDependencyRuleId = "CPMR0062";
        private const string HookDependencyRuleId = "CPMR0074";

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

            foreach (var dependency in dependencies)
            {
                if (dependency.Id.Equals("chocolatey", StringComparison.OrdinalIgnoreCase))
                {
                    var message = dependency.VersionRange.HasLowerBound || dependency.VersionRange.HasUpperBound ?
                        "A dependency on Chocolatey CLI was found. Ensure that you use functionality that requires the specified version range of Chocolatey CLI." :
                        "An open ended dependency on Chocolatey CLI was found. Ensure this was not added by a mistake.";

                    yield return GetRule(ChocolateyDependencyRuleId, message);
                }
                else if (dependency.Id.EndsWith(".hook", StringComparison.OrdinalIgnoreCase))
                {
                    yield return GetRule(HookDependencyRuleId, $"Package has dependency on {dependency.Id} package. Hook packages should not be defined as a dependency.");
                }
            }
        }

        protected internal override IEnumerable<(RuleType severity, string? id, string summary)> GetRulesInformation()
        {
            yield return (RuleType.Error, DeprecatedNoDependencyRuleId, "Deprecated packages must have a dependency.");
            yield return (RuleType.Note, ChocolateyDependencyRuleId, "A dependency on Chocolatey CLI has been added.");
            yield return (RuleType.Error, HookDependencyRuleId, "Package has dependency on .hook package.");
        }
    }
}
