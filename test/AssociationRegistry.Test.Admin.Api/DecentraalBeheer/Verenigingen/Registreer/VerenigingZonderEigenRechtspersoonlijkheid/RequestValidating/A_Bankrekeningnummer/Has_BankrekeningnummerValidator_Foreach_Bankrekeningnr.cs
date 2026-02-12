namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestValidating.A_Bankrekeningnummer;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Test.Common.StubsMocksFakes.Clocks;
using FluentValidation.TestHelper;
using Xunit;

public class Has_BankrekeningnummerValidator_Foreach_Bankrekeningnr
{
    [Fact]
    public void Should_use_BankrekeningnummerValidator_for_each_bankrekeningnummer()
    {
        var validator = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(
            clock: new ClockStub(now: DateOnly.MaxValue)
        );

        validator.ShouldHaveChildValidator(
            expression: x => x.Bankrekeningnummers,
            childValidatorType: typeof(VoegBankrekeningnummerToeValidator.BankrekeningnummerValidator)
        );
    }
}
