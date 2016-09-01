--
-- PostgreSQL database dump
--

-- Dumped from database version 9.5rc1
-- Dumped by pg_dump version 9.5rc1

-- Started on 2015-12-26 21:28:38

SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 189 (class 3079 OID 12355)
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- TOC entry 2175 (class 0 OID 0)
-- Dependencies: 189
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET search_path = public, pg_catalog;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- TOC entry 184 (class 1259 OID 16451)
-- Name: car; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE car (
    id uuid NOT NULL,
    id_car_model uuid NOT NULL,
    license_plate character varying(50) NOT NULL,
    capacity integer NOT NULL,
    status character varying(50) NOT NULL,
    is_active boolean DEFAULT true NOT NULL
);


ALTER TABLE car OWNER TO postgres;

--
-- TOC entry 185 (class 1259 OID 16454)
-- Name: car_brand; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE car_brand (
    id uuid NOT NULL,
    name character varying(50)
);


ALTER TABLE car_brand OWNER TO postgres;

--
-- TOC entry 183 (class 1259 OID 16441)
-- Name: car_model; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE car_model (
    id uuid NOT NULL,
    name character varying(100) NOT NULL,
    id_car_brand uuid NOT NULL
);


ALTER TABLE car_model OWNER TO postgres;

--
-- TOC entry 181 (class 1259 OID 16415)
-- Name: customer; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE customer (
    id uuid NOT NULL,
    name character varying(100) NOT NULL,
    customer_type character varying(50),
    phone_number character varying(100),
    address text,
    city character varying(100),
    email text,
    notes text,
    id_owner uuid NOT NULL
);


ALTER TABLE customer OWNER TO postgres;

--
-- TOC entry 182 (class 1259 OID 16428)
-- Name: driver; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE driver (
    id uuid NOT NULL,
    name text NOT NULL,
    driver_type character varying(50) NOT NULL,
    phone_number character varying(100),
    address text,
    city text,
    email text,
    notes text,
    id_owner uuid NOT NULL
);


ALTER TABLE driver OWNER TO postgres;

--
-- TOC entry 187 (class 1259 OID 16521)
-- Name: invoice; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE invoice (
    id uuid NOT NULL,
    id_rent uuid NOT NULL,
    price integer NOT NULL,
    code character varying(50) NOT NULL,
    invoice_date date NOT NULL,
    status character varying(50) NOT NULL,
    cancel_notes text,
    created_by character varying(50) NOT NULL,
    created_time timestamp with time zone NOT NULL,
    updated_by character varying(50),
    updated_time timestamp with time zone
);


ALTER TABLE invoice OWNER TO postgres;

--
-- TOC entry 180 (class 1259 OID 16394)
-- Name: owner; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE owner (
    id uuid NOT NULL,
    code character varying(5) NOT NULL,
    name character varying(100) NOT NULL,
    created_by character varying(50) NOT NULL,
    created_time timestamp with time zone NOT NULL,
    updated_by character varying(50),
    updated_time timestamp with time zone
);


ALTER TABLE owner OWNER TO postgres;

--
-- TOC entry 188 (class 1259 OID 16801)
-- Name: owner_user; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE owner_user (
    id_owner uuid NOT NULL,
    username character varying(250) NOT NULL,
    id uuid NOT NULL
);


ALTER TABLE owner_user OWNER TO postgres;

--
-- TOC entry 186 (class 1259 OID 16471)
-- Name: rent; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE rent (
    id uuid NOT NULL,
    id_customer uuid NOT NULL,
    id_owner uuid NOT NULL,
    pickup_location text,
    id_driver uuid,
    id_car_model uuid NOT NULL,
    id_car uuid,
    start_rent timestamp with time zone NOT NULL,
    finish_rent timestamp with time zone NOT NULL,
    price integer NOT NULL,
    notes text,
    code character varying(50) NOT NULL,
    created_by character varying(50) NOT NULL,
    created_time timestamp with time zone NOT NULL,
    updated_by character varying(50),
    updated_time timestamp with time zone,
    status character varying(50),
    cancel_notes text
);


ALTER TABLE rent OWNER TO postgres;

--
-- TOC entry 2163 (class 0 OID 16451)
-- Dependencies: 184
-- Data for Name: car; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY car (id, id_car_model, license_plate, capacity, status, is_active) FROM stdin;
\.


--
-- TOC entry 2164 (class 0 OID 16454)
-- Dependencies: 185
-- Data for Name: car_brand; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY car_brand (id, name) FROM stdin;
\.


--
-- TOC entry 2162 (class 0 OID 16441)
-- Dependencies: 183
-- Data for Name: car_model; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY car_model (id, name, id_car_brand) FROM stdin;
\.


--
-- TOC entry 2160 (class 0 OID 16415)
-- Dependencies: 181
-- Data for Name: customer; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY customer (id, name, customer_type, phone_number, address, city, email, notes, id_owner) FROM stdin;
\.


--
-- TOC entry 2161 (class 0 OID 16428)
-- Dependencies: 182
-- Data for Name: driver; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY driver (id, name, driver_type, phone_number, address, city, email, notes, id_owner) FROM stdin;
\.


--
-- TOC entry 2166 (class 0 OID 16521)
-- Dependencies: 187
-- Data for Name: invoice; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY invoice (id, id_rent, price, code, invoice_date, status, cancel_notes, created_by, created_time, updated_by, updated_time) FROM stdin;
\.


--
-- TOC entry 2159 (class 0 OID 16394)
-- Dependencies: 180
-- Data for Name: owner; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY owner (id, code, name, created_by, created_time, updated_by, updated_time) FROM stdin;
\.


--
-- TOC entry 2167 (class 0 OID 16801)
-- Dependencies: 188
-- Data for Name: owner_user; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY owner_user (id_owner, username, id) FROM stdin;
\.


--
-- TOC entry 2165 (class 0 OID 16471)
-- Dependencies: 186
-- Data for Name: rent; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY rent (id, id_customer, id_owner, pickup_location, id_driver, id_car_model, id_car, start_rent, finish_rent, price, notes, code, created_by, created_time, updated_by, updated_time, status, cancel_notes) FROM stdin;
\.


--
-- TOC entry 2017 (class 2606 OID 16402)
-- Name: Primary Key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY owner
    ADD CONSTRAINT "Primary Key" PRIMARY KEY (id);


--
-- TOC entry 2025 (class 2606 OID 16465)
-- Name: pk_car; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car
    ADD CONSTRAINT pk_car PRIMARY KEY (id);


--
-- TOC entry 2027 (class 2606 OID 16458)
-- Name: pk_car_brand; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car_brand
    ADD CONSTRAINT pk_car_brand PRIMARY KEY (id);


--
-- TOC entry 2023 (class 2606 OID 16445)
-- Name: pk_car_type; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car_model
    ADD CONSTRAINT pk_car_type PRIMARY KEY (id);


--
-- TOC entry 2019 (class 2606 OID 16422)
-- Name: pk_customer; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY customer
    ADD CONSTRAINT pk_customer PRIMARY KEY (id);


--
-- TOC entry 2021 (class 2606 OID 16435)
-- Name: pk_driver; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY driver
    ADD CONSTRAINT pk_driver PRIMARY KEY (id);


--
-- TOC entry 2031 (class 2606 OID 16529)
-- Name: pk_invoice; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY invoice
    ADD CONSTRAINT pk_invoice PRIMARY KEY (id);


--
-- TOC entry 2033 (class 2606 OID 16808)
-- Name: pk_owner_user; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY owner_user
    ADD CONSTRAINT pk_owner_user PRIMARY KEY (id);


--
-- TOC entry 2029 (class 2606 OID 16475)
-- Name: pk_rent; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT pk_rent PRIMARY KEY (id);


--
-- TOC entry 2037 (class 2606 OID 16466)
-- Name: fk_car_car_model; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car
    ADD CONSTRAINT fk_car_car_model FOREIGN KEY (id_car_model) REFERENCES car_model(id);


--
-- TOC entry 2036 (class 2606 OID 16459)
-- Name: fk_car_model_car_brand; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car_model
    ADD CONSTRAINT fk_car_model_car_brand FOREIGN KEY (id_car_brand) REFERENCES car_brand(id);


--
-- TOC entry 2034 (class 2606 OID 16423)
-- Name: fk_customer_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY customer
    ADD CONSTRAINT fk_customer_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2035 (class 2606 OID 16436)
-- Name: fk_driver_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY driver
    ADD CONSTRAINT fk_driver_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2043 (class 2606 OID 16530)
-- Name: fk_invoice_rent; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY invoice
    ADD CONSTRAINT fk_invoice_rent FOREIGN KEY (id_rent) REFERENCES rent(id);


--
-- TOC entry 2044 (class 2606 OID 16809)
-- Name: fk_ou_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY owner_user
    ADD CONSTRAINT fk_ou_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2042 (class 2606 OID 16499)
-- Name: fk_rent_car; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_car FOREIGN KEY (id_car) REFERENCES car(id);


--
-- TOC entry 2041 (class 2606 OID 16494)
-- Name: fk_rent_car_model; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_car_model FOREIGN KEY (id_car_model) REFERENCES car_model(id);


--
-- TOC entry 2038 (class 2606 OID 16476)
-- Name: fk_rent_customer; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_customer FOREIGN KEY (id_customer) REFERENCES customer(id);


--
-- TOC entry 2040 (class 2606 OID 16489)
-- Name: fk_rent_driver; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_driver FOREIGN KEY (id_driver) REFERENCES driver(id);


--
-- TOC entry 2039 (class 2606 OID 16484)
-- Name: fk_rent_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2174 (class 0 OID 0)
-- Dependencies: 5
-- Name: public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;


-- Completed on 2015-12-26 21:28:42

--
-- PostgreSQL database dump complete
--

