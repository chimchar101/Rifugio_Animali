using System;
using MySql.Data.MySqlClient;

public class Responsabile : Staff
{
    public void AggiungiSpecie(MySqlConnection connection)
    {
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("INSERIMENTO SPECIE");
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("Inserisci il nome della specie da inserire:");
        string specie = Console.ReadLine() ?? "Specie";

        string query = "insert into specie (specie) values (@specie)";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@specie", specie);

        try
        {
            cmd.ExecuteNonQuery();
            Console.WriteLine("Inserimento specie completata con successo.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante l'inserimento: " + ex.Message);
        }
    }

    public void AggiungiStaff(MySqlConnection connection)
    {
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("REGISTRAZIONE STAFF");
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("Inserisci nome:");
        string nome = Console.ReadLine() ?? "Nome";
        Console.WriteLine("Inserisci cognome:");
        string cognome = Console.ReadLine() ?? "Cognome";
        Console.WriteLine("Inserisci numero di telefono:");
        string telefono = Console.ReadLine() ?? "Telefono";
        Console.WriteLine("Inserisci email:");
        string email = Console.ReadLine() ?? "Email";
        Console.WriteLine("Inserisci password:");
        string password = Console.ReadLine() ?? "Password";

        string query = @"insert into utente (nome, cognome, telefono, email, password)
                        values (@nome, @cognome, @telefono, @email, @password)";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@nome", nome);
        cmd.Parameters.AddWithValue("@cognome", cognome);
        cmd.Parameters.AddWithValue("@telefono", telefono);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@password", password);

        try
        {
            cmd.ExecuteNonQuery();
            Console.WriteLine("Registrazione utente completata con successo.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante la registrazione: " + ex.Message);
        }

        query = "select max(utente_id) from utente";
        cmd = new MySqlCommand(query, connection);
        int utenteId = 0;
        try
        {
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                utenteId = Convert.ToInt32(reader["utente_id"]);
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

    public void RimuoviStaff(MySqlConnection connection)
    {
        StampaStaff(connection);

        Console.WriteLine("Inserisci l'ID dello staff da rimuovere:");
        int staffId = int.Parse(Console.ReadLine() ?? "0");

        string query = "delete from staff where staff_id = @staffId";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@staffId", staffId);
        try
        {
            cmd.ExecuteNonQuery();
            Console.WriteLine("Staff rimosso con successo.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante la rimozione dello staff: " + ex.Message);
        }
    }

    public void StampaStaff(MySqlConnection connection)
    {
        string query = @"select u.nome as Nome, u.cognome as Cognome, u.email as Email, u.telefono as Telefono, s.is_admin as IsAdmin
                        from staff s
                        join utente u on s.utente_id = u.utente_id";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        try
        {
            MySqlDataReader reader = cmd.ExecuteReader();
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("ELENCO STAFF");
            Console.WriteLine("--------------------------------------------------");
            while (reader.Read())
            {
                Console.WriteLine($"Nome: {reader["Nome"]}, Cognome: {reader["Cognome"]}, Email: {reader["Email"]}, Telefono: {reader["Telefono"]}, IsAdmin: {reader["IsAdmin"]}");
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante la stampa dello staff: " + ex.Message);
        }
    }

    public void StampaClienti(MySqlConnection connection)
    {
        string query = @"select u.nome as Nome, u.cognome as Cognome, u.email as Email, u.telefono as Telefono
                        from cliente c
                        join utente u on c.utente_id = u.utente_id";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        try
        {
            MySqlDataReader reader = cmd.ExecuteReader();
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("ELENCO CLIENTI");
            Console.WriteLine("--------------------------------------------------");
            while (reader.Read())
            {
                Console.WriteLine($"Nome: {reader["Nome"]}, Cognome: {reader["Cognome"]}, Email: {reader["Email"]}, Telefono: {reader["Telefono"]}");
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante la stampa dei clienti: " + ex.Message);
        }
    }
}