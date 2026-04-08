namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

using System.Collections.ObjectModel;
using DecentraalBeheer.Vereniging.Exceptions;
using Events;
using Exceptions;
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

    public Erkenning GetById(int erkenningId) =>
        this.Single(x => x.ErkenningId == erkenningId);

    private new Erkenning this[int erkenningId] =>
        this.Single(x => x.ErkenningId == erkenningId);

    public Erkenning VoegToe(TeRegistrerenErkenning erkenning, string vCode)
    {
        var teRegistrerenErkenning = Erkenning.Create(NextId, erkenning);
        teRegistrerenErkenning.VCode = vCode;
        ThrowIfCannotAppendOrUpdate(teRegistrerenErkenning);

        return teRegistrerenErkenning;
    }
    public Erkenning[] VoegToe(TeRegistrerenErkenning[] teRegistrerenErkenningen, string vCode)
    {
        var erkenningen = this;
        var toegevoegdeErkenningen = Array.Empty<Erkenning>();

        foreach (var teRegistrerenErkenning in teRegistrerenErkenningen)
        {
            var erkenningenMetId = erkenningen.VoegToe(teRegistrerenErkenning, vCode);

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
        Throw<StartdatumLigtNaEinddatum>.If(
            teRegisterenErkenning.Einddatum <= teRegisterenErkenning.Startdatum);

        Throw<IpdcProductNummerOntbreekt>.If(teRegisterenErkenning.IpdcProduct == null ||
            string.IsNullOrEmpty(teRegisterenErkenning.IpdcProduct.Nummer));

        Throw<URLStartNietMetHttpOfHttps>.If(
            !teRegisterenErkenning.HernieuwingsUrl.StartsWith("http://")
            && !teRegisterenErkenning.HernieuwingsUrl.StartsWith("https://")
            );

        var erkenningen = this.Append(teRegisterenErkenning);
        Throw<ErkenningBestaatAl>.If(HasDuplicateErkenning(erkenningen));

    }

    private bool HasDuplicateErkenning(IEnumerable<Erkenning> erkenningen)
    {
        return erkenningen
              .DistinctBy(x => (x.VCode, x.IpdcProduct.Nummer, x.IpdcProduct.Naam))
              .Count() != erkenningen.Count();
    }

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

public static class ErkenningenEnumerableExtensions
{
    public static IEnumerable<Erkenning> AppendFromEventData(
        this IEnumerable<Erkenning> erkenningen,
        ErkenningWerdGeregistreerd eventData
    ) =>
        erkenningen.Append(
            Erkenning.Hydrate(
                eventData.ErkenningId,
                eventData.VCode,
                eventData.GeregistreerdDoor,
                eventData.IpdcProduct,
                eventData.Startdatum,
                eventData.Einddatum,
                eventData.Hernieuwingsdatum,
                eventData.HernieuwingsUrl,
                string.Empty
            )
        );
}
