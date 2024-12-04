namespace AssociationRegistry.Grar.Models.PostalInfo;

using System.Collections.ObjectModel;

public record PostalInformationResponse(
    string Postcode,
    string Gemeentenaam,
    Postnamen Postnamen);

public class Postnamen: ReadOnlyCollection<Postnaam>
{
    private Postnamen(IList<Postnaam> list) : base(list)
    {
    }

    public bool HasSinglePostnaam => this.Count == 1;


    public Postnaam? FindSingleWithGemeentenaam(string origineleGemeentenaamClean)
    {
        return this.SingleOrDefault(
            sod => sod.Value.Equals(origineleGemeentenaamClean, StringComparison.InvariantCultureIgnoreCase));
    }

    public Postnaam? FindSingleOrDefault()
        => HasSinglePostnaam ? this.SingleOrDefault() : null;

    public static Postnamen FromPostalInfo(List<Models.Postnaam> postnamen)
        => new (postnamen.Select(Postnaam.FromGrar).ToList());

    public static Postnamen FromValues(params string[] values)
        => new(values.Select(Postnaam.FromValue).ToList());

    public static Postnamen Empty => new ([]);
}

public record Postnaam
{
    public string Value { get; init; }

    private Postnaam(string Value)
    {
        this.Value = Value;
    }

    public static Postnaam FromGrar(Models.Postnaam postnaam)
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
        => string.Equals(gemeentenaam, this.Value,
                         StringComparison.CurrentCultureIgnoreCase);
};
