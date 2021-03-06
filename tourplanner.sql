CREATE TABLE TOUR(
	ID SERIAL PRIMARY KEY,
	NAME VARCHAR(75) NOT NULL UNIQUE,
	STARTLOCATION VARCHAR(150) NOT NULL,
	ENDLOCATION VARCHAR(150) NOT NULL,
	ROUTEINFO VARCHAR(250) NOT NULL,
	DISTANCE DECIMAL NOT NULL,
	ROUTETYPE INT NOT NULL,
	DESCRIPTION TEXT
);

CREATE TABLE TOURLOG(
	ID SERIAL PRIMARY KEY,
	TOURID INT NOT NULL,
	STARTDATE TIMESTAMP NOT NULL,
	ENDDATE TIMESTAMP NOT NULL,
	DISTANCE DECIMAL NOT NULL,
	TOTALTIME DECIMAL NOT NULL,
	RATING DECIMAL NOT NULL,
	AVERAGESPEED DECIMAL NOT NULL,
	WEATHER INT NOT NULL,
	TEMPERATURE DECIMAL NOT NULL,
	TRAVELMETHOD INT NOT NULL,
	REPORT TEXT,
	CONSTRAINT fk_tourlog FOREIGN KEY(TOURID) REFERENCES TOUR(ID) ON DELETE CASCADE
);

CREATE TABLE MANEUVER(
	ID SERIAL PRIMARY KEY,
	TOURID INT NOT NULL,
	NARRATIVE TEXT NOT NULL,
	DISTANCE DECIMAL NOT NULL,
	CONSTRAINT fk_maneuvers FOREIGN KEY(TOURID) REFERENCES TOUR(ID) ON DELETE CASCADE 
);
