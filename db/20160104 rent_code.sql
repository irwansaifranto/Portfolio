--
-- PostgreSQL database dump
--

-- Dumped from database version 9.5rc1
-- Dumped by pg_dump version 9.5rc1

-- Started on 2016-01-04 13:11:34

SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

SET search_path = public, pg_catalog;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- TOC entry 189 (class 1259 OID 25062)
-- Name: rent_code; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE rent_code (
    code character varying(10) NOT NULL
);


ALTER TABLE rent_code OWNER TO postgres;

--
-- TOC entry 2012 (class 2606 OID 25066)
-- Name: pk_rent_code; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent_code
    ADD CONSTRAINT pk_rent_code PRIMARY KEY (code);


-- Completed on 2016-01-04 13:11:34

--
-- PostgreSQL database dump complete
--

