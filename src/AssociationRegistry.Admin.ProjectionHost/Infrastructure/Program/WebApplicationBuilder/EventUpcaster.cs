namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using Events;
using Events.Enriched;
using Marten;
using Persoonsgegevens;

public static class EventUpcaster
{
    private const string Onbekend = "<Onbekend>";

    public static StoreOptions UpcastEvents(this StoreOptions opts, IServiceProvider sp)
    {
        Func<IQuerySession> openQuerySession = () =>
            sp.GetRequiredService<IDocumentStore>().QuerySession();

        opts.Events.Upcast<VertegenwoordigerWerdToegevoegd, VertegenwoordigerWerdToegevoegdMetPersoonsgegevens>(
            async (vertegenwoordigerWerdToegevoegd, ct) =>
            {
                await using var session = openQuerySession();

                var vertegenwoordigerPersoonsgegevens = await session.Query<VertegenwoordigerPersoonsgegevens>()
                                      .Where(x => x.RefId == vertegenwoordigerWerdToegevoegd.RefId)
                                      .SingleOrDefaultAsync(ct);

                return new VertegenwoordigerWerdToegevoegdMetPersoonsgegevens(
                    VertegenwoordigerId: vertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                    Insz: vertegenwoordigerPersoonsgegevens?.Insz ?? Onbekend,
                    IsPrimair: vertegenwoordigerWerdToegevoegd.IsPrimair,
                    Roepnaam: vertegenwoordigerPersoonsgegevens?.Roepnaam ?? Onbekend,
                    Rol: vertegenwoordigerPersoonsgegevens?.Rol ?? Onbekend,
                    Voornaam: vertegenwoordigerPersoonsgegevens?.Voornaam ?? Onbekend,
                    Achternaam: vertegenwoordigerPersoonsgegevens?.Achternaam ?? Onbekend,
                    Email: vertegenwoordigerPersoonsgegevens?.Email ?? Onbekend,
                    Telefoon: vertegenwoordigerPersoonsgegevens?.Telefoon ?? Onbekend,
                    Mobiel: vertegenwoordigerPersoonsgegevens?.Mobiel ?? Onbekend,
                    SocialMedia: vertegenwoordigerPersoonsgegevens?.SocialMedia ?? Onbekend
                );
            });

        return opts;
    }
}
