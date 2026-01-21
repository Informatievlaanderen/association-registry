namespace AssociationRegistry.Persoonsgegevens;

using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Bankrekeningen;

public record BankrekeningnummerPersoonsgegevens : IPersoonsgegevens
{
    public Guid RefId { get; init; }
    public VCode VCode { get; init; }
    public int BankrekeningnummerId { get; init; }
    public string? Iban { get; init; }
    public string? Titularis { get; init; }

    public BankrekeningnummerPersoonsgegevens(
        Guid refId,
        VCode VCode,
        int bankrekeningnummerId,
        string? iban,
        string? titularis)
    {
        RefId = refId;
        this.VCode = VCode;
        BankrekeningnummerId = bankrekeningnummerId;
        Iban = iban;
        Titularis = titularis;
    }

    public static BankrekeningnummerPersoonsgegevens ToBankrekeningnummerPersoonsgegevens(Guid refId, VCode vCode, Bankrekeningnummer bankrekeningnummer)
        => new(refId,
               vCode,
               bankrekeningnummer.BankrekeningnummerId,
               bankrekeningnummer.Iban.Value,
               bankrekeningnummer.Titularis);
}
