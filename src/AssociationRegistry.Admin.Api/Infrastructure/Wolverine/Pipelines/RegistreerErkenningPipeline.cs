namespace AssociationRegistry.Admin.Api.Infrastructure.Wolverine.Pipelines;

using global::Wolverine;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.ValideerBankrekening;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning.Middleware;
using Framework;

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

public static class ValideerBankrekeningnummerPipeline
{
    public static void Setup(WolverineOptions options)
    {
        options
            .Policies.ForMessagesOfType<CommandEnvelope<ValideerBankrekeningnummerCommand>>()
            .AddMiddleware(typeof(EnrichGegevensInitiatorMiddleware));
    }
}
