namespace AssociationRegistry.MartenDb.Transformers;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;
using Store;

/// <summary>
///     Bewaart de persoonsgegevens van vertegenwoordigers en bankrekeningnummers met een nieuwe RefId onder de
///     authentieke vereniging. Ze verwijzen dus niet naar de persoonsgegevens van de dubbele vereniging, die verdwijnen
///     wanneer haar bewaartermijn verloopt.
///     Niet-overgenomen vertegenwoordigers en bankrekeningnummers bevatten enkel hun id en worden ongewijzigd bewaard.
/// </summary>
public class DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVerenigingTransformer : IPersoonsgegevensEventTransformer
{
    public Type EventType => typeof(DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging);

    public Type PersistedEventType =>
        typeof(DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVerenigingZonderPersoonsgegevens);

    public EventTransformationResult Transform(IEvent @event, string vCode)
    {
        var originalEvent = (DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging)@event;
        var splitter = new PersoonsgegevensSplitter(VCode.Hydrate(vCode));

        var overgenomen = originalEvent.OvergenomenGegevens;

        var transformedEvent = new DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVerenigingZonderPersoonsgegevens(
            originalEvent.VCode,
            originalEvent.VCodeDubbeleVereniging,
            overgenomen.With(
                overgenomen
                    .Vertegenwoordigers.Select(
                        x => new SamenvoegingDubbeleVereniging.OvergenomenGegeven<Registratiedata.VertegenwoordigerZonderPersoonsgegevens>(
                            x.Actie,
                            x.Beschrijving,
                            splitter.Split(x.OrigineleWaarde),
                            splitter.Split(x.NieuweWaarde)
                        )
                    )
                    .ToArray(),
                overgenomen
                    .Bankrekeningnummers.Select(
                        x => new SamenvoegingDubbeleVereniging.OvergenomenGegeven<SamenvoegingDubbeleVereniging.BankrekeningnummerZonderPersoonsgegevens>(
                            x.Actie,
                            x.Beschrijving,
                            splitter.Split(x.OrigineleWaarde),
                            splitter.Split(x.NieuweWaarde)
                        )
                    )
                    .ToArray()
            ),
            originalEvent.NietOvergenomenGegevens
        );

        return new EventTransformationResult(transformedEvent, splitter.ExtractedPersoonsgegevens.ToArray());
    }

    private class PersoonsgegevensSplitter(VCode vCode)
    {
        public List<IPersoonsgegevens> ExtractedPersoonsgegevens { get; } = [];

        public Registratiedata.VertegenwoordigerZonderPersoonsgegevens Split(Registratiedata.Vertegenwoordiger v)
        {
            var refId = Guid.NewGuid();

            ExtractedPersoonsgegevens.Add(
                new VertegenwoordigerPersoonsgegevens(
                    refId,
                    vCode,
                    v.VertegenwoordigerId,
                    Insz.Hydrate(v.Insz),
                    v.Roepnaam,
                    v.Rol,
                    v.Voornaam,
                    v.Achternaam,
                    v.Email,
                    v.Telefoon,
                    v.Mobiel,
                    v.SocialMedia
                )
            );

            return new Registratiedata.VertegenwoordigerZonderPersoonsgegevens(
                refId,
                v.VertegenwoordigerId,
                v.IsPrimair
            );
        }

        public SamenvoegingDubbeleVereniging.BankrekeningnummerZonderPersoonsgegevens Split(
            SamenvoegingDubbeleVereniging.Bankrekeningnummer b
        )
        {
            var refId = Guid.NewGuid();

            ExtractedPersoonsgegevens.Add(
                new BankrekeningnummerPersoonsgegevens(refId, vCode, b.BankrekeningnummerId, b.Iban, b.Titularissen)
            );

            return new SamenvoegingDubbeleVereniging.BankrekeningnummerZonderPersoonsgegevens(
                refId,
                b.BankrekeningnummerId,
                b.Doel,
                b.BevestigdDoor
            );
        }
    }
}
