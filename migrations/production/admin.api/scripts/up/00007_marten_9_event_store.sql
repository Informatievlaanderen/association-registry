CREATE OR REPLACE FUNCTION public.mt_jsonb_append_key_value(jsonb, text[], jsonb, boolean)
    RETURNS jsonb
    LANGUAGE plpgsql
AS $function$
DECLARE
    retval ALIAS FOR $1;
    location ALIAS FOR $2;
    val ALIAS FOR $3;
    if_not_exists ALIAS FOR $4;
    tmp_value jsonb;
    _key text;
BEGIN
    tmp_value = retval #> location;

    IF tmp_value IS NOT NULL AND jsonb_typeof(tmp_value) = 'object' THEN
        CASE
            WHEN NOT if_not_exists THEN
                retval = jsonb_set(retval, location, tmp_value || val, FALSE);
            WHEN NOT tmp_value ?| ARRAY(SELECT jsonb_object_keys(val)) THEN
                retval = jsonb_set(retval, location, tmp_value || val, FALSE);
            ELSE NULL;
        END CASE;
    END IF;
    RETURN retval;
END;
$function$;


CREATE OR REPLACE FUNCTION public.mt_jsonb_remove_key(jsonb, text[], text)
    RETURNS jsonb
    LANGUAGE plpgsql
AS $function$
DECLARE
    retval ALIAS FOR $1;
    location ALIAS FOR $2;
    _key ALIAS FOR $3;
    tmp_value jsonb;
    tmp_remove jsonb;
    patch_remove jsonb;
BEGIN
    tmp_value = retval #> location;
    IF tmp_value IS NOT NULL AND jsonb_typeof(tmp_value) = 'object' THEN
        RETURN jsonb_set(retval, location, tmp_value - _key, FALSE);
    END IF;
    RETURN retval;
END;
$function$;


DROP FUNCTION IF EXISTS public.mt_jsonb_patch(jsonb, jsonb) cascade;
CREATE OR REPLACE FUNCTION public.mt_jsonb_patch(jsonb, jsonb)
    RETURNS jsonb
    LANGUAGE plpgsql
AS $function$
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
            WHEN 'append_key_value' THEN
                retval = public.mt_jsonb_append_key_value(retval, patch_path, (patch->'key_value')::jsonb, FALSE);
            WHEN 'append_key_value_if_not_exists' THEN
                retval = public.mt_jsonb_append_key_value(retval, patch_path, (patch->'key_value')::jsonb, TRUE);
            WHEN 'insert' THEN
                retval = public.mt_jsonb_insert(retval, patch_path, (patch->'value')::jsonb, (patch->>'index')::integer, FALSE);
            WHEN 'insert_if_not_exists' THEN
                retval = public.mt_jsonb_insert(retval, patch_path, (patch->'value')::jsonb, (patch->>'index')::integer, TRUE, patch_expression);
            WHEN 'remove' THEN
                retval = public.mt_jsonb_remove(retval, patch_path, COALESCE(patch_expression, (patch->'value')::jsonb));
            WHEN 'remove_key' THEN
                retval = public.mt_jsonb_remove_key(retval, patch_path, (patch->>'key'));
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
$function$;


alter table public.mt_streams drop column snapshot;
alter table public.mt_streams drop column snapshot_version;
alter table public.mt_events add column bdata bytea NULL;
DROP FUNCTION IF EXISTS public.mt_quick_append_events(stream character varying, stream_type character varying, tenantid character varying, event_ids uuid[], event_types character varying[], dotnet_types character varying[], bodies jsonb[], causation_ids character varying[], correlation_ids character varying[], headers jsonb[]) cascade;

CREATE OR REPLACE FUNCTION public.mt_quick_append_events(stream varchar, stream_type varchar, tenantid varchar, event_ids uuid[], event_types varchar[], dotnet_types varchar[], bodies jsonb[], bdatas bytea[], causation_ids varchar[], correlation_ids varchar[], headers jsonb[], expected_version integer DEFAULT NULL::integer) RETURNS int[] AS $$
DECLARE
	event_version int;
	stream_is_archived boolean;
	event_type varchar;
	event_id uuid;
	body jsonb;
	index int;
	seq int;
    actual_tenant varchar;
	return_value int[];
BEGIN
    if expected_version IS NOT NULL then
        -- COALESCE turns the NULL we get for a brand-new stream into 0, so a
        -- FetchForWriting against a non-existent stream (which sets
        -- ExpectedVersionOnServer = 0) and a StartStream(id, version: 0) both
        -- land on the new-stream branch instead of mis-firing the guard.
        select version, is_archived into event_version, stream_is_archived from public.mt_streams where id = stream;
        if COALESCE(event_version, 0) != expected_version then
            RAISE EXCEPTION 'Stream version mismatch on ''%'': expected %, actual %', stream, expected_version, COALESCE(event_version, 0) USING ERRCODE = 'MT003';
        end if;
    else
        select version, is_archived into event_version, stream_is_archived from public.mt_streams where id = stream;
    end if;

	if event_version IS NULL then
		event_version = 0;
		insert into public.mt_streams (id, type, version, timestamp, tenant_id) values (stream, stream_type, 0, now(), tenantid);
    else
        if stream_is_archived then
            RAISE EXCEPTION 'Attempted to append event to archived stream with Id ''%''.', stream USING ERRCODE = 'MT001';
        end if;
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

		-- #4515 / #4578 / Phase 2: bdatas[index] carries the binary payload
		-- for events opted in to binary serialization (NULL otherwise).
		-- bodies[index] is the {} JSON placeholder for those events so the
		-- existing data jsonb NOT NULL constraint stays intact.
		insert into public.mt_events
			(seq_id, id, stream_id, version, data, bdata, type, tenant_id, timestamp, mt_dotnet_type, is_archived, causation_id, correlation_id, headers)
		values
			(seq, event_id, stream, event_version, body, bdatas[index], event_type, tenantid, (now() at time zone 'utc'), dotnet_types[index], FALSE, causation_ids[index], correlation_ids[index], headers[index]);

		index := index + 1;
	end loop;

	update public.mt_streams set version = event_version, timestamp = now() where id = stream;

	return return_value;
END
$$ LANGUAGE plpgsql;

