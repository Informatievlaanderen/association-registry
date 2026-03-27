namespace AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;

using Schema.Historiek;
using Schema.Historiek.EventData;

public static class VertegenwoordigerGebeurtenissenAnonymizer
{
    public static BeheerVerenigingHistoriekGebeurtenis Anonymize(
        BeheerVerenigingHistoriekGebeurtenis gebeurtenis,
        int vertegenwoordigerId
    )
    {
        if (
            TeAnonimiserenGebeurtenissenConfig.TeAnonimiserenGebeurtenissen.TryGetValue(
                gebeurtenis.Gebeurtenis,
                out var anonymize
            )
        )
        {
            return anonymize(gebeurtenis, vertegenwoordigerId) ?? gebeurtenis;
        }

        return gebeurtenis;
    }

    public static BeheerVerenigingHistoriekGebeurtenis? AnonimiseerSingle<T>(
        BeheerVerenigingHistoriekGebeurtenis g,
        int vertegenwoordigerId,
        string beschrijving
    )
        where T : class, IVertegenwoordigerData<T> =>
        g.Data is not T data || data.VertegenwoordigerId != vertegenwoordigerId
            ? null
            : g with
            {
                Beschrijving = beschrijving,
                Data = data.MakeAnonymous(),
            };

    public static BeheerVerenigingHistoriekGebeurtenis? AnonimiseerMultiple<T>(
        BeheerVerenigingHistoriekGebeurtenis g,
        int vertegenwoordigerId
    )
        where T : class, IHasVertegenwoordigers =>
        g.Data is not T data || !data.Vertegenwoordigers.Any(v => v.VertegenwoordigerId == vertegenwoordigerId)
            ? null
            : g with
            {
                Data = data.MakeAnonymous(vertegenwoordigerId),
            };
}
