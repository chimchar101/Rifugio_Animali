# 🐱 Rifugio Animali

[![Linguaggio](https://img.shields.io/badge/Linguaggio-C%23-green)]()
[![Scopo](https://img.shields.io/badge/Scopo-Didattico-blue)]()

## 📋 Descrizione

Questa repository contiene il progetto finale del gruppo 2 per l'Academy C# di Itconsulting.<br />
Il progetto simula un programma gestionale di un rifugio per animali: partendo dalla progettazione e creazione del database del rifugio con MySQL Workbench, fino ad arrivare allo sviluppo delle principali funzionalità attraverso la scrittura del codice in C# con Visual Studio Code.

## 🛠️ Tecnologie e Strumenti utilizzati

- **Progettazione**: Miro
- **Database**: MySQL Community 8.0.42 / MySQL Workbench
- **Linguaggio**: C#
- **IDE**: Visual Studio Code
- **Framework**: .NET 9.0
- **Versioning**: GitHub / GitHub Desktop

## 📝 Funzionalità
Il programma è stato sviluppato in modo che possa essere utilizzato da tre tipi diversi di utenti: clienti, volontari e responsabili.<br />
La schermata principale permette la registrazione di nuovi clienti, e il login di utenti già esistenti: una volta effettuato l'accesso, ognuno avrà a disposizione tutte le funzionalità disponibili in base al livello di account.

### Cliente
Gli account di livello Cliente hanno a disposizione 4 funzioni:
- **Visualizza animali**: permette di stampare l'elenco di tutti gli animali disponibili per l'adozione.
- **Visualizza adozioni**: permette di stampare l'elenco delle adozioni effettuate dal cliente.
- **Visualizza diario clinico**: permette di visualizzare il diario clinico di un animale presente in rifugio.
- **Modifica profilo personale**: permette al cliente di modificare i dati del proprio account.

### Volontario
Gli account di livello Volontario hanno a disposizione 9 funzioni:
- **Visualizza animali**: permette di stampare l'elenco di tutti gli animali.
- **Affida animale**: permette al volontario di dare in adozione un animale del rifugio a un cliente.
- **Aggiungi animale**: permette la registrazione di un nuovo animale nel rifugio.
- **Visualizza adozioni**: permette la visualizzazione di tutte le adozioni.
- **Aggiorna diario clinico animale**: permette al volontario di aggiornare il diario clinico di un animale.
- **Aggiungi inventario**: permette al volontario di aggiungere un prodotto (cibo, medicine, accessori) nell'inventario.
- **Rimuovi inventario**: permette al volontario di rimuovere un prodotto dall'inventario.
- **Visualizza inventario**: permette di visualizzare tutti i prodotti presenti nell'inventario.
- **Modifica profilo personale**: permette al volontario di modificare i dati del proprio account.

### Responsabile
Gli account di livello Responsabile, oltre alle funzioni dei volontari, hanno a dispozione 7 nuove funzioni:
- **Aggiungi specie**: permette al responsabile di aggiungere una nuova specie di animali al rifugio.
- **Aggiungi staff**: permette la creazione di un utente e la successiva assegnazione al ruolo di volontario o responsabile.
- **Rimuovi staff**: permette al responsabile di rimuovere un membro dello staff.
- **Visualizza staff**: permette la visualizzazione di tutti i membri dello staff.
- **Visualizza clienti**: permette la visualizzazione di tutti i clienti del rifugio.
- **Modifica utente**: permette al responsabile di modificare i dati di un utente a scelta.
- **Visualizza statistiche**: permette al responsabile di visualizzare varie statistiche sul rifugio e i suoi animali.

## ⬇️ Come accedere al progetto

### Prerequisiti
- .NET SDK installato
- MySQL installato
- IDE installato
### Processo
1. Apri Git Bash
2. Clona la repository
   ```
   git clone https://github.com/chimchar101/Rifugio_Animali.git
   ```
3. Ricrea il database con MySQL utilizzando il file presente nella cartella [SQL Rifugio](https://github.com/chimchar101/Rifugio_Animali/tree/main/RifugioAnimali/SQL%20Rifugio)
4. Prova e modifica il codice a tuo piacimento
### Consigliati
- **Database**: [MySQL Community 8.0.42](https://dev.mysql.com/downloads/installer/) (installa pacchetto completo con Workbench)
- **Framework**: [.NET 9.0](https://dotnet.microsoft.com/it-it/download/dotnet/9.0)
- **IDE**: [Visual Studio Code](https://code.visualstudio.com/)

## 👤 Autori
- **Andrea Rottura** - [Andrearttr](https://github.com/Andrearttr)
- **Gianluca Boglione** - [GianlucaBoglione](https://github.com/GianlucaBoglione)
- **Simone Addesso** - [chimchar101](https://github.com/chimchar101)
- **Yuliya Stepanyuk** - [yuliyastepanyuk](https://github.com/yuliyastepanyuk)

## 👨‍🏫 Insegnanti
- Edoardo Del Sarto
- Mirko Campari
- Michael Bagnoli
