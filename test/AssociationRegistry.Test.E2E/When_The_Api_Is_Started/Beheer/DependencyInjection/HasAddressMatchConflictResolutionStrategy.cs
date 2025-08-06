namespace AssociationRegistry.Test.E2E.When_The_Api_Is_Started.Beheer.DependencyInjection;

using EventStore;
using EventStore.ConflictResolution;
using FluentAssertions;
using Framework.ApiSetup;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

[CollectionDefinition(nameof(ApiStartedCollection))]
public class AddressMatchConflictResolutionStrategyTests
{
    private readonly FullBlownApiSetup _apiSetup;

    public AddressMatchConflictResolutionStrategyTests(FullBlownApiSetup apiSetup)
    {
        _apiSetup = apiSetup;
    }

    [Fact]
    public void CheckForAddressMatchConflictResolutionStrategy()
    {
        var serivice = _apiSetup.AdminApiHost.Services.GetRequiredService<EventConflictResolver>();

        serivice._postStrategies.Should()
                .Contain(x => x is AddressMatchConflictResolutionStrategy);

        serivice._preStrategies.Should()
                .Contain(x => x is AddressMatchConflictResolutionStrategy);
    }
}
