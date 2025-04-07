namespace AssociationRegistry.Grar.NutsLau;

public static class Postcode
{
    public static bool IsVlaamsePostcode(string postcode)
    {
        if (!int.TryParse(postcode, out var code))
            return false;

        return (code >= 1500 && code <= 3999) ||
               (code >= 8000 && code <= 9999);
    }
}
