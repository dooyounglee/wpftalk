﻿--docker run -p 5432:5432 --name postgres_talk -e POSTGRES_PASSWORD=postgres -e TZ=Asia/Seoul -d postgres:17.4-bookworm

-- DROP SCHEMA talk;
CREATE SCHEMA talk AUTHORIZATION postgres;

-- DROP TABLE talk."user";
CREATE TABLE talk."user" (
	usr_no int8 NOT NULL,
	usr_nm varchar NOT NULL,
	div_no int8 NULL,
	usr_id varchar NULL,
	usr_pw varchar NULL,
	CONSTRAINT user_pk PRIMARY KEY (usr_no)
);

-- DROP TABLE talk.div;
CREATE TABLE talk.div (
	div_no int8 NOT NULL,
	div_nm varchar NOT NULL,
	CONSTRAINT div_pk PRIMARY KEY (div_no)
);

-- DROP TABLE talk.chat;
CREATE TABLE talk.chat (
	chat_no int8 NOT NULL,
	chat text NULL,
	chat_fg varchar NOT NULL,
	room_no int8 NOT NULL,
	usr_no int8 NOT NULL,
	rgt_dtm varchar NULL,
	CONSTRAINT chat_pk PRIMARY KEY (chat_no)
);

-- DROP TABLE talk.chatuser;
CREATE TABLE talk.chatuser (
	room_no int8 NOT NULL,
	usr_no int8 NOT NULL,
	title varchar NOT NULL,
	chat_no int8,
	CONSTRAINT chatuser_pk PRIMARY KEY (room_no,usr_no)
);

-- DROP TABLE talk.room;
CREATE TABLE talk.room (
	room_no int8 NOT NULL,
	usr_no int8 NOT NULL,
	title varchar NOT NULL,
	rgt_dtm varchar NULL,
	CONSTRAINT room_pk PRIMARY KEY (room_no)
);