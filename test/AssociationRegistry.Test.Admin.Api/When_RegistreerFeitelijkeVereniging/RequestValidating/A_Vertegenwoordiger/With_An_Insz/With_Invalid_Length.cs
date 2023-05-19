namespace AssociationRegistry.Test.Admin.Api.When_RegistreerFeitelijkeVereniging.RequestValidating.A_Vertegenwoordiger.With_An_Insz;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.DecentraalBeheerdeVereniging;
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
        var validator = new RegistreerDecentraalBeheerdeVerenigingRequestValidator();
        var request = new RegistreerDecentraalBeheerdeVerenigingRequest
        {
            Vertegenwoordigers = new[]
            {
                new ToeTeVoegenVertegenwoordiger
                {
                    Insz = insz,
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(request.Vertegenwoordigers)}[0].{nameof(ToeTeVoegenVertegenwoordiger.Insz)}")
            .WithErrorMessage("Insz moet 11 cijfers bevatten");
    }
}
