namespace AssociationRegistry.ContactInfo;

using Exceptions;
using Framework;
using Urls;
using Vereniging.RegistreerVereniging;

public class ContactLijst : List<ContactInfo>
{
    private ContactLijst(IEnumerable<ContactInfo> listOfContactInfo)
    {
        AddRange(listOfContactInfo);
    }

    private ContactLijst()
    {
    }

    public static ContactLijst Empty
        => new();

    public static ContactLijst Create(IEnumerable<ContactInfo>? listOfContactInfo)
    {
        if (listOfContactInfo is null)
            return Empty;

        var contactInfoArray = listOfContactInfo as ContactInfo[] ?? listOfContactInfo.ToArray();

        Throw<DuplicateContactnaam>.If(HasDuplicateContactnaam(contactInfoArray));
        Throw<MultiplePrimaryContactInfos>.If(HasMultiplePrimaryContactInfos(contactInfoArray));
        return new ContactLijst(contactInfoArray);
    }

    private static bool HasMultiplePrimaryContactInfos(IEnumerable<ContactInfo> contactInfos)
        => contactInfos.Count(info => info.PrimairContactInfo) > 1;

    private static bool HasDuplicateContactnaam(ContactInfo[] contactInfos)
    {
        var totalItems = contactInfos.Count();
        var distinct = contactInfos.Select(i => i.Contactnaam).Distinct().Count();
        return totalItems != distinct;
    }

    public static ContactLijst Create(IEnumerable<RegistreerVerenigingCommand.ContactInfo>? listOfContactInfo)
        => listOfContactInfo is null
            ? Empty
            : Create(
                listOfContactInfo.Select(
                    info => ContactInfo.CreateInstance(
                        info.Contactnaam,
                        info.Email,
                        info.Telefoon,
                        info.Website,
                        info.SocialMedia,
                        info.PrimairContactInfo)));
}
