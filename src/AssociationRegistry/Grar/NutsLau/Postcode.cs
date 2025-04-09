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

    public static bool IsBrusselPostcode(string postcode)
    {
        if (!int.TryParse(postcode, out var code))
            return false;

        return (code >= 1000 && code <= 1299);
    }
    
    public static bool IsWaalsePostcode(string postcode) => !IsBrusselPostcode(postcode) && !IsVlaamsePostcode(postcode);
}
