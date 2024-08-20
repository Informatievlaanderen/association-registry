namespace AssociationRegistry.Vereniging;

using Emails;
using Events;
using Exceptions;
using Framework;
using Grar;
using Grar.Exceptions;
using Grar.Models;
using SocialMedias;
using System.Net;
using TelefoonNummers;
using VerenigingWerdVerwijderd = Events.VerenigingWerdVerwijderd;

public class Vereniging : VerenigingsBase, IHydrate<VerenigingState>
{
    public static Vereniging RegistreerFeitelijkeVereniging(
        VCode vCode,
        VerenigingsNaam naam,
        string? korteNaam,
        string? korteBeschrijving,
        Datum? startDatum,
        Doelgroep doelgroep,
        bool uitgeschrevenUitPubliekeDatastroom,
        Contactgegeven[] toeTeVoegenContactgegevens,
        Locatie[] toeTeVoegenLocaties,
        Vertegenwoordiger[] toeTeVoegenVertegenwoordigers,
        HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloketLijst,
        IClock clock)
    {
        Throw<StartdatumMagNietInToekomstZijn>.If(startDatum?.IsInFutureOf(clock.Today) ?? false);

        var toegevoegdeLocaties = Locaties.Empty.VoegToe(toeTeVoegenLocaties);
        var toegevoegdeContactgegevens = Contactgegevens.Empty.VoegToe(toeTeVoegenContactgegevens);
        var toegevoegdeVertegenwoordigers = Vertegenwoordigers.Empty.VoegToe(toeTeVoegenVertegenwoordigers);

        var vereniging = new Vereniging();

        vereniging.AddEvent(
            new FeitelijkeVerenigingWerdGeregistreerd(
                vCode,
                naam,
                korteNaam ?? string.Empty,
                korteBeschrijving ?? string.Empty,
                startDatum?.Value,
                Registratiedata.Doelgroep.With(doelgroep),
                uitgeschrevenUitPubliekeDatastroom,
                ToEventContactgegevens(toegevoegdeContactgegevens),
                ToLocatieLijst(toegevoegdeLocaties),
                ToVertegenwoordigersLijst(toegevoegdeVertegenwoordigers),
                ToHoofdactiviteitenLijst(HoofdactiviteitenVerenigingsloket.FromArray(hoofdactiviteitenVerenigingsloketLijst).ToArray())));

        return vereniging;
    }

    private static Registratiedata.Contactgegeven[] ToEventContactgegevens(Contactgegeven[] contactgegevens)
        => contactgegevens.Select(Registratiedata.Contactgegeven.With).ToArray();

    private static Registratiedata.HoofdactiviteitVerenigingsloket[] ToHoofdactiviteitenLijst(
        HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloketLijst)
        => hoofdactiviteitenVerenigingsloketLijst.Select(Registratiedata.HoofdactiviteitVerenigingsloket.With).ToArray();

    private static Registratiedata.Vertegenwoordiger[] ToVertegenwoordigersLijst(Vertegenwoordiger[] vertegenwoordigersLijst)
        => vertegenwoordigersLijst.Select(Registratiedata.Vertegenwoordiger.With).ToArray();

    private static Registratiedata.Locatie[] ToLocatieLijst(Locatie[] locatieLijst)
        => locatieLijst.Select(Registratiedata.Locatie.With).ToArray();

    public void WijzigNaam(VerenigingsNaam naam)
    {
        if (naam.Equals(State.Naam))
            return;

        AddEvent(new NaamWerdGewijzigd(VCode, naam));
    }

    public void WijzigKorteNaam(string korteNaam)
    {
        if (korteNaam.Equals(State.KorteNaam))
            return;

        AddEvent(new KorteNaamWerdGewijzigd(VCode, korteNaam));
    }

    public void WijzigKorteBeschrijving(string korteBeschrijving)
    {
        if (korteBeschrijving.Equals(State.KorteBeschrijving))
            return;

        AddEvent(new KorteBeschrijvingWerdGewijzigd(VCode, korteBeschrijving));
    }

    public void WijzigStartdatum(Datum? startDatum, IClock clock)
    {
        if (Datum.Equals(State.Startdatum, startDatum))
            return;

        Throw<StartdatumLigtNaEinddatum>.If(State.Einddatum?.IsInPastOf(startDatum) ?? false);
        Throw<StartdatumMagNietInToekomstZijn>.If(startDatum?.IsInFutureOf(clock.Today) ?? false);

        AddEvent(StartdatumWerdGewijzigd.With(State.VCode, startDatum));
    }

    public void Stop(Datum einddatum, IClock clock)
    {
        if (einddatum == State.Einddatum) return;

        Throw<EinddatumMagNietInToekomstZijn>.If(einddatum.IsInFutureOf(clock.Today));
        Throw<EinddatumLigtVoorStartdatum>.If(einddatum.IsInPastOf(State.Startdatum));

        if (State.IsGestopt)
            AddEvent(EinddatumWerdGewijzigd.With(einddatum));
        else
            AddEvent(VerenigingWerdGestopt.With(einddatum));
    }

    public void Verwijder(string reden)
    {
        Throw<VerenigingKanNietVerwijderdWorden>.If(State.IsVerwijderd);
        AddEvent(VerenigingWerdVerwijderd.With(VCode, reden));
    }

    public void WijzigDoelgroep(Doelgroep doelgroep)
    {
        if (Doelgroep.Equals(State.Doelgroep, doelgroep)) return;

        AddEvent(DoelgroepWerdGewijzigd.With(doelgroep));
    }

    public void WijzigHoofdactiviteitenVerenigingsloket(HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloket)
    {
        if (HoofdactiviteitenVerenigingsloket.Equals(hoofdactiviteitenVerenigingsloket, State.HoofdactiviteitenVerenigingsloket))
            return;

        Throw<LaatsteHoofdActiviteitKanNietVerwijderdWorden>.If(State.HoofdactiviteitenVerenigingsloket.Count == 1 && hoofdactiviteitenVerenigingsloket.Length == 0);

        var hoofdactiviteiten = HoofdactiviteitenVerenigingsloket.FromArray(hoofdactiviteitenVerenigingsloket);
        AddEvent(HoofdactiviteitenVerenigingsloketWerdenGewijzigd.With(hoofdactiviteiten.ToArray()));
    }

    public void VoegVertegenwoordigerToe(Vertegenwoordiger vertegenwoordiger)
    {
        var toegevoegdeVertegenwoordiger = State.Vertegenwoordigers.VoegToe(vertegenwoordiger);

        AddEvent(VertegenwoordigerWerdToegevoegd.With(toegevoegdeVertegenwoordiger));
    }

    public void WijzigVertegenwoordiger(
        int vertegenwoordigerId,
        string? rol,
        string? roepnaam,
        Email? email,
        TelefoonNummer? telefoonNummer,
        TelefoonNummer? mobiel,
        SocialMedia? socialMedia,
        bool? isPrimair)
    {
        var gewijzigdeVertegenwoordiger =
            State.Vertegenwoordigers.Wijzig(vertegenwoordigerId, rol, roepnaam, email, telefoonNummer, mobiel, socialMedia, isPrimair);

        if (gewijzigdeVertegenwoordiger is null)
            return;

        AddEvent(VertegenwoordigerWerdGewijzigd.With(gewijzigdeVertegenwoordiger));
    }

    public void VerwijderVertegenwoordiger(int vertegenwoordigerId)
    {
        var vertegenwoordiger = State.Vertegenwoordigers.Verwijder(vertegenwoordigerId);
        AddEvent(VertegenwoordigerWerdVerwijderd.With(vertegenwoordiger));
    }

    public void SchrijfUitUitPubliekeDatastroom()
    {
        if (State.IsUitgeschrevenUitPubliekeDatastroom) return;
        AddEvent(new VerenigingWerdUitgeschrevenUitPubliekeDatastroom());
    }

    public void SchrijfInInPubliekeDatastroom()
    {
        if (!State.IsUitgeschrevenUitPubliekeDatastroom) return;
        AddEvent(new VerenigingWerdIngeschrevenInPubliekeDatastroom());
    }

    public void Hydrate(VerenigingState obj)
    {
        Throw<ActieIsNietToegestaanVoorVerenigingstype>.If(obj.Verenigingstype != Verenigingstype.FeitelijkeVereniging);
        State = obj;
    }

    public async Task NeemAdresDetailOver(int locatieId, Registratiedata.AdresId adresId, IGrarClient grarClient, CancellationToken cancellationToken)
    {
        var adresDetailResponse = await grarClient.GetAddressById(adresId.ToString(), cancellationToken);

        if (!adresDetailResponse.IsActief)
            throw new AdressenregisterReturnedInactiefAdres();

        var postalInformation = await grarClient.GetPostalInformation(adresDetailResponse.Postcode);

        AddEvent(new AdresWerdOvergenomenUitAdressenregister(VCode, locatieId,
                                                             adresDetailResponse.AdresId,
                                                             Registratiedata.AdresUitAdressenregister.With(AdresDetailUitAdressenregister
                                                                .FromResponse(adresDetailResponse)
                                                                .DecorateWithPostalInformation(
                                                                     adresDetailResponse.Gemeente, postalInformation))));
    }
}
