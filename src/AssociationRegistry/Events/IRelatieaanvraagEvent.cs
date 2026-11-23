namespace AssociationRegistry.Events;

/// <summary>
///     marker interface: do not add logic.
///     The events of a relatieaanvraag live in their own stream (aanvraagId), not in the stream of a vereniging,
///     so the projections of a vereniging do not handle them.
/// </summary>
public interface IRelatieaanvraagEvent : IEvent { }
