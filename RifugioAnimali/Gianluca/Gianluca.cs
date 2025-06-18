using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;

public abstract class Utente
{
    //public string Utente_id { get; set; }
    public string Nome { get; set; }
    public string Cognome { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Telefono { get; set; }

    public Utente(string nome, string cognome, string email, string password, string telefono)
    {
        Nome = nome;
        Cognome = cognome;
        Email = email;
        Password = password;
        Telefono = telefono;
    }

    public abstract bool Login(string email, string password);

}

public class Program
{
    public static void Main(string[] args)
    {

        string connStr = "server=localhost; user =root; password=1234; port=3306; database=rifugio_animali;";
        MySqlConnection conn = new MySqlConnection(connStr);

        try
        {
            conn.Open();
            Console.WriteLine("Connessione riuscita!");

            bool esci = false;

            while (!esci)
            {
                Console.WriteLine("Benvenuto nel nostro rifuggio animali: ");
                Console.Write("Scegli un'opzione: ");
                Console.Write("[1]: Registrati");
                Console.Write("[2]: Accedi");
                Console.Write("[3]: Esci");
                string scelta = Console.ReadLine() ?? "Campo Obligatorio";

                switch (scelta)
                {
                    case "1":
                        Registrazione(conn);
                        break;
                    case "2":
                        Login(conn);
                        break;
                    case "3":
                        esci = true;
                        break;
                    default:
                        Console.WriteLine("Opzione non valida.");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore: " + ex.Message);
        }
    }

    private static void Registrazione(MySqlConnection conn)
    {
        Console.Write("Inserisci il tuo nome: ");
        string nome = Console.ReadLine() ?? "Campo oblogatorio";

        Console.Write("Inserisci il tuo cognome: ");
        string cognome = Console.ReadLine() ?? "Campo oblogatorio";

        Console.Write("Inserisci la tua email: ");
        string email = Console.ReadLine() ?? "Campo oblogatorio";

        Console.Write("Inserisci la tua password: ");
        string password = Console.ReadLine() ?? "Campo oblogatorio";

        Console.Write("Inserisci il tuo telefono: ");
        string telefono = Console.ReadLine() ?? "Campo oblogatorio";

        string query = "INSERT INTO utenti (nome, cognome, email, password, telefono) VALUES (@nome, @cognome, @email, @password, @telefono)";
        using MySqlCommand cmd = new MySqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@nome", nome);
        cmd.Parameters.AddWithValue("@cognome", cognome);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@password", password);
        cmd.Parameters.AddWithValue("@telefono", telefono);

        try
        {
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
            Console.WriteLine("Errore durante la registrazione: " + ex.Message);
        }

    }
    public static void Login(MySqlConnection conn)
    {
        Console.Write("Inserisci la tua email: ");
        string email = Console.ReadLine() ?? "Campo oblogatorio";

        Console.Write("Inserisci la tua password: ");
        string password = Console.ReadLine() ?? "Campo oblogatorio";

        string query = "SELECT * FROM utenti WHERE email = @Email AND password = @Password";
        using MySqlCommand cmd = new MySqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@Password", password);

        try
        {
            using MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
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







/*
    registrazione:

            // controllo input
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
        {
            Console.WriteLine("Email non valida.");
            return;
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            Console.WriteLine("Password troppo corta. Deve contenere almeno 6 caratteri.");
            return;
        }

        if (telefono.Length < 8 || telefono.Length > 15)
        {
            Console.WriteLine("Numero di telefono non valido.");
            return;
        }

        // controllo email gia registrate
        string checkQuery = "SELECT COUNT(*) FROM utenti WHERE email = @Email";
        using var checkCmd = new MySqlCommand(checkQuery, conn);
        checkCmd.Parameters.AddWithValue("@Email", email);
        long count = (long)checkCmd.ExecuteScalar();
        if (count > 0)
        {
            Console.WriteLine("Email già registrata.");
            return;
        } 
*/