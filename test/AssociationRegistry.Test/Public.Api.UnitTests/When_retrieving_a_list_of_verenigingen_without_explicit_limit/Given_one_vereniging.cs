namespace AssociationRegistry.Test.Public.Api.UnitTests.When_retrieving_a_list_of_verenigingen_without_explicit_limit;

using AssociationRegistry.Public.Api.ListVerenigingen;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_one_vereniging
{
    private readonly List<VerenigingListItem> _verenigingen;
    private readonly VerenigingListItem _vereniging;
    private readonly Uri _beheerder;

    public Given_one_vereniging()
    {
        var fixture = new VerenigingenFixture();
        _beheerder = fixture.Create<Uri>();
        _vereniging = fixture.Create<VerenigingListItem>();
        _vereniging = _vereniging with
        {
            Activiteiten =
            _vereniging.Activiteiten.Add(new Activiteit("badminton", _beheerder)),
        };

        _verenigingen = new List<VerenigingListItem>
        {
            _vereniging,
        };
    }

    [Fact]
    public async Task Then_a_list_with_1_vereniging_is_returned()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Verenigingen.Should().HaveCount(1);

    [Fact]
    public async Task Then_a_vereniging_has_a_vcode()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Verenigingen.Single().Id.Should()
            .Be(_vereniging.Id);

    [Fact]
    public async Task Then_a_vereniging_has_a_naam()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Verenigingen.Single().Naam.Should()
            .Be(_vereniging.Naam);

    [Fact]
    public async Task Then_a_vereniging_has_a_korte_naam()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Verenigingen.Single().KorteNaam
            .Should()
            .Be(_vereniging.KorteNaam);

    [Fact]
    public async Task Then_a_vereniging_has_list_of_activiteiten()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Verenigingen.Single().Activiteiten
            .Should().BeEquivalentTo(_vereniging.Activiteiten);

    [Fact]
    public async Task Then_a_vereniging_has_an_activiteit_with_beheerder()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Verenigingen.Single().Activiteiten
            .Should().ContainEquivalentOf(new Activiteit("badminton", _beheerder));

    [Fact]
    public async Task Then_a_vereniging_has_a_hoofdLocatie_postcode()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Verenigingen.Single().Hoofdlocatie.Postcode
            .Should()
            .Be(_vereniging.Hoofdlocatie.Postcode);

    [Fact]
    public async Task Then_a_vereniging_has_a_hoofdLocatie_gemeentenaam()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Verenigingen.Single().Hoofdlocatie.Gemeentenaam
            .Should()
            .Be(_vereniging.Hoofdlocatie.Gemeentenaam);

    [Fact]
    public async Task Then_a_vereniging_has_a_jsonld_context_base()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Context
            .Should()
            .Be("http://localhost:11003/api/v1/contexten/list-verenigingen-context.json");
}
