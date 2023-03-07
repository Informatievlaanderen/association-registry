namespace AssociationRegistry.Admin.Api.Infrastructure.Json;

using System;
using Constants;
using Newtonsoft.Json;
using Primitives;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

public class NullOrEmptyJsonConvertor : JsonConverter<NullOrEmpty<DateOnly>>
{
    public NullOrEmptyJsonConvertor()
    {
    }

    public override void WriteJson(JsonWriter writer, NullOrEmpty<DateOnly> value, JsonSerializer serializer)
    {
        if (value.IsNull) writer.WriteNull();
        if (value.IsEmpty) writer.WriteValue(string.Empty);
        if (value.HasValue) serializer.Serialize(writer, value.Value);
    }


    public override NullOrEmpty<DateOnly> ReadJson(JsonReader reader, Type objectType, NullOrEmpty<DateOnly> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value! is not string readerValue) return NullOrEmpty<DateOnly>.Null;
        if (string.IsNullOrEmpty(readerValue)) return NullOrEmpty<DateOnly>.Empty;
        return NullOrEmpty<DateOnly>.Create(DateOnlyHelpers.Parse(readerValue, WellknownFormats.DateOnly));
    }
}
