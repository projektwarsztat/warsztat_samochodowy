CREATE TABLE Adres
(
    ID_Adres INT PRIMARY KEY,
    Ulica VARCHAR(255),
    Numer VARCHAR(50),
    Kod_pocztowy VARCHAR(10),
    Miejscowosc VARCHAR(100)
);

CREATE TABLE Czesc
(
    ID_Czesci INT PRIMARY KEY,
    Nazwa VARCHAR(255),
    Cena DECIMAL(10, 2)
);

