namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using Xunit;

public interface ITestContext<TRequest>: IAsyncLifetime
{
    IApiSetup ApiSetup { get; }
    TRequest Request { get; }
}
