namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning.Middleware;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions.Ipdc;
using Framework;
using Integrations.Ipdc.Clients;
using Integrations.Ipdc.Responses;
using Resources;

public class EnrichIpdcProductMiddleware
{
    // The message *has* to be first in the parameter list
    // Before or BeforeAsync tells Wolverine this method should be called before the actual action
    public static async Task<IpdcProduct> BeforeAsync(
        CommandEnvelope<RegistreerErkenningCommand> envelope,
        IIpdcClient ipdcClient
    )
    {
        var ipdcProductNummer = envelope.Command.Erkenning.IpdcProductNummer;

        var ipdcProductResponse = await ipdcClient.GetById(ipdcProductNummer);

        ValidateIpdcResponse(ipdcProductResponse, ipdcProductNummer);

        return new IpdcProduct()
        {
            Naam = ipdcProductResponse!.Naam!.Nl,
            Nummer = ipdcProductNummer,
        };
    }

    private static void ValidateIpdcResponse(
        IpdcProductResponse? ipdcProductResponse,
        string ipdcProductNummer)
    {
        Throw<IpdcException>.If(ipdcProductResponse == null,
                                string.Format(ExceptionMessages.IpdcLegeResponseException, ipdcProductNummer));

        Throw<IpdcException>.If(string.IsNullOrEmpty(ipdcProductResponse?.Naam?.Nl),
                                string.Format(ExceptionMessages.IpdcLegeNaamException, ipdcProductNummer));
    }
}
