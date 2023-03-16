namespace AssociationRegistry.Vereniging;

using ContactInfo;
using ContactInfo = ContactInfo.ContactInfo;

public static class ContactInfoLijstDiffer
{
    public static Diff Calculate(ContactLijst lijst1, ContactLijst lijst2)
    {
        var toevoegingen = lijst1.ExcludeByName(lijst2);

        var verwijderingen = lijst2.ExcludeByName(lijst1);

        var wijzigingen = lijst2
            .Except(toevoegingen)
            .Except(verwijderingen)
            .Where(info => !lijst1.Any(info.Equals))
            .ToArray();

        return new Diff(toevoegingen, verwijderingen, wijzigingen);
    }

    public record Diff(ContactInfo[] Toevoegingen, ContactInfo[] Verwijderingen, ContactInfo[] Wijzigingen)
    {
        public bool HasChanges
            => Toevoegingen.Any() ||
               Verwijderingen.Any() ||
               Wijzigingen.Any();
    }
}
