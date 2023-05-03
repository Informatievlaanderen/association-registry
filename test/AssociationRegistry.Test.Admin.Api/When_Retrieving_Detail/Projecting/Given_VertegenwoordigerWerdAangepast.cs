namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Projections.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using JasperFx.Core;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VertegenwoordigerWerdAangepast
{
    [Fact]
    public void Then_it_updates_the_vertegenwoordiger_in_the_detail()
    {
        var fixture = new Fixture().CustomizeAll();
        var vertegenwoordigerWerdAangepast = fixture.Create<VertegenwoordigerWerdAangepast>();
        var vertegenwoordigerToUpdate = fixture.Create<BeheerVerenigingDetailDocument.Vertegenwoordiger>() with
        {
            VertegenwoordigerId = vertegenwoordigerWerdAangepast.VertegenwoordigerId,
        };

        var detailDocument = When<VertegenwoordigerWerdAangepast>
            .Applying(_ => vertegenwoordigerWerdAangepast)
            .ToDetailProjectie(
                doc => doc with
                {
                    Vertegenwoordigers = doc.Vertegenwoordigers.Append(vertegenwoordigerToUpdate),
                });

        detailDocument.Vertegenwoordigers.Should().Contain(
            new BeheerVerenigingDetailDocument.Vertegenwoordiger
            {
                VertegenwoordigerId = vertegenwoordigerWerdAangepast.VertegenwoordigerId,
                Insz = vertegenwoordigerToUpdate.Insz,
                Achternaam = vertegenwoordigerToUpdate.Achternaam,
                Voornaam = vertegenwoordigerToUpdate.Voornaam,
                Roepnaam = vertegenwoordigerWerdAangepast.Roepnaam,
                Rol = vertegenwoordigerWerdAangepast.Rol,
                IsPrimair = vertegenwoordigerWerdAangepast.IsPrimair!.Value,
                Email = vertegenwoordigerWerdAangepast.Email!,
                Telefoon = vertegenwoordigerWerdAangepast.Telefoon!,
                Mobiel = vertegenwoordigerWerdAangepast.Mobiel!,
                SocialMedia = vertegenwoordigerWerdAangepast.SocialMedia!,
            });
    }
}
