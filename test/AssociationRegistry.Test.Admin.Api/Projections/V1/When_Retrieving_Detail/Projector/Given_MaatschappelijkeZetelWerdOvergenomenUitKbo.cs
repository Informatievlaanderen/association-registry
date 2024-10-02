namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Detail.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Formats;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging.Bronnen;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_MaatschappelijkeZetelWerdOvergenomenUitKbo
{
    [Fact]
    public void Then_it_adds_a_locatie()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var maatschappelijkeZetelWerdOvergenomenUitKbo = fixture.Create<TestEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(maatschappelijkeZetelWerdOvergenomenUitKbo, doc);

        doc.Locaties.Should().HaveCount(4);

        var locatie = doc.Locaties.Should()
                         .ContainSingle(locatie => locatie.LocatieId == maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.LocatieId)
                         .Subject;

        locatie.Should().BeEquivalentTo(
            new Locatie
            {
                JsonLdMetadata = new JsonLdMetadata
                {
                    Id = JsonLdType.Locatie.CreateWithIdValues(
                        doc.VCode, maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.LocatieId.ToString()),
                    Type = JsonLdType.Locatie.Type,
                },
                LocatieId = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.LocatieId,
                IsPrimair = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.IsPrimair,
                Naam = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Naam,
                Locatietype = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Locatietype,
                Adres = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Adres is null
                    ? null
                    : new Adres
                    {
                        JsonLdMetadata = new JsonLdMetadata
                        {
                            Id = JsonLdType.Adres.CreateWithIdValues(
                                doc.VCode, maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.LocatieId.ToString()),
                            Type = JsonLdType.Adres.Type,
                        },
                        Straatnaam = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Adres.Straatnaam,
                        Huisnummer = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Adres.Huisnummer,
                        Busnummer = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Adres.Busnummer,
                        Postcode = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Adres.Postcode,
                        Gemeente = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Adres.Gemeente,
                        Land = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Adres.Land,
                    },
                Adresvoorstelling = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Adres.ToAdresString(),
                AdresId = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.AdresId is null
                    ? null
                    : new AdresId
                    {
                        Broncode = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.AdresId?.Broncode,
                        Bronwaarde = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.AdresId?.Bronwaarde,
                    },
                VerwijstNaar = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.AdresId is null
                    ? null
                    : new AdresVerwijzing
                    {
                        JsonLdMetadata = new JsonLdMetadata
                        {
                            Id = JsonLdType.AdresVerwijzing.CreateWithIdValues(
                                maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.AdresId?.Bronwaarde.Split('/').Last()),
                            Type = JsonLdType.AdresVerwijzing.Type,
                        },
                    },
                Bron = Bron.KBO,
            });

        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
    }
}

[UnitTest]
public class Given_MaatschappelijkeZetelWerdGewijzigdInKbo
{
    [Fact]
    public void Then_it_updates_a_locatie()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var maatschappelijkeZetelWerdOvergenomenUitKbo = fixture.Create<TestEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo>>();

        var maatschappelijkeZetelWerdGewijzigdInKbo = new TestEvent<MaatschappelijkeZetelWerdGewijzigdInKbo>(
            new MaatschappelijkeZetelWerdGewijzigdInKbo(
                Locatie: fixture.Create<Registratiedata.Locatie>() with
                {
                    LocatieId = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.LocatieId,
                })
        );

        var doc = new BeheerVerenigingDetailDocument();

        BeheerVerenigingDetailProjector.Apply(maatschappelijkeZetelWerdOvergenomenUitKbo, doc);
        BeheerVerenigingDetailProjector.Apply(maatschappelijkeZetelWerdGewijzigdInKbo, doc);

        doc.Locaties.Should().HaveCount(1);

        var locatie = doc.Locaties.Should()
                         .ContainSingle(locatie => locatie.LocatieId == maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.LocatieId)
                         .Subject;

        locatie.Should().BeEquivalentTo(
            new Locatie
            {
                JsonLdMetadata = new JsonLdMetadata
                {
                    Id = JsonLdType.Locatie.CreateWithIdValues(
                        doc.VCode, maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.LocatieId.ToString()),
                    Type = JsonLdType.Locatie.Type,
                },
                LocatieId = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.LocatieId,
                IsPrimair = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.IsPrimair,
                Naam = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Naam,
                Locatietype = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Locatietype,
                Adres = maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.Adres is null
                    ? null
                    : new Adres
                    {
                        JsonLdMetadata = new JsonLdMetadata
                        {
                            Id = JsonLdType.Adres.CreateWithIdValues(
                                doc.VCode, maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.LocatieId.ToString()),
                            Type = JsonLdType.Adres.Type,
                        },
                        Straatnaam = maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.Adres.Straatnaam,
                        Huisnummer = maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.Adres.Huisnummer,
                        Busnummer = maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.Adres.Busnummer,
                        Postcode = maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.Adres.Postcode,
                        Gemeente = maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.Adres.Gemeente,
                        Land = maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.Adres.Land,
                    },
                Adresvoorstelling = maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.Adres.ToAdresString(),
                AdresId = maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.AdresId is null
                    ? null
                    : new AdresId
                    {
                        Broncode = maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.AdresId?.Broncode,
                        Bronwaarde = maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.AdresId?.Bronwaarde,
                    },
                VerwijstNaar = maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.AdresId is null
                    ? null
                    : new AdresVerwijzing
                    {
                        JsonLdMetadata = new JsonLdMetadata
                        {
                            Id = JsonLdType.AdresVerwijzing.CreateWithIdValues(
                                maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.AdresId?.Bronwaarde.Split('/').Last()),
                            Type = JsonLdType.AdresVerwijzing.Type,
                        },
                    },
                Bron = Bron.KBO,
            });

        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
    }
}

[UnitTest]
public class Given_MaatschappelijkeZetelWerdVerwijderdUitKbo
{
    [Fact]
    public void Then_it_removes_a_locatie()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var maatschappelijkeZetelWerdOvergenomenUitKbo = fixture.Create<TestEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo>>();

        var maatschappelijkeZetelWerdVerwijderdUitKbo = new TestEvent<MaatschappelijkeZetelWerdVerwijderdUitKbo>(
            new MaatschappelijkeZetelWerdVerwijderdUitKbo(
                Locatie: maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie)
        );

        var doc = new BeheerVerenigingDetailDocument();

        BeheerVerenigingDetailProjector.Apply(maatschappelijkeZetelWerdOvergenomenUitKbo, doc);
        BeheerVerenigingDetailProjector.Apply(maatschappelijkeZetelWerdVerwijderdUitKbo, doc);

        doc.Locaties.Should().HaveCount(0);
    }
}
