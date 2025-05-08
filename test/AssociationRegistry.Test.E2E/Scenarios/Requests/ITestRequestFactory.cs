namespace AssociationRegistry.Test.E2E.Scenarios.Requests;

using Framework.ApiSetup;
using Vereniging;

public interface ITestRequestFactory<TRequest>
{
    Task<CommandResult<TRequest>> ExecuteRequest(IApiSetup apiSetup);
}

public record CommandResult<TRequest>(VCode VCode, TRequest Request, long? Sequence = null)
{
    public static CommandResult<NullRequest> NullCommandResult() => new CommandResult<NullRequest>(null, new NullRequest(), null);
};
