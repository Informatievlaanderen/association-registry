namespace AssociationRegistry.Admin.Api.Infrastructure.Json;

using System;
using Newtonsoft.Json;
using Primitives;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

public class NullOrEmptyJsonConvertor<T> : JsonConverter<NullOrEmpty<T>>
{
    public NullOrEmptyJsonConvertor()
    {
    }

    public override void WriteJson(JsonWriter writer, NullOrEmpty<T> value, JsonSerializer serializer)
    {
        if (value.IsNull) writer.WriteNull();
        if (value.IsEmpty) writer.WriteValue(string.Empty);
        if (value.HasValue) writer.WriteValue(value.Value); }


    public override NullOrEmpty<T> ReadJson(JsonReader reader, Type objectType, NullOrEmpty<T> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value is null) return NullOrEmpty<T>.Null;
        if (string.IsNullOrEmpty(reader.Value! as string)) return NullOrEmpty<T>.Empty;
        return NullOrEmpty<T>.Create(serializer.Deserialize<T>(reader)!);
    }
}
