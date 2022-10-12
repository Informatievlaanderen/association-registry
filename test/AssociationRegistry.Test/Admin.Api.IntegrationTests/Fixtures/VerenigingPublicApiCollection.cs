namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.Fixtures;

using Public.Api.IntegrationTests.Fixtures;
using Xunit;

[CollectionDefinition(Name)]
public class VerenigingAdminApiCollection :
    ICollectionFixture<VerenigingPublicApiFixture>
{
    public const string Name = "Vereniging admin api collection";
}
