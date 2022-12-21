namespace AssociationRegistry.Contacten;

using Vereniging;

public class Contacten : List<ContactInfo>
{
    internal Contacten(IEnumerable<ContactInfo> listOfContactInfo)
    {
        AddRange(listOfContactInfo);
    }

    private Contacten()
    {
    }

    public static Contacten Empty
        => new();

    public static Contacten Create(IEnumerable<ContactInfo>? listOfContactInfo)
        => listOfContactInfo is null ? Empty : new Contacten(listOfContactInfo);

    public static Contacten Create(IEnumerable<RegistreerVerenigingCommand.ContactInfo>? listOfContactInfo)
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
