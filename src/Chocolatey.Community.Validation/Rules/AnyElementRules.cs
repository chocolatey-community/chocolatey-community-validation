namespace Chocolatey.Community.Validation.Rules
{
    using System;
    using System.Collections.Generic;
    using chocolatey;
    using chocolatey.infrastructure.rules;

    /// <summary>
    /// Runs one or more rules against all elements in the nuspec file.
    /// </summary>
    /// <seealso cref="CCRMetadataRuleBase" />
    /// <remarks>This file should only be reused for rules that do the exact same logic for all metadata elements.</remarks>
    public sealed class AnyElementRules : CCRMetadataRuleBase
    {
        private const string TemplateRuleId = "CPMR0019";

        public override IEnumerable<RuleResult> Validate(global::NuGet.Packaging.NuspecReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            // This only gets elements inside of package -> metadata. If we want other
            // elements in the future we need to add additional code to get that information.
            var elements = reader.GetMetadata();

            foreach (var element in elements)
            {
                var value = element.Value;

                if (value.ContainsSafe("__replace") ||
                    value.ContainsSafe("space_separated") ||
                    value.ContainsSafe("tag1"))
                {
                    var message = $"The element '{element.Key}' contained a templated value. Templated values should not be present in the Metadata file.";

                    yield return GetRule(TemplateRuleId, message);
                }
            }
        }

        protected internal override IEnumerable<(RuleType severity, string? id, string summary)> GetRulesInformation()
        {
            yield return (RuleType.Error, TemplateRuleId, "Templated values should not be present in Metadata file.");
        }
    }
}
