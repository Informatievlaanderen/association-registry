namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Publiek.ZoekenAuthentiekeVereniging;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Public.Api.WebApi.Verenigingen.Search.ResponseModels;
using Xunit;

/// <summary>De authentieke vereniging neemt de basisgegevens over die ze zelf nog niet had.</summary>
[Collection(nameof(MarkeerAlsDubbelVanCollection))]
public class Returns_Vereniging : End2EndTest<SearchVerenigingenResponse>
{
    private readonly MarkeerAlsDubbelVanContext _testContext;

    public Returns_Vereniging(MarkeerAlsDubbelVanContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<SearchVerenigingenResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.PublicApiHost.GetPubliekZoeken(
            $"vCode:{_testContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode}",
            _testContext.DubbeleVerenigingWerdSamengevoegd!.Sequence
        );

    private Vereniging AuthentiekeVereniging =>
        Response.Verenigingen.Single(x =>
            x.VCode == _testContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode
        );

    [Fact]
    public void With_The_Own_KorteNaam_Kept() =>
        AuthentiekeVereniging
            .KorteNaam.Should()
            .Be(MarkeerAlsDubbelVanContext.BasisgegevensAuthentiekeVereniging.KorteNaam);

    [Fact]
    public void With_The_KorteBeschrijving_Of_The_Dubbele_Vereniging() =>
        AuthentiekeVereniging
            .KorteBeschrijving.Should()
            .Be(MarkeerAlsDubbelVanContext.BasisgegevensDubbeleVereniging.KorteBeschrijving);

    [Fact]
    public void With_The_Doelgroep_Of_The_Dubbele_Vereniging()
    {
        var doelgroep = MarkeerAlsDubbelVanContext.BasisgegevensDubbeleVereniging.Doelgroep;

        AuthentiekeVereniging.Doelgroep.Minimumleeftijd.Should().Be(doelgroep.Minimumleeftijd);
        AuthentiekeVereniging.Doelgroep.Maximumleeftijd.Should().Be(doelgroep.Maximumleeftijd);
    }
}
