namespace AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;

using Events;
using Schema.Historiek;
using Schema.Historiek.EventData;

public static class TeAnonimiserenGebeurtenissenConfig
{
    public static readonly IReadOnlyDictionary<
        string,
        Func<BeheerVerenigingHistoriekGebeurtenis, int, BeheerVerenigingHistoriekGebeurtenis?>
    > TeAnonimiserenGebeurtenissen = new Dictionary<
        string,
        Func<BeheerVerenigingHistoriekGebeurtenis, int, BeheerVerenigingHistoriekGebeurtenis?>
    >
    {
        [nameof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)] = (g, id) =>
            VertegenwoordigerGebeurtenissenAnonymizer.AnonimiseerMultiple<VerenigingWerdGeregistreerdData>(g, id),

        [nameof(FeitelijkeVerenigingWerdGeregistreerd)] = (g, id) =>
            VertegenwoordigerGebeurtenissenAnonymizer.AnonimiseerMultiple<VerenigingWerdGeregistreerdData>(g, id),

        [nameof(VertegenwoordigerWerdToegevoegd)] = (g, id) =>
            VertegenwoordigerGebeurtenissenAnonymizer.AnonimiseerSingle<VertegenwoordigerData>(
                g,
                id,
                BeheerHistoriekBeschrijvingen.VertegenwoordigerWerdToegevoegd
            ),

        [nameof(VertegenwoordigerWerdGewijzigd)] = (g, id) =>
            VertegenwoordigerGebeurtenissenAnonymizer.AnonimiseerSingle<VertegenwoordigerData>(
                g,
                id,
                BeheerHistoriekBeschrijvingen.VertegenwoordigerWerdGewijzigd
            ),

        [nameof(VertegenwoordigerWerdVerwijderd)] = (g, id) =>
            VertegenwoordigerGebeurtenissenAnonymizer.AnonimiseerSingle<VertegenwoordigerWerdVerwijderdData>(
                g,
                id,
                BeheerHistoriekBeschrijvingen.VertegenwoordigerWerdVerwijderd
            ),

        [nameof(KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend)] = (g, id) =>
            VertegenwoordigerGebeurtenissenAnonymizer.AnonimiseerSingle<VertegenwoordigerWerdVerwijderdData>(
                g,
                id,
                BeheerHistoriekBeschrijvingen.KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend
            ),

        [nameof(KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden)] = (g, id) =>
            VertegenwoordigerGebeurtenissenAnonymizer.AnonimiseerSingle<VertegenwoordigerWerdVerwijderdData>(
                g,
                id,
                BeheerHistoriekBeschrijvingen.KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden
            ),

        [nameof(VertegenwoordigerWerdOvergenomenUitKBO)] = (g, id) =>
            VertegenwoordigerGebeurtenissenAnonymizer.AnonimiseerSingle<KBOVertegenwoordigerData>(
                g,
                id,
                BeheerHistoriekBeschrijvingen.VertegenwoordigerWerdOvergenomenUitKBO
            ),

        [nameof(VertegenwoordigerWerdToegevoegdVanuitKBO)] = (g, id) =>
            VertegenwoordigerGebeurtenissenAnonymizer.AnonimiseerSingle<KBOVertegenwoordigerData>(
                g,
                id,
                BeheerHistoriekBeschrijvingen.VertegenwoordigerWerdToegevoegdVanuitKBO
            ),

        [nameof(VertegenwoordigerWerdGewijzigdInKBO)] = (g, id) =>
            VertegenwoordigerGebeurtenissenAnonymizer.AnonimiseerSingle<KBOVertegenwoordigerData>(
                g,
                id,
                BeheerHistoriekBeschrijvingen.VertegenwoordigerWerdGewijzigdInKBO
            ),

        [nameof(VertegenwoordigerWerdVerwijderdUitKBO)] = (g, id) =>
            VertegenwoordigerGebeurtenissenAnonymizer.AnonimiseerSingle<KBOVertegenwoordigerData>(
                g,
                id,
                BeheerHistoriekBeschrijvingen.VertegenwoordigerWerdVerwijderdUitKBO
            ),
    };
}
