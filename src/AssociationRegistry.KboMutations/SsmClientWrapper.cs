namespace AssociationRegistry.KboMutations;

using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using System.Net;

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

        if (parameter.HttpStatusCode != HttpStatusCode.OK)
            throw new ArgumentException($"Could not find parameter with name {nameof(name)}");

        return parameter.Parameter.Value;
    }
}
