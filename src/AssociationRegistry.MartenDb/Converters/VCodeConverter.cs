namespace AssociationRegistry.MartenDb.Converters;

using Newtonsoft.Json;
using AssociationRegistry.DecentraalBeheer.Vereniging;

public class VCodeJsonConverter : JsonConverter<VCode>
{
    public override void WriteJson(JsonWriter writer, VCode value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
        }
        else
        {
            writer.WriteValue(value.Value);
        }
    }

    public override VCode ReadJson(JsonReader reader, Type objectType, VCode existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonToken.String)
        {
            var stringValue = (string)reader.Value;
            return string.IsNullOrEmpty(stringValue) ? null : VCode.Create(stringValue);
        }

        throw new JsonSerializationException($"Unexpected token type: {reader.TokenType}");
    }
}
