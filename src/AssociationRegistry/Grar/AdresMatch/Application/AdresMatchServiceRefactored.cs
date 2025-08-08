namespace AssociationRegistry.Grar.AdresMatch.Application;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.Events;
using AssociationRegistry.Events.Factories;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Vereniging;
using Domain;

public class AdresMatchServiceRefactored : IAdresMatchService
{
    private readonly IGrarClient _grarClient;
    private readonly IAdresMatchStrategy _matchStrategy;
    private readonly IAdresVerrijkingService _verrijkingService;

    public AdresMatchServiceRefactored(
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
        var request = new AdresMatchRequest(vCode, locatieId, locatie);
        var result = await ProcessAdresMatch(request, cancellationToken);
        return result.ToEvent(vCode, locatieId);
    }

    private async Task<AdresMatchResult> ProcessAdresMatch(
        AdresMatchRequest request,
        CancellationToken cancellationToken)
    {
        if (request.HeeftGeenLocatie)
            return new LocatieVerwijderdResult();

        var adresMatches = await FetchAdresMatches(request.Adres, cancellationToken);
        
        if (adresMatches.HasNoResponse)
            return new AdresNietGevondenResult(request.Locatie!);

        var matchedAdres = _matchStrategy.DetermineMatch(adresMatches);
        
        if (matchedAdres is null)
            return CreateNietUniekResult(request, adresMatches);

        return await CreateGevondenResult(request, matchedAdres, cancellationToken);
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

    private AdresNietUniekResult CreateNietUniekResult(
        AdresMatchRequest request,
        AdresMatchResponseCollection adresMatches)
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