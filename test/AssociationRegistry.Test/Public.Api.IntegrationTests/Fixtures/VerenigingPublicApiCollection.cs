using Xunit;

namespace AssociationRegistry.Test.Public.Api.IntegrationTests.Fixtures;

[CollectionDefinition(Name)]
public class VerenigingPublicApiCollection :
    ICollectionFixture<VerenigingPublicApiFixture>
{
    public const string Name = "Vereniging public api collection";
}
