namespace Chocolatey.Community.Validation.Validations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using chocolatey;
    using chocolatey.infrastructure.app.configuration;
    using chocolatey.infrastructure.rules;
    using chocolatey.infrastructure.services;
    using chocolatey.infrastructure.validations;

    public sealed class CommunityRepositoryPushValidation : IValidation
    {
        private readonly IRuleService _ruleService;

        public CommunityRepositoryPushValidation(IRuleService ruleService)
        {
            _ruleService = ruleService ?? throw new ArgumentNullException(nameof(ruleService));
        }

        public ICollection<ValidationResult> Validate(ChocolateyConfiguration config)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (!config.CommandName.IsEqualTo("push") || !UsingCommunityAsPushSource(config))
            {
                return Array.Empty<ValidationResult>();
            }

            var rules = _ruleService.ValidateRules(config.Input);
            var result = new List<ValidationResult>();

            foreach (var rule in rules)
            {
                var status = rule.Severity switch
                {
                    RuleType.Note => ValidationStatus.Checked,
                    RuleType.Information => ValidationStatus.Checked,
                    RuleType.Warning => ValidationStatus.Warning,
                    RuleType.Error => ValidationStatus.Error,
                    RuleType.None => ValidationStatus.Success,
                    _ => throw new InvalidOperationException()
                };

                var message = "{0}{1}{2}.".FormatWith(
                    string.IsNullOrEmpty(rule.Id) ? string.Empty : rule.Id + ": ",
                    rule.Message,
                    string.IsNullOrEmpty(rule.HelpUrl) ? string.Empty : ".\n   See " + rule.HelpUrl);

                var validation = new ValidationResult
                {
                    ExitCode = status == ValidationStatus.Error ? 1 : 0,
                    Message = message,
                    Status = status
                };
                result.Add(validation);
            }

            return result;
        }

        private bool UsingCommunityAsPushSource(ChocolateyConfiguration config)
        {
            if (string.IsNullOrWhiteSpace(config.Sources) ||
                (!Uri.TryCreate(config.Sources, UriKind.Absolute, out var sourceUri) &&
                 !Uri.TryCreate(config.PushCommand.DefaultSource, UriKind.Absolute, out sourceUri)))
            {
                return false;
            }

            var possibleDomains = new[] { "chocolatey.org", "push.chocolatey.org", "community.chocolatey.org" };

            return possibleDomains.Contains(sourceUri.Host, StringComparer.OrdinalIgnoreCase);
        }

        public ICollection<ValidationResult> validate(ChocolateyConfiguration config)
        {
            return Validate(config);
        }
    }
}
