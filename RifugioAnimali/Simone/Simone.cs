using System;
using MySql.Data.MySqlClient;

public class Responsabile : Staff
{
    public Responsabile(int utenteId, MySqlConnection connection) : base(utenteId, connection)
    {
        _utenteId = utenteId;

        string sql = "select staff_id from staff where utente_id = @utente_id";
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@utente_id", utenteId);
        MySqlDataReader rdr = cmd.ExecuteReader();
        rdr.Read();
        _staffId = (int)rdr[0];
        rdr.Close();
    }

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

    /* public void AggiungiStaff(MySqlConnection connection)
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
    } */

    public void RimuoviStaff(MySqlConnection connection)
    {
        StampaVolontari(connection);

        Console.WriteLine("Inserisci l'ID dello staff da rimuovere:");
        int staffId = int.Parse(Console.ReadLine() ?? "0");

        string query = "select utente_id from staff where staff_id = @staff_id;";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@staff_id", staffId);
        MySqlDataReader rdr = cmd.ExecuteReader();
        rdr.Read();
        int utenteId = (int)rdr[0];
        rdr.Close();

        query = "delete from staff where staff_id = @staffId";
        cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@staffId", staffId);
        try
        {
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante la rimozione dello staff: " + ex.Message);
        }

        query = "delete from utente where utente_id = @utente_id";
        cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@utente_id", utenteId);
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
        string query = @"select s.staff_id, u.nome as Nome, u.cognome as Cognome, u.email as Email, u.telefono as Telefono, s.is_admin as IsAdmin
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
                Console.WriteLine($"ID: {reader["staff_id"]}, Nome: {reader["Nome"]}, Cognome: {reader["Cognome"]}, Email: {reader["Email"]}, Telefono: {reader["Telefono"]}, IsAdmin: {reader["IsAdmin"]}");
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante la stampa dello staff: " + ex.Message);
        }
    }

    public void StampaVolontari(MySqlConnection connection)
    {
        string query = @"select s.staff_id, u.nome as Nome, u.cognome as Cognome, u.email as Email, u.telefono as Telefono, s.is_admin as IsAdmin
                        from staff s
                        join utente u on s.utente_id = u.utente_id
                        where s.is_admin = false";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        try
        {
            MySqlDataReader reader = cmd.ExecuteReader();
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("ELENCO VOLONTARI");
            Console.WriteLine("--------------------------------------------------");
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["staff_id"]}, Nome: {reader["Nome"]}, Cognome: {reader["Cognome"]}, Email: {reader["Email"]}, Telefono: {reader["Telefono"]}, IsAdmin: {reader["IsAdmin"]}");
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
        string query = @"select c.cliente_id, u.nome as Nome, u.cognome as Cognome, u.email as Email, u.telefono as Telefono
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
                Console.WriteLine($"ID: {reader["cliente_id"]}, Nome: {reader["Nome"]}, Cognome: {reader["Cognome"]}, Email: {reader["Email"]}, Telefono: {reader["Telefono"]}");
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante la stampa dei clienti: " + ex.Message);
        }
    }

    public void StampaUtenti(MySqlConnection connection)
    {
        string query = @"select u.utente_id, u.nome as Nome, u.cognome as Cognome, u.email as Email, u.telefono as Telefono, i.indirizzo as Indirizzo, c.citta as Citta
                        from utente u
                        join indirizzo i on i.indirizzo_id = u.indirizzo_id
                        join citta c on c.citta_id = i.citta_id
                        where u.utente_id not in(
                            select utente_id from staff where is_admin = true
                        )";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        try
        {
            MySqlDataReader reader = cmd.ExecuteReader();
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("ELENCO UTENTI");
            Console.WriteLine("--------------------------------------------------");
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["utente_id"]}, Nome: {reader["Nome"]}, Cognome: {reader["Cognome"]}, Email: {reader["Email"]}, Telefono: {reader["Telefono"]}, Indirizzo: {reader["Indirizzo"]}, Città: {reader["Citta"]}");
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante la stampa dello staff: " + ex.Message);
        }
    }
    public void ModificaUtente(MySqlConnection connection)
    {
        StampaUtenti(connection);

        Console.Write("Inserisci ID dell'utente da modificare: ");
        int selectedUtenteId = int.Parse(Console.ReadLine() ?? "Campo obbligatorio");

        Console.WriteLine("Modifica il tuo profilo:");
        Console.WriteLine("Quale campo vuoi modificare?\n 1. Nome\n 2. Cognome\n 3. Telefono\n 4. Indirizzo\n 5. Città\n 6. Esci");
        string scelta = Console.ReadLine().Trim();
        switch (scelta)
        {
            case "1":
                Console.WriteLine("Inserisci il nuovo nome");
                string nome = Console.ReadLine().Trim().ToLower();
                if (!string.IsNullOrEmpty(nome))
                {
                    string query = "UPDATE utente SET nome = @nome WHERE utente_id = @UtenteID";
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
                    string query = "UPDATE utente SET cognome = @cognome WHERE utente_id = @UtenteID";
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
                Console.WriteLine("Inserisci il nuovo telefono");
                string telefono = Console.ReadLine().Trim().ToLower();
                if (!string.IsNullOrEmpty(telefono))
                {
                    string query = "UPDATE utente SET telefono = @telefono WHERE utente_id = @UtenteID";
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
            case "4":
                Console.WriteLine("Inserisci il nuovo indirizzo");
                string indirizzo = Console.ReadLine().Trim().ToLower();
                if (!string.IsNullOrEmpty(indirizzo))
                {
                    string query = "UPDATE indirizzo JOIN utente ON utente.indirizzo_id = indirizzo.indirizzo_id SET indirizzo = @indirizzo WHERE utente_id = @UtenteID";
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
            case "5":
                Console.WriteLine("Inserisci la nuova città");
                string citta = Console.ReadLine().Trim().ToLower();
                if (!string.IsNullOrEmpty(citta))
                {
                    string query = @"UPDATE citta JOIN indirizzo ON citta.citta_id = indirizzo.citta_id JOIN utente on utente.indirizzo_id = indirizzo.indirizzo_idSET citta = @citta 
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