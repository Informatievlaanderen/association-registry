--
-- PostgreSQL database dump
--

-- Dumped from database version 14.17
-- Dumped by pg_dump version 17.5

-- SET statement_timeout = 0;
-- SET lock_timeout = 0;
-- SET idle_in_transaction_session_timeout = 0;
-- SET transaction_timeout = 0;
-- SET client_encoding = 'UTF8';
-- SET standard_conforming_strings = on;
-- SELECT pg_catalog.set_config('search_path', '', false);
-- SET check_function_bodies = false;
-- SET xmloption = content;
-- SET client_min_messages = warning;
-- SET row_security = off;

--
-- Name: public; Type: SCHEMA; Schema: -; Owner: -
--

-- *not* creating schema, since initdb creates it


--
-- Name: wolverine_queues; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA wolverine_queues;


--
-- Name: mt_archive_stream(character varying); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_archive_stream(streamid character varying) RETURNS void
    LANGUAGE plpgsql
    AS $$
BEGIN
  update public.mt_streams set is_archived = TRUE where id = streamid;
  update public.mt_events set is_archived = TRUE where stream_id = streamid;
END;
$$;


--
-- Name: mt_grams_array(text, boolean); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_grams_array(words text, use_unaccent boolean DEFAULT false) RETURNS text[]
    LANGUAGE plpgsql IMMUTABLE STRICT
    AS $$
        DECLARE
result text[];
        DECLARE
word text;
        DECLARE
clean_word text;
BEGIN
                FOREACH
word IN ARRAY string_to_array(words, ' ')
                LOOP
                     clean_word = regexp_replace(public.mt_safe_unaccent(use_unaccent, word), '[^a-zA-Z0-9]+', '','g');
FOR i IN 1 .. length(clean_word)
                     LOOP
                         result := result || quote_literal(substr(lower(clean_word), i, 1));
                         result
:= result || quote_literal(substr(lower(clean_word), i, 2));
                         result
:= result || quote_literal(substr(lower(clean_word), i, 3));
END LOOP;
END LOOP;

RETURN ARRAY(SELECT DISTINCT e FROM unnest(result) AS a(e) ORDER BY e);
END;
$$;


--
-- Name: mt_grams_query(text, boolean); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_grams_query(text, use_unaccent boolean DEFAULT false) RETURNS tsquery
    LANGUAGE plpgsql IMMUTABLE STRICT
    AS $_$
BEGIN
RETURN (SELECT array_to_string(public.mt_grams_array($1, use_unaccent), ' & ') ::tsquery);
END
$_$;


--
-- Name: mt_grams_vector(text, boolean); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_grams_vector(text, use_unaccent boolean DEFAULT false) RETURNS tsvector
    LANGUAGE plpgsql IMMUTABLE STRICT
    AS $_$
BEGIN
RETURN (SELECT array_to_string(public.mt_grams_array($1, use_unaccent), ' ') ::tsvector);
END
$_$;


--
-- Name: mt_immutable_date(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_immutable_date(value text) RETURNS date
    LANGUAGE sql IMMUTABLE
    AS $$
select value::date

$$;


--
-- Name: mt_immutable_time(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_immutable_time(value text) RETURNS time without time zone
    LANGUAGE sql IMMUTABLE
    AS $$
select value::time

$$;


--
-- Name: mt_immutable_timestamp(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_immutable_timestamp(value text) RETURNS timestamp without time zone
    LANGUAGE sql IMMUTABLE
    AS $$
select value::timestamp

$$;


--
-- Name: mt_immutable_timestamptz(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_immutable_timestamptz(value text) RETURNS timestamp with time zone
    LANGUAGE sql IMMUTABLE
    AS $$
select value::timestamptz

$$;


--
-- Name: mt_insert_addresskafkaconsumeroffset(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_insert_addresskafkaconsumeroffset(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO public.mt_doc_addresskafkaconsumeroffset ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp());

  RETURN docVersion;
END;
$$;


--
-- Name: mt_insert_beheerkbosynchistoriekgebeurtenisdocument(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_insert_beheerkbosynchistoriekgebeurtenisdocument(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO public.mt_doc_beheerkbosynchistoriekgebeurtenisdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp());

  RETURN docVersion;
END;
$$;


--
-- Name: mt_insert_beheerverenigingdetaildocument(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_insert_beheerverenigingdetaildocument(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO public.mt_doc_beheerverenigingdetaildocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp());

  RETURN docVersion;
END;
$$;


--
-- Name: mt_insert_beheervereniginghistoriekdocument(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_insert_beheervereniginghistoriekdocument(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO public.mt_doc_beheervereniginghistoriekdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp());

  RETURN docVersion;
END;
$$;


--
-- Name: mt_insert_deadletterevent(jsonb, character varying, uuid, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_insert_deadletterevent(doc jsonb, docdotnettype character varying, docid uuid, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO public.mt_doc_deadletterevent ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp());

  RETURN docVersion;
END;
$$;


--
-- Name: mt_insert_envelope(jsonb, character varying, uuid, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_insert_envelope(doc jsonb, docdotnettype character varying, docid uuid, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO public.mt_doc_envelope ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp());

  RETURN docVersion;
END;
$$;


--
-- Name: mt_insert_geotagmigration(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_insert_geotagmigration(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO public.mt_doc_geotagmigration ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp());

  RETURN docVersion;
END;
$$;


--
-- Name: mt_insert_locatielookupdocument(jsonb, character varying, character varying, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_insert_locatielookupdocument(doc jsonb, docdotnettype character varying, docid character varying, revision integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO public.mt_doc_locatielookupdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, revision, transaction_timestamp());
  RETURN 1;
END;
$$;


--
-- Name: mt_insert_locatiezonderadresmatchdocument(jsonb, character varying, character varying, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_insert_locatiezonderadresmatchdocument(doc jsonb, docdotnettype character varying, docid character varying, revision integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO public.mt_doc_locatiezonderadresmatchdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, revision, transaction_timestamp());
  RETURN 1;
END;
$$;


--
-- Name: mt_insert_magdacallreference(jsonb, character varying, uuid, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_insert_magdacallreference(doc jsonb, docdotnettype character varying, docid uuid, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO public.mt_doc_magdacallreference ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp());

  RETURN docVersion;
END;
$$;


--
-- Name: mt_insert_postalnutslauinfo(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_insert_postalnutslauinfo(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO public.mt_doc_postalnutslauinfo ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp());

  RETURN docVersion;
END;
$$;


--
-- Name: mt_insert_powerbiexportdocument(jsonb, character varying, character varying, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_insert_powerbiexportdocument(doc jsonb, docdotnettype character varying, docid character varying, revision integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO public.mt_doc_powerbiexportdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, revision, transaction_timestamp());
  RETURN 1;
END;
$$;


--
-- Name: mt_insert_publiekverenigingdetaildocument(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_insert_publiekverenigingdetaildocument(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO public.mt_doc_publiekverenigingdetaildocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp());

  RETURN docVersion;
END;
$$;


--
-- Name: mt_insert_publiekverenigingsequencedocument(jsonb, character varying, character varying, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_insert_publiekverenigingsequencedocument(doc jsonb, docdotnettype character varying, docid character varying, revision integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO public.mt_doc_publiekverenigingsequencedocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, revision, transaction_timestamp());
  RETURN 1;
END;
$$;


--
-- Name: mt_insert_settingoverride(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_insert_settingoverride(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO public.mt_doc_settingoverride ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp());

  RETURN docVersion;
END;
$$;


--
-- Name: mt_insert_verenigingdocument(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_insert_verenigingdocument(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO public.mt_doc_verenigingdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp());

  RETURN docVersion;
END;
$$;


--
-- Name: mt_insert_verenigingenperinszdocument(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_insert_verenigingenperinszdocument(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO public.mt_doc_verenigingenperinszdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp());

  RETURN docVersion;
END;
$$;


--
-- Name: mt_insert_verenigingstate(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_insert_verenigingstate(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO public.mt_doc_verenigingstate ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp());

  RETURN docVersion;
END;
$$;


--
-- Name: mt_jsonb_append(jsonb, text[], jsonb, boolean, jsonb); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_jsonb_append(jsonb, text[], jsonb, boolean, jsonb DEFAULT NULL::jsonb) RETURNS jsonb
    LANGUAGE plpgsql
    AS $_$
DECLARE
    retval ALIAS FOR $1;
    location ALIAS FOR $2;
    val ALIAS FOR $3;
    if_not_exists ALIAS FOR $4;
    patch_expression ALIAS FOR $5;
    tmp_value jsonb;
BEGIN
    tmp_value = retval #> location;
    IF tmp_value IS NOT NULL AND jsonb_typeof(tmp_value) = 'array' THEN
        CASE
            WHEN NOT if_not_exists THEN
                retval = jsonb_set(retval, location, tmp_value || val, FALSE);
            WHEN patch_expression IS NULL AND jsonb_typeof(val) = 'object' AND NOT tmp_value @> jsonb_build_array(val) THEN
                retval = jsonb_set(retval, location, tmp_value || val, FALSE);
            WHEN patch_expression IS NULL AND jsonb_typeof(val) <> 'object' AND NOT tmp_value @> val THEN
                retval = jsonb_set(retval, location, tmp_value || val, FALSE);
            WHEN patch_expression IS NOT NULL AND jsonb_typeof(patch_expression) = 'array' AND jsonb_array_length(patch_expression) = 0 THEN
                retval = jsonb_set(retval, location, tmp_value || val, FALSE);
            ELSE NULL;
            END CASE;
    END IF;
    RETURN retval;
END;
$_$;


--
-- Name: mt_jsonb_copy(jsonb, text[], text[]); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_jsonb_copy(jsonb, text[], text[]) RETURNS jsonb
    LANGUAGE plpgsql
    AS $_$
DECLARE
    retval ALIAS FOR $1;
    src_path ALIAS FOR $2;
    dst_path ALIAS FOR $3;
    tmp_value jsonb;
BEGIN
    tmp_value = retval #> src_path;
    retval = public.mt_jsonb_fix_null_parent(retval, dst_path);
    RETURN jsonb_set(retval, dst_path, tmp_value::jsonb, TRUE);
END;
$_$;


--
-- Name: mt_jsonb_duplicate(jsonb, text[], jsonb); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_jsonb_duplicate(jsonb, text[], jsonb) RETURNS jsonb
    LANGUAGE plpgsql
    AS $_$
DECLARE
    retval ALIAS FOR $1;
    location ALIAS FOR $2;
    targets ALIAS FOR $3;
    tmp_value jsonb;
    target_path text[];
    target text;
BEGIN
    FOR target IN SELECT jsonb_array_elements_text(targets)
    LOOP
        target_path = public.mt_jsonb_path_to_array(target, '\.');
        retval = public.mt_jsonb_copy(retval, location, target_path);
    END LOOP;

    RETURN retval;
END;
$_$;


--
-- Name: mt_jsonb_fix_null_parent(jsonb, text[]); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_jsonb_fix_null_parent(jsonb, text[]) RETURNS jsonb
    LANGUAGE plpgsql
    AS $_$
DECLARE
retval ALIAS FOR $1;
    dst_path ALIAS FOR $2;
    dst_path_segment text[] = ARRAY[]::text[];
    dst_path_array_length integer;
    i integer = 1;
BEGIN
    dst_path_array_length = array_length(dst_path, 1);
    WHILE i <=(dst_path_array_length - 1)
    LOOP
        dst_path_segment = dst_path_segment || ARRAY[dst_path[i]];
        IF retval #> dst_path_segment IS NULL OR retval #> dst_path_segment = 'null'::jsonb THEN
            retval = jsonb_set(retval, dst_path_segment, '{}'::jsonb, TRUE);
        END IF;
        i = i + 1;
    END LOOP;

    RETURN retval;
END;
$_$;


--
-- Name: mt_jsonb_increment(jsonb, text[], numeric); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_jsonb_increment(jsonb, text[], numeric) RETURNS jsonb
    LANGUAGE plpgsql
    AS $_$
DECLARE
retval ALIAS FOR $1;
    location ALIAS FOR $2;
    increment_value ALIAS FOR $3;
    tmp_value jsonb;
BEGIN
    tmp_value = retval #> location;
    IF tmp_value IS NULL THEN
        tmp_value = to_jsonb(0);
END IF;

RETURN jsonb_set(retval, location, to_jsonb(tmp_value::numeric + increment_value), TRUE);
END;
$_$;


--
-- Name: mt_jsonb_insert(jsonb, text[], jsonb, integer, boolean, jsonb); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_jsonb_insert(jsonb, text[], jsonb, integer, boolean, jsonb DEFAULT NULL::jsonb) RETURNS jsonb
    LANGUAGE plpgsql
    AS $_$
DECLARE
    retval ALIAS FOR $1;
    location ALIAS FOR $2;
    val ALIAS FOR $3;
    elm_index ALIAS FOR $4;
    if_not_exists ALIAS FOR $5;
    patch_expression ALIAS FOR $6;
    tmp_value jsonb;
BEGIN
    tmp_value = retval #> location;
    IF tmp_value IS NOT NULL AND jsonb_typeof(tmp_value) = 'array' THEN
        IF elm_index IS NULL THEN
            elm_index = jsonb_array_length(tmp_value) + 1;
        END IF;
        CASE
            WHEN NOT if_not_exists THEN
                retval = jsonb_insert(retval, location || elm_index::text, val);
            WHEN patch_expression IS NULL AND jsonb_typeof(val) = 'object' AND NOT tmp_value @> jsonb_build_array(val) THEN
                retval = jsonb_insert(retval, location || elm_index::text, val);
            WHEN patch_expression IS NULL AND jsonb_typeof(val) <> 'object' AND NOT tmp_value @> val THEN
                retval = jsonb_insert(retval, location || elm_index::text, val);
            WHEN patch_expression IS NOT NULL AND jsonb_typeof(patch_expression) = 'array' AND jsonb_array_length(patch_expression) = 0 THEN
                retval = jsonb_insert(retval, location || elm_index::text, val);
            ELSE NULL;
        END CASE;
    END IF;
    RETURN retval;
END;
$_$;


--
-- Name: mt_jsonb_move(jsonb, text[], text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_jsonb_move(jsonb, text[], text) RETURNS jsonb
    LANGUAGE plpgsql
    AS $_$
DECLARE
    retval ALIAS FOR $1;
    src_path ALIAS FOR $2;
    dst_name ALIAS FOR $3;
    dst_path text[];
    tmp_value jsonb;
BEGIN
    tmp_value = retval #> src_path;
    retval = retval #- src_path;
    dst_path = src_path;
    dst_path[array_length(dst_path, 1)] = dst_name;
    retval = public.mt_jsonb_fix_null_parent(retval, dst_path);
    RETURN jsonb_set(retval, dst_path, tmp_value, TRUE);
END;
$_$;


--
-- Name: mt_jsonb_patch(jsonb, jsonb); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_jsonb_patch(jsonb, jsonb) RETURNS jsonb
    LANGUAGE plpgsql
    AS $_$
DECLARE
    retval ALIAS FOR $1;
    patchset ALIAS FOR $2;
    patch jsonb;
    patch_path text[];
    patch_expression jsonb;
    value jsonb;
BEGIN
    FOR patch IN SELECT * from jsonb_array_elements(patchset)
    LOOP
        patch_path = public.mt_jsonb_path_to_array((patch->>'path')::text, '\.');

        patch_expression = null;
        IF (patch->>'type') IN ('remove', 'append_if_not_exists', 'insert_if_not_exists') AND (patch->>'expression') IS NOT NULL THEN
            patch_expression = jsonb_path_query_array(retval #> patch_path, (patch->>'expression')::jsonpath);
        END IF;

        CASE patch->>'type'
            WHEN 'set' THEN
                retval = jsonb_set(retval, patch_path, (patch->'value')::jsonb, TRUE);
            WHEN 'delete' THEN
                retval = retval#-patch_path;
            WHEN 'append' THEN
                retval = public.mt_jsonb_append(retval, patch_path, (patch->'value')::jsonb, FALSE);
            WHEN 'append_if_not_exists' THEN
                retval = public.mt_jsonb_append(retval, patch_path, (patch->'value')::jsonb, TRUE, patch_expression);
            WHEN 'insert' THEN
                retval = public.mt_jsonb_insert(retval, patch_path, (patch->'value')::jsonb, (patch->>'index')::integer, FALSE);
            WHEN 'insert_if_not_exists' THEN
                retval = public.mt_jsonb_insert(retval, patch_path, (patch->'value')::jsonb, (patch->>'index')::integer, TRUE, patch_expression);
            WHEN 'remove' THEN
                retval = public.mt_jsonb_remove(retval, patch_path, COALESCE(patch_expression, (patch->'value')::jsonb));
            WHEN 'duplicate' THEN
                retval = public.mt_jsonb_duplicate(retval, patch_path, (patch->'targets')::jsonb);
            WHEN 'rename' THEN
                retval = public.mt_jsonb_move(retval, patch_path, (patch->>'to')::text);
            WHEN 'increment' THEN
                retval = public.mt_jsonb_increment(retval, patch_path, (patch->>'increment')::numeric);
            WHEN 'increment_float' THEN
                retval = public.mt_jsonb_increment(retval, patch_path, (patch->>'increment')::numeric);
            ELSE NULL;
        END CASE;
    END LOOP;
    RETURN retval;
END;
$_$;


--
-- Name: mt_jsonb_path_to_array(text, character); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_jsonb_path_to_array(text, character) RETURNS text[]
    LANGUAGE plpgsql
    AS $_$
DECLARE
    location ALIAS FOR $1;
    regex_pattern ALIAS FOR $2;
BEGIN
RETURN regexp_split_to_array(location, regex_pattern)::text[];
END;
$_$;


--
-- Name: mt_jsonb_remove(jsonb, text[], jsonb); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_jsonb_remove(jsonb, text[], jsonb) RETURNS jsonb
    LANGUAGE plpgsql
    AS $_$
DECLARE
    retval ALIAS FOR $1;
    location ALIAS FOR $2;
    val ALIAS FOR $3;
    tmp_value jsonb;
    tmp_remove jsonb;
    patch_remove jsonb;
BEGIN
    tmp_value = retval #> location;
    IF tmp_value IS NOT NULL AND jsonb_typeof(tmp_value) = 'array' THEN
        IF jsonb_typeof(val) = 'array' THEN
            tmp_remove = val;
        ELSE
            tmp_remove = jsonb_build_array(val);
        END IF;

        FOR patch_remove IN SELECT * FROM jsonb_array_elements(tmp_remove)
        LOOP
            tmp_value =(SELECT jsonb_agg(elem)
            FROM jsonb_array_elements(tmp_value) AS elem
            WHERE elem <> patch_remove);
        END LOOP;

        IF tmp_value IS NULL THEN
            tmp_value = '[]'::jsonb;
        END IF;
    END IF;
    RETURN jsonb_set(retval, location, tmp_value, FALSE);
END;
$_$;


--
-- Name: mt_mark_event_progression(character varying, bigint); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_mark_event_progression(name character varying, last_encountered bigint) RETURNS void
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO public.mt_event_progression (name, last_seq_id, last_updated)
VALUES (name, last_encountered, transaction_timestamp())
ON CONFLICT ON CONSTRAINT pk_mt_event_progression
    DO
UPDATE SET last_seq_id = last_encountered, last_updated = transaction_timestamp();

END;

$$;


--
-- Name: mt_overwrite_locatielookupdocument(jsonb, character varying, character varying, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_overwrite_locatielookupdocument(doc jsonb, docdotnettype character varying, docid character varying, revision integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN

  if revision = 0 then
    SELECT mt_version FROM public.mt_doc_locatielookupdocument into current_version WHERE id = docId ;
    if current_version is not null then
      revision = current_version + 1;
    else
      revision = 1;
    end if;
  end if;

  INSERT INTO public.mt_doc_locatielookupdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, revision, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_locatielookupdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_overwrite_locatiezonderadresmatchdocument(jsonb, character varying, character varying, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_overwrite_locatiezonderadresmatchdocument(doc jsonb, docdotnettype character varying, docid character varying, revision integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN

  if revision = 0 then
    SELECT mt_version FROM public.mt_doc_locatiezonderadresmatchdocument into current_version WHERE id = docId ;
    if current_version is not null then
      revision = current_version + 1;
    else
      revision = 1;
    end if;
  end if;

  INSERT INTO public.mt_doc_locatiezonderadresmatchdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, revision, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_locatiezonderadresmatchdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_overwrite_powerbiexportdocument(jsonb, character varying, character varying, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_overwrite_powerbiexportdocument(doc jsonb, docdotnettype character varying, docid character varying, revision integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN

  if revision = 0 then
    SELECT mt_version FROM public.mt_doc_powerbiexportdocument into current_version WHERE id = docId ;
    if current_version is not null then
      revision = current_version + 1;
    else
      revision = 1;
    end if;
  end if;

  INSERT INTO public.mt_doc_powerbiexportdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, revision, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_powerbiexportdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_overwrite_publiekverenigingsequencedocument(jsonb, character varying, character varying, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_overwrite_publiekverenigingsequencedocument(doc jsonb, docdotnettype character varying, docid character varying, revision integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN

  if revision = 0 then
    SELECT mt_version FROM public.mt_doc_publiekverenigingsequencedocument into current_version WHERE id = docId ;
    if current_version is not null then
      revision = current_version + 1;
    else
      revision = 1;
    end if;
  end if;

  INSERT INTO public.mt_doc_publiekverenigingsequencedocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, revision, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_publiekverenigingsequencedocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_quick_append_events(character varying, character varying, character varying, uuid[], character varying[], character varying[], jsonb[], character varying[], character varying[], jsonb[]); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_quick_append_events(stream character varying, stream_type character varying, tenantid character varying, event_ids uuid[], event_types character varying[], dotnet_types character varying[], bodies jsonb[], causation_ids character varying[], correlation_ids character varying[], headers jsonb[]) RETURNS integer[]
    LANGUAGE plpgsql
    AS $$
DECLARE
	event_version int;
	event_type varchar;
	event_id uuid;
	body jsonb;
	index int;
	seq int;
    actual_tenant varchar;
	return_value int[];
BEGIN
	select version into event_version from public.mt_streams where id = stream;
	if event_version IS NULL then
		event_version = 0;
		insert into public.mt_streams (id, type, version, timestamp, tenant_id) values (stream, stream_type, 0, now(), tenantid);
    else
        if tenantid IS NOT NULL then
            select tenant_id into actual_tenant from public.mt_streams where id = stream;
            if actual_tenant != tenantid then
                RAISE EXCEPTION 'The tenantid does not match the existing stream';
            end if;
        end if;
	end if;

	index := 1;
	return_value := ARRAY[event_version + array_length(event_ids, 1)];

	foreach event_id in ARRAY event_ids
	loop
	    seq := nextval('public.mt_events_sequence');
		return_value := array_append(return_value, seq);

	    event_version := event_version + 1;
		event_type = event_types[index];
		body = bodies[index];

		insert into public.mt_events
			(seq_id, id, stream_id, version, data, type, tenant_id, timestamp, mt_dotnet_type, is_archived, causation_id, correlation_id, headers)
		values
			(seq, event_id, stream, event_version, body, event_type, tenantid, (now() at time zone 'utc'), dotnet_types[index], FALSE, causation_ids[index], correlation_ids[index], headers[index]);

		index := index + 1;
	end loop;

	update public.mt_streams set version = event_version, timestamp = now() where id = stream;

	return return_value;
END
$$;


--
-- Name: mt_safe_unaccent(boolean, text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_safe_unaccent(use_unaccent boolean, word text) RETURNS text
    LANGUAGE plpgsql IMMUTABLE STRICT
    AS $$
BEGIN
IF use_unaccent THEN
    RETURN unaccent(word);
ELSE
    RETURN word;
END IF;
END;
$$;


--
-- Name: mt_update_addresskafkaconsumeroffset(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_update_addresskafkaconsumeroffset(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
  UPDATE public.mt_doc_addresskafkaconsumeroffset SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where id = docId;

  SELECT mt_version FROM public.mt_doc_addresskafkaconsumeroffset into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_update_beheerkbosynchistoriekgebeurtenisdocument(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_update_beheerkbosynchistoriekgebeurtenisdocument(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
  UPDATE public.mt_doc_beheerkbosynchistoriekgebeurtenisdocument SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where id = docId;

  SELECT mt_version FROM public.mt_doc_beheerkbosynchistoriekgebeurtenisdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_update_beheerverenigingdetaildocument(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_update_beheerverenigingdetaildocument(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
  UPDATE public.mt_doc_beheerverenigingdetaildocument SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where id = docId;

  SELECT mt_version FROM public.mt_doc_beheerverenigingdetaildocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_update_beheervereniginghistoriekdocument(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_update_beheervereniginghistoriekdocument(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
  UPDATE public.mt_doc_beheervereniginghistoriekdocument SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where id = docId;

  SELECT mt_version FROM public.mt_doc_beheervereniginghistoriekdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_update_deadletterevent(jsonb, character varying, uuid, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_update_deadletterevent(doc jsonb, docdotnettype character varying, docid uuid, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
  UPDATE public.mt_doc_deadletterevent SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where id = docId;

  SELECT mt_version FROM public.mt_doc_deadletterevent into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_update_envelope(jsonb, character varying, uuid, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_update_envelope(doc jsonb, docdotnettype character varying, docid uuid, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
  UPDATE public.mt_doc_envelope SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where id = docId;

  SELECT mt_version FROM public.mt_doc_envelope into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_update_geotagmigration(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_update_geotagmigration(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
  UPDATE public.mt_doc_geotagmigration SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where id = docId;

  SELECT mt_version FROM public.mt_doc_geotagmigration into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_update_locatielookupdocument(jsonb, character varying, character varying, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_update_locatielookupdocument(doc jsonb, docdotnettype character varying, docid character varying, revision integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN
  if revision <= 1 then
    SELECT mt_version FROM public.mt_doc_locatielookupdocument into current_version WHERE id = docId ;
    if current_version is not null then
      revision = current_version + 1;
    end if;
  end if;

  UPDATE public.mt_doc_locatielookupdocument SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp() where revision > public.mt_doc_locatielookupdocument.mt_version and id = docId;

  SELECT mt_version FROM public.mt_doc_locatielookupdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_update_locatiezonderadresmatchdocument(jsonb, character varying, character varying, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_update_locatiezonderadresmatchdocument(doc jsonb, docdotnettype character varying, docid character varying, revision integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN
  if revision <= 1 then
    SELECT mt_version FROM public.mt_doc_locatiezonderadresmatchdocument into current_version WHERE id = docId ;
    if current_version is not null then
      revision = current_version + 1;
    end if;
  end if;

  UPDATE public.mt_doc_locatiezonderadresmatchdocument SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp() where revision > public.mt_doc_locatiezonderadresmatchdocument.mt_version and id = docId;

  SELECT mt_version FROM public.mt_doc_locatiezonderadresmatchdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_update_magdacallreference(jsonb, character varying, uuid, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_update_magdacallreference(doc jsonb, docdotnettype character varying, docid uuid, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
  UPDATE public.mt_doc_magdacallreference SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where id = docId;

  SELECT mt_version FROM public.mt_doc_magdacallreference into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_update_postalnutslauinfo(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_update_postalnutslauinfo(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
  UPDATE public.mt_doc_postalnutslauinfo SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where id = docId;

  SELECT mt_version FROM public.mt_doc_postalnutslauinfo into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_update_powerbiexportdocument(jsonb, character varying, character varying, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_update_powerbiexportdocument(doc jsonb, docdotnettype character varying, docid character varying, revision integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN
  if revision <= 1 then
    SELECT mt_version FROM public.mt_doc_powerbiexportdocument into current_version WHERE id = docId ;
    if current_version is not null then
      revision = current_version + 1;
    end if;
  end if;

  UPDATE public.mt_doc_powerbiexportdocument SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp() where revision > public.mt_doc_powerbiexportdocument.mt_version and id = docId;

  SELECT mt_version FROM public.mt_doc_powerbiexportdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_update_publiekverenigingdetaildocument(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_update_publiekverenigingdetaildocument(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
  UPDATE public.mt_doc_publiekverenigingdetaildocument SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where id = docId;

  SELECT mt_version FROM public.mt_doc_publiekverenigingdetaildocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_update_publiekverenigingsequencedocument(jsonb, character varying, character varying, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_update_publiekverenigingsequencedocument(doc jsonb, docdotnettype character varying, docid character varying, revision integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN
  if revision <= 1 then
    SELECT mt_version FROM public.mt_doc_publiekverenigingsequencedocument into current_version WHERE id = docId ;
    if current_version is not null then
      revision = current_version + 1;
    end if;
  end if;

  UPDATE public.mt_doc_publiekverenigingsequencedocument SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp() where revision > public.mt_doc_publiekverenigingsequencedocument.mt_version and id = docId;

  SELECT mt_version FROM public.mt_doc_publiekverenigingsequencedocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_update_settingoverride(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_update_settingoverride(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
  UPDATE public.mt_doc_settingoverride SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where id = docId;

  SELECT mt_version FROM public.mt_doc_settingoverride into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_update_verenigingdocument(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_update_verenigingdocument(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
  UPDATE public.mt_doc_verenigingdocument SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where id = docId;

  SELECT mt_version FROM public.mt_doc_verenigingdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_update_verenigingenperinszdocument(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_update_verenigingenperinszdocument(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
  UPDATE public.mt_doc_verenigingenperinszdocument SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where id = docId;

  SELECT mt_version FROM public.mt_doc_verenigingenperinszdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_update_verenigingstate(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_update_verenigingstate(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
  UPDATE public.mt_doc_verenigingstate SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where id = docId;

  SELECT mt_version FROM public.mt_doc_verenigingstate into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_upsert_addresskafkaconsumeroffset(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_upsert_addresskafkaconsumeroffset(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
INSERT INTO public.mt_doc_addresskafkaconsumeroffset ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_addresskafkaconsumeroffset into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_upsert_beheerkbosynchistoriekgebeurtenisdocument(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_upsert_beheerkbosynchistoriekgebeurtenisdocument(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
INSERT INTO public.mt_doc_beheerkbosynchistoriekgebeurtenisdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_beheerkbosynchistoriekgebeurtenisdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_upsert_beheerverenigingdetaildocument(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_upsert_beheerverenigingdetaildocument(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
INSERT INTO public.mt_doc_beheerverenigingdetaildocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_beheerverenigingdetaildocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_upsert_beheervereniginghistoriekdocument(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_upsert_beheervereniginghistoriekdocument(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
INSERT INTO public.mt_doc_beheervereniginghistoriekdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_beheervereniginghistoriekdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_upsert_deadletterevent(jsonb, character varying, uuid, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_upsert_deadletterevent(doc jsonb, docdotnettype character varying, docid uuid, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
INSERT INTO public.mt_doc_deadletterevent ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_deadletterevent into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_upsert_envelope(jsonb, character varying, uuid, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_upsert_envelope(doc jsonb, docdotnettype character varying, docid uuid, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
INSERT INTO public.mt_doc_envelope ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_envelope into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_upsert_geotagmigration(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_upsert_geotagmigration(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
INSERT INTO public.mt_doc_geotagmigration ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_geotagmigration into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_upsert_locatielookupdocument(jsonb, character varying, character varying, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_upsert_locatielookupdocument(doc jsonb, docdotnettype character varying, docid character varying, revision integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN

SELECT mt_version into current_version FROM public.mt_doc_locatielookupdocument WHERE id = docId ;
if revision = 0 then
  if current_version is not null then
    revision = current_version + 1;
  else
    revision = 1;
  end if;
else
  if current_version is not null then
    if current_version >= revision then
      return 0;
    end if;
  end if;
end if;

INSERT INTO public.mt_doc_locatielookupdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, revision, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp() where revision > public.mt_doc_locatielookupdocument.mt_version;

  SELECT mt_version into final_version FROM public.mt_doc_locatielookupdocument WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_upsert_locatiezonderadresmatchdocument(jsonb, character varying, character varying, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_upsert_locatiezonderadresmatchdocument(doc jsonb, docdotnettype character varying, docid character varying, revision integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN

SELECT mt_version into current_version FROM public.mt_doc_locatiezonderadresmatchdocument WHERE id = docId ;
if revision = 0 then
  if current_version is not null then
    revision = current_version + 1;
  else
    revision = 1;
  end if;
else
  if current_version is not null then
    if current_version >= revision then
      return 0;
    end if;
  end if;
end if;

INSERT INTO public.mt_doc_locatiezonderadresmatchdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, revision, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp() where revision > public.mt_doc_locatiezonderadresmatchdocument.mt_version;

  SELECT mt_version into final_version FROM public.mt_doc_locatiezonderadresmatchdocument WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_upsert_magdacallreference(jsonb, character varying, uuid, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_upsert_magdacallreference(doc jsonb, docdotnettype character varying, docid uuid, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
INSERT INTO public.mt_doc_magdacallreference ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_magdacallreference into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_upsert_postalnutslauinfo(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_upsert_postalnutslauinfo(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
INSERT INTO public.mt_doc_postalnutslauinfo ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_postalnutslauinfo into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_upsert_powerbiexportdocument(jsonb, character varying, character varying, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_upsert_powerbiexportdocument(doc jsonb, docdotnettype character varying, docid character varying, revision integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN

SELECT mt_version into current_version FROM public.mt_doc_powerbiexportdocument WHERE id = docId ;
if revision = 0 then
  if current_version is not null then
    revision = current_version + 1;
  else
    revision = 1;
  end if;
else
  if current_version is not null then
    if current_version >= revision then
      return 0;
    end if;
  end if;
end if;

INSERT INTO public.mt_doc_powerbiexportdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, revision, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp() where revision > public.mt_doc_powerbiexportdocument.mt_version;

  SELECT mt_version into final_version FROM public.mt_doc_powerbiexportdocument WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_upsert_publiekverenigingdetaildocument(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_upsert_publiekverenigingdetaildocument(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
INSERT INTO public.mt_doc_publiekverenigingdetaildocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_publiekverenigingdetaildocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_upsert_publiekverenigingsequencedocument(jsonb, character varying, character varying, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_upsert_publiekverenigingsequencedocument(doc jsonb, docdotnettype character varying, docid character varying, revision integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN

SELECT mt_version into current_version FROM public.mt_doc_publiekverenigingsequencedocument WHERE id = docId ;
if revision = 0 then
  if current_version is not null then
    revision = current_version + 1;
  else
    revision = 1;
  end if;
else
  if current_version is not null then
    if current_version >= revision then
      return 0;
    end if;
  end if;
end if;

INSERT INTO public.mt_doc_publiekverenigingsequencedocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, revision, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp() where revision > public.mt_doc_publiekverenigingsequencedocument.mt_version;

  SELECT mt_version into final_version FROM public.mt_doc_publiekverenigingsequencedocument WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_upsert_settingoverride(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_upsert_settingoverride(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
INSERT INTO public.mt_doc_settingoverride ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_settingoverride into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_upsert_verenigingdocument(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_upsert_verenigingdocument(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
INSERT INTO public.mt_doc_verenigingdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_verenigingdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_upsert_verenigingenperinszdocument(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_upsert_verenigingenperinszdocument(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
INSERT INTO public.mt_doc_verenigingenperinszdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_verenigingenperinszdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


--
-- Name: mt_upsert_verenigingstate(jsonb, character varying, character varying, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.mt_upsert_verenigingstate(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
INSERT INTO public.mt_doc_verenigingstate ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_verenigingstate into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: mt_doc_addresskafkaconsumeroffset; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_doc_addresskafkaconsumeroffset (
    id character varying NOT NULL,
    data jsonb NOT NULL,
    mt_last_modified timestamp with time zone DEFAULT transaction_timestamp(),
    mt_version uuid DEFAULT (md5(((random())::text || (clock_timestamp())::text)))::uuid NOT NULL,
    mt_dotnet_type character varying
);


--
-- Name: mt_doc_beheerkbosynchistoriekgebeurtenisdocument; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_doc_beheerkbosynchistoriekgebeurtenisdocument (
    id character varying NOT NULL,
    data jsonb NOT NULL,
    mt_last_modified timestamp with time zone DEFAULT transaction_timestamp(),
    mt_version uuid DEFAULT (md5(((random())::text || (clock_timestamp())::text)))::uuid NOT NULL,
    mt_dotnet_type character varying
);


--
-- Name: mt_doc_beheerverenigingdetaildocument; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_doc_beheerverenigingdetaildocument (
    id character varying NOT NULL,
    data jsonb NOT NULL,
    mt_last_modified timestamp with time zone DEFAULT transaction_timestamp(),
    mt_version uuid DEFAULT (md5(((random())::text || (clock_timestamp())::text)))::uuid NOT NULL,
    mt_dotnet_type character varying,
    mt_deleted boolean DEFAULT false,
    mt_deleted_at timestamp with time zone
);


--
-- Name: mt_doc_beheervereniginghistoriekdocument; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_doc_beheervereniginghistoriekdocument (
    id character varying NOT NULL,
    data jsonb NOT NULL,
    mt_last_modified timestamp with time zone DEFAULT transaction_timestamp(),
    mt_version uuid DEFAULT (md5(((random())::text || (clock_timestamp())::text)))::uuid NOT NULL,
    mt_dotnet_type character varying
);


--
-- Name: mt_doc_deadletterevent; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_doc_deadletterevent (
    id uuid NOT NULL,
    data jsonb NOT NULL,
    mt_last_modified timestamp with time zone DEFAULT transaction_timestamp(),
    mt_version uuid DEFAULT (md5(((random())::text || (clock_timestamp())::text)))::uuid NOT NULL,
    mt_dotnet_type character varying
);


--
-- Name: mt_doc_envelope; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_doc_envelope (
    id uuid NOT NULL,
    data jsonb NOT NULL,
    mt_last_modified timestamp with time zone DEFAULT transaction_timestamp(),
    mt_version uuid DEFAULT (md5(((random())::text || (clock_timestamp())::text)))::uuid NOT NULL,
    mt_dotnet_type character varying
);


--
-- Name: mt_doc_geotagmigration; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_doc_geotagmigration (
    id character varying NOT NULL,
    data jsonb NOT NULL,
    mt_last_modified timestamp with time zone DEFAULT transaction_timestamp(),
    mt_version uuid DEFAULT (md5(((random())::text || (clock_timestamp())::text)))::uuid NOT NULL,
    mt_dotnet_type character varying
);


--
-- Name: mt_doc_locatielookupdocument; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_doc_locatielookupdocument (
    id character varying NOT NULL,
    data jsonb NOT NULL,
    mt_last_modified timestamp with time zone DEFAULT transaction_timestamp(),
    mt_dotnet_type character varying,
    mt_version integer DEFAULT 0 NOT NULL
);


--
-- Name: mt_doc_locatiezonderadresmatchdocument; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_doc_locatiezonderadresmatchdocument (
    id character varying NOT NULL,
    data jsonb NOT NULL,
    mt_last_modified timestamp with time zone DEFAULT transaction_timestamp(),
    mt_dotnet_type character varying,
    mt_version integer DEFAULT 0 NOT NULL
);


--
-- Name: mt_doc_magdacallreference; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_doc_magdacallreference (
    id uuid NOT NULL,
    data jsonb NOT NULL,
    mt_last_modified timestamp with time zone DEFAULT transaction_timestamp(),
    mt_version uuid DEFAULT (md5(((random())::text || (clock_timestamp())::text)))::uuid NOT NULL,
    mt_dotnet_type character varying
);


--
-- Name: mt_doc_postalnutslauinfo; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_doc_postalnutslauinfo (
    id character varying NOT NULL,
    data jsonb NOT NULL,
    mt_last_modified timestamp with time zone DEFAULT transaction_timestamp(),
    mt_version uuid DEFAULT (md5(((random())::text || (clock_timestamp())::text)))::uuid NOT NULL,
    mt_dotnet_type character varying
);


--
-- Name: mt_doc_powerbiexportdocument; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_doc_powerbiexportdocument (
    id character varying NOT NULL,
    data jsonb NOT NULL,
    mt_last_modified timestamp with time zone DEFAULT transaction_timestamp(),
    mt_dotnet_type character varying,
    mt_version integer DEFAULT 0 NOT NULL
);


--
-- Name: mt_doc_publiekverenigingdetaildocument; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_doc_publiekverenigingdetaildocument (
    id character varying NOT NULL,
    data jsonb NOT NULL,
    mt_last_modified timestamp with time zone DEFAULT transaction_timestamp(),
    mt_version uuid DEFAULT (md5(((random())::text || (clock_timestamp())::text)))::uuid NOT NULL,
    mt_dotnet_type character varying,
    mt_deleted boolean DEFAULT false,
    mt_deleted_at timestamp with time zone
);


--
-- Name: mt_doc_publiekverenigingsequencedocument; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_doc_publiekverenigingsequencedocument (
    id character varying NOT NULL,
    data jsonb NOT NULL,
    mt_last_modified timestamp with time zone DEFAULT transaction_timestamp(),
    mt_dotnet_type character varying,
    mt_version integer DEFAULT 0 NOT NULL
);


--
-- Name: mt_doc_settingoverride; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_doc_settingoverride (
    id character varying NOT NULL,
    data jsonb NOT NULL,
    mt_last_modified timestamp with time zone DEFAULT transaction_timestamp(),
    mt_version uuid DEFAULT (md5(((random())::text || (clock_timestamp())::text)))::uuid NOT NULL,
    mt_dotnet_type character varying
);


--
-- Name: mt_doc_verenigingdocument; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_doc_verenigingdocument (
    id character varying NOT NULL,
    data jsonb NOT NULL,
    mt_last_modified timestamp with time zone DEFAULT transaction_timestamp(),
    mt_version uuid DEFAULT (md5(((random())::text || (clock_timestamp())::text)))::uuid NOT NULL,
    mt_dotnet_type character varying,
    mt_deleted boolean DEFAULT false,
    mt_deleted_at timestamp with time zone
);


--
-- Name: mt_doc_verenigingenperinszdocument; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_doc_verenigingenperinszdocument (
    id character varying NOT NULL,
    data jsonb NOT NULL,
    mt_last_modified timestamp with time zone DEFAULT transaction_timestamp(),
    mt_version uuid DEFAULT (md5(((random())::text || (clock_timestamp())::text)))::uuid NOT NULL,
    mt_dotnet_type character varying
);


--
-- Name: mt_doc_verenigingstate; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_doc_verenigingstate (
    id character varying NOT NULL,
    data jsonb NOT NULL,
    mt_last_modified timestamp with time zone DEFAULT transaction_timestamp(),
    mt_version uuid DEFAULT (md5(((random())::text || (clock_timestamp())::text)))::uuid NOT NULL,
    mt_dotnet_type character varying
);


--
-- Name: mt_event_progression; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_event_progression (
    name character varying NOT NULL,
    last_seq_id bigint,
    last_updated timestamp with time zone DEFAULT transaction_timestamp()
);


--
-- Name: mt_events; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_events (
    seq_id bigint NOT NULL,
    id uuid NOT NULL,
    stream_id character varying,
    version bigint NOT NULL,
    data jsonb NOT NULL,
    type character varying(500) NOT NULL,
    "timestamp" timestamp with time zone DEFAULT '2024-08-05 15:14:15.225004+00'::timestamp with time zone NOT NULL,
    tenant_id character varying DEFAULT '*DEFAULT*'::character varying,
    mt_dotnet_type character varying,
    correlation_id character varying,
    causation_id character varying,
    headers jsonb,
    is_archived boolean DEFAULT false
);


--
-- Name: mt_events_sequence; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.mt_events_sequence
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: mt_events_sequence; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.mt_events_sequence OWNED BY public.mt_events.seq_id;


--
-- Name: mt_streams; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mt_streams (
    id character varying NOT NULL,
    type character varying,
    version bigint,
    "timestamp" timestamp with time zone DEFAULT now() NOT NULL,
    snapshot jsonb,
    snapshot_version integer,
    created timestamp with time zone DEFAULT now() NOT NULL,
    tenant_id character varying DEFAULT '*DEFAULT*'::character varying,
    is_archived boolean DEFAULT false
);


--
-- Name: mt_vcodesequence; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.mt_vcodesequence
    START WITH 1001
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: wolverine_control_queue; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.wolverine_control_queue (
    id uuid NOT NULL,
    message_type character varying NOT NULL,
    node_id uuid NOT NULL,
    body bytea NOT NULL,
    posted timestamp with time zone DEFAULT now() NOT NULL,
    expires timestamp with time zone
);


--
-- Name: wolverine_dead_letters; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.wolverine_dead_letters (
    id uuid NOT NULL,
    execution_time timestamp with time zone,
    body bytea NOT NULL,
    message_type character varying NOT NULL,
    received_at character varying,
    source character varying,
    exception_type character varying,
    exception_message character varying,
    sent_at timestamp with time zone,
    replayable boolean
);


--
-- Name: wolverine_incoming_envelopes; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.wolverine_incoming_envelopes (
    id uuid NOT NULL,
    status character varying NOT NULL,
    owner_id integer NOT NULL,
    execution_time timestamp with time zone,
    attempts integer DEFAULT 0,
    body bytea NOT NULL,
    message_type character varying NOT NULL,
    received_at character varying,
    keep_until timestamp with time zone
);


--
-- Name: wolverine_node_assignments; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.wolverine_node_assignments (
    id character varying NOT NULL,
    node_id uuid,
    started timestamp with time zone DEFAULT now() NOT NULL
);


--
-- Name: wolverine_node_records; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.wolverine_node_records (
    id integer NOT NULL,
    node_number integer NOT NULL,
    event_name character varying NOT NULL,
    "timestamp" timestamp with time zone DEFAULT now() NOT NULL,
    description character varying
);


--
-- Name: wolverine_node_records_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.wolverine_node_records_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: wolverine_node_records_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.wolverine_node_records_id_seq OWNED BY public.wolverine_node_records.id;


--
-- Name: wolverine_nodes; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.wolverine_nodes (
    id uuid NOT NULL,
    node_number integer NOT NULL,
    description character varying NOT NULL,
    uri character varying NOT NULL,
    started timestamp with time zone DEFAULT now() NOT NULL,
    health_check timestamp with time zone DEFAULT now() NOT NULL,
    capabilities text[],
    version character varying
);


--
-- Name: wolverine_nodes_node_number_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.wolverine_nodes_node_number_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: wolverine_nodes_node_number_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.wolverine_nodes_node_number_seq OWNED BY public.wolverine_nodes.node_number;


--
-- Name: wolverine_outgoing_envelopes; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.wolverine_outgoing_envelopes (
    id uuid NOT NULL,
    owner_id integer NOT NULL,
    destination character varying NOT NULL,
    deliver_by timestamp with time zone,
    body bytea NOT NULL,
    attempts integer DEFAULT 0,
    message_type character varying NOT NULL
);


--
-- Name: wolverine_queue_aanvaard_dubbele_vereniging_queue; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.wolverine_queue_aanvaard_dubbele_vereniging_queue (
    id uuid NOT NULL,
    body bytea NOT NULL,
    message_type character varying NOT NULL,
    keep_until timestamp with time zone,
    "timestamp" timestamp with time zone DEFAULT (now() AT TIME ZONE 'utc'::text)
);


--
-- Name: wolverine_queue_aanvaard_dubbele_vereniging_queue_scheduled; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.wolverine_queue_aanvaard_dubbele_vereniging_queue_scheduled (
    id uuid NOT NULL,
    body bytea NOT NULL,
    message_type character varying NOT NULL,
    execution_time timestamp with time zone NOT NULL,
    keep_until timestamp with time zone,
    "timestamp" timestamp with time zone DEFAULT (now() AT TIME ZONE 'utc'::text)
);


--
-- Name: wolverine_queue_aanvaard_dubbele_vereniging_queue; Type: TABLE; Schema: wolverine_queues; Owner: -
--

CREATE TABLE wolverine_queues.wolverine_queue_aanvaard_dubbele_vereniging_queue (
    id uuid NOT NULL,
    body bytea NOT NULL,
    message_type character varying NOT NULL,
    keep_until timestamp with time zone,
    "timestamp" timestamp with time zone DEFAULT (now() AT TIME ZONE 'utc'::text)
);


--
-- Name: wolverine_queue_aanvaard_dubbele_vereniging_queue_scheduled; Type: TABLE; Schema: wolverine_queues; Owner: -
--

CREATE TABLE wolverine_queues.wolverine_queue_aanvaard_dubbele_vereniging_queue_scheduled (
    id uuid NOT NULL,
    body bytea NOT NULL,
    message_type character varying NOT NULL,
    execution_time timestamp with time zone NOT NULL,
    keep_until timestamp with time zone,
    "timestamp" timestamp with time zone DEFAULT (now() AT TIME ZONE 'utc'::text)
);


--
-- Name: wolverine_queue_nachtelijke_adressync_queue; Type: TABLE; Schema: wolverine_queues; Owner: -
--

CREATE TABLE wolverine_queues.wolverine_queue_nachtelijke_adressync_queue (
    id uuid NOT NULL,
    body bytea NOT NULL,
    message_type character varying NOT NULL,
    keep_until timestamp with time zone,
    "timestamp" timestamp with time zone DEFAULT (now() AT TIME ZONE 'utc'::text)
);


--
-- Name: wolverine_queue_nachtelijke_adressync_queue_scheduled; Type: TABLE; Schema: wolverine_queues; Owner: -
--

CREATE TABLE wolverine_queues.wolverine_queue_nachtelijke_adressync_queue_scheduled (
    id uuid NOT NULL,
    body bytea NOT NULL,
    message_type character varying NOT NULL,
    execution_time timestamp with time zone NOT NULL,
    keep_until timestamp with time zone,
    "timestamp" timestamp with time zone DEFAULT (now() AT TIME ZONE 'utc'::text)
);


--
-- Name: wolverine_node_records id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.wolverine_node_records ALTER COLUMN id SET DEFAULT nextval('public.wolverine_node_records_id_seq'::regclass);


--
-- Name: wolverine_nodes node_number; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.wolverine_nodes ALTER COLUMN node_number SET DEFAULT nextval('public.wolverine_nodes_node_number_seq'::regclass);


--
-- Name: mt_event_progression pk_mt_event_progression; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_event_progression
    ADD CONSTRAINT pk_mt_event_progression PRIMARY KEY (name);


--
-- Name: mt_doc_addresskafkaconsumeroffset pkey_mt_doc_addresskafkaconsumeroffset_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_doc_addresskafkaconsumeroffset
    ADD CONSTRAINT pkey_mt_doc_addresskafkaconsumeroffset_id PRIMARY KEY (id);


--
-- Name: mt_doc_beheerkbosynchistoriekgebeurtenisdocument pkey_mt_doc_beheerkbosynchistoriekgebeurtenisdocument_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_doc_beheerkbosynchistoriekgebeurtenisdocument
    ADD CONSTRAINT pkey_mt_doc_beheerkbosynchistoriekgebeurtenisdocument_id PRIMARY KEY (id);


--
-- Name: mt_doc_beheerverenigingdetaildocument pkey_mt_doc_beheerverenigingdetaildocument_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_doc_beheerverenigingdetaildocument
    ADD CONSTRAINT pkey_mt_doc_beheerverenigingdetaildocument_id PRIMARY KEY (id);


--
-- Name: mt_doc_beheervereniginghistoriekdocument pkey_mt_doc_beheervereniginghistoriekdocument_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_doc_beheervereniginghistoriekdocument
    ADD CONSTRAINT pkey_mt_doc_beheervereniginghistoriekdocument_id PRIMARY KEY (id);


--
-- Name: mt_doc_deadletterevent pkey_mt_doc_deadletterevent_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_doc_deadletterevent
    ADD CONSTRAINT pkey_mt_doc_deadletterevent_id PRIMARY KEY (id);


--
-- Name: mt_doc_envelope pkey_mt_doc_envelope_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_doc_envelope
    ADD CONSTRAINT pkey_mt_doc_envelope_id PRIMARY KEY (id);


--
-- Name: mt_doc_geotagmigration pkey_mt_doc_geotagmigration_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_doc_geotagmigration
    ADD CONSTRAINT pkey_mt_doc_geotagmigration_id PRIMARY KEY (id);


--
-- Name: mt_doc_locatielookupdocument pkey_mt_doc_locatielookupdocument_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_doc_locatielookupdocument
    ADD CONSTRAINT pkey_mt_doc_locatielookupdocument_id PRIMARY KEY (id);


--
-- Name: mt_doc_locatiezonderadresmatchdocument pkey_mt_doc_locatiezonderadresmatchdocument_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_doc_locatiezonderadresmatchdocument
    ADD CONSTRAINT pkey_mt_doc_locatiezonderadresmatchdocument_id PRIMARY KEY (id);


--
-- Name: mt_doc_magdacallreference pkey_mt_doc_magdacallreference_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_doc_magdacallreference
    ADD CONSTRAINT pkey_mt_doc_magdacallreference_id PRIMARY KEY (id);


--
-- Name: mt_doc_postalnutslauinfo pkey_mt_doc_postalnutslauinfo_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_doc_postalnutslauinfo
    ADD CONSTRAINT pkey_mt_doc_postalnutslauinfo_id PRIMARY KEY (id);


--
-- Name: mt_doc_powerbiexportdocument pkey_mt_doc_powerbiexportdocument_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_doc_powerbiexportdocument
    ADD CONSTRAINT pkey_mt_doc_powerbiexportdocument_id PRIMARY KEY (id);


--
-- Name: mt_doc_publiekverenigingdetaildocument pkey_mt_doc_publiekverenigingdetaildocument_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_doc_publiekverenigingdetaildocument
    ADD CONSTRAINT pkey_mt_doc_publiekverenigingdetaildocument_id PRIMARY KEY (id);


--
-- Name: mt_doc_publiekverenigingsequencedocument pkey_mt_doc_publiekverenigingsequencedocument_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_doc_publiekverenigingsequencedocument
    ADD CONSTRAINT pkey_mt_doc_publiekverenigingsequencedocument_id PRIMARY KEY (id);


--
-- Name: mt_doc_settingoverride pkey_mt_doc_settingoverride_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_doc_settingoverride
    ADD CONSTRAINT pkey_mt_doc_settingoverride_id PRIMARY KEY (id);


--
-- Name: mt_doc_verenigingdocument pkey_mt_doc_verenigingdocument_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_doc_verenigingdocument
    ADD CONSTRAINT pkey_mt_doc_verenigingdocument_id PRIMARY KEY (id);


--
-- Name: mt_doc_verenigingenperinszdocument pkey_mt_doc_verenigingenperinszdocument_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_doc_verenigingenperinszdocument
    ADD CONSTRAINT pkey_mt_doc_verenigingenperinszdocument_id PRIMARY KEY (id);


--
-- Name: mt_doc_verenigingstate pkey_mt_doc_verenigingstate_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_doc_verenigingstate
    ADD CONSTRAINT pkey_mt_doc_verenigingstate_id PRIMARY KEY (id);


--
-- Name: mt_events pkey_mt_events_seq_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_events
    ADD CONSTRAINT pkey_mt_events_seq_id PRIMARY KEY (seq_id);


--
-- Name: mt_streams pkey_mt_streams_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_streams
    ADD CONSTRAINT pkey_mt_streams_id PRIMARY KEY (id);


--
-- Name: wolverine_control_queue pkey_wolverine_control_queue_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.wolverine_control_queue
    ADD CONSTRAINT pkey_wolverine_control_queue_id PRIMARY KEY (id);


--
-- Name: wolverine_dead_letters pkey_wolverine_dead_letters_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.wolverine_dead_letters
    ADD CONSTRAINT pkey_wolverine_dead_letters_id PRIMARY KEY (id);


--
-- Name: wolverine_incoming_envelopes pkey_wolverine_incoming_envelopes_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.wolverine_incoming_envelopes
    ADD CONSTRAINT pkey_wolverine_incoming_envelopes_id PRIMARY KEY (id);


--
-- Name: wolverine_node_assignments pkey_wolverine_node_assignments_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.wolverine_node_assignments
    ADD CONSTRAINT pkey_wolverine_node_assignments_id PRIMARY KEY (id);


--
-- Name: wolverine_node_records pkey_wolverine_node_records_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.wolverine_node_records
    ADD CONSTRAINT pkey_wolverine_node_records_id PRIMARY KEY (id);


--
-- Name: wolverine_nodes pkey_wolverine_nodes_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.wolverine_nodes
    ADD CONSTRAINT pkey_wolverine_nodes_id PRIMARY KEY (id);


--
-- Name: wolverine_outgoing_envelopes pkey_wolverine_outgoing_envelopes_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.wolverine_outgoing_envelopes
    ADD CONSTRAINT pkey_wolverine_outgoing_envelopes_id PRIMARY KEY (id);


--
-- Name: wolverine_queue_aanvaard_dubbele_vereniging_queue pkey_wolverine_queue_aanvaard_dubbele_vereniging_queue_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.wolverine_queue_aanvaard_dubbele_vereniging_queue
    ADD CONSTRAINT pkey_wolverine_queue_aanvaard_dubbele_vereniging_queue_id PRIMARY KEY (id);


--
-- Name: wolverine_queue_aanvaard_dubbele_vereniging_queue_scheduled pkey_wolverine_queue_aanvaard_dubbele_vereniging_queue_schedule; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.wolverine_queue_aanvaard_dubbele_vereniging_queue_scheduled
    ADD CONSTRAINT pkey_wolverine_queue_aanvaard_dubbele_vereniging_queue_schedule PRIMARY KEY (id);


--
-- Name: wolverine_queue_aanvaard_dubbele_vereniging_queue pkey_wolverine_queue_aanvaard_dubbele_vereniging_queue_id; Type: CONSTRAINT; Schema: wolverine_queues; Owner: -
--

ALTER TABLE ONLY wolverine_queues.wolverine_queue_aanvaard_dubbele_vereniging_queue
    ADD CONSTRAINT pkey_wolverine_queue_aanvaard_dubbele_vereniging_queue_id PRIMARY KEY (id);


--
-- Name: wolverine_queue_aanvaard_dubbele_vereniging_queue_scheduled pkey_wolverine_queue_aanvaard_dubbele_vereniging_queue_schedule; Type: CONSTRAINT; Schema: wolverine_queues; Owner: -
--

ALTER TABLE ONLY wolverine_queues.wolverine_queue_aanvaard_dubbele_vereniging_queue_scheduled
    ADD CONSTRAINT pkey_wolverine_queue_aanvaard_dubbele_vereniging_queue_schedule PRIMARY KEY (id);


--
-- Name: wolverine_queue_nachtelijke_adressync_queue pkey_wolverine_queue_nachtelijke_adressync_queue_id; Type: CONSTRAINT; Schema: wolverine_queues; Owner: -
--

ALTER TABLE ONLY wolverine_queues.wolverine_queue_nachtelijke_adressync_queue
    ADD CONSTRAINT pkey_wolverine_queue_nachtelijke_adressync_queue_id PRIMARY KEY (id);


--
-- Name: wolverine_queue_nachtelijke_adressync_queue_scheduled pkey_wolverine_queue_nachtelijke_adressync_queue_scheduled_id; Type: CONSTRAINT; Schema: wolverine_queues; Owner: -
--

ALTER TABLE ONLY wolverine_queues.wolverine_queue_nachtelijke_adressync_queue_scheduled
    ADD CONSTRAINT pkey_wolverine_queue_nachtelijke_adressync_queue_scheduled_id PRIMARY KEY (id);


--
-- Name: idx_wolverine_queue_aanvaard_dubbele_vereniging_queue_scheduled; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_wolverine_queue_aanvaard_dubbele_vereniging_queue_scheduled ON public.wolverine_queue_aanvaard_dubbele_vereniging_queue_scheduled USING btree (execution_time);


--
-- Name: mt_doc_beheerverenigingdetaildocument_idx_mt_deleted; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX mt_doc_beheerverenigingdetaildocument_idx_mt_deleted ON public.mt_doc_beheerverenigingdetaildocument USING btree (mt_deleted);


--
-- Name: mt_doc_publiekverenigingdetaildocument_idx_mt_deleted; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX mt_doc_publiekverenigingdetaildocument_idx_mt_deleted ON public.mt_doc_publiekverenigingdetaildocument USING btree (mt_deleted);


--
-- Name: mt_doc_verenigingdocument_idx_mt_deleted; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX mt_doc_verenigingdocument_idx_mt_deleted ON public.mt_doc_verenigingdocument USING btree (mt_deleted);


--
-- Name: pk_mt_events_stream_and_version; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX pk_mt_events_stream_and_version ON public.mt_events USING btree (stream_id, version);


--
-- Name: idx_wolverine_queue_aanvaard_dubbele_vereniging_queue_scheduled; Type: INDEX; Schema: wolverine_queues; Owner: -
--

CREATE INDEX idx_wolverine_queue_aanvaard_dubbele_vereniging_queue_scheduled ON wolverine_queues.wolverine_queue_aanvaard_dubbele_vereniging_queue_scheduled USING btree (execution_time);


--
-- Name: idx_wolverine_queue_nachtelijke_adressync_queue_scheduled_execu; Type: INDEX; Schema: wolverine_queues; Owner: -
--

CREATE INDEX idx_wolverine_queue_nachtelijke_adressync_queue_scheduled_execu ON wolverine_queues.wolverine_queue_nachtelijke_adressync_queue_scheduled USING btree (execution_time);


--
-- Name: mt_events fkey_mt_events_stream_id; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mt_events
    ADD CONSTRAINT fkey_mt_events_stream_id FOREIGN KEY (stream_id) REFERENCES public.mt_streams(id) ON DELETE CASCADE;


--
-- Name: wolverine_node_assignments fkey_wolverine_node_assignments_node_id; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.wolverine_node_assignments
    ADD CONSTRAINT fkey_wolverine_node_assignments_node_id FOREIGN KEY (node_id) REFERENCES public.wolverine_nodes(id) ON DELETE CASCADE;


--
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: -
--

REVOKE USAGE ON SCHEMA public FROM PUBLIC;
GRANT ALL ON SCHEMA public TO PUBLIC;


--
-- PostgreSQL database dump complete
--

