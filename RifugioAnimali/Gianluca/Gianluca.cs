using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;

public abstract class Utente
{
    // Proprietà base comuni a tutti gli utenti
    public int UtenteId { get; set; }  // ID dell'utente

    // Costruttore per inizializzare un utente
    public Utente(int utenteId)
    {
        if (utenteId <= 0)
        {
            throw new ArgumentException("ID utente non valido. Deve essere maggiore di zero.");
        }

        UtenteId = utenteId;
    }

    // Metodo per la registrazione
    public static int? Login(MySqlConnection conn)
    {
        int LoggedUserId;    // Variabile per memorizzare l'ID dell'utente loggato

        // Verifica se l'email è valida
        Console.Write("Inserisci la tua email: ");
        string email = Console.ReadLine()?.Trim() ?? "Campo obbligatorio";

        // Verifica se la password è valida
        Console.Write("Inserisci la tua password: ");
        string password = Console.ReadLine()?.Trim() ?? "Campo obbligatorio";


        // Query per verificare le credenziali dell'utente
        string query = "SELECT * FROM utente WHERE email = @Email AND password = @Password";
        using MySqlCommand cmd = new MySqlCommand(query, conn);

        // Aggiunta dei parametri alla query per prevenire SQL injection
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@Password", password);


        // Esecuzione della query e lettura dei risultati
        using MySqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            LoggedUserId = Convert.ToInt32(reader["utente_id"]);   // Recupero dell'ID dell'utente loggato
            Console.WriteLine("Accesso riuscito! Benvenuto " + reader["nome"]);
            return LoggedUserId;

        }
        else
        {
            Console.WriteLine("Email o password errati.");
            return null;
        }

    }

    private static void Registrazione(MySqlConnection connection) // metodo per la registrazione dell'utente
    {
        while (true) // Ciclo per richiedere i dati fino a quando non sono validi
        {
            Console.WriteLine("Inserisci i tuoi dati per la registrazione:");
            Console.Write("Nome: ");
            string nome = Console.ReadLine().Trim().ToLower();  // Legge il nome e lo converte in minuscolo e toglie gli spazi iniziali e finali
            Console.Write("Cognome: ");
            string cognome = Console.ReadLine().Trim().ToLower();

            Console.Write("Email: ");
            string email;
            do
            {
                email = Console.ReadLine().Trim().ToLower();
                if (!IsEmailValid(email, connection))  // Richiama il metodo IsEmailValid per verificare se l'email è valida
                {
                    Console.WriteLine("Inserisci un'email valida.");
                }
            } while (!IsEmailValid(email, connection));

            Console.Write("Password: ");
            string password;
            do
            {
                password = Console.ReadLine().Trim();
                if (!IsPasswordValid(password, connection)) // Richiama il metodo IsPasswordValid per verificare se la password è valida
                {
                    Console.WriteLine("Inserisci una password valida.");
                }
            } while (!IsPasswordValid(password, connection));

            Console.Write("Telefono: ");
            string telefono;
            do
            {
                telefono = Console.ReadLine().Trim();
                if (!IsTelefonoValid(telefono, connection)) // Richiama il metodo IsTelefonoValid per verificare se il telefono è valido
                {
                    Console.WriteLine("Inserisci un numero di telefono valido.");
                }
            } while (!IsTelefonoValid(telefono, connection));

            Console.Write("Indirizzo: ");
            string indirizzo = Console.ReadLine().Trim().ToLower();
            Console.Write("Città: ");
            string citta = Console.ReadLine().Trim().ToLower();

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(cognome) || string.IsNullOrEmpty(email) || // Controlla se tutti i campi sono stati compilati
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(telefono) ||
                string.IsNullOrEmpty(indirizzo) || string.IsNullOrEmpty(citta))
            {
                Console.WriteLine("Tutti i campi sono obbligatori. Riprova da capo.\n");
                continue; // Se uno o più campi sono vuoti, richiede di inserire nuovamente i dati
            }

            int cittaId = 0;

            // Prima query: controllo se la città esiste
            string queryCittaEsiste = @"SELECT citta_id 
                                        FROM citta 
                                        WHERE citta = @Citta";

            using (MySqlCommand cmd = new MySqlCommand(queryCittaEsiste, connection))
            {
                cmd.Parameters.AddWithValue("@Citta", citta);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Se esiste, prendo l'id
                        cittaId = (int)(reader["citta_id"]);
                    }
                    else
                    {
                        // Chiudo il reader PRIMA di fare un'altra query
                        reader.Close();

                        // Inserisco la nuova città
                        string queryInsertCitta = "INSERT INTO citta (citta) VALUES (@Citta)";
                        using (MySqlCommand insertCmd = new MySqlCommand(queryInsertCitta, connection))
                        {
                            insertCmd.Parameters.AddWithValue("@Citta", citta);
                            insertCmd.ExecuteNonQuery(); // Esegui l'insert
                        }

                        // Recupero l'ID massimo dopo l'inserimento
                        string querySelectMax = "SELECT MAX(citta_id) AS citta_id FROM citta";
                        using (MySqlCommand selectMaxCmd = new MySqlCommand(querySelectMax, connection))
                        {
                            using (MySqlDataReader reader2 = selectMaxCmd.ExecuteReader())
                            {
                                if (reader2.Read())
                                {
                                    cittaId = (int)(reader2["citta_id"]);
                                }
                            }
                        }
                    }
                }
            }

            int indirizzoId = 0;

            // Controlla se l’indirizzo esiste per quella città
            string queryIndirizzoEsiste = @"SELECT indirizzo_id 
                                FROM indirizzo 
                                WHERE indirizzo = @Indirizzo AND citta_id = @CittaId";

            using (MySqlCommand cmd = new MySqlCommand(queryIndirizzoEsiste, connection))
            {
                cmd.Parameters.AddWithValue("@Indirizzo", indirizzo);
                cmd.Parameters.AddWithValue("@CittaId", cittaId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        indirizzoId = (int)(reader["indirizzo_id"]);
                    }
                    else
                    {
                        reader.Close(); // Chiudere prima di una nuova query

                        // Inserimento nuovo indirizzo
                        string queryInsertIndirizzo = "INSERT INTO indirizzo (indirizzo, citta_id) VALUES (@Indirizzo, @CittaId)";
                        using (MySqlCommand insertCmd = new MySqlCommand(queryInsertIndirizzo, connection))
                        {
                            insertCmd.Parameters.AddWithValue("@Indirizzo", indirizzo);
                            insertCmd.Parameters.AddWithValue("@CittaId", cittaId);
                            insertCmd.ExecuteNonQuery();
                        }

                        // Recupera l'id appena inserito
                        string queryGetMaxId = "SELECT MAX(indirizzo_id) AS indirizzo_id FROM indirizzo";
                        using (MySqlCommand getIdCmd = new MySqlCommand(queryGetMaxId, connection))
                        {
                            using (MySqlDataReader reader2 = getIdCmd.ExecuteReader())
                            {
                                if (reader2.Read())
                                {
                                    indirizzoId = (int)(reader2["indirizzo_id"]);
                                }
                            }
                        }
                    }
                }
            }

            // Inserisco finalmente l'utente
            string queryUtente = "INSERT INTO utente (nome, cognome, email, password, telefono, indirizzo_id) VALUES (@Nome, @Cognome, @Email, @Password, @Telefono, @IndirizzoId)";
            using (MySqlCommand cmd = new MySqlCommand(queryUtente, connection))
            {
                cmd.Parameters.AddWithValue("@Nome", nome);
                cmd.Parameters.AddWithValue("@Cognome", cognome);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@Telefono", telefono);
                cmd.Parameters.AddWithValue("@IndirizzoId", indirizzoId);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Errore durante la registrazione: " + ex.Message);
                }
            }

            break; // Esce dal ciclo
        }
    }

    public static void RegistrazioneCliente(MySqlConnection connection)
    {
        Registrazione(connection);

        int utenteId;
        string queryUtenteId = "SELECT MAX(utente_id) FROM utente";
        using (MySqlCommand cmd = new MySqlCommand(queryUtenteId, connection))
        {
            MySqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();
            utenteId = (int)rdr[0];
            rdr.Close();
        }

        string queryCliente = "INSERT INTO cliente (utente_id) VALUES (@utente_id)";
        using (MySqlCommand cmd = new MySqlCommand(queryCliente, connection))
        {
            cmd.Parameters.AddWithValue("@utente_id", utenteId);
            cmd.ExecuteNonQuery();
        }
        Console.WriteLine("Registrazione completata con successo.");
    }

    public void RegistrazioneStaff(MySqlConnection connection)
    {
        Registrazione(connection);
        string query = "select max(utente_id) from utente";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        int utenteId = 0;
        try
        {
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                utenteId = Convert.ToInt32(reader[0]);
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante richiamo di utente_id: " + ex.Message);
        }

        bool ruolo;
        Console.WriteLine("Il nuovo utente è un:\n[1] Volontario\n[2] Responsabile");

        do
        {
            string scelta = Console.ReadLine() ?? "1";
            switch (scelta)
            {
                case "1":
                    ruolo = false;
                    query = @"insert into staff (utente_id, is_admin)
                            values (@utente_id, false)";
                    cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@utente_id", utenteId);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Registrazione volontario completata con successo.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Errore durante la registrazione: " + ex.Message);
                    }
                    break;
                case "2":
                    ruolo = false;
                    query = @"insert into staff (utente_id, is_admin)
                            values (@utente_id, true)";
                    cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@utente_id", utenteId);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Registrazione responsabile completata con successo.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Errore durante la registrazione: " + ex.Message);
                    }
                    break;
                default:
                    ruolo = true;
                    Console.WriteLine("Errore input. Inserisci 1 per registrare un volontario, o 2 per registrare un responsabile.");
                    break;
            }
        } while (ruolo);
    }

    public static bool IsEmailValid(string email, MySqlConnection connection) // Metodo per verificare se l'email è già registrata nel database
    {
        string query = @"SELECT email FROM utente WHERE email = @Email"; // Query per verificare se l'email esiste già
        using (MySqlCommand cmd = new MySqlCommand(query, connection))
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

    public static bool IsTelefonoValid(string telefono, MySqlConnection connection) // Metodo per verificare se il numero di telefono è già registrato nel database
    {
        string query = "SELECT telefono FROM utente WHERE telefono = @Telefono"; // Query per verificare se il telefono esiste già
        using (MySqlCommand cmd = new MySqlCommand(query, connection))
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

    public static bool IsPasswordValid(string password, MySqlConnection connection) // Metodo per verificare se la password è valida
    {
        // Controlla se la password è valida con controlli sulla lunghezza minima, carattere maiuscolo, minuscolo e numero
        if (password.Length < 8 || !password.Any(char.IsUpper) || !password.Any(char.IsLower) || !password.Any(char.IsDigit))
        {
            Console.WriteLine("La password deve contenere almeno 8 caratteri, una maiuscola, una minuscola e un numero.");
            return false;
        }
        return true;
    }

    public void ModificaProfilo(MySqlConnection connection, int selectedUtenteId)
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
                    string query = @"UPDATE utente 
                                    SET nome = @nome 
                                    WHERE utente_id = @UtenteID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@nome", nome);
                        cmd.Parameters.AddWithValue("@UtenteID", selectedUtenteId);

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
                    string query = @"UPDATE utente 
                                    SET cognome = @cognome 
                                    WHERE utente_id = @UtenteID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@cognome", cognome);
                        cmd.Parameters.AddWithValue("@UtenteID", selectedUtenteId);

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
                    string query = @"UPDATE utente 
                                    SET email = @email 
                                    WHERE utente_id = @UtenteID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@UtenteID", selectedUtenteId);

                        int rows = 0;
                        if (IsTelefonoValid(email, connection))
                        {
                            rows = cmd.ExecuteNonQuery();
                        }

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
                    string query = @"UPDATE utente 
                                    SET password = @password 
                                    WHERE utente_id = @UtenteID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@UtenteID", selectedUtenteId);

                        int rows = 0;
                        if (IsTelefonoValid(password, connection))
                        {
                            rows = cmd.ExecuteNonQuery();
                        }

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
                    string query = @"UPDATE utente 
                                    SET telefono = @telefono 
                                    WHERE utente_id = @UtenteID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@telefono", telefono);
                        cmd.Parameters.AddWithValue("@UtenteID", selectedUtenteId);

                        int rows = 0;
                        if (IsTelefonoValid(telefono, connection))
                        {
                            rows = cmd.ExecuteNonQuery();
                        }

                        if (rows > 0)
                        {
                            Console.WriteLine("Telefono modificato con successo");
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
                    string query = @"UPDATE indirizzo 
                                    JOIN utente ON utente.indirizzo_id = indirizzo.indirizzo_id 
                                    SET indirizzo = @indirizzo 
                                    WHERE utente_id = @UtenteID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@indirizzo", indirizzo);
                        cmd.Parameters.AddWithValue("@UtenteID", selectedUtenteId);

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
                    string query = @"UPDATE citta 
                                    JOIN indirizzo ON citta.citta_id = indirizzo.citta_id 
                                    JOIN utente on utente.indirizzo_id = indirizzo.indirizzo_idSET citta = @citta 
                                    WHERE utente_id = @UtenteID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@citta", citta);
                        cmd.Parameters.AddWithValue("@UtenteID", selectedUtenteId);

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
            default:
                Console.WriteLine("Opzione non valida");
                break;

        }
    }
}


public class Program
{
    public enum TipoUtente
    {
        cliente,
        staff,
        responsabile
    }

    public static void Main(string[] args)
    {
        // Stringa di connessione al database MySQL locale
        string connStr = "server=localhost; user=root; password=1234; port=3306; database=rifugio_animali;";
        using MySqlConnection conn = new MySqlConnection(connStr);

        Utente utente;

        try
        {
            // Apertura connessione
            conn.Open();
            Console.WriteLine("Connessione riuscita!");

            // Variabile per controllare l'uscita dal ciclo
            bool esci = false;

            // Ciclo principale del programma
            // che permette all'utente di registrarsi, accedere o uscire
            while (!esci)
            {
                Console.WriteLine("Benvenuto nel nostro rifuggio animali: ");
                StampaDisegno();
                Console.WriteLine("Scegli un'opzione: ");
                Console.WriteLine("[1]: Registrati");
                Console.WriteLine("[2]: Accedi");
                Console.WriteLine("[3]: Esci");
                string scelta = Console.ReadLine()?.Trim() ?? "Campo Obligatorio";

                // Gestione delle scelte dell'utente
                scelta = scelta.Trim();
                switch (scelta)
                {
                    case "1":
                        Cliente.RegistrazioneCliente(conn);    // Chiamata al metodo di registrazione
                        break;
                    case "2":
                        int? utenteID = Utente.Login(conn);            // Chiamata al metodo di login
                        if (utenteID != null)
                        {
                            TipoUtente tipoUtente = ControllaTipo(conn, (int)utenteID);
                            switch (tipoUtente)
                            {
                                case TipoUtente.cliente:
                                    MenuCliente(conn, (int)utenteID);
                                    break;

                                case TipoUtente.staff:
                                    MenuStaff(conn, (int)utenteID);
                                    break;

                                case TipoUtente.responsabile:
                                    MenuResponsabile(conn, (int)utenteID);
                                    break;
                            }
                        }
                        break;
                    case "3":
                        esci = true;            // uscita dal ciclo
                        break;
                    default:
                        Console.WriteLine("Opzione non valida.");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            // Gestione delle eccezioni durante la connessione al database
            Console.WriteLine("Errore: " + ex.Message);
        }
    }

    // Metodo per la registrazione di un nuovo utente
    /* private static void Registrazione(MySqlConnection conn)
    {
        // Inserimento dei dati dell'utente
        Console.Write("Inserisci il tuo nome: ");
        string nome = Console.ReadLine()?.Trim() ?? "Campo oblogatorio";

        Console.Write("Inserisci il tuo cognome: ");
        string cognome = Console.ReadLine()?.Trim() ?? "Campo oblogatorio";

        Console.Write("Inserisci la tua email: ");
        string email = Console.ReadLine()?.Trim() ?? "Campo oblogatorio";

        Console.Write("Inserisci la tua password: ");
        string password = Console.ReadLine()?.Trim() ?? "Campo oblogatorio";

        Console.Write("Inserisci il tuo telefono: ");
        string telefono = Console.ReadLine()?.Trim() ?? "Campo oblogatorio";


        // Query per inserire un nuovo utente nel database
        string query = "INSERT INTO utente (nome, cognome, email, password, telefono) VALUES (@nome, @cognome, @email, @password, @telefono)";
        using MySqlCommand cmd = new MySqlCommand(query, conn);

        // Aggiunta dei parametri alla query per prevenire SQL injection
        cmd.Parameters.AddWithValue("@nome", nome);
        cmd.Parameters.AddWithValue("@cognome", cognome);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@password", password);
        cmd.Parameters.AddWithValue("@telefono", telefono);

        try
        {
            // Esecuzione della query per inserire l'utente
            int row = cmd.ExecuteNonQuery();
            if (row > 0)
            {
                Console.WriteLine("Registrazione avvenuta con successo!");
            }
            else
            {
                Console.WriteLine("Registrazione fallita.");
            }
        }
        catch (Exception ex)
        {
            // Gestione delle eccezioni durante l'inserimento nel database
            Console.WriteLine("Errore durante la registrazione: " + ex.Message);
        }

    } */

    private static TipoUtente ControllaTipo(MySqlConnection conn, int utenteID)
    {
        string query = "select * from cliente where utente_id = @utente_id";
        MySqlCommand cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@utente_id", utenteID);
        MySqlDataReader rdr = cmd.ExecuteReader();

        if (rdr.Read())
        {
            rdr.Close();
            return (TipoUtente.cliente);
        }
        rdr.Close();

        query = "select is_admin from staff where utente_id = @utente_id";
        cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@utente_id", utenteID);
        rdr = cmd.ExecuteReader();
        rdr.Read();
        bool isAdmin = (bool)rdr[0];
        rdr.Close();

        if (isAdmin)
        {
            return (TipoUtente.responsabile);
        }
        else
        {
            return (TipoUtente.staff);
        }
    }

    private static void MenuCliente(MySqlConnection connection, int utenteId)
    {
        bool exit = false;
        Cliente cliente = new Cliente(utenteId, connection);

        while (!exit)
        {
            Console.WriteLine("\nMenu cliente");
            Console.WriteLine("[1] Visualizza animali");
            Console.WriteLine("[2] Visualizza adozioni");
            Console.WriteLine("[3] Visualizza diario clinico");
            Console.WriteLine("[4] Modifica profilo personale");
            Console.WriteLine("[0] Esci");
            Console.Write("Scelta: ");
            string menuAction = Console.ReadLine() ?? "Campo obbligatorio";

            switch (menuAction)
            {
                case "1":
                    cliente.StampaAnimali(connection);
                    break;

                case "2":
                    cliente.StampaAdozioni(connection);
                    break;

                case "3":
                    cliente.VisualizzaDiarioClinico(connection);
                    break;

                case "4":
                    cliente.ModificaProfilo(connection, cliente.ClienteId);
                    break;

                case "0":
                    exit = true;
                    break;

                default:
                    Console.WriteLine("Scelta non valida.");
                    break;
            }
        }
    }

    private static void MenuStaff(MySqlConnection connection, int utenteId)
    {
        bool exit = false;
        Staff staff = new Staff(utenteId, connection);

        while (!exit)
        {
            Console.WriteLine("\nMenu staff");
            Console.WriteLine("[1] Visualizza animali");
            Console.WriteLine("[2] Affida animale");
            Console.WriteLine("[3] Aggiungi animale");
            Console.WriteLine("[4] Visualizza adozioni");
            Console.WriteLine("[5] Aggiorna diario clinico animale");
            Console.WriteLine("[6] Aggiungi inventario");
            Console.WriteLine("[7] Rimuovi inventario");
            Console.WriteLine("[8] Visualizza inventario");
            Console.WriteLine("[9] Modifica profilo personale");
            Console.WriteLine("[0] Esci");
            Console.Write("Scelta: ");
            string menuAction = Console.ReadLine() ?? "Campo obbligatorio";

            switch (menuAction)
            {
                case "1":
                    staff.StampaAnimali(connection);
                    break;

                case "2":
                    staff.AffidaAnimale(connection);
                    break;

                case "3":
                    staff.AggiungiAnimale(connection);
                    break;

                case "4":
                    staff.StampaAdozioni(connection);
                    break;

                case "5":
                    staff.AggiornaDiarioClinico(connection);
                    break;

                case "6":
                    staff.AggiungiInventario(connection);
                    break;

                case "7":
                    staff.RimuoviInventario(connection);
                    break;

                case "8":
                    staff.StampaInventario(connection);
                    break;

                case "9":
                    staff.ModificaProfilo(connection, staff.UtenteId);
                    break;

                case "0":
                    exit = true;
                    break;

                default:
                    Console.WriteLine("Scelta non valida.");
                    break;
            }
        }
    }

    private static void MenuResponsabile(MySqlConnection connection, int utenteId)
    {
        bool exit = false;
        Responsabile responsabile = new Responsabile(utenteId, connection);

        while (!exit)
        {
            Console.WriteLine("\nMenu");
            Console.WriteLine("[1] Visualizza animali");
            Console.WriteLine("[2] Affida animale");
            Console.WriteLine("[3] Aggiungi animale");
            Console.WriteLine("[4] Visualizza adozioni");
            Console.WriteLine("[5] Aggiungi inventario");
            Console.WriteLine("[6] Rimuovi inventario");
            Console.WriteLine("[7] Visualizza inventario");
            Console.WriteLine("[8] Aggiungi specie");
            Console.WriteLine("[9] Aggiungi staff");
            Console.WriteLine("[10] Rimuovi staff");
            Console.WriteLine("[11] Visualizza staff");
            Console.WriteLine("[12] Visualizza clienti");
            Console.WriteLine("[13] Modifica profilo personale");
            Console.WriteLine("[14] Modifica utente");
            Console.WriteLine("[0] Esci");
            Console.Write("Scelta: ");
            string menuAction = Console.ReadLine() ?? "Campo obbligatorio";

            switch (menuAction)
            {
                case "1":
                    responsabile.StampaAnimali(connection);
                    break;

                case "2":
                    responsabile.AffidaAnimale(connection);
                    break;

                case "3":
                    responsabile.AggiungiAnimale(connection);
                    break;

                case "4":
                    responsabile.StampaAdozioni(connection);
                    break;

                case "5":
                    responsabile.AggiungiInventario(connection);
                    break;

                case "6":
                    responsabile.RimuoviInventario(connection);
                    break;

                case "7":
                    responsabile.StampaInventario(connection);
                    break;

                case "8":
                    responsabile.AggiungiSpecie(connection);
                    break;

                case "9":
                    responsabile.RegistrazioneStaff(connection);
                    break;

                case "10":
                    responsabile.RimuoviStaff(connection);
                    break;

                case "11":
                    responsabile.StampaStaff(connection);
                    break;

                case "12":
                    responsabile.StampaClienti(connection);
                    break;

                case "13":
                    responsabile.ModificaProfilo(connection, responsabile.UtenteId);
                    break;

                case "14":
                    responsabile.ModificaUtente(connection);
                    break;

                case "0":
                    exit = true;
                    break;

                default:
                    Console.WriteLine("Scelta non valida.");
                    break;
            }
        }
    }

    public static void StampaDisegno()
    {
        Console.WriteLine("      (\\___/)");
        Console.WriteLine("     ( o   o )");
        Console.WriteLine("     (  =^=  )");
        Console.WriteLine("     (       )");
        Console.WriteLine("    (         )");
        Console.WriteLine("   (           )");
        Console.WriteLine("  ( (  )   (  ) )");
        Console.WriteLine(" (__(__)___(__)__)");
    }
}