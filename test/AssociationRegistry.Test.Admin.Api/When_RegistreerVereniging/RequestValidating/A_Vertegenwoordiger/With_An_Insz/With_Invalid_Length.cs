namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_Vertegenwoordiger.With_An_Insz;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Invalid_Length
{
    [Theory]
    [InlineData("0123456789012")]
    [InlineData("0123456")]
    public void Has_validation_error__insz_moet_11_cijfers_bevatten(string insz)
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            Vertegenwoordigers = new[]
            {
                new RegistreerVerenigingRequest.Vertegenwoordiger()
                {
                    Insz = insz,
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(request.Vertegenwoordigers)}[0].{nameof(RegistreerVerenigingRequest.Vertegenwoordiger.Insz)}")
            .WithErrorMessage("Insz moet 11 cijfers bevatten");
    }
}
