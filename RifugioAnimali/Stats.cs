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
	public static void StatsMenu()
	{
		bool exit = false;

		while (!exit)
		{
			Console.WriteLine("\nMenù");
			Console.WriteLine("[1] Opzione 1");
			Console.WriteLine("[2] Opzione 2");
			Console.WriteLine("[3] Opzione 3");
			Console.WriteLine("[4] Opzione 4");
			Console.WriteLine("[0] Esci");
			Console.Write("Scelta: ");
			int menuAction = int.Parse(Console.ReadLine() ?? "Campo obbligatorio");

			switch (menuAction)
			{
				case 1:
					// TODO: Opzione 1 logic
					break;

				case 2:
					// TODO: Opzione 2 logic
					break;

				case 3:
					// TODO: Opzione 3 logic
					break;

				case 4:
					// TODO: Opzione 4 logic
					break;

				case 0:
					exit = true;
					break;

				default:
					Console.WriteLine("Scelta non valida.");
					break;
			}
		}
	}

	private static void AdozioniMese(MySqlConnection connection)
	{
		string sql = @"";
        MySqlCommand cmd = new MySqlCommand(sql, connection);
        MySqlDataReader rdr = cmd.ExecuteReader();
        rdr.Read();
        rdr.Close();
	}
}