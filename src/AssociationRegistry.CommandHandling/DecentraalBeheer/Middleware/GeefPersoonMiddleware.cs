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

        var personenUitKsz = new List<PersoonUitKsz>();
        foreach (var vertegenwoordiger in envelope.Command.Vertegenwoordigers)
        {
            var registreerInschrijvingPersoon = await magdaClient.RegistreerInschrijvingPersoon(vertegenwoordiger.Insz,  AanroependeFunctie.RegistreerVzer, envelope.Metadata, cancellationToken);

            var persoon = await magdaClient.GeefPersoon(vertegenwoordiger.Insz, AanroependeFunctie.RegistreerVzer, envelope.Metadata, cancellationToken);

            personenUitKsz.Add(new PersoonUitKsz(vertegenwoordiger.Insz, vertegenwoordiger.Voornaam, vertegenwoordiger.Achternaam, persoon.Body.GeefPersoonResponse.Repliek.Antwoorden.Antwoord.Inhoud.Persoon.Overlijden != null));
        }

        return new PersonenUitKsz(personenUitKsz);
    }
}
