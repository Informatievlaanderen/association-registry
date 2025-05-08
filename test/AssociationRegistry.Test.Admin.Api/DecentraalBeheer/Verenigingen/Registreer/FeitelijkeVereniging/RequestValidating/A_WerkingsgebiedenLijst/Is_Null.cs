namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequestValidating.
    A_WerkingsgebiedenLijst;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Test.Framework;
using FluentValidation.TestHelper;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

public class Is_Null : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));

        var result = validator.TestValidate(new RegistreerFeitelijkeVerenigingRequest());

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Werkingsgebieden);
    }
}
