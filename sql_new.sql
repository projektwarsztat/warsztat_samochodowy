CREATE TABLE Adres
(
    "ID_Adres" INT PRIMARY KEY,
    "Ulica" VARCHAR(255),
    "Numer" VARCHAR(50),
    "Kod_pocztowy" VARCHAR(10),
    "Miejscowosc" VARCHAR(100)
);

CREATE TABLE Czesc
(
    "ID_Czesci" INT PRIMARY KEY,
    "Nazwa" VARCHAR(255),
    "Cena" DECIMAL(10, 2)
);

CREATE TABLE Dane_logowania
(
    "ID_Dane_logowania" INT PRIMARY KEY,
    "Login" VARCHAR(255),
    "Haslo" VARCHAR(255)
);

CREATE TABLE Faktura
(
    "ID_Faktura" INT PRIMARY KEY,
    "ID_Warsztat" INT,
    "ID_Klient" INT,
    "ID_Naprawa" INT
);

CREATE TABLE Klient
(
    "ID_Klient" INT PRIMARY KEY,
    "Imie" VARCHAR(100),
    "Nazwisko" VARCHAR(100),
    "ID_Adres" INT,
    "Telefon" BIGINT
);

CREATE TABLE Warsztat (
    "ID_Warsztat" INT PRIMARY KEY,
    "Nazwa" VARCHAR(255),
    "ID_Adres" INT,
    "Telefon" INT,
    "NIP" VARCHAR(20),
    "Numer_konta_bankowego" VARCHAR(50),
    "Nazwa_banku" VARCHAR(255)
);

CREATE TABLE Uzyte_czesci (
    "ID" INT PRIMARY KEY,
    "ID_Naprawa" INT,
    "ID_Czesci" INT,
    "Ilosc" INT
);

CREATE TABLE Pracownik (
    "ID_Pracownik" INT PRIMARY KEY,
    "Imie" VARCHAR(255) NOT NULL,
    "Nazwisko" VARCHAR(255) NOT NULL,
    "ID_Adres" INT,
    "Telefon" INT,
    "ID_Dane_logowania" INT
);

CREATE TABLE Pojazd (
    "Numer_rejestracyjny" VARCHAR(20) PRIMARY KEY,
    "ID_Klient" INT,
    "Marka" VARCHAR(255),
    "Model" VARCHAR(255),
    "Numer_VIN" VARCHAR(50),
    "Rok_produkcji" INT,
    "Typ_paliwa" VARCHAR(50)
);

CREATE TABLE Naprawa (
    "ID_Naprawa" INT PRIMARY KEY,
    "Numer_rejestracyjny" VARCHAR(255),
    "ID_Pracownik" INT,
    "Data_przyjecia" DATE,
    "Data_wydania" DATE,
    "Status_naprawy" VARCHAR(50),
    "Opis_usterek" TEXT,
    "Wiadomosc_zwrotna" TEXT
);

ALTER TABLE Warsztat ADD CONSTRAINT "Warsztat_fk0" FOREIGN KEY ("ID_Adres") REFERENCES Adres("ID_Adres");
ALTER TABLE Pracownik ADD CONSTRAINT "Pracownik_fk0" FOREIGN KEY ("ID_Adres") REFERENCES Adres("ID_Adres");
ALTER TABLE Pojazd ADD CONSTRAINT "Pojazd_fk0" FOREIGN KEY ("ID_Klient") REFERENCES Klient("ID_Klient");
ALTER TABLE Klient ADD CONSTRAINT "Klient_fk0" FOREIGN KEY ("ID_Adres") REFERENCES Adres("ID_Adres");
ALTER TABLE Naprawa ADD CONSTRAINT "Naprawa_fk0" FOREIGN KEY ("Numer_rejestracyjny") REFERENCES Pojazd("Numer_rejestracyjny");
ALTER TABLE Naprawa ADD CONSTRAINT "Naprawa_fk1" FOREIGN KEY ("ID_Pracownik") REFERENCES Pracownik("ID_Pracownik");
ALTER TABLE Uzyte_czesci ADD CONSTRAINT "Uzyte_czesci_fk0" FOREIGN KEY ("ID_Naprawa") REFERENCES Naprawa("ID_Naprawa");
ALTER TABLE Uzyte_czesci ADD CONSTRAINT "Uzyte_czesci_fk1" FOREIGN KEY ("ID_Czesci") REFERENCES Czesc("ID_Czesci");
ALTER TABLE Faktura ADD CONSTRAINT "Faktura_fk0" FOREIGN KEY ("ID_Warsztat") REFERENCES Warsztat("ID_Warsztat");
ALTER TABLE Faktura ADD CONSTRAINT "Faktura_fk1" FOREIGN KEY ("ID_Klient") REFERENCES Klient("ID_Klient");
ALTER TABLE Faktura ADD CONSTRAINT "Faktura_fk2" FOREIGN KEY ("ID_Naprawa") REFERENCES Naprawa("ID_Naprawa");

