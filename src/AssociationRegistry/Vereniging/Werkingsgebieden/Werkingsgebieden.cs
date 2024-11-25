namespace AssociationRegistry.Vereniging;

using Exceptions;
using Framework;
using System.Collections.ObjectModel;

public class Werkingsgebieden : ReadOnlyCollection<Werkingsgebied>
{
    private Werkingsgebieden(Werkingsgebied[] werkingsgebieden) : base(werkingsgebieden)
    {
    }

    public static Werkingsgebieden NietBepaald = new(Array.Empty<Werkingsgebied>());

    public static Werkingsgebieden NietVanToepassing = new([Werkingsgebied.NietVanToepassing]);
    public bool IsNietBepaald => Equals(NietBepaald);
    public bool IsNietVanToepassing => Equals(NietVanToepassing);

    public static Werkingsgebieden FromArray(Werkingsgebied[] werkingsgebieden)
    {
        Throw<WerkingsgebiedNietVanToepassingKanNietGecombineerdWorden>.If(HasMoreThanOnlyNietVanToepassing(werkingsgebieden));

        Throw<WerkingsgebiedIsDuplicaat>.If(HasDuplicateWerkingsgebied(werkingsgebieden));

        return new Werkingsgebieden(werkingsgebieden);
    }

    private static bool HasDuplicateWerkingsgebied(Werkingsgebied[] werkingsgebieden)
    {
        return werkingsgebieden.DistinctBy(x => x.Code).Count() != werkingsgebieden.Length;
    }

    private static bool HasMoreThanOnlyNietVanToepassing(Werkingsgebied[] werkingsgebieden)
        => werkingsgebieden.Any(werkingsgebied => werkingsgebied == Werkingsgebied.NietVanToepassing) && werkingsgebieden.Length > 1;

    public static Werkingsgebieden Hydrate(Werkingsgebied[] werkingsgebieden)
        => new(werkingsgebieden);

    public static implicit operator Werkingsgebied[](Werkingsgebieden werkingsgebieden)
        => werkingsgebieden.ToArray();

    public static bool Equals(
        Werkingsgebied[] werkingsgebieden1,
        Werkingsgebied[] werkingsgebieden2)
        => werkingsgebieden1.Length == werkingsgebieden2.Length &&
           werkingsgebieden1.All(werkingsgebieden2.Contains);

    protected bool Equals(Werkingsgebieden other)
    {
        if (other is null)
            return false;

        if (Count != other.Count)
            return false;

        // Compare all elements.
        return this.SequenceEqual(other);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj.GetType() != GetType())
            return false;

        return Equals((Werkingsgebieden)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 19;
            foreach (var item in this)
            {
                hash = hash * 31 + (item?.GetHashCode() ?? 0);
            }
            return hash;
        }
    }

    public static bool operator ==(Werkingsgebieden? left, Werkingsgebieden? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Werkingsgebieden? left, Werkingsgebieden? right)
        => !(left == right);

}
