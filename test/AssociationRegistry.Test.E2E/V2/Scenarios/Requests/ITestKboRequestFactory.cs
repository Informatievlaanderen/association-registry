namespace AssociationRegistry.Test.E2E.V2.Scenarios.Requests;

using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Vereniging;

public interface ITestKboRequestFactory<TRequest>
{
    Task<RequestKboResult<TRequest>> ExecuteRequest(IApiSetup apiSetup);
}

public record RequestKboResult<TRequest>(KboNummer KboNummer, TRequest Request);
