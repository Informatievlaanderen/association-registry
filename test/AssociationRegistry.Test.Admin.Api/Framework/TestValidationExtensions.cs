namespace AssociationRegistry.Test.Admin.Api.Framework;

using AssociationRegistry.Admin.Api;
using FluentValidation.TestHelper;

public static class TestValidationExtensions
{
    public static ITestValidationWith WithVeldIsVerplichtErrorMessage(this ITestValidationWith failures, string veld)
        => failures.WithErrorMessage(string.Format(ValidationMessages.VeldIsVerplicht, veld));
}
