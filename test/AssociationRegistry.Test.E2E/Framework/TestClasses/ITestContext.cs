namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using ApiSetup;
using Xunit;

public interface ITestContext<TRequest>: IAsyncLifetime
{
    IApiSetup ApiSetup { get; }
    TRequest Request { get; }
}
