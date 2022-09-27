namespace AssociationRegistry.Test.Public.Api.UnitTests.When_retrieving_a_list_of_verenigingen_without_explicit_limit;

using AssociationRegistry.Public.Api.Constants;
using AssociationRegistry.Public.Api.ListVerenigingen;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_less_or_equal_than_limit_number_of_verenigingen
{
    private readonly List<VerenigingListItem> _verenigingen;

    public Given_less_or_equal_than_limit_number_of_verenigingen()
    {
        var fixture = new VerenigingenFixture();
        var lessOrEqualThanLimit = new Random().Next(0, PagingConstants.DefaultLimit);
        _verenigingen = fixture
            .CreateMany<VerenigingListItem>(lessOrEqualThanLimit).ToList();
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
