namespace AssociationRegistry.Admin.Api.Verenigingen.KboNummers;

using Exceptions;

public class KboNummer
{
    private KboNummer(string kboNummer)
    {
        var value = kboNummer
            .Replace(" ", string.Empty)
            .Replace(".", string.Empty);

        if (value.Length != 10)
            throw new InvalidKboNummer();

        if (!ulong.TryParse(value, out _))
            throw new InvalidKboNummer();

        Value = value;
    }

    public string Value { get; }

    public override string ToString()
        => Value;

    public static KboNummer? Create(string? maybeKboNummer)
        => maybeKboNummer is { } kboNummer
            ? new KboNummer(kboNummer)
            : null;
}
