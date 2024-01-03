namespace Chocolatey.Community.Validation.Tests.Validations
{
    using System.Collections;
    using System.Linq;
    using System.Threading.Tasks;
    using chocolatey.infrastructure.app.configuration;
    using chocolatey.infrastructure.rules;
    using chocolatey.infrastructure.services;
    using Chocolatey.Community.Validation.Validations;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using static VerifyNUnit.Verifier;

    [Category("Validations")]
    public class CommunityRepositoryPushValidationTests
    {
        public static IEnumerable NonPushCommands
        {
            get
            {
                return new[]
                {
                    "apikey",
                    "cache",
                    "config",
                    "feature",
                    "features",
                    "find",
                    "help",
                    "info",
                    "install",
                    "list",
                    "new",
                    "outdated",
                    "pack",
                    "pin",
                    "search",
                    "setapikey",
                    "source",
                    "sources",
                    "template",
                    "templates",
                    "uninstall",
                    "unpackself",
                    "upgrade"
                };
            }
        }

        [TestCaseSource(nameof(NonPushCommands))]
        public void ShouldNotRunThroughValidationOnCommand(string command)
        {
            var config = new ChocolateyConfiguration
            {
                CommandName = command,
                Sources = "https://push.chocolatey.org/"
            };

            var ruleService = new Mock<IRuleService>();
            ruleService.Setup(r => r.ValidateRules(It.IsAny<string>()))
                .Returns(new[] { new RuleResult(RuleType.Error, string.Empty, "Not Expected") });

            var validator = new CommunityRepositoryPushValidation(ruleService.Object);

            var result = validator.Validate(config).ToList();

            result.Should().NotBeNull().And.BeEmpty();
            ruleService.Verify(r => r.ValidateRules(It.IsAny<string>()), Times.Never);
        }

        [TestCase("https://kimitek.net/")]
        [TestCase("http://myget.org")]
        [TestCase("https://hermes.chocolatey.org/")]
        public void ShouldNotRunThroughValidationOnDefaultPushSource(string source)
        {
            var config = new ChocolateyConfiguration
            {
                CommandName = "push"
            };
            config.PushCommand.DefaultSource = source;

            var ruleService = new Mock<IRuleService>();
            ruleService.Setup(r => r.ValidateRules(It.IsAny<string>()))
                .Returns(new[] { new RuleResult(RuleType.Error, string.Empty, "Not Expected") });

            var validator = new CommunityRepositoryPushValidation(ruleService.Object);

            var result = validator.Validate(config).ToList();

            result.Should().NotBeNull().And.BeEmpty();
            ruleService.Verify(r => r.ValidateRules(It.IsAny<string>()), Times.Never);
        }

        [TestCase("https://kimitek.net/")]
        [TestCase("http://myget.org")]
        [TestCase("https://hermes.chocolatey.org/")]
        public void ShouldNotRunThroughValidationOnSource(string source)
        {
            var config = new ChocolateyConfiguration
            {
                CommandName = "push",
                Sources = source
            };

            var ruleService = new Mock<IRuleService>();
            ruleService.Setup(r => r.ValidateRules(It.IsAny<string>()))
                .Returns(new[] { new RuleResult(RuleType.Error, string.Empty, "Not Expected") });

            var validator = new CommunityRepositoryPushValidation(ruleService.Object);

            var result = validator.Validate(config).ToList();

            result.Should().NotBeNull().And.BeEmpty();
            ruleService.Verify(r => r.ValidateRules(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public Task ShouldThrowExceptionOnNullConfigurationBeingNull()
        {
            static void RunValidation()
            {
                var validator = new CommunityRepositoryPushValidation(new Mock<IRuleService>().Object);

                validator.Validate(null!);
            }

            return Throws(RunValidation)
                .IgnoreStackTrace();
        }

        [Test]
        public Task ShouldThrowExceptionOnRuleServiceBeingNull()
        {
            static void CreateValidator() => new CommunityRepositoryPushValidation(null!);

            return Throws(CreateValidator)
                .IgnoreStackTrace();
        }

        [TestCase("https://push.chocolatey.org/")]
        public async Task ShouldValidateOnExpectedPushSources(string source)
        {
            var config = new ChocolateyConfiguration
            {
                CommandName = "push",
                Sources = source,
                Input = "C:\\testing\\pkg.nupkg"
            };

            var ruleService = new Mock<IRuleService>();

            ruleService.Setup(r => r.ValidateRules(It.IsAny<string>()))
                .Returns(new[]
                {
                    new RuleResult(RuleType.Error, string.Empty, "I am an error without ID and Help URL"),
                    new RuleResult(RuleType.Error, "ID2", "I am an error with an ID and without an Help URL"),
                    new RuleResult(RuleType.Error, string.Empty, "I am an error without an ID and with a Help URL") { HelpUrl = "https://docs.failures.com/" },
                    new RuleResult(RuleType.Error, "ID4", "I am an error with an ID and a Help URL") { HelpUrl = "https://docs.failures.com/" },
                    new RuleResult(RuleType.Warning, string.Empty, "I am a warning without ID and Help URL"),
                    new RuleResult(RuleType.Warning, "ID6", "I am a warning with an ID and without an Help URL"),
                    new RuleResult(RuleType.Warning, string.Empty, "I am a warning without an ID and with a Help URL") { HelpUrl = "https://docs.failures.com/" },
                    new RuleResult(RuleType.Warning, "ID8", "I am a warning with an ID and a Help URL") { HelpUrl = "https://docs.failures.com/" },
                    new RuleResult(RuleType.Information, string.Empty, "I am information without ID and Help URL"),
                    new RuleResult(RuleType.Information, "ID10", "I am information with an ID and without an Help URL"),
                    new RuleResult(RuleType.Information, string.Empty, "I am information without an ID and with a Help URL") { HelpUrl = "https://docs.failures.com/" },
                    new RuleResult(RuleType.Information, "ID12", "I am information with an ID and a Help URL") { HelpUrl = "https://docs.failures.com/" },
                    new RuleResult(RuleType.Note, string.Empty, "I am a note without ID and Help URL"),
                    new RuleResult(RuleType.Note, "ID14", "I am a note with an ID and without an Help URL"),
                    new RuleResult(RuleType.Note, string.Empty, "I am a note without an ID and with a Help URL") { HelpUrl = "https://docs.failures.com/" },
                    new RuleResult(RuleType.Note, "ID16", "I am a note with an ID and a Help URL") { HelpUrl = "https://docs.failures.com/" },
                    new RuleResult(RuleType.Information, "ID12", "I am information with an ID and a Help URL") { HelpUrl = "https://docs.failures.com/" },
                    new RuleResult(RuleType.None, string.Empty, "I am nothing without an ID and Help URL"),
                    new RuleResult(RuleType.None, "ID18", "I am nothing with an ID and without an Help URL"),
                    new RuleResult(RuleType.None, string.Empty, "I am nothing without an ID and with a Help URL") { HelpUrl = "https://docs.failures.com/" },
                    new RuleResult(RuleType.None, "ID16", "I am nothing with an ID and a Help URL") { HelpUrl = "https://docs.failures.com/" },
                });

            var validator = new CommunityRepositoryPushValidation(ruleService.Object);

            var result = validator.Validate(config).ToList();

            await Verify(result);

            ruleService.Verify(r => r.ValidateRules(config.Input), Times.Once);
        }
    }
}
