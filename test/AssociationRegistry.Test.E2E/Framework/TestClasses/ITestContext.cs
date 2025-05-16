namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using ApiSetup;
using Scenarios.Requests;
using Xunit;

public interface ITestContext<TRequest>: IAsyncLifetime
{
    IApiSetup ApiSetup { get; }
    TRequest Request { get; }
    CommandResult<TRequest> CommandResult { get; set; }
    Task Init();
}
