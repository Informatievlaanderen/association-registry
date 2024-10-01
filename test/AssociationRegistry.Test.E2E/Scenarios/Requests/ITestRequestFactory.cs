namespace AssociationRegistry.Test.E2E.Scenarios.Commands;

using Framework.ApiSetup;
using Vereniging;

public interface ITestRequestFactory<TRequest>
{
    Task<RequestResult<TRequest>> ExecuteRequest(FullBlownApiSetup apiSetup);
}

public record RequestResult<TRequest>(VCode VCode, TRequest Request);
