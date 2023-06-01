namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
using Framework;
using AutoFixture;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Invalid_Locatietype : ValidatorTest
{
    [Fact]
    public void Has_validation_error__locatieType_moet_juiste_waarde_hebben()
    {
        var validator = new RegistreerAfdelingRequestValidator();
        var request = new RegistreerAfdelingRequest
        {
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
                {
                    Locatietype = new Fixture().Create<string>(),
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerAfdelingRequest.Locaties)}[0].{nameof(ToeTeVoegenLocatie.Locatietype)}")
            .WithErrorMessage($"'Locatietype' moet een geldige waarde hebben. ({Locatietypes.Correspondentie}, {Locatietypes.Activiteiten}");
    }
}
