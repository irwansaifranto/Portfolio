--
-- PostgreSQL database dump
--

-- Dumped from database version 9.5rc1
-- Dumped by pg_dump version 9.5rc1

-- Started on 2016-03-25 14:07:13

SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 204 (class 3079 OID 12355)
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- TOC entry 2244 (class 0 OID 0)
-- Dependencies: 204
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET search_path = public, pg_catalog;

--
-- TOC entry 618 (class 1247 OID 16677)
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
-- TOC entry 621 (class 1247 OID 16680)
-- Name: profile_info; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE profile_info AS (
	property_name character varying(250),
	property_type character varying(250),
	property_value text
);


ALTER TYPE profile_info OWNER TO postgres;

--
-- TOC entry 615 (class 1247 OID 16674)
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
-- TOC entry 233 (class 1255 OID 16705)
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
-- TOC entry 234 (class 1255 OID 16688)
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
-- TOC entry 235 (class 1255 OID 16689)
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
-- TOC entry 218 (class 1255 OID 16690)
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
-- TOC entry 219 (class 1255 OID 16691)
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
-- TOC entry 190 (class 1259 OID 16635)
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
-- TOC entry 225 (class 1255 OID 16701)
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
-- TOC entry 236 (class 1255 OID 16681)
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
-- TOC entry 243 (class 1255 OID 16720)
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
-- TOC entry 220 (class 1255 OID 16685)
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
-- TOC entry 226 (class 1255 OID 16700)
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
-- TOC entry 217 (class 1255 OID 16687)
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
-- TOC entry 237 (class 1255 OID 16686)
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
-- TOC entry 221 (class 1255 OID 16692)
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
-- TOC entry 238 (class 1255 OID 16693)
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
-- TOC entry 222 (class 1255 OID 16682)
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
-- TOC entry 223 (class 1255 OID 16683)
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
-- TOC entry 239 (class 1255 OID 16699)
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
-- TOC entry 224 (class 1255 OID 16684)
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
-- TOC entry 232 (class 1255 OID 16706)
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
-- TOC entry 228 (class 1255 OID 16694)
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
-- TOC entry 241 (class 1255 OID 16703)
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
-- TOC entry 240 (class 1255 OID 16704)
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
-- TOC entry 229 (class 1255 OID 16695)
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
-- TOC entry 230 (class 1255 OID 16696)
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
-- TOC entry 231 (class 1255 OID 16697)
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
-- TOC entry 227 (class 1255 OID 16698)
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
-- TOC entry 242 (class 1255 OID 16702)
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
-- TOC entry 200 (class 1259 OID 16726)
-- Name: Actions; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE "Actions" (
    "Id" uuid NOT NULL,
    "ActionName" character varying(256) NOT NULL
);


ALTER TABLE "Actions" OWNER TO postgres;

--
-- TOC entry 201 (class 1259 OID 16731)
-- Name: ActionsInModules; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE "ActionsInModules" (
    "ActionId" uuid NOT NULL,
    "ModuleId" uuid NOT NULL
);


ALTER TABLE "ActionsInModules" OWNER TO postgres;

--
-- TOC entry 203 (class 1259 OID 16773)
-- Name: ActionsInModulesChosen; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE "ActionsInModulesChosen" (
    "ModuleInRoleId" uuid NOT NULL,
    "ActionId" uuid NOT NULL
);


ALTER TABLE "ActionsInModulesChosen" OWNER TO postgres;

--
-- TOC entry 199 (class 1259 OID 16721)
-- Name: Modules; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE "Modules" (
    "Id" uuid NOT NULL,
    "ModuleName" character varying(256) NOT NULL,
    "ParentModule" character varying(50)
);


ALTER TABLE "Modules" OWNER TO postgres;

--
-- TOC entry 202 (class 1259 OID 16756)
-- Name: ModulesInRoles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE "ModulesInRoles" (
    "ModuleId" uuid NOT NULL,
    "RoleId" integer NOT NULL,
    "Id" uuid NOT NULL
);


ALTER TABLE "ModulesInRoles" OWNER TO postgres;

--
-- TOC entry 191 (class 1259 OID 16641)
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
-- TOC entry 2245 (class 0 OID 0)
-- Dependencies: 191
-- Name: roles_role_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE roles_role_id_seq OWNED BY roles.role_id;


--
-- TOC entry 192 (class 1259 OID 16647)
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
-- TOC entry 193 (class 1259 OID 16651)
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
-- TOC entry 2246 (class 0 OID 0)
-- Dependencies: 193
-- Name: user_login_activity_activity_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE user_login_activity_activity_id_seq OWNED BY user_login_activity.activity_id;


--
-- TOC entry 188 (class 1259 OID 16618)
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
-- TOC entry 194 (class 1259 OID 16657)
-- Name: users_roles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE users_roles (
    user_id integer NOT NULL,
    role_id integer NOT NULL
);


ALTER TABLE users_roles OWNER TO postgres;

--
-- TOC entry 189 (class 1259 OID 16627)
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
-- TOC entry 2247 (class 0 OID 0)
-- Dependencies: 189
-- Name: users_user_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE users_user_id_seq OWNED BY users.user_id;


--
-- TOC entry 198 (class 1259 OID 16715)
-- Name: versions; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE versions (
    name character varying(250) NOT NULL,
    version character varying(15) NOT NULL
);


ALTER TABLE versions OWNER TO postgres;

--
-- TOC entry 2074 (class 2604 OID 16643)
-- Name: role_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY roles ALTER COLUMN role_id SET DEFAULT nextval('roles_role_id_seq'::regclass);


--
-- TOC entry 2076 (class 2604 OID 16653)
-- Name: activity_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY user_login_activity ALTER COLUMN activity_id SET DEFAULT nextval('user_login_activity_activity_id_seq'::regclass);


--
-- TOC entry 2073 (class 2604 OID 16629)
-- Name: user_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY users ALTER COLUMN user_id SET DEFAULT nextval('users_user_id_seq'::regclass);


--
-- TOC entry 2233 (class 0 OID 16726)
-- Dependencies: 200
-- Data for Name: Actions; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "Actions" VALUES ('061cfe44-942b-4aca-83b9-f76ebb789930', 'View');


--
-- TOC entry 2234 (class 0 OID 16731)
-- Dependencies: 201
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
INSERT INTO "ActionsInModules" VALUES ('061cfe44-942b-4aca-83b9-f76ebb789930', 'd893889a-598e-444f-b824-47e3d97728f4');
INSERT INTO "ActionsInModules" VALUES ('061cfe44-942b-4aca-83b9-f76ebb789930', 'e5736d7d-05e9-48a7-9c4d-048215c0be1f');
INSERT INTO "ActionsInModules" VALUES ('061cfe44-942b-4aca-83b9-f76ebb789930', 'ae14066f-f9d1-4508-b938-95f746ea129f');
INSERT INTO "ActionsInModules" VALUES ('061cfe44-942b-4aca-83b9-f76ebb789930', '8eef9731-4fc6-45d9-af6f-bdecec09bbac');
INSERT INTO "ActionsInModules" VALUES ('061cfe44-942b-4aca-83b9-f76ebb789930', '2215e83f-5e7f-4e76-9c67-af0006d1afc8');


--
-- TOC entry 2236 (class 0 OID 16773)
-- Dependencies: 203
-- Data for Name: ActionsInModulesChosen; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "ActionsInModulesChosen" VALUES ('6f77ef25-d0b5-42e4-aa97-11972f268ed9', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('90c5e323-f110-4f0a-a7a6-2231280ad06d', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('0e88ae65-17af-47a2-9b82-aeb30f8a1727', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('d08b0ecb-8d26-4ad7-a9f9-1de6582be6cf', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('94f004b1-436b-4c11-a72a-79baaba45f0d', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('a5ae6540-11a7-4022-b506-457b96048bd0', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('9d79baef-b169-4880-b05e-5a62783af376', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('8637a98a-ad70-4d5a-9917-2ebb8301696c', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('c61c16d6-f8bf-4ea8-a6a5-6d6193b5a0b8', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('79a0184b-cb75-4fa4-9941-d708206908d6', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('6fce7549-972f-488a-8109-9ae03d757956', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('68e9c0d1-ceea-4bda-a8e7-316f0c4f57b8', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('d5c7827a-66e6-4e6a-b167-e9c0d68cd81b', '061cfe44-942b-4aca-83b9-f76ebb789930');
INSERT INTO "ActionsInModulesChosen" VALUES ('0bf5b49e-78b7-4dfa-9ae1-f669db0833e4', '061cfe44-942b-4aca-83b9-f76ebb789930');


--
-- TOC entry 2232 (class 0 OID 16721)
-- Dependencies: 199
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
INSERT INTO "Modules" VALUES ('847158f7-f59e-4a6d-ae26-d60754edba5a', 'Report', NULL);
INSERT INTO "Modules" VALUES ('ae14066f-f9d1-4508-b938-95f746ea129f', 'Sales Report', 'Report');
INSERT INTO "Modules" VALUES ('8eef9731-4fc6-45d9-af6f-bdecec09bbac', 'Utilization Report', 'Report');
INSERT INTO "Modules" VALUES ('d893889a-598e-444f-b824-47e3d97728f4', 'Car Report', 'Report');
INSERT INTO "Modules" VALUES ('e5736d7d-05e9-48a7-9c4d-048215c0be1f', 'Driver Report', 'Report');
INSERT INTO "Modules" VALUES ('2215e83f-5e7f-4e76-9c67-af0006d1afc8', 'Expense', NULL);


--
-- TOC entry 2235 (class 0 OID 16756)
-- Dependencies: 202
-- Data for Name: ModulesInRoles; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "ModulesInRoles" VALUES ('e9d44547-71ee-49fb-a501-dfecfd4f9c22', 2, '6f77ef25-d0b5-42e4-aa97-11972f268ed9');
INSERT INTO "ModulesInRoles" VALUES ('09733138-42a7-45f1-9e43-fc2fd8f25434', 2, '90c5e323-f110-4f0a-a7a6-2231280ad06d');
INSERT INTO "ModulesInRoles" VALUES ('6483d77f-2172-4937-b783-0d628ac2e550', 2, '0e88ae65-17af-47a2-9b82-aeb30f8a1727');
INSERT INTO "ModulesInRoles" VALUES ('f10c147a-7881-426c-ba8b-5a463839c4b1', 2, 'd08b0ecb-8d26-4ad7-a9f9-1de6582be6cf');
INSERT INTO "ModulesInRoles" VALUES ('e979b0b7-2cae-497d-a699-22604bc37dc8', 3, '94f004b1-436b-4c11-a72a-79baaba45f0d');
INSERT INTO "ModulesInRoles" VALUES ('e0c2e04f-6128-4b3a-bce4-4363e8e15f36', 3, 'a5ae6540-11a7-4022-b506-457b96048bd0');
INSERT INTO "ModulesInRoles" VALUES ('9e66cce7-29ec-4a5b-842b-4618ea7ec117', 3, '9d79baef-b169-4880-b05e-5a62783af376');
INSERT INTO "ModulesInRoles" VALUES ('29ca68e4-7809-498b-82e0-0d4a002ebbf0', 3, '8637a98a-ad70-4d5a-9917-2ebb8301696c');
INSERT INTO "ModulesInRoles" VALUES ('2215e83f-5e7f-4e76-9c67-af0006d1afc8', 3, 'c61c16d6-f8bf-4ea8-a6a5-6d6193b5a0b8');
INSERT INTO "ModulesInRoles" VALUES ('3b8388bc-c2ad-4fa5-af55-0faed32192a1', 3, '79a0184b-cb75-4fa4-9941-d708206908d6');
INSERT INTO "ModulesInRoles" VALUES ('d893889a-598e-444f-b824-47e3d97728f4', 3, '6fce7549-972f-488a-8109-9ae03d757956');
INSERT INTO "ModulesInRoles" VALUES ('e5736d7d-05e9-48a7-9c4d-048215c0be1f', 3, '68e9c0d1-ceea-4bda-a8e7-316f0c4f57b8');
INSERT INTO "ModulesInRoles" VALUES ('ae14066f-f9d1-4508-b938-95f746ea129f', 3, 'd5c7827a-66e6-4e6a-b167-e9c0d68cd81b');
INSERT INTO "ModulesInRoles" VALUES ('8eef9731-4fc6-45d9-af6f-bdecec09bbac', 3, '0bf5b49e-78b7-4dfa-9ae1-f669db0833e4');


--
-- TOC entry 2226 (class 0 OID 16635)
-- Dependencies: 190
-- Data for Name: roles; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO roles VALUES (1, 'SecurityGuard', 'UserManagement', NULL);
INSERT INTO roles VALUES (2, 'Karental Admin', 'UserManagement', 'Karental Admin');
INSERT INTO roles VALUES (3, 'Owner Admin', 'UserManagement', 'Owner Admin');


--
-- TOC entry 2248 (class 0 OID 0)
-- Dependencies: 191
-- Name: roles_role_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('roles_role_id_seq', 4, true);


--
-- TOC entry 2228 (class 0 OID 16647)
-- Dependencies: 192
-- Data for Name: user_login_activity; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO user_login_activity VALUES (1, '2015-12-25 16:20:56.785276+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (2, '2015-12-25 16:29:49.594751+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (3, '2015-12-25 16:50:24.236368+00', 'Machine: CHANDRA-PC, Application: UserManagement', false, 1);
INSERT INTO user_login_activity VALUES (4, '2015-12-25 16:50:28.46161+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (5, '2015-12-25 17:18:01.18614+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (6, '2015-12-26 10:36:30.686553+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (7, '2015-12-26 14:26:14.260311+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (8, '2015-12-26 14:26:44.614047+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (9, '2015-12-27 04:23:39.740857+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (10, '2015-12-27 04:36:22.341476+00', 'Machine: CHANDRA-PC, Application: UserManagement', false, 2);
INSERT INTO user_login_activity VALUES (11, '2015-12-27 04:37:31.743445+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (12, '2015-12-27 05:34:34.784974+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (13, '2015-12-27 05:38:22.102976+00', 'Machine: CHANDRA-PC, Application: UserManagement', false, 2);
INSERT INTO user_login_activity VALUES (14, '2015-12-27 06:18:00.502013+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (15, '2015-12-27 06:21:29.462965+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (16, '2015-12-27 06:22:40.877049+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (17, '2015-12-27 06:46:34.042022+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (18, '2015-12-27 06:47:07.276923+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (19, '2015-12-27 06:48:01.933049+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (20, '2015-12-27 06:57:04.28607+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (21, '2015-12-30 06:32:25.836201+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (22, '2015-12-30 06:33:41.218512+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (23, '2015-12-30 06:56:39.969373+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (24, '2015-12-30 07:28:18.063937+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (25, '2015-12-30 07:29:35.992395+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (26, '2015-12-30 08:18:18.657561+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (27, '2015-12-30 09:32:16.729405+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (28, '2015-12-31 03:10:23.17828+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (29, '2016-01-02 02:18:15.396425+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (30, '2016-01-03 04:22:34.677396+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (31, '2016-01-06 18:11:36.114496+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (32, '2016-01-06 18:24:46.316693+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (33, '2016-01-06 18:25:40.614799+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 4);
INSERT INTO user_login_activity VALUES (34, '2016-01-06 18:25:53.840555+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 4);
INSERT INTO user_login_activity VALUES (35, '2016-01-06 18:28:24.623179+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (36, '2016-01-07 02:43:23.536672+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (37, '2016-01-07 02:52:16.493155+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (38, '2016-01-07 02:59:35.120243+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (39, '2016-01-07 03:00:16.683621+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (40, '2016-01-07 03:00:27.619246+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (41, '2016-01-07 03:00:43.43115+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (42, '2016-01-07 03:20:00.705343+00', 'Machine: CHANDRA-PC, Application: UserManagement', false, 3);
INSERT INTO user_login_activity VALUES (43, '2016-01-07 03:20:06.362666+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (44, '2016-01-07 03:49:53.335875+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (45, '2016-01-12 12:23:18.251345+00', 'Machine: CHANDRA-PC, Application: UserManagement', false, 2);
INSERT INTO user_login_activity VALUES (46, '2016-01-12 12:23:26.637825+00', 'Machine: CHANDRA-PC, Application: UserManagement', false, 2);
INSERT INTO user_login_activity VALUES (47, '2016-01-12 12:23:49.943158+00', 'Machine: CHANDRA-PC, Application: UserManagement', false, 2);
INSERT INTO user_login_activity VALUES (48, '2016-01-12 12:24:17.746748+00', 'Machine: CHANDRA-PC, Application: UserManagement', false, 3);
INSERT INTO user_login_activity VALUES (49, '2016-01-12 12:25:05.141459+00', 'Machine: CHANDRA-PC, Application: UserManagement', false, 2);
INSERT INTO user_login_activity VALUES (50, '2016-01-12 12:25:31.821985+00', 'Machine: CHANDRA-PC, Application: UserManagement', false, 2);
INSERT INTO user_login_activity VALUES (51, '2016-01-12 12:34:16.803012+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (52, '2016-01-12 12:34:45.155634+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (53, '2016-01-19 09:02:44.780972+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (54, '2016-01-20 16:19:49.548392+00', 'Machine: CHANDRA-PC, Application: UserManagement', false, 3);
INSERT INTO user_login_activity VALUES (55, '2016-01-20 16:19:55.155712+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (56, '2016-01-28 04:57:02.951045+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (57, '2016-01-28 11:44:10.786718+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (58, '2016-01-29 14:38:36.120616+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (59, '2016-01-31 05:15:01.83092+00', 'Machine: CHANDRA-PC, Application: UserManagement', false, 3);
INSERT INTO user_login_activity VALUES (60, '2016-01-31 05:15:06.673197+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (61, '2016-01-31 05:15:31.671627+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (62, '2016-01-31 05:23:16.996242+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (63, '2016-01-31 13:31:12.057745+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (64, '2016-01-31 15:35:39.90913+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (65, '2016-02-02 04:16:30.568434+00', 'Machine: CHANDRA-PC, Application: UserManagement', false, 2);
INSERT INTO user_login_activity VALUES (66, '2016-02-02 04:16:35.643724+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (67, '2016-02-02 05:36:05.73116+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (68, '2016-02-17 05:56:21.579877+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (69, '2016-02-17 14:33:18.322258+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (70, '2016-02-18 01:55:20.696027+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (71, '2016-02-20 09:14:37.459142+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (72, '2016-02-22 02:44:33.974516+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (73, '2016-02-25 04:19:52.389521+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (74, '2016-02-27 13:15:56.549878+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (75, '2016-02-28 06:26:03.996284+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (76, '2016-03-01 13:18:36.699643+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (77, '2016-03-01 13:25:41.779956+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (78, '2016-03-01 13:28:41.013207+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (79, '2016-03-02 10:44:28.01108+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (80, '2016-03-03 04:34:12.749944+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (81, '2016-03-05 10:15:30.891174+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (82, '2016-03-07 07:48:31.973598+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (83, '2016-03-07 10:49:13.753287+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (84, '2016-03-13 07:56:39.216755+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (85, '2016-03-14 03:05:12.632491+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (86, '2016-03-14 03:06:11.567862+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (87, '2016-03-14 03:08:34.447034+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (88, '2016-03-15 06:55:12.554784+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (89, '2016-03-15 06:59:41.294155+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 23);
INSERT INTO user_login_activity VALUES (90, '2016-03-15 07:00:28.341846+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (91, '2016-03-15 07:02:52.327082+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 23);
INSERT INTO user_login_activity VALUES (92, '2016-03-15 07:04:41.874348+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (93, '2016-03-15 07:06:37.621968+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 24);
INSERT INTO user_login_activity VALUES (94, '2016-03-15 07:10:32.680413+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (95, '2016-03-15 07:23:04.049388+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (96, '2016-03-15 07:23:25.280603+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 24);
INSERT INTO user_login_activity VALUES (97, '2016-03-15 07:48:41.532327+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (98, '2016-03-15 09:39:13.364383+00', 'Machine: CHANDRA-PC, Application: UserManagement', false, 2);
INSERT INTO user_login_activity VALUES (99, '2016-03-15 09:39:17.931644+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (100, '2016-03-15 09:44:50.555669+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (101, '2016-03-15 09:45:26.928749+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 2);
INSERT INTO user_login_activity VALUES (102, '2016-03-15 09:47:04.245316+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (103, '2016-03-16 06:59:05.437622+00', 'Machine: CHANDRA-PC, Application: UserManagement', false, 3);
INSERT INTO user_login_activity VALUES (104, '2016-03-16 06:59:17.168293+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (105, '2016-03-21 12:55:44.569217+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (106, '2016-03-24 03:22:41.671329+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);
INSERT INTO user_login_activity VALUES (107, '2016-03-24 12:28:57.587599+00', 'Machine: CHANDRA-PC, Application: UserManagement', true, 3);


--
-- TOC entry 2249 (class 0 OID 0)
-- Dependencies: 193
-- Name: user_login_activity_activity_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('user_login_activity_activity_id_seq', 107, true);


--
-- TOC entry 2224 (class 0 OID 16618)
-- Dependencies: 188
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO users VALUES (5, 'A1', 'UserManagement', NULL, '2016-02-02 04:17:20.576294+00', 'a1@karental.id', 'rWKMAoxzGWF9tfgvCz6NudXZ8MdebWYBOntDnPLoEIJyEyb-h-YYeOMS9J', '\xd201b4ec55e1bb914d04235dd8ee75ec3a648d46b8bdc4a35623369dd2eb8867e03c0eaf456ae9add41e2f7dd7a067df', true, NULL, NULL, '2016-02-02 04:17:20.795307+00', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (3, 'Reynhart', 'UserManagement', '2016-03-24 12:28:57.659603+00', '2015-12-26 14:13:11.815558+00', 'reynhart@rekadia.co.id', '3-o1LVA80CWM90F0mQQg261X1ZV5zQ8gQu0o9I7RQmImLK9f1qE2f1e-4m', '\x5e144f5bdf620b5f53a515713be1eb3d645528dd24c0a41fa2a1f858b83c89a073e7be2b736b612b3c0df0daafd1b640', true, NULL, '2016-03-24 12:28:57.587599+00', '2016-01-07 03:00:43.517155+00', NULL, NULL, NULL, 'tes');
INSERT INTO users VALUES (6, 'A2', 'UserManagement', NULL, '2016-02-02 04:17:43.322595+00', 'a2@karental.id', '8gyJWCfKXTPqjBEFqrzHUCayum5KFWwyLyYvJYhXzoXEnCbohYgDfuQgPp4', '\xa2ead9700f0296e3578c01ea22e3874df49a1f937b43c4451dc757d635fbbc9a47874d0bf864e3305abeb2d24488d825', true, NULL, NULL, '2016-02-02 04:17:43.431602+00', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (18, 'A14', 'UserManagement', NULL, '2016-02-02 04:25:55.86426+00', 'a14@karental.id', 'QVzqKbYRNDMb0ssXOhNUX8mfUDft5jyTfMVwk-Vgs', '\xcc78c07ac3e7763409f0f34d9e5edcd8e0de068db49abc745a9d0f1efe6cae5807a5f3bd86720aa1bfcbbc2f5685aea5', true, NULL, NULL, '2016-02-02 04:25:55.938265+00', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (7, 'A3', 'UserManagement', NULL, '2016-02-02 04:17:57.838426+00', 'a3@karental.id', 'Td1VneLpyaoDFW7j3Db4nPjsu7fJtMPNRsk1cBlCif', '\x69b6b0f8c35ad3d0679392045bfc01c5a293b491cd88676d89d68f1588ff7aa012d02f7da776f43e5ac179d2112075de', true, NULL, NULL, '2016-02-02 04:17:57.931431+00', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (8, 'A4', 'UserManagement', NULL, '2016-02-02 04:18:18.165588+00', 'a4@karenta.id', 'ibIhO0OjgmVGZ47XAAxgJcu4k8qCyATo2x6bALENGuu', '\x6fb3b855cd1bd868da7845d184e3e3886e39c47092c69878307f34a48683695c0183a65f6f18609580491cd93fc2b647', true, NULL, NULL, '2016-02-02 04:18:18.256593+00', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (1, 'superadmin', 'UserManagement', '2015-12-25 16:50:24.27437+00', '2014-04-06 02:33:06.223+00', 'chandra_oey@yahoo.com', '3RVFxDfcnEVWNz3Io8sgwMav16228PSg8OSArdd2H', '\x511636a71b488fe79e8438583681d62a7f8f970e368e7f10122f896586b80347fa19ad8e68ba67b8d5c8f983313ff60c', true, NULL, NULL, '2015-12-27 04:34:40.912674+00', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (19, 'A15', 'UserManagement', NULL, '2016-02-02 04:26:13.613276+00', 'a15@karental.id', 'uCSOinClz0yKn5R5ezRi1DUU9ObpMeCfXZBDLGlLfUgZ', '\x3c8a2225053e2ba5797a45f8455331a6af1788a8ac496647f4833ca8dc5d38740a0456fd21e0589e67a699ba905853d0', true, NULL, NULL, '2016-02-02 04:26:13.69528+00', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (9, 'A5', 'UserManagement', NULL, '2016-02-02 04:18:32.778424+00', 'a5@karental.id', 'm8eYs1AG14Ae8WdDXd1alMI4bQuIvD', '\x451ae3c7bc39cb60d1de48f44f5308b89962c88fbfafed04d2ffa53f606cf9ab805e41d3499907a36ceb52bc613a2e3d', true, NULL, NULL, '2016-02-02 04:18:33.501465+00', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (10, 'A6', 'UserManagement', NULL, '2016-02-02 04:18:50.02241+00', 'a6@karental.id', 'mZgNHGHKn19tqw1ljcgUJOZvDpcvHq3sUVvILMlAZQjB', '\x0c8db810ddcfd1f242b11d01404e2f738688ae3e5df2850d6899bf7e0a61648f7760cdad374ff9cd37f89532f78374d6', true, NULL, NULL, '2016-02-02 04:18:50.096415+00', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (11, 'A7', 'UserManagement', NULL, '2016-02-02 04:19:13.205736+00', 'a7@karental.id', 'gn8gyfykZ5OmkdcJIhKs-OfaaBEsMim4z2rL2KM3KGJgoU8HAqgb2e6D', '\x14ecd3b051f3e8304d1c6fa3fe942f8a212abdf74329b60da6ad493bccaa574c6b61c9cd7706d6e251ae225c10d8cb42', true, NULL, NULL, '2016-02-02 04:19:13.403748+00', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (20, 'A16', 'UserManagement', NULL, '2016-02-02 04:26:55.856692+00', 'a16@karental.id', 'yY82ZmZIRgPjpZV8uH5LFyFquC1QDvdNjyh4gAYY8B2TM', '\xcce8574c1688ef67da2de2d6f256b05f1d484f68faff63f808665f50977649fd0a33413d3a8ab7ebec43412ffed42684', true, NULL, NULL, '2016-02-02 04:26:55.930696+00', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (12, 'A8', 'UserManagement', NULL, '2016-02-02 04:19:27.999582+00', 'a8@karental.id', 'Fzg42kvm6znWh6AUHjxTePk2aN-gLd7-uUJqu9nloJXegnWIynx', '\x3969359ddc4f19dac79b9e66bb1defb88bfe0ea2f098cc4d3f526aee376c4a68ff042805bbb06743b1160234226a12f5', true, NULL, NULL, '2016-02-02 04:19:28.094588+00', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (13, 'A9', 'UserManagement', NULL, '2016-02-02 04:19:47.085674+00', 'a9@karental.id', 'QzfoEvZwH9Laq3HJ4PEZhYQadiTuLb', '\xdf18f7ec45520a4fec496870b152cabb94881006b4cc7f4bdae5ebef7e9e5e4e44a3596f641b6ba72849c10d6627affb', true, NULL, NULL, '2016-02-02 04:19:47.163679+00', NULL, NULL, NULL, '');
INSERT INTO users VALUES (21, 'A17', 'UserManagement', NULL, '2016-02-02 04:27:17.769945+00', 'a17@karental.id', 'tzLKCi5rsI1TMsoDVXbIAs-7ybhEWo2ibGDJk9W', '\x2d8eb800f4d5013ca420a1fd196dcd0a996605642056cf94d749c0108ff6831e8393aab5b49671e88a516556d5d264a1', true, NULL, NULL, '2016-02-02 04:27:17.837949+00', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (14, 'A10', 'UserManagement', NULL, '2016-02-02 04:24:43.863142+00', 'a10@karental.id', '5mPY5-0Qy8j08IwAcB2asCS5EuqsxMa95QPZyY7w8tOumv7', '\x69d7bdc667f993ed3796bcae241cb987f0a3b47e98566593bc31e42ce2643ef86c6093b330ed5ad3375c0294791fccf8', true, NULL, NULL, '2016-02-02 04:24:44.110156+00', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (15, 'A11', 'UserManagement', NULL, '2016-02-02 04:25:00.08707+00', 'a11@karental.id', '5NPjCVtowhCWGR6W09iF6yDdfOVHFtEsFQ5Hi5VY59few', '\xfe8aa016de9b560c133c8897d1a12f5d7e6d73b537d6d18d99f97e7315b0487fd2eb6fe5b82ad20181bbac085a065bf9', true, NULL, NULL, '2016-02-02 04:25:00.273081+00', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (4, 'yosef', 'UserManagement', '2016-01-06 18:25:53.897558+00', '2016-01-06 18:25:22.184745+00', 'yosefsukianto@rekadia.co.id', 'VelLyt-CDc3CxHaWOq4xa3--QrA95mql2ilwJBRf11F', '\x4599ee1f6ee167f35b50be5cb0e1915241ec4e3110e2643ea1569695d9d00f9ced04841d312a5d43fc0682dbc6f2a0cf', true, NULL, '2016-01-06 18:25:53.840555+00', '2016-01-06 18:25:22.292751+00', NULL, NULL, NULL, '');
INSERT INTO users VALUES (16, 'A12', 'UserManagement', NULL, '2016-02-02 04:25:17.03704+00', 'a12@karental.id', '6LDdyoVCaCuPmHzzCVuSjVJpRbxhXluljLhTsjBx5lom2GKRlOhx3fd8HGf', '\x6a60dc4fb458991a983fb7b76178a0fbcdfed7502a28ebafa71b3fee0ddd3c9e99d689383d3164dae9b3c4f23f98bf1b', true, NULL, NULL, '2016-02-02 04:25:17.186048+00', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (22, 'A18', 'UserManagement', NULL, '2016-02-02 04:27:33.987873+00', 'a18@karental.id', 'yBB1cFYAN0kA2IFARR9hiMzbtavrCyoznFPEs3NursNOCPjH3', '\x1219ebd90df2d32600754511fa66a891e29b9dc7c41ce192a144c969955942a5d41eb113530078cbef0e0ae96dad66da', true, NULL, NULL, '2016-02-02 04:27:34.085878+00', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (17, 'A13', 'UserManagement', NULL, '2016-02-02 04:25:34.418034+00', 'a13@karental.id', 'Y9GWpyh8iP00Wnvw9bsqLD4b-b4TC8l', '\x659666a4080b47b595fb6e1d5e6567b0ef0b25c714e913554ada441d6112517b06c935179411f41477d97a8e6b11e757', true, NULL, NULL, '2016-02-02 04:25:34.496038+00', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (23, 'api.bali', 'UserManagement', '2016-03-15 07:02:52.388085+00', '2016-03-15 06:58:17.009335+00', 'info@karental.id', 'oziqXbq-AQxnKzGoUaMndPKvnPpfzkcw-GR6UzGxJiMnXqCELIAd-', '\x8f9a70b3492d7f2d891c280b1d3c357fead919512c9dffae4fed0bbc9b2dc03a329c4042fa3a3bd55eb1098d4759411d', true, NULL, '2016-03-15 07:02:52.327082+00', '2016-03-15 06:58:17.28535+00', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (24, 'api.bandung', 'UserManagement', '2016-03-15 07:23:25.321605+00', '2016-03-15 06:59:07.44822+00', 'info1@karental.id', 'YLSriUlouCO67GLZItBKkfnHK0WxbuUqMWryGtfSvbkGF0UNEBAxpqrdo', '\x43eb7661001747ed9b28ad716987cbf5bbc8c504a681af9a20aa56748064e0f5487b8e901b97ddacc312ec6621f1088a', true, NULL, '2016-03-15 07:23:25.280603+00', '2016-03-15 06:59:07.519224+00', NULL, NULL, NULL, NULL);
INSERT INTO users VALUES (2, 'chandra', 'UserManagement', '2016-03-15 09:45:26.93175+00', '2015-12-25 16:19:52.777615+00', 'chandra@rekadia.co.id', 'R6EYij85SbCNDPTuUdP--sMRdjCPll7EjklP7NmFAw62rp4', '\xda8d4e7d934450bfd6b914591d974c7a21f37b4e788d773ab6278d6d9403443d1336cf46b378185f139e97f4c1757de4', true, NULL, '2016-03-15 09:45:26.928749+00', '2016-01-07 03:00:17.55667+00', NULL, NULL, NULL, NULL);


--
-- TOC entry 2230 (class 0 OID 16657)
-- Dependencies: 194
-- Data for Name: users_roles; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO users_roles VALUES (2, 1);
INSERT INTO users_roles VALUES (2, 2);
INSERT INTO users_roles VALUES (3, 3);
INSERT INTO users_roles VALUES (4, 3);
INSERT INTO users_roles VALUES (23, 3);
INSERT INTO users_roles VALUES (24, 3);


--
-- TOC entry 2250 (class 0 OID 0)
-- Dependencies: 189
-- Name: users_user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('users_user_id_seq', 24, true);


--
-- TOC entry 2231 (class 0 OID 16715)
-- Dependencies: 198
-- Data for Name: versions; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO versions VALUES ('application', '1.7');


--
-- TOC entry 2095 (class 2606 OID 16730)
-- Name: pk_action; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "Actions"
    ADD CONSTRAINT pk_action PRIMARY KEY ("Id");


--
-- TOC entry 2097 (class 2606 OID 16735)
-- Name: pk_actions_in_modules; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "ActionsInModules"
    ADD CONSTRAINT pk_actions_in_modules PRIMARY KEY ("ActionId", "ModuleId");


--
-- TOC entry 2101 (class 2606 OID 16777)
-- Name: pk_aimc; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "ActionsInModulesChosen"
    ADD CONSTRAINT pk_aimc PRIMARY KEY ("ModuleInRoleId", "ActionId");


--
-- TOC entry 2099 (class 2606 OID 16772)
-- Name: pk_mir; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "ModulesInRoles"
    ADD CONSTRAINT pk_mir PRIMARY KEY ("Id");


--
-- TOC entry 2093 (class 2606 OID 16725)
-- Name: pk_module; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "Modules"
    ADD CONSTRAINT pk_module PRIMARY KEY ("Id");


--
-- TOC entry 2087 (class 2606 OID 16655)
-- Name: pk_user_login_activity; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY user_login_activity
    ADD CONSTRAINT pk_user_login_activity PRIMARY KEY (activity_id);


--
-- TOC entry 2080 (class 2606 OID 16631)
-- Name: pk_users; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY users
    ADD CONSTRAINT pk_users PRIMARY KEY (user_id);


--
-- TOC entry 2083 (class 2606 OID 16645)
-- Name: roles_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (role_id);


--
-- TOC entry 2089 (class 2606 OID 16661)
-- Name: users_roles_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY users_roles
    ADD CONSTRAINT users_roles_pkey PRIMARY KEY (user_id, role_id);


--
-- TOC entry 2091 (class 2606 OID 16719)
-- Name: versions_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY versions
    ADD CONSTRAINT versions_pkey PRIMARY KEY (name);


--
-- TOC entry 2085 (class 1259 OID 16656)
-- Name: ix_user_login_activity_user_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_user_login_activity_user_id ON user_login_activity USING btree (user_id, "when");


--
-- TOC entry 2077 (class 1259 OID 16712)
-- Name: ix_users_email; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_users_email ON users USING btree (lower((application_name)::text), lower((email)::text));


--
-- TOC entry 2078 (class 1259 OID 16713)
-- Name: ix_users_last_activity; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_users_last_activity ON users USING btree (lower((application_name)::text), last_activity);


--
-- TOC entry 2084 (class 1259 OID 16711)
-- Name: ux_roles_role_name_application_name; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX ux_roles_role_name_application_name ON roles USING btree (lower((role_name)::text), lower((application_name)::text));


--
-- TOC entry 2081 (class 1259 OID 16714)
-- Name: ux_users_user_name_application_name; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX ux_users_user_name_application_name ON users USING btree (lower((application_name)::text), lower((user_name)::text));


--
-- TOC entry 2104 (class 2606 OID 16736)
-- Name: fk_aim_action; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "ActionsInModules"
    ADD CONSTRAINT fk_aim_action FOREIGN KEY ("ActionId") REFERENCES "Actions"("Id");


--
-- TOC entry 2105 (class 2606 OID 16741)
-- Name: fk_aim_module; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "ActionsInModules"
    ADD CONSTRAINT fk_aim_module FOREIGN KEY ("ModuleId") REFERENCES "Modules"("Id");


--
-- TOC entry 2109 (class 2606 OID 16783)
-- Name: fk_aimc_action; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "ActionsInModulesChosen"
    ADD CONSTRAINT fk_aimc_action FOREIGN KEY ("ActionId") REFERENCES "Actions"("Id");


--
-- TOC entry 2108 (class 2606 OID 16778)
-- Name: fk_aimc_mir; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "ActionsInModulesChosen"
    ADD CONSTRAINT fk_aimc_mir FOREIGN KEY ("ModuleInRoleId") REFERENCES "ModulesInRoles"("Id");


--
-- TOC entry 2106 (class 2606 OID 16761)
-- Name: fk_mir_module; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "ModulesInRoles"
    ADD CONSTRAINT fk_mir_module FOREIGN KEY ("ModuleId") REFERENCES "Modules"("Id");


--
-- TOC entry 2107 (class 2606 OID 16766)
-- Name: fk_mir_role; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY "ModulesInRoles"
    ADD CONSTRAINT fk_mir_role FOREIGN KEY ("RoleId") REFERENCES roles(role_id);


--
-- TOC entry 2102 (class 2606 OID 16662)
-- Name: users_roles_role_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY users_roles
    ADD CONSTRAINT users_roles_role_id_fkey FOREIGN KEY (role_id) REFERENCES roles(role_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 2103 (class 2606 OID 16667)
-- Name: users_roles_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY users_roles
    ADD CONSTRAINT users_roles_user_id_fkey FOREIGN KEY (user_id) REFERENCES users(user_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 2243 (class 0 OID 0)
-- Dependencies: 5
-- Name: public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;


-- Completed on 2016-03-25 14:07:15

--
-- PostgreSQL database dump complete
--

