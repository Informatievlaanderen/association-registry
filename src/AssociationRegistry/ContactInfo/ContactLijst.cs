namespace AssociationRegistry.ContactInfo;

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
        => listOfContactInfo is null ? Empty : new ContactLijst(listOfContactInfo);

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
                        info.SocialMedia)));
}
