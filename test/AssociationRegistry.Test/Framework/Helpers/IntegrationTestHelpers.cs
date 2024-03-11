﻿namespace AssociationRegistry.Test.Framework.Helpers;

using System.Text;

public static class IntegrationTestHelpers
{
    public static StringContent AsJsonContent(this string jsonContent)
        => new(
            jsonContent,
            Encoding.UTF8,
            mediaType: "application/json");
}
