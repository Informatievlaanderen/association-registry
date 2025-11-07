namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using Events;
using Marten;
using Marten.Services.Json.Transformations;
using Marten.Services.Json.Transformations.JsonNet;
using Schema.Persoonsgegevens;

public static class EventUpcaster
{
    public static StoreOptions UpcastEvents(this StoreOptions opts, Func<IQuerySession> querySessionFunc)
    {
        opts.Events.Upcast<VertegenwoordigerWerdToegevoegd, VertegenwoordigerWerdToegevoegdMetPersoonsgegevens>(
            async (vertegenwoordigerWerdToegevoegd, ct) =>
            {
                await using var session = querySessionFunc();

                var vertegenwoordigerPersoonsgegevens = await session.Query<VertegenwoordigerPersoonsgegevensDocument>()
                                                                     .Where(x => x.RefId == vertegenwoordigerWerdToegevoegd.RefId)
                                                                     .SingleOrDefaultAsync(ct);

                return new VertegenwoordigerWerdToegevoegdMetPersoonsgegevens(
                    VertegenwoordigerId: vertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                    IsPrimair: vertegenwoordigerWerdToegevoegd.IsPrimair,
                    vertegenwoordigerPersoonsgegevens is not null
                        ? new VertegenwoordigerPersoonsgegevens(
                            vertegenwoordigerPersoonsgegevens.Insz,
                            vertegenwoordigerPersoonsgegevens.Roepnaam,
                            vertegenwoordigerPersoonsgegevens.Rol,
                            vertegenwoordigerPersoonsgegevens.Voornaam,
                            vertegenwoordigerPersoonsgegevens.Achternaam,
                            vertegenwoordigerPersoonsgegevens.Email,
                            vertegenwoordigerPersoonsgegevens.Telefoon,
                            vertegenwoordigerPersoonsgegevens.Mobiel,
                            vertegenwoordigerPersoonsgegevens.SocialMedia
                        )
                        : null);
            });

        return opts;
    }
}
