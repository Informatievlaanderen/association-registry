namespace AssociationRegistry.Admin.Api.Verenigingen.CommonRequestDataTypes;

using System.Linq;
using AssociationRegistry.Admin.Api.Infrastructure.Validation;
using FluentValidation;

public class ContactInfoValidator : AbstractValidator<ContactInfo>
{
    public ContactInfoValidator()
    {
        RuleFor(contactInfo => contactInfo)
            .Must(HaveAtLeastOneValue)
            .WithMessage("Een contact moet minstens één waarde bevatten.");

        this.RequireNotNullOrEmpty(contactInfo => contactInfo.Contactnaam);
    }

    private static bool HaveAtLeastOneValue(ContactInfo contactInfo)
        => !string.IsNullOrEmpty(contactInfo.Email) ||
           !string.IsNullOrEmpty(contactInfo.Telefoon) ||
           !string.IsNullOrEmpty(contactInfo.Website) ||
           !string.IsNullOrEmpty(contactInfo.SocialMedia);

    internal static bool NotHaveMultiplePrimaryContactInfos(ContactInfo[] contactInfos)
        => contactInfos.Count(i => i.PrimairContactInfo) <= 1;

    public static bool NotHaveDuplicateContactnaam(ContactInfo[] arg)
    {
        var totalItems = arg.Length;
        var distinct = arg.Select(i => i.Contactnaam).Distinct().Count();
        return totalItems == distinct;
    }
}
