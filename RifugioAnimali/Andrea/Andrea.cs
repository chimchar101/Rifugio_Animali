using System;
using System.Collections.Specialized;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Cms;

public abstract class Utente { }

public class Staff : Utente
{
    private int _staffID;
    public int StaffID
    {
        get { return _staffID; }
    }

    public Staff(int staffID)
    {
        _staffID = staffID;
    }

    private int SelectSpecieID(MySqlConnection connection)
    {
        string sql = @"select specie_id, specie from specie;";
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        MySqlDataReader rdr = cmd.ExecuteReader();
        Console.WriteLine("id -- specie");
        while (rdr.Read())
        {
            Console.WriteLine(rdr[0] + " -- " + rdr[1]);
        }
        rdr.Close();
        Console.Write("Seleziona ID specie: ");
        int specieID = int.Parse(Console.ReadLine() ?? "Campo obbligatorio");
        return specieID;
    }

    public void StampaAnimali(MySqlConnection connection)
    {
        string sql = @"select a.animale_id, s.specie, a.nome, a.eta, a.vaccinato, a.adottato 
                    from animale a join specie s on s.specie_id = a.specie_id;";
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        MySqlDataReader rdr = cmd.ExecuteReader();
        Console.WriteLine("id -- specie -- nome -- età -- vaccinato -- adottato");
        while (rdr.Read())
        {
            Console.WriteLine(rdr[0] + " -- " + rdr[1] + " -- " + rdr[2] + " -- " + rdr[3] + " -- " + rdr[4] + " -- " + rdr[5]);
        }
        rdr.Close();
    }

    public void AffidaAnimale(MySqlConnection connection)
    {
        try
        {
            int specieID = SelectSpecieID(connection);

            string sql = @"select a.animale_id, s.specie, a.nome, a.eta, a.vaccinato
            from animale a join specie s on s.specie_id = a.specie_id
            where specie_id = @specie_id and adottato = false";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@specie_id", specieID);
            MySqlDataReader rdr = cmd.ExecuteReader();

            if (!rdr.Read())
            {
                Console.WriteLine("Nessun animale trovato");
                rdr.Close();
                return;
            }

            Console.WriteLine("id -- specie -- nome -- età -- vaccinato");
            while (rdr.Read())
            {
                Console.WriteLine(rdr[0] + " -- " + rdr[1] + " -- " + rdr[2] + " -- " + rdr[3] + " -- " + rdr[4]);
            }
            rdr.Close();

            Console.Write("Seleziona ID animale: ");
            int animaleID = int.Parse(Console.ReadLine() ?? "Campo obbligatorio");

            Console.Write("Inserisci nome cliente: ");
            string nomeCliente = Console.ReadLine() ?? "Campo obbligatorio";
            nomeCliente = nomeCliente.ToLower().Trim();
            Console.Write("Inserisci cognome cliente: ");
            string cognomeCliente = Console.ReadLine() ?? "Campo obbligatorio";
            cognomeCliente = cognomeCliente.ToLower().Trim();

            sql = @"select u.utente_id, u.nome, u.cognome, i.indirizzo, c.citta
            from utente u join indirizzo i on u.indirizzo_id = i.indirizzo_id
            join citta c on i.citta_id = c.citta_id
            where u.nome = @nome and u.cognome = @cognome";
            cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@nome", nomeCliente);
            cmd.Parameters.AddWithValue("@cognome", cognomeCliente);
            rdr = cmd.ExecuteReader();

            if (!rdr.Read())
            {
                Console.WriteLine("Nessun cliente trovato");
                rdr.Close();
                return;
            }

            Console.WriteLine("id -- nome -- cognome -- indirizzo -- città");
            while (rdr.Read())
            {
                Console.WriteLine(rdr[0] + " -- " + rdr[1] + " -- " + rdr[2] + " -- " + rdr[3] + " -- " + rdr[4]);
            }
            rdr.Close();

            Console.WriteLine("Seleziona ID cliente:");
            int clienteID = int.Parse(Console.ReadLine() ?? "Campo obbligatorio");

            Console.Write("Usare la data di oggi? (true/false): ");
            bool useCurrDate = bool.Parse(Console.ReadLine() ?? "Campo obbligatorio");

            sql = @"insert into adozione(data, animale_id, cliente_id, staff_id)
            values (@data, @animale_id, @cliente_id, @staff_id)";
            cmd = new MySqlCommand(sql, connection);

            if (!useCurrDate)
            {
                Console.Write("Inserire la data (yyyy,mm,gg): ");
                DateTime data = DateTime.Parse(Console.ReadLine() ?? "Campo obbligatorio");
                cmd.Parameters.AddWithValue("@data", data);
            }

            cmd.Parameters.AddWithValue("@animale_id", animaleID);
            cmd.Parameters.AddWithValue("@cliente_id", clienteID);
            cmd.Parameters.AddWithValue("@staff_id", StaffID);
            cmd.ExecuteNonQuery();
            Console.WriteLine("Animale affidato con successo.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public void AggiungiAnimale(MySqlConnection connection)
    {
        try
        {
            int specieID = SelectSpecieID(connection);

            string sql = @"insert into diario_clinico(numero_visite, ultima_visita, prossimo_richiamo)
            values (0, null, null)";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.ExecuteNonQuery();

            Console.Write("Inserisci nome: ");
            string nomeAnimale = Console.ReadLine() ?? "Campo obbligatorio";
            nomeAnimale = nomeAnimale.ToLower().Trim();

            Console.Write("Inserisci età: ");
            int etaAnimale = int.Parse(Console.ReadLine() ?? "Campo obbligatorio");

            Console.Write("E' stato vaccinato? (true/false): ");
            bool isVaccinato = bool.Parse(Console.ReadLine() ?? "Campo obbligatorio");

            Console.Write("E' stato adottato? (true/false): ");
            bool isAdottato = bool.Parse(Console.ReadLine() ?? "Campo obbligatorio");

            sql = @"select max(diario_id) from diario_clinico";
            cmd = new MySqlCommand(sql, connection);
            MySqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();
            int diarioID = (int)rdr[0];
            rdr.Close();

            sql = @"insert into animale(nome, eta, vaccinato, adottato, specie_id, diario_id)
            values (@nome, @eta, @vaccinato, @adottato, @specie_id, @diario_id)";
            cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@nome", nomeAnimale);
            cmd.Parameters.AddWithValue("@eta", etaAnimale);
            cmd.Parameters.AddWithValue("@vaccinato", isVaccinato);
            cmd.Parameters.AddWithValue("@adottato", isAdottato);
            cmd.Parameters.AddWithValue("@specie_id", specieID);
            cmd.Parameters.AddWithValue("@diario_id", diarioID);
            cmd.ExecuteNonQuery();

            Console.Write("Usare la data di oggi? (true/false): ");
            bool useCurrDate = bool.Parse(Console.ReadLine() ?? "Campo obbligatorio");

            sql = @"insert into ingresso(data, animale_id, staff_id)
            values (@data, @animale_id, @staff_id)";
            cmd = new MySqlCommand(sql, connection);

            if (!useCurrDate)
            {
                Console.Write("Inserire la data (yyyy,mm,gg): ");
                DateTime data = DateTime.Parse(Console.ReadLine() ?? "Campo obbligatorio");
                cmd.Parameters.AddWithValue("@data", data);
            }

            sql = @"select max(animale_id) from animale";
            cmd = new MySqlCommand(sql, connection);
            rdr = cmd.ExecuteReader();
            rdr.Read();
            int animaleID = (int)rdr[0];
            rdr.Close();

            cmd.Parameters.AddWithValue("@animale_id", animaleID);
            cmd.Parameters.AddWithValue("@staff_id", StaffID);
            cmd.ExecuteNonQuery();

            Console.WriteLine("Animale aggiunto con successo.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public void StampaAdozioni(MySqlConnection connection)
    {
        string sql = @"select ad.adozione:id, ad.data, a.nome, concat(c.nome, c.cognome) as nome_cliente, concat(s.nome, s.cognome) as nome_staff,
                    from adozione ad join animale a on ad.animale_id = a.animale_id join
                    join cliente c on ad.cliente_id = c.cliente_id join staff s on ad.staff_id = s.staff_id;";
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        MySqlDataReader rdr = cmd.ExecuteReader();
        Console.WriteLine("id -- data -- nome animale -- nome cliente -- nome staff");
        while (rdr.Read())
        {
            Console.WriteLine(rdr[0] + " -- " + rdr[1] + " -- " + rdr[2] + " -- " + rdr[3] + " -- " + rdr[4]);
        }
        rdr.Close();
    }

    public void AggiungiInventario(MySqlConnection connection)
    {
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\nSeleziona il tipo:");
            Console.WriteLine("[1] Cibo");
            Console.WriteLine("[2] Medicina");
            Console.WriteLine("[3] Accessorio");
            Console.Write("Scelta: ");
            int menuAction = int.Parse(Console.ReadLine() ?? "Campo obbligatorio");

            switch (menuAction)
            {
                case 1:
                    AggiungiCibo(connection);
                    break;

                case 2:
                    AggiungiMedicina(connection);
                    break;

                case 3:
                    AggiungiAccessorio(connection);
                    break;

                default:
                    Console.WriteLine("Scelta non valida.");
                    break;
            }
        }
    }

    private void AggiungiCibo(MySqlConnection connection)
    {

    }

    private void AggiungiMedicina(MySqlConnection connection)
    {

    }
    
    private void AggiungiAccessorio(MySqlConnection connection)
    {

    }
}