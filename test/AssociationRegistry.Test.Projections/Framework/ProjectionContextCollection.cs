namespace AssociationRegistry.Test.Projections.Framework;

using Xunit;

[CollectionDefinition(nameof(ProjectionContext))]
public class ProjectionContextCollection : ICollectionFixture<ProjectionContext>
{
}
