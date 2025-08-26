namespace AssociationRegistry.Test.E2E.When_The_Api_Is_Started.Publiek.Werkingsgebieden;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Xunit;

[CollectionDefinition(nameof(ApiStartedCollection))]
public class Returns_List_Of_All_Werkingsgebieden
{
    private readonly FullBlownApiSetup _apiSetup;

    public Returns_List_Of_All_Werkingsgebieden(FullBlownApiSetup apiSetup)
    {
        _apiSetup = apiSetup;
    }

    [Fact]
    public async Task Returns_Werkingsgebieden()
    {
        var result = await _apiSetup.PublicApiHost.GetWerkingsgebieden();
        result.Werkingsgebieden.Should().NotBeEmpty();
    }
}
