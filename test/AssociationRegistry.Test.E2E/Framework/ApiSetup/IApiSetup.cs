namespace AssociationRegistry.Test.E2E;

using Alba;

public interface IApiSetup
{
    Task InitializeAsync(string schema);
    IAlbaHost AdminApiHost { get; }
    IAlbaHost ProjectionHost { get; }
    IAlbaHost QueryApiHost { get; }
}
