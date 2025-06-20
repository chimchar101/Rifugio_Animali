using System;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;

public class Cliente : Utente // Classe Cliente che estende la classe Utente
{
    private int _utenteId;
    public int UtenteId
    {
        get { return _utenteId; }
    }

    private int _clienteId;
    public int ClienteId
    {
        get { return _clienteId; }
    }
    public Cliente(int utenteId, MySqlConnection connection) : base(utenteId)
    {
        _utenteId = utenteId;

        string sql = @"SELECT cliente_id 
                    FROM cliente 
                    WHERE utente_id = @utente_id";
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@utente_id", utenteId);
        MySqlDataReader rdr = cmd.ExecuteReader();
        rdr.Read();
        _clienteId = (int)rdr[0];
        rdr.Close();
    }

    /*public static void Registrazione(MySqlConnection connessione) // metodo per la registrazione dell'utente
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
                if (!IsEmailValid(email, connessione))  // Richiama il metodo IsEmailValid per verificare se l'email è valida
                {
                    Console.WriteLine("Inserisci un'email valida.");
                }
            } while (!IsEmailValid(email, connessione));

            Console.Write("Password: ");
            string password;
            do
            {
                password = Console.ReadLine().Trim();
                if (!IsPasswordValid(password, connessione)) // Richiama il metodo IsPasswordValid per verificare se la password è valida
                {
                    Console.WriteLine("Inserisci una password valida.");
                }
            } while (!IsPasswordValid(password, connessione));

            Console.Write("Telefono: ");
            string telefono;
            do
            {
                telefono = Console.ReadLine().Trim();
                if (!IsTelefonoValid(telefono, connessione)) // Richiama il metodo IsTelefonoValid per verificare se il telefono è valido
                {
                    Console.WriteLine("Inserisci un numero di telefono valido.");
                }
            } while (!IsTelefonoValid(telefono, connessione));

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

            int cittaId;    // Controlla se la città esiste

            string queryCittaEsiste = "SELECT citta_id FROM citta WHERE citta = @Citta";
            using (MySqlCommand cmd = new MySqlCommand(queryCittaEsiste, connessione))
            {
                cmd.Parameters.AddWithValue("@Citta", citta);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cittaId = Convert.ToInt32(reader["citta_id"]); // se esiste recupera id
                    }
                    else
                    {
                        reader.Close(); // chiudiamo il reader prima di eseguire un'altra query

                        string queryInsertCitta = "INSERT INTO citta (citta) VALUES (@Citta); SELECT LAST_INSERT_ID();"; // altrimenti la inserisco e recupero l'id appena inserito
                        using (MySqlCommand insertCmd = new MySqlCommand(queryInsertCitta, connessione))
                        {
                            insertCmd.Parameters.AddWithValue("@Citta", citta);
                            cittaId = Convert.ToInt32(insertCmd.ExecuteScalar()); // restituisce il primo valore della prima riga del risultato e lo salva nella variabile
                        }
                    }
                }
            }

            int indirizzoId;   // Controlla se l’indirizzo esiste per quella città

            string queryIndirizzoCheck = "SELECT indirizzo_id FROM indirizzo WHERE indirizzo = @Indirizzo AND citta_id = @CittaId";
            using (MySqlCommand cmd = new MySqlCommand(queryIndirizzoCheck, connessione))
            {
                cmd.Parameters.AddWithValue("@Indirizzo", indirizzo);
                cmd.Parameters.AddWithValue("@CittaId", cittaId);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    indirizzoId = Convert.ToInt32(result); // se esiste resituisce l'id dell'indirizzo
                }
                else
                {
                    string queryInsertIndirizzo = "INSERT INTO indirizzo (indirizzo, citta_id) VALUES (@Indirizzo, @CittaId); SELECT LAST_INSERT_ID();";
                    using (MySqlCommand insertCmd = new MySqlCommand(queryInsertIndirizzo, connessione))
                    {
                        insertCmd.Parameters.AddWithValue("@Indirizzo", indirizzo); // altrimenti lo inserisce e recupera l'id appena creato
                        insertCmd.Parameters.AddWithValue("@CittaId", cittaId);
                        indirizzoId = Convert.ToInt32(insertCmd.ExecuteScalar());
                    }
                }
            }

            // Inserisco finalmente l'utente
            string queryUtente = "INSERT INTO utente (nome, cognome, email, password, telefono, indirizzo_id) VALUES (@Nome, @Cognome, @Email, @Password, @Telefono, @IndirizzoId)";
            using (MySqlCommand cmd = new MySqlCommand(queryUtente, connessione))
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
                    Console.WriteLine("Registrazione completata con successo.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Errore durante la registrazione: " + ex.Message);
                }
            }

            int utenteId;
            string queryUtenteId = "select max(utente_id) from utente";
            using (MySqlCommand cmd = new MySqlCommand(queryUtenteId, connessione))
            {
                MySqlDataReader rdr = cmd.ExecuteReader();
                rdr.Read();
                utenteId = (int)rdr[0];
                rdr.Close();
            }

            string queryCliente = "insert into cliente(utente_id) values(@utente_id)";
            using (MySqlCommand cmd = new MySqlCommand(queryCliente, connessione))
            {
                cmd.Parameters.AddWithValue("@utente_id", utenteId);
                cmd.ExecuteNonQuery();
            }

            break; // Esce dal ciclo
        }
    } */

    public void StampaAnimali(MySqlConnection connection) // Metodo per stampare gli animali disponibili per l'adozione
    {
        string query = @"SELECT * FROM animale 
                        JOIN specie ON specie.specie_id=animale.specie_id 
                        WHERE adottato = false "; // Solo animali non adottati
        using (MySqlCommand cmd = new MySqlCommand(query, connection))
        {
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows) // Controlla se ci sono animali disponibili
                {
                    Console.WriteLine("--------------------------------------------------");
                    Console.WriteLine("ELENCO ANIMALI DISPONIBILI PER L'ADOZIONE");
                    Console.WriteLine("--------------------------------------------------");
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
    public void StampaAdozioni(MySqlConnection connection) // Metodo per stampare le adozioni effettuate dal cliente
    {
        string query = @"SELECT * FROM adozione
                        JOIN animale ON animale.animale_id = adozione.animale_id
                        WHERE cliente_id = @ClienteID";
        using (MySqlCommand cmd = new MySqlCommand(query, connection))
        {
            cmd.Parameters.AddWithValue("@ClienteID", ClienteId);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows) // Controlla se ci sono adozioni effettuate dal cliente
                {
                    Console.WriteLine("--------------------------------------------------");
                    Console.WriteLine("ELENCO ADOZIONI EFFETTUATE");
                    Console.WriteLine("--------------------------------------------------");
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID Adozione: {reader["adozione_id"]}, Animale Nome: {reader["nome"]}, Data Adozione: {reader["data"]}");
                    }
                }
                else
                {
                    Console.WriteLine("Nessuna adozione trovata.");
                }
            }
        }
    }

    /* public void ModificaProfilo(MySqlConnection connessione)
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
                    string query = "UPDATE utente SET nome = @nome WHERE utente_id = @UtenteID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connessione)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@nome", nome);
                        cmd.Parameters.AddWithValue("@UtenteID", this.UtenteId);

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
                    using (MySqlCommand cmd = new MySqlCommand(query, connessione)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@cognome", cognome);
                        cmd.Parameters.AddWithValue("@UtenteID", this.UtenteId);

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
                    string query = "UPDATE utente SET email = @email WHERE utente_id = @UtenteID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connessione)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@UtenteID", this.UtenteId);

                        int rows = 0;
                        if (IsTelefonoValid(email, connessione))
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
                    string query = "UPDATE utente SET password = @password WHERE utente_id = @UtenteID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connessione)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@UtenteID", this.UtenteId);

                        int rows = 0;
                        if (IsTelefonoValid(password, connessione))
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
                    string query = "UPDATE utente SET telefono = @telefono WHERE utente_id = @UtenteID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connessione)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@telefono", telefono);
                        cmd.Parameters.AddWithValue("@UtenteID", this.UtenteId);

                        int rows = 0;
                        if (IsTelefonoValid(telefono, connessione))
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
                    string query = "UPDATE indirizzo JOIN utente ON utente.indirizzo_id = indirizzo.indirizzo_id SET indirizzo = @indirizzo WHERE utente_id = @UtenteID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connessione)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@indirizzo", indirizzo);
                        cmd.Parameters.AddWithValue("@UtenteID", this.UtenteId);

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
                    string query = @"UPDATE citta JOIN indirizzo ON citta.citta_id = indirizzo.citta_id JOIN utente on utente.indirizzo_id = indirizzo.indirizzo_idSET citta = @citta 
                                    WHERE utente_id = @UtenteID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connessione)) // connessione deve essere già aperta
                    {
                        cmd.Parameters.AddWithValue("@citta", citta);
                        cmd.Parameters.AddWithValue("@UtenteID", this.UtenteId);

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
    }*/
    public void VisualizzaDiarioClinico(MySqlConnection connection)
    {
        string sql = @"SELECT animale_id, nome, eta 
                    FROM animale;";
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        MySqlDataReader rdr = cmd.ExecuteReader();
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("ELENCO ANIMALI");
        Console.WriteLine("--------------------------------------------------");
        while (rdr.Read())
        {
            Console.WriteLine($"ID: {rdr[0]}, Nome: {rdr[1]}, Età: {rdr[2]}");
        }
        rdr.Close();
        Console.Write("Seleziona ID animale: ");
        int animaleId = int.Parse(Console.ReadLine() ?? "Campo obbligatorio");


        string query = @"SELECT animale.animale_id, animale.nome, animale.vaccinato, diario_clinico.numero_visite, diario_clinico.ultima_visita, diario_clinico.prossimo_richiamo
        FROM animale
        JOIN diario_clinico ON diario_clinico.diario_id = animale.diario_id
        WHERE animale_id = @animaleId";

        using (cmd = new MySqlCommand(query, connection))
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

    /*
        public static bool IsEmailValid(string email, MySqlConnection connection) // Metodo per verificare se l'email è già registrata nel database
        {
            string query = "SELECT email FROM utente WHERE email = @Email"; // Query per verificare se l'email esiste già
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
    */

    /* public void AggiornaDiarioClinico(MySqlConnection connection) // metodo per aggiornare i dati del diario clinico
    {
        string sql = @"SELECT animale_id, nome, eta FROM animale;"; // stampo prima gli animali
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        MySqlDataReader rdr = cmd.ExecuteReader();
        Console.WriteLine("ID -- NOME -- ETA'");
        while (rdr.Read())
        {
            Console.WriteLine(rdr[0] + " -- " + rdr[1] + " -- " + rdr[2]);
        }
        rdr.Close();
        Console.Write("Seleziona ID animale: "); // faccio scegliere l'animale di cui aggiornare il diario clinico
        int animaleId = int.Parse(Console.ReadLine() ?? "Campo obbligatorio");

        string query = @"UPDATE diario_clinico 
                        JOIN animale ON animale.diario_id = diario_clinico.diario_id 
                        SET numero_visite = numero_visite + 1, ultima_visita = CURDATE(), prossimo_richiamo = DATE_ADD(CURDATE(), INTERVAL 1 YEAR
                        WHERE animale_id = @animale_id)";
        using (cmd = new MySqlCommand(query, connection)) // eseguo la query dell'update
        {
            cmd.Parameters.AddWithValue("@animale_id", animaleId);
        }
        int rows = cmd.ExecuteNonQuery();

        if (rows > 0)
        {
            Console.WriteLine("Visita aggiornata correttamente.");
        }
        else
        {
            Console.WriteLine("Nessun diario trovato con l'ID specificato.");
        }
    }
 */

    /* public void StampaDisegno()
    {
        Console.WriteLine("      (\\___/)");
        Console.WriteLine("     ( o   o )");
        Console.WriteLine("     (  =^=  )");
        Console.WriteLine("     (        )");
        Console.WriteLine("    (         )");
        Console.WriteLine("   (           )");
        Console.WriteLine("  ( (  )   (  ) )");
        Console.WriteLine(" (__(__)___(__)__)");
    } */
}