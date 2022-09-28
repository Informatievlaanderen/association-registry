namespace AssociationRegistry.Test.Public.Api.UnitTests.When_retrieving_a_vereniging_detail;

using AssociationRegistry.Public.Api.DetailVerenigingen;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_existing_vereniging
{
    private readonly VerenigingDetail _vereniging;
    private readonly string _expectedTelefoon;
    private readonly string _expectedEmail;
    private Uri _beheerder;

    public Given_existing_vereniging()
    {
        var fixture = new VerenigingenFixture();
        _vereniging = fixture.Create<VerenigingDetail>();

        _expectedTelefoon = fixture.Create<string>();
        _expectedEmail = fixture.Create<string>();
        _beheerder = fixture.Create<Uri>();
        _vereniging = _vereniging with
        {
            ContactPersoon = _vereniging.ContactPersoon! with
            {
                ContactGegevens =
                _vereniging.ContactPersoon.ContactGegevens
                    .Add(new ContactGegeven("telefoon", _expectedTelefoon))
                    .Add(new ContactGegeven("email", _expectedEmail)),
            },
            Activiteiten =
            _vereniging.Activiteiten.Add(
                new Activiteit("badminton", _beheerder)),
        };
    }

    [Fact]
    public async Task Then_a_vereniging_has_a_vCode()
        => (await Scenario.When_retrieving_a_vereniging_detail(_vereniging)).VerenigingDetail.Id.Should()
            .Be(_vereniging.Id);

    [Fact]
    public async Task Then_a_vereniging_has_a_naam()
        => (await Scenario.When_retrieving_a_vereniging_detail(_vereniging)).VerenigingDetail.Naam.Should()
            .Be(_vereniging.Naam);

    [Fact]
    public async Task Then_a_vereniging_has_a_korte_naam()
        => (await Scenario.When_retrieving_a_vereniging_detail(_vereniging)).VerenigingDetail.KorteNaam
            .Should()
            .Be(_vereniging.KorteNaam);

    [Fact]
    public async Task Then_a_vereniging_has_a_korte_omschrijving()
        => (await Scenario.When_retrieving_a_vereniging_detail(_vereniging)).VerenigingDetail.KorteOmschrijving
            .Should()
            .Be(_vereniging.KorteOmschrijving);

    [Fact]
    public async Task Then_a_vereniging_has_list_of_activiteiten()
        => (await Scenario.When_retrieving_a_vereniging_detail(_vereniging)).VerenigingDetail.Activiteiten
            .Should().BeEquivalentTo(_vereniging.Activiteiten);
    [Fact]
    public async Task Then_a_vereniging_has_an_activiteit_with_beheerder()
        => (await Scenario.When_retrieving_a_vereniging_detail(_vereniging)).VerenigingDetail.Activiteiten
            .Should().ContainEquivalentOf(new Activiteit("badminton", _beheerder));

    [Fact]
    public async Task Then_a_vereniging_has_a_rechtsvorm()
        => (await Scenario.When_retrieving_a_vereniging_detail(_vereniging)).VerenigingDetail.Rechtsvorm
            .Should()
            .Be(_vereniging.Rechtsvorm);

    [Fact]
    public async Task Then_a_vereniging_has_a_startDatum()
        => (await Scenario.When_retrieving_a_vereniging_detail(_vereniging)).VerenigingDetail.StartDatum
            .Should()
            .Be(_vereniging.StartDatum);

    [Fact]
    public async Task Then_a_vereniging_has_a_eindDatum()
        => (await Scenario.When_retrieving_a_vereniging_detail(_vereniging)).VerenigingDetail.EindDatum
            .Should()
            .Be(_vereniging.EindDatum);

    [Fact]
    public async Task Then_a_vereniging_has_a_hoofdLocatie_postcode()
        => (await Scenario.When_retrieving_a_vereniging_detail(_vereniging)).VerenigingDetail.Hoofdlocatie.Postcode
            .Should()
            .Be(_vereniging.Hoofdlocatie.Postcode);

    [Fact]
    public async Task Then_a_vereniging_has_a_hoofdLocatie_gemeentenaam()
        => (await Scenario.When_retrieving_a_vereniging_detail(_vereniging)).VerenigingDetail.Hoofdlocatie.Gemeentenaam
            .Should()
            .Be(_vereniging.Hoofdlocatie.Gemeentenaam);

    [Fact]
    public async Task Then_a_vereniging_has_a_contactpersoon_voornaam()
        => (await Scenario.When_retrieving_a_vereniging_detail(_vereniging)).VerenigingDetail.ContactPersoon!.Voornaam
            .Should()
            .Be(_vereniging.ContactPersoon!.Voornaam);

    [Fact]
    public async Task Then_a_vereniging_has_a_contactpersoon_achternaam()
        => (await Scenario.When_retrieving_a_vereniging_detail(_vereniging)).VerenigingDetail.ContactPersoon!.Achternaam
            .Should()
            .Be(_vereniging.ContactPersoon!.Achternaam);

    [Fact]
    public async Task Then_a_vereniging_has_a_contactpersoon_email()
        => (await Scenario.When_retrieving_a_vereniging_detail(_vereniging)).VerenigingDetail.ContactPersoon!
            .ContactGegevens
            .Should().Contain(new ContactGegeven("email", _expectedEmail));

    [Fact]
    public async Task Then_a_vereniging_has_a_contactpersoon_telefoon()
        => (await Scenario.When_retrieving_a_vereniging_detail(_vereniging)).VerenigingDetail.ContactPersoon!
            .ContactGegevens
            .Should().Contain(new ContactGegeven("telefoon", _expectedTelefoon));

    [Fact]
    public async Task Then_a_vereniging_has_a_laatstGewijzigd_date()
        => (await Scenario.When_retrieving_a_vereniging_detail(_vereniging)).VerenigingDetail
            .LaatstGewijzigd
            .Should().Be(_vereniging.LaatstGewijzigd);

    [Fact]
    public async Task Then_a_vereniging_has_list_of_locaties()
        => (await Scenario.When_retrieving_a_vereniging_detail(_vereniging)).VerenigingDetail.Locaties
            .Should().BeEquivalentTo(_vereniging.Locaties);

    [Fact]
    public async Task Then_a_vereniging_has_a_jsonld_context()
        => (await Scenario.When_retrieving_a_vereniging_detail(_vereniging)).Context
            .Should()
            .Be("http://localhost:11003/api/v1/contexten/detail-vereniging-context.json");
}
