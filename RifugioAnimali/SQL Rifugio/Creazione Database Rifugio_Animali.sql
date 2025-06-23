create database rifugio_animali;

create table citta
(
	citta_id int primary key auto_increment,
    citta varchar(100) not null unique
);

create table indirizzo
(
	indirizzo_id int primary key auto_increment,
    indirizzo varchar(100) not null,
    citta_id int,
    foreign key (citta_id) references citta (citta_id)
);

create table utente
(
	utente_id int primary key auto_increment,
    nome varchar(100) not null,
    cognome varchar(100) not null,
    email varchar(100) not null unique,
    password varchar(100) not null,
    telefono varchar(15) not null unique,
    indirizzo_id int,
    foreign key (indirizzo_id) references indirizzo (indirizzo_id)
);

create table staff
(
	staff_id int primary key auto_increment,
    is_admin bool not null,
    utente_id int unique,
    foreign key (utente_id) references utente (utente_id)
);

create table cliente
(
	cliente_id int primary key auto_increment,
    utente_id int unique,
    foreign key (utente_id) references utente (utente_id)
);

create table specie
(
	specie_id int primary key auto_increment,
    specie varchar(100) not null unique
);

create table diario_clinico
(
	diario_id int primary key auto_increment,
    numero_visite int,
    ultima_visita date,
    prossimo_richiamo date
);

create table animale
(
	animale_id int primary key auto_increment,
    nome varchar(100) not null,
    eta int not null,
    vaccinato bool not null,
    adottato bool not null,
    specie_id int,
    diario_id int unique,
    foreign key (specie_id) references specie (specie_id),
    foreign key (diario_id) references diario_clinico (diario_id)
);

create table ingresso
(
	ingresso_id int primary key auto_increment,
    data date not null default (CURRENT_DATE()),
    animale_id int,
    staff_id int,
    foreign key (animale_id) references animale (animale_id),
    foreign key (staff_id) references staff (staff_id)
);

create table adozione
(
	adozione_id int primary key auto_increment,
    data date not null default (CURRENT_DATE()),
    animale_id int,
    cliente_id int,
    staff_id int,
    foreign key (animale_id) references animale (animale_id),
    foreign key (cliente_id) references cliente (cliente_id),
    foreign key (staff_id) references staff (staff_id)
);

create table inventario
(
	inventario_id int primary key auto_increment,
    scadenza date
);

create table categoria_cibo
(
	categoria_id int primary key auto_increment,
    nome varchar(100) not null unique,
    descrizione varchar(100)
);

create table categoria_medicina
(
	categoria_id int primary key auto_increment,
    nome varchar(100) not null unique,
    descrizione varchar(100)
);

create table categoria_accessorio
(
	categoria_id int primary key auto_increment,
    nome varchar(100) not null unique,
    descrizione varchar(100)
);

create table cibo
(
	cibo_id int primary key auto_increment,
    nome varchar(100) not null,
    categoria_id int,
    inventario_id int,
    specie_id int,
    foreign key (categoria_id) references categoria_cibo (categoria_id),
    foreign key (inventario_id) references inventario (inventario_id),
    foreign key (specie_id) references specie (specie_id)
);

create table medicina
(
	medicina_id int primary key auto_increment,
    nome varchar(100) not null,
    categoria_id int,
    inventario_id int,
    specie_id int,
    foreign key (categoria_id) references categoria_medicina (categoria_id),
    foreign key (inventario_id) references inventario (inventario_id),
    foreign key (specie_id) references specie (specie_id)
);

create table accessorio
(
	accessorio_id int primary key auto_increment,
    nome varchar(100) not null,
    taglia varchar(5) not null,
    categoria_id int,
    inventario_id int,
    specie_id int,
    foreign key (categoria_id) references categoria_accessorio (categoria_id),
    foreign key (inventario_id) references inventario (inventario_id),
    foreign key (specie_id) references specie (specie_id)
);