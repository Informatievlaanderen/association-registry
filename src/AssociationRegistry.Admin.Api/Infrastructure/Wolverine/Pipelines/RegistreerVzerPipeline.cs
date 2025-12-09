namespace AssociationRegistry.Admin.Api.Infrastructure.Wolverine.Pipelines;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Middleware;
using Framework;
using global::Wolverine;

public static class RegistreerVzerPipeline
{
    public static void Setup(WolverineOptions options)
    {
        options.Policies.ForMessagesOfType<CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>>()
               .AddMiddleware(typeof(GeefPersonenMiddleware))
               .AddMiddleware(typeof(EnrichLocatiesMiddleware))
               .AddMiddleware(typeof(DuplicateDetectionMiddleware));
    }
}
