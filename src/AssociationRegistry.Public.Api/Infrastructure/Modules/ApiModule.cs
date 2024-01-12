namespace AssociationRegistry.Public.Api.Infrastructure.Modules;

using Autofac;
using Autofac.Extensions.DependencyInjection;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Microsoft.Extensions.DependencyInjection;

public class ApiModule : Module
{
    private readonly IServiceCollection _services;

    public ApiModule(IServiceCollection services)
    {
        _services = services;
    }

    protected override void Load(ContainerBuilder builder)
    {
        // builder
        // .RegisterModule(new DataDogModule(_configuration))
        // .RegisterModule(new LegacyModule(_configuration, _services, _loggerFactory));

        builder
           .RegisterType<ProblemDetailsHelper>()
           .AsSelf();

        builder
           .Populate(_services);
    }
}
