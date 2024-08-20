<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Detail/Projector/Given_ContactgegevenWerdGewijzigd.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Detail.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Detail.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Detail/Projecting/Given_ContactgegevenWerdGewijzigd.cs

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_contactgegeven()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenWerdGewijzigd = fixture.Create<TestEvent<ContactgegevenWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        doc.Contactgegevens = new[]
        {
            new Contactgegeven
            {
                ContactgegevenId = contactgegevenWerdGewijzigd.Data.ContactgegevenId,
                Contactgegeventype = contactgegevenWerdGewijzigd.Data.Contactgegeventype,
                Waarde = fixture.Create<string>(),
                Beschrijving = fixture.Create<string>(),
                IsPrimair = true,
            },
        };

        BeheerVerenigingDetailProjector.Apply(contactgegevenWerdGewijzigd, doc);

        doc.Contactgegevens.Should().BeEquivalentTo(
            new[]
            {
                new Contactgegeven
                {
                    ContactgegevenId = contactgegevenWerdGewijzigd.Data.ContactgegevenId,
                    Contactgegeventype = contactgegevenWerdGewijzigd.Data.Contactgegeventype,
                    Waarde = contactgegevenWerdGewijzigd.Data.Waarde,
                    Beschrijving = contactgegevenWerdGewijzigd.Data.Beschrijving,
                    IsPrimair = contactgegevenWerdGewijzigd.Data.IsPrimair,
                },
            });

        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
    }
}
