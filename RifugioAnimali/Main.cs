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