namespace AssociationRegistry.Test.Acm.Api.Fixtures;

using Xunit;

[CollectionDefinition(nameof(AcmApiCollection))]
public class AcmApiCollection : ICollectionFixture<EventsInDbScenariosFixture>
{
}
