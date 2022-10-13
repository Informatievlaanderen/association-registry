namespace AssociationRegistry.Admin.Api.Infrastructure.Modules;

using Autofac;
using Autofac.Extensions.DependencyInjection;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class ApiModule : Module
{
    private readonly IServiceCollection _services;

    public ApiModule(IServiceCollection services)
    {
        _services = services;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder
            .RegisterType<ProblemDetailsHelper>()
            .AsSelf();

        builder
            .Populate(_services);
    }
}
