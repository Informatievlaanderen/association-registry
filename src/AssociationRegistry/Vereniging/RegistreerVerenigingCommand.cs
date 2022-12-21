namespace AssociationRegistry.Vereniging;

using System;
using MediatR;

public record RegistreerVerenigingCommand(
    string Naam,
    string? KorteNaam = null,
    string? KorteBeschrijving = null,
    DateOnly? Startdatum = null,
    string? KboNummber = null,
    IEnumerable<RegistreerVerenigingCommand.ContactInfo>? Contacten = null) : IRequest<Unit>
{
    public record ContactInfo(
        string? Contactnaam = null,
        string? Email = null,
        string? Telefoon = null,
        string? Website = null,
        string? SocialMedia = null);
}
