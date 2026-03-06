namespace AssociationRegistry.Test.Projections.Framework;

/// <summary>
/// xUnit collection fixture registration for <see cref="ProjectionContext"/>.
///
/// IMPORTANT:
/// This class is discovered by xUnit via reflection. It may appear unused,
/// but it is required so xUnit can create and inject a shared ProjectionContext
/// into tests and fixtures that depend on it (e.g. scenario fixtures).
///
/// Do NOT delete this class even if the IDE reports it as unused.
/// </summary>
[CollectionDefinition(nameof(ProjectionContext))]
public class ProjectionContextCollection : ICollectionFixture<ProjectionContext>
{
    // Intentionally empty.
    // This class only exists to register ProjectionContext as a collection fixture.
}
