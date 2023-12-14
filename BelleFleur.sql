DROP DATABASE IF EXISTS floral;
CREATE DATABASE floral;
use floral;
set sql_safe_updates=0;
--
drop table if exists Client;
drop table if exists Employe;
drop table if exists Bouquet_Standard;
drop table if exists Bouquet_personnalise;
drop table if exists Inventaire;
drop table if exists Commande;
drop table if exists bon_de_commande;
drop table if exists Historique;
drop table if exists Paiement;
drop table if exists Boutique;
--
CREATE TABLE `floral`.`Client`(
`nom_client` varchar(40),
`prenom_client` varchar(40),
`telephone_client` varchar(12),
`courriel_client` varchar(40),
`mot_de_passe` varchar(60),
`adresse_facturation` varchar(60),
`carte_credit` varchar(20),
primary key(`courriel_client`) );
--
CREATE TABLE `floral`.`Employe`(
`id_employe` int auto_increment,
`nom_employe` varchar(40),
`prenom_employe` varchar(40),
`mot_de_passe` varchar(40),
Primary key(`id_employe`));
--
CREATE TABLE `floral`.`Bouquet_standard`(
`nom_bouquet` varchar(20),
`composition_bouquet` varchar(100),
`prix` int,
`categorie` varchar(40),
PRIMARY KEY (nom_bouquet));
--
CREATE TABLE `floral`.`Bouquet_personnalise`(
`budget` int,
`description` varchar(40),
PRIMARY KEY(`budget`,`description`) );
--
CREATE TABLE `floral`.`Boutique`(
`nom_boutique` varchar(100),
`adresse_boutique` varchar(40),
`id_boutique` INT ,
PRIMARY KEY(`id_boutique`));
--
CREATE TABLE `floral`.`Inventaire`(
`id_boutique` int,
`nom_fleur` varchar(40),
`prix_fleur` int,
`disponibilite_fleur` varchar(40),
`type_fleur` varchar(40),
PRIMARY KEY(`nom_fleur`),
FOREIGN KEY(`id_boutique`) references Boutique(`id_boutique`));
--
CREATE TABLE `floral`.`bon_de_commande`(
`email` varchar(40),
`numero_commande` INT AUTO_INCREMENT,
`date_commande` DATETIME DEFAULT CURRENT_TIMESTAMP,
`adresse_livraison` varchar(100),
`message_floral` varchar(100),
`date_livraison` varchar(100),
PRIMARY KEY(`numero_commande`),
FOREIGN KEY(`email`) references Client(`courriel_client`));
--
CREATE TABLE `floral`.`Commande`(
`code_etat` varchar(5),
`etat_nom` varchar(150),
`id_commande` int,
`nom_bouquet` varchar(20),
PRIMARY KEY(`id_commande`),
FOREIGN KEY(`id_commande`) REFERENCES bon_de_commande(`numero_commande`));
--
CREATE TABLE `floral`.`Historique`(
`id_commande` int auto_increment,
`email` varchar(100),
`fidelite` varchar(100),
`achat` varchar(100),
`mois` int,
PRIMARY KEY(`id_commande`),
FOREIGN KEY(`email`) references Client(`courriel_client`) );
--
CREATE TABLE `floral`.`Paiement`(
`id_paiement` int auto_increment,
`prix` int,
`id_boutique` int,
`nom_bouquet` varchar(20),
PRIMARY KEY(`id_paiement`),
FOREIGN KEY(`id_paiement`) REFERENCES bon_de_commande(`numero_commande`),
FOREIGN KEY(`id_boutique`) REFERENCES boutique(`id_boutique`),
FOREIGN KEY(`nom_bouquet`) REFERENCES bouquet_standard(`nom_bouquet`));


INSERT INTO `floral`.`client` (`nom_client`, `prenom_client`, `telephone_client`, `courriel_client`, `mot_de_passe`, `adresse_facturation`, `carte_credit`) VALUES ('Bodénan', 'Thomas', 'test', 'test@gmail.com', 'test', 'Paris', 'Visa');
INSERT INTO `floral`.`client` (`nom_client`, `prenom_client`, `telephone_client`, `courriel_client`, `mot_de_passe`, `adresse_facturation`, `carte_credit`) VALUES ('Besset', 'Lucas', 'test2', 'test2@gmail.com', 'test2', 'Paris', 'Visa2');


INSERT INTO `floral`.`boutique` (`nom_boutique`, `adresse_boutique`,`id_boutique`) VALUES ('Boutique 1', '45 avenue des Champs-élysées',1);
INSERT INTO `floral`.`boutique` (`nom_boutique`, `adresse_boutique`,`id_boutique`) VALUES ('Boutique 2', '12 rue victor Hugo',2);

INSERT INTO `floral`.`Bouquet_standard` (`nom_bouquet`, `composition_bouquet`, `prix`, `categorie`) VALUES ('Gros Merci', 'Arrangement floral avec marguerites et verdure', 45 , 'Toute occasion');
INSERT INTO `floral`.`Bouquet_standard` (`nom_bouquet`, `composition_bouquet`, `prix`, `categorie`) VALUES ('L amoureux', 'Arrangement floral avec roses blanches et roses rouges', 65 , 'St-Valentin');
INSERT INTO `floral`.`Bouquet_standard` (`nom_bouquet`, `composition_bouquet`, `prix`, `categorie`) VALUES ('L exotique', 'Arrangement floral avec ginger, oiseaux du paradis, roses et genet', 40 , 'Toute occasion');
INSERT INTO `floral`.`Bouquet_standard` (`nom_bouquet`, `composition_bouquet`, `prix`, `categorie`) VALUES ('Maman', 'Arrangement floral avec gerbera, roses blanches, Irys et alstroméria', 80 , 'Fête des mères');
INSERT INTO `floral`.`Bouquet_standard` (`nom_bouquet`, `composition_bouquet`, `prix`, `categorie`) VALUES ('Vive la mariée', 'Arrangement floral avec lys et orchidées', 120 , 'Mariage');


INSERT INTO `floral`.`employe` (`id_employe`,`nom_employe`, `prenom_employe`, `mot_de_passe`) VALUES (1,'Bodénan','Thomas','test');
