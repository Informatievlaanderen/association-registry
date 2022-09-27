namespace AssociationRegistry.Test.Public.Api.UnitTests.When_retrieving_a_list_of_verenigingen_without_explicit_limit;

using AssociationRegistry.Public.Api.ListVerenigingen;
using FluentAssertions;
using Xunit;

public class Given_no_verenigingen
{
    [Fact]
    public async Task Then_an_empty_list_is_returned()
        => (await Scenario.When_retrieving_a_list_of_verenigingen(new List<VerenigingListItem>())).Verenigingen.Should()
            .BeEmpty();
}
