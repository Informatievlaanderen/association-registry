namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Koepels.When_Wijzig_Koepelgegevens.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Koepels.RequestModels;
using FluentValidation.TestHelper;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

public class A_Valid_Request : ValidatorTest
{
    private readonly WijzigKoepelgegevensRequestValidator _validator = new();

    [Theory]
    [InlineData("Koepel", true)]
    [InlineData("Koepel", false)]
    [InlineData("GeenKoepel", false)]
    [InlineData("NietBepaald", false)]
    [InlineData("Koepel", null)]
    [InlineData("koepel", true)]
    [InlineData("GEENKOEPEL", null)]
    [InlineData("nietBepaald", false)]
    [InlineData(null, true)]
    [InlineData(null, false)]
    public void Has_no_validation_errors(string? koepelstatus, bool? privacyVerklaringGeaccepteerd)
    {
        var result = _validator.TestValidate(
            new WijzigKoepelgegevensRequest
            {
                Koepelstatus = koepelstatus,
                PrivacyVerklaringGeaccepteerd = privacyVerklaringGeaccepteerd,
            }
        );

        result.ShouldNotHaveAnyValidationErrors();
    }
}
