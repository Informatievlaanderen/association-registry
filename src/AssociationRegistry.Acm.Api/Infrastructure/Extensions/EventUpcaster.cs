namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using Events;
using Marten;
using Persoonsgegevens;

public static class EventUpcaster
{
    public static StoreOptions UpcastEvents(this StoreOptions opts, Func<IQuerySession> querySessionFunc)
    {
        opts.Events.Upcast<VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens, VertegenwoordigerWerdToegevoegd>(
            async (vertegenwoordigerWerdToegevoegd, ct) =>
            {
                await using var session = querySessionFunc();

                var vertegenwoordigerPersoonsgegevens = await session.Query<VertegenwoordigerPersoonsgegevensDocument>()
                                                                     .Where(x => x.RefId == vertegenwoordigerWerdToegevoegd.RefId)
                                                                     .SingleOrDefaultAsync(ct);

                return new VertegenwoordigerWerdToegevoegd(
                    VertegenwoordigerId: vertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                    vertegenwoordigerPersoonsgegevens.Insz,
                    vertegenwoordigerWerdToegevoegd.IsPrimair,
                    vertegenwoordigerPersoonsgegevens.Roepnaam,
                    vertegenwoordigerPersoonsgegevens.Rol,
                    vertegenwoordigerPersoonsgegevens.Voornaam,
                    vertegenwoordigerPersoonsgegevens.Achternaam,
                    vertegenwoordigerPersoonsgegevens.Email,
                    vertegenwoordigerPersoonsgegevens.Telefoon,
                    vertegenwoordigerPersoonsgegevens.Mobiel,
                    vertegenwoordigerPersoonsgegevens.SocialMedia
                );
            });

        return opts;
    }
}
