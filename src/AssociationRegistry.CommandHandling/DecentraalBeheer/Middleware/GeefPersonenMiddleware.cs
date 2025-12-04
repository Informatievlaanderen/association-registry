namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Middleware;

using Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using Magda.Persoon;
using Framework;

public static class GeefPersonenMiddleware
{
    // The message *has* to be first in the parameter list
    // Before or BeforeAsync tells Wolverine this method should be called before the actual action
    public static async Task<PersonenUitKsz> BeforeAsync(
        CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> envelope,
        IGeefPersoonService geefPersoonService,
        CancellationToken cancellationToken)
    {
        if (!envelope.Command.Vertegenwoordigers.Any())
            return PersonenUitKsz.Empty;

        return await geefPersoonService.GeefPersonen(envelope.Command.Vertegenwoordigers, envelope.Metadata, cancellationToken);
    }
}
