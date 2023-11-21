namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestValidating.A_Vertegenwoordiger.With_A_Voornaam;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling.RequestModels;
using Fakes;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Valid
{
    [Theory]
    [InlineData("Jef")]
    [InlineData("Marie")]
    [InlineData("@#(!i i")]
    public void Has_no_validation_errors(string voornaam)
    {
        var validator = new RegistreerAfdelingRequestValidator(new ClockStub(DateOnly.MaxValue));
        var request = new RegistreerAfdelingRequest
        {
            Vertegenwoordigers = new []
            {
                new ToeTeVoegenVertegenwoordiger
                {
                    Voornaam = voornaam,
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(
            $"{nameof(RegistreerAfdelingRequest.Vertegenwoordigers)}[0].{nameof(ToeTeVoegenVertegenwoordiger.Voornaam)}");
    }
}
