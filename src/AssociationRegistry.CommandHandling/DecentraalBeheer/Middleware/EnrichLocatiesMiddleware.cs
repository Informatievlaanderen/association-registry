namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Middleware;

using Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.Framework;
using AssociationRegistry.GemeentenaamVerrijking;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Exceptions;
using Integrations.Grar.Clients;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class EnrichLocatiesMiddleware
{
    // The message *has* to be first in the parameter list
    // Before or BeforeAsync tells Wolverine this method should be called before the actual action
    public static async Task<VerrijkteAdressenUitGrar> BeforeAsync(
        CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> envelope,
        IGrarClient grarClient)
    {
        var enrichedLocaties = new Dictionary<string, Adres>();

        foreach (var locatie in envelope.Command.Locaties.Where(x => x.AdresId is not null)
                                        .DistinctBy(x => x.AdresId))
        {
            var addressDetailResponse = await grarClient.GetAddressById(locatie.AdresId.ToId(), CancellationToken.None);

            if (!addressDetailResponse.IsActief)
                throw new AdressenregisterReturnedInactiefAdres();

            var postalInformation = await grarClient.GetPostalInformationDetail(addressDetailResponse.Postcode);

            var verrijkteGemeentenaam = GemeentenaamDecorator.VerrijkGemeentenaam(
                postalInformation,
                addressDetailResponse.Gemeente);

            enrichedLocaties.Add(
                locatie.AdresId.Bronwaarde,
                Adres.Create(addressDetailResponse.Straatnaam,
                             addressDetailResponse.Huisnummer,
                             addressDetailResponse.Busnummer,
                             addressDetailResponse.Postcode,
                             verrijkteGemeentenaam.Gemeentenaam,
                             Adres.België));
        }

        return new VerrijkteAdressenUitGrar(enrichedLocaties);
    }
}
