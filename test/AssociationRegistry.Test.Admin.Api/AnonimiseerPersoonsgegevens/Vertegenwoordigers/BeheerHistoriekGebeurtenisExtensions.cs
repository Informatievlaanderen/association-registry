namespace AssociationRegistry.Test.Admin.Api.AnonimiseerPersoonsgegevens.Vertegenwoordigers;

using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using Events;

public static class BeheerHistoriekGebeurtenisExtensions
{
    public static IList<BeheerVerenigingHistoriekGebeurtenis> GetVertegenwoordigerGewijzigdGebeurtenissen(
        this IEnumerable<BeheerVerenigingHistoriekGebeurtenis> gebeurtenissen,
        int vertegenwoordigerId
    ) =>
        gebeurtenissen
            .Where(x =>
                x.Gebeurtenis == nameof(VertegenwoordigerWerdGewijzigd)
                && ((VertegenwoordigerData)x.Data).VertegenwoordigerId == vertegenwoordigerId
            )
            .ToList();

    public static IList<BeheerVerenigingHistoriekGebeurtenis> GetVertegenwoordigerWerdGewijzigdInKBOGebeurtenissen(
        this IEnumerable<BeheerVerenigingHistoriekGebeurtenis> gebeurtenissen,
        int vertegenwoordigerId
    ) =>
        gebeurtenissen
            .Where(x =>
                x.Gebeurtenis == nameof(VertegenwoordigerWerdGewijzigdInKBO)
                && ((KBOVertegenwoordigerData)x.Data).VertegenwoordigerId == vertegenwoordigerId
            )
            .ToList();

    public static BeheerVerenigingHistoriekGebeurtenis GetVertegenwoordigerToegevoegdGebeurtenis(
        this IEnumerable<BeheerVerenigingHistoriekGebeurtenis> gebeurtenissen,
        int vertegenwoordigerId
    ) =>
        gebeurtenissen.Single(x =>
            x.Gebeurtenis == nameof(VertegenwoordigerWerdToegevoegd)
            && ((VertegenwoordigerData)x.Data).VertegenwoordigerId == vertegenwoordigerId
        );

    public static BeheerVerenigingHistoriekGebeurtenis GetVertegenwoordigerVerwijderdGebeurtenis(
        this IEnumerable<BeheerVerenigingHistoriekGebeurtenis> gebeurtenissen,
        int vertegenwoordigerId
    ) =>
        gebeurtenissen.Single(x =>
            x.Gebeurtenis == nameof(VertegenwoordigerWerdVerwijderd)
            && ((VertegenwoordigerWerdVerwijderdData)x.Data).VertegenwoordigerId == vertegenwoordigerId
        );

    public static BeheerVerenigingHistoriekGebeurtenis GetVertegenwoordigerVerwijderdUitKBOGebeurtenis(
        this IEnumerable<BeheerVerenigingHistoriekGebeurtenis> gebeurtenissen,
        int vertegenwoordigerId
    ) =>
        gebeurtenissen.Single(x =>
            x.Gebeurtenis == nameof(VertegenwoordigerWerdVerwijderdUitKBO)
            && ((KBOVertegenwoordigerData)x.Data).VertegenwoordigerId == vertegenwoordigerId
        );

    public static BeheerVerenigingHistoriekGebeurtenis GetKszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendGebeurtenis(
        this IEnumerable<BeheerVerenigingHistoriekGebeurtenis> gebeurtenissen,
        int vertegenwoordigerId
    ) =>
        gebeurtenissen.Single(x =>
            x.Gebeurtenis == nameof(KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend)
            && ((VertegenwoordigerWerdVerwijderdData)x.Data).VertegenwoordigerId == vertegenwoordigerId
        );

    public static BeheerVerenigingHistoriekGebeurtenis GetKszSyncHeeftVertegenwoordigerAangeduidAlsOverledenGebeurtenis(
        this IEnumerable<BeheerVerenigingHistoriekGebeurtenis> gebeurtenissen,
        int vertegenwoordigerId
    ) =>
        gebeurtenissen.Single(x =>
            x.Gebeurtenis == nameof(KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden)
            && ((VertegenwoordigerWerdVerwijderdData)x.Data).VertegenwoordigerId == vertegenwoordigerId
        );

    public static BeheerVerenigingHistoriekGebeurtenis GetVertegenwoordigerWerdOvergenomenUitKBOGebeurtenis(
        this IEnumerable<BeheerVerenigingHistoriekGebeurtenis> gebeurtenissen,
        int vertegenwoordigerId
    ) =>
        gebeurtenissen.Single(x =>
            x.Gebeurtenis == nameof(VertegenwoordigerWerdOvergenomenUitKBO)
            && ((KBOVertegenwoordigerData)x.Data).VertegenwoordigerId == vertegenwoordigerId
        );

    public static BeheerVerenigingHistoriekGebeurtenis GetVertegenwoordigerWerdToegevoegdVanuitKBOGebeurtenis(
        this IEnumerable<BeheerVerenigingHistoriekGebeurtenis> gebeurtenissen,
        int vertegenwoordigerId
    ) =>
        gebeurtenissen.Single(x =>
            x.Gebeurtenis == nameof(VertegenwoordigerWerdToegevoegdVanuitKBO)
            && ((KBOVertegenwoordigerData)x.Data).VertegenwoordigerId == vertegenwoordigerId
        );
}
