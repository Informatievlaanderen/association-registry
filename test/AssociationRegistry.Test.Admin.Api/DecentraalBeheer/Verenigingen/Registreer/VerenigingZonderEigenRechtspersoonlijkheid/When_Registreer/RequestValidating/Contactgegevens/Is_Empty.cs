namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingZonderEigenRechtspersoonlijkheid.When_Registreer.RequestValidating.
    Contactgegevens;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequetsModels;
using AssociationRegistry.Test.Framework;
using FluentValidation.TestHelper;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error__contactgegeven_is_verplicht()
    {
        var validator = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(new ClockStub(DateOnly.MaxValue));
        var request = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest();
        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(toeRequest => toeRequest.Contactgegevens);
    }
}
