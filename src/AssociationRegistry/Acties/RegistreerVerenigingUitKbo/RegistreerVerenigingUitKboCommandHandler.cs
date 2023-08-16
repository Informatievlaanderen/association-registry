namespace AssociationRegistry.Acties.RegistreerVerenigingUitKbo;

using DuplicateVerenigingDetection;
using Framework;
using Kbo;
using Vereniging;
using ResultNet;
using Vereniging.Exceptions;

public class RegistreerVerenigingUitKboCommandHandler
{
    private readonly IVCodeService _vCodeService;
    private readonly IMagdaGeefVerenigingService _magdaGeefVerenigingService;
    private readonly IVerenigingsRepository _verenigingsRepository;

    public RegistreerVerenigingUitKboCommandHandler(
        IVerenigingsRepository verenigingsRepository,
        IVCodeService vCodeService,
        IMagdaGeefVerenigingService magdaGeefVerenigingService)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
        _magdaGeefVerenigingService = magdaGeefVerenigingService;
    }

    public async Task<Result> Handle(CommandEnvelope<RegistreerVerenigingUitKboCommand> message, CancellationToken cancellationToken = default)
    {
        var command = message.Command;

        var duplicateResult = await CheckForDuplicate(command.KboNummer);
        if (duplicateResult.IsFailure()) return duplicateResult;

        var vereniging = await _magdaGeefVerenigingService.GeefVereniging(command.KboNummer, message.Metadata, cancellationToken);
        if (vereniging.IsFailure()) throw new GeenGeldigeVerenigingInKbo();

        return Result.Success(await RegisteerVereniging(vereniging, message.Metadata, cancellationToken));
    }

    private async Task<Result> CheckForDuplicate(KboNummer kboNummer)
    {
        var duplicateKbo = await _verenigingsRepository.GetVCodeAndNaam(kboNummer);
        return duplicateKbo is not null ? DuplicateKboFound.WithVcode(duplicateKbo.VCode!) : Result.Success();
    }

    private async Task<CommandResult> RegisteerVereniging(VerenigingVolgensKbo verenigingVolgensKbo, CommandMetadata messageMetadata, CancellationToken cancellationToken)
    {
        var vCode = await _vCodeService.GetNext();

        var vereniging = VerenigingMetRechtspersoonlijkheid.Registreer(
            vCode,
            verenigingVolgensKbo);

        if (verenigingVolgensKbo.Adres is not null)
            AddAdressAlsMaatschappelijkeZetel(vereniging, verenigingVolgensKbo.Adres);

        if (verenigingVolgensKbo.Contactgegevens is not null)
        {
            AddContactgegeven(vereniging, verenigingVolgensKbo.Contactgegevens.Email, ContactgegevenTypeVolgensKbo.Email);
            AddContactgegeven(vereniging, verenigingVolgensKbo.Contactgegevens.Website, ContactgegevenTypeVolgensKbo.Website);
            AddContactgegeven(vereniging, verenigingVolgensKbo.Contactgegevens.Telefoonnummer, ContactgegevenTypeVolgensKbo.Telefoon);
            AddContactgegeven(vereniging, verenigingVolgensKbo.Contactgegevens.GSM, ContactgegevenTypeVolgensKbo.GSM);
        }


        var result = await _verenigingsRepository.Save(vereniging, messageMetadata, cancellationToken);
        return CommandResult.Create(vCode, result);
    }

    private static void AddContactgegeven(VerenigingMetRechtspersoonlijkheid vereniging, string? waarde, ContactgegevenTypeVolgensKbo type)
    {
        if (waarde is null) return;

        try
        {
            var contactgeven = Contactgegeven.Create(type.ContactgegevenType, waarde);
            vereniging.VoegContactgegevenToe(contactgeven, type);
        }
        catch
        {
            vereniging.VoegFoutiefContactgegevenToe(type, waarde);
        }
    }

    private static void AddAdressAlsMaatschappelijkeZetel(VerenigingMetRechtspersoonlijkheid vereniging, AdresVolgensKbo adresVolgensKbo)
    {
        try
        {
            var adres = Adres.Create(adresVolgensKbo.Straatnaam, adresVolgensKbo.Huisnummer, adresVolgensKbo.Busnummer, adresVolgensKbo.Postcode, adresVolgensKbo.Gemeente, adresVolgensKbo.Land);
            vereniging.AddMaatschappelijkeZetel(adres);
        }
        catch
        {
            // ignored
            //TODO in OR-1864
        }
    }
}
