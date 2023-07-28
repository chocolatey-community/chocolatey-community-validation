namespace Chocolatey.CCR.Tests.Registration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using chocolatey.infrastructure.app.registration;
    using Chocolatey.CCR.Registration;
    using NUnit.Framework;
    using static VerifyNUnit.Verifier;

    [Category("Registration")]
    public class ChocolateyCCRRegistrationModuleTests
    {
        [Test]
        public async Task ShouldHaveAddedExpectedRegistrations()
        {
            var module = new ChocolateyCCRRegistrationModule();
            var registrator = new ContainerTestRegistrator();

            module.RegisterDependencies(registrator, new chocolatey.infrastructure.app.configuration.ChocolateyConfiguration());

            await Verify(registrator);
        }

        [Test]
        public Task ShouldHaveAddedExpectedRegistrationsInLegacyMethod()
        {
            var module = new ChocolateyCCRRegistrationModule();
            var registrator = new ContainerTestRegistrator();
            module.register_dependencies(registrator, new chocolatey.infrastructure.app.configuration.ChocolateyConfiguration());

            return Verify(registrator);
        }

        private class ContainerTestRegistrator : IContainerRegistrator
        {
            public ConcurrentDictionary<string, List<string>> RegisteredServices { get; } = new ConcurrentDictionary<string, List<string>>();

            public bool RegistrationFailed { get; private set; }

            public void register_instance<TImplementation>(Func<TImplementation> instance) where TImplementation : class
            {
                throw new NotImplementedException();
            }

            public void register_instance<TService, TImplementation>(Func<TImplementation> instance) where TImplementation : class, TService
            {
                throw new NotImplementedException();
            }

            public void register_instance<TService, TImplementation>(Func<IContainerResolver, TImplementation> instance) where TImplementation : class, TService
            {
                throw new NotImplementedException();
            }

            public void register_service<TService, TImplementation>(bool transient = false) where TImplementation : class, TService
            {
                throw new NotImplementedException();
            }

            public void register_service<TService>(params Type[] types)
            {
                throw new NotImplementedException();
            }

            public void register_validator(Func<Type, bool> validation_func)
            {
                throw new NotImplementedException();
            }

            public void RegisterInstance<TImplementation>(Func<TImplementation> instance) where TImplementation : class
            {
                throw new NotImplementedException();
            }

            public void RegisterInstance<TService, TImplementation>(Func<TImplementation> instance) where TImplementation : class, TService
            {
                throw new NotImplementedException();
            }

            public void RegisterInstance<TService, TImplementation>(Func<IContainerResolver, TImplementation> instance) where TImplementation : class, TService
            {
                throw new NotImplementedException();
            }

            public void RegisterService<TService, TImplementation>(bool transient = false) where TImplementation : class, TService
            {
                var serviceList = RegisteredServices.GetOrAdd(typeof(TService).FullName, (_) => new List<string>());

                serviceList.Add(typeof(TImplementation).FullName);
            }

            public void RegisterService<TService>(params Type[] types)
            {
                var serviceList = RegisteredServices.GetOrAdd(typeof(TService).FullName, (_) => new List<string>());

                serviceList.AddRange(types.Select(t => t.FullName));
            }

            public void RegisterSourceRunner<TService>() where TService : class
            {
                throw new NotImplementedException();
            }

            public void RegisterSourceRunner(Type serviceType)
            {
                throw new NotImplementedException();
            }

            public void RegisterValidator(Func<Type, bool> validation_func)
            {
                throw new NotImplementedException();
            }
        }
    }
}
