namespace AssociationRegistry.Test.Public.Api.When_Searching.With_Sorting;

using Fixtures;
using Fixtures.GivenEvents;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_Sort_By_Field_In_A_List
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly PublicApiClient _publicApiClient;

    public Given_Sort_By_Field_In_A_List(GivenEventsFixture fixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Theory]
    [InlineData("locaties.postcode")]
    public async Task? Then_it_returns200_but_we_dont_support_it(string field)
    {
        var response = await _publicApiClient.Search(q: "*", field);

        response.Should().BeSuccessful();
    }
}
