﻿namespace AssociationRegistry.Vereniging;

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
                "VZW",
                $"VZW {verenigingVolgensKbo.KboNummer}",
                string.Empty,
                null));
        return vereniging;
    }

    public void Hydrate(VerenigingState obj)
    {
        Throw<UnsupportedOperationForVerenigingstype>.If(obj.Verenigingstype != Verenigingstype.VerenigingMetRechtspersoonlijkheid);
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
}
