﻿namespace AssociationRegistry.Test.Helpers;

using System.Text;

public static class IntegrationTestHelpers
{
    public static StringContent AsJsonContent(this string jsonContent)
        => new(
            jsonContent,
            Encoding.UTF8,
            "application/json");
}
