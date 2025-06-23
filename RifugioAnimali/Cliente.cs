using System;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;

public class Cliente : Utente // Classe Cliente che estende la classe Utente
{
    private int _utenteId;

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

    public void VisualizzaDiarioClinico(MySqlConnection connection)
    {
        string sql = @"SELECT animale_id, nome, eta 
                    FROM animale;";
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        MySqlDataReader rdr = cmd.ExecuteReader();
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("ELENCO ANIMALI");
        Console.WriteLine("--------------------------------------------------");
        List<int> idList = new List<int>();
        while (rdr.Read())
        {
            idList.Add((int)rdr[0]);
            Console.WriteLine($"ID: {rdr[0]}, Nome: {rdr[1]}, Età: {rdr[2]}");
        }
        rdr.Close();
        Console.Write("Seleziona ID animale: ");
        int animaleId = Input.SelectId(idList);


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
}