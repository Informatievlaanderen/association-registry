namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using Confluent.Kafka;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

public static class MessageExtensions
{
    public static object? GetTypedMessage<TKey, TValue>(this Message<TKey, TValue> message)
    {
        var jToken = JToken.Parse(message.Value.ToString());
        var incomingType = jToken.SelectToken("type").ToString();
        var incomingData = jToken.SelectToken("data").ToString();

        var contractAssembly = Assembly.Load("Be.Vlaanderen.Basisregisters.GrAr.Contracts");
        var type = contractAssembly.GetType(incomingType);

        if (type is null) return null;

        return JsonConvert.DeserializeObject(incomingData, type, new JsonSerializerSettings());
    }
}

public class KafkaMessageTypeNotFound : Exception
{
    public string Type { get; }

    public KafkaMessageTypeNotFound(string type)
    {
        Type = type;
    }
}
