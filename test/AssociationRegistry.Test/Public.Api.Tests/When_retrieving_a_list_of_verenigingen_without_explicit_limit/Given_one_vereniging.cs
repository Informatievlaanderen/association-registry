using AssociationRegistry.Public.Api.ListVerenigingen;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace AssociationRegistry.Test.Public.Api.Tests.When_retrieving_a_list_of_verenigingen_without_explicit_limit;

public class Given_one_vereniging
{
    private readonly List<Vereniging> _verenigingen;
    private readonly Vereniging _vereniging;

    public Given_one_vereniging()
    {
        var fixture = new VerenigingenFixture();
        _vereniging = fixture.Create<Vereniging>();
        _verenigingen = new List<Vereniging>
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
    public async Task Then_a_vereniging_has_list_of_locaties()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Verenigingen.Single().Locaties
            .Should().BeEquivalentTo(_vereniging.Locaties);

    [Fact]
    public async Task Then_a_vereniging_has_a_jsonld_context_base()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Context.Base
            .Should()
            .Be("https://data.vlaanderen.be/ns/FeitelijkeVerenigingen");

    [Fact]
    public async Task Then_a_vereniging_has_a_jsonld_context_vocab()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Context.Vocab
            .Should()
            .Be("@");

    [Fact]
    public async Task Then_a_vereniging_has_a_jsonld_context_identificator()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Context.Identificator
            .Should()
            .Be("@nest");
    [Fact]
    public async Task Then_a_vereniging_has_a_jsonld_context_id()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Context.Id
            .Should()
            .Be("@id");
    [Fact]
    public async Task Then_a_vereniging_has_a_jsonld_context_voorkeursnaam_id()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Context.Naam.Id
            .Should()
            .Be("http://www.w3.org/2004/02/skos/core#prefLabel");
    [Fact]
    public async Task Then_a_vereniging_has_a_jsonld_context_voorkeursnaam_type()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Context.Naam.Type
            .Should()
            .Be("@id");


}
