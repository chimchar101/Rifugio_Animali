using System;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;

public class Cliente : Utente // Classe Cliente che estende la classe Utente
{
    public Cliente(int utenteId, string nome, string cognome, string email, string password, string telefono, int indirizzoId) // Costruttore della classe Cliente
        : base(utenteId, nome, cognome, email, password, telefono, indirizzoId)
    {

    }

    public void Registrazione(MySqlConnection connessione) // Metodo per la registrazione di un nuovo cliente che come parametro accetta una connessione al database
    {
        while (true) // Ciclo per richiedere i dati fino a quando non sono validi
        {
            Console.WriteLine("Inserisci i tuoi dati per la registrazione:");
            Console.WriteLine("Nome: ");
            string nome = Console.ReadLine().Trim().ToLower(); // Legge il nome e lo converte in minuscolo e toglie gli spazi iniziali e finali
            Console.WriteLine("Cognome: ");
            string cognome = Console.ReadLine().Trim().ToLower();

            Console.WriteLine("Email: ");
            string email;
            do
            {
                email = Console.ReadLine().Trim().ToLower();
                if (!IsEmailValid(email, connessione)) // Richiama il metodo IsEmailValid per verificare se l'email è valida
                {
                    Console.WriteLine("Inserisci un'email valida.");
                }
            } while (!IsEmailValid(email, connessione));

            Console.WriteLine("Password: ");
            string password;
            do
            {
                password = Console.ReadLine().Trim();
                if (!IsPasswordValid(password, connessione))  // Richiama il metodo IsPasswordValid per verificare se la password è valida
                {
                    Console.WriteLine("Inserisci una password valida.");
                }
            } while (!IsPasswordValid(password, connessione));

            Console.WriteLine("Telefono: ");
            string telefono;
            do
            {
                telefono = Console.ReadLine().Trim();
                if (!IsTelefonoValid(telefono, connessione)) // Richiama il metodo IsTelefonoValid per verificare se il telefono è valido
                {
                    Console.WriteLine("Inserisci un numero di telefono valido.");
                }
            } while (!IsTelefonoValid(telefono, connessione));

            Console.WriteLine("Indirizzo: ");
            string indirizzo = Console.ReadLine().Trim().ToLower();
            Console.WriteLine("Città: ");
            string citta = Console.ReadLine().Trim().ToLower();

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(cognome) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(telefono) || string.IsNullOrEmpty(indirizzo) || string.IsNullOrEmpty(citta)) // Controlla se tutti i campi sono stati compilati
            {
                Console.WriteLine("Tutti i campi sono obbligatori. Riprova da capo.\n"); // Se uno o più campi sono vuoti, richiede di inserire nuovamente i dati
                continue; // Ricomincia la registrazione
            }
            else
            {
                string query = "INSERT INTO utente (nome, cognome, email, password, telefono, indirizzo, citta) VALUES (@Nome, @Cognome, @Email, @Password, @Telefono, @Indirizzo, @Citta)";
                using (MySqlCommand cmd = new MySqlCommand(query, connessione)) // Crea un comando SQL per inserire i dati nel database
                {
                    cmd.Parameters.AddWithValue("@Nome", nome);
                    cmd.Parameters.AddWithValue("@Cognome", cognome);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Telefono", telefono);
                    cmd.Parameters.AddWithValue("@Indirizzo", indirizzo);
                    cmd.Parameters.AddWithValue("@Citta", citta);

                    try
                    {
                        cmd.ExecuteNonQuery(); // Esegue il comando SQL per inserire i dati nel database
                        Console.WriteLine("Registrazione completata con successo.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Errore durante la registrazione: " + ex.Message);
                    }
                }
            }
            break; // Esci dal ciclo se tutto è andato bene
        }
    }

    public void StampaAnimali() // Metodo per stampare gli animali disponibili per l'adozione
    {
        string query = "SELECT * FROM animale join specie on specie.specie_id=animale.specie_id WHERE adottato = false "; // Solo animali non adottati
        using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection connessione))
        {
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows) // Controlla se ci sono animali disponibili
                {
                    Console.WriteLine("Animali disponibili per l'adozione:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader["animale_id"]}, Nome: {reader["nome"]}, Specie: {reader["specie"]}, Età: {reader["eta"]}");
                    }
                }
                else
                {
                    Console.WriteLine("Nessun animale disponibile per l'adozione.");
                }
            }
        }
    }
    public void StampaAdozioni() // Metodo per stampare le adozioni effettuate dal cliente
    {
        string query = "SELECT * FROM adozione WHERE cliente_id = @ClienteID";
        using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection connessione))
        {
            cmd.Parameters.AddWithValue("@ClienteID", this.UtenteId);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows) // Controlla se ci sono adozioni effettuate dal cliente
                {
                    Console.WriteLine("Le tue adozioni:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID Adozione: {reader["adozione_id"]}, Animale ID: {reader["animale_id"]}, Data Adozione: {reader["data"]}");
                    }
                }
                else
                {
                    Console.WriteLine("Nessuna adozione trovata.");
                }
            }
        }
    }
    public void ModificaProfilo()
    {
        Console.WriteLine("Modifica il tuo profilo:");
        Console.WriteLine("Quale campo vuoi modificare?\n 1. Nome\n 2. Cognome\n 3. Email\n 4. Password\n 5. Telefono\n 6. Indirizzo\n 7. Città\n 8. Esci");
        string scelta = Console.ReadLine().Trim();
        switch (scelta)
        {
            case "1":
                Console.WriteLine("Inserisci il nuovo nome");
                string nome = Console.ReadLine().Trim().ToLower();
                if (!string.IsNullOrEmpty(nome))
                {
                    string query = "UPDATE utente SET nome = @nome WHERE cliente_id = @ClienteID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connessione)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@nome", nome);
                        cmd.Parameters.AddWithValue("@ClienteID", this.UtenteId);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            Console.WriteLine("Nome modificato con successo");
                        }
                        else
                        {
                            Console.WriteLine("Modifica non riuscita");
                        }
                    }
                }
                break;
            case "2":
                Console.WriteLine("Inserisci il nuovo cognome");
                string cognome = Console.ReadLine().Trim().ToLower();
                if (!string.IsNullOrEmpty(cognome))
                {
                    string query = "UPDATE utente SET cognome = @cognome WHERE cliente_id = @ClienteID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connessione)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@cognome", cognome);
                        cmd.Parameters.AddWithValue("@ClienteID", this.UtenteId);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            Console.WriteLine("Cognome modificato con successo");
                        }
                        else
                        {
                            Console.WriteLine("Modifica non riuscita");
                        }
                    }
                }
                break;
            case "3":
                Console.WriteLine("Inserisci la nuova email");
                string email = Console.ReadLine().Trim().ToLower();
                if (!string.IsNullOrEmpty(email))
                {
                    string query = "UPDATE utente SET email = @email WHERE cliente_id = @ClienteID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connessione)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@ClienteID", this.UtenteId);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            Console.WriteLine("Email modificata con successo");
                        }
                        else
                        {
                            Console.WriteLine("Modifica non riuscita");
                        }
                    }
                }
                break;
            case "4":
                Console.WriteLine("Inserisci la nuova password");
                string password = Console.ReadLine().Trim().ToLower();
                if (!string.IsNullOrEmpty(password))
                {
                    string query = "UPDATE utente SET password = @password WHERE cliente_id = @ClienteID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connessione)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@ClienteID", this.UtenteId);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            Console.WriteLine("Password modificata con successo");
                        }
                        else
                        {
                            Console.WriteLine("Modifica non riuscita");
                        }
                    }
                }
                break;
            case "5":
                Console.WriteLine("Inserisci il nuovo telefono");
                string telefono = Console.ReadLine().Trim().ToLower();
                if (!string.IsNullOrEmpty(telefono))
                {
                    string query = "UPDATE utente SET telefono = @telefono WHERE cliente_id = @ClienteID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connessione)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@telefono", telefono);
                        cmd.Parameters.AddWithValue("@ClienteID", this.UtenteId);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            Console.WriteLine("Password modificata con successo");
                        }
                        else
                        {
                            Console.WriteLine("Modifica non riuscita");
                        }
                    }
                }
                break;
            case "6":
                Console.WriteLine("Inserisci il nuovo indirizzo");
                string indirizzo = Console.ReadLine().Trim().ToLower();
                if (!string.IsNullOrEmpty(indirizzo))
                {
                    string query = "UPDATE utente SET indirizzo = @indirizzo WHERE cliente_id = @ClienteID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connessione)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@indirizzo", indirizzo);
                        cmd.Parameters.AddWithValue("@ClienteID", this.UtenteId);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            Console.WriteLine("Indirizzo modificato con successo");
                        }
                        else
                        {
                            Console.WriteLine("Modifica non riuscita");
                        }
                    }
                }
                break;
                    case "7":
                Console.WriteLine("Inserisci la nuova città");
                string citta = Console.ReadLine().Trim().ToLower();
                if (!string.IsNullOrEmpty(citta))
                {
                    string query = "UPDATE utente SET citta = @citta WHERE cliente_id = @ClienteID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connessione)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@citta", citta);
                        cmd.Parameters.AddWithValue("@ClienteID", this.UtenteId);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            Console.WriteLine("Città modificata con successo");
                        }
                        else
                        {
                            Console.WriteLine("Modifica non riuscita");
                        }
                    }
                }
                break;


        }
    }

    void VisualizzaDettaglioClinico(int animaleId)
    {
        string query = @"SELECT animale.id, animale.nome, animale.vaccinato, diario_clinico.numero_visite, diario_clinico.ultima_visita, diario_clinico.prossimo_richiamo AS storico
        FROM animale
        JOIN diario_clinico ON diario_clinico.animale_id = animale.animale_id
        WHERE animale_id = @animaleId";

        using (MySqlCommand cmd = new MySqlCommand(query, MySqlConnection connessione))
        {
            cmd.Parameters.AddWithValue("@animaleId", animaleId);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    Console.WriteLine("Dettagli dell'animale:");
                    Console.WriteLine($"ID: {reader["animale_id"]}");
                    Console.WriteLine($"Nome: {reader["nome"]}");
                    Console.WriteLine($"Numero Visite: {reader["numero_visite"]}");
                    Console.WriteLine($"Ultima Visita: {reader["ultima_visita"]}");
                    Console.WriteLine($"Prossimo Richiamo: {reader["prossimo_richiamo"]}");
                    Console.WriteLine($"Vaccinato: {(Convert.ToBoolean(reader["vaccinato"]) ? "Sì" : "No")}");
                }
                else
                {
                    Console.WriteLine("Animale non trovato.");
                }
            }
        }
    }


    public static bool IsEmailValid(string email, MySqlConnection connessione) // Metodo per verificare se l'email è già registrata nel database
    {
        string query = "SELECT email FROM utente WHERE email = @Email"; // Query per verificare se l'email esiste già
        using (MySqlCommand cmd = new MySqlCommand(query, connessione))
        {
            cmd.Parameters.AddWithValue("@Email", email);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())  // se esiste almeno una riga con quella email
                {
                    Console.WriteLine("Email già registrata.");
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }

    public static bool IsTelefonoValid(string telefono, MySqlConnection connessione) // Metodo per verificare se il numero di telefono è già registrato nel database
    {
        string query = "SELECT telefono FROM utente WHERE telefono = @Telefono"; // Query per verificare se il telefono esiste già
        using (MySqlCommand cmd = new MySqlCommand(query, connessione))
        {
            cmd.Parameters.AddWithValue("@Telefono", telefono);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())  // se esiste almeno una riga con quel telefono
                {
                    Console.WriteLine("Telefono già registrato.");
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }

    public static bool IsPasswordValid(string password, MySqlConnection connessione) // Metodo per verificare se la password è valida
    {
        // Controlla se la password è valida con controlli sulla lunghezza minima, carattere maiuscolo, minuscolo e numero
        if (password.Length < 8 || !password.Any(char.IsUpper) || !password.Any(char.IsLower) || !password.Any(char.IsDigit))
        {
            Console.WriteLine("La password deve contenere almeno 8 caratteri, una maiuscola, una minuscola e un numero.");
            return false;
        }
        return true;
    }
}