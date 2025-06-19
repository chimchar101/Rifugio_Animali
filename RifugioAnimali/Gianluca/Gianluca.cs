using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;

public abstract class Utente
{
    // Proprietà base comuni a tutti gli utenti
    public string Nome { get; set; }
    public string Cognome { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Telefono { get; set; }

    // Costruttore per inizializzare un utente
    public Utente(string nome, string cognome, string email, string password, string telefono)
    {
        Nome = nome;
        Cognome = cognome;
        Email = email;
        Password = password;
        Telefono = telefono;
    }

    // Metodo astratto per il login, da implementare nelle sottoclassi
    public abstract bool Login(string email, string password);

    // Metodo per la registrazione
    public static void Login(MySqlConnection conn)  
    {
        int? LoggedUserId = null;    // Variabile per memorizzare l'ID dell'utente loggato

        // Verifica se l'email è valida
        Console.Write("Inserisci la tua email: ");
        string email = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(email))
        {
            Console.WriteLine("Email obbligatoria.");
            return;
        }

        // Verifica se la password è valida
        Console.Write("Inserisci la tua password: ");
        string password = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(password))
        {
            Console.WriteLine("Password obbligatoria.");
            return;
        }
        
        // Query per verificare le credenziali dell'utente
        string query = "SELECT * FROM utente WHERE email = @Email AND password = @Password";
        using MySqlCommand cmd = new MySqlCommand(query, conn);

        // Aggiunta dei parametri alla query per prevenire SQL injection
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@Password", password);

        try
        {
            // Esecuzione della query e lettura dei risultati
            using MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                LoggedUserId = Convert.ToInt32(reader["id"]);   // Recupero dell'ID dell'utente loggato
                Console.WriteLine("Accesso riuscito! Benvenuto " + reader["nome"]);

            }
            else
            {
                Console.WriteLine("Email o password errati.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante l'accesso: " + ex.Message);
        }
    }

}
public class Program
{
    public static void Main(string[] args)
    {
        // Stringa di connessione al database MySQL locale
        string connStr = "server=localhost; user=root; password=1234; port=3306; database=rifugio_animali;";
        using MySqlConnection conn = new MySqlConnection(connStr);

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
                        Registrazione(conn);    // Chiamata al metodo di registrazione
                        break;
                    case "2":
                        Login(conn);            // Chiamata al metodo di login
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

}





























































