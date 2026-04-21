namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning.Middleware;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions.Wegwijs;
using Framework;
using Integrations.Wegwijs.Clients;
using Resources;

public class EnrichGegevensInitiatorMiddleware
{
    // The message *has* to be first in the parameter list
    // Before or BeforeAsync tells Wolverine this method should be called before the actual action
    public static async Task<GegevensInitiator> BeforeAsync(
        CommandEnvelope<RegistreerErkenningCommand> envelope,
        IWegwijsClient wegwijsClient
    )
    {
        var initiator = envelope.Metadata.Initiator;

        var organisationResponse = await wegwijsClient.GetOrganisationByOvoCode(initiator);

        Throw<WegwijsException>.If(string.IsNullOrWhiteSpace(organisationResponse.Name),
                                   string.Format(ExceptionMessages.WegwijsLegeOrganisatieNaamException,
                                                 initiator));

        return new GegevensInitiator
        {
            OvoCode = initiator,
            Naam = organisationResponse.Name,
        };
    }
}
