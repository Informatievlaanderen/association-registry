namespace AssociationRegistry.Grar.Models.PostalInfo;

public record Postnaam
{
    public string Value { get; init; }

    private Postnaam(string Value)
    {
        this.Value = Value;
    }

    public static Postnaam FromGrar(Contracts.Contracts.Postnaam postnaam)
        => new(postnaam.GeografischeNaam.Spelling);

    public static Postnaam FromValue(string postnaam)
    {
        if (string.IsNullOrEmpty(postnaam))
            throw new ArgumentException(nameof(postnaam));

        return new Postnaam(postnaam);
    }

    public static implicit operator string(Postnaam postnaam)
        => postnaam.Value;

    public override string ToString()
        => Value;

    public void Deconstruct(out string Value)
    {
        Value = this.Value;
    }

    public bool IsEquivalentTo(string gemeentenaam)
        => string.Equals(gemeentenaam, Value,
                         StringComparison.CurrentCultureIgnoreCase);
};
