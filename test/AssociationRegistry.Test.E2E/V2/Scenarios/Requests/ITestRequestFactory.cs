namespace AssociationRegistry.Test.E2E.V2.Scenarios.Requests;

using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Vereniging;

public interface ITestRequestFactory<TRequest>
{
    Task<RequestResult<TRequest>> ExecuteRequest(IApiSetup apiSetup);
}

public record RequestResult<TRequest>(VCode VCode, TRequest Request);
