namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestValidating.A_Locatie.A_AdresId;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Framework;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using Test.Framework;
using Xunit;
using Xunit.Categories;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class With_A_Null_Broncode : ValidatorTest
{
    [Fact]
    public void Has_validation_error__broncode_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));
        var request = new Fixture().CustomizeAdminApi().Create<RegistreerFeitelijkeVerenigingRequest>();
        request.Locaties[0].AdresId!.Broncode = null!;

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                $"{nameof(RegistreerFeitelijkeVerenigingRequest.Locaties)}[0].{nameof(ToeTeVoegenLocatie.AdresId)}.{nameof(ToeTeVoegenLocatie.AdresId.Broncode)}")
            .WithErrorMessage("'Broncode' is verplicht.");
    }
}
