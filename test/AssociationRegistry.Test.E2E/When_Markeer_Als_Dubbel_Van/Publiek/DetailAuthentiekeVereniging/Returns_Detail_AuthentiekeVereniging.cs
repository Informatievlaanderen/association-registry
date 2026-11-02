namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Publiek.DetailAuthentiekeVereniging;

using AssociationRegistry.Test.Common.DubbelMerge;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Public.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Xunit;

/// <summary>De authentieke vereniging neemt de basisgegevens over die ze zelf nog niet had.</summary>
[Collection(nameof(MarkeerAlsDubbelVanCollection))]
public class Returns_Vereniging : End2EndTest<PubliekVerenigingDetailResponse>
{
    private readonly MarkeerAlsDubbelVanContext _testContext;

    public Returns_Vereniging(MarkeerAlsDubbelVanContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<PubliekVerenigingDetailResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.PublicApiHost.GetPubliekDetail(
            _testContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode,
            _testContext.DubbeleVerenigingWerdSamengevoegd!.Sequence
        );

    [Fact]
    public void With_The_Own_KorteNaam_Kept() =>
        Response
            .Vereniging.KorteNaam.Should()
            .Be(MarkeerAlsDubbelVanContext.BasisgegevensAuthentiekeVereniging.KorteNaam);

    [Fact]
    public void With_The_KorteBeschrijving_Of_The_Dubbele_Vereniging() =>
        Response
            .Vereniging.KorteBeschrijving.Should()
            .Be(MarkeerAlsDubbelVanContext.BasisgegevensDubbeleVereniging.KorteBeschrijving);

    [Fact]
    public void With_The_Startdatum_Of_The_Dubbele_Vereniging() =>
        Response
            .Vereniging.Startdatum.Should()
            .Be(MarkeerAlsDubbelVanContext.BasisgegevensDubbeleVereniging.Startdatum);

    [Fact]
    public void With_The_Own_And_The_New_Hoofdactiviteiten() =>
        Response
            .Vereniging.HoofdactiviteitenVerenigingsloket.Select(x => x.Code)
            .Should()
            .BeEquivalentTo([TestHoofdactiviteiten.Sport.Code, TestHoofdactiviteiten.Jeugdwerk.Code]);

    [Fact]
    public void With_The_Werkingsgebieden_Of_The_Dubbele_Vereniging() =>
        Response
            .Vereniging.Werkingsgebieden.Select(x => x.Code)
            .Should()
            .BeEquivalentTo([TestWerkingsgebieden.VlaamsBrabant.Code]);

    [Fact]
    public void With_The_Own_And_The_Overgenomen_Contactgegevens() =>
        Response
            .Vereniging.Contactgegevens.Select(x => new
            {
                x.Contactgegeventype,
                x.Waarde,
                x.IsPrimair,
            })
            .Should()
            .BeEquivalentTo([
                new
                {
                    TestContactgegevens.EmailRivendell.Contactgegeventype,
                    TestContactgegevens.EmailRivendell.Waarde,
                    IsPrimair = true,
                },
                new
                {
                    TestContactgegevens.TelefoonRivendell.Contactgegeventype,
                    TestContactgegevens.TelefoonRivendell.Waarde,
                    IsPrimair = false,
                },
                new
                {
                    TestContactgegevens.EmailFellowship.Contactgegeventype,
                    TestContactgegevens.EmailFellowship.Waarde,
                    IsPrimair = false,
                },
                new
                {
                    TestContactgegevens.WebsiteFellowship.Contactgegeventype,
                    TestContactgegevens.WebsiteFellowship.Waarde,
                    IsPrimair = true,
                },
                new
                {
                    TestContactgegevens.SocialMediaFellowship.Contactgegeventype,
                    TestContactgegevens.SocialMediaFellowship.Waarde,
                    IsPrimair = false,
                },
            ]);

    [Fact]
    public void With_The_Own_And_The_Overgenomen_Locaties() =>
        Response
            .Vereniging.Locaties.Select(x => new
            {
                x.Naam,
                x.Locatietype,
                x.IsPrimair,
            })
            .Should()
            .BeEquivalentTo([
                new
                {
                    TestLocaties.LokaalRivendell.Naam,
                    TestLocaties.LokaalRivendell.Locatietype,
                    IsPrimair = true,
                },
                new
                {
                    TestLocaties.PostadresRivendell.Naam,
                    TestLocaties.PostadresRivendell.Locatietype,
                    IsPrimair = false,
                },
                new
                {
                    TestLocaties.LokaalFellowship.Naam,
                    TestLocaties.LokaalFellowship.Locatietype,
                    IsPrimair = false,
                },
                new
                {
                    TestLocaties.TweedeLokaalFellowship.Naam,
                    TestLocaties.TweedeLokaalFellowship.Locatietype,
                    IsPrimair = false,
                },
            ]);

    [Fact]
    public void With_The_Doelgroep_Of_The_Dubbele_Vereniging()
    {
        var doelgroep = MarkeerAlsDubbelVanContext.BasisgegevensDubbeleVereniging.Doelgroep;

        Response.Vereniging.Doelgroep.Minimumleeftijd.Should().Be(doelgroep.Minimumleeftijd);
        Response.Vereniging.Doelgroep.Maximumleeftijd.Should().Be(doelgroep.Maximumleeftijd);
    }
}
