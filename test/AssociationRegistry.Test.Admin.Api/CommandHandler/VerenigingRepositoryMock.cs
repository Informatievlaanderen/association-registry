namespace AssociationRegistry.Test.Admin.Api.CommandHandler;

using global::AssociationRegistry.Framework;
using VCodes;
using Vereniging;

public class VerenigingRepositoryMock : IVerenigingsRepository
{
    private readonly Vereniging? _verenigingToLoad;

    public record InvocationSave(Vereniging Vereniging);
    public record InvocationLoad(VCode VCode);

    public readonly List<InvocationSave> InvocationsSave = new();
    public readonly List<InvocationLoad> InvocationsLoad = new();

    public VerenigingRepositoryMock(Vereniging? verenigingToLoad = null)
    {
        _verenigingToLoad = verenigingToLoad;
    }

    public async Task<long?> Save(Vereniging vereniging, CommandMetadata metadata)
    {
        InvocationsSave.Add(new InvocationSave(vereniging));
        return await Task.FromResult(-1L);
    }

    public async Task<Vereniging> Load(VCode vCode)
    {
        InvocationsLoad.Add(new InvocationLoad(vCode));
        return (await Task.FromResult(_verenigingToLoad))!;
    }
}
