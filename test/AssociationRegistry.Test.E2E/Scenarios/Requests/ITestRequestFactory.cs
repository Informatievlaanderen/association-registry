namespace AssociationRegistry.Test.E2E.Scenarios.Requests;

using Framework.ApiSetup;
using Vereniging;

public interface ITestRequestFactory<TRequest>
{
    Task<RequestResult<TRequest>> ExecuteRequest(IApiSetup apiSetup);
}

public record RequestResult<TRequest>(VCode VCode, TRequest Request, long? Sequence = null);
