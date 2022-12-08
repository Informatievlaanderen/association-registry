using Xunit;

namespace AssociationRegistry.Test.Public.Api.IntegrationTests.Fixtures;

[CollectionDefinition(Name)]
public class VerenigingPublicApiCollection :
    ICollectionFixture<StaticPublicApiFixture>
{
    public const string Name = "Vereniging public api collection";
}
