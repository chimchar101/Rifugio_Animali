//yuliya

// Quante adozioni sono avvenute nell’ultimo mese/anno?
// SELECT COUNT(adozione_id) from adozione WHERE year(data) = YEAR(CURDATE())GROUP BY MONTH(data) ORDER BY mese;
// SELECT COUNT(adozione_id) from adozione WHERE month(data) = month(CURDATE());

//Tasso di adozione = animali adottati / animali totali

/* SELECT 
    (TotaleAdozioni * 100.0 / TotaleAnimali) AS PercentualeAdozioni
FROM 
    (SELECT COUNT(*) AS TotaleAdozioni FROM adozione) AS A,
    (SELECT COUNT(*) AS TotaleAnimali FROM animale) AS B; */


//andrea

//Quali membri dello staff gestiscono più adozioni?
/* SELECT u.nome, u.cognome, s.staff_id, COUNT(adozione_id) 
FROM staff s
JOIN utente u ON u.utente_id = s.utente_id
JOIN adozione a ON a.staff_id = s.staff_id 
GROUP BY u.nome, u.cognome, s.staff_id
ORDER BY COUNT(adozione_id) DESC
LIMIT 3*/

/* -- Quanto tempo resta mediamente un animale prima di essere adottato?
select sum(to_days(ad.data) - to_days(ing.data)) / count(distinct ad.adozione_id) as permanenza_media
from adozione ad
join animale an on an.animale_id = ad.animale_id
join ingresso ing on ing.animale_id = an.animale_id
where an.animale_id in(
	select animale_id from adozione
)
; */

/* -- Quanti clienti sono effettivamente attivi (almeno un’adozione)?
select c.cliente_id, u.nome, u.cognome, u.email, count(ad.adozione_id) as numero_adozioni
from utente u
join cliente c on c.utente_id = u.utente_id
join adozione ad on ad.cliente_id = c.cliente_id
group by c.cliente_id
having numero_adozioni > 0; */


//gianluca

/*
1.Quanti animali ho attualmente nel rifugio?

string query = @"SELECT COUNT(*) AS totale_animali FROM animale WHERE adottato = false";




2.Quanti sono stati vaccinati? E quanti no?

	string queryVaccinati = "SELECT COUNT(*) FROM animale WHERE vaccinato = true";
	string queryNonVaccinati = "SELECT COUNT(*) FROM animale WHERE vaccinato = false";




3.Qual è l’età media per specie? (per capire ad esempio se ospiti più cuccioli o adulti)

string query = @"
	SELECT specie, AVG(eta) AS eta_media
	FROM animale
	JOIN specie ON animale.specie_id = specie.specie_id
	GROUP BY specie";



//simone

4.Ci sono animali da molto tempo non ancora adottati?
→ segnalazione per azioni mirate (es. campagne di adozione)

string query = @"
	SELECT animale_id, nome, specie, data_ingresso
	FROM animale
	WHERE adottato = false
		AND data_ingresso <= DATE_SUB(CURDATE(), INTERVAL @sogliaGiorni DAY)
	ORDER BY data_ingresso ASC";
*/

/*
-- Quali specie sono più frequenti nel rifugio?

select s.specie as Nome_Specie, count(*) as Numero
from animale a
join specie s on s.specie_id = a.specie_id
group by s.specie
order by Numero desc;

-- Percentuale di animali vaccinati

SELECT ROUND(SUM(vaccinato) * 100.0 / COUNT(*), 2) AS Percentuale_Vaccinati
FROM animale;
*/

using MySql.Data.MySqlClient;

public class Stats
{
	public static void StatsMenu(MySqlConnection connection)
	{
		bool exit = false;

		while (!exit)
		{
			Console.WriteLine("\nMenu statistiche");
			Console.WriteLine("[1] Staff che gestisce più adozioni");
			Console.WriteLine("[2] Permanenza media animali nel rifugio");
			Console.WriteLine("[3] Clienti attivi (almeno un'adozione)");
			Console.WriteLine("[4] Numero animali nel rifugio");
			Console.WriteLine("[5] Numero animali vaccinati");
			Console.WriteLine("[6] Età media per specie");
			Console.WriteLine("[7] Numero adozioni");
			Console.WriteLine("[8] Tasso di adozione");
			Console.WriteLine("[9] Percentuale vaccinazione");
			Console.WriteLine("[10] Numero animali per specie");
			Console.WriteLine("[11] Animali presenti da almeno un anno");
			Console.WriteLine("[0] Esci");
			Console.Write("Scelta: ");
			string menuAction = Input.String();

			switch (menuAction)
			{
				case "1":
					StaffPiuAdozioni(connection);
					break;

				case "2":
					PermanenzaMediaAnimale(connection);
					break;

				case "3":
					ClientiAttivi(connection);
					break;

				case "4":
					ContaAnimaliNelRifugio(connection);
					break;

				case "5":
					ContaAnimaliVaccinati(connection);
					break;

				case "6":
					EtaMediaPerSpecie(connection);
					break;

				case "7":
					AdozioniCount(connection);
					break;

				case "8":
					TassoAdozione(connection);
					break;

				case "9":
					PercentualeVaccinati(connection);
					break;

				case "10":
					FrequenzaSpecie(connection);
					break;

				case "11":
					AnimaliNelRifugioDaMolto(connection);
					break;

				case "0":
					exit = true;
					break;

				default:
					Console.WriteLine("Scelta non valida.");
					break;
			}
		}
	}

	private static void StaffPiuAdozioni(MySqlConnection connection)
	{
		string sql = @"SELECT u.nome, u.cognome, s.staff_id, COUNT(adozione_id) 
					FROM staff s
					JOIN utente u ON u.utente_id = s.utente_id
					JOIN adozione a ON a.staff_id = s.staff_id 
					GROUP BY u.nome, u.cognome, s.staff_id
					ORDER BY COUNT(adozione_id) DESC
					LIMIT 3";
		MySqlCommand cmd = new MySqlCommand(sql, connection);
		MySqlDataReader rdr = cmd.ExecuteReader();
		Console.WriteLine("--------------------------------------------------");
		Console.WriteLine("MEMBRI DELLO STAFF CHE GESTISCONO PIU' ADOZIONI");
		Console.WriteLine("--------------------------------------------------");
		while (rdr.Read())
		{
			Console.WriteLine($"ID: {rdr[2]}, Nome: {rdr[0]}, Cognome: {rdr[1]}, Adozioni gestite: {rdr[3]}");
		}
		rdr.Close();
	}

	private static void PermanenzaMediaAnimale(MySqlConnection connection)
	{
		string sql = @"select sum(to_days(ad.data) - to_days(ing.data)) / count(distinct ad.adozione_id) as permanenza_media
					from adozione ad
					join animale an on an.animale_id = ad.animale_id
					join ingresso ing on ing.animale_id = an.animale_id
					where an.animale_id in (select animale_id from adozione)";
		MySqlCommand cmd = new MySqlCommand(sql, connection);
		MySqlDataReader rdr = cmd.ExecuteReader();
		Console.WriteLine("--------------------------------------------------");
		Console.WriteLine("PERMANENZA MEDIA DEGLI ANIMALI NEL RIFUGIO");
		Console.WriteLine("--------------------------------------------------");
		while (rdr.Read())
		{
			Console.WriteLine($"Giorni: {rdr[0]}");
		}
		rdr.Close();
	}

	private static void ClientiAttivi(MySqlConnection connection)
	{
		string sql = @"select c.cliente_id, u.nome, u.cognome, u.email, count(ad.adozione_id) as numero_adozioni
					from utente u
					join cliente c on c.utente_id = u.utente_id
					join adozione ad on ad.cliente_id = c.cliente_id
					group by c.cliente_id
					having numero_adozioni > 0
					order by numero_adozioni";
		MySqlCommand cmd = new MySqlCommand(sql, connection);
		MySqlDataReader rdr = cmd.ExecuteReader();
		Console.WriteLine("--------------------------------------------------");
		Console.WriteLine("CLIENTI ATTIVI (ALMENO UN'ADOZIONE)");
		Console.WriteLine("--------------------------------------------------");
		while (rdr.Read())
		{
			Console.WriteLine($"ID: {rdr[0]}, Nome: {rdr[1]}, Cognome: {rdr[2]}, Email: {rdr[3]}, Numero Adozioni: {rdr[4]}");
		}
		rdr.Close();
	}

	private static void ContaAnimaliNelRifugio(MySqlConnection connection)
	{
		string query = @"SELECT COUNT(*) AS totale_animali
                    FROM animale 
                    WHERE adottato = false";

		using (MySqlCommand cmd = new MySqlCommand(query, connection))
		{
			//Se result è null o DBNull, assegna 0, altrimenti converte in int
			MySqlDataReader rdr = cmd.ExecuteReader();
			rdr.Read();
			int totale = Convert.ToInt32(rdr[0]);
			rdr.Close();

			//Stampa il numero totale di animali nel rifugio (non ancora adottati).
			Console.WriteLine("----------------------------------------------------");
			Console.WriteLine($"NUMERO ANIMALI ATTUALMENTE NEL RIFUGIO");
			Console.WriteLine("----------------------------------------------------");
			Console.WriteLine($"Animali: {totale}");
		}
	}

	private static void ContaAnimaliVaccinati(MySqlConnection connection)
	{
		//Query SQL per contare gli animali con campo vaccinato impostato a true (vaccinati).
		string queryVaccinati = "SELECT COUNT(*) FROM animale WHERE vaccinato = true";

		//Query SQL per contare gli animali con campo vaccinato impostato a false (non vaccinati).
		string queryNonVaccinati = "SELECT COUNT(*) FROM animale WHERE vaccinato = false";

		//Esegue la query per gli animali vaccinati, ottiene il risultato come oggetto e lo converte in intero.
		int vaccinati = Convert.ToInt32(new MySqlCommand(queryVaccinati, connection).ExecuteScalar());

		//Esegue la query per gli animali non vaccinati e converte il risultato in intero.
		int nonVaccinati = Convert.ToInt32(new MySqlCommand(queryNonVaccinati, connection).ExecuteScalar());

		//Stampa a console i risultati ottenuti, con una linea decorativa sopra e sotto.
		Console.WriteLine("--------------------------------------------------");
		Console.WriteLine($"NUMERO ANIMALI PER STATO DI VACCINAZIONE");
		Console.WriteLine("--------------------------------------------------");
		Console.WriteLine($"Animali vaccinati {vaccinati}");
		Console.WriteLine($"Animali non vaccinati {nonVaccinati}");
	}

	private static void EtaMediaPerSpecie(MySqlConnection connection)
	{
		//Query SQL per ottenere la specie e l'età media degli animali associati a quella specie.
		string query = @"
        SELECT specie, AVG(eta) AS eta_media
        FROM animale
        JOIN specie ON animale.specie_id = specie.specie_id
        GROUP BY specie";

		//Si crea ed esegue il comando SQL usando la connessione fornita.
		using (MySqlCommand cmd = new MySqlCommand(query, connection))
		{
			//Si apre un lettore dati per leggere i risultati riga per riga.
			using (MySqlDataReader reader = cmd.ExecuteReader())
			{
				Console.WriteLine("--------------------------------------------------");
				Console.WriteLine("ETA' MEDIA DEGLI ANIMALI PER SPECIE");
				Console.WriteLine("--------------------------------------------------");

				//Ciclo che legge ogni riga del risultato della query.
				while (reader.Read())
				{
					//Legge il valore della colonna specie come stringa.
					string specie = (string)reader["specie"];

					//Legge e converte la colonna eta_media in un double (media dell'età).
					double etaMedia = Convert.ToDouble(reader["eta_media"]);

					//Stampa la specie con la relativa età media, formattata con 2 decimali.
					Console.WriteLine($"{specie} {etaMedia} anni");
				}
			}
		}
	}

	private static void AdozioniCount(MySqlConnection connection)
	{
		string query1 = @"
        SELECT MONTH(data) AS mese, COUNT(adozione_id) AS numero_adozioni
        FROM adozione 
        WHERE YEAR(data) = YEAR(CURDATE())
        GROUP BY MONTH(data) 
        ORDER BY mese;";

		using (MySqlCommand cmd = new MySqlCommand(query1, connection))
		using (MySqlDataReader reader = cmd.ExecuteReader())
		{
			Console.WriteLine("--------------------------------------------------");
			Console.WriteLine("NUMERO ADOZIONI ANNO IN CORSO");
			Console.WriteLine("--------------------------------------------------");
			if (reader.HasRows)
			{
				while (reader.Read())
				{
					int mese = Convert.ToInt32(reader["mese"]);
					int numeroAdozioni = Convert.ToInt32(reader["numero_adozioni"]);
					Console.WriteLine($"Mese: {mese}, Adozioni: {numeroAdozioni}");
				}
			}
			else
			{
				Console.WriteLine("Nessuna adozione registrata quest'anno.");
			}
		}

		string query2 = @"
        SELECT COUNT(adozione_id) AS numero_adozioni
        FROM adozione 
        WHERE MONTH(data) = MONTH(CURDATE())";

		using (MySqlCommand cmd2 = new MySqlCommand(query2, connection))
		using (MySqlDataReader reader2 = cmd2.ExecuteReader())
		{
			Console.WriteLine("--------------------------------------------------");
			Console.WriteLine("NUMERO ADOZIONI MESE IN CORSO");
			Console.WriteLine("--------------------------------------------------");
			if (reader2.HasRows)
			{
				while (reader2.Read())
				{
					int numeroAdozioni = Convert.ToInt32(reader2["numero_adozioni"]);
					Console.WriteLine($"Adozioni: {numeroAdozioni}");
				}
			}
			else
			{
				Console.WriteLine("Nessuna adozione registrata questo mese.");
			}
		}
	}

	private static void TassoAdozione(MySqlConnection connection)
	{
		string query = @"SELECT (TotaleAdozioni * 100.0 / TotaleAnimali) AS PercentualeAdozioni
                    FROM (SELECT COUNT(*) AS TotaleAdozioni FROM adozione) AS A,
                    (SELECT COUNT(*) AS TotaleAnimali FROM animale) AS B";
		using (MySqlCommand cmd = new MySqlCommand(query, connection))
		using (MySqlDataReader reader = cmd.ExecuteReader())
		{

			Console.WriteLine("--------------------------------------------------");
			Console.WriteLine("PERCENTUALE ANIMALI ADOTTATI");
			Console.WriteLine("--------------------------------------------------");
			if (reader.HasRows)
			{
				while (reader.Read())
				{

					int tassoAdozione = Convert.ToInt32(reader["PercentualeAdozioni"]);
					Console.WriteLine($"{tassoAdozione} %");
				}
			}
		}
	}

	private static void PercentualeVaccinati(MySqlConnection connection)
	{
		string query = @"SELECT ROUND(SUM(vaccinato) * 100.0 / COUNT(*), 2) AS Percentuale_Vaccinati
                        FROM animale";
		MySqlCommand cmd = new MySqlCommand(query, connection);
		try
		{
			MySqlDataReader reader = cmd.ExecuteReader();
			Console.WriteLine("--------------------------------------------------");
			Console.WriteLine("PERCENTUALE ANIMALI VACCINATI");
			Console.WriteLine("--------------------------------------------------");
			while (reader.Read())
			{
				Console.WriteLine($"{reader["Percentuale_Vaccinati"]}%");
			}
			reader.Close();
		}
		catch (Exception ex)
		{
			Console.WriteLine("Errore durante la stampa della percentuale: " + ex.Message);
		}
	}

	private static void FrequenzaSpecie(MySqlConnection connection)
	{
		string query = @"select s.specie as Nome_Specie, count(*) as Numero
                        from animale a
                        join specie s on s.specie_id = a.specie_id
                        group by s.specie
                        order by Numero desc";
		MySqlCommand cmd = new MySqlCommand(query, connection);
		try
		{
			MySqlDataReader reader = cmd.ExecuteReader();
			Console.WriteLine("--------------------------------------------------");
			Console.WriteLine("FREQUENZA DELLE SPECIE NEL RIFUGIO");
			Console.WriteLine("--------------------------------------------------");
			while (reader.Read())
			{
				Console.WriteLine($"Specie: {reader["Nome_Specie"]}, Frequenza: {reader["Numero"]}");
			}
			reader.Close();
		}
		catch (Exception ex)
		{
			Console.WriteLine("Errore durante la stampa della frequenza: " + ex.Message);
		}
	}

	private static void AnimaliNelRifugioDaMolto(MySqlConnection connection)
    {
        string query = @"select a.nome as Nome, s.specie as Specie, a.eta as Eta, a.vaccinato as Vaccinato, i.data as Data_Ingresso
        from animale a
        join specie s on s.specie_id = a.specie_id
        join ingresso i on i.animale_id = a.animale_id
        where adottato = false
        and i.data <= date_sub(curdate(), interval 365 day)
        order by Data_Ingresso asc";
        MySqlCommand cmd = new MySqlCommand(query, connection);
		try
		{
			MySqlDataReader reader = cmd.ExecuteReader();
			Console.WriteLine("--------------------------------------------------");
			Console.WriteLine("ANIMALI PRESENTI NEL RIFUGIO DA PIÙ DI UN ANNO");
			Console.WriteLine("--------------------------------------------------");
			while (reader.Read())
			{
				Console.WriteLine($"Nome: {reader["Nome"]}, Specie: {reader["Specie"]}, Età: {reader["Eta"]}, Vaccinato: {reader["Vaccinato"]}, Data ingresso: {reader["Data_Ingresso"]}");
			}
			reader.Close();
        }
		catch (Exception ex)
		{
			Console.WriteLine("Errore durante la stampa degli animali: " + ex.Message);
		}
    }

}