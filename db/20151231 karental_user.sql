--
-- PostgreSQL database dump
--

-- Dumped from database version 9.5rc1
-- Dumped by pg_dump version 9.5rc1

-- Started on 2015-12-31 10:08:17

SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 202 (class 3079 OID 12355)
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- TOC entry 2242 (class 0 OID 0)
-- Dependencies: 202
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET search_path = public, pg_catalog;

--
-- TOC entry 616 (class 1247 OID 16677)
-- Name: creds; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE creds AS (
	salt character varying(250),
	password bytea,
	password_answer bytea,
	answer_salt character varying(250),
	last_lockout timestamp with time zone
);


ALTER TYPE creds OWNER TO postgres;

--
-- TOC entry 619 (class 1247 OID 16680)
-- Name: profile_info; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE profile_info AS (
	property_name character varying(250),
	property_type character varying(250),
	property_value text
);


ALTER TYPE profile_info OWNER TO postgres;

--
-- TOC entry 613 (class 1247 OID 16674)
-- Name: user_record; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE user_record AS (
	user_id integer,
	user_name character varying(250),
	last_activity timestamp with time zone,
	created timestamp with time zone,
	email character varying(250),
	approved boolean,
	last_lockout timestamp with time zone,
	last_login timestamp with time zone,
	last_password_changed timestamp with time zone,
	password_question character varying(1000),
	comment text
);


ALTER TYPE user_record OWNER TO postgres;

--
-- TOC entry 231 (class 1255 OID 16705)
-- Name: assign_users_to_roles(character varying[], character varying[], character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION assign_users_to_roles(_users character varying[], _roles character varying[], _application_name character varying) RETURNS boolean
    LANGUAGE plpgsql
    AS $$begin
create temporary table usernames (username varchar(250) not null primary key) on commit drop;
create temporary table rolenames (rolename varchar(250) not null primary key) on commit drop;
-- Create the tables based off the arrays.
insert into usernames
	(
	username
	)
select distinct
	username
from
	unnest(_users) as username;

insert into rolenames
	(
	rolename
	)
select distinct
	rolename
from
	unnest(_roles) as rolename;

-- Per the role provider pattern, an exception is to be thrown if any of the role names or user names specified
-- do not exist for the given application name.
if exists (select null from usernames as un left outer join users as u on lower(u.application_name) = lower(_application_name) 
	and lower(u.user_name) = lower(un.username) where u.user_id is null)
	or exists (select null from rolenames as rn left outer join roles as r on lower(r.application_name) = lower(_application_name) 
		and lower(r.role_name) = lower(rn.rolename) where r.role_id is null) then
	raise exception 'At least one user name or role specified does not exist in the application scope.' using errcode='MSING';
end if;

-- Insert the records linking the users to the roles, excluding pre-existing relationships.
insert into users_roles
	(
	user_id,
	role_id
	)
select distinct
	u.user_id,
	r.role_id
from
	usernames as un
inner join
	users as u on lower(u.application_name) = lower(_application_name) and lower(u.user_name) = lower(un.username)
cross join
	rolenames as rn
inner join
	roles as r on lower(r.application_name) = lower(_application_name) and lower(r.role_name) = lower(rn.rolename)
where not exists(
	select null
	from users_roles
	where
		user_id = u.user_id
		and role_id = r.role_id
	);

return true;
end;
$$;


ALTER FUNCTION public.assign_users_to_roles(_users character varying[], _roles character varying[], _application_name character varying) OWNER TO postgres;

--
-- TOC entry 232 (class 1255 OID 16688)
-- Name: create_role(character varying, character varying, text); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION create_role(_role_name character varying, _application_name character varying, _role_description text) RETURNS integer
    LANGUAGE plpgsql
    AS $$
declare _role_id integer;
begin
	-- per the role provider pattern, need to throw an exception if the role exists already.
	if exists(select null from roles where application_name = _application_name and role_name = _role_name) then
		raise exception 'The role already exists in this application.' using errcode='DUPRL';
	end if;

	insert into roles
		(
		role_name,
		application_name,
		role_description
		)
	values
		(
		_role_name,
		_application_name,
		_role_description
		)
	returning role_id into _role_id;
	return _role_id;
end;
$$;


ALTER FUNCTION public.create_role(_role_name character varying, _application_name character varying, _role_description text) OWNER TO postgres;

--
-- TOC entry 233 (class 1255 OID 16689)
-- Name: create_user(character varying, character varying, character varying, boolean, boolean); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION create_user(_user_name character varying, _application_name character varying, _email character varying, _approved boolean, _email_is_unique boolean) RETURNS integer
    LANGUAGE plpgsql
    AS $$	declare userid integer;
	begin
		
	if _email_is_unique
		and exists(select null from users as u where lower(u.application_name) = lower(_application_name) and  lower(u.email) = lower(_email)) then
		raise exception 'The email address specified is already in use for this application, and email addresses are configured to be unique.' using errcode='DUPEM';
	end if;

	if exists(select null from users as u where lower(u.application_name) = lower(_application_name) and lower(u.user_name) = lower(_user_name)) then
		raise exception 'The user name specified is already in use for this application.' using errcode='DUPUN';
	end if;

	insert into users
		(
		user_name,
		application_name,
		email,
		approved
		)
	values
		(
		_user_name,
		_application_name,
		_email,
		_approved
		)
	returning user_id into userid;
		
	return userid;
	end;
	
$$;


ALTER FUNCTION public.create_user(_user_name character varying, _application_name character varying, _email character varying, _approved boolean, _email_is_unique boolean) OWNER TO postgres;

--
-- TOC entry 216 (class 1255 OID 16690)
-- Name: delete_role(character varying, character varying, boolean); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION delete_role(_role_name character varying, _application_name character varying, _throw_on_populated boolean) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
declare _role_id integer;
begin
	if _throw_on_populated and exists(
		select null from roles as r
		inner join users_roles as ur
		on ur.role_id = r.role_id
		where lower(r.application_name) = lower(_application_name) 
		and lower(r.role_name) = lower(_role_name)
		) then
		raise exception 'The specified role is populated; cannot delete.' using errcode='RLPOP';
	end if;

	if not exists(select null from roles where lower(application_name) = lower(_application_name) and lower(role_name) = lower(_role_name)) then
		raise exception 'The specified role does not exist.' using errcode='NOROL';
	end if;

	delete from roles
	where lower(application_name) = lower(_application_name)
		and lower(role_name) = lower(_role_name);
	return true;
end;
$$;


ALTER FUNCTION public.delete_role(_role_name character varying, _application_name character varying, _throw_on_populated boolean) OWNER TO postgres;

--
-- TOC entry 217 (class 1255 OID 16691)
-- Name: delete_user(character varying, character varying, boolean); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION delete_user(_user_name character varying, _application_name character varying, _delete_related boolean) RETURNS boolean
    LANGUAGE plpgsql
    AS $$begin
if _delete_related then
	delete from user_login_activity as ula
	using users as u
	where lower(u.application_name) = lower(_application_name)
		and lower(u.user_name) = lower(_user_name)
		and u.user_id = ula.user_id;
end if;
delete from users
where lower(application_name) = lower(_application_name )
	and lower(user_name) = lower(_user_name);
return true;
end;
$$;


ALTER FUNCTION public.delete_user(_user_name character varying, _application_name character varying, _delete_related boolean) OWNER TO postgres;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- TOC entry 188 (class 1259 OID 16635)
-- Name: roles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE roles (
    role_id integer NOT NULL,
    role_name character varying(250) NOT NULL,
    application_name character varying(250) NOT NULL,
    role_description text
);


ALTER TABLE roles OWNER TO postgres;

--
-- TOC entry 223 (class 1255 OID 16701)
-- Name: get_all_roles(character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION get_all_roles(_application_name character varying) RETURNS SETOF roles
    LANGUAGE plpgsql
    AS $$
begin
return query
select
	*
from
	roles
where
	lower(application_name) = lower(_application_name)
order by
	lower(role_name);
end;
$$;


ALTER FUNCTION public.get_all_roles(_application_name character varying) OWNER TO postgres;

--
-- TOC entry 234 (class 1255 OID 16681)
-- Name: get_all_users(character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION get_all_users(_application_name character varying) RETURNS SETOF user_record
    LANGUAGE plpgsql
    AS $$
begin
	return query
	select
		user_id,
		user_name,
		last_activity,
		created,
		email,
		approved,
		last_lockout,
		last_login,
		last_password_changed,
		password_question,
		comment
	from
		users
	where
		lower(application_name) = lower(_application_name)
	order by
		user_id asc;

end;
$$;


ALTER FUNCTION public.get_all_users(_application_name character varying) OWNER TO postgres;

--
-- TOC entry 241 (class 1255 OID 16720)
-- Name: get_number_of_users_online(integer, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION get_number_of_users_online(_session_timeout integer, _application_name character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
declare record_count integer;
begin

	select
		count(*) into record_count
	from
		users as u
	where
		u.application_name = _application_name
		and last_activity::timestamp with time zone + cast(_session_timeout || ' minutes' as interval) > current_timestamp;	

	return record_count;
end;
$$;


ALTER FUNCTION public.get_number_of_users_online(_session_timeout integer, _application_name character varying) OWNER TO postgres;

--
-- TOC entry 218 (class 1255 OID 16685)
-- Name: get_online_count(integer, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION get_online_count(_session_timeout integer, _application_name character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
begin
	return (
		select 
			count(*) 
		from 
			users
		where 
			lower(application_name) = lower(_application_name)
			and last_activity::time + cast(_session_timeout + ' minutes' as interval) < current_timestamp);
end;
$$;


ALTER FUNCTION public.get_online_count(_session_timeout integer, _application_name character varying) OWNER TO postgres;

--
-- TOC entry 224 (class 1255 OID 16700)
-- Name: get_roles_for_user(character varying, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION get_roles_for_user(_user_name character varying, _application_name character varying) RETURNS SETOF roles
    LANGUAGE plpgsql
    AS $$begin
return query
select
	r.*
from
	users as u
inner join
	users_roles as ur
	on ur.user_id = u.user_id
inner join
	roles as r
	on r.role_id = ur.role_id
where
	lower(u.user_name) = lower(_user_name)
	and lower(u.application_name) = lower(_application_name)
	and lower(r.application_name) = lower(_application_name);
end;$$;


ALTER FUNCTION public.get_roles_for_user(_user_name character varying, _application_name character varying) OWNER TO postgres;

--
-- TOC entry 215 (class 1255 OID 16687)
-- Name: get_user_by_id(integer, boolean); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION get_user_by_id(_user_id integer, _online boolean) RETURNS SETOF user_record
    LANGUAGE plpgsql
    AS $$
begin
if _online then
	return query
	update users
	set 
		last_activity = current_timestamp
	where 
		user_id = _user_id
	returning
		user_id,
		user_name,
		last_activity,
		created,
		email,
		approved,
		last_lockout,
		last_login,
		last_password_changed,
		password_question,
		comment;
else
	return query
	select
		user_id,
		user_name,
		last_activity,
		created,
		email,
		approved,
		last_lockout,
		last_login,
		last_password_changed,
		password_question,
		comment
	from
		users
	where
		user_id = _user_id;
end if;
end;
$$;


ALTER FUNCTION public.get_user_by_id(_user_id integer, _online boolean) OWNER TO postgres;

--
-- TOC entry 235 (class 1255 OID 16686)
-- Name: get_user_by_username(character varying, character varying, boolean); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION get_user_by_username(_user_name character varying, _application_name character varying, _online boolean) RETURNS SETOF user_record
    LANGUAGE plpgsql
    AS $$begin
if _online then
	return query
	update users
	set 
		last_activity = current_timestamp
	where 
		lower(application_name) = lower(_application_name)
		and lower(user_name) = lower(_user_name)
	returning
		user_id,
		user_name,
		last_activity,
		created,
		email,
		approved,
		last_lockout,
		last_login,
		last_password_changed,
		password_question,
		comment;
else
	return query
	select
		user_id,
		user_name,
		last_activity,
		created,
		email,
		approved,
		last_lockout,
		last_login,
		last_password_changed,
		password_question,
		comment
	from
		users
	where
		lower(application_name) = lower(_application_name)
		and lower(user_name) = lower(_user_name);
end if;
end;
$$;


ALTER FUNCTION public.get_user_by_username(_user_name character varying, _application_name character varying, _online boolean) OWNER TO postgres;

--
-- TOC entry 219 (class 1255 OID 16692)
-- Name: get_user_credentials(character varying, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION get_user_credentials(_user_name character varying, _application_name character varying) RETURNS SETOF creds
    LANGUAGE plpgsql
    AS $$begin
	return query
	select
		salt,
		password,
		password_answer,
		answer_salt,
		last_lockout
	from
		users
	where
		lower(application_name) = lower(_application_name)
		and lower(user_name) = lower(_user_name)
	limit 1;
end;
$$;


ALTER FUNCTION public.get_user_credentials(_user_name character varying, _application_name character varying) OWNER TO postgres;

--
-- TOC entry 236 (class 1255 OID 16693)
-- Name: get_user_name_by_email(character varying, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION get_user_name_by_email(_email character varying, _application_name character varying) RETURNS character varying
    LANGUAGE plpgsql
    AS $$
	declare username varchar(250);
begin
	select
		user_name
	from
		users
	where
		lower(application_name) = lower(_application_name)
		and lower(email) = lower(_email)
	limit 1
	into 
		username;
	return username;
end;$$;


ALTER FUNCTION public.get_user_name_by_email(_email character varying, _application_name character varying) OWNER TO postgres;

--
-- TOC entry 220 (class 1255 OID 16682)
-- Name: get_users_by_email(character varying, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION get_users_by_email(partial_email character varying, _application_name character varying) RETURNS SETOF user_record
    LANGUAGE plpgsql
    AS $$
	begin
	return query
	select
		user_id,
		user_name,
		last_activity,
		created,
		email,
		approved,
		last_lockout,
		last_login,
		last_password_changed,
		password_question,
		comment
	from
		users
	where
		lower(application_name) = lower(_application_name) 
		and email ilike '%' || partial_email || '%'
	order by
		user_id asc;
end;$$;


ALTER FUNCTION public.get_users_by_email(partial_email character varying, _application_name character varying) OWNER TO postgres;

--
-- TOC entry 221 (class 1255 OID 16683)
-- Name: get_users_by_username(character varying, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION get_users_by_username(partial_username character varying, _application_name character varying) RETURNS SETOF user_record
    LANGUAGE plpgsql
    AS $$
	begin
	return query
	select
		user_id,
		user_name,
		last_activity,
		created,
		email,
		approved,
		last_lockout,
		last_login,
		last_password_changed,
		password_question,
		comment
	from
		users
	where
		lower(application_name) = lower(_application_name)
		and user_name ilike '%' || partial_username || '%'
	order by
		user_id asc;
end;$$;


ALTER FUNCTION public.get_users_by_username(partial_username character varying, _application_name character varying) OWNER TO postgres;

--
-- TOC entry 237 (class 1255 OID 16699)
-- Name: get_users_in_role(character varying, character varying, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION get_users_in_role(_role_name character varying, _application_name character varying, _partial_username character varying) RETURNS SETOF user_record
    LANGUAGE plpgsql
    AS $$
	begin

if not exists (select null from roles where lower(application_name) = lower(_application_name) and lower(role_name) = lower(_role_name)) then
	raise exception 'The specified role does not exist.' using errcode='ROLNA';
end if;

return query
select
	u.user_id,
	u.user_name,
	u.last_activity,
	u.created,
	u.email,
	u.approved,
	u.last_lockout,
	u.last_login,
	u.last_password_changed,
	u.password_question,
	u.comment
from
	roles as r
inner join
	users_roles as ur
	on ur.role_id = r.role_id
inner join
	users as u
	on u.user_id = ur.user_id
where
	lower(r.role_name) = lower(_role_name)
	and lower(r.application_name) = lower(_application_name)
	and u.user_name ilike '%' || _partial_username || '%'
	and lower(u.application_name) = lower(_application_name)
order by
	u.user_name;
end;
$$;


ALTER FUNCTION public.get_users_in_role(_role_name character varying, _application_name character varying, _partial_username character varying) OWNER TO postgres;

--
-- TOC entry 222 (class 1255 OID 16684)
-- Name: get_users_online(integer, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION get_users_online(_session_timeout integer, _appliction_name character varying) RETURNS SETOF user_record
    LANGUAGE plpgsql
    AS $$
begin

	return query
	select
		user_id,
		user_name,
		last_activity,
		created,
		email,
		approved,
		last_lockout,
		last_login,
		last_password_changed,
		password_question,
		comment
	from
		users
	where
		application_name = _appliction_name
		and last_activity::timestamp with time zone + cast(_session_timeout || ' minutes' as interval) > current_timestamp;	
end;
$$;


ALTER FUNCTION public.get_users_online(_session_timeout integer, _appliction_name character varying) OWNER TO postgres;

--
-- TOC entry 230 (class 1255 OID 16706)
-- Name: purge_activity(bigint); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION purge_activity(olderthan bigint) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
begin
delete from user_login_activity
where
	"from" < now() - cast(olderthan || ' seconds' as interval);
return true;
end;
$$;


ALTER FUNCTION public.purge_activity(olderthan bigint) OWNER TO postgres;

--
-- TOC entry 226 (class 1255 OID 16694)
-- Name: record_login_event(character varying, character varying, character varying, boolean, integer, integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION record_login_event(_user_name character varying, _application_name character varying, _origin character varying, _success_indicator boolean, _attempt_window integer, _attempt_count integer) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
	declare userid integer;
begin
select
	user_id
from users as u
where lower(u.application_name) = lower(_application_name)
	and lower(u.user_name) = lower(_user_name)
limit 1 into userid;

insert into user_login_activity
	(
	"from",
	success,
	"user_id"
	)
values
	(
	_origin,
	_success_indicator,
	userid
	);

-- check for last x attempts if failure
if _success_indicator = false and _attempt_count <> 0 then
	if (select count(*) from 
		(select "success" from user_login_activity as ula
			where ula.user_id = userid
			and ula.when > current_timestamp - cast(_attempt_window || ' minutes' as interval)
			order by ula.when desc limit _attempt_count) as last_login_attempts
		where "success" = false) >= _attempt_count then

		update users
		set
			last_lockout = current_timestamp
		where
			user_id = userid;
	end if;
end if;

-- mark the success.
if _success_indicator = true then
	update users
	set
		last_login = current_timestamp
	where
		user_id = userid;
end if;

return true;
end;
$$;


ALTER FUNCTION public.record_login_event(_user_name character varying, _application_name character varying, _origin character varying, _success_indicator boolean, _attempt_window integer, _attempt_count integer) OWNER TO postgres;

--
-- TOC entry 239 (class 1255 OID 16703)
-- Name: remove_users_from_roles(character varying[], character varying[], character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION remove_users_from_roles(_users character varying[], _roles character varying[], _application_name character varying) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
	begin
create temporary table usernames (username varchar(250) not null primary key) on commit drop;
create temporary table rolenames (rolename varchar(250) not null primary key) on commit drop;
-- Create the tables based off the arrays.
insert into usernames
	(
	username
	)
select distinct
	username
from
	unnest(_users) as username;

insert into rolenames
	(
	rolename
	)
select distinct
	rolename
from
	unnest(_roles) as rolename;

-- Per the role provider pattern, an exception is to be thrown if any of the role names or user names specified
-- do not exist for the given application name.
if exists (select null from usernames as un left outer join users as u on lower(u.application_name) = lower(_application_name) and lower(u.user_name) = lower(un.username) where u.user_id is null)
	or exists (select null from rolenames as rn left outer join roles as r on lower(r.application_name) = lower(_application_name) and lower(r.role_name) = lower(rn.rolename) where r.role_id is null) then
	raise exception 'At least one user name or role specified does not exist in the application scope.' using errcode='MSING';
end if;

-- Insert the records linking the users to the roles, excluding pre-existing relationships.
delete from users_roles
using
	usernames as un
inner join
	users as u on lower(u.application_name) = lower(_application_name) and lower(u.user_name) = lower(un.username)
cross join
	rolenames as rn
inner join
	roles as r on lower(r.application_name) = lower(_application_name) and lower(r.role_name) = lower(rn.rolename)
where 
	users_roles.user_id = u.user_id
	and users_roles.role_id = r.role_id;
return true;
end;
$$;


ALTER FUNCTION public.remove_users_from_roles(_users character varying[], _roles character varying[], _application_name character varying) OWNER TO postgres;

--
-- TOC entry 238 (class 1255 OID 16704)
-- Name: role_exists(character varying, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION role_exists(_role_name character varying, _application_name character varying) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
declare
	retval boolean;
begin

select exists(
	select null from roles where lower(application_name) = lower(_application_name) and lower(role_name) = lower(_role_name))
into retval;

return retval;
end;
$$;


ALTER FUNCTION public.role_exists(_role_name character varying, _application_name character varying) OWNER TO postgres;

--
-- TOC entry 227 (class 1255 OID 16695)
-- Name: unlock_user(character varying, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION unlock_user(_user_name character varying, _application_name character varying) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
	begin	
	update users
	set
		last_lockout = null
	where
		lower(application_name) = lower(_application_name)
		and lower(user_name) = lower(_user_name);
	return true;
end;$$;


ALTER FUNCTION public.unlock_user(_user_name character varying, _application_name character varying) OWNER TO postgres;

--
-- TOC entry 228 (class 1255 OID 16696)
-- Name: update_user(integer, character varying, character varying, character varying, boolean, text, boolean); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION update_user(_user_id integer, _user_name character varying, _application_name character varying, _email character varying, _approved boolean, _comment text, _email_is_unique boolean) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
	begin
if _email_is_unique and exists(
	select null from users where lower(application_name) = lower(_application_name)
		and lower(email) = lower(_email) and user_id <> _user_id) then
	raise exception 'The email address specified is already in use by another user.' using errcode='DUPEM';
end if;
update users
set
	user_name = _user_name,
	email = _email,
	approved = _approved,
	comment = _comment
where
	user_id = _user_id;
return true;
end;$$;


ALTER FUNCTION public.update_user(_user_id integer, _user_name character varying, _application_name character varying, _email character varying, _approved boolean, _comment text, _email_is_unique boolean) OWNER TO postgres;

--
-- TOC entry 229 (class 1255 OID 16697)
-- Name: update_user_password(character varying, character varying, character varying, bytea); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION update_user_password(_user_name character varying, _application_name character varying, _salt character varying, _password bytea) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
	begin
update users
set
	salt = _salt,
	password = _password,
	last_password_changed = current_timestamp
where
	lower(application_name) = lower(_application_name)
	and lower(user_name) = lower(_user_name);
return true;
end;
$$;


ALTER FUNCTION public.update_user_password(_user_name character varying, _application_name character varying, _salt character varying, _password bytea) OWNER TO postgres;

--
-- TOC entry 225 (class 1255 OID 16698)
-- Name: update_user_q_and_a(character varying, character varying, character varying, character varying, bytea); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION update_user_q_and_a(_user_name character varying, _application_name character varying, _password_question character varying, _answer_salt character varying, _password_answer bytea) RETURNS boolean
    LANGUAGE plpgsql
    AS $$begin
update users
set
	password_question = _password_question,
	password_answer = _password_answer,
	answer_salt = _answer_salt
where
	lower(application_name) = lower(_application_name)
	and lower(user_name) = lower(_user_name);
return true;
end;$$;


ALTER FUNCTION public.update_user_q_and_a(_user_name character varying, _application_name character varying, _password_question character varying, _answer_salt character varying, _password_answer bytea) OWNER TO postgres;

--
-- TOC entry 240 (class 1255 OID 16702)
-- Name: user_is_in_role(character varying, character varying, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION user_is_in_role(_user_name character varying, _role_name character varying, _application_name character varying) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
	declare
	retval boolean;
begin
-- per the roleprovider pattern, throw an exception if the role does not exist
if not exists(select null from roles where lower(role_name) = lower(_role_name) and lower(application_name) = lower(_application_name)) then
	raise exception 'The specified role does not exist.' using errcode='NOROL';
end if;
-- per the roleprovider pattern, throw an exception if the user does not exist
if not exists(select null from users where lower(user_name) = lower(_user_name) and lower(application_name) = lower(_application_name)) then
	raise exception 'The specified user does not exist.' using errcode='NOUSR';
end if;

select
	exists(
		select null
		from users as u
		inner join users_roles as ur
		on ur.user_id = u.user_id
		inner join roles as r
		on r.role_id = ur.role_id
		where
			lower(u.user_name) = lower(_user_name)
			and lower(u.application_name) =lower( _application_name)
			and lower(r.role_name) = lower( _role_name)
			and lower(r.application_name) = lower(_application_name)
		)
into retval;
return retval;
end;$$;


ALTER FUNCTION public.user_is_in_role(_user_name character varying, _role_name character varying, _application_name character varying) OWNER TO postgres;

--
-- TOC entry 198 (class 1259 OID 16726)
-- Name: Actions; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE "Actions" (
    "Id" uuid NOT NULL,
    "ActionName" character varying(256) NOT NULL
);


ALTER TABLE "Actions" OWNER TO postgres;

--
-- TOC entry 199 (class 1259 OID 16731)
-- Name: ActionsInModules; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE "ActionsInModules" (
    "ActionId" uuid NOT NULL,
    "ModuleId" uuid NOT NULL
);


ALTER TABLE "ActionsInModules" OWNER TO postgres;

--
-- TOC entry 201 (class 1259 OID 16773)
-- Name: ActionsInModulesChosen; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE "ActionsInModulesChosen" (
    "ModuleInRoleId" uuid NOT NULL,
    "ActionId" uuid NOT NULL
);


ALTER TABLE "ActionsInModulesChosen" OWNER TO postgres;

--
-- TOC entry 197 (class 1259 OID 16721)
-- Name: Modules; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE "Modules" (
    "Id" uuid NOT NULL,
    "ModuleName" character varying(256) NOT NULL,
    "ParentModule" character varying(50)
);


ALTER TABLE "Modules" OWNER TO postgres;

--
-- TOC entry 200 (class 1259 OID 16756)
-- Name: ModulesInRoles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE "ModulesInRoles" (
    "ModuleId" uuid NOT NULL,
    "RoleId" integer NOT NULL,
    "Id" uuid NOT NULL
);


ALTER TABLE "ModulesInRoles" OWNER TO postgres;

--
-- TOC entry 189 (class 1259 OID 16641)
-- Name: roles_role_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE roles_role_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE roles_role_id_seq OWNER TO postgres;

--
-- TOC entry 2243 (class 0 OID 0)
-- Dependencies: 189
-- Name: roles_role_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE roles_role_id_seq OWNED BY roles.role_id;


--
-- TOC entry 190 (class 1259 OID 16647)
-- Name: user_login_activity; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE user_login_activity (
    activity_id integer NOT NULL,
    "when" timestamp with time zone DEFAULT now() NOT NULL,
    "from" character varying(250) NOT NULL,
    success boolean NOT NULL,
    user_id integer NOT NULL
);


ALTER TABLE user_login_activity OWNER TO postgres;

--
-- TOC entry 191 (class 1259 OID 16651)
-- Name: user_login_activity_activity_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE user_login_activity_activity_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE user_login_activity_activity_id_seq OWNER TO postgres;

--
-- TOC entry 2244 (class 0 OID 0)
-- Dependencies: 191
-- Name: user_login_activity_activity_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE user_login_activity_activity_id_seq OWNED BY user_login_activity.activity_id;


--
-- TOC entry 186 (class 1259 OID 16618)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE users (
    user_id integer NOT NULL,
    user_name character varying(250) NOT NULL,
    application_name character varying(250) NOT NULL,
    last_activity timestamp with time zone,
    created timestamp with time zone DEFAULT now() NOT NULL,
    email character varying(250),
    salt character varying(250),
    password bytea,
    approved boolean DEFAULT true NOT NULL,
    last_lockout timestamp with time zone,
    last_login timestamp with time zone,
    last_password_changed timestamp with time zone DEFAULT now() NOT NULL,
    password_question character varying(1000),
    password_answer bytea,
    answer_salt character varying(250),
    comment text
);


ALTER TABLE users OWNER TO postgres;

--
-- TOC entry 192 (class 1259 OID 16657)
-- Name: users_roles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE users_roles (
    user_id integer NOT NULL,
    role_id integer NOT NULL
);


ALTER TABLE users_roles OWNER TO postgres;

--
-- TOC entry 187 (class 1259 OID 16627)
-- Name: users_user_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE users_user_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE users_user_id_seq OWNER TO postgres;

--
-- TOC entry 2245 (class 0 OID 0)
-- Dependencies: 187
-- Name: users_user_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE users_user_id_seq OWNED BY users.user_id;


--
-- TOC entry 196 (class 1259 OID 16715)
-- Name: versions; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE versions (
    name character varying(250) NOT NULL,
    version character varying(15) NOT NULL
);


ALTER TABLE versions OWNER TO postgres;

--
-- TOC entry 2072 (class 2604 OID 16643)
-- Name: role_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY roles ALTER COLUMN role_id SET DEFAULT nextval('roles_role_id_seq'::regclass);


--
-- TOC entry 2074 (class 2604 OID 16653)
-- Name: activity_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY user_login_activity ALTER COLUMN activity_id SET DEFAULT nextval('user_login_activity_activity_id_seq'::regclass);


--
-- TOC entry 2071 (class 2604 OID 16629)
-- Name: user_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY users ALTER COLUMN user_id SET DEFAULT nextval('users_user_id_seq'::regclass);


--
-- TOC entry 2231 (class 0 OID 16726)
-- Dependencies: 198
-- Data for Name: Actions; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "Actions" VALUES ('061cfe44-942b-4aca-83b9-f76ebb789930', 'View');


--
-- TOC entry 2232 (class 0 OID 16731)
-- Dependencies: 199
-- Data for Name: ActionsInModules; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "ActionsInModules" VALUES ('061cfe44-942b-4aca-83b9-f76ebb789930', 'e979b0b7-2cae-497d-a699-22604bc37dc8');
INSERT INTO "ActionsInModules" VALUES ('061cfe44-942b-4aca-83b9-f76ebb789930', 'e0c2e04f-6128-4b3a-bce4-4363e8e15f36');
INSERT INTO "ActionsInModules" VALUES ('061cfe44-942b-4aca-83b9-f76ebb789930', 'e9d44547-71ee-49fb-a501-dfecfd4f9c22');
INSERT INTO "ActionsInModules" VALUES ('061cfe44-942b-4aca-83b9-f76ebb789930', '09733138-42a7-45f1-9e43-fc2fd8f25434');
INSERT INTO "ActionsInModules" VALUES ('061cfe44-942b-4aca-83b9-f76ebb789930', '9e66cce7-29ec-4a5b-842b-4618ea7ec117');
INSERT INTO "ActionsInModules" VALUES ('061cfe44-942b-4aca-83b9-f76ebb789930', '29ca68e4-7809-498b-82e0-0d4a002ebbf0');
INSERT INTO "ActionsInModules" VALUES ('061cfe44-942b-4aca-83b9-f76ebb789930', '3b8388bc-c2ad-4fa5-af55-0faed32192a1');
INSERT INTO "ActionsInModules" VALUES ('061cfe44-942b-4aca-83b9-f76ebb789930', '6483d77f-2172-4937-b783-0d628ac2e550');
INSERT INTO "ActionsInModules" VALUES ('061cfe44-942b-4aca-83b9-f76ebb789930', 'f10c147a-7881-426c-ba8b-5a463839c4b1');


--
-- TOC entry 2234 (class 0 OID 16773)
-- Dependencies: 201
-- Data for Name: ActionsInModulesChosen; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "ActionsInModulesChosen" VALUES ('6f77ef25-d0b5-42e4-aa97-11972f268ed9', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('90c5e323-f110-4f0a-a7a6-2231280ad06d', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('0e88ae65-17af-47a2-9b82-aeb30f8a1727', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('d08b0ecb-8d26-4ad7-a9f9-1de6582be6cf', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('df1a51e1-c3b6-4b54-8ab8-2657bf32c431', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('11d1e535-8df2-4f7c-8815-47c43d185d30', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('3dadb8b9-d708-47aa-829f-1322bf9141d0', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('06814096-d919-4c04-a5d4-7178e6fb73e8', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('3bdcd7bc-4da8-46a5-a943-45d037e5d23d', '061cfe44-942b-4aca-83b9-f76ebb789930');


--
-- TOC entry 2230 (class 0 OID 16721)
-- Dependencies: 197
-- Data for Name: Modules; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "Modules" VALUES ('f10c147a-7881-426c-ba8b-5a463839c4b1', 'Security Guard', NULL);
INSERT INTO "Modules" VALUES ('e9d44547-71ee-49fb-a501-dfecfd4f9c22', 'Car Brand', NULL);
INSERT INTO "Modules" VALUES ('09733138-42a7-45f1-9e43-fc2fd8f25434', 'Car Model', NULL);
INSERT INTO "Modules" VALUES ('e0c2e04f-6128-4b3a-bce4-4363e8e15f36', 'Car', NULL);
INSERT INTO "Modules" VALUES ('9e66cce7-29ec-4a5b-842b-4618ea7ec117', 'Customer', NULL);
INSERT INTO "Modules" VALUES ('29ca68e4-7809-498b-82e0-0d4a002ebbf0', 'Driver', NULL);
INSERT INTO "Modules" VALUES ('e979b0b7-2cae-497d-a699-22604bc37dc8', 'Booking', NULL);
INSERT INTO "Modules" VALUES ('3b8388bc-c2ad-4fa5-af55-0faed32192a1', 'Invoice', NULL);
INSERT INTO "Modules" VALUES ('6483d77f-2172-4937-b783-0d628ac2e550', 'Owner', NULL);


--
-- TOC entry 2233 (class 0 OID 16756)
-- Dependencies: 200
-- Data for Name: ModulesInRoles; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "ModulesInRoles" VALUES ('e9d44547-71ee-49fb-a501-dfecfd4f9c22', 2, '6f77ef25-d0b5-42e4-aa97-11972f268ed9');
INSERT INTO "ModulesInRoles" VALUES ('09733138-42a7-45f1-9e43-fc2fd8f25434', 2, '90c5e323-f110-4f0a-a7a6-2231280ad06d');
INSERT INTO "ModulesInRoles" VALUES ('6483d77f-2172-4937-b783-0d628ac2e550', 2, '0e88ae65-17af-47a2-9b82-aeb30f8a1727');
INSERT INTO "ModulesInRoles" VALUES ('f10c147a-7881-426c-ba8b-5a463839c4b1', 2, 'd08b0ecb-8d26-4ad7-a9f9-1de6582be6cf');
INSERT INTO "ModulesInRoles" VALUES ('e979b0b7-2cae-497d-a699-22604bc37dc8', 3, 'df1a51e1-c3b6-4b54-8ab8-2657bf32c431');
INSERT INTO "ModulesInRoles" VALUES ('e0c2e04f-6128-4b3a-bce4-4363e8e15f36', 3, '11d1e535-8df2-4f7c-8815-47c43d185d30');
INSERT INTO "ModulesInRoles" VALUES ('9e66cce7-29ec-4a5b-842b-4618ea7ec117', 3, '3dadb8b9-d708-47aa-829f-1322bf9141d0');
INSERT INTO "ModulesInRoles" VALUES ('29ca68e4-7809-498b-82e0-0d4a002ebbf0', 3, '06814096-d919-4c04-a5d4-7178e6fb73e8');
INSERT INTO "ModulesInRoles" VALUES ('3b8388bc-c2ad-4fa5-af55-0faed32192a1', 3, '3bdcd7bc-4da8-46a5-a943-45d037e5d23d');


--
-- TOC entry 2224 (class 0 OID 16635)
-- Dependencies: 188
-- Data for Name: roles; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO roles VALUES (1, 'SecurityGuard', 'UserManagement', NULL);
INSERT INTO roles VALUES (2, 'Karental Admin', 'UserManagement', 'Karental Admin');
INSERT INTO roles VALUES (3, 'Owner Admin', 'UserManagement', 'Owner Admin');


--
-- TOC entry 2246 (class 0 OID 0)
-- Dependencies: 189
-- Name: roles_role_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('roles_role_id_seq', 4, true);


--
-- TOC entry 2226 (class 0 OID 16647)
-- Dependencies: 190
-- Data for Name: user_login_activity; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO user_login_activity VALUES (1, '2015-12-25 23:20:56.785276+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (2, '2015-12-25 23:29:49.594751+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (3, '2015-12-25 23:50:24.236368+07', 'Machine: CHANDRA-PC, Application: UserManagement', false, 1);
INSERT INTO user_login_activity VALUES (4, '2015-12-25 23:50:28.46161+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (5, '2015-12-26 00:18:01.18614+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (6, '2015-12-26 17:36:30.686553+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (7, '2015-12-26 21:26:14.260311+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (8, '2015-12-26 21:26:44.614047+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (9, '2015-12-27 11:23:39.740857+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (10, '2015-12-27 11:36:22.341476+07', 'Machine: CHANDRA-PC, Application: UserManagement', false, 2);
INSERT INTO user_login_activity VALUES (11, '2015-12-27 11:37:31.743445+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (12, '2015-12-27 12:34:34.784974+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (13, '2015-12-27 12:38:22.102976+07', 'Machine: CHANDRA-PC, Application: UserManagement', false, 2);
INSERT INTO user_login_activity VALUES (14, '2015-12-27 13:18:00.502013+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (15, '2015-12-27 13:21:29.462965+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (16, '2015-12-27 13:22:40.877049+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (17, '2015-12-27 13:46:34.042022+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (18, '2015-12-27 13:47:07.276923+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (19, '2015-12-27 13:48:01.933049+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (20, '2015-12-27 13:57:04.28607+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (21, '2015-12-30 13:32:25.836201+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (22, '2015-12-30 13:33:41.218512+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (23, '2015-12-30 13:56:39.969373+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (24, '2015-12-30 14:28:18.063937+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (25, '2015-12-30 14:29:35.992395+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (26, '2015-12-30 15:18:18.657561+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (27, '2015-12-30 16:32:16.729405+07', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);


--
-- TOC entry 2247 (class 0 OID 0)
-- Dependencies: 191
-- Name: user_login_activity_activity_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('user_login_activity_activity_id_seq', 27, true);


--
-- TOC entry 2222 (class 0 OID 16618)
-- Dependencies: 186
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO users VALUES (3, 'Reynhart', 'UserManagement', '2015-12-30 15:18:18.713565+07', '2015-12-26 21:13:11.815558+07', 'reynhart@rekadia.co.id', 'HveK2B04SIn4GoouhUU5oy-rEDKeJSwW0CyVeIrnKOzYndkqAeFT3RIJ', '\x44f3872449855ddcda297291a0bab81b510aa9380c121aa9f2a7a14694351dad9c61683584a4136485a8445c13a71f0d', true, NULL, '2015-12-30 15:18:18.657561+07', '2015-12-26 21:13:12.059571+07', NULL, NULL, NULL, 'tes');
INSERT INTO users VALUES (2, 'chandra', 'UserManagement', '2015-12-30 16:32:16.739405+07', '2015-12-25 23:19:52.777615+07', 'chandra@rekadia.co.id', 'NoDR6ywUrUrV4n8F0PUgOGZXwUY8h1GgjSwe84LtY1AAF', '\x696477387f0758d627fee14e1b99df43cb9a427a82a4e7b4e6fa0fdb23e9ec925fa5b9560fbcd52b288e8c486c8b3375', true, NULL, '2015-12-30 16:32:16.729405+07', '2015-12-27 12:34:34.860979+07', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (1, 'superadmin', 'UserManagement', '2015-12-25 23:50:24.27437+07', '2014-04-06 09:33:06.223+07', 'chandra_oey@yahoo.com', '3RVFxDfcnEVWNz3Io8sgwMav16228PSg8OSArdd2H', '\x511636a71b488fe79e8438583681d62a7f8f970e368e7f10122f896586b80347fa19ad8e68ba67b8d5c8f983313ff60c', true, NULL, NULL, '2015-12-27 11:34:40.912674+07', NULL, NULL, NULL, NULL);


--
-- TOC entry 2228 (class 0 OID 16657)
-- Dependencies: 192
-- Data for Name: users_roles; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO users_roles VALUES (2, 1);
INSERT INTO users_roles VALUES (2, 2);
INSERT INTO users_roles VALUES (3, 3);


--
-- TOC entry 2248 (class 0 OID 0)
-- Dependencies: 187
-- Name: users_user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('users_user_id_seq', 3, true);


--
-- TOC entry 2229 (class 0 OID 16715)
-- Dependencies: 196
-- Data for Name: versions; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO versions VALUES ('application', '1.7');


--
-- TOC entry 2093 (class 2606 OID 16730)
-- Name: pk_action; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "Actions"
    ADD CONSTRAINT pk_action PRIMARY KEY ("Id");


--
-- TOC entry 2095 (class 2606 OID 16735)
-- Name: pk_actions_in_modules; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "ActionsInModules"
    ADD CONSTRAINT pk_actions_in_modules PRIMARY KEY ("ActionId", "ModuleId");


--
-- TOC entry 2099 (class 2606 OID 16777)
-- Name: pk_aimc; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "ActionsInModulesChosen"
    ADD CONSTRAINT pk_aimc PRIMARY KEY ("ModuleInRoleId", "ActionId");


--
-- TOC entry 2097 (class 2606 OID 16772)
-- Name: pk_mir; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "ModulesInRoles"
    ADD CONSTRAINT pk_mir PRIMARY KEY ("Id");


--
-- TOC entry 2091 (class 2606 OID 16725)
-- Name: pk_module; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "Modules"
    ADD CONSTRAINT pk_module PRIMARY KEY ("Id");


--
-- TOC entry 2085 (class 2606 OID 16655)
-- Name: pk_user_login_activity; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY user_login_activity
    ADD CONSTRAINT pk_user_login_activity PRIMARY KEY (activity_id);


--
-- TOC entry 2078 (class 2606 OID 16631)
-- Name: pk_users; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY users
    ADD CONSTRAINT pk_users PRIMARY KEY (user_id);


--
-- TOC entry 2081 (class 2606 OID 16645)
-- Name: roles_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (role_id);


--
-- TOC entry 2087 (class 2606 OID 16661)
-- Name: users_roles_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY users_roles
    ADD CONSTRAINT users_roles_pkey PRIMARY KEY (user_id, role_id);


--
-- TOC entry 2089 (class 2606 OID 16719)
-- Name: versions_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY versions
    ADD CONSTRAINT versions_pkey PRIMARY KEY (name);


--
-- TOC entry 2083 (class 1259 OID 16656)
-- Name: ix_user_login_activity_user_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_user_login_activity_user_id ON user_login_activity USING btree (user_id, "when");


--
-- TOC entry 2075 (class 1259 OID 16712)
-- Name: ix_users_email; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_users_email ON users USING btree (lower((application_name)::text), lower((email)::text));


--
-- TOC entry 2076 (class 1259 OID 16713)
-- Name: ix_users_last_activity; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_users_last_activity ON users USING btree (lower((application_name)::text), last_activity);


--
-- TOC entry 2082 (class 1259 OID 16711)
-- Name: ux_roles_role_name_application_name; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX ux_roles_role_name_application_name ON roles USING btree (lower((role_name)::text), lower((application_name)::text));


--
-- TOC entry 2079 (class 1259 OID 16714)
-- Name: ux_users_user_name_application_name; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX ux_users_user_name_application_name ON users USING btree (lower((application_name)::text), lower((user_name)::text));


--
-- TOC entry 2102 (class 2606 OID 16736)
-- Name: fk_aim_action; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "ActionsInModules"
    ADD CONSTRAINT fk_aim_action FOREIGN KEY ("ActionId") REFERENCES "Actions"("Id");


--
-- TOC entry 2103 (class 2606 OID 16741)
-- Name: fk_aim_module; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "ActionsInModules"
    ADD CONSTRAINT fk_aim_module FOREIGN KEY ("ModuleId") REFERENCES "Modules"("Id");


--
-- TOC entry 2107 (class 2606 OID 16783)
-- Name: fk_aimc_action; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "ActionsInModulesChosen"
    ADD CONSTRAINT fk_aimc_action FOREIGN KEY ("ActionId") REFERENCES "Actions"("Id");


--
-- TOC entry 2106 (class 2606 OID 16778)
-- Name: fk_aimc_mir; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "ActionsInModulesChosen"
    ADD CONSTRAINT fk_aimc_mir FOREIGN KEY ("ModuleInRoleId") REFERENCES "ModulesInRoles"("Id");


--
-- TOC entry 2104 (class 2606 OID 16761)
-- Name: fk_mir_module; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "ModulesInRoles"
    ADD CONSTRAINT fk_mir_module FOREIGN KEY ("ModuleId") REFERENCES "Modules"("Id");


--
-- TOC entry 2105 (class 2606 OID 16766)
-- Name: fk_mir_role; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "ModulesInRoles"
    ADD CONSTRAINT fk_mir_role FOREIGN KEY ("RoleId") REFERENCES roles(role_id);


--
-- TOC entry 2100 (class 2606 OID 16662)
-- Name: users_roles_role_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY users_roles
    ADD CONSTRAINT users_roles_role_id_fkey FOREIGN KEY (role_id) REFERENCES roles(role_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 2101 (class 2606 OID 16667)
-- Name: users_roles_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY users_roles
    ADD CONSTRAINT users_roles_user_id_fkey FOREIGN KEY (user_id) REFERENCES users(user_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 2241 (class 0 OID 0)
-- Dependencies: 5
-- Name: public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;


-- Completed on 2015-12-31 10:08:18

--
-- PostgreSQL database dump complete
--

