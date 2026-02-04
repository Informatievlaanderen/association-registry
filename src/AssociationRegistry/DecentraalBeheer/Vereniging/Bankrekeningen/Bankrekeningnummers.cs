namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;

using System.Collections.ObjectModel;
using Events;
using Exceptions;
using Framework;

public class Bankrekeningnummers : ReadOnlyCollection<Bankrekeningnummer>
{
    private const int InitialId = 1;
    public int NextId { get; }

    public static Bankrekeningnummers Empty => new(Array.Empty<Bankrekeningnummer>(), InitialId);

    private Bankrekeningnummers(IEnumerable<Bankrekeningnummer> bankrekeningnummers, int nextId)
        : base(bankrekeningnummers.ToArray())
    {
        NextId = nextId;
    }

    public Bankrekeningnummer GetById(int bankrekeningnummerId) =>
        this.Single(x => x.BankrekeningnummerId == bankrekeningnummerId);

    private new Bankrekeningnummer this[int bankrekeningummerId] =>
        this.Single(x => x.BankrekeningnummerId == bankrekeningummerId);

    public Bankrekeningnummer VoegToe(ToeTevoegenBankrekeningnummer bankrekeningnummer)
    {
        var toeTeVoegenBankrekeningnummer = Bankrekeningnummer.Create(NextId, bankrekeningnummer);

        ThrowIfCannotAppendOrUpdate(toeTeVoegenBankrekeningnummer);

        return toeTeVoegenBankrekeningnummer;
    }

    public Bankrekeningnummer? Wijzig(TeWijzigenBankrekeningnummer bankrekeningnummer)
    {
        MustContain(bankrekeningnummer.BankrekeningnummerId);

        var teWijzigenBankrekeningnummer = this[bankrekeningnummer.BankrekeningnummerId];

        if (teWijzigenBankrekeningnummer.WouldBeEquivalent(bankrekeningnummer, out var gewijzigdBankrekeningnummer))
            return null;

        return gewijzigdBankrekeningnummer;
    }

    private void MustContain(int bankrekeningnummerId)
    {
        Throw<BankrekeningnummerIsNietGekend>.If(!HasKey(bankrekeningnummerId), bankrekeningnummerId.ToString());
    }

    private bool HasKey(int bankrekeningnummerId) =>
        this.Any(bankrekeningnummer => bankrekeningnummer.BankrekeningnummerId == bankrekeningnummerId);

    private void ThrowIfCannotAppendOrUpdate(Bankrekeningnummer toeTeVoegenBankrekeningnummer)
    {
        var bankrekeningnummers = this.Append(toeTeVoegenBankrekeningnummer).ToArray();

        Throw<IbanMoetUniekZijn>.If(HasDuplicateIban(bankrekeningnummers));
    }

    private bool HasDuplicateIban(Bankrekeningnummer[] bankrekeningnummers)
    {
        return bankrekeningnummers.DistinctBy(x => x.Iban).Count() != bankrekeningnummers.Count();
    }

    private bool HasDuplicateDoel(Bankrekeningnummer[] bankrekeningnummers)
    {
        return bankrekeningnummers.DistinctBy(x => x.Doel).Count() != bankrekeningnummers.Count();
    }

    public Bankrekeningnummers Hydrate(IEnumerable<Bankrekeningnummer> bankrekeningnummers)
    {
        bankrekeningnummers = bankrekeningnummers.ToArray();

        if (!bankrekeningnummers.Any())
            return new Bankrekeningnummers(Empty, Math.Max(InitialId, NextId));

        return new Bankrekeningnummers(
            bankrekeningnummers,
            Math.Max(bankrekeningnummers.Max(x => x.BankrekeningnummerId) + 1, NextId)
        );
    }
}

public static class BankrekeningnummersEnumerableExtensions
{
    public static Bankrekeningnummer[] FindToeTeVoegenBankrekeningnummers(
        this Bankrekeningnummers source,
        Bankrekeningnummer[] vertegenwoordigersUitKbo
    )
    {
        var nextId = source.NextId;

        var toeTeVoegenVertegenwoordigers = new List<Bankrekeningnummer>();

        foreach (var v in vertegenwoordigersUitKbo)
        {
            if (!source.Select(x => x.Iban).Contains(v.Iban))
                toeTeVoegenVertegenwoordigers.Add(v with { BankrekeningnummerId = nextId++ });
        }

        return toeTeVoegenVertegenwoordigers.ToArray();
    }

    public static IEnumerable<Bankrekeningnummer> FindTeVerwijderdenBankrekeningnummers(
        this Bankrekeningnummers source,
        Bankrekeningnummer[] vertegenwoordigersUitKbo
    )
    {
        return source.Where(s => !vertegenwoordigersUitKbo.Select(x => x.Iban).Contains(s.Iban));
    }

    public static IEnumerable<Bankrekeningnummer> Without(
        this IEnumerable<Bankrekeningnummer> source,
        Bankrekeningnummer bankrekeningnummer
    )
    {
        return source.Where(c => c.BankrekeningnummerId != bankrekeningnummer.BankrekeningnummerId);
    }

    public static IEnumerable<Bankrekeningnummer> Without(this IEnumerable<Bankrekeningnummer> source, int id)
    {
        return source.Where(c => c.BankrekeningnummerId != id);
    }

    public static IEnumerable<Bankrekeningnummer> AppendFromEventData(
        this IEnumerable<Bankrekeningnummer> bankrekeningnummers,
        BankrekeningnummerWerdToegevoegd eventData
    ) =>
        bankrekeningnummers.Append(
            Bankrekeningnummer.Hydrate(
                eventData.BankrekeningnummerId,
                eventData.Iban,
                eventData.Doel,
                eventData.Titularis
            )
        );

    public static IEnumerable<Bankrekeningnummer> AppendFromEventData(
        this IEnumerable<Bankrekeningnummer> bankrekeningnummers,
        BankrekeningnummerWerdToegevoegdVanuitKBO eventData
    ) =>
        bankrekeningnummers.Append(
            Bankrekeningnummer.Hydrate(eventData.BankrekeningnummerId, eventData.Iban, string.Empty, string.Empty)
        );
}
