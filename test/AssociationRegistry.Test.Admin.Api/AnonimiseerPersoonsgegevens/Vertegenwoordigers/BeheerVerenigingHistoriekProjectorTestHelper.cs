namespace AssociationRegistry.Test.Admin.Api.AnonimiseerPersoonsgegevens.Vertegenwoordigers;

using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AutoFixture;
using Common.AutoFixture;
using Events;

public static class BeheerVerenigingHistoriekProjectorTestHelper
{
    public static BeheerVerenigingHistoriekDocument Create(
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd vzer
    ) =>
        BeheerVerenigingHistoriekProjector.Create(
            new TestEvent<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>(vzer)
        );

    public static BeheerVerenigingHistoriekDocument Create(FeitelijkeVerenigingWerdGeregistreerd fv) =>
        BeheerVerenigingHistoriekProjector.Create(new TestEvent<FeitelijkeVerenigingWerdGeregistreerd>(fv));

    public static BeheerVerenigingHistoriekDocument CreateKBO(
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd kbo
    ) =>
        BeheerVerenigingHistoriekProjector.Create(
            new TestEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>(kbo)
        );

    public static BeheerVerenigingHistoriekDocument Apply(
        this BeheerVerenigingHistoriekDocument doc,
        VertegenwoordigerWerdToegevoegd @event
    )
    {
        BeheerVerenigingHistoriekProjector.Apply(new TestEvent<VertegenwoordigerWerdToegevoegd>(@event), doc);
        return doc;
    }

    public static BeheerVerenigingHistoriekDocument Apply(
        this BeheerVerenigingHistoriekDocument doc,
        VertegenwoordigerWerdGewijzigd @event
    )
    {
        BeheerVerenigingHistoriekProjector.Apply(new TestEvent<VertegenwoordigerWerdGewijzigd>(@event), doc);
        return doc;
    }

    public static BeheerVerenigingHistoriekDocument Apply(
        this BeheerVerenigingHistoriekDocument doc,
        VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd @event
    )
    {
        BeheerVerenigingHistoriekProjector.Apply(
            new TestEvent<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd>(@event),
            doc
        );
        return doc;
    }

    public static BeheerVerenigingHistoriekDocument ApplyVertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd(
        this BeheerVerenigingHistoriekDocument doc,
        int vertegenwoordigerId
    )
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var @event = fixture.Create<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd>() with
        {
            VertegenwoordigerId = vertegenwoordigerId,
        };

        BeheerVerenigingHistoriekProjector.Apply(
            new TestEvent<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd>(@event),
            doc
        );

        return doc;
    }

    public static BeheerVerenigingHistoriekDocument ApplyVertegenwoordigerWerdGewijzigd(
        this BeheerVerenigingHistoriekDocument doc,
        int vertegenwoordigerId
    )
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var @event = fixture.Create<VertegenwoordigerWerdGewijzigd>() with
        {
            VertegenwoordigerId = vertegenwoordigerId,
            IsPrimair = false,
        };

        BeheerVerenigingHistoriekProjector.Apply(new TestEvent<VertegenwoordigerWerdGewijzigd>(@event), doc);

        return doc;
    }

    public static BeheerVerenigingHistoriekDocument ApplyVertegenwoordigerWerdGewijzigdInKBO(
        this BeheerVerenigingHistoriekDocument doc,
        int vertegenwoordigerId
    )
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var @event = fixture.Create<VertegenwoordigerWerdGewijzigdInKBO>() with
        {
            VertegenwoordigerId = vertegenwoordigerId,
        };

        BeheerVerenigingHistoriekProjector.Apply(new TestEvent<VertegenwoordigerWerdGewijzigdInKBO>(@event), doc);

        return doc;
    }

    public static BeheerVerenigingHistoriekDocument ApplyVertegenwoordigerWerdVerwijderd(
        this BeheerVerenigingHistoriekDocument doc,
        int vertegenwoordigerId
    )
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var @event = fixture.Create<VertegenwoordigerWerdVerwijderd>() with
        {
            VertegenwoordigerId = vertegenwoordigerId,
        };

        BeheerVerenigingHistoriekProjector.Apply(new TestEvent<VertegenwoordigerWerdVerwijderd>(@event), doc);

        return doc;
    }

    public static BeheerVerenigingHistoriekDocument ApplyVertegenwoordigerWerdVerwijderdUitKBO(
        this BeheerVerenigingHistoriekDocument doc,
        int vertegenwoordigerId
    )
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var @event = fixture.Create<VertegenwoordigerWerdVerwijderdUitKBO>() with
        {
            VertegenwoordigerId = vertegenwoordigerId,
        };

        BeheerVerenigingHistoriekProjector.Apply(new TestEvent<VertegenwoordigerWerdVerwijderdUitKBO>(@event), doc);

        return doc;
    }

    public static BeheerVerenigingHistoriekDocument ApplyVertegenwoordigerWerdToegevoegd(
        this BeheerVerenigingHistoriekDocument doc,
        int vertegenwoordigerId
    )
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var @event = fixture.Create<VertegenwoordigerWerdToegevoegd>() with
        {
            VertegenwoordigerId = vertegenwoordigerId,
            IsPrimair = false,
        };

        BeheerVerenigingHistoriekProjector.Apply(new TestEvent<VertegenwoordigerWerdToegevoegd>(@event), doc);

        return doc;
    }

    public static BeheerVerenigingHistoriekDocument ApplyKszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend(
        this BeheerVerenigingHistoriekDocument doc,
        int vertegenwoordigerId
    )
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var @event = fixture.Create<KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend>() with
        {
            VertegenwoordigerId = vertegenwoordigerId,
        };

        BeheerVerenigingHistoriekProjector.Apply(
            new TestEvent<KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend>(@event),
            doc
        );

        return doc;
    }

    public static BeheerVerenigingHistoriekDocument ApplyKszSyncHeeftVertegenwoordigerAangeduidAlsOverleden(
        this BeheerVerenigingHistoriekDocument doc,
        int vertegenwoordigerId
    )
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var @event = fixture.Create<KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden>() with
        {
            VertegenwoordigerId = vertegenwoordigerId,
        };

        BeheerVerenigingHistoriekProjector.Apply(
            new TestEvent<KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden>(@event),
            doc
        );

        return doc;
    }

    public static BeheerVerenigingHistoriekDocument ApplyVertegenwoordigerWerdOvergenomenUitKBO(
        this BeheerVerenigingHistoriekDocument doc,
        int vertegenwoordigerId
    )
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var @event = fixture.Create<VertegenwoordigerWerdOvergenomenUitKBO>() with
        {
            VertegenwoordigerId = vertegenwoordigerId,
        };

        BeheerVerenigingHistoriekProjector.Apply(new TestEvent<VertegenwoordigerWerdOvergenomenUitKBO>(@event), doc);

        return doc;
    }

    public static BeheerVerenigingHistoriekDocument ApplyVertegenwoordigerWerdToegevoegdVanuitKBO(
        this BeheerVerenigingHistoriekDocument doc,
        int vertegenwoordigerId
    )
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var @event = fixture.Create<VertegenwoordigerWerdToegevoegdVanuitKBO>() with
        {
            VertegenwoordigerId = vertegenwoordigerId,
        };

        BeheerVerenigingHistoriekProjector.Apply(new TestEvent<VertegenwoordigerWerdToegevoegdVanuitKBO>(@event), doc);

        return doc;
    }
}
