namespace AssociationRegistry.Vereniging;

using Events;
using Exceptions;
using Framework;
using Kbo;

public class VerenigingMetRechtspersoonlijkheid : VerenigingsBase, IHydrate<VerenigingState>
{
    public static VerenigingMetRechtspersoonlijkheid Registreer(VCode vCode, VerenigingVolgensKbo verenigingVolgensKbo)
    {
        var vereniging = new VerenigingMetRechtspersoonlijkheid();
        vereniging.AddEvent(
            new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
                vCode,
                verenigingVolgensKbo.KboNummer,
                verenigingVolgensKbo.Type.Code,
                verenigingVolgensKbo.Naam ?? "",
                verenigingVolgensKbo.KorteNaam ?? "",
                verenigingVolgensKbo.StartDatum ?? null));
        return vereniging;
    }

    public void Hydrate(VerenigingState obj)
    {
        Throw<UnsupportedOperationForVerenigingstype>.If(obj.Verenigingstype != Verenigingstype.VZW);
        State = obj;
    }

    public void WijzigKorteBeschrijving(string korteBeschrijving)
    {
        if (korteBeschrijving.Equals(State.KorteBeschrijving))
            return;

        AddEvent(new KorteBeschrijvingWerdGewijzigd(VCode, korteBeschrijving));
    }

    public void WijzigHoofdactiviteitenVerenigingsloket(HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloket)
    {
        if (HoofdactiviteitenVerenigingsloket.Equals(hoofdactiviteitenVerenigingsloket, State.HoofdactiviteitenVerenigingsloket))
            return;

        var hoofdactiviteiten = HoofdactiviteitenVerenigingsloket.FromArray(hoofdactiviteitenVerenigingsloket);
        AddEvent(HoofdactiviteitenVerenigingsloketWerdenGewijzigd.With(hoofdactiviteiten.ToArray()));
    }

    public void AddMaatschappelijkeZetel(Adres adres)
    {
        AddEvent(
            MaatschappelijkeZetelWerdOvergenomenUitKbo.With(
                Locatie.Create(
                        string.Empty,
                        false,
                        Locatietype.MaatschappelijkeZetelVolgensKbo,
                        null,
                        adres)
                    with
                    {
                        LocatieId = State.Locaties.NextId,
                    }
            )
        );
    }

    public void VoegContactgegevenToe(Contactgegeven contactgegeven, ContactgegevenTypeVolgensKbo typeVolgensKbo)
    {
        var toegevoegdContactgegeven = State.Contactgegevens.VoegToe(contactgegeven);

        AddEvent(ContactgegevenWerdOvergenomenUitKBO.With(toegevoegdContactgegeven, typeVolgensKbo));
    }

    public void VoegFoutiefContactgegevenToe(ContactgegevenTypeVolgensKbo type, string waarde)
    {
        AddEvent(new ContactgegevenKonNietOvergenomenWordenUitKBO(type.ContactgegevenType.Waarde, type.Waarde, waarde));
    }

    public void VoegFoutieveMaatscheppelijkeZetelToe(AdresVolgensKbo adres)
    {
        AddEvent(
            new MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo(
                adres.Straatnaam ?? string.Empty,
                adres.Huisnummer ?? string.Empty,
                adres.Busnummer ?? string.Empty,
                adres.Postcode ?? string.Empty,
                adres.Gemeente ?? string.Empty,
                adres.Land ?? string.Empty));
    }
}
