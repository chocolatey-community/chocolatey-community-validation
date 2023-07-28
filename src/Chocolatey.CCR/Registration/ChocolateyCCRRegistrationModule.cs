namespace Chocolatey.CCR.Registration
{
    using System;
    using System.Linq;
    using chocolatey.infrastructure.app.configuration;
    using chocolatey.infrastructure.app.registration;
    using chocolatey.infrastructure.rules;
    using chocolatey.infrastructure.validations;
    using Chocolatey.CCR.Validations;

    public sealed class ChocolateyCCRRegistrationModule : IExtensionModule
    {
        public void RegisterDependencies(IContainerRegistrator registrator, ChocolateyConfiguration? configuration)
        {
            if (registrator is null)
            {
                throw new ArgumentNullException(nameof(registrator));
            }

            var rules = typeof(ChocolateyCCRRegistrationModule).Assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IMetadataRule).IsAssignableFrom(t))
                .ToArray();

            registrator.RegisterService<IMetadataRule>(rules);
            registrator.RegisterService<IValidation, CommunityRepositoryPushValidation>();
        }

        public void register_dependencies(IContainerRegistrator registrator, ChocolateyConfiguration? configuration)
            => RegisterDependencies(registrator, configuration);
    }
}
