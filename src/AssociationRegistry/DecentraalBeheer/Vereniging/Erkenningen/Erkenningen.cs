namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

using System.Collections.ObjectModel;
using Events;
using Exceptions;
using Exceptions.Ipdc;
using Framework;

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

    public Erkenning GetById(int erkenningId) => this.Single(x => x.ErkenningId == erkenningId);

    private new Erkenning this[int erkenningId] => this.Single(x => x.ErkenningId == erkenningId);

    public Erkenning VoegToe(TeRegistrerenErkenning erkenning, IpdcProduct ipdcProduct, GegevensInitiator initiator)
    {
        var teRegistrerenErkenning = Erkenning.Create(NextId, erkenning, ipdcProduct, initiator);

        ThrowIfCannotAppendOrUpdate(teRegistrerenErkenning);

        return teRegistrerenErkenning;
    }

    private void ThrowIfCannotAppendOrUpdate(Erkenning teRegisterenErkenning)
    {
        Throw<IpdcProductNummerOntbreekt>.If(string.IsNullOrEmpty(teRegisterenErkenning.IpdcProduct.Nummer));

        Throw<HernieuwingsDatumMoetTussenStartEnEindDatumLiggen>.If(
            teRegisterenErkenning.ErkenningsPeriode.Einddatum <= teRegisterenErkenning.ErkenningsPeriode.Startdatum
        );

        var erkenningen = this.Append(teRegisterenErkenning);
        Throw<ErkenningBestaatAl>.If(HasDuplicateErkenning(erkenningen));
    }

    private bool HasDuplicateErkenning(IEnumerable<Erkenning> erkenningen)
    {
        return erkenningen.DistinctBy(x => (x.VCode, x.IpdcProduct.Nummer, x.GeregistreerdDoor.OvoCode)).Count()
            != erkenningen.Count();
    }

    public Erkenningen Hydrate(IEnumerable<Erkenning> erkenningen)
    {
        erkenningen = erkenningen.ToArray();

        if (!erkenningen.Any())
            return new Erkenningen(Empty, Math.Max(InitialId, NextId));

        return new Erkenningen(erkenningen, Math.Max(erkenningen.Max(x => x.ErkenningId) + 1, NextId));
    }
}

public static class ErkenningenEnumerableExtensions
{
    public static IEnumerable<Erkenning> AppendFromEventData(
        this IEnumerable<Erkenning> erkenningen,
        ErkenningWerdGeregistreerd eventData
    ) =>
        erkenningen.Append(
            Erkenning.Hydrate(
                eventData.ErkenningId,
                eventData.GeregistreerdDoor,
                eventData.IpdcProduct,
                eventData.Startdatum,
                eventData.Einddatum,
                eventData.Hernieuwingsdatum,
                eventData.HernieuwingsUrl,
                string.Empty,
                eventData.Status
            )
        );
}
