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
        string specie = Input.String();

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

    public void RimuoviStaff(MySqlConnection connection)
    {
        StampaVolontari(connection);

        Console.WriteLine("Inserisci l'ID dello staff da rimuovere:");
        int staffId = Input.Int();

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

    public void ModificaUtente(MySqlConnection connection)
    {
        string query = @"select u.utente_id, u.nome as Nome, u.cognome as Cognome, u.email as Email, u.telefono as Telefono, i.indirizzo as Indirizzo, c.citta as Citta
                        from utente u
                        join indirizzo i on i.indirizzo_id = u.indirizzo_id
                        join citta c on c.citta_id = i.citta_id
                        where u.utente_id not in(
                            select utente_id from staff where is_admin = true
                        )";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        MySqlDataReader reader = cmd.ExecuteReader();
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("ELENCO UTENTI");
        Console.WriteLine("--------------------------------------------------");
        List<int> idList = new List<int>();
        while (reader.Read())
        {
            idList.Add((int)reader[0]);
            Console.WriteLine($"ID: {reader["utente_id"]}, Nome: {reader["Nome"]}, Cognome: {reader["Cognome"]}, Email: {reader["Email"]}, Telefono: {reader["Telefono"]}, Indirizzo: {reader["Indirizzo"]}, Città: {reader["Citta"]}");
        }
        reader.Close();

        Console.Write("Inserisci ID dell'utente da modificare: ");
        int selectedUtenteId = Input.SelectId(idList);

        Console.WriteLine("Modifica il tuo profilo:");
        Console.WriteLine("Quale campo vuoi modificare?\n 1. Nome\n 2. Cognome\n 3. Telefono\n 4. Indirizzo\n 5. Città\n 6. Esci");
        string scelta = Input.String();
        switch (scelta)
        {
            case "1":
                Console.WriteLine("Inserisci il nuovo nome");
                string nome = Input.String();
                nome = nome.Trim().ToLower();

                if (!string.IsNullOrEmpty(nome))
                {
                    query = "UPDATE utente SET nome = @nome WHERE utente_id = @UtenteID";
                    using (cmd = new MySqlCommand(query, connection)) // connessione deve essere già aperta
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
                string cognome = Input.String();
                cognome = cognome.Trim().ToLower();
                if (!string.IsNullOrEmpty(cognome))
                {
                    query = "UPDATE utente SET cognome = @cognome WHERE utente_id = @UtenteID";
                    using (cmd = new MySqlCommand(query, connection)) // connessione deve essere già aperta
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
                string telefono = Input.String();
                telefono = telefono.Trim().ToLower();
                if (!string.IsNullOrEmpty(telefono))
                {
                    query = "UPDATE utente SET telefono = @telefono WHERE utente_id = @UtenteID";
                    using (cmd = new MySqlCommand(query, connection)) // connessione deve essere già aperta
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
                string indirizzo = Input.String();
                indirizzo = indirizzo.Trim().ToLower();
                if (!string.IsNullOrEmpty(indirizzo))
                {
                    query = "UPDATE indirizzo JOIN utente ON utente.indirizzo_id = indirizzo.indirizzo_id SET indirizzo = @indirizzo WHERE utente_id = @UtenteID";
                    using (cmd = new MySqlCommand(query, connection)) // connessione deve essere già aperta
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
                string citta = Input.String();
                citta = citta.Trim().ToLower();
                if (!string.IsNullOrEmpty(citta))
                {
                    query = @"UPDATE citta JOIN indirizzo ON citta.citta_id = indirizzo.citta_id JOIN utente on utente.indirizzo_id = indirizzo.indirizzo_idSET citta = @citta 
                                    WHERE utente_id = @UtenteID";
                    using (cmd = new MySqlCommand(query, connection)) // connessione deve essere già aperta
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