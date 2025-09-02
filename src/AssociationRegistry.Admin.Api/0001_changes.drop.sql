DROP TABLE IF EXISTS public.mt_doc_newtabledocument2 CASCADE;
drop function if exists public.mt_upsert_newtabledocument2(JSONB, varchar, varchar, uuid);
drop function if exists public.mt_insert_newtabledocument2(JSONB, varchar, varchar, uuid);
drop function if exists public.mt_update_newtabledocument2(JSONB, varchar, varchar, uuid);
