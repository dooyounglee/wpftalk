--docker run -p 5432:5432 --name postgres_talk -e POSTGRES_PASSWORD=postgres -e TZ=Asia/Seoul -d postgres:17.4-bookworm

-- DROP SCHEMA talk;
CREATE SCHEMA talk AUTHORIZATION postgres;

-- DROP TABLE talk."USER";
CREATE TABLE talk."USER" (
	usr_no int8 NOT NULL,
	usr_nm varchar NOT NULL,
	div_no int8 NULL,
	usr_id varchar NULL,
	usr_pw varchar NULL,
	CONSTRAINT usr_pk PRIMARY KEY (usr_no)
);

-- DROP TABLE talk.div;
CREATE TABLE talk.div (
	div_no int8 NOT NULL,
	div_nm varchar NOT NULL
);

-- DROP TABLE talk.chat;
CREATE TABLE talk.chat (
	chat_no int8 NOT NULL,
	chat text NULL,
	chat_fg varchar NOT NULL,
	room_no int8 NOT NULL,
	usr_no int8 NOT NULL,
	rgt_dtm varchar NULL
);

-- DROP TABLE talk.chatuser;
CREATE TABLE talk.chatuser (
	room_no int8 NOT NULL,
	usr_no int8 NOT NULL,
	title varchar NOT NULL
);

-- DROP TABLE talk.room;
CREATE TABLE talk.room (
	room_no int8 NOT NULL,
	usr_no int8 NOT NULL,
	title varchar NOT NULL,
	rgt_dtm varchar NULL
);