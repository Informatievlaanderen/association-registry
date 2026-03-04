update public.mt_event_progression
set name = 'beheer.postgres.detail:All'
where name = 'AssociationRegistry.Admin.ProjectionHost.Projections.Detail.BeheerVerenigingDetailProjection:All';

update public.mt_event_progression
set name = 'beheer.postgres.historiek:All'
where name = 'AssociationRegistry.Admin.ProjectionHost.Projections.Historiek.BeheerVerenigingHistoriekProjection:All';

update public.mt_event_progression
set name = 'beheer.elastic.zoeken:All'
where name = 'BeheerVerenigingZoekenDocument:All';

update public.mt_event_progression
set name = 'beheer.postgres.kbo.synchistoriek:All'
where name = 'AssociationRegistry.Admin.ProjectionHost.Projections.Sync.BeheerKboSyncHistoriekProjection:All';

update public.mt_event_progression
set name = 'beheer.postgres.ksz.synchistoriek:All'
where name = 'AssociationRegistry.Admin.ProjectionHost.Projections.Sync.BeheerKszSyncHistoriekProjection:All';

update public.mt_event_progression
set name = 'beheer.postgres.vertegenwoordigerspervcode:All'
where name = 'VertegenwoordigersPerVCodeDocument:All';

update public.mt_event_progression
set name = 'beheer.postgres.bewaartermijn:All'
where name = 'BewaartermijnDocument:All';

update public.mt_event_progression
set name = 'beheer.postgres.locatiesgekoppeldmetgrar:All'
where name = 'LocatieLookupDocument:All';

update public.mt_event_progression
set name = 'beheer.postgres.locatiezonderadresmatch:All'
where name = 'LocatieZonderAdresMatchDocument:All';

update public.mt_event_progression
set name = 'beheer.elastic.duplicatedetection:All'
where name = 'DuplicateDetection:All';

update public.mt_event_progression
set name = 'beheer.postgres.powerbi.export:All'
where name = 'PowerBiExportDocument:All';

update public.mt_event_progression
set name = 'beheer.postgres.powerbi.dubbeldetectie:All'
where name = 'PowerBiExportDubbelDetectieDocument:All';
