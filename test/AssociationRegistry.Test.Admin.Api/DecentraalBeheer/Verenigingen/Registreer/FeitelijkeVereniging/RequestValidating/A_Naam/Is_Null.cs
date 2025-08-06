namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequestValidating.A_Naam;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Test.Framework;
using FluentValidation.TestHelper;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

public class Is_Null : ValidatorTest
{
    [Fact]
    public void Has_validation_error__naam_is_verplicht()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));
        var result = validator.TestValidate(new RegistreerFeitelijkeVerenigingRequest());

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Naam)
              .WithErrorMessage("'Naam' is verplicht.");
    }
}
