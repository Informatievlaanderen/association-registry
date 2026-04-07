namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

using System.Collections.ObjectModel;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging.Bronnen;
using Bankrekeningen;

public class Erkenningen : ReadOnlyCollection<Erkenning>
{
    private const int InitialId = 1;
    public int NextId { get; }
    public static Erkenningen Empty => new(Array.Empty<Erkenning>(), InitialId);

    private Erkenningen(IEnumerable<Erkenning> erkenningen, int nextId)
        : base(erkenningen.ToArray())
    {
        NextId = nextId;
    }

    public Erkenning GetById(int erkenningId) =>
        this.Single(x => x.ErkenningId == erkenningId);

    private new Erkenning this[int erkenningId] =>
        this.Single(x => x.ErkenningId == erkenningId);

    public Erkenning VoegToe(TeRegistrerenErkenning erkenning)
    {
        var teRegistrerenErkenning = Erkenning.Create(NextId, erkenning);

        ThrowIfCannotAppendOrUpdate(teRegistrerenErkenning);

        return teRegistrerenErkenning;
    }
    public Erkenning[] VoegToe(TeRegistrerenErkenning[] teRegistrerenErkenningen)
    {
        var erkenningen = this;
        var toegevoegdeErkenningen = Array.Empty<Erkenning>();

        foreach (var teRegistrerenErkenning in teRegistrerenErkenningen)
        {
            var erkenningenMetId = erkenningen.VoegToe(teRegistrerenErkenning);

            erkenningen = new Erkenningen(
                erkenningen.Append(erkenningenMetId),
                erkenningen.NextId + 1
            );

            toegevoegdeErkenningen = toegevoegdeErkenningen.Append(erkenningenMetId).ToArray();
        }

        return toegevoegdeErkenningen;
    }


    private void ThrowIfCannotAppendOrUpdate(Erkenning teRegisterenErkenning)
    {
        var erkenningen = this.Append(teRegisterenErkenning).ToArray();

        // Throw<IbanMoetUniekZijn>.If(HasDuplicateIban(bankrekeningnummers));
    }

    // private bool HasDuplicateIban(Erkenning[] erkenningen)
    // {
    //     return erkenningen.DistinctBy(x => x.Iban).Count() != bankrekeningnummers.Count();
    // }

    public Erkenningen Hydrate(IEnumerable<Erkenning> erkenningen)
    {
        erkenningen = erkenningen.ToArray();

        if (!erkenningen.Any())
            return new Erkenningen(Empty, Math.Max(InitialId, NextId));

        return new Erkenningen(
            erkenningen,
            Math.Max(erkenningen.Max(x => x.ErkenningId) + 1, NextId)
        );
    }
}
