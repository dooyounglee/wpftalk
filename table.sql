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

-- kakao.chat definition

-- Drop table

-- DROP TABLE kakao.chat;

CREATE TABLE kakao.chat (
	chat_no int8 NOT NULL,
	chat text NULL,
	chat_fg varchar NOT NULL,
	room_no int8 NOT NULL,
	usr_no int8 NOT NULL,
	rgt_dtm varchar NULL
);

-- kakao.chatuser definition

-- Drop table

-- DROP TABLE kakao.chatuser;

CREATE TABLE kakao.chatuser (
	room_no int8 NOT NULL,
	usr_no int8 NOT NULL,
	room_title varchar NOT NULL
);

-- kakao.div definition

-- Drop table

-- DROP TABLE kakao.div;

CREATE TABLE kakao.div (
	div_no int8 NOT NULL,
	div_nm varchar NOT NULL
);

-- kakao.room definition

-- Drop table

-- DROP TABLE kakao.room;

CREATE TABLE kakao.room (
	room_no int8 NOT NULL,
	usr_no int8 NOT NULL,
	title varchar NOT NULL,
	rgt_dtm varchar NULL
);