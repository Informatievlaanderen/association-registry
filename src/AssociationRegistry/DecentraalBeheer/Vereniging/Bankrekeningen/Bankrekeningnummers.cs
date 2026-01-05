namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;

using Events;
using System.Collections.ObjectModel;

public class Bankrekeningnummers : ReadOnlyCollection<Bankrekeningnummer>
{
    private const int InitialId = 1;
    public int NextId { get; }

    public static Bankrekeningnummers Empty
        => new(Array.Empty<Bankrekeningnummer>(), InitialId);

    private Bankrekeningnummers(IEnumerable<Bankrekeningnummer> bankrekeningnummers, int nextId)
        : base(bankrekeningnummers.ToArray())
    {
        NextId = nextId;
    }

    public Bankrekeningnummer VoegToe(ToeTevoegenBankrekeningnummer bankrekeningnummer)
    {
        var toeTeVoegenBankrekeningnummer = Bankrekeningnummer.Create(
            NextId,
            bankrekeningnummer);

        ThrowIfCannotAppendOrUpdate(toeTeVoegenBankrekeningnummer);

        return toeTeVoegenBankrekeningnummer;
    }

    private void ThrowIfCannotAppendOrUpdate(Bankrekeningnummer toeTeVoegenBankrekeningnummer)
    {
        // TODO: add validation
    }

    public Bankrekeningnummers Hydrate(IEnumerable<Bankrekeningnummer> bankrekeningnummers)
    {
        bankrekeningnummers = bankrekeningnummers.ToArray();

        if (!bankrekeningnummers.Any())
            return new Bankrekeningnummers(Empty, Math.Max(InitialId, NextId));

        return new Bankrekeningnummers(bankrekeningnummers, Math.Max(bankrekeningnummers.Max(x => x.Id) + 1, NextId));
    }


}

public static class BankrekeningnummersEnumerableExtensions
{
    public static IEnumerable<Bankrekeningnummer> Without(this IEnumerable<Bankrekeningnummer> source, Bankrekeningnummer bankrekeningnummer)
    {
        return source.Where(c => c.Id != bankrekeningnummer.Id);
    }

    public static IEnumerable<Bankrekeningnummer> Without(this IEnumerable<Bankrekeningnummer> source, int id)
    {
        return source.Where(c => c.Id != id);
    }

    public static IEnumerable<Bankrekeningnummer> AppendFromEventData(this IEnumerable<Bankrekeningnummer> bankrekeningnummers, BankrekeningnummerWerdToegevoegd eventData)
        => bankrekeningnummers.Append(
            Bankrekeningnummer.Hydrate(
                eventData.Id,
                eventData.IBAN,
                eventData.GebruiktVoor,
                eventData.Titularis));
}
