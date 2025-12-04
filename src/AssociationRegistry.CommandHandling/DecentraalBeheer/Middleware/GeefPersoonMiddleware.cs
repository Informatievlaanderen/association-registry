namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Middleware;

using Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using Acties.Vertegenwoordigers.VoegVertegenwoordigerToe;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Magda.Persoon;
using Framework;

public static class GeefPersoonMiddleware
{
    // The message *has* to be first in the parameter list
    // Before or BeforeAsync tells Wolverine this method should be called before the actual action
    public static async Task<PersoonUitKsz> BeforeAsync(
        CommandEnvelope<VoegVertegenwoordigerToeCommand> envelope,
        IGeefPersoonService geefPersoonService,
        CancellationToken cancellationToken)
        => await geefPersoonService.GeefPersoon(envelope.Command.Vertegenwoordiger, envelope.Metadata, cancellationToken);
}
