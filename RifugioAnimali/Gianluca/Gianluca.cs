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
            LoggedUserId = Convert.ToInt32(reader["id"]);   // Recupero dell'ID dell'utente loggato
            Console.WriteLine("Accesso riuscito! Benvenuto " + reader["nome"]);
            return LoggedUserId;

        }
        else
        {
            Console.WriteLine("Email o password errati.");
            return null;
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
                        Cliente.Registrazione(conn);    // Chiamata al metodo di registrazione
                        break;
                    case "2":
                        int? utenteID = Utente.Login(conn);            // Chiamata al metodo di login
                        if (utenteID != null)
                        {
                            TipoUtente tipoUtente = ControllaTipo(conn, (int)utenteID);
                            switch (tipoUtente)
                            {
                                case TipoUtente.cliente:

                                    break;

                                case TipoUtente.staff:
                                    break;

                                case TipoUtente.responsabile:
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
    private static void Registrazione(MySqlConnection conn)
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

    }

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
        Cliente cliente= new Cliente(utenteId);
        
        while (!exit)
        {
            Console.WriteLine("\nMenù");
            Console.WriteLine("[1] Visualizza animali");
            Console.WriteLine("[2] Visualizza adozioni");
            Console.WriteLine("[3] Modifica profilo");
            Console.WriteLine("[4] Visualizza diario clinico");
            Console.WriteLine("[0] Esci");
            Console.Write("Scelta: ");
            int menuAction = int.Parse(Console.ReadLine() ?? "Campo obbligatorio");

            switch (menuAction)
            {
                case 1:
                    cliente.StampaAnimali(connection);
                    break;

                case 2:
                    cliente.StampaAdozioni(connection);
                    break;

                case 3:
                    cliente.ModificaProfilo(connection);
                    break;

                case 4:
                    cliente.VisualizzaDiarioClinico(connection, );
                    break;

                case 0:
                    exit = true;
                    break;

                default:
                    Console.WriteLine("Scelta non valida.");
                    break;
            }
        }
    }
}





























































