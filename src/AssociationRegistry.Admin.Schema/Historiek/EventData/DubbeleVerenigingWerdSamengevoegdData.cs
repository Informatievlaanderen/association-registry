namespace AssociationRegistry.Admin.Schema.Historiek.EventData;

using Events;
using Gegevens = Events.SamenvoegingDubbeleVereniging;

/// <summary>
///     De samenvoeging zoals ze in de historiek staat. Vertegenwoordigers staan erin zoals bij andere gebeurtenissen,
///     zonder INSZ, zodat ze geanonimiseerd kunnen worden wanneer hun bewaartermijn verloopt.
/// </summary>
#pragma warning disable CS0618
public record DubbeleVerenigingWerdSamengevoegdData(
    string VCode,
    string VCodeDubbeleVereniging,
    Gegevens.OvergenomenGegevensPerGroep<VertegenwoordigerData, Registratiedata.Bankrekeningnummer> OvergenomenGegevens,
    Gegevens.NietOvergenomenGegevensPerGroep<
        VertegenwoordigerData,
        Registratiedata.Bankrekeningnummer
    > NietOvergenomenGegevens
)
{
    public static DubbeleVerenigingWerdSamengevoegdData Create(
        DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging e
    ) =>
        new(
            e.VCode,
            e.VCodeDubbeleVereniging,
            e.OvergenomenGegevens.With(
                e.OvergenomenGegevens.Vertegenwoordigers.Select(
                        x => new Gegevens.OvergenomenGegeven<VertegenwoordigerData>(
                            x.Actie,
                            x.Beschrijving,
                            VertegenwoordigerData.Create(x.OrigineleWaarde),
                            VertegenwoordigerData.Create(x.NieuweWaarde)
                        )
                    )
                    .ToArray(),
                e.OvergenomenGegevens.Bankrekeningnummers
            ),
            e.NietOvergenomenGegevens.With(
                e.NietOvergenomenGegevens.Vertegenwoordigers.Select(
                        x => new Gegevens.NietOvergenomenGegeven<VertegenwoordigerData>(
                            x.Actie,
                            x.Reden,
                            x.Beschrijving,
                            VertegenwoordigerData.Create(x.Waarde)
                        )
                    )
                    .ToArray(),
                e.NietOvergenomenGegevens.Bankrekeningnummers
            )
        );

    /// <summary>
    ///     De persoonsgegevens van de samenvoeging worden bewaard onder deze vereniging, per vertegenwoordiger-id.
    ///     Verloopt de bewaartermijn van een id, dan worden al die waarden verwijderd; hier dus ook.
    /// </summary>
    public bool HasVertegenwoordiger(int vertegenwoordigerId) =>
        OvergenomenGegevens.Vertegenwoordigers.Any(x =>
            x.OrigineleWaarde.VertegenwoordigerId == vertegenwoordigerId
            || x.NieuweWaarde.VertegenwoordigerId == vertegenwoordigerId
        ) || NietOvergenomenGegevens.Vertegenwoordigers.Any(x => x.Waarde.VertegenwoordigerId == vertegenwoordigerId);

    public DubbeleVerenigingWerdSamengevoegdData MakeAnonymous(int vertegenwoordigerId) =>
        this with
        {
            OvergenomenGegevens = OvergenomenGegevens.With(
                OvergenomenGegevens
                    .Vertegenwoordigers.Select(x =>
                        x with
                        {
                            OrigineleWaarde = MakeAnonymous(x.OrigineleWaarde, vertegenwoordigerId),
                            NieuweWaarde = MakeAnonymous(x.NieuweWaarde, vertegenwoordigerId),
                        }
                    )
                    .ToArray(),
                OvergenomenGegevens.Bankrekeningnummers
            ),
            NietOvergenomenGegevens = NietOvergenomenGegevens.With(
                NietOvergenomenGegevens
                    .Vertegenwoordigers.Select(x => x with { Waarde = MakeAnonymous(x.Waarde, vertegenwoordigerId) })
                    .ToArray(),
                NietOvergenomenGegevens.Bankrekeningnummers
            ),
        };

    private static VertegenwoordigerData MakeAnonymous(
        VertegenwoordigerData vertegenwoordiger,
        int vertegenwoordigerId
    ) =>
        vertegenwoordiger.VertegenwoordigerId == vertegenwoordigerId
            ? vertegenwoordiger.MakeAnonymous()
            : vertegenwoordiger;
}
#pragma warning restore CS0618
