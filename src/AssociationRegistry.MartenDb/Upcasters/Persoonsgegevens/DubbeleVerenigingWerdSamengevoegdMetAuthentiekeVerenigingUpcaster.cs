namespace AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;
using Marten;

/// <summary>
///     Vult de persoonsgegevens van de overgenomen gegevens aan vanuit de documenten onder de authentieke vereniging.
///     Werden ze intussen verwijderd, dan worden ze als geanonimiseerd getoond.
/// </summary>
public class DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVerenigingUpcaster
{
    private readonly Func<IQuerySession> _querySessionFunc;

    public DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVerenigingUpcaster(Func<IQuerySession> querySessionFunc)
    {
        _querySessionFunc = querySessionFunc;
    }

    public async Task<DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging> UpcastAsync(
        DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVerenigingZonderPersoonsgegevens @event,
        CancellationToken ct
    )
    {
        var overgenomen = @event.OvergenomenGegevens;

        var vertegenwoordigerRefIds = overgenomen
            .Vertegenwoordigers.SelectMany(x => new[] { x.OrigineleWaarde.RefId, x.NieuweWaarde.RefId })
            .ToList();

        var bankrekeningnummerRefIds = overgenomen
            .Bankrekeningnummers.SelectMany(x => new[] { x.OrigineleWaarde.RefId, x.NieuweWaarde.RefId })
            .ToList();

        await using var session = _querySessionFunc();

        var vertegenwoordigers = await LoadVertegenwoordigers(session, vertegenwoordigerRefIds, ct);
        var bankrekeningnummers = await LoadBankrekeningnummers(session, bankrekeningnummerRefIds, ct);

        return new DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
            @event.VCode,
            @event.VCodeDubbeleVereniging,
            overgenomen.With(
                overgenomen
                    .Vertegenwoordigers.Select(
                        x => new SamenvoegingDubbeleVereniging.OvergenomenGegeven<Registratiedata.Vertegenwoordiger>(
                            x.Actie,
                            x.Beschrijving,
                            ToVertegenwoordiger(x.OrigineleWaarde, vertegenwoordigers),
                            ToVertegenwoordiger(x.NieuweWaarde, vertegenwoordigers)
                        )
                    )
                    .ToArray(),
                overgenomen
                    .Bankrekeningnummers.Select(
                        x => new SamenvoegingDubbeleVereniging.OvergenomenGegeven<SamenvoegingDubbeleVereniging.Bankrekeningnummer>(
                            x.Actie,
                            x.Beschrijving,
                            ToBankrekeningnummer(x.OrigineleWaarde, bankrekeningnummers),
                            ToBankrekeningnummer(x.NieuweWaarde, bankrekeningnummers)
                        )
                    )
                    .ToArray()
            ),
            @event.NietOvergenomenGegevens
        );
    }

    private static async Task<Dictionary<Guid, VertegenwoordigerPersoonsgegevensDocument>> LoadVertegenwoordigers(
        IQuerySession session,
        List<Guid> refIds,
        CancellationToken ct
    )
    {
        if (refIds.Count == 0)
            return [];

        var documents = await session
            .Query<VertegenwoordigerPersoonsgegevensDocument>()
            .Where(x => refIds.Contains(x.RefId))
            .ToListAsync(ct);

        return documents.ToDictionary(x => x.RefId);
    }

    private static async Task<Dictionary<Guid, BankrekeningnummerPersoonsgegevensDocument>> LoadBankrekeningnummers(
        IQuerySession session,
        List<Guid> refIds,
        CancellationToken ct
    )
    {
        if (refIds.Count == 0)
            return [];

        var documents = await session
            .Query<BankrekeningnummerPersoonsgegevensDocument>()
            .Where(x => refIds.Contains(x.RefId))
            .ToListAsync(ct);

        return documents.ToDictionary(x => x.RefId);
    }

    private static Registratiedata.Vertegenwoordiger ToVertegenwoordiger(
        Registratiedata.VertegenwoordigerZonderPersoonsgegevens v,
        Dictionary<Guid, VertegenwoordigerPersoonsgegevensDocument> documents
    )
    {
        documents.TryGetValue(v.RefId, out var doc);

        return new Registratiedata.Vertegenwoordiger(
            v.VertegenwoordigerId,
            doc?.Insz ?? WellKnownAnonymousFields.Geanonimiseerd,
            v.IsPrimair,
            doc?.Roepnaam ?? WellKnownAnonymousFields.Geanonimiseerd,
            doc?.Rol ?? WellKnownAnonymousFields.Geanonimiseerd,
            doc?.Voornaam ?? WellKnownAnonymousFields.Geanonimiseerd,
            doc?.Achternaam ?? WellKnownAnonymousFields.Geanonimiseerd,
            doc?.Email ?? WellKnownAnonymousFields.Geanonimiseerd,
            doc?.Telefoon ?? WellKnownAnonymousFields.Geanonimiseerd,
            doc?.Mobiel ?? WellKnownAnonymousFields.Geanonimiseerd,
            doc?.SocialMedia ?? WellKnownAnonymousFields.Geanonimiseerd
        );
    }

    private static SamenvoegingDubbeleVereniging.Bankrekeningnummer ToBankrekeningnummer(
        SamenvoegingDubbeleVereniging.BankrekeningnummerZonderPersoonsgegevens b,
        Dictionary<Guid, BankrekeningnummerPersoonsgegevensDocument> documents
    )
    {
        documents.TryGetValue(b.RefId, out var doc);

        return new SamenvoegingDubbeleVereniging.Bankrekeningnummer(
            b.BankrekeningnummerId,
            doc?.Iban ?? WellKnownAnonymousFields.Geanonimiseerd,
            b.Doel,
            doc?.Titularissen ?? [],
            b.BevestigdDoor
        );
    }
}
