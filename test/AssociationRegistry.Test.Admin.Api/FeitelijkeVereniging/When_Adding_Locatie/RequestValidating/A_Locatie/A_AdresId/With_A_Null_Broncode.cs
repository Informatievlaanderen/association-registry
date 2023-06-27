﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Adding_Locatie.RequestValidating.A_Locatie.A_AdresId;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using Framework;
using AutoFixture;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Null_Broncode : ValidatorTest
{
    [Fact]
    public void Has_validation_error__broncode_mag_niet_leeg_zijn()
    {
        var validator = new VoegLocatieToeValidator();
        var request = new Fixture().CustomizeAll().Create<VoegLocatieToeRequest>();
        request.Locatie.AdresId!.Broncode = null!;

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                $"{nameof(VoegLocatieToeRequest.Locatie)}.{nameof(ToeTeVoegenLocatie.AdresId)}.{nameof(ToeTeVoegenLocatie.AdresId.Broncode)}")
            .WithErrorMessage("'Broncode' is verplicht.");
    }
}
