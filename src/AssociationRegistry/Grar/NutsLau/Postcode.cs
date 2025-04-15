namespace AssociationRegistry.Grar.NutsLau;

public static class Postcode
{
    public static bool IsVlaamsePostcode(string postcode)
    {
        if (!int.TryParse(postcode, out var code))
            return false;

        return (1500 <= code && code <= 3999) ||
               (8000 <= code && code <= 9999);
    }

    public static bool IsBrusselPostcode(string postcode)
    {
        if (!int.TryParse(postcode, out var code))
            return false;

        return (1000 <= code && code <= 1299);
    }

    public static bool IsWaalsePostcode(string postcode) => !IsBrusselPostcode(postcode) && !IsVlaamsePostcode(postcode);
}
