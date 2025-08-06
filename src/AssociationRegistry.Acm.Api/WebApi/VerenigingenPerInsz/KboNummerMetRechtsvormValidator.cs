namespace AssociationRegistry.Acm.Api.WebApi.VerenigingenPerInsz;

using FluentValidation;

public class KboNummerMetRechtsvormValidator : AbstractValidator<VerenigingenPerInszRequest.KboNummerMetRechtsvormRequest>
{
    public KboNummerMetRechtsvormValidator()
    {
        RuleFor(r => r.KboNummer)
           .NotEmpty()
           .WithMessage("'KboNummer' mag niet leeg zijn.")
           .NotNull()
           .WithMessage("'KboNummer' moet ingevuld zijn.");

        RuleFor(r => r.Rechtsvorm)
           .NotEmpty()
           .WithMessage("'Rechtsvorm' mag niet leeg zijn.")
           .NotNull()
           .WithMessage("'Rechtsvorm' moet ingevuld zijn.");
    }
}
