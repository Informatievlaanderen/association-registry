namespace AssociationRegistry.Test.Public.Api.Tests.When_retrieving_a_list_of_verenigingen_with_explicit_limit;

using AssociationRegistry.Public.Api;
using AssociationRegistry.Public.Api.Constants;
using AssociationRegistry.Public.Api.ListVerenigingen;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_73_verenigingen
{
    private const int MoreThanLimit = 73;
    private readonly List<VerenigingListItem> _verenigingen;

    public Given_73_verenigingen()
    {
        var fixture = new VerenigingenFixture();
        _verenigingen = fixture
            .CreateMany<VerenigingListItem>(MoreThanLimit).ToList();
    }

    [Theory]
    [InlineData(0, 20, 20)]
    [InlineData(60, 20, 13)]
    public async Task Then_a_list_with_correct_number_of_verenigingen_is_returned(int offset, int limit,
        int expectedNumberOfVerenigingen)
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen, offset, limit)).Verenigingen.Should()
            .HaveCount(expectedNumberOfVerenigingen);

    [Theory]
    [InlineData(0, 20, 0)]
    [InlineData(60, 20, 60)]
    [InlineData(0, 64, 0)]
    [InlineData(10, 64, 10)]
    public async Task Then_the_offset_is_returned(int offset, int limit,
        int expectedOffset)
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen, offset, limit)).Metadata.Pagination.Offset
            .Should().Be(expectedOffset);

    [Theory]
    [InlineData(0, 20, 20)]
    [InlineData(60, 20, 20)]
    public async Task Then_the_limit_is_returned(int offset, int limit,
        int expectedLimit)
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen, offset, limit)).Metadata.Pagination.Limit
            .Should().Be(expectedLimit);

    [Theory]
    [InlineData(0, 100)]
    [InlineData(10, 64)]
    public async Task With_limit_more_than_default_Then_the_default_limit_is_returned(int offset, int limit)
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen, offset, limit)).Metadata.Pagination.Limit
            .Should().Be(PagingConstants.DefaultLimit);

    [Theory]
    [InlineData(0, 64)]
    [InlineData(10, 64)]
    public async Task With_limit_more_than_default_Then_a_list_with_default_number_of_verenigingen_is_returned(int offset, int limit)
        => (await Scenario.When_retrieving_a_list_of_verenigingen(_verenigingen, offset, limit)).Verenigingen.Should()
            .HaveCount(PagingConstants.DefaultLimit);
}
