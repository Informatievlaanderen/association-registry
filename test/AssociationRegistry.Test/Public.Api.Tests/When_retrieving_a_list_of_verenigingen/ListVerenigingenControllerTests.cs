using System.Collections.Immutable;
using AssociationRegistry.Public.Api.ListVerenigingen;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AssociationRegistry.Test.Public.Api.Tests.When_retrieving_a_list_of_verenigingen;

using AutoFixture;

public static class Scenario
{
    public static async Task<ListVerenigingenResponse> When_retrieving_a_list_of_verenigingen(
        List<Vereniging> verenigingen)
    {
        var verenigingenRepositoryStub = new VerenigingenRepositoryStub(verenigingen);
        var controller = new ListVerenigingenController(verenigingenRepositoryStub);

        var response = await controller.List();

        return response.Should().BeOfType<OkObjectResult>()
            .Which.Value?.Should().BeOfType<ListVerenigingenResponse>()
            .Subject!;
    }
}

public class Given_no_verenigingen
{
    [Fact]
    public async Task Then_an_empty_list_is_returned()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(new List<Vereniging>())).Verenigingen.Should()
            .BeEmpty();
}

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
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Verenigingen.Single().VCode.Should()
            .Be(_vereniging.VCode);

    [Fact]
    public async Task Then_a_vereniging_has_a_naam()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Verenigingen.Single().Naam.Should()
            .Be(_vereniging.Naam);

    [Fact]
    public async Task Then_a_vereniging_has_a_korte_naam()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Verenigingen.Single().KorteNaam.Should()
            .Be(_vereniging.KorteNaam);

    [Fact]
    public async Task Then_a_vereniging_has_list_of_activiteiten()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Verenigingen.Single().Activiteiten
            .Should().BeEquivalentTo(_vereniging.Activiteiten);

    [Fact]
    public async Task Then_a_vereniging_has_list_of_locaties()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Verenigingen.Single().Locaties
            .Should().BeEquivalentTo(_vereniging.Locaties);

}

public class Given_many_verenigingen
{
    private readonly List<Vereniging> _verenigingen;

    public Given_many_verenigingen()
    {
        var fixture = new VerenigingenFixture();
        _verenigingen = fixture.CreateMany<Vereniging>().ToList();
    }

    [Fact]
    public async Task Then_a_list_with_correct_number_of_verenigingen_is_returned()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Verenigingen.Should()
            .HaveCount(_verenigingen.Count);

    [Fact]
    public async Task Then_a_list_with_the_different_verenigingen_is_returned()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Verenigingen.Should()
            .BeEquivalentTo(_verenigingen);
}

public class VerenigingenRepositoryStub : IVerenigingenRepository
{
    private readonly List<Vereniging> _verenigingen;

    public VerenigingenRepositoryStub(List<Vereniging> verenigingen)
    {
        _verenigingen = verenigingen;
    }

    public Task<ImmutableArray<Vereniging>> List() => Task.FromResult(_verenigingen.ToImmutableArray());
}
