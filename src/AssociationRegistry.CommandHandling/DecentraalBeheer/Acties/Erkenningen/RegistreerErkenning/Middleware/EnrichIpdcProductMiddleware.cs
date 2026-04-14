namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning.Middleware;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Framework;
using Integrations.Ipdc.Clients;

public class EnrichIpdcProductMiddleware
{
    // The message *has* to be first in the parameter list
    // Before or BeforeAsync tells Wolverine this method should be called before the actual action
    public static async Task<IpdcProduct> BeforeAsync(
        CommandEnvelope<RegistreerErkenningCommand> envelope,
        IIpdcClient ipdcClient
    )
    {
        var ipdcProductResponse = await ipdcClient.GetById(envelope.Command.Erkenning.IpdcProductNummer);

        return new IpdcProduct()
        {
            Naam = ipdcProductResponse.Naam.Nl,
            Nummer = envelope.Command.Erkenning.IpdcProductNummer,
        };
    }
}
