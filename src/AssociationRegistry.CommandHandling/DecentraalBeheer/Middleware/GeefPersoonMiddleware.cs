namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Middleware;

using Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Magda.Kbo;
using Framework;
using Integrations.Magda;

public static class GeefPersoonMiddleware
{
    // The message *has* to be first in the parameter list
    // Before or BeforeAsync tells Wolverine this method should be called before the actual action
    public static async Task<PersonenUitKsz> BeforeAsync(
        CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> envelope,
        IMagdaClient magdaClient,
        CancellationToken cancellationToken)
    {
        if (!envelope.Command.Vertegenwoordigers.Any())
            return PersonenUitKsz.Empty;

        var tasks = envelope.Command.Vertegenwoordigers.Select(async vertegenwoordiger =>
        {
            // First: Register subscription (must succeed)
            await magdaClient.RegistreerInschrijvingPersoon(
                vertegenwoordiger.Insz,
                AanroependeFunctie.RegistreerVzer,
                envelope.Metadata,
                cancellationToken);

            // Second: Get person details (only runs if registration succeeded)
            var persoon = await magdaClient.GeefPersoon(
                vertegenwoordiger.Insz,
                AanroependeFunctie.RegistreerVzer,
                envelope.Metadata,
                cancellationToken);

            return new PersoonUitKsz(
                vertegenwoordiger.Insz,
                vertegenwoordiger.Voornaam,
                vertegenwoordiger.Achternaam,
                persoon.Body.GeefPersoonResponse.Repliek.Antwoorden.Antwoord.Inhoud.Persoon.Overlijden != null);
        });

        var personenUitKsz = await Task.WhenAll(tasks);

        return new PersonenUitKsz(personenUitKsz);
    }
}
