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
    public static async Task<EnrichedLocaties> BeforeAsync(
        CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> envelope,
        IGrarClient grarClient)
    {
        var enrichedLocaties = new List<EnrichedLocatie>();

        foreach (var locatie in envelope.Command.Locaties)
        {
            if (locatie.Adres is not null)
            {
                enrichedLocaties.Add(EnrichedLocatie.FromLocatieWithAdres(locatie));

                continue;
            }

            var addressDetailResponse = await grarClient.GetAddressById(locatie.AdresId.ToId(), CancellationToken.None);

            var postalInformation = await grarClient.GetPostalInformationDetail(addressDetailResponse.Postcode);

            var verrijkteGemeentenaam = GemeentenaamDecorator.VerrijkGemeentenaam(
                postalInformation,
                addressDetailResponse.Gemeente);

            enrichedLocaties.Add(EnrichedLocatie.FromLocatieWithAdresId(
                                     locatie,
                                     Adres.Create(addressDetailResponse.Straatnaam,
                                                  addressDetailResponse.Huisnummer,
                                                  addressDetailResponse.Busnummer,
                                                  addressDetailResponse.Postcode,
                                                  verrijkteGemeentenaam.Gemeentenaam,
                                                  Adres.België)));
        }

        return new EnrichedLocaties(enrichedLocaties.ToArray());
    }
}
