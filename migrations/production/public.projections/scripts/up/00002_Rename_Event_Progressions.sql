update public.mt_event_progression
set name = 'publiek.elastic.zoeken:All'
where name = 'PubliekVerenigingZoekenDocument:All';

update public.mt_event_progression
set name = 'publiek.postgres.sequence:All'
where name = 'PubliekVerenigingSequenceDocument:All';

update public.mt_event_progression
set name = 'publiek.postgres.detail:All'
where name = 'AssociationRegistry.Public.ProjectionHost.Projections.Detail.PubliekVerenigingDetailProjection:All';
