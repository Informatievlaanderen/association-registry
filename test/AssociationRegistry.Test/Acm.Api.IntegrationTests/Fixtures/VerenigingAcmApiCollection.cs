namespace AssociationRegistry.Test.Acm.Api.IntegrationTests.Fixtures;

using Xunit;

[CollectionDefinition(Name)]
public class VerenigingAcmApiCollection :
    ICollectionFixture<VerenigingAcmApiFixture>
{
    public const string Name = "Vereniging acm api collection";
}

