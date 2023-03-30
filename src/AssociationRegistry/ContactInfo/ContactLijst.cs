namespace AssociationRegistry.ContactInfo;

using System.Collections.ObjectModel;
using Exceptions;
using Framework;

public class ContactLijst : ReadOnlyCollection<ContactInfo>
{
    private ContactLijst(IEnumerable<ContactInfo> listOfContactInfo) : base(listOfContactInfo.ToList())
    {
    }

    public static ContactLijst Empty
        => new(Enumerable.Empty<ContactInfo>());

    public ContactInfo[] ExcludeByName(ContactLijst contactInfoLijst)
    {
        return contactInfoLijst.ExceptBy(
                this.Select(x => x.Contactnaam),
                info => info.Contactnaam)
            .ToArray();
    }

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

    public static ContactLijst Create(IEnumerable<Vereniging.CommonCommandDataTypes.ContactInfo>? listOfContactInfo)
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

    public ContactLijst MetToevoegingen(Events.CommonEventDataTypes.ContactInfo[] toevoegingen)
        => Create(this.Concat(toevoegingen.Select(ContactInfo.FromEvent)));


    public ContactLijst MetVerwijderingen(Events.CommonEventDataTypes.ContactInfo[] verwijderingen)
        => Create(this.Except(verwijderingen.Select(ContactInfo.FromEvent)));

    public ContactLijst MetWijzigingen(Events.CommonEventDataTypes.ContactInfo[] wijzigingen)
        => Create(
            this
                .ExceptBy(wijzigingen.Select(x => x.Contactnaam), info => info.Contactnaam)
                .Concat(wijzigingen.Select(ContactInfo.FromEvent)));
}
