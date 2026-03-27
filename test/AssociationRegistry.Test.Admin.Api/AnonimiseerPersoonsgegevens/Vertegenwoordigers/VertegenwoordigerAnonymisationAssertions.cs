namespace AssociationRegistry.Test.Admin.Api.AnonimiseerPersoonsgegevens.Vertegenwoordigers;

using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AssociationRegistry.Persoonsgegevens;
using FluentAssertions;

public static class VertegenwoordigerAnonymisationAssertions
{
    public static void ShouldBeAnonymised(this VertegenwoordigerData vertegenwoordiger, int vertegenwoordigerId) =>
        vertegenwoordiger
            .Should()
            .BeEquivalentTo(
                new VertegenwoordigerData(
                    VertegenwoordigerId: vertegenwoordigerId,
                    IsPrimair: false, // set to false in setup
                    Achternaam: WellKnownAnonymousFields.Geanonimiseerd,
                    Voornaam: WellKnownAnonymousFields.Geanonimiseerd,
                    Email: WellKnownAnonymousFields.Geanonimiseerd,
                    Mobiel: WellKnownAnonymousFields.Geanonimiseerd,
                    Roepnaam: WellKnownAnonymousFields.Geanonimiseerd,
                    Rol: WellKnownAnonymousFields.Geanonimiseerd,
                    SocialMedia: WellKnownAnonymousFields.Geanonimiseerd,
                    Telefoon: WellKnownAnonymousFields.Geanonimiseerd
                )
            );

    public static void ShouldNotBeAnonymised(this VertegenwoordigerData vertegenwoordiger)
    {
        vertegenwoordiger.Achternaam.Should().NotBe(WellKnownAnonymousFields.Geanonimiseerd);
        vertegenwoordiger.Voornaam.Should().NotBe(WellKnownAnonymousFields.Geanonimiseerd);
        vertegenwoordiger.Email.Should().NotBe(WellKnownAnonymousFields.Geanonimiseerd);
        vertegenwoordiger.Mobiel.Should().NotBe(WellKnownAnonymousFields.Geanonimiseerd);
        vertegenwoordiger.Roepnaam.Should().NotBe(WellKnownAnonymousFields.Geanonimiseerd);
        vertegenwoordiger.Rol.Should().NotBe(WellKnownAnonymousFields.Geanonimiseerd);
        vertegenwoordiger.SocialMedia.Should().NotBe(WellKnownAnonymousFields.Geanonimiseerd);
        vertegenwoordiger.Telefoon.Should().NotBe(WellKnownAnonymousFields.Geanonimiseerd);
    }

    public static void ShouldBeAnonymised(
        this VertegenwoordigerWerdVerwijderdData vertegenwoordiger,
        int vertegenwoordigerId
    ) =>
        vertegenwoordiger
            .Should()
            .BeEquivalentTo(
                new VertegenwoordigerWerdVerwijderdData(
                    VertegenwoordigerId: vertegenwoordigerId,
                    Achternaam: WellKnownAnonymousFields.Geanonimiseerd,
                    Voornaam: WellKnownAnonymousFields.Geanonimiseerd
                )
            );

    public static void ShouldNotBeAnonymised(this VertegenwoordigerWerdVerwijderdData vertegenwoordiger)
    {
        vertegenwoordiger.Achternaam.Should().NotBe(WellKnownAnonymousFields.Geanonimiseerd);
        vertegenwoordiger.Voornaam.Should().NotBe(WellKnownAnonymousFields.Geanonimiseerd);
    }

    public static void ShouldBeAnonymised(this KBOVertegenwoordigerData vertegenwoordiger, int vertegenwoordigerId) =>
        vertegenwoordiger
            .Should()
            .BeEquivalentTo(
                new KBOVertegenwoordigerData(
                    VertegenwoordigerId: vertegenwoordigerId,
                    Achternaam: WellKnownAnonymousFields.Geanonimiseerd,
                    Voornaam: WellKnownAnonymousFields.Geanonimiseerd
                )
            );

    public static void ShouldNotBeAnonymised(this KBOVertegenwoordigerData vertegenwoordiger)
    {
        vertegenwoordiger.Achternaam.Should().NotBe(WellKnownAnonymousFields.Geanonimiseerd);
        vertegenwoordiger.Voornaam.Should().NotBe(WellKnownAnonymousFields.Geanonimiseerd);
    }
}
