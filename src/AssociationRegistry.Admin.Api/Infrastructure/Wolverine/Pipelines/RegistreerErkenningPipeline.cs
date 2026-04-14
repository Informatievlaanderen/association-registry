namespace AssociationRegistry.Admin.Api.Infrastructure.Wolverine.Pipelines;

using global::Wolverine;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Middleware;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning.Middleware;
using Framework;

public static class RegistreerErkenningPipeline
{
    public static void Setup(WolverineOptions options)
    {
        options
            .Policies.ForMessagesOfType<CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>>()
            .AddMiddleware(typeof(EnrichIpdcProductMiddleware));
    }
}
