using AssociationRegistry.Public.Api;
using AssociationRegistry.Public.Api.ListVerenigingen;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace AssociationRegistry.Test.Public.Api.Tests.When_retrieving_a_list_of_verenigingen_without_explicit_limit;

using AssociationRegistry.Public.Api.Constants;

public class Given_more_than_limit_number_of_verenigingen
{
    private readonly List<VerenigingListItem> _verenigingen;

    public Given_more_than_limit_number_of_verenigingen()
    {
        var fixture = new VerenigingenFixture();
        var moreThanLimit = PagingConstants.DefaultLimit + new Random().Next(0, 100);
        _verenigingen = fixture
            .CreateMany<VerenigingListItem>(moreThanLimit).ToList();
    }

    [Fact]
    public async Task Then_a_list_with_correct_number_of_verenigingen_is_returned()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Verenigingen.Should()
            .HaveCount(PagingConstants.DefaultLimit);

    [Fact]
    public async Task Then_the_total_number_of_verenigingen_is_returned()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Metadata.Pagination.TotalCount
            .Should().Be(
                _verenigingen.Count);

    [Fact]
    public async Task Then_the_default_limit_is_returned()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Metadata.Pagination.Limit
            .Should().Be(
                PagingConstants.DefaultLimit);

    [Fact]
    public async Task Then_the_default_offset_is_returned()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen)).Metadata.Pagination.Offset
            .Should().Be(
                PagingConstants.DefaultOffset);
}
