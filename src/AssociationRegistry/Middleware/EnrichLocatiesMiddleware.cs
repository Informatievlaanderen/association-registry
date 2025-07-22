namespace AssociationRegistry.Middleware;

using DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using Framework;
using GemeentenaamVerrijking;
using Grar.Clients;
using Vereniging;

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
