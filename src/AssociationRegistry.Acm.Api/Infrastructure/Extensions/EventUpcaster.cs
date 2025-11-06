namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using Events;
using Events.Enriched;
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
                        ? VertegenwoordigerPersoonsgegevens.Create(
                            insz: vertegenwoordigerPersoonsgegevens.Insz,
                            roepnaam: vertegenwoordigerPersoonsgegevens.Roepnaam,
                            rol: vertegenwoordigerPersoonsgegevens.Rol,
                            voornaam: vertegenwoordigerPersoonsgegevens.Voornaam,
                            achternaam: vertegenwoordigerPersoonsgegevens.Achternaam,
                            email: vertegenwoordigerPersoonsgegevens.Email,
                            telefoon: vertegenwoordigerPersoonsgegevens.Telefoon,
                            mobiel: vertegenwoordigerPersoonsgegevens.Mobiel,
                            socialMedia: vertegenwoordigerPersoonsgegevens.SocialMedia
                        )
                        : VertegenwoordigerPersoonsgegevens.Verlopen);
            });

        return opts;
    }
}
