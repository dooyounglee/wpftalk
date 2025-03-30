-- kakao."USER" definition

-- Drop table

-- DROP TABLE kakao."USER";

CREATE TABLE kakao."USER" (
	usr_no int8 NOT NULL,
	usr_nm varchar NOT NULL,
	div_no int8 NULL,
	usr_id varchar NULL,
	"password" varchar NULL,
	CONSTRAINT user_pk PRIMARY KEY (usr_no)
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