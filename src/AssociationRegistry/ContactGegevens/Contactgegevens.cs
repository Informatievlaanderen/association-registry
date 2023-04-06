﻿namespace AssociationRegistry.ContactGegevens;

using System.Collections.ObjectModel;
using Exceptions;
using Framework;

public class Contactgegevens : ReadOnlyCollection<Contactgegeven>
{
    private const int InitialId = 1;
    public int NextId { get; }

    public static Contactgegevens Empty
        => new(Enumerable.Empty<Contactgegeven>().ToList(), InitialId);

    public static Contactgegevens FromArray(Contactgegeven[] contactgegevenArray)
    {
        var contactgegevens = Empty;
        foreach (var contactgegeven in contactgegevenArray)
        {
            Throw<DuplicateContactgegeven>.If(contactgegevens.ContainsMetZelfdeWaarden(contactgegeven), contactgegeven.Type);
            Throw<MultiplePrimaryContactgegevens>.If(contactgegeven.IsPrimair && contactgegevens.HasPrimairForType(contactgegeven.Type), contactgegeven.Type);
            contactgegevens = contactgegevens.Append(contactgegeven with { ContactgegevenId = contactgegevens.NextId });
        }

        return contactgegevens;
    }

    private Contactgegevens(IList<Contactgegeven> list, int nextId) : base(list)
    {
        NextId = nextId;
    }

    public Contactgegevens Append(Contactgegeven contactgegeven)
    {
        var nextId = Math.Max(contactgegeven.ContactgegevenId, NextId) + 1;
        return new(Items.Append(contactgegeven).ToList(), nextId);
    }

    public Contactgegevens Remove(int contectgegevenId)
        => new(Items.Where(c => c.ContactgegevenId != contectgegevenId).ToList(), NextId);

    public Contactgegevens Replace(Contactgegeven contactgegeven)
        => Remove(contactgegeven.ContactgegevenId).Append(contactgegeven);

    public bool ContainsMetZelfdeWaarden(Contactgegeven contactgegeven)
        => this.Any(contactgegeven.MetZelfdeWaarden);

    public new Contactgegeven this[int contactgegevenId]
        => this.Single(x => x.ContactgegevenId == contactgegevenId);

    public bool HasKey(int contactgegevenId)
        => this.Any(contactgegeven => contactgegeven.ContactgegevenId == contactgegevenId);

    public bool ContainsOther(Contactgegeven contactgegeven)
        => Items
            .Without(contactgegeven)
            .Any(contactgegeven.MetZelfdeWaarden);
}

public static class ContactgegevenEnumerableExtensions
{
    public static IEnumerable<Contactgegeven> Without(this IEnumerable<Contactgegeven> source, Contactgegeven contactgegeven)
    {
        return source.Where(c => c.ContactgegevenId != contactgegeven.ContactgegevenId);
    }

    public static bool HasPrimairForType(this IEnumerable<Contactgegeven> source, ContactgegevenType type)
        => source.Any(contactgegeven => contactgegeven.Type == type && contactgegeven.IsPrimair);
}
