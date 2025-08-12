namespace AssociationRegistry.Grar.AdresMatch;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.Events;
using AssociationRegistry.Events.Factories;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Models;

public class AdresMatchService : IAdresMatchService
{
    private readonly IGrarClient _grarClient;
    private readonly IAdresMatchStrategy _matchStrategy;
    private readonly IAdresVerrijkingService _verrijkingService;

    public AdresMatchService(
        IGrarClient grarClient,
        IAdresMatchStrategy matchStrategy,
        IAdresVerrijkingService verrijkingService)
    {
        _grarClient = grarClient;
        _matchStrategy = matchStrategy;
        _verrijkingService = verrijkingService;
    }

    public async Task<IEvent> GetAdresMatchEvent(
        int locatieId,
        Locatie? locatie,
        VCode vCode,
        CancellationToken cancellationToken)
    {
        var request = new AdresMatchRequest(locatie);
        var result = await ProcessAdresMatch(request, cancellationToken);
        return result.ToEvent(vCode, locatieId);
    }

    public async Task<AdresMatchResult> ProcessAdresMatch(
        AdresMatchRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request.HeeftGeenLocatie)
                return new LocatieVerwijderdResult();

            var adresMatches = await FetchAdresMatches(request.Adres, cancellationToken);

            if (adresMatches.HasNoResponse)
                return new AdresNietGevondenResult(request.Locatie!);

            var matchedAdres = _matchStrategy.DetermineMatch(adresMatches);

            if (matchedAdres is null)
                return CreateNietUniekResult(adresMatches);

            return await CreateGevondenResult(request, matchedAdres, cancellationToken);
        }
        catch (Exceptions.AdressenregisterReturnedNonSuccessStatusCode ex)
        {
            return new AdresKonNietOvergenomenResult(request.Locatie?.Adres.ToAdresString() ?? "", ex.Message);
        }
        catch (Exceptions.AdressenregisterReturnedNotFoundStatusCode ex)
        {
            return new AdresNietGevondenResult(request.Locatie!);
        }
    }

    private async Task<AdresMatchResponseCollection> FetchAdresMatches(
        Adres adres,
        CancellationToken cancellationToken)
    {
        return await _grarClient.GetAddressMatches(
            adres.Straatnaam,
            adres.Huisnummer,
            adres.Busnummer,
            adres.Postcode,
            adres.Gemeente.Naam,
            cancellationToken);
    }

    private AdresNietUniekResult CreateNietUniekResult(AdresMatchResponseCollection adresMatches)
    {
        var matches = adresMatches
            .Select(EventFactory.NietUniekeAdresMatchUitAdressenregister)
            .ToArray();

        return new AdresNietUniekResult(matches);
    }

    private async Task<AdresGevondenResult> CreateGevondenResult(
        AdresMatchRequest request,
        AddressMatchResponse matchedAdres,
        CancellationToken cancellationToken)
    {
        var verrijktAdres = await _verrijkingService.VerrijkAdres(
            matchedAdres,
            request.Adres,
            cancellationToken);

        return new AdresGevondenResult(
            matchedAdres.AdresId!,
            verrijktAdres);
    }
}
