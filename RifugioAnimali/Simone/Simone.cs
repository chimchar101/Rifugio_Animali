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
        string specie = Console.ReadLine() ?? "Campo Obbligatorio";

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
    }

    public void RimuoviStaff(MySqlConnection connection)
    {
        StampaStaff(connection);

        Console.WriteLine("Inserisci l'ID dello staff da rimuovere:");
        int staffId = int.Parse(Console.ReadLine() ?? "Campo Obbligatorio");

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