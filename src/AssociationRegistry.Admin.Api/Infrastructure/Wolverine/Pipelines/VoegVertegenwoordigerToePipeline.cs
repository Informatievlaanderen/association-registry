namespace AssociationRegistry.Admin.Api.Infrastructure.Wolverine.Pipelines;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VoegVertegenwoordigerToe;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Middleware;
using Framework;
using global::Wolverine;

public static class VoegVertegenwoordigerToePipeline
{
    public static void Setup(WolverineOptions options)
    {
        options.Policies.ForMessagesOfType<CommandEnvelope<VoegVertegenwoordigerToeCommand>>()
               .AddMiddleware(typeof(GeefPersoonMiddleware));
    }
}
