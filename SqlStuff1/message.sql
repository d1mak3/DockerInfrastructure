DROP TABLE IF EXISTS messenger_database.message;

CREATE TABLE messenger_database.messages (
	Id INT PRIMARY KEY,
	Sender VARCHAR(50),
	Content VARCHAR(200)
);