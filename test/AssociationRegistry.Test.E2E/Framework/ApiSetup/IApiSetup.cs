namespace AssociationRegistry.Test.E2E.Framework.ApiSetup;

using Alba;

public interface IApiSetup
{
    Task InitializeAsync(string schema);
    IAlbaHost AdminApiHost { get; }
    IAlbaHost ProjectionHost { get; }
    IAlbaHost QueryApiHost { get; }
}
