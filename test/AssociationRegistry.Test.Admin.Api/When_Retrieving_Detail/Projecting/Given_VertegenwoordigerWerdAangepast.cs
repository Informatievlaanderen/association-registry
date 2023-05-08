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
public class Given_VertegenwoordigerWerdGewijzigd
{
    [Fact]
    public void Then_it_updates_the_vertegenwoordiger_in_the_detail()
    {
        var fixture = new Fixture().CustomizeAll();
        var vertegenwoordigerWerdGewijzigd = fixture.Create<VertegenwoordigerWerdGewijzigd>();
        var vertegenwoordigerToUpdate = fixture.Create<BeheerVerenigingDetailDocument.Vertegenwoordiger>() with
        {
            VertegenwoordigerId = vertegenwoordigerWerdGewijzigd.VertegenwoordigerId,
        };

        var detailDocument = When<VertegenwoordigerWerdGewijzigd>
            .Applying(_ => vertegenwoordigerWerdGewijzigd)
            .ToDetailProjectie(
                doc => doc with
                {
                    Vertegenwoordigers = doc.Vertegenwoordigers.Append(vertegenwoordigerToUpdate),
                });

        detailDocument.Vertegenwoordigers.Should().Contain(
            new BeheerVerenigingDetailDocument.Vertegenwoordiger
            {
                VertegenwoordigerId = vertegenwoordigerWerdGewijzigd.VertegenwoordigerId,
                Insz = vertegenwoordigerToUpdate.Insz,
                Achternaam = vertegenwoordigerToUpdate.Achternaam,
                Voornaam = vertegenwoordigerToUpdate.Voornaam,
                Roepnaam = vertegenwoordigerWerdGewijzigd.Roepnaam,
                Rol = vertegenwoordigerWerdGewijzigd.Rol,
                IsPrimair = vertegenwoordigerWerdGewijzigd.IsPrimair,
                Email = vertegenwoordigerWerdGewijzigd.Email!,
                Telefoon = vertegenwoordigerWerdGewijzigd.Telefoon!,
                Mobiel = vertegenwoordigerWerdGewijzigd.Mobiel!,
                SocialMedia = vertegenwoordigerWerdGewijzigd.SocialMedia!,
            });
    }
}
