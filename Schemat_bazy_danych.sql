--Napisany zgodnie ze schematem bazy danych (Schemat_bazy_danych.png)

CREATE TABLE Warsztat (
	Nazwa varchar(100) PRIMARY KEY,
	Adres integer NOT NULL,
	Telefon integer NOT NULL,
	NIP varchar(12) NOT NULL,
	Numer_konta_bankowego varchar(28) NOT NULL,
	Nazwa_banku varchar(50) NOT NULL
);

CREATE TABLE Adres (
	ID_Adres integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
	Miejscowosc varchar(50) NOT NULL,
	Ulica varchar(50) NOT NULL,
	Numer varchar(10) NOT NULL,
	Kod_pocztowy varchar(6) NOT NULL
);

CREATE TABLE Pracownik (
	ID_Pracownik integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
	Imie varchar(50) NOT NULL,
	Nazwisko varchar(50) NOT NULL,
	Adres integer NOT NULL,
	Telefon integer NOT NULL
);

CREATE TABLE Pojazd (
	Numer_rejestracyjny varchar(7) PRIMARY KEY,
	Wlasciciel integer NOT NULL,
	Marka varchar(20) NOT NULL,
	Model varchar(20) NOT NULL,
	Numer_VIM varchar(17) NOT NULL UNIQUE,
	Rok_produkcji integer NOT NULL,
	Typ_paliwa varchar(15) NOT NULL
);

CREATE TABLE Klient (
	ID_Klient integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
	Imie varchar(50) NOT NULL,
	Nazwisko varchar(50) NOT NULL,
	Adres integer NOT NULL,
	Telefon integer NOT NULL
);

CREATE TABLE Naprawa (
	ID_Naprawy integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
	Numer_rejestracyjny varchar(7) NOT NULL,
	Mechanik integer NOT NULL,
	Data_przyjecia DATE NOT NULL,
	Data_wydania DATE NOT NULL,
	Status_naprawy varchar(15) NOT NULL,
	Opis_usterek varchar(500) NOT NULL
);

CREATE TABLE Czesc (
	ID_Czesci integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
	Nazwa varchar(60) NOT NULL,
	Cena FLOAT NOT NULL
);

CREATE TABLE Uzyte_czesci (
	Naprawa integer NOT NULL,
	Czesci integer NOT NULL,
	Ilosc integer NOT NULL
);

CREATE TABLE Faktura (
	ID_Faktura integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
	Warsztat varchar(100) NOT NULL,
	Klient integer NOT NULL,
	Naprawa integer NOT NULL
);

ALTER TABLE Warsztat ADD CONSTRAINT Warsztat_fk0 FOREIGN KEY (Adres) REFERENCES Adres(ID_Adres);
ALTER TABLE Pracownik ADD CONSTRAINT Pracownik_fk0 FOREIGN KEY (Adres) REFERENCES Adres(ID_Adres);
ALTER TABLE Pojazd ADD CONSTRAINT Pojazd_fk0 FOREIGN KEY (Wlasciciel) REFERENCES Klient(ID_Klient);
ALTER TABLE Klient ADD CONSTRAINT Klient_fk0 FOREIGN KEY (Adres) REFERENCES Adres(ID_Adres);
ALTER TABLE Naprawa ADD CONSTRAINT Naprawa_fk0 FOREIGN KEY (Numer_rejestracyjny) REFERENCES Pojazd(Numer_rejestracyjny);
ALTER TABLE Naprawa ADD CONSTRAINT Naprawa_fk1 FOREIGN KEY (Mechanik) REFERENCES Pracownik(ID_Pracownik);
ALTER TABLE Uzyte_czesci ADD CONSTRAINT Uzyte_czesci_fk0 FOREIGN KEY (Naprawa) REFERENCES Naprawa(ID_Naprawy);
ALTER TABLE Uzyte_czesci ADD CONSTRAINT Uzyte_czesci_fk1 FOREIGN KEY (Czesci) REFERENCES Czesc(ID_Czesci);
ALTER TABLE Faktura ADD CONSTRAINT Faktura_fk0 FOREIGN KEY (Warsztat) REFERENCES Warsztat(Nazwa);
ALTER TABLE Faktura ADD CONSTRAINT Faktura_fk1 FOREIGN KEY (Klient) REFERENCES Klient(ID_Klient);
ALTER TABLE Faktura ADD CONSTRAINT Faktura_fk2 FOREIGN KEY (Naprawa) REFERENCES Naprawa(ID_Naprawy);