--
-- PostgreSQL database dump
--

-- Dumped from database version 9.5rc1
-- Dumped by pg_dump version 9.5rc1

-- Started on 2015-12-31 10:16:00

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
-- TOC entry 2176 (class 0 OID 0)
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
    is_active boolean DEFAULT true NOT NULL,
    id_owner uuid NOT NULL
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
-- TOC entry 2164 (class 0 OID 16451)
-- Dependencies: 184
-- Data for Name: car; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO car VALUES ('bdbb36a4-ff64-4e68-9d07-4ac52e48d821', '125700cd-f447-4a6c-9e19-206c36a8369e', 'I 1234 AA', 6, 'AVAILABLE', true, 'd021a28b-6aca-4bbf-930c-935ff63691e9');
INSERT INTO car VALUES ('892b1090-d859-4a06-8922-932bdf7e9d40', '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', 'A 1234 AA', 6, 'AVAILABLE', true, 'd021a28b-6aca-4bbf-930c-935ff63691e9');
INSERT INTO car VALUES ('4b96c548-f6b3-420a-8a20-000388bc0bb4', '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', 'A 1122 AA', 6, 'AVAILABLE', true, 'd021a28b-6aca-4bbf-930c-935ff63691e9');
INSERT INTO car VALUES ('4ac204b3-fc1d-4cc3-bd46-132d2e3c8ca7', '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', 'A 2233 BB', 6, 'AVAILABLE', true, 'f9a31c82-f545-45b4-b25e-90d5c023d3a5');


--
-- TOC entry 2165 (class 0 OID 16454)
-- Dependencies: 185
-- Data for Name: car_brand; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO car_brand VALUES ('eac29cfa-fc1b-4e32-85dd-804c5eb3b576', 'Toyota');
INSERT INTO car_brand VALUES ('9572d6ec-a2df-4407-b2f8-f06d61dd122f', 'Honda');


--
-- TOC entry 2163 (class 0 OID 16441)
-- Dependencies: 183
-- Data for Name: car_model; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO car_model VALUES ('545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', 'Avanza', 'eac29cfa-fc1b-4e32-85dd-804c5eb3b576');
INSERT INTO car_model VALUES ('125700cd-f447-4a6c-9e19-206c36a8369e', 'Innova', 'eac29cfa-fc1b-4e32-85dd-804c5eb3b576');
INSERT INTO car_model VALUES ('913e8705-f367-4e00-a6a5-6b403156c79b', 'Freed', '9572d6ec-a2df-4407-b2f8-f06d61dd122f');


--
-- TOC entry 2161 (class 0 OID 16415)
-- Dependencies: 181
-- Data for Name: customer; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO customer VALUES ('e9b9d948-0a77-4089-aca0-324b913d0d34', 'Chandra', 'CORPORATE', '085722925900', NULL, NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9');
INSERT INTO customer VALUES ('46d30608-492e-44a1-9cfe-37b5584ed4c1', 'Hutami', 'Corporate', '08112210960', NULL, NULL, NULL, NULL, 'f9a31c82-f545-45b4-b25e-90d5c023d3a5');
INSERT INTO customer VALUES ('199d978b-3eb1-4598-9613-84928b05f9fc', 'Lauw Siu Hung', 'CORPORATE', '08122356566', 'Veteran 84', NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9');


--
-- TOC entry 2162 (class 0 OID 16428)
-- Dependencies: 182
-- Data for Name: driver; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO driver VALUES ('ff0c7f36-6c3b-413b-9311-4d4a0149cfc4', 'Suryono', 'PARTTIME', NULL, NULL, NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9');
INSERT INTO driver VALUES ('807b9a6c-7c5b-44f8-ae33-a07b424dc8a2', 'Johnny', 'PARTTIME', NULL, NULL, NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9');
INSERT INTO driver VALUES ('1d4a3bfe-a585-42e8-9b67-17f4309108e6', 'Bobon', 'PARTTIME', NULL, NULL, NULL, NULL, NULL, 'f9a31c82-f545-45b4-b25e-90d5c023d3a5');


--
-- TOC entry 2167 (class 0 OID 16521)
-- Dependencies: 187
-- Data for Name: invoice; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 2160 (class 0 OID 16394)
-- Dependencies: 180
-- Data for Name: owner; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO owner VALUES ('d021a28b-6aca-4bbf-930c-935ff63691e9', 'RK', 'Rekadia', 'superadmin', '2015-01-01 00:00:00+07', NULL, NULL);
INSERT INTO owner VALUES ('f9a31c82-f545-45b4-b25e-90d5c023d3a5', 'AM', 'Ameco', 'superadmin', '2015-02-02 00:00:00+07', NULL, NULL);


--
-- TOC entry 2168 (class 0 OID 16801)
-- Dependencies: 188
-- Data for Name: owner_user; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO owner_user VALUES ('d021a28b-6aca-4bbf-930c-935ff63691e9', 'reynhart', 'e59fc4a1-11b7-44ee-9c75-73a645693269');


--
-- TOC entry 2166 (class 0 OID 16471)
-- Dependencies: 186
-- Data for Name: rent; Type: TABLE DATA; Schema: public; Owner: postgres
--



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
-- TOC entry 2038 (class 2606 OID 16868)
-- Name: fk_car_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car
    ADD CONSTRAINT fk_car_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


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
-- TOC entry 2044 (class 2606 OID 16530)
-- Name: fk_invoice_rent; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY invoice
    ADD CONSTRAINT fk_invoice_rent FOREIGN KEY (id_rent) REFERENCES rent(id);


--
-- TOC entry 2045 (class 2606 OID 16809)
-- Name: fk_ou_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY owner_user
    ADD CONSTRAINT fk_ou_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2043 (class 2606 OID 16499)
-- Name: fk_rent_car; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_car FOREIGN KEY (id_car) REFERENCES car(id);


--
-- TOC entry 2042 (class 2606 OID 16494)
-- Name: fk_rent_car_model; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_car_model FOREIGN KEY (id_car_model) REFERENCES car_model(id);


--
-- TOC entry 2039 (class 2606 OID 16476)
-- Name: fk_rent_customer; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_customer FOREIGN KEY (id_customer) REFERENCES customer(id);


--
-- TOC entry 2041 (class 2606 OID 16489)
-- Name: fk_rent_driver; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_driver FOREIGN KEY (id_driver) REFERENCES driver(id);


--
-- TOC entry 2040 (class 2606 OID 16484)
-- Name: fk_rent_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2175 (class 0 OID 0)
-- Dependencies: 5
-- Name: public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;


-- Completed on 2015-12-31 10:16:01

--
-- PostgreSQL database dump complete
--

