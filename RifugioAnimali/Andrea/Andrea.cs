using System;
using MySql.Data.MySqlClient;

public class Staff : Utente
{
    protected int _utenteId;
    public int UtenteId
    {
        get { return _utenteId; }
    }

    protected int _staffId;
    public int StaffId
    {
        get { return _staffId; }
    }

    public Staff(int utenteId, MySqlConnection connection) : base(utenteId)
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

    private enum InvType
    {
        cibo,
        medicina,
        accessorio
    }

    private int SelectSpecieID(MySqlConnection connection) // stampa la lista di specie e fa selezionare l'id all'utente
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

    public void StampaAnimali(MySqlConnection connection) // stampa la lista degli animali
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

    public void AffidaAnimale(MySqlConnection connection) // da in adozione un animale ad un cliente
    {
        try
        {
            int specieID = SelectSpecieID(connection); // seleziona specieID

            string sql = @"select a.animale_id, s.specie, a.nome, a.eta, a.vaccinato
            from animale a join specie s on s.specie_id = a.specie_id
            where s.specie_id = @specie_id and a.adottato = false";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@specie_id", 1);
            MySqlDataReader rdr = cmd.ExecuteReader();  // seleziona lista animali non adottati della specie selezionata

            if (!rdr.Read())
            {
                Console.WriteLine("Nessun animale trovato");
                rdr.Close();
                return;
            }
            rdr.Close();

            rdr = cmd.ExecuteReader();
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

            sql = @"select cl.cliente_id, u.nome, u.cognome, i.indirizzo, c.citta
            from utente u join indirizzo i on u.indirizzo_id = i.indirizzo_id
            join citta c on i.citta_id = c.citta_id
            join cliente cl on cl.utente_id = u.utente_id
            where u.nome = @nome and u.cognome = @cognome";
            cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@nome", nomeCliente);
            cmd.Parameters.AddWithValue("@cognome", cognomeCliente);
            rdr = cmd.ExecuteReader();  // seleziona utenti tramite nome e cognome  

            if (!rdr.Read())
            {
                Console.WriteLine("Nessun cliente trovato");
                rdr.Close();
                return;
            }
            rdr.Close();

            rdr = cmd.ExecuteReader();
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
            cmd.Parameters.AddWithValue("@staff_id", StaffId);
            cmd.ExecuteNonQuery();  // aggiunge l'animale nella tabella adozione

            sql = @"update animale set adottato = true where animale_id = @animale_id";
            cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@animale_id", animaleID);
            cmd.ExecuteNonQuery();  // imposta a true il valore "adottato" dell'animale

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
            cmd.ExecuteNonQuery();  //crea un nuovo diario clinico vuoto

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
            cmd.ExecuteNonQuery();  // aggiunge l'animale alla tabella animale

            Console.Write("Usare la data di oggi? (true/false): ");
            bool useCurrDate = bool.Parse(Console.ReadLine() ?? "Campo obbligatorio");

            sql = @"insert into ingresso(animale_id, staff_id)
            values (@animale_id, @staff_id)";
            cmd = new MySqlCommand(sql, connection);

            DateTime data;

            if (!useCurrDate)
            {
                sql = @"insert into ingresso(data, animale_id, staff_id)
                    values (@data, @animale_id, @staff_id)";
                Console.Write("Inserire la data (yyyy,mm,gg): ");
                data = DateTime.Parse(Console.ReadLine() ?? "Campo obbligatorio");
                cmd.Parameters.AddWithValue("@data", data);
            }
        
            string sql2 = @"select max(animale_id) from animale";
            MySqlCommand cmd2 = new MySqlCommand(sql2, connection);
            MySqlDataReader rdr2 = cmd2.ExecuteReader();
            rdr2.Read();
            int animaleID = (int)rdr2[0];
            rdr2.Close();

            cmd.Parameters.AddWithValue("@animale_id", animaleID);
            cmd.Parameters.AddWithValue("@staff_id", 1);
            cmd.ExecuteNonQuery();  // aggiunge i dati di ingresso nella tabella ingresso

            Console.WriteLine("Animale aggiunto con successo.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public void StampaAdozioni(MySqlConnection connection)
    {
        string sql = @"select ad.adozione_id, ad.data, a.nome, concat(u.nome,' ', u.cognome) as nome_cliente, concat(u2.nome,' ', u2.cognome) as nome_staff
                from adozione ad join animale a on ad.animale_id = a.animale_id join cliente c on ad.cliente_id = c.cliente_id 
                join utente u on u.utente_id = c.utente_id join staff s on ad.staff_id = s.staff_id join utente u2 on u2.utente_id = s.utente_id;";
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        MySqlDataReader rdr = cmd.ExecuteReader();  // seleziona lista adozioni
        Console.WriteLine("id -- data -- nome animale -- nome cliente -- nome staff");
        while (rdr.Read())
        {
            Console.WriteLine(rdr[0] + " -- " + rdr[1] + " -- " + rdr[2] + " -- " + rdr[3] + " -- " + rdr[4]);
        }
        rdr.Close();
    }

    public void AggiungiInventario(MySqlConnection connection)
    {

        Console.WriteLine("\nSeleziona il tipo:");
        Console.WriteLine("[1] Cibo");
        Console.WriteLine("[2] Medicina");
        Console.WriteLine("[3] Accessorio");
        Console.Write("Scelta: ");
        string menuAction = Console.ReadLine() ?? "Campo obbligatorio";

        switch (menuAction)
        {
            case "1":
                AggiungiCibo(connection);
                break;

            case "2":
                AggiungiMedicina(connection);
                break;

            case "3":
                AggiungiAccessorio(connection);
                break;

            default:
                Console.WriteLine("Scelta non valida.");
                break;
        
        }
    }

    private int SelectOrAddCategoriaCiboID(MySqlConnection connection) // fa selezionare una categoria di cibo in inventario o ne aggiunge una
    {
        string sql = @"select categoria_id, nome, descrizione from categoria_cibo;";
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        MySqlDataReader rdr = cmd.ExecuteReader();
        Console.WriteLine("id -- categoria -- descrizione");
        while (rdr.Read())
        {
            Console.WriteLine(rdr[0] + " -- " + rdr[1] + " -- " + rdr[2]);
        }
        rdr.Close();
        Console.Write("Seleziona ID categoria o inserisci \"0\" per aggiungerne una nuova: ");
        int categoriaID = int.Parse(Console.ReadLine() ?? "Campo obbligatorio");
        if (categoriaID == 0)
        {
            Console.Write("Inserisci nome categoria: ");
            string nomeCategoria = Console.ReadLine() ?? "Campo obbligatorio";
            nomeCategoria = nomeCategoria.ToLower().Trim();
            Console.Write("Inserisci descrizione: ");
            string descrizioneCategoria = Console.ReadLine() ?? "Campo obbligatorio";
            descrizioneCategoria = descrizioneCategoria.ToLower().Trim();

            sql = @"insert into categoria_cibo(nome, descrizione)
            values (@nome, @descrizione)";
            cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@nome", nomeCategoria);
            cmd.Parameters.AddWithValue("@descrizione", descrizioneCategoria);
            cmd.ExecuteNonQuery();  // aggiunge la nuova categoria

            sql = @"select max(categoria_id) from categoria_cibo";
            cmd = new MySqlCommand(sql, connection);
            rdr = cmd.ExecuteReader();
            rdr.Read();
            categoriaID = (int)rdr[0];
            rdr.Close();
        }
        return categoriaID;
    }

    private int SelectOrAddCategoriaMedicinaID(MySqlConnection connection) // fa selezionare una categoria di medicina in inventario o ne aggiunge una
    {
        string sql = @"select categoria_id, nome, descrizione from categoria_medicina;";
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        MySqlDataReader rdr = cmd.ExecuteReader();
        Console.WriteLine("id -- categoria -- descrizione");
        while (rdr.Read())
        {
            Console.WriteLine(rdr[0] + " -- " + rdr[1] + " -- " + rdr[2]);
        }
        rdr.Close();
        Console.Write("Seleziona ID categoria o inserisci \"0\" per aggiungerne una nuova: ");
        int categoriaID = int.Parse(Console.ReadLine() ?? "Campo obbligatorio");
        if (categoriaID == 0)
        {
            Console.Write("Inserisci nome categoria: ");
            string nomeCategoria = Console.ReadLine() ?? "Campo obbligatorio";
            nomeCategoria = nomeCategoria.ToLower().Trim();
            Console.Write("Inserisci descrizione: ");
            string descrizioneCategoria = Console.ReadLine() ?? "Campo obbligatorio";
            descrizioneCategoria = descrizioneCategoria.ToLower().Trim();

            sql = @"insert into categoria_medicina(nome, descrizione)
            values (@nome, @descrizione)";
            cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@nome", nomeCategoria);
            cmd.Parameters.AddWithValue("@descrizione", descrizioneCategoria);
            cmd.ExecuteNonQuery();  // aggiunge la nuova categoria

            sql = @"select max(categoria_id) from categoria_medicina";
            cmd = new MySqlCommand(sql, connection);
            rdr = cmd.ExecuteReader();
            rdr.Read();
            categoriaID = (int)rdr[0];
            rdr.Close();
        }
        return categoriaID;
    }

    private int SelectOrAddCategoriaAccessorioID(MySqlConnection connection) // fa selezionare una categoria di accessorio in inventario o ne aggiunge una
    {
        string sql = @"select categoria_id, nome, descrizione from categoria_accessorio;";
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        MySqlDataReader rdr = cmd.ExecuteReader();
        Console.WriteLine("id -- categoria -- descrizione");
        while (rdr.Read())
        {
            Console.WriteLine(rdr[0] + " -- " + rdr[1] + " -- " + rdr[2]);
        }
        rdr.Close();
        Console.Write("Seleziona ID categoria o inserisci \"0\" per aggiungerne una nuova: ");
        int categoriaID = int.Parse(Console.ReadLine() ?? "Campo obbligatorio");
        if (categoriaID == 0)
        {
            Console.Write("Inserisci nome categoria: ");
            string nomeCategoria = Console.ReadLine() ?? "Campo obbligatorio";
            nomeCategoria = nomeCategoria.ToLower().Trim();
            Console.Write("Inserisci descrizione: ");
            string descrizioneCategoria = Console.ReadLine() ?? "Campo obbligatorio";
            descrizioneCategoria = descrizioneCategoria.ToLower().Trim();

            sql = @"insert into categoria_accessorio(nome, descrizione)
            values (@nome, @descrizione)";
            cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@nome", nomeCategoria);
            cmd.Parameters.AddWithValue("@descrizione", descrizioneCategoria);
            cmd.ExecuteNonQuery();  // aggiunge la nuova categoria

            sql = @"select max(categoria_id) from categoria_accessorio";
            cmd = new MySqlCommand(sql, connection);
            rdr = cmd.ExecuteReader();
            rdr.Read();
            categoriaID = (int)rdr[0];
            rdr.Close();
        }
        return categoriaID;
    }

    private int AddInventarioID(MySqlConnection connection, DateTime? scadenza) // aggiunge un nuovo elemento in inventario e ritorna l'id
    {
        string sql = @"insert into inventario(scadenza)
                    values (@scadenza)";
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@scadenza", scadenza);
        cmd.ExecuteNonQuery();

        sql = @"select max(inventario_id) from inventario";
        cmd = new MySqlCommand(sql, connection);
        MySqlDataReader rdr = cmd.ExecuteReader();
        rdr.Read();
        int inventarioID = (int)rdr[0];
        rdr.Close();

        return inventarioID;
    }

    private void AggiungiCibo(MySqlConnection connection) // aggiunge cibo nella tabella cibo e inventario
    {
        try
        {
            int categoriaID = SelectOrAddCategoriaCiboID(connection); // seleziona categoriaID

            Console.Write("Inserisci nome cibo: ");
            string nomeCibo = Console.ReadLine() ?? "Campo obbligatorio";
            nomeCibo = nomeCibo.ToLower().Trim();

            Console.Write("Inserire la data di scadenza(yyyy,mm,gg): ");
            DateTime scadenza = DateTime.Parse(Console.ReadLine() ?? "Campo obbligatorio");

            int specieID = SelectSpecieID(connection);  // seleziona specieID

            Console.Write("Inserisci quantità da aggiungere: ");
            int amount = int.Parse(Console.ReadLine() ?? "Campo obbligatorio");
            
            for (int i = 0; i < amount; i++)    // aggiunge la quantità desiderata
            {
                int inventarioID = AddInventarioID(connection, scadenza);
                string sql = @"insert into cibo(nome, categoria_id, inventario_id, specie_id)
                values (@nome, @categoria_id, @inventario_id, @specie_id)";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@nome", nomeCibo);
                cmd.Parameters.AddWithValue("@categoria_id", categoriaID);
                cmd.Parameters.AddWithValue("@inventario_id", inventarioID);
                cmd.Parameters.AddWithValue("@specie_id", specieID);
                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("Cibo aggiunto con successo");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private void AggiungiMedicina(MySqlConnection connection)
    {
        try
        {
            int categoriaID = SelectOrAddCategoriaMedicinaID(connection);

            Console.Write("Inserisci nome medicina: ");
            string nomeCibo = Console.ReadLine() ?? "Campo obbligatorio";
            nomeCibo = nomeCibo.ToLower().Trim();

            Console.Write("Inserire la data di scadenza(yyyy,mm,gg): ");
            DateTime scadenza = DateTime.Parse(Console.ReadLine() ?? "Campo obbligatorio");

            int specieID = SelectSpecieID(connection);

            Console.Write("Inserisci quantità da aggiungere: ");
            int amount = int.Parse(Console.ReadLine() ?? "Campo obbligatorio");

            for (int i = 0; i < amount; i++)
            {
                int inventarioID = AddInventarioID(connection, scadenza);
                string sql = @"insert into medicina(nome, categoria_id, inventario_id, specie_id)
                    values (@nome, @categoria_id, @inventario_id, @specie_id)";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@nome", nomeCibo);
                cmd.Parameters.AddWithValue("@categoria_id", categoriaID);
                cmd.Parameters.AddWithValue("@inventario_id", inventarioID);
                cmd.Parameters.AddWithValue("@specie_id", specieID);
                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("Medicina aggiunta con successo");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private void AggiungiAccessorio(MySqlConnection connection)
    {
        try
        {
            int categoriaID = SelectOrAddCategoriaAccessorioID(connection);

            Console.Write("Inserisci nome accessorio: ");
            string nomeCibo = Console.ReadLine() ?? "Campo obbligatorio";
            nomeCibo = nomeCibo.ToLower().Trim();

            Console.Write("Inserisci taglia: ");
            string taglia = Console.ReadLine() ?? "Campo obbligatorio";
            taglia = taglia.ToLower().Trim();

            int specieID = SelectSpecieID(connection);

            Console.Write("Inserisci quantità da aggiungere: ");
            int amount = int.Parse(Console.ReadLine() ?? "Campo obbligatorio");

            for (int i = 0; i < amount; i++)
            {
                int inventarioID = AddInventarioID(connection, null);
                string sql = @"insert into cibo(nome, taglia, categoria_id, inventario_id, specie_id)
                    values (@nome, @taglia, @categoria_id, @inventario_id, @specie_id)";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@nome", nomeCibo);
                cmd.Parameters.AddWithValue("@taglia", taglia);
                cmd.Parameters.AddWithValue("@categoria_id", categoriaID);
                cmd.Parameters.AddWithValue("@inventario_id", inventarioID);
                cmd.Parameters.AddWithValue("@specie_id", specieID);
                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("Accessorio aggiunto con successo");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public void RimuoviInventario(MySqlConnection connection) // rimuove un elemento da inventario
    {
        Console.WriteLine("\nSeleziona il tipo:");
        Console.WriteLine("[1] Cibo");
        Console.WriteLine("[2] Medicina");
        Console.WriteLine("[3] Accessorio");
        Console.Write("Scelta: ");
        string menuAction = Console.ReadLine() ?? "Campo obbligatorio";

        switch (menuAction)
        {
            case "1":
                RimuoviCibo(connection);
                break;

            case "2":
                RimuoviMedicina(connection);
                break;

            case "3":
                RimuoviAccessorio(connection);
                break;

            default:
                Console.WriteLine("Scelta non valida.");
                break;
        }
        
    }

    private int SelectInventarioID(MySqlConnection connection, string tipo) // seleziona inventarioID
    {
        string sql = $@"select i.inventario_id, t.nome, i.scadenza from inventario i
                    join {tipo} t on i.inventario_id = t.inventario_id;";
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@tipo", tipo);
        MySqlDataReader rdr = cmd.ExecuteReader();
        Console.WriteLine("id -- nome -- scadenza");
        while (rdr.Read())
        {
            Console.WriteLine(rdr[0] + " -- " + rdr[1] + " -- " + rdr[2]);
        }
        rdr.Close();
        Console.Write($"Seleziona ID {tipo}: ");
        int inventarioID = int.Parse(Console.ReadLine() ?? "Campo obbligatorio");
        return inventarioID;
    }


    private void RimuoviCibo(MySqlConnection connection) // rimuove un elemento da inventario e cibo
    {
        int inventarioID = SelectInventarioID(connection, InvType.cibo.ToString());

        string sql = @"delete from cibo where inventario_id = @inventario_id";
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@inventario_id", inventarioID);
        cmd.ExecuteNonQuery();

        sql = @"delete from inventario where inventario_id = @inventario_id";
        cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@inventario_id", inventarioID);
        cmd.ExecuteNonQuery();

        Console.WriteLine("Cibo rimosso con successo");
    }

    private void RimuoviMedicina(MySqlConnection connection)    // rimuove un elemento da inventario e medicina
    {
        int inventarioID = SelectInventarioID(connection, InvType.medicina.ToString());

        string sql = @"delete from medicina where inventario_id = @inventario_id";
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@inventario_id", inventarioID);
        cmd.ExecuteNonQuery();

        sql = @"delete from inventario where inventario_id = @inventario_id";
        cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@inventario_id", inventarioID);
        cmd.ExecuteNonQuery();

        Console.WriteLine("Medicina rimossa con successo");
    }

    private void RimuoviAccessorio(MySqlConnection connection)  // rimuove un elemento da inventario e accessorio
    {
        int inventarioID = SelectInventarioID(connection, InvType.accessorio.ToString());

        string sql = @"delete from accessorio where inventario_id = @inventario_id";
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@inventario_id", inventarioID);
        cmd.ExecuteNonQuery();

        sql = @"delete from inventario where inventario_id = @inventario_id";
        cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@inventario_id", inventarioID);
        cmd.ExecuteNonQuery();

        Console.WriteLine("Accessorio rimosso con successo");
    }

    public void StampaInventario(MySqlConnection connection) // stampa l'inventario del tipo selezionato
    {
        
        Console.WriteLine("\nSeleziona il tipo:");
        Console.WriteLine("[1] Cibo");
        Console.WriteLine("[2] Medicina");
        Console.WriteLine("[3] Accessorio");
        Console.Write("Scelta: ");
        string menuAction = Console.ReadLine() ?? "Campo obbligatorio";

        switch (menuAction)
        {
            case "1":
                StampaCibo(connection);
                break;

            case "2":
                StampaMedicina(connection);
                break;

            case "3":
                StampaAccessorio(connection);
                break;

            default:
                Console.WriteLine("Scelta non valida.");
                break;
        }
        
    }

    private void StampaCibo(MySqlConnection connection) // stampa il cibo con la quantità
    {
        string sql = @"select count(c.inventario_id) as quantita, c.nome, ca.nome, s.specie
                    from cibo c join categoria_cibo ca on c.categoria_id = ca.categoria_id
                    join specie s on c.specie_id = s.specie_id
                    group by c.nome, ca.nome, s.specie;";
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        MySqlDataReader rdr = cmd.ExecuteReader();
        Console.WriteLine("quantità -- nome -- categoria -- specie");
        while (rdr.Read())
        {
            Console.WriteLine(rdr[0] + " -- " + rdr[1] + " -- " + rdr[2] + " -- " + rdr[3]);
        }
        rdr.Close();
    }

    private void StampaMedicina(MySqlConnection connection) // stampa le medicine con la quantità
    {
        string sql = @"select count(m.inventario_id) as quantita, m.nome, ca.nome, s.specie
                    from medicina m join categoria_medicina ca on m.categoria_id = ca.categoria_id
                    join specie s on c.specie_id = s.specie_id
                    group by c.nome, ca.nome, s.specie;";
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        MySqlDataReader rdr = cmd.ExecuteReader();
        Console.WriteLine("quantità -- nome -- categoria -- specie");
        while (rdr.Read())
        {
            Console.WriteLine(rdr[0] + " -- " + rdr[1] + " -- " + rdr[2] + " -- " + rdr[3]);
        }
        rdr.Close();
    }

    private void StampaAccessorio(MySqlConnection connection)   // stampa gli accessori con la quantità
    {
        string sql = @"select count(a.inventario_id) as quantita, a.nome, ca.nome, a.taglia, s.specie
                    from accessorio a join categoria_accessorio ca on a.categoria_id = ca.categoria_id
                    join specie s on c.specie_id = s.specie_id
                    group by c.nome, ca.nome, s.specie;";
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        MySqlDataReader rdr = cmd.ExecuteReader();
        Console.WriteLine("quantità -- nome -- categoria -- taglia -- specie");
        while (rdr.Read())
        {
            Console.WriteLine(rdr[0] + " -- " + rdr[1] + " -- " + rdr[2] + " -- " + rdr[3]+ " -- " + rdr[4]);
        }
        rdr.Close();
    }
}