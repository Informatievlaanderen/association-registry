namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Middleware;

using Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Magda.Kbo;
using Framework;
using Integrations.Magda;

public class GeefPersoonMiddleware
{
    // The message *has* to be first in the parameter list
    // Before or BeforeAsync tells Wolverine this method should be called before the actual action
    public static async Task BeforeAsync(
        CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> envelope,
        IMagdaClient magdaClient,
        CancellationToken cancellationToken)
    {
        foreach (var vertegenwoordiger in envelope.Command.Vertegenwoordigers)
        {
            var registreerInschrijvingPersoon = await magdaClient.RegistreerInschrijvingPersoon(vertegenwoordiger.Insz,  AanroependeFunctie.RegistreerVzer, envelope.Metadata, cancellationToken);

            var persoon = await magdaClient.GeefPersoon(vertegenwoordiger.Insz, AanroependeFunctie.RegistreerVzer, envelope.Metadata, cancellationToken);
        }

    }
}
