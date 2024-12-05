namespace Chocolatey.Community.Validation.Rules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using chocolatey;
    using chocolatey.infrastructure.rules;

    public sealed class IconUrlElementRules : CCRMetadataRuleBase
    {
        private const string RawUrlRuleId = "CPMR0076";

        public override IEnumerable<RuleResult> Validate(global::NuGet.Packaging.NuspecReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var iconUrl = reader.GetIconUrl();

            if (!Uri.TryCreate(iconUrl, UriKind.Absolute, out var iconUri))
            {
                yield break;
            }

            if (IsGitHubIconUrl(iconUri))
            {
                yield return GetRule(RawUrlRuleId);
            }
            else if (iconUri.Host.IsEqualTo("cdn.rawgit.com") || iconUri.Host.IsEqualTo("rawgit.com"))
            {
                yield return GetRule(RawUrlRuleId, "Icon URL uses a URL that is a RawGit URL.");
            }
        }

        private bool IsGitHubIconUrl(Uri iconUri)
        {
            var hosts = new[]
            {
                "raw.githubusercontent.com",
                "githubusercontent.com",
                "github.com",
                "gist.github.com"
            };

            return hosts.Contains(iconUri.Host.ToLowerInvariant());
        }

        protected internal override IEnumerable<(RuleType severity, string? id, string summary)> GetRulesInformation()
        {
            yield return (RuleType.Error, RawUrlRuleId, "Icon URL uses a URL that is a GitHub raw URL.");
        }
    }
}
