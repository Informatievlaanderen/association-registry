namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestValidating.A_Bankrekeningnummer;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.StubsMocksFakes.Clocks;
using AutoFixture;
using FluentValidation.TestHelper;
using Xunit;

public class With_Duplicate_Iban
{
    [Fact]
    public void Has_validation_error_Iban_Moet_Uniek_Zijn()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var validator = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(
            clock: new ClockStub(now: DateOnly.MaxValue)
        );

        var iban = fixture.Create<IbanNummer>().Value;

        var request = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest()
        {
            Bankrekeningnummers =
            [
                new ToeTeVoegenBankrekeningnummer() { Iban = iban },
                new ToeTeVoegenBankrekeningnummer() { Iban = iban },
            ],
        };

        var result = validator.TestValidate(objectToTest: request);

        result
            .ShouldHaveValidationErrorFor(
                propertyName: nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest.Bankrekeningnummers)
            )
            .WithErrorMessage(
                expectedErrorMessage: "Het IBAN van een bankrekening moet uniek zijn binnen de vereniging."
            );
    }
}
