<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Detail/Projector/Given_KorteNaamWerdGewijzigdInKbo.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Detail.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Detail.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Detail/Projecting/Given_KorteNaamWerdGewijzigdInKbo.cs

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_KorteNaamWerdGewijzigdInKbo
{
    [Fact]
    public void Then_it_modifies_the_korteNaam()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var korteNaamWerdGewijzigdInKbo = fixture.Create<TestEvent<KorteNaamWerdGewijzigdInKbo>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(korteNaamWerdGewijzigdInKbo, doc);

        doc.KorteNaam.Should().Be(korteNaamWerdGewijzigdInKbo.Data.KorteNaam);
    }
}
