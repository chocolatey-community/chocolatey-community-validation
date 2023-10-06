namespace Chocolatey.CCR.Rules
{
    using System;
    using System.Collections.Generic;
    using chocolatey.infrastructure.rules;

    public sealed class CopyrightCharacterCountTooLowRule : CCRMetadataRuleBase
    {
        // We do not reference NuGet. Packaging ourselves, as such we need to use the explicit global namespace.
        public override IEnumerable<RuleResult> Validate(global::NuGet.Packaging.NuspecReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (!HasElement(reader, "copyright"))
            {
                yield break;
            }

            var copyright = reader.GetCopyright()?.Trim() ?? string.Empty;

            if (copyright.Length < 4)
            {
                yield return GetRule("CPMR0001");
            }
        }

        protected internal override IEnumerable<(RuleType severity, string? id, string summary)> GetRulesInformation()
        {
            yield return (RuleType.Error, "CPMR0001", "Copyright Character Count Below 4 characters");
        }
    }
}
