namespace AssociationRegistry.Admin.Api.Infrastructure.Wolverine.Pipelines;

using CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning.Middleware;
using Framework;
using global::Wolverine;

public static class RegistreerErkenningPipeline
{
    public static void Setup(WolverineOptions options)
    {
        options
            .Policies.ForMessagesOfType<CommandEnvelope<RegistreerErkenningCommand>>()
            .AddMiddleware(typeof(EnrichIpdcProductMiddleware))
            .AddMiddleware(typeof(EnrichGegevensInitiatorMiddleware));
    }
}
