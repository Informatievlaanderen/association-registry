namespace AssociationRegistry.Admin.Api.Infrastructure.Json;

using Constants;
using Formats;
using Newtonsoft.Json;
using Primitives;
using System;

public class NullOrEmptyDateOnlyJsonConvertor : JsonConverter<NullOrEmpty<DateOnly>>
{
    public override void WriteJson(JsonWriter writer, NullOrEmpty<DateOnly> value, JsonSerializer serializer)
    {
        if (value.IsNull) writer.WriteNull();
        if (value.IsEmpty) writer.WriteValue(string.Empty);
        if (value.HasValue) serializer.Serialize(writer, value.Value);
    }

    public override NullOrEmpty<DateOnly> ReadJson(
        JsonReader reader,
        Type objectType,
        NullOrEmpty<DateOnly> existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.Value! is not string readerValue) return NullOrEmpty<DateOnly>.Null;
        if (string.IsNullOrWhiteSpace(readerValue)) return NullOrEmpty<DateOnly>.Empty;

        return NullOrEmpty<DateOnly>.Create(DateOnlyHelpers.Parse(readerValue, WellknownFormats.DateOnly));
    }
}

public class NullableNullOrEmptyDateOnlyJsonConvertor : JsonConverter<NullOrEmpty<DateOnly>?>
{
    private readonly NullOrEmptyDateOnlyJsonConvertor _nullOrEmptyDateOnlyJsonConvertor;

    public NullableNullOrEmptyDateOnlyJsonConvertor()
    {
        _nullOrEmptyDateOnlyJsonConvertor = new NullOrEmptyDateOnlyJsonConvertor();
    }

    public override void WriteJson(JsonWriter writer, NullOrEmpty<DateOnly>? value, JsonSerializer serializer)
    {
        if (value is null) writer.WriteNull();
        _nullOrEmptyDateOnlyJsonConvertor.WriteJson(writer, value!.Value, serializer);
    }

    public override NullOrEmpty<DateOnly>? ReadJson(
        JsonReader reader,
        Type objectType,
        NullOrEmpty<DateOnly>? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
        => _nullOrEmptyDateOnlyJsonConvertor.ReadJson(reader,
                                                      objectType,
                                                      existingValue ?? NullOrEmpty<DateOnly>.Null,
                                                      hasExistingValue,
                                                      serializer);
}
