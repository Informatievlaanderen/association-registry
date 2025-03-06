namespace AssociationRegistry.Test.Public.Api.Framework;

using AssociationRegistry.Public.Api.Infrastructure.Extensions;
using System;

public static class TypeExtensions
{
    /// <summary>
    /// usage: put a json-file next to the type
    /// the name of the resource (without namespace and without extension) is passed
    /// in the 'resourceName' parameter
    /// </summary>
    /// <returns>the contents of the embedded json that matches the calculated filename</returns>
    public static string GetAssociatedResourceJson(this Type type, string resourceName)
        => type.GetResourceString(resourceName, extension: "json");

    private static string GetResourceString(this Type type, string methodName, string extension)
        => type.Assembly.GetResourceString($"{type.Namespace!}.{methodName}.{extension}");
}
