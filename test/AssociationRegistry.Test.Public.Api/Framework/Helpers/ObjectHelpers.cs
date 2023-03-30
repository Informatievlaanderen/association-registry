namespace AssociationRegistry.Test.Public.Api.Framework.Helpers;

using Newtonsoft.Json;

public static class ObjectHelpers
{
    public static T Copy<T>(this T obj)
        => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj))!;
}
