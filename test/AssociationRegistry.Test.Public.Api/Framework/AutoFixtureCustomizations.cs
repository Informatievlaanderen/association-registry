namespace AssociationRegistry.Test.Public.Api.Framework;

using AssociationRegistry.Public.Schema.Detail;
using AssociationRegistry.Public.Schema.Search;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using JsonLdContext;
using Vereniging;

public static class AutoFixtureCustomizations
{
    public static Fixture CustomizePublicApi(this Fixture fixture)
    {
        fixture.CustomizeDomain();

        fixture.CustomizeTestEvent(typeof(TestEvent<>));

        fixture.CustomizeVerenigingZoekDocument();

        return fixture;
    }

    private static void CustomizeVerenigingZoekDocument(this IFixture fixture, bool withoutWerkingsgebieden = false)
    {
        fixture.Customize<VerenigingZoekDocument>(
            composer => composer.FromFactory<int>(
                _ =>
                {

                    var document = new VerenigingZoekDocument();
                    document.VCode = fixture.Create<VCode>();
                    document.Locaties = fixture.CreateMany<AssociationRegistry.Public.Schema.Search.VerenigingZoekDocument.Types.Locatie>().DistinctBy(l => l.Locatietype).ToArray();
                    document.Naam = fixture.Create<string>();
                    document.Doelgroep = fixture.Create<AssociationRegistry.Public.Schema.Search.VerenigingZoekDocument.Types.Doelgroep>();

                    document.HoofdactiviteitenVerenigingsloket = fixture.CreateMany<HoofdactiviteitVerenigingsloket>()
                                                                       .Select(x => new AssociationRegistry.Public.Schema.Search.VerenigingZoekDocument.Types.HoofdactiviteitVerenigingsloket()
                                                                         {
                                                                             JsonLdMetadata = new JsonLdMetadata(
                                                                                 JsonLdType.Hoofdactiviteit.CreateWithIdValues(JsonLdType.Hoofdactiviteit.Type, x.Code),
                                                                                 JsonLdType.Hoofdactiviteit.Type),
                                                                             Code = x.Code,
                                                                             Naam = x.Naam,
                                                                         })
                                                                       .Distinct()
                                                                       .ToArray();

                    document.KorteNaam = fixture.Create<string>();
                    document.Sleutels = [];
                    document.Lidmaatschappen = [];
                    document.Relaties = [];
                    document.IsDubbel = false;
                    document.IsVerwijderd = false;
                    document.IsUitgeschrevenUitPubliekeDatastroom = false;
                    document.Status = AssociationRegistry.Public.Schema.Constants.VerenigingStatus.Actief;
                    document.KorteBeschrijving = fixture.Create<string>();
                    document.Werkingsgebieden = withoutWerkingsgebieden
                        ? []
                        : fixture.CreateMany<AssociationRegistry.Public.Schema.Search.VerenigingZoekDocument.Types.Werkingsgebied>()
                                 .Distinct()
                                 .Select(x => new AssociationRegistry.Public.Schema.Search.VerenigingZoekDocument.Types.Werkingsgebied()
                                  {
                                      JsonLdMetadata = new JsonLdMetadata(
                                          JsonLdType.Werkingsgebied.CreateWithIdValues(JsonLdType.Werkingsgebied.Type, x.Code),
                                          JsonLdType.Werkingsgebied.Type),
                                      Code = x.Code,
                                      Naam = x.Naam,
                                  }).ToArray();
                    document.Geotags = fixture.CreateMany<VerenigingZoekDocument.Types.Geotag>().ToArray();

                    return document;
                }).OmitAutoProperties());
    }

}
