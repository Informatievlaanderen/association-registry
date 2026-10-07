DELETE FROM public.mt_event_progression
WHERE name = 'beheer.postgres.vertegenwoordigerspervcode:All';

DROP FUNCTION IF EXISTS public.mt_upsert_vertegenwoordigerspervcodedocument(JSONB, varchar, varchar, integer);
DROP FUNCTION IF EXISTS public.mt_insert_vertegenwoordigerspervcodedocument(JSONB, varchar, varchar, integer);
DROP FUNCTION IF EXISTS public.mt_update_vertegenwoordigerspervcodedocument(JSONB, varchar, varchar, integer);
DROP FUNCTION IF EXISTS public.mt_overwrite_vertegenwoordigerspervcodedocument(JSONB, varchar, varchar, integer);

DROP TABLE IF EXISTS public.mt_doc_vertegenwoordigerspervcodedocument CASCADE;

