using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace AssociationRegistry.KboMutations.SyncLambda.Aws;

public class SsmClientWrapper
{
    private readonly IAmazonSimpleSystemsManagement _client;

    public SsmClientWrapper(IAmazonSimpleSystemsManagement client)
    {
        _client = client;
    }

    public async Task<string> GetParameterAsync(string name)
    {
        var parameter = await _client.GetParameterAsync(new GetParameterRequest()
        {
            Name = name,
            WithDecryption = true
        });
        return parameter.Parameter.Value;
    }
}