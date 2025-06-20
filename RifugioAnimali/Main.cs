// Quante adozioni sono avvenute nell’ultimo mese/anno?
// SELECT COUNT(adozione_id) from adozione WHERE year(data) = YEAR(CURDATE())GROUP BY MONTH(data) ORDER BY mese;
// SELECT COUNT(adozione_id) from adozione WHERE month(data) = month(CURDATE());

//Tasso di adozione = animali adottati / animali totali

/* SELECT 
    (TotaleAdozioni * 100.0 / TotaleAnimali) AS PercentualeAdozioni
FROM 
    (SELECT COUNT(*) AS TotaleAdozioni FROM adozione) AS A,
    (SELECT COUNT(*) AS TotaleAnimali FROM animale) AS B; */


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