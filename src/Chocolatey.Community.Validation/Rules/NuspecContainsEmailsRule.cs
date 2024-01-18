namespace Chocolatey.Community.Validation.Rules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using chocolatey.infrastructure.rules;

    public sealed class NuspecContainsEmailsRule : CCRMetadataRuleBase
    {
        private const string EmailRuleId = "CPMR0020";
        private const string EmailRegexPattern = @"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}";

        public static bool OutputFoundEmails { get; set; } = true;

        public override IEnumerable<RuleResult> Validate(global::NuGet.Packaging.NuspecReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var elements = reader.GetMetadata();
            var emailFlagged = false;

            foreach (var element in elements)
            {
                var value = element.Value;
                var emails = new List<string>();

                foreach (var email in GetMatchedEmails(value))
                {
                    emails.Add(email);
                }

                if (emails.Count > 0)
                {
                    emailFlagged = true;
                    var message = OutputFoundEmails ?
                        $"The element '{element.Key}' contained the emails '{string.Join(", ", emails)}'. Emails should not be present in the Metadata file." :
                        $"The element '{element.Key}' contains {emails.Count} email(s). Emails should not be present in the Metadata file.";

                    yield return GetRule(EmailRuleId, message);
                }
            }

            if (!emailFlagged && GetMatchedEmails(reader.Xml.ToString()).Any())
            {
                yield return GetRule(EmailRuleId);
            }
        }

        protected internal override IEnumerable<(RuleType severity, string? id, string summary)> GetRulesInformation()
        {
            yield return (RuleType.Error, EmailRuleId, "An Email Address should not be used in Metadata file.");
        }

        private static IEnumerable<string> GetMatchedEmails(string valueToTest)
        {
            var matches = Regex.Matches(valueToTest, EmailRegexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                yield return match.Value;
            }
        }
    }
}
