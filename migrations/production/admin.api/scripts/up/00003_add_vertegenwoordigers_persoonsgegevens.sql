DROP TABLE IF EXISTS public.mt_doc_vertegenwoordigerpersoonsgegevensdocument CASCADE;
CREATE TABLE public.mt_doc_vertegenwoordigerpersoonsgegevensdocument (
                                                                       id                  uuid                        NOT NULL,
                                                                       data                jsonb                       NOT NULL,
                                                                       mt_last_modified    timestamp with time zone    NULL DEFAULT (transaction_timestamp()),
                                                                       mt_version          uuid                        NOT NULL DEFAULT (md5(random()::text || clock_timestamp()::text)::uuid),
                                                                       mt_dotnet_type      varchar                     NULL,
                                                                       CONSTRAINT pkey_mt_doc_vertegenwoordigerpersoonsgegevensdocument_id PRIMARY KEY (id)
);

CREATE OR REPLACE FUNCTION public.mt_upsert_vertegenwoordigerpersoonsgegevensdocument(doc JSONB, docDotNetType varchar, docId uuid, docVersion uuid) RETURNS UUID LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
final_version uuid;
BEGIN
INSERT INTO public.mt_doc_vertegenwoordigerpersoonsgegevensdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp();

SELECT mt_version FROM public.mt_doc_vertegenwoordigerpersoonsgegevensdocument into final_version WHERE id = docId ;
RETURN final_version;
END;
$function$;


CREATE OR REPLACE FUNCTION public.mt_insert_vertegenwoordigerpersoonsgegevensdocument(doc JSONB, docDotNetType varchar, docId uuid, docVersion uuid) RETURNS UUID LANGUAGE plpgsql SECURITY INVOKER AS $function$
BEGIN
INSERT INTO public.mt_doc_vertegenwoordigerpersoonsgegevensdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp());

RETURN docVersion;
END;
$function$;


CREATE OR REPLACE FUNCTION public.mt_update_vertegenwoordigerpersoonsgegevensdocument(doc JSONB, docDotNetType varchar, docId uuid, docVersion uuid) RETURNS UUID LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
final_version uuid;
BEGIN
UPDATE public.mt_doc_vertegenwoordigerpersoonsgegevensdocument SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where id = docId;

SELECT mt_version FROM public.mt_doc_vertegenwoordigerpersoonsgegevensdocument into final_version WHERE id = docId ;
RETURN final_version;
END;
$function$;


-- ==================================================================================
-- DATA SPLIT: Extract persoonsgegevens from events into separate table
-- ==================================================================================
--
-- For GDPR compliance, we split personal data from events into a separate table.
-- This allows us to manage retention periods independently from the immutable event store.
--
-- For each event containing persoonsgegevens:
--   1. Generate a new RefId (UUID)
--   2. INSERT persoonsgegevens into mt_doc_vertegenwoordigerpersoonsgegevensdocument with that RefId
--   3. UPDATE the event's JSON data to add the RefId field
--   4. RENAME the event type to include "ZonderPersoonsgegevens" suffix (done later in script)
--
-- Upcasters will later join events with persoonsgegevens using the RefId.
-- If persoonsgegevens is NULL (deleted after retention period), projections show "[VERWIJDERD]".
-- ==================================================================================

-- Step 1: VertegenwoordigerWerdGewijzigd
-- Persoonsgegevens: Voornaam, Achternaam, Roepnaam, Rol, Email, Telefoon, Mobiel, SocialMedia
-- Note: Insz is NOT present in this event type (set to NULL)

-- Create temp table to hold event_id -> ref_id mapping
CREATE TEMP TABLE temp_event_refids_gewijzigd (
    event_id uuid,
    stream_id varchar,
    data jsonb,
    ref_id uuid
);

-- Generate RefIds for each event
INSERT INTO temp_event_refids_gewijzigd (event_id, stream_id, data, ref_id)
SELECT
    e.id as event_id,
    e.stream_id,
    e.data,
    gen_random_uuid() as ref_id
FROM public.mt_events e
WHERE e.type = 'vertegenwoordiger_werd_gewijzigd'
  AND e.mt_dotnet_type = 'AssociationRegistry.Events.VertegenwoordigerWerdGewijzigd, AssociationRegistry';

-- Insert persoonsgegevens into separate table
INSERT INTO public.mt_doc_vertegenwoordigerpersoonsgegevensdocument (id, data, mt_dotnet_type, mt_version)
SELECT
    ref_id as id,
    jsonb_build_object(
        'RefId', ref_id,
        'VCode', stream_id,
        'VertegenwoordigerId', (data->>'VertegenwoordigerId')::int,
        'Insz', NULL,  -- Not present in VertegenwoordigerWerdGewijzigd
        'IsPrimair', (data->>'IsPrimair')::boolean,
        'Roepnaam', data->>'Roepnaam',
        'Rol', data->>'Rol',
        'Voornaam', data->>'Voornaam',
        'Achternaam', data->>'Achternaam',
        'Email', data->>'Email',
        'Telefoon', data->>'Telefoon',
        'Mobiel', data->>'Mobiel',
        'SocialMedia', data->>'SocialMedia'
    ) as data,
    'AssociationRegistry.Persoonsgegevens.VertegenwoordigerPersoonsgegevensDocument, AssociationRegistry.Admin.Schema' as mt_dotnet_type,
    md5(random()::text || clock_timestamp()::text)::uuid as mt_version
FROM temp_event_refids_gewijzigd;

-- Update events: Add RefId and REMOVE personal data fields
UPDATE public.mt_events e
SET data = e.data
    - 'Voornaam' - 'Achternaam' - 'Roepnaam' - 'Rol'
    - 'Email' - 'Telefoon' - 'Mobiel' - 'SocialMedia'
    || jsonb_build_object('RefId', t.ref_id)
FROM temp_event_refids_gewijzigd t
WHERE e.id = t.event_id;

DROP TABLE temp_event_refids_gewijzigd;

-- Step 2: VertegenwoordigerWerdVerwijderd
-- Persoonsgegevens: Insz, Voornaam, Achternaam

CREATE TEMP TABLE temp_event_refids_verwijderd (
    event_id uuid,
    stream_id varchar,
    data jsonb,
    ref_id uuid
);

INSERT INTO temp_event_refids_verwijderd (event_id, stream_id, data, ref_id)
SELECT
    e.id as event_id,
    e.stream_id,
    e.data,
    gen_random_uuid() as ref_id
FROM public.mt_events e
WHERE e.type = 'vertegenwoordiger_werd_verwijderd'
  AND e.mt_dotnet_type = 'AssociationRegistry.Events.VertegenwoordigerWerdVerwijderd, AssociationRegistry';

INSERT INTO public.mt_doc_vertegenwoordigerpersoonsgegevensdocument (id, data, mt_dotnet_type, mt_version)
SELECT
    ref_id as id,
    jsonb_build_object(
        'RefId', ref_id,
        'VCode', stream_id,
        'VertegenwoordigerId', (data->>'VertegenwoordigerId')::int,
        'Insz', data->>'Insz',
        'IsPrimair', NULL,
        'Roepnaam', NULL,
        'Rol', NULL,
        'Voornaam', data->>'Voornaam',
        'Achternaam', data->>'Achternaam',
        'Email', NULL,
        'Telefoon', NULL,
        'Mobiel', NULL,
        'SocialMedia', NULL
    ) as data,
    'AssociationRegistry.Persoonsgegevens.VertegenwoordigerPersoonsgegevensDocument, AssociationRegistry.Admin.Schema' as mt_dotnet_type,
    md5(random()::text || clock_timestamp()::text)::uuid as mt_version
FROM temp_event_refids_verwijderd;

-- Update events: Add RefId and REMOVE personal data fields
UPDATE public.mt_events e
SET data = e.data
    - 'Insz' - 'Voornaam' - 'Achternaam'
    || jsonb_build_object('RefId', t.ref_id)
FROM temp_event_refids_verwijderd t
WHERE e.id = t.event_id;

DROP TABLE temp_event_refids_verwijderd;

-- Step 3: VertegenwoordigerWerdToegevoegd
-- Persoonsgegevens: ALL fields (Insz, Voornaam, Achternaam, Roepnaam, Rol, Email, Telefoon, Mobiel, SocialMedia)

CREATE TEMP TABLE temp_event_refids_toegevoegd (
    event_id uuid,
    stream_id varchar,
    data jsonb,
    ref_id uuid
);

INSERT INTO temp_event_refids_toegevoegd (event_id, stream_id, data, ref_id)
SELECT
    e.id as event_id,
    e.stream_id,
    e.data,
    gen_random_uuid() as ref_id
FROM public.mt_events e
WHERE e.type = 'vertegenwoordiger_werd_toegevoegd'
  AND e.mt_dotnet_type = 'AssociationRegistry.Events.VertegenwoordigerWerdToegevoegd, AssociationRegistry';

INSERT INTO public.mt_doc_vertegenwoordigerpersoonsgegevensdocument (id, data, mt_dotnet_type, mt_version)
SELECT
    ref_id as id,
    jsonb_build_object(
        'RefId', ref_id,
        'VCode', stream_id,
        'VertegenwoordigerId', (data->>'VertegenwoordigerId')::int,
        'Insz', data->>'Insz',
        'IsPrimair', (data->>'IsPrimair')::boolean,
        'Roepnaam', data->>'Roepnaam',
        'Rol', data->>'Rol',
        'Voornaam', data->>'Voornaam',
        'Achternaam', data->>'Achternaam',
        'Email', data->>'Email',
        'Telefoon', data->>'Telefoon',
        'Mobiel', data->>'Mobiel',
        'SocialMedia', data->>'SocialMedia'
    ) as data,
    'AssociationRegistry.Persoonsgegevens.VertegenwoordigerPersoonsgegevensDocument, AssociationRegistry.Admin.Schema' as mt_dotnet_type,
    md5(random()::text || clock_timestamp()::text)::uuid as mt_version
FROM temp_event_refids_toegevoegd;

-- Update events: Add RefId and REMOVE personal data fields
UPDATE public.mt_events e
SET data = e.data
    - 'Insz' - 'Voornaam' - 'Achternaam' - 'Roepnaam' - 'Rol'
    - 'Email' - 'Telefoon' - 'Mobiel' - 'SocialMedia'
    || jsonb_build_object('RefId', t.ref_id)
FROM temp_event_refids_toegevoegd t
WHERE e.id = t.event_id;

DROP TABLE temp_event_refids_toegevoegd;

-- Step 4: VertegenwoordigerWerdToegevoegdVanuitKBO
-- Persoonsgegevens: Insz, Voornaam, Achternaam (KBO events have limited fields)

CREATE TEMP TABLE temp_event_refids_kbo_toegevoegd (
    event_id uuid,
    stream_id varchar,
    data jsonb,
    ref_id uuid
);

INSERT INTO temp_event_refids_kbo_toegevoegd (event_id, stream_id, data, ref_id)
SELECT
    e.id as event_id,
    e.stream_id,
    e.data,
    gen_random_uuid() as ref_id
FROM public.mt_events e
WHERE e.type = 'vertegenwoordiger_werd_toegevoegd_vanuit_kbo'
  AND e.mt_dotnet_type = 'AssociationRegistry.Events.VertegenwoordigerWerdToegevoegdVanuitKBO, AssociationRegistry';

INSERT INTO public.mt_doc_vertegenwoordigerpersoonsgegevensdocument (id, data, mt_dotnet_type, mt_version)
SELECT
    ref_id as id,
    jsonb_build_object(
        'RefId', ref_id,
        'VCode', stream_id,
        'VertegenwoordigerId', (data->>'VertegenwoordigerId')::int,
        'Insz', data->>'Insz',
        'IsPrimair', NULL,
        'Roepnaam', NULL,
        'Rol', NULL,
        'Voornaam', data->>'Voornaam',
        'Achternaam', data->>'Achternaam',
        'Email', NULL,
        'Telefoon', NULL,
        'Mobiel', NULL,
        'SocialMedia', NULL
    ) as data,
    'AssociationRegistry.Persoonsgegevens.VertegenwoordigerPersoonsgegevensDocument, AssociationRegistry.Admin.Schema' as mt_dotnet_type,
    md5(random()::text || clock_timestamp()::text)::uuid as mt_version
FROM temp_event_refids_kbo_toegevoegd;

-- Update events: Add RefId and REMOVE personal data fields
UPDATE public.mt_events e
SET data = e.data
    - 'Insz' - 'Voornaam' - 'Achternaam'
    || jsonb_build_object('RefId', t.ref_id)
FROM temp_event_refids_kbo_toegevoegd t
WHERE e.id = t.event_id;

DROP TABLE temp_event_refids_kbo_toegevoegd;

-- Step 5: VertegenwoordigerWerdOvergenomenUitKBO
-- Persoonsgegevens: Insz, Voornaam, Achternaam

CREATE TEMP TABLE temp_event_refids_kbo_overgenomen (
    event_id uuid,
    stream_id varchar,
    data jsonb,
    ref_id uuid
);

INSERT INTO temp_event_refids_kbo_overgenomen (event_id, stream_id, data, ref_id)
SELECT
    e.id as event_id,
    e.stream_id,
    e.data,
    gen_random_uuid() as ref_id
FROM public.mt_events e
WHERE e.type = 'vertegenwoordiger_werd_overgenomen_uit_kbo'
  AND e.mt_dotnet_type = 'AssociationRegistry.Events.VertegenwoordigerWerdOvergenomenUitKBO, AssociationRegistry';

INSERT INTO public.mt_doc_vertegenwoordigerpersoonsgegevensdocument (id, data, mt_dotnet_type, mt_version)
SELECT
    ref_id as id,
    jsonb_build_object(
        'RefId', ref_id,
        'VCode', stream_id,
        'VertegenwoordigerId', (data->>'VertegenwoordigerId')::int,
        'Insz', data->>'Insz',
        'IsPrimair', NULL,
        'Roepnaam', NULL,
        'Rol', NULL,
        'Voornaam', data->>'Voornaam',
        'Achternaam', data->>'Achternaam',
        'Email', NULL,
        'Telefoon', NULL,
        'Mobiel', NULL,
        'SocialMedia', NULL
    ) as data,
    'AssociationRegistry.Persoonsgegevens.VertegenwoordigerPersoonsgegevensDocument, AssociationRegistry.Admin.Schema' as mt_dotnet_type,
    md5(random()::text || clock_timestamp()::text)::uuid as mt_version
FROM temp_event_refids_kbo_overgenomen;

-- Update events: Add RefId and REMOVE personal data fields
UPDATE public.mt_events e
SET data = e.data
    - 'Insz' - 'Voornaam' - 'Achternaam'
    || jsonb_build_object('RefId', t.ref_id)
FROM temp_event_refids_kbo_overgenomen t
WHERE e.id = t.event_id;

DROP TABLE temp_event_refids_kbo_overgenomen;

-- Step 6: VertegenwoordigerWerdVerwijderdUitKBO
-- Persoonsgegevens: Insz, Voornaam, Achternaam

CREATE TEMP TABLE temp_event_refids_kbo_verwijderd (
    event_id uuid,
    stream_id varchar,
    data jsonb,
    ref_id uuid
);

INSERT INTO temp_event_refids_kbo_verwijderd (event_id, stream_id, data, ref_id)
SELECT
    e.id as event_id,
    e.stream_id,
    e.data,
    gen_random_uuid() as ref_id
FROM public.mt_events e
WHERE e.type = 'vertegenwoordiger_werd_verwijderd_uit_kbo'
  AND e.mt_dotnet_type = 'AssociationRegistry.Events.VertegenwoordigerWerdVerwijderdUitKBO, AssociationRegistry';

INSERT INTO public.mt_doc_vertegenwoordigerpersoonsgegevensdocument (id, data, mt_dotnet_type, mt_version)
SELECT
    ref_id as id,
    jsonb_build_object(
        'RefId', ref_id,
        'VCode', stream_id,
        'VertegenwoordigerId', (data->>'VertegenwoordigerId')::int,
        'Insz', data->>'Insz',
        'IsPrimair', NULL,
        'Roepnaam', NULL,
        'Rol', NULL,
        'Voornaam', data->>'Voornaam',
        'Achternaam', data->>'Achternaam',
        'Email', NULL,
        'Telefoon', NULL,
        'Mobiel', NULL,
        'SocialMedia', NULL
    ) as data,
    'AssociationRegistry.Persoonsgegevens.VertegenwoordigerPersoonsgegevensDocument, AssociationRegistry.Admin.Schema' as mt_dotnet_type,
    md5(random()::text || clock_timestamp()::text)::uuid as mt_version
FROM temp_event_refids_kbo_verwijderd;

-- Update events: Add RefId and REMOVE personal data fields
UPDATE public.mt_events e
SET data = e.data
    - 'Insz' - 'Voornaam' - 'Achternaam'
    || jsonb_build_object('RefId', t.ref_id)
FROM temp_event_refids_kbo_verwijderd t
WHERE e.id = t.event_id;

DROP TABLE temp_event_refids_kbo_verwijderd;

-- Step 7: FeitelijkeVerenigingWerdGeregistreerd (Vertegenwoordigers ARRAY)
-- This is more complex - each event may have MULTIPLE vertegenwoordigers in an array
-- We need to extract each one and update the array element with RefId

-- First, create temp table with one row per vertegenwoordiger (flattened array)
CREATE TEMP TABLE temp_registratie_feitelijk (
    event_id uuid,
    stream_id varchar,
    vertegenwoordiger_id int,
    array_index int,
    vertegenwoordiger jsonb,
    ref_id uuid
);

-- Flatten the array: one row per vertegenwoordiger
INSERT INTO temp_registratie_feitelijk (event_id, stream_id, vertegenwoordiger_id, array_index, vertegenwoordiger, ref_id)
SELECT
    e.id as event_id,
    e.stream_id,
    (vertegenwoordiger->>'VertegenwoordigerId')::int as vertegenwoordiger_id,
    (vertegenwoordiger_index - 1) as array_index,
    vertegenwoordiger,
    gen_random_uuid() as ref_id
FROM public.mt_events e
CROSS JOIN LATERAL jsonb_array_elements(e.data->'Vertegenwoordigers') WITH ORDINALITY AS t(vertegenwoordiger, vertegenwoordiger_index)
WHERE e.type = 'feitelijke_vereniging_werd_geregistreerd'
  AND e.mt_dotnet_type = 'AssociationRegistry.Events.FeitelijkeVerenigingWerdGeregistreerd, AssociationRegistry';

-- Insert persoonsgegevens for each vertegenwoordiger
INSERT INTO public.mt_doc_vertegenwoordigerpersoonsgegevensdocument (id, data, mt_dotnet_type, mt_version)
SELECT
    ref_id as id,
    jsonb_build_object(
        'RefId', ref_id,
        'VCode', stream_id,
        'VertegenwoordigerId', vertegenwoordiger_id,
        'Insz', vertegenwoordiger->>'Insz',
        'IsPrimair', (vertegenwoordiger->>'IsPrimair')::boolean,
        'Roepnaam', vertegenwoordiger->>'Roepnaam',
        'Rol', vertegenwoordiger->>'Rol',
        'Voornaam', vertegenwoordiger->>'Voornaam',
        'Achternaam', vertegenwoordiger->>'Achternaam',
        'Email', vertegenwoordiger->>'Email',
        'Telefoon', vertegenwoordiger->>'Telefoon',
        'Mobiel', vertegenwoordiger->>'Mobiel',
        'SocialMedia', vertegenwoordiger->>'SocialMedia'
    ) as data,
    'AssociationRegistry.Persoonsgegevens.VertegenwoordigerPersoonsgegevensDocument, AssociationRegistry.Admin.Schema' as mt_dotnet_type,
    md5(random()::text || clock_timestamp()::text)::uuid as mt_version
FROM temp_registratie_feitelijk;

-- Update each array element: Add RefId, remove personal data
-- We rebuild the entire Vertegenwoordigers array for each event
WITH updated_vertegenwoordigers AS (
    SELECT
        event_id,
        jsonb_agg(
            vertegenwoordiger
            - 'Insz' - 'Voornaam' - 'Achternaam' - 'Roepnaam' - 'Rol'
            - 'Email' - 'Telefoon' - 'Mobiel' - 'SocialMedia'
            || jsonb_build_object('RefId', ref_id)
            ORDER BY array_index
        ) as new_vertegenwoordigers_array
    FROM temp_registratie_feitelijk
    GROUP BY event_id
)
UPDATE public.mt_events e
SET data = jsonb_set(e.data, '{Vertegenwoordigers}', uv.new_vertegenwoordigers_array)
FROM updated_vertegenwoordigers uv
WHERE e.id = uv.event_id;

DROP TABLE temp_registratie_feitelijk;

-- Step 8: VerenigingMetRechtspersoonlijkheidWerdGeregistreerd (Vertegenwoordigers ARRAY)

CREATE TEMP TABLE temp_registratie_rechtspersoon (
    event_id uuid,
    stream_id varchar,
    vertegenwoordiger_id int,
    array_index int,
    vertegenwoordiger jsonb,
    ref_id uuid
);

INSERT INTO temp_registratie_rechtspersoon (event_id, stream_id, vertegenwoordiger_id, array_index, vertegenwoordiger, ref_id)
SELECT
    e.id as event_id,
    e.stream_id,
    (vertegenwoordiger->>'VertegenwoordigerId')::int as vertegenwoordiger_id,
    (vertegenwoordiger_index - 1) as array_index,
    vertegenwoordiger,
    gen_random_uuid() as ref_id
FROM public.mt_events e
CROSS JOIN LATERAL jsonb_array_elements(e.data->'Vertegenwoordigers') WITH ORDINALITY AS t(vertegenwoordiger, vertegenwoordiger_index)
WHERE e.type = 'vereniging_met_rechtspersoonlijkheid_werd_geregistreerd'
  AND e.mt_dotnet_type = 'AssociationRegistry.Events.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, AssociationRegistry';

INSERT INTO public.mt_doc_vertegenwoordigerpersoonsgegevensdocument (id, data, mt_dotnet_type, mt_version)
SELECT
    ref_id as id,
    jsonb_build_object(
        'RefId', ref_id,
        'VCode', stream_id,
        'VertegenwoordigerId', vertegenwoordiger_id,
        'Insz', vertegenwoordiger->>'Insz',
        'IsPrimair', (vertegenwoordiger->>'IsPrimair')::boolean,
        'Roepnaam', vertegenwoordiger->>'Roepnaam',
        'Rol', vertegenwoordiger->>'Rol',
        'Voornaam', vertegenwoordiger->>'Voornaam',
        'Achternaam', vertegenwoordiger->>'Achternaam',
        'Email', vertegenwoordiger->>'Email',
        'Telefoon', vertegenwoordiger->>'Telefoon',
        'Mobiel', vertegenwoordiger->>'Mobiel',
        'SocialMedia', vertegenwoordiger->>'SocialMedia'
    ) as data,
    'AssociationRegistry.Persoonsgegevens.VertegenwoordigerPersoonsgegevensDocument, AssociationRegistry.Admin.Schema' as mt_dotnet_type,
    md5(random()::text || clock_timestamp()::text)::uuid as mt_version
FROM temp_registratie_rechtspersoon;

WITH updated_vertegenwoordigers AS (
    SELECT
        event_id,
        jsonb_agg(
            vertegenwoordiger
            - 'Insz' - 'Voornaam' - 'Achternaam' - 'Roepnaam' - 'Rol'
            - 'Email' - 'Telefoon' - 'Mobiel' - 'SocialMedia'
            || jsonb_build_object('RefId', ref_id)
            ORDER BY array_index
        ) as new_vertegenwoordigers_array
    FROM temp_registratie_rechtspersoon
    GROUP BY event_id
)
UPDATE public.mt_events e
SET data = jsonb_set(e.data, '{Vertegenwoordigers}', uv.new_vertegenwoordigers_array)
FROM updated_vertegenwoordigers uv
WHERE e.id = uv.event_id;

DROP TABLE temp_registratie_rechtspersoon;

-- ==================================================================================
-- EVENT RENAMING: Mark old events as "ZonderPersoonsgegevens"
-- ==================================================================================
--
-- After extracting persoonsgegevens, we rename the event types to reflect that
-- they no longer contain personal data inline. This allows the application code
-- to distinguish between:
--   - Old events (now renamed): Use upcasters to JOIN with persoonsgegevens table
--   - New events (going forward): Already created with RefId by application code
--
-- Event type renaming updates BOTH:
--   1. type: snake_case event name (e.g., 'vertegenwoordiger_werd_gewijzigd')
--   2. mt_dotnet_type: Fully qualified .NET type name with assembly
-- ==================================================================================

-- VertegenwoordigerWerdGewijzigd -> VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens
UPDATE public.mt_events
SET type = 'vertegenwoordiger_werd_gewijzigd_zonder_persoonsgegevens',
    mt_dotnet_type = 'AssociationRegistry.Events.VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens, AssociationRegistry'
WHERE type = 'vertegenwoordiger_werd_gewijzigd'
  AND mt_dotnet_type = 'AssociationRegistry.Events.VertegenwoordigerWerdGewijzigd, AssociationRegistry';

-- VertegenwoordigerWerdVerwijderd -> VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens
UPDATE public.mt_events
SET type = 'vertegenwoordiger_werd_verwijderd_zonder_persoonsgegevens',
    mt_dotnet_type = 'AssociationRegistry.Events.VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens, AssociationRegistry'
WHERE type = 'vertegenwoordiger_werd_verwijderd'
  AND mt_dotnet_type = 'AssociationRegistry.Events.VertegenwoordigerWerdVerwijderd, AssociationRegistry';

-- VertegenwoordigerWerdToegevoegd -> VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens
UPDATE public.mt_events
SET type = 'vertegenwoordiger_werd_toegevoegd_zonder_persoonsgegevens',
    mt_dotnet_type = 'AssociationRegistry.Events.VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens, AssociationRegistry'
WHERE type = 'vertegenwoordiger_werd_toegevoegd'
  AND mt_dotnet_type = 'AssociationRegistry.Events.VertegenwoordigerWerdToegevoegd, AssociationRegistry';

-- FeitelijkeVerenigingWerdGeregistreerd -> FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens
UPDATE public.mt_events
SET type = 'feitelijke_vereniging_werd_geregistreerd_zonder_persoonsgegevens',
    mt_dotnet_type = 'AssociationRegistry.Events.FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens, AssociationRegistry'
WHERE type = 'feitelijke_vereniging_werd_geregistreerd'
  AND mt_dotnet_type = 'AssociationRegistry.Events.FeitelijkeVerenigingWerdGeregistreerd, AssociationRegistry';

-- VerenigingMetRechtspersoonlijkheidWerdGeregistreerd -> VerenigingMetRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens
UPDATE public.mt_events
SET type = 'vereniging_met_rechtspersoonlijkheid_werd_geregistreerd_zonder_persoonsgegevens',
    mt_dotnet_type = 'AssociationRegistry.Events.VerenigingMetRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens, AssociationRegistry'
WHERE type = 'vereniging_met_rechtspersoonlijkheid_werd_geregistreerd'
  AND mt_dotnet_type = 'AssociationRegistry.Events.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, AssociationRegistry';

-- VertegenwoordigerWerdToegevoegdVanuitKBO -> VertegenwoordigerWerdToegevoegdVanuitKBOZonderPersoonsgegevens
UPDATE public.mt_events
SET type = 'vertegenwoordiger_werd_toegevoegd_vanuit_kbo_zonder_persoonsgegevens',
    mt_dotnet_type = 'AssociationRegistry.Events.VertegenwoordigerWerdToegevoegdVanuitKBOZonderPersoonsgegevens, AssociationRegistry'
WHERE type = 'vertegenwoordiger_werd_toegevoegd_vanuit_kbo'
  AND mt_dotnet_type = 'AssociationRegistry.Events.VertegenwoordigerWerdToegevoegdVanuitKBO, AssociationRegistry';

-- VertegenwoordigerWerdOvergenomenUitKBO -> VertegenwoordigerWerdOvergenomenUitKBOZonderPersoonsgegevens
UPDATE public.mt_events
SET type = 'vertegenwoordiger_werd_overgenomen_uit_kbo_zonder_persoonsgegevens',
    mt_dotnet_type = 'AssociationRegistry.Events.VertegenwoordigerWerdOvergenomenUitKBOZonderPersoonsgegevens, AssociationRegistry'
WHERE type = 'vertegenwoordiger_werd_overgenomen_uit_kbo'
  AND mt_dotnet_type = 'AssociationRegistry.Events.VertegenwoordigerWerdOvergenomenUitKBO, AssociationRegistry';

-- VertegenwoordigerWerdVerwijderdUitKBO -> VertegenwoordigerWerdVerwijderdUitKBOZonderPersoonsgegevens
UPDATE public.mt_events
SET type = 'vertegenwoordiger_werd_verwijderd_uit_kbo_zonder_persoonsgegevens',
    mt_dotnet_type = 'AssociationRegistry.Events.VertegenwoordigerWerdVerwijderdUitKBOZonderPersoonsgegevens, AssociationRegistry'
WHERE type = 'vertegenwoordiger_werd_verwijderd_uit_kbo'
  AND mt_dotnet_type = 'AssociationRegistry.Events.VertegenwoordigerWerdVerwijderdUitKBO, AssociationRegistry';

-- Fix typo: VertegenwoordigerWerdGewijzigdZonderPersoongegevens -> VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens
UPDATE public.mt_events
SET type = 'vertegenwoordiger_werd_gewijzigd_zonder_persoonsgegevens',
    mt_dotnet_type = 'AssociationRegistry.Events.VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens, AssociationRegistry'
WHERE type = 'vertegenwoordiger_werd_gewijzigd_zonder_persoongegevens'
  AND mt_dotnet_type = 'AssociationRegistry.Events.VertegenwoordigerWerdGewijzigdZonderPersoongegevens, AssociationRegistry';
