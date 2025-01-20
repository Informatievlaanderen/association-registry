namespace AssociationRegistry.Admin.Api.Verenigingen.Common;

using AssociationRegistry.Vereniging;
using FluentValidation;

public class DoelgroepRequestValidator : AbstractValidator<DoelgroepRequest>
{
    public DoelgroepRequestValidator()
    {
        RuleFor(r => r)
           .Must(HaveValidLeeftijdRange)
           .WithMessage("Bij 'doelgroep' moet de 'minimumleeftijd' kleiner of gelijk aan 'maximumleeftijd' zijn.");

        RuleFor(r => r.Minimumleeftijd)
           .Must(BeBetween0And150Inclusive)
           .When(r => r.Minimumleeftijd is not null)
           .WithMessage("De 'minimumleeftijd' moet binnen 0 en 150 vallen.");

        RuleFor(r => r.Maximumleeftijd)
           .Must(BeBetween0And150Inclusive)
           .When(r => r.Maximumleeftijd is not null)
           .WithMessage("De 'maximumleeftijd' moet binnen 0 en 150 vallen.");
    }

    private static bool BeBetween0And150Inclusive(int? number)
        => number is >= Doelgroep.StandaardMinimumleeftijd and <= Doelgroep.StandaardMaximumleeftijd;

    private static bool HaveValidLeeftijdRange(DoelgroepRequest doelgroep)
        => (doelgroep.Maximumleeftijd ?? Doelgroep.StandaardMaximumleeftijd) >=
           (doelgroep.Minimumleeftijd ?? Doelgroep.StandaardMinimumleeftijd);
}
